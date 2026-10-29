API_PROJECT := Mandys.Api
API_LAUNCH_PROFILE := http

EF_ARGS := --project Mandys.Infrastructure --startup-project Mandys.Api
MIGRATIONS_DIR := Mandys.Infrastructure/Migrations

.PHONY: run-api run-database stop-database run-storage stop-storage migrations-fresh migrations-clean migrations-init

run: run-database run-storage run-api

stop: stop-database stop-storage

run-api:
	dotnet run --project $(API_PROJECT) --launch-profile $(API_LAUNCH_PROFILE)

restart-database: stop-database run-database

run-database:
	docker compose -f docker/compose.yml up database pgadmin -d

stop-database:
	docker compose -f docker/compose.yml down database pgadmin -v

run-storage:
	docker compose -f docker/compose.yml up storage storage-init -d

stop-storage:
	docker compose -f docker/compose.yml down storage storage-init -v

fresh-migrations: clean-migrations init-migrations

clean-migrations:
	dotnet ef database update 0 $(EF_ARGS)
	rm -f $(MIGRATIONS_DIR)/*.cs

init-migrations:
	dotnet ef migrations add Initial $(EF_ARGS)
	dotnet ef database update $(EF_ARGS)
