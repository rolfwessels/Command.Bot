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

RUN mkdir /app
WORKDIR /app

RUN printf 'export PS1="\[$(tput setaf 4)\] __v_\\n\[$(tput setaf 4)\]($(tput smul)₀   $(tput rmul)\/{\[$(tput sgr0)\] \\t \[$(tput setaf 5)\][\w]\[$(tput sgr0)\]\$ "' >> ~/.bashrc
CMD ["top"]
