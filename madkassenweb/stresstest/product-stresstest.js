/* eslint-disable */
import http from "k6/http";
import { check, sleep } from "k6";

export const options = {
    stages: [
        { duration: "30s", target: 20 },
        { duration: "1m",  target: 100 },
        { duration: "2m",  target: 300 },
        { duration: "2m",  target: 600 },
        { duration: "1m",  target: 0 },
    ],
    thresholds: {
        http_req_failed: ["rate<0.01"],
        http_req_duration: ["p(95)<500"],
    },
};

export default function () {
    const res = http.get("http://localhost:5092/api/Product", {
        tags: { name: "GET /api/Product" },
    });

    check(res, {
        "status is 200": (r) => r.status === 200,
        "returns JSON": (r) =>
            (r.headers["Content-Type"] || "").includes("application/json"),
        "has products": (r) => r.json().length > 0,
    });

    // simulate user browsing time
    sleep(Math.random() * 2 + 0.5);
}
