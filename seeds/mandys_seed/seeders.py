"""One idempotent seeder per entity, run in dependency order.

Each seeder looks up existing rows by natural key (name / description /
email) and only creates the missing ones. IDs are resolved at runtime because
the API assigns them.
"""
from .client import ApiClient, log
from .loaders import load


def _summary(label: str, created: int, skipped: int) -> None:
    log(f"{label}: {created} created, {skipped} skipped")


def _id_map(data_name: str, key: str, live_ids: dict) -> dict:
    """Map the seed ids used in the data files to the ids the API assigned."""
    return {row["id"]: live_ids[row[key]] for row in load(data_name)}


def seed_branches(api: ApiClient, ctx: dict) -> None:
    existing = {b["name"]: b["id"] for b in api.list_all("/branches")}
    created = skipped = 0
    for row in load("branches"):
        if row["name"] in existing:
            skipped += 1
            continue
        existing[row["name"]] = api.post("/branches", row)["id"]
        created += 1
    ctx["branches"] = existing
    _summary("branches", created, skipped)


def seed_users(api: ApiClient, ctx: dict, password: str) -> None:
    existing = {u["email"].lower() for u in api.list_all("/users")}
    created = skipped = 0
    for row in load("users"):
        if row["email"].lower() in existing:
            skipped += 1
            continue
        body = {k: v for k, v in row.items() if k != "branch"}
        body["password"] = password
        if row.get("branch"):
            body["branchId"] = ctx["branches"][row["branch"]]
        api.post("/users", body)
        created += 1
    _summary("users", created, skipped)


def seed_products(api: ApiClient, ctx: dict) -> None:
    existing = {p["description"]: p["id"] for p in api.list_all("/products")}
    created = skipped = 0
    for row in load("products"):
        if row["description"] in existing:
            skipped += 1
            continue
        body = {k: v for k, v in row.items() if k != "id"}
        existing[row["description"]] = api.post("/products", body)["id"]
        created += 1
    ctx["products"] = _id_map("products", "description", existing)
    _summary("products", created, skipped)


def seed_dishes(api: ApiClient, ctx: dict) -> None:
    existing = {d["name"]: d["id"] for d in api.list_all("/dishes")}
    created = skipped = 0
    for row in load("dishes"):
        if row["name"] in existing:
            skipped += 1
            continue
        body = {
            "name": row["name"],
            "price": row["price"],
            "recipe": [
                {"productId": ctx["products"][l["productId"]], "quantity": l["quantity"]}
                for l in row["recipe"]
            ],
        }
        existing[row["name"]] = api.post("/dishes", body)["id"]
        created += 1
    ctx["dishes"] = _id_map("dishes", "name", existing)
    _summary("dishes", created, skipped)


def seed_combos(api: ApiClient, ctx: dict) -> None:
    existing = {c["name"] for c in api.list_all("/combos")}
    created = skipped = 0
    for row in load("combos"):
        if row["name"] in existing:
            skipped += 1
            continue
        body = {
            "name": row["name"],
            "price": row["price"],
            "dishes": [
                {"dishId": ctx["dishes"][l["dishId"]], "quantity": l["quantity"]}
                for l in row["dishes"]
            ],
            "products": [
                {"productId": ctx["products"][l["productId"]], "quantity": l["quantity"]}
                for l in row["products"]
            ],
        }
        api.post("/combos", body)
        created += 1
    _summary("combos", created, skipped)
