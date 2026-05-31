#!/bin/bash
set -euo pipefail
cd "$(dirname "$0")"

shopt -s nullglob
shopt -s extglob

if [ $# -ne 1 ]; then
    echo >&2 "Usage: $0 <Version>"
    exit 1
fi

CURRENT_VERSION="$1"

if [ -d "./publish" ]; then
    read -rp "Delete ./publish directory [y/N]? "

    if [[ $REPLY =~ ^[Yy]$ ]]; then
        rm -rf ./publish
    else
        exit 1
    fi
fi

function build() {
    local os="$1"
    local arch="$2"
    local output_name="$3"
    local configuration="$4"
    shift 4

    if ! dotnet publish ./WireguardAllowedIPs/WireguardAllowedIPs.csproj \
        --configuration "$configuration" \
        -o "./publish/$output_name" \
        --os "$os" -a "$arch" \
        "$@"; then
        echo >&2 "$output_name: FAILED"
    fi
}

function build_dependent() {
    local os="$1"
    local arch="$2"
    shift 2

    build "$os" "$arch" "$os-$arch-dependent" "Release" \
        /p:PublishSingleFile=true
}

function build_selfcontained() {
    local os="$1"
    local arch="$2"
    shift 2

    build "$os" "$arch" "$os-$arch-selfcontained" "Release" \
        /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true \
        --self-contained
}

function build_aot() {
    local os="$1"
    local arch="$2"
    shift 2

    build "$os" "$arch" "$os-$arch-aot" "ReleaseAOT"
}

mkdir -p ./publish

dotnet build wireguard-allowed-ips.slnx --configuration Release

# Selfcontained executables

# Linux
build_selfcontained linux x64
build_selfcontained linux arm
build_selfcontained linux arm64

# Windows
build_selfcontained win x86
build_selfcontained win x64

# Macos
build_selfcontained osx x64
build_selfcontained osx arm64

# Runtime dependent executables

# Linux
build_dependent linux x64
build_dependent linux arm
build_dependent linux arm64

# Windows
build_dependent win x86
build_dependent win x64

# Macos
build_dependent osx x64
build_dependent osx arm64

# AOT executables

# Linux
build_aot linux x64
build_aot linux arm
build_aot linux arm64

# Windows
build_aot win x86
build_aot win x64

# Macos
build_aot osx x64
build_aot osx arm64

echo "Done building!"

cd ./publish

echo "Renaming binaries..."
for dir in *; do
    rm -f "./$dir/"*.{pdb,dbg}

    files=("./$dir/"*)

    if [ "${#files[@]}" -eq 0 ]; then
        continue
    elif [ "${#files[@]}" -gt 1 ]; then
        echo >&2 "Expected single file in '$dir'."
        exit 1
    fi

    file="$(basename "${files[0]}")"

    extension="${file#"${file%%"."*}"}"
    output_name="$dir"

    if [[ "$output_name" == *"-dependent" ]]; then
        output_name="${output_name%-dependent}"
    fi

    mv "./$dir/$file" "./$dir/wg-ips-$output_name$extension"
done

echo "Creating archives..."

mkdir ./contents

function contents() {
    if [ $# -eq 0 ]; then
        return 1
    fi

    rm -f ./contents/*
    cp -- "$@" ./contents
}

# Linux
contents ./linux-*-dependent/* &&
    tar -czvf "wg-ips-$CURRENT_VERSION-linux.tar.gz" -C ./contents . || :

contents ./linux-*-selfcontained/* &&
    tar -czvf "wg-ips-$CURRENT_VERSION-linux-selfcontained.tar.gz" -C ./contents . || :

contents ./linux-*-aot/* &&
    tar -czvf "wg-ips-$CURRENT_VERSION-linux-aot.tar.gz" -C ./contents . || :

# Macos
contents ./osx-*-dependent/* &&
    tar -czvf "wg-ips-$CURRENT_VERSION-macos.tar.gz" -C ./contents . || :

contents ./osx-*-selfcontained/* &&
    tar -czvf "wg-ips-$CURRENT_VERSION-macos-selfcontained.tar.gz" -C ./contents . || :

contents ./osx-*-aot/* &&
    tar -czvf "wg-ips-$CURRENT_VERSION-macos-aot.tar.gz" -C ./contents . || :

# Windows
contents ./win-*-dependent/* &&
    zip -FS9orj "wg-ips-$CURRENT_VERSION-windows.zip" ./contents || :

contents ./win-*-selfcontained/* &&
    zip -FS9orj "wg-ips-$CURRENT_VERSION-windows-selfcontained.zip" ./contents || :

contents ./win-*-aot/* &&
    zip -FS9orj "wg-ips-$CURRENT_VERSION-windows-aot.zip" ./contents || :

rm -rf ./contents

echo "Done!"
