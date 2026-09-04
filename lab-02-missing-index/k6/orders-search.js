import http from "k6/http";
import { check, sleep } from "k6";

export const options = {
  vus: 10,
  duration: "30s",
};

export default function () {
  const customerId = ((__VU - 1 + __ITER * 10) % 10000) + 1;
  const response = http.get(
    `http://localhost:5248/api/orders/search?customerId=${customerId}`,
    { tags: { name: "GET /api/orders/search" } },
  );

  check(response, {
    "status is 200": (r) => r.status === 200,
  });

  sleep(1);
}
