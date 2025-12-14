/* eslint-disable */
import http from "k6/http";
import { check, sleep } from "k6";

export const options = {
    stages: [
        { duration: "30s", target: 20 },
        { duration: "5s",  target: 600 },
        { duration: "20s", target: 600 },
        { duration: "5s",  target: 20 },
        { duration: "30s", target: 0 },
    ],
    thresholds: {
        http_req_failed: ["rate<0.02"],
        http_req_duration: ["p(95)<500"],
    },
};

export default function () {
    const res = http.get("http://localhost:5092/api/Product", {
        tags: { name: "GET /api/Product (spike)" },
    });

    check(res, {
        "status is 200": (r) => r.status === 200,
        "returns JSON": (r) =>
            (r.headers["Content-Type"] || "").includes("application/json"),
        "has products": (r) => {
            try {
                return r.json().length > 0;
            } catch {
                return false;
            }
        },
    });

    sleep(Math.random() + 0.2);
}
