#!/bin/bash

# amd64
# archs=( 386) # ppc64le ppc64 s390x

# for arch in ${archs[@]}
# do
#     # go build --buildmode=c-shared -ldflags="-s -w"  -o main.dll .\minify.go 
#     echo "Building for $arch"
#     env GOOS=linux GOARCH=$arch go build --buildmode=c-shared -ldflags="-s -w"  -o ../bin/native/linux/${arch}/minifier ./minify.go
#     # env GOOS=linux GOARCH=${arch} go build -o ../bin/native/linux/${arch}/minifier
# done

# /mnt/c/Users/W31rd0/source/repos/Random/minify-dotnet/Minifier

# env GOOS=linux GOARCH=arm64 go build --buildmode=c-shared -ldflags="-s -w"  -o ../bin/native/linux/arm64/minifier ./minify.go


# go tool dist list | grep linux

# env GOOS=linux GOARCH=arm64 CGO_ENABLED=1 /bin/bash
# env GOOS=linux GOARCH=386 CGO_ENABLED=1 /bin/bash
# env GOOS=windows GOARCH=amd64 CGO_ENABLED=1 /bin/bash

# go build --buildmode=c-shared -ldflags="-s -w" -o minifier ./minify.go



# apt-get install gcc-multilib gcc-mingw-w64


# .\xgo.exe -buildmode=c-shared .

#   -branch string
#         Version control branch to build
#   -buildmode string
#         Indicates which kind of object file to build (default "default")
#   -buildvcs string
#         Whether to stamp binaries with version control information
#   -deps string
#         CGO dependencies (configure/make based archives)
#   -depsargs string
#         CGO dependency configure arguments
#   -dest string
#         Destination folder to put binaries in (empty = current)
#   -docker-image string
#         Use custom docker image instead of official distribution
#   -docker-repo string
#         Use custom docker repo instead of official distribution
#   -go string
#         Go release to use for cross compilation (default "latest")
#   -goproxy string
#         Set a Global Proxy for Go Modules
#   -ldflags string
#         Arguments to pass on each go tool link invocation
#   -out string
#         Prefix to use for output naming (empty = package name)
#   -pkg string
#         Sub-package to build if not root import
#   -race
#         Enable data race detection (supported only on amd64)
#   -remote string
#         Version control remote repository to build
#   -tags string
#         List of build tags to consider satisfied during the build
#   -targets string
#         Comma separated targets to build for (default "*/*")
#   -trimpath
#         Remove all file system paths from the resulting executable
#   -v    Print the names of packages as they are compiled
#   -x    Print the command as executing the builds

# Get the path to the directory of this script
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
echo "I am at $DIR"

# exit script
exit 1;

if [ ! -f "$DIR/../tmp/xgo" ]; then
    mkdir ../tmp
    cd ../tmp

    wget https://github.com/crazy-max/xgo/releases/download/v0.24.0/xgo_0.24.0_linux_amd64.tar.gz
    tar -xvf xgo_0.24.0_linux_amd64.tar.gz
    chmod +x xgo
    cd ../Minifier
else
    echo "xgo already downloaded"
fi

mkdir output
cd output

../../tmp/xgo -buildmode=c-shared ../
