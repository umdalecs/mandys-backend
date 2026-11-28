API_PROJECT := Mandys.Api
API_LAUNCH_PROFILE := http
SEED_PROJECT := Mandys.Seeding
BASE_URL ?= http://localhost:8080

EF_ARGS := --project Mandys.Infrastructure --startup-project Mandys.Api
MIGRATIONS_DIR := Mandys.Infrastructure/Migrations

.PHONY: run-api seed run-database stop-database run-storage stop-storage migrations-fresh migrations-clean migrations-init

run: run-dependencies
	dotnet run --project $(API_PROJECT) --launch-profile $(API_LAUNCH_PROFILE)

run-dependencies:
	docker compose up database storage storage-init -d

stop-dependencies:
	docker compose down database storage storage-init

seed:
	dotnet run --project $(SEED_PROJECT) -- $(BASE_URL)

restart-database: stop-database run-database

run-pgadmin:
	docker compose up pgadmin -d

stop-pgadmin:
	docker compose down pgadmin -v
