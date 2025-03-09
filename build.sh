#!/bin/bash

set -e

# Default parameters
CONFIGURATION="Release"
RUNTIME=("win-x64" "linux-x64" "osx-x64")
NO_RESTORE=false
NO_BUILD=false
NO_PUBLISH=false

# Parse arguments
while [[ $# -gt 0 ]]; do
    case "$1" in
        --configuration|-c)
            CONFIGURATION="$2"
            shift 2
            ;;
        --runtime|-r)
            RUNTIME=("$2")
            shift 2
            ;;
        --no-restore)
            NO_RESTORE=true
            shift
            ;;
        --no-build)
            NO_BUILD=true
            shift
            ;;
        --no-publish)
            NO_PUBLISH=true
            shift
            ;;
        *)
            echo "Unknown argument: $1"
            exit 1
            ;;
    esac
done

# Set up directories
ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ARTIFACTS_DIR="$ROOT_DIR/artifacts"

# Clean up artifacts
if [ -d "$ARTIFACTS_DIR" ]; then
    rm -rf "$ARTIFACTS_DIR"
fi

mkdir -p "$ARTIFACTS_DIR"

# Restore
if [ "$NO_RESTORE" = false ]; then
    echo "Restoring packages..."
    dotnet restore
fi

# Build
if [ "$NO_BUILD" = false ]; then
    echo "Building ($CONFIGURATION)..."
    dotnet build --configuration "$CONFIGURATION" --no-restore
fi

# Publish
if [ "$NO_PUBLISH" = false ]; then
    for rt in "${RUNTIME[@]}"; do
        echo "Publishing for $rt..."
        
        RUNTIME_DIR="$ARTIFACTS_DIR/$rt"
        mkdir -p "$RUNTIME_DIR"
        
        dotnet publish GuildedChatExporter.Cli \
            --configuration "$CONFIGURATION" \
            --runtime "$rt" \
            --no-restore \
            --no-build \
            --self-contained true \
            --output "$RUNTIME_DIR"
            
        # Zip the artifacts
        ZIP_PATH="$ARTIFACTS_DIR/GuildedChatExporter-$rt.zip"
        (cd "$RUNTIME_DIR" && zip -r "$ZIP_PATH" .)
    done
fi

echo "Done!"

# Make the script executable
chmod +x "$0"
