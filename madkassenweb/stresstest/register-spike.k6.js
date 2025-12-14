/* eslint-disable */

import http from "k6/http";
import { check, sleep } from "k6";

const BASE_URL = __ENV.BASE_URL || "http://localhost:5092";
const URL = `${BASE_URL}/api/Users`;

export const options = {
    stages: [
        { duration: "10s", target: 5 },     // baseline
        { duration: "10s", target: 150 },   // SPIKE up fast
        { duration: "30s", target: 150 },   // hold spike
        { duration: "10s", target: 5 },     // drop back down
        { duration: "20s", target: 0 },     // finish
    ],
    thresholds: {
        http_req_failed: ["rate<0.05"],         // allow higher error under spike
        http_req_duration: ["p(95)<1500"],      // p95 can be higher under spike
    },
};

export default function () {
    const email = `spike_${__VU}_${__ITER}_${Date.now()}@test.dk`;

    const payload = JSON.stringify({
        email,
        passwordHash: "Test123!",
        userName: "Spike Test User",
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
