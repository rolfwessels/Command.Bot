.DEFAULT_GOAL := help

# General Variables
test-dir := $(wildcard src/*.Tests/.)
version-prefix := 0.0.1.$(shell git rev-list HEAD --count)

# Targets
help:
	@echo "Command Bot"
	@echo "---------------------------------------------------------------------------------------------"
	@echo "Targets:"
	@echo "  Docker Targets (run from local machine)"
	@echo "   - up                : launch and execute container"
	@echo "   - down              : stops the container(s)"
	@echo "   - rebuild           : rebuild containers(s)"
	@echo "  "
	@echo "  Docker Targets (run in container)"
	@echo "   - build              : build the app"

up: 
	@echo "✔️ Starting containers..."
	docker-compose up -d
	@echo "✔️ Attaching shell..."
	docker-compose exec dev bash

rebuild: down 
	@echo "✔️ Building containers..."
	@docker-compose build

down:
	@echo "✔️ Stopping containers..."
	docker-compose down

version: 
	@echo "✔️ Setting version prefix $(version-prefix)"
	find src/*/*.csproj -type f -exec sed -i 's/<FileVersion>.*<\/FileVersion>/<FileVersion>$(version-prefix)<\/FileVersion>/g' {} +
	find src/*/*.csproj -type f -exec sed -i 's/<AssemblyVersion>.*<\/AssemblyVersion>/<AssemblyVersion>$(version-prefix)<\/AssemblyVersion>/g' {} +
