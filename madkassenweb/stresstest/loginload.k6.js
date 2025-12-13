/* eslint-disable no-restricted-globals */
/* eslint-disable no-undef */
import http from "k6/http";
import { check, sleep } from "k6";
import { SharedArray } from "k6/data";

const users = new SharedArray("users", () => {
    const raw = open("./users.json").replace(/^\uFEFF/, "");
    return JSON.parse(raw);
});

function pickUser(vu) {
    return users[(vu - 1) % users.length];
}

export const options = {
    vus: 50,
    duration: "10m",
    thresholds: {
        http_req_failed: ["rate<0.01"],
        http_req_duration: ["p(95)<800"],
        checks: ["rate>0.98"],
    },
};

export default function () {
    const baseUrl = "http://localhost:5092";
    const user = pickUser(__VU);

    const res = http.post(
        `${baseUrl}/api/Auth`,
        JSON.stringify({ email: user.email, password: user.password }),
        {
            headers: {
                "Content-Type": "application/json",
                Accept: "application/json",
            },
            tags: { name: "POST /api/Auth (load)" },
        }
    );

    check(res, {
        "status 200": (r) => r.status === 200,
    });

    sleep(1);
}
