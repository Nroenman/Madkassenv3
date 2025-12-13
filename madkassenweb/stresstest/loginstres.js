/* eslint-disable no-restricted-globals */
/* eslint-disable no-undef */

import http from "k6/http";
import { check, sleep } from "k6";
import { SharedArray } from "k6/data";

const users = new SharedArray("users", function () {
  const raw = open("./users.json");
  const cleaned = raw.replace(/^\uFEFF/, ""); 
  const parsed = JSON.parse(cleaned);
  console.log(`Loaded users: ${parsed.length}`);
  return parsed;
});


function pickUser(vu) {
  return users[(vu - 1) % users.length];
}

// Stress  configuration
export const options = {
  stages: [
    { duration: "30s", target: 20 },
    { duration: "1m",  target: 100 },
    { duration: "2m",  target: 300 },
    { duration: "2m",  target: 600 },
    { duration: "2m",  target: 1000 },
    { duration: "1m",  target: 0 },
  ],


  thresholds: {
    http_req_failed: ["rate<0.02"],
    http_req_duration: ["p(95)<800"],
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
