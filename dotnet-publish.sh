#!/bin/bash
cd "$(dirname "$0")"

CURRENT_VERSION="1.0.0"

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

# Macos
dotnet publish ./WireguardAllowedIPs/WireguardAllowedIPs.csproj --configuration Release -o ./publish/macos-x64-selfcontained --os osx -a x64 /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true --self-contained &&
dotnet publish ./WireguardAllowedIPs/WireguardAllowedIPs.csproj --configuration Release -o ./publish/macos-arm64-selfcontained --os osx -a arm64 /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true --self-contained &&

# Runtime Executables

# Linux
dotnet publish ./WireguardAllowedIPs/WireguardAllowedIPs.csproj --configuration Release -o ./publish/linux-x64 --os linux -a x64 /p:PublishSingleFile=true &&
dotnet publish ./WireguardAllowedIPs/WireguardAllowedIPs.csproj --configuration Release -o ./publish/linux-arm --os linux -a arm /p:PublishSingleFile=true &&
dotnet publish ./WireguardAllowedIPs/WireguardAllowedIPs.csproj --configuration Release -o ./publish/linux-arm64 --os linux -a arm64 /p:PublishSingleFile=true &&

# Windows
dotnet publish ./WireguardAllowedIPs/WireguardAllowedIPs.csproj --configuration Release -o ./publish/windows-x86 --os win -a x86 /p:PublishSingleFile=true &&
dotnet publish ./WireguardAllowedIPs/WireguardAllowedIPs.csproj --configuration Release -o ./publish/windows-x64 --os win -a x64 /p:PublishSingleFile=true &&

# Macos
dotnet publish ./WireguardAllowedIPs/WireguardAllowedIPs.csproj --configuration Release -o ./publish/macos-x64 --os osx -a x64 /p:PublishSingleFile=true &&
dotnet publish ./WireguardAllowedIPs/WireguardAllowedIPs.csproj --configuration Release -o ./publish/macos-arm64 --os osx -a arm64 /p:PublishSingleFile=true &&

echo "Done building!" &&

echo "Renaming and copying all binaries to ./publish" &&
cd ./publish &&
for dir in $(ls); do 
    cp $dir/$(ls $dir -I "*.pdb") ./wg-ips-$dir$(ls $dir -I "*.pdb" | egrep -o '\..+$'); 
done &&
echo "Creating archives..." &&

# Linux
tar -czf wg-ips-$CURRENT_VERSION-linux.tar.gz $(find . -maxdepth 1 -type f -regex '^.*linux.*$' | grep -v 'selfcontained') &&
tar -czf wg-ips-$CURRENT_VERSION-linux-selfcontained.tar.gz $(find . -maxdepth 1 -type f -regex '^.*linux.*selfcontained.*$') &&

# Macos
tar -czf wg-ips-$CURRENT_VERSION-macos.tar.gz $(find . -maxdepth 1 -type f -regex '^.*macos.*$' | grep -v 'selfcontained') &&
tar -czf wg-ips-$CURRENT_VERSION-macos-selfcontained.tar.gz $(find . -maxdepth 1 -type f -regex '^.*macos.*selfcontained.*$') &&

# Windows
zip wg-ips-$CURRENT_VERSION-windows.zip $(find . -maxdepth 1 -type f -regex '^.*windows.*$' | grep -v 'selfcontained') &&
zip wg-ips-$CURRENT_VERSION-windows-selfcontained.zip $(find . -maxdepth 1 -type f -regex '^.*windows.*selfcontained.*$') &&

echo "Done!"