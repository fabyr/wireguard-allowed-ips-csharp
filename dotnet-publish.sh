#!/bin/bash
cd "$(dirname "$0")"

if [ -d "./publish" ]; then
    read -p "Delete ./publish directory [y/N]? " -r
    
    if [[ $REPLY =~ ^[Yy]$ ]]; then
        rm -rf ./publish
    else
        exit 1
    fi
fi

dotnet build wireguard-allowed-ips.sln --configuration Release &&

# Single-File executables

# Linux
dotnet publish ./WireguardAllowedIPs/WireguardAllowedIPs.csproj --configuration Release -o ./publish/linux-x64-selfcontained --os linux -a x64 /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true --self-contained &&
dotnet publish ./WireguardAllowedIPs/WireguardAllowedIPs.csproj --configuration Release -o ./publish/linux-arm-selfcontained --os linux -a arm /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true --self-contained &&
dotnet publish ./WireguardAllowedIPs/WireguardAllowedIPs.csproj --configuration Release -o ./publish/linux-arm64-selfcontained --os linux -a arm64 /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true --self-contained &&

# Windows
dotnet publish ./WireguardAllowedIPs/WireguardAllowedIPs.csproj --configuration Release -o ./publish/windows-x86-selfcontained --os win -a x86 /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true --self-contained &&
dotnet publish ./WireguardAllowedIPs/WireguardAllowedIPs.csproj --configuration Release -o ./publish/windows-x64-selfcontained --os win -a x64 /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true --self-contained &&

# Mac
dotnet publish ./WireguardAllowedIPs/WireguardAllowedIPs.csproj --configuration Release -o ./publish/osx-x64-selfcontained --os osx -a x64 /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true --self-contained &&
dotnet publish ./WireguardAllowedIPs/WireguardAllowedIPs.csproj --configuration Release -o ./publish/osx-arm64-selfcontained --os osx -a arm64 /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true --self-contained &&

# Runtime Executables

# Linux
dotnet publish ./WireguardAllowedIPs/WireguardAllowedIPs.csproj --configuration Release -o ./publish/linux-x64 --os linux -a x64 /p:PublishSingleFile=true &&
dotnet publish ./WireguardAllowedIPs/WireguardAllowedIPs.csproj --configuration Release -o ./publish/linux-arm --os linux -a arm /p:PublishSingleFile=true &&
dotnet publish ./WireguardAllowedIPs/WireguardAllowedIPs.csproj --configuration Release -o ./publish/linux-arm64 --os linux -a arm64 /p:PublishSingleFile=true &&

# Windows
dotnet publish ./WireguardAllowedIPs/WireguardAllowedIPs.csproj --configuration Release -o ./publish/windows-x86 --os win -a x86 /p:PublishSingleFile=true &&
dotnet publish ./WireguardAllowedIPs/WireguardAllowedIPs.csproj --configuration Release -o ./publish/windows-x64 --os win -a x64 /p:PublishSingleFile=true &&

# Mac
dotnet publish ./WireguardAllowedIPs/WireguardAllowedIPs.csproj --configuration Release -o ./publish/osx-x64 --os osx -a x64 /p:PublishSingleFile=true &&
dotnet publish ./WireguardAllowedIPs/WireguardAllowedIPs.csproj --configuration Release -o ./publish/osx-arm64 --os osx -a arm64 /p:PublishSingleFile=true &&

echo "Done!"
