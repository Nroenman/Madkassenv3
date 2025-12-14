/* eslint-disable */
import http from "k6/http";
import { check, sleep } from "k6";

export const options = {
    vus: 50,
    duration: "10m",
    thresholds: {
        http_req_failed: ["rate<0.01"],
        http_req_duration: ["p(95)<500"],
    },
};

export default function () {
    const res = http.get("http://localhost:5092/api/Product", {
        tags: { name: "GET /api/Product (load)" },
    });

    check(res, {
        "status is 200": (r) => r.status === 200,
        "returns JSON": (r) =>
            (r.headers["Content-Type"] || "").includes("application/json"),
        "has products": (r) => {
            try {
                const data = r.json();
                return Array.isArray(data) && data.length > 0;
            } catch {
                return false;
            }
        },
    });

    sleep(Math.random() * 2 + 0.5);
}
