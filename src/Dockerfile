FROM microsoft/dotnet:2.1-sdk-alpine3.7 as dotnet

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
RUN ./build.sh --target=Test
RUN dotnet publish -c Release -o out

FROM microsoft/dotnet:2.1-runtime

WORKDIR /app
COPY --from=dotnet /app/Command.Bot/out .

ENTRYPOINT ["dotnet", "Command.Bot.dll", "run", "-v", "-c"]