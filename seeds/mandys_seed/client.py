"""Thin HTTP client for the Mandys API (bearer auth)."""
import sys

import requests


class ApiError(RuntimeError):
    pass


class ApiClient:
    def __init__(self, base_url: str):
        self.base_url = base_url.rstrip("/")
        self.session = requests.Session()

    def login(self, email: str, password: str) -> None:
        resp = self.session.post(
            f"{self.base_url}/login",
            params={"useCookies": "false"},
            json={"email": email, "password": password},
        )
        if not resp.ok:
            raise ApiError(f"Login failed ({resp.status_code}): {resp.text}")
        token = resp.json()["accessToken"]
        self.session.headers["Authorization"] = f"Bearer {token}"

    def request(self, method: str, path: str, **kwargs):
        resp = self.session.request(method, f"{self.base_url}{path}", **kwargs)
        if not resp.ok:
            raise ApiError(f"{method} {path} -> {resp.status_code}: {resp.text}")
        return resp.json() if resp.content else None

    def post(self, path: str, body: dict):
        return self.request("POST", path, json=body)

    def list_all(self, path: str, **params) -> list[dict]:
        """Walk every page of a paged list endpoint."""
        items: list[dict] = []
        page = 1
        while True:
            data = self.request("GET", path, params={**params, "page": page, "pageSize": 100})
            items.extend(data["items"])
            if page >= data["totalPages"]:
                return items
            page += 1


def log(msg: str) -> None:
    print(msg, file=sys.stderr if msg.startswith("ERROR") else sys.stdout)
