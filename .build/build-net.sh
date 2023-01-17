#!/bin/bash

# Called by .build/build-net.sh --version=1.0.9

SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )" # Get the path to the directory of this script
ROOT_DIR=$SCRIPT_DIR/..
SRC_DIR=$ROOT_DIR/src/minify-net/core
VERSION=1.0.0

# Parse arguments
for i in "$@"
do
case $i in
    --version=*)
    VERSION="${i#*=}"
    shift # past argument=value
    ;;
    *)
          # unknown option
    ;;
esac
done
echo "VERSION: $VERSION"


echo "SCRIPT_DIR: $SCRIPT_DIR"
echo "ROOT_DIR: $ROOT_DIR"
echo "SRC_DIR: $SRC_DIR"
echo "VERSION: $VERSION"

# restore
cd $SRC_DIR
dotnet restore

# build release mode without restoring
# version 1.0.5
cd $SRC_DIR
dotnet build -c Release --no-restore -p:Version=1.0.5


mkdir -p $ROOT_DIR/.bin/net/lib/
rm -rf $ROOT_DIR/.bin/net/lib/*
mv $ROOT_DIR/src/minify-net/Core/bin/Release/*.nupkg $ROOT_DIR/.bin/net/lib/

