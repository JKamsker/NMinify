#!/bin/bash
error() {
    printf "'%s' failed with exit code %d in function '%s' at line %d.\n" "${1-something}" "$?" "${FUNCNAME[1]}" "${BASH_LINENO[0]}"
}

ORIG_DIR=$(pwd)
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )" # Get the path to the directory of this script
ROOT_DIR=$SCRIPT_DIR/..
SRC_DIR=$ROOT_DIR/src/minify-go/
OUTPUT_DIR=$ROOT_DIR/.bin/go/lib

# BEGIN BUILD
cd $SCRIPT_DIR

if [ ! -f "$SCRIPT_DIR/tmp/xgo" ]; then
    mkdir $SCRIPT_DIR/tmp
    cd $SCRIPT_DIR/tmp

    wget https://github.com/crazy-max/xgo/releases/download/v0.24.0/xgo_0.24.0_linux_amd64.tar.gz
    tar -xvf xgo_0.24.0_linux_amd64.tar.gz
    chmod +x xgo

    cd $SCRIPT_DIR
else
    echo "xgo already downloaded"
fi
# exit;
# delete $ROOT_DIR/.bin/go/tmp if exists
if [ -d "$ROOT_DIR/.bin/go/tmp" ]; then
    rm -rf $ROOT_DIR/.bin/go/tmp
fi

mkdir -p $ROOT_DIR/.bin/go/tmp/output
cd $ROOT_DIR/.bin/go/tmp/output

# does not work, just for debug
# $SCRIPT_DIR/tmp/xgo -buildmode=c-shared -targets windows/386 $SRC_DIR
$SCRIPT_DIR/tmp/xgo -buildmode=c-shared -targets windows/amd64 $SRC_DIR
# END BUILD


cd $ROOT_DIR/.bin/go/tmp


rm -rf $OUTPUT_DIR
# TargetPath: .bin/go/lib/$platform-$architecture/native
mkdir -p $OUTPUT_DIR

# Loop through the files in the current directory
for file in output/*; do
    echo $file
    
    # Extract the platform and architecture from the filename using regular expressions
    if [[ $file =~ minifier-bindings-(darwin|linux|windows)-(amd64|arm64|ppc64le|riscv64|s390x|386).* ]]; then
        platform=${BASH_REMATCH[1]}
        architecture=${BASH_REMATCH[2]}
        
        # Create the platform directory if it doesn't already exist
        if [ ! -d "lib/$platform-$architecture/native" ]; then
            mkdir -p "lib/$platform-$architecture/native"
        fi
        
        # Copy the file to the platform directory
        cp "$file" "lib/$platform-$architecture/native/"
    fi
done

mv lib $OUTPUT_DIR/..

rm -rf $ROOT_DIR/.bin/go/tmp
