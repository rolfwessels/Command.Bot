FROM microsoft/dotnet:2.1-sdk-alpine3.7 as dotnet

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
    sudo \
    vim \
    nano \
    busybox \
    busybox-extras \
    nmap \
    screen \ 
    man \
    nodejs \
    ncurses \
    zip \
    nss

RUN mkdir /command.bot
WORKDIR /command.bot
# install this cake tool globally 
ENV TERM xterm-256color
RUN printf 'export PS1="\[\e[0;34;0;33m\][DCKR]\[\e[0m\] \\t \[\e[40;38;5;28m\][\w]\[\e[0m\] \$ "' >> ~/.bashrc
CMD ["top"]
