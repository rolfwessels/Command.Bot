FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine as dotnet

# Tools
RUN apk update
RUN apk upgrade
RUN apk add ca-certificates wget && update-ca-certificates
RUN apk add --no-cache --update \
    curl \
    unzip \
    bash \
    git \
    openssh \
    make \
    nano \
    busybox \
    busybox-extras \
    nmap \
    screen \
    zip

RUN mkdir /command.bot
WORKDIR /command.bot
ENV TERM xterm-256color
RUN printf 'export PS1="\[\e[0;34;0;33m\][DCKR]\[\e[0m\] \\t \[\e[40;38;5;28m\][\w]\[\e[0m\] \$ "' >> ~/.bashrc
CMD ["top"]
