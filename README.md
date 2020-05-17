[![Build Status](https://travis-ci.org/rolfwessels/Command.Bot.svg?branch=master)](https://travis-ci.org/rolfwessels/Command.Bot)
[![Github release](https://img.shields.io/github/v/release/rolfwessels/Command.Bot)](https://github.com/rolfwessels/Command.Bot/releases)
[![Dockerhub Status](https://img.shields.io/badge/dockerhub-ok-blue.svg)](https://hub.docker.com/r/rolfwessels/command-bot/tags)
[![Dockerhub Version](https://img.shields.io/docker/v/rolfwessels/command-bot?sort=semver)](https://hub.docker.com/r/rolfwessels/command-bot/tags)

# Command.Bot

## Installing Command bot

- Add "bots" to your slack installation. (on https://slack.com/apps search for bots and add it. Give the bot the username that you desire)
- Copy the API Token
- Download the bot from https://github.com/rolfwessels/Command.Bot/releases
- Extract bot into working folder.
- Update the `appSettings.json`. Change BotKey setting and the AllowedUser settings. (Note allow user can be multiple users, that are comma seperated)
- For testing you can run "Command.Bot.Service.exe run -v"
- Once that is working you can add command bot as a service by running "Command.Bot.exe service install" as an _administator_

## Running on linux-docker

```bash
docker run -e Bot__BotKey=${Bot__BotKey} -e Bot__AllowedUser=${Bot__AllowedUser} --name command-bot rolfwessels/command-bot:latest
```

### Using docker compose

Add the following

```docker
version: "3"
networks:
  slack-bot-dev-network:

services:
  slack_bot_dev:
    image: rolfwessels/command-bot:latest
    deploy:
    restart: unless-stopped
    environment:
      - Bot__BotKey=${Bot__BotKey}
      - Bot__AllowedUser=${Bot__AllowedUser}
      - Bot__ScriptsPath=/scripts
    volumes:
      - ./Command.Bot.Core.Tests/Samples:/scripts
    networks:
      - slack-bot-dev-network
```

## Using command bot

- First type 'help' as a dm to the bot that you have just added.
- Add files to /script folder to be able to execute them
- type the file name of the script that you want to execute to run it.
- note that you can also pass parameters to the scripts by appending them to the command.

## Developers

### Cake targets on windows

- `.\build.ps1 -t Default` The default task that just build and tests.
- `.\build.ps1 -t Build` Build the project.
- `.\build.ps1 -t Test` Run unit tests.
- `.\build.ps1 -t Dist` Build release zip file in the dist folder.
- `.\build.ps1 -t Clean` Cleans all directories that are used during the build process.
- `.\build.ps1 -t Build-Docker` Build docker dev container.
- `.\build.ps1 -t Up` Open in docker dev container.
- `.\build.ps1 -t Down` Stop docker dev container.

### Cake targets on linux

- `./build.sh --target=Default` The default task that just build and tests.
- `./build.sh --target=Build` Build the project.
- `./build.sh --target=Test` Run unit tests.
- `./build.sh --target=Dist` Build release zip file in the dist folder.
- `./build.sh --target=Clean` Cleans all directories that are used during the build process.
- `./build.sh --target=Build-Docker` Build docker dev container.
- `./build.sh --target=Up` Open in docker dev container.
- `./build.sh --target=Down` Stop docker dev container.

### Testings and playing with docker

```cmd
.\build.ps1 -t Build-Docker
.\build.ps1 -t Up
```

### Build and deploy docker

```bash
cd src\
docker login
docker build -t rolfwessels/command-bot:latest -t rolfwessels/command-bot:v1.0.4 ./
docker push -t rolfwessels/command-bot
```

## Adding required security for cake ps build script

see <https://cakebuild.net/docs/tutorials/powershell-security>

```cmd
Get-ExecutionPolicy -List
Set-ExecutionPolicy RemoteSigned -Scope CurrentUser
Unblock-File build.ps1
```
