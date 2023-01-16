#!/bin/bash

rm -rf ../bin/lib
rm -rf lib

# Create the base directory
mkdir lib
mkdir ../bin

# Loop through the files in the current directory
for file in output/*; do
  # Extract the platform and architecture from the filename using regular expressions
  if [[ $file =~ minifier-bindings-(darwin|linux|windows)-(amd64|arm64|ppc64le|riscv64|s390x).* ]]; then
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

mv lib ../bin/lib