#   make migrations-fresh   # delete all migrations + create + apply (one flow)
#   make migrations-clean   # revert database to zero + delete migration files
#   make migrations-init    # scaffold a single Initial migration + apply it

EF_ARGS := --project Mandys.Infrastructure --startup-project Mandys.Api
MIGRATIONS_DIR := Mandys.Infrastructure/Migrations

.PHONY: migrations-fresh migrations-clean migrations-init

migrations-fresh: migrations-clean migrations-init

# Reverts all applied migrations on the database, then deletes every
# scaffolded migration (the directory itself is kept for the csproj).
migrations-clean:
	dotnet ef database update 0 $(EF_ARGS)
	rm -f $(MIGRATIONS_DIR)/*.cs

# Scaffolds one Initial migration from the current model and applies it.
migrations-init:
	dotnet ef migrations add Initial $(EF_ARGS)
	dotnet ef database update $(EF_ARGS)
