/* eslint-disable */

import http from "k6/http";
import { check, sleep } from "k6";

const BASE_URL = __ENV.BASE_URL || "http://localhost:5092";
const URL = `${BASE_URL}/api/Users`;

export const options = {
    stages: [
        { duration: "30s", target: 10 },
        { duration: "2m", target: 10 },
        { duration: "30s", target: 25 },
        { duration: "2m", target: 25 },
        { duration: "30s", target: 0 },
    ],
    thresholds: {
        http_req_failed: ["rate<0.01"],
        http_req_duration: ["p(95)<800"],
    },
};

export default function () {
    // unique per VU + iteration so DB unique email constraint won't break the test
    const email = `load_${__VU}_${__ITER}_${Date.now()}@test.dk`;

    const payload = JSON.stringify({
        email,
        passwordHash: "Test123!",
        userName: "Load Test User",
        roles: "Consumer",
    });

    const params = {
        headers: {
            "Content-Type": "application/json",
        },
    };

    const res = http.post(URL, payload, params);

    check(res, {
        "register status 200/201": (r) => r.status === 200 || r.status === 201,
    });

    sleep(1);
}
