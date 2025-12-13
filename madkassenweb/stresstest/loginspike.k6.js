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
    stages: [
        { duration: "30s", target: 20 },
        { duration: "5s",  target: 300 },
        { duration: "20s", target: 300 },
        { duration: "5s",  target: 20 },
        { duration: "30s", target: 0 },
    ],
    thresholds: {
        http_req_failed: ["rate<0.02"],
        http_req_duration: ["p(95)<800"],
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
            tags: { name: "POST /api/Auth (spike)" },
        }
    );

    check(res, {
        "status 200": (r) => r.status === 200,
    });

    sleep(1);
}
