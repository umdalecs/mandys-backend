API_PROJECT := Mandys.Api
API_LAUNCH_PROFILE := http

EF_ARGS := --project Mandys.Infrastructure --startup-project Mandys.Api
MIGRATIONS_DIR := Mandys.Infrastructure/Migrations

.PHONY: run-api run-database migrations-fresh migrations-clean migrations-init

run-api:
	dotnet run --project $(API_PROJECT) --launch-profile $(API_LAUNCH_PROFILE)

restart-database: stop-database run-database

run-database:
	docker compose -f docker/compose.yml up database -d

stop-database:
	docker compose -f docker/compose.yml down database -v

fresh-migrations: migrations-clean migrations-init

clean-migrations:
	dotnet ef database update 0 $(EF_ARGS)
	rm -f $(MIGRATIONS_DIR)/*.cs

init-migrations:
	dotnet ef migrations add Initial $(EF_ARGS)
	dotnet ef database update $(EF_ARGS)
