/* eslint-disable no-restricted-globals */
/* eslint-disable no-undef */

import http from "k6/http";
import { check, sleep } from "k6";
import { SharedArray } from "k6/data";

// Load test users once and share between VUs
const users = new SharedArray("users", function () {
  return JSON.parse(open("./users.json"));
});

// Deterministic user selection per VU
function pickUser(vu) {
  return users[(vu - 1) % users.length];
}

// Stress test configuration
export const options = {
  stages: [
    { duration: "30s", target: 20 },    // warm-up
    { duration: "1m",  target: 100 },   // early load
    { duration: "2m",  target: 300 },   // stress
    { duration: "2m",  target: 600 },   // heavy stress
    { duration: "2m",  target: 1000 },  // extreme stress
    { duration: "1m",  target: 0 },     // cool down
  ],


  thresholds: {
    http_req_failed: ["rate<0.02"],      // < 2% errors
    http_req_duration: ["p(95)<800"],    // 95% under 800 ms
    checks: ["rate>0.98"],
  },
};

export default function () {
  const baseUrl = "http://localhost:5092";
  const loginPath = "/api/Auth";

  // Always use users.json (no fallback)
  const user = pickUser(__VU);

  const payload = JSON.stringify({
    email: user.email,
    password: user.password,
  });

  const params = {
    headers: {
      "Content-Type": "application/json",
      "Accept": "application/json",
    },
    tags: { name: "POST /api/Auth" },
  };

  const res = http.post(`${baseUrl}${loginPath}`, payload, params);

  check(res, {
    "status is 200": (r) => r.status === 200,
    "response is JSON": (r) =>
        (r.headers["Content-Type"] || "").includes("application/json"),
  });

  sleep(1);
}
