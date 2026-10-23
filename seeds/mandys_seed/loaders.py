import json
from pathlib import Path

DATA_DIR = Path(__file__).resolve().parent.parent / "data"


def load(name: str) -> list[dict]:
    with open(DATA_DIR / f"{name}.json", encoding="utf-8") as fh:
        return json.load(fh)
