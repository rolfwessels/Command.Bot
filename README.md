[![Github release](https://img.shields.io/github/v/release/rolfwessels/Command.Bot)](https://github.com/rolfwessels/Command.Bot/releases)
[![Dockerhub Status](https://img.shields.io/badge/dockerhub-ok-blue.svg)](https://hub.docker.com/r/rolfwessels/command-bot/tags)
[![Dockerhub Version](https://img.shields.io/docker/v/rolfwessels/command-bot?sort=semver)](https://hub.docker.com/r/rolfwessels/command-bot/tags)

# Command.Bot

Slack bot that allows you to run scripts remotely.

## Installing Command bot

- Add "bots" to your slack installation. (on https://slack.com/apps search for bots and add it. Give the bot the username that you desire)
- Copy the API Token
- Download the bot from https://github.com/rolfwessels/Command.Bot/releases
- Extract bot into working folder.
- Update the `appSettings.json`. Change BotKey setting and the AllowedUser settings. (Note allow user can be multiple users, that are comma separated)
- For testing you can run "Command.Bot.Service.exe run -v"
- Once that is working you can add command bot as a service by running "Command.Bot.exe service install" as an _administator_
- If you would like a description next to the help message add a comment string containing `Description:={your description here}`

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

### Build and deploy docker

The following make commands are available.

```bash
Docker Targets (run from local machine)
  - up          : brings up the container & attach to the default container (dev)
  - down        : stops the container
  - build       : (re)builds the container

Service Targets (should only be run inside the docker container)
  - version         : Set current version number Command.Bot
  - start           : Run the Command.Bot
  - test            : Run the Command.Bot tests
  - publish         : Build the Command.Bot and publish to docker -t rolfwessels/command-bot:alpha
  - publish-nuget   : Publish the Command.Bot.Core and to nuget under version 1.1.61-alpha
  - publish-zip     : Build and compile to dist/Command.Bot.1.1.61-alpha.zip
  - deploy          : Deploy the Command.Bot
```

## Adding required security for cake ps build script

see <https://cakebuild.net/docs/tutorials/powershell-security>

```cmd
Get-ExecutionPolicy -List
Set-ExecutionPolicy RemoteSigned -Scope CurrentUser
Unblock-File build.ps1
```
