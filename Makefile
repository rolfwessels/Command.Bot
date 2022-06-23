.DEFAULT_GOAL := help

# General Variables
date=$(shell date +'%y.%m.%d.%H.%M')
project := Command.Bot
container := dev
docker-filecheck := /.dockerenv
docker-warning := ""
RED=\033[0;31m
GREEN=\033[0;32m
NC=\033[0m # No Color
version := 1.1.$(shell git rev-list HEAD --count)
main-branch := master
dockerhub := rolfwessels/command-bot

ifdef GITHUB_BASE_REF
	current-branch :=  $(patsubst refs/heads/%,%,${GITHUB_HEAD_REF})
else ifdef GITHUB_REF
	current-branch :=  $(patsubst refs/heads/%,%,${GITHUB_REF})
else 
	current-branch :=  $(shell git rev-parse --abbrev-ref HEAD)
endif

release := 'development'
ifeq ($(env), prod)
	release := 'production'
endif


ifeq ($(current-branch), $(main-branch))
  docker-tags := -t $(dockerhub):latest -t $(dockerhub):v$(version)
else ifeq ($(current-branch), develop)
  docker-tags := -t $(dockerhub):beta 
else
  docker-tags := -t $(dockerhub):alpha 
endif

ifeq ($(current-branch), $(main-branch))
  version-tag :=  $(version)
else ifeq ($(current-branch), develop)
  version := $(versionPrefix).$(shell git rev-list origin/master --count).$(shell git rev-list origin/master..HEAD --count)
  version-tag := $(version)-beta
else
  version := $(versionPrefix).$(shell git rev-list origin/master --count).$(shell git rev-list origin/master..HEAD --count)
  version-tag := $(version)-alpha
endif

# Docker Warning
ifeq ("$(wildcard $(docker-filecheck))","")
	docker-warning = "⚠️  WARNING: Can't find /.dockerenv - it's strongly recommended that you run this from within the docker container."
endif

# Targets
help:
	@echo "The following commands can be used for building & running & deploying the the $(project) container"
	@echo "---------------------------------------------------------------------------------------------"
	@echo "Targets:"
	@echo "  Docker Targets (run from local machine)"
	@echo "   - up          : brings up the container & attach to the default container ($(container))"
	@echo "   - down        : stops the container"
	@echo "   - build       : (re)builds the container"
	@echo ""
	@echo "  Service Targets (should only be run inside the docker container)"
	@echo "   - version         : Set current version number $(project)"
	@echo "   - start           : Run the $(project)"
	@echo "   - test            : Run the $(project) tests"
	@echo "   - publish         : Build the $(project) and publish to docker $(docker-tags)"
	@echo "   - publish-nuget   : Publish the Command.Bot.Core and to nuget under version $(version-tag)"
	@echo "   - publish-zip     : Build and compile to dist/Command.Bot.$(version-tag).zip"
	@echo "   - deploy          : Deploy the $(project)"

	@echo ""
	@echo "Options:"
	@echo " - env    : sets the environment - supported environments are: dev | prod"
	@echo ""
	@echo "Examples:"
	@echo " - Start Docker Container              : make up"
	@echo " - Rebuild Docker Container            : make build"
	@echo " - Rebuild & Start Docker Container    : make build up"
	@echo " - Publish                             : make publish"

up:
	@echo "Starting containers..."
	@docker-compose up -d
	@echo "Attachig shell..."
	@docker-compose exec $(container) bash

down:
	@echo "Stopping containers..."
	@docker-compose down

build: down
	@echo "Stopping containers..."
	@docker-compose down
	@echo "Building containers..."
	@docker-compose build

publish: 
	@echo -e "${GREEN}Publish branch $(current-branch) to $(docker-tags) to version $(version) as user ${DOCKER_USER}${NC}"
	@docker login -u ${DOCKER_USER} -p ${DOCKER_PASSWORD}
	@echo -e "${GREEN}Building $(docker-tags)${NC}"
	@cd src && docker build ${docker-tags} --build-arg VERSION=$(version) .
	@echo -e "${GREEN}Pusing to $(docker-tags)${NC}"
	@docker push --all-tags $(dockerhub)

restore: 
	@echo -e "${GREEN}Restore $(project) nuget packages${NC}"
	@cd src && dotnet restore

test: restore
	@echo -e "${GREEN}Testing the $(project)${NC}"
	export DOTNET_ENVIRONMENT "Development"
	@cd src && dotnet test --filter TestCategory!=windows-only

start: docker-check
	@echo -e "${GREEN}Starting the $(release) release of $(project)${NC}"
	@cd src/Command.Bot/ && dotnet run -- run -v

version:
	@echo -e "${GREEN}Setting version number $(version) ${NC}"
	@sed 's/Version>.*</Version>$(version-tag)</' src/Command.Bot.Core/Command.Bot.Core.csproj > src/Command.Bot.Core/Command.Bot.Core.csproj.ch  
	@mv  src/Command.Bot.Core/Command.Bot.Core.csproj.ch src/Command.Bot.Core/Command.Bot.Core.csproj

publish-nuget: version
	@echo -e "${GREEN}Publish branch $(current-branch) to $(version-tag)${NC}"
	@cd src/Command.Bot.Core && dotnet build --configuration Release
	dotnet pack src/Command.Bot.Core/Command.Bot.Core.csproj
	dotnet nuget push src/Command.Bot.Core/bin/Debug/Command.Bot.Core.*.nupkg -k ${NUGET_KEY} -s https://api.nuget.org/v3/index.json

publish-zip: version
	@echo -e "${GREEN}Publish branch $(current-branch) to $(version-tag) zip file${NC}"
	@echo -e "Clean folder"
	@if [ -d "./dist/Command.Bot.$(version-tag)" ]; then rm -Rf ./dist/Command.Bot.$(version-tag); fi
	@if [ -f "./dist/Command.Bot.$(version-tag).zip" ]; then rm -Rf ./dist/Command.Bot.$(version-tag).zip; fi
	@echo -e "Publish"
	@cd src/Command.Bot.Service && dotnet publish -c Release -o ../../dist/Command.Bot.$(version-tag)  /p:Version=$(version)
	@mkdir ./dist/Command.Bot.$(version-tag)/scripts/
	@cp src/Command.Bot.Core.Tests/Samples/* ./dist/Command.Bot.$(version-tag)/scripts/
	@echo -e "Zip to ./Command.Bot.$(version-tag).zip"
	@cd dist/ && zip -rq ./Command.Bot.$(version-tag).zip ./Command.Bot.$(version-tag)

    
docker-check:
	$(call assert-file-exists,$(docker-filecheck), This step should only be run from Docker. Please run `make up` first.)

env-check:
	$(call check_defined, env, No environment set. Supported environments are: [ master | dev | prod ]. Please set the env variable. e.g. `make env=dev plan`)

# Check that given variables are set and all have non-empty values,
# die with an error otherwise.
#
# Params:
#   1. Variable name(s) to test.
#   2. (optional) Error message to print.
check_defined = \
    $(strip $(foreach 1,$1, \
    	$(call __check_defined,$1,$(strip $(value 2)))))
__check_defined = \
    $(if $(value $1),, \
    	$(error Undefined $1$(if $2, ($2))))

define assert
  $(if $1,,$(error Assertion failed: $2))
endef

define assert_warn
  $(if $1,,$(warn Assertion failed: $2))
endef

define assert-file-exists
  $(call assert,$(wildcard $1),$1 does not exist. $2)
endef
