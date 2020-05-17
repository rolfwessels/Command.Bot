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
- For testing you can run "Command.Bot.exe service run"
- Once that is working you can add command bot as a service by running "Command.Bot.exe service install" as an _administator_

## Running on linux-docker

```bash
docker run -e Bot__BotKey=${Bot__BotKey} -e Bot__AllowedUser=${Bot__AllowedUser} --name command-bot rolfwessels/command-bot:latest
```

### Execute from command line

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

### Build release for github

```bash
#update the version
go deploy -properties @{'buildConfiguration'='Release'}
#upload the zip under build/dist to github
```

### Testings and playing with docker

```bash
docker-compose config
docker-compose up -d
docker-compose exec dev sh
docker-compose down
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
Set-ExecutionPolicy RemoteSigned -Scope Process
Unblock-File build.ps1
```
