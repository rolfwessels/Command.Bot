FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine as dotnet

RUN mkdir /tmp/app
WORKDIR /tmp/app

COPY Command.Bot/*.csproj ./Command.Bot/
COPY Command.Bot.Core/*.csproj ./Command.Bot.Core/
COPY Command.Bot.Core.Tests/*.csproj ./Command.Bot.Core.Tests/
COPY Command.Bot.Service/*.csproj ./Command.Bot.Service/
COPY Command.Bot.Shared/*.csproj ./Command.Bot.Shared/

WORKDIR /tmp/app/Command.Bot/
RUN dotnet restore

WORKDIR /app

COPY ./ ./
WORKDIR /app/Command.Bot
ARG VERSION=1.0.0
RUN dotnet publish -c Release -o /app/Command.Bot/out /p:Version=$VERSION 

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine

WORKDIR /app
COPY --from=dotnet /app/Command.Bot/out .

ENTRYPOINT ["dotnet", "Command.Bot.dll", "run", "-v", "-c"]