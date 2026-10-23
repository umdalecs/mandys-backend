# Seed scripts

Seeds the catalog through the running API (the admin and demo customer users are seeded by the EF migration).

```bash
pip install -r seeds/requirements.txt
python seeds/seed.py            # all steps, idempotent
python seeds/seed.py --only products dishes
```

Order: branches -> users (staff) -> products -> dishes -> combos. Existing rows (matched by name / description / email) are skipped.

| Env var | Default |
|---|---|
| `SEED_BASE_URL` | `http://localhost:8080` |
| `SEED_ADMIN_EMAIL` | `admin@mandyspos.com` |
| `SEED_ADMIN_PASSWORD` | `mandyspos` |
| `SEED_USER_PASSWORD` | `mandyspos` (staff users) |

Data lives in `seeds/data/*.json`.
