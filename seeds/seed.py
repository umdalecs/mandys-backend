#!/usr/bin/env python3
"""Seed catalog data (branches, staff users, products, dishes, combos) via the Mandys HTTP API."""
import argparse
import os
import sys

from mandys_seed import seeders
from mandys_seed.client import ApiClient, ApiError

STEPS = ["branches", "users", "products", "dishes", "combos"]


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--base-url", default=os.getenv("SEED_BASE_URL", "http://localhost:8080"))
    parser.add_argument("--email", default=os.getenv("SEED_ADMIN_EMAIL", "admin@mandyspos.com"))
    parser.add_argument("--password", default=os.getenv("SEED_ADMIN_PASSWORD", "mandyspos"))
    parser.add_argument("--user-password", default=os.getenv("SEED_USER_PASSWORD", "mandyspos"),
                        help="password given to seeded staff users")
    parser.add_argument("--only", nargs="+", choices=STEPS,
                        help="run only these steps (dependencies are looked up from the API when needed)")
    args = parser.parse_args()

    steps = args.only or STEPS
    api = ApiClient(args.base_url)
    ctx: dict = {}
    try:
        api.login(args.email, args.password)
        # Lookups needed by later steps even when earlier ones are not selected.
        if "users" in steps and "branches" not in steps:
            ctx["branches"] = {b["name"]: b["id"] for b in api.list_all("/branches")}
        if ("dishes" in steps or "combos" in steps) and "products" not in steps:
            ctx["products"] = seeders._id_map(
                "products", "description", {p["description"]: p["id"] for p in api.list_all("/products")})
        if "combos" in steps and "dishes" not in steps:
            ctx["dishes"] = seeders._id_map(
                "dishes", "name", {d["name"]: d["id"] for d in api.list_all("/dishes")})

        if "branches" in steps:
            seeders.seed_branches(api, ctx)
        if "users" in steps:
            seeders.seed_users(api, ctx, args.user_password)
        if "products" in steps:
            seeders.seed_products(api, ctx)
        if "dishes" in steps:
            seeders.seed_dishes(api, ctx)
        if "combos" in steps:
            seeders.seed_combos(api, ctx)
    except ApiError as ex:
        print(f"ERROR: {ex}", file=sys.stderr)
        return 1
    return 0


if __name__ == "__main__":
    sys.exit(main())
