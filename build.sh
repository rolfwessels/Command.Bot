#!/usr/bin/env bash
# Define varibles
SCRIPT_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
#!/usr/bin/env bash
CAKE_VERSION=0.38.4
DOTNET_VERSION=2.1.701

TOOLS_DIR=$SCRIPT_DIR/tools/.linux/
CAKE_EXE="~/.dotnet/tools/dotnet-cake"
CAKE_PATH=$TOOLS_DIR/.store/cake.tool/$CAKE_VERSION

if [ "$CAKE_VERSION" = "" ] || [ "$DOTNET_VERSION" = "" ]; then
    echo "An error occured while parsing Cake / .NET Core SDK version."
    exit 1
fi

# Make sure the tools folder exist.
if [ ! -d "$TOOLS_DIR" ]; then
  mkdir "$TOOLS_DIR"
fi

###########################################################################
# INSTALL .NET CORE CLI
###########################################################################

export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_SYSTEM_NET_HTTP_USESOCKETSHTTPHANDLER=0
export DOTNET_ROLL_FORWARD_ON_NO_CANDIDATE_FX=2

DOTNET_INSTALLED_VERSION=$(dotnet --version 2>&1)

if [ "$DOTNET_VERSION" != "$DOTNET_INSTALLED_VERSION" ]; then
    echo "YOU NEED TO INSTALL DOTNET???... or its is there as version $DOTNET_INSTALLED_VERSION"
    # if [ ! -d "$SCRIPT_DIR/.dotnet" ]; then
    #   mkdir "$SCRIPT_DIR/.dotnet"
    # fi
    # curl -Lsfo "$SCRIPT_DIR/.dotnet/dotnet-install.sh" https://dot.net/v1/dotnet-install.sh
    # bash "$SCRIPT_DIR/.dotnet/dotnet-install.sh" --version $DOTNET_VERSION --install-dir .dotnet --no-path
    # export PATH="$SCRIPT_DIR/.dotnet":$PATH
    # export DOTNET_ROOT="$SCRIPT_DIR/.dotnet"
fi

###########################################################################
# INSTALL CAKE
###########################################################################

CAKE_INSTALLED_VERSION=$(~/.dotnet/tools/dotnet-cake --version 2>&1)

if [ "$CAKE_VERSION" != "$CAKE_INSTALLED_VERSION" ]; then
    if [ ! -f "$CAKE_EXE" ] || [ ! -d "$CAKE_PATH" ]; then
        if [ -f "$CAKE_EXE" ]; then
            dotnet tool uninstall --tool-path $TOOLS_DIR Cake.Tool
        fi

        echo "Installing Cake $CAKE_VERSION..."
        dotnet tool install -g --version $CAKE_VERSION Cake.Tool
        
        if [ $? -ne 0 ]; then
            echo "An error occured while installing Cake."
            exit 1
        fi
    fi
    CAKE_EXE="~/.dotnet/tools/dotnet-cake"
else
    CAKE_EXE="~/.dotnet/tools/dotnet-cake"
fi

###########################################################################
# RUN BUILD SCRIPT
###########################################################################

# Start Cake
(exec "$CAKE_EXE" build.cake --bootstrap) && (exec "$CAKE_EXE" build.cake "$@")