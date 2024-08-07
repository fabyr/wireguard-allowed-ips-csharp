# wireguard-allowed-ips-csharp

A small CLI tool or library written in C# to calculate the `AllowedIPs` field 
of a wireguard config if you want to exclude certain IP-Ranges.

Inspired by (and used for test reference):
https://krasovs.ky/2021/07/04/wireguard-allowed-ips.html

## Table of contents
1. [Dependencies](#dependencies)
2. [Releases](#releases)
3. [Usage](#usage)
4. [Usage within another C# application](#usage-within-another-c-application)
5. [Motivation](#motivation)
6. [License](#license)

## Dependencies
This program requires [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).
Other than that it was designed to require no other dependencies.

## Releases
Executable files are provided for all major platforms. (Because why not :>) \
All files ending in `-selfcontained` do not require the `.NET 8` Runtime to be installed on the system.
Though they are *massively greater in filesize* than the runtime dependent ones.

## Usage
(*Note:* if you want to use `dotnet run`, make sure the terminal's working directory is [WireguardAllowedIPs](/WireguardAllowedIPs/): `cd WireguardAllowedIPs` inside the reposity directory)

Upon running the program without arguments via `dotnet run` or via the precompiled executable,
a help is displayed:
```
Usage: wireguard-allowed-ips [-h] [-a "all"|allowed-ranges] [-d disallowed-ranges]

Calculate the allowed ranges from an underlying allowed range
and disallowed ranges within that range.

Options:
 -h, --help             Displays this help.

 -a, --allowed          Set the allowed ip ranges in CIDR notation, separated by commas.
                        Default Value if omitted: All IPs are allowed (both IPv4 and IPv6).

 -d, --disallowed       Set the disallowed ip ranges in CIDR notation, separated by commas.
```
Make sure not to place spaces between multiple networks: \
Incorrect: `--disallowed 10.0.0.0/8, 192.168.0.0/16` \
Correct: `--disallowed 10.0.0.0/8,192.168.0.0/16` \
The former only works if quoted: \
`--disallowed "10.0.0.0/8, 192.168.0.0/16"`

By default, if no allowed IP-Ranges are supplied, all networks are included in the calculation (`0.0.0.0/0, ::0/0`).

Examples: \
(*Note:* If you're using the precompiled executable, replace `dotnet run` with the name of the executable)
```
dotnet run --disallowed 10.0.0.0/8,192.168.0.0/16,72.8.99.100/32
```
```
0.0.0.0/5, 8.0.0.0/7, 11.0.0.0/8, 12.0.0.0/6, 16.0.0.0/4, 32.0.0.0/3, 64.0.0.0/5, 72.0.0.0/13, 72.8.0.0/18, 72.8.64.0/19, 72.8.96.0/23, 72.8.98.0/24, 72.8.99.0/26, 72.8.99.64/27, 72.8.99.96/30, 72.8.99.101/32, 72.8.99.102/31, 72.8.99.104/29, 72.8.99.112/28, 72.8.99.128/25, 72.8.100.0/22, 72.8.104.0/21, 72.8.112.0/20, 72.8.128.0/17, 72.9.0.0/16, 72.10.0.0/15, 72.12.0.0/14, 72.16.0.0/12, 72.32.0.0/11, 72.64.0.0/10, 72.128.0.0/9, 73.0.0.0/8, 74.0.0.0/7, 76.0.0.0/6, 80.0.0.0/4, 96.0.0.0/3, 128.0.0.0/2, 192.0.0.0/9, 192.128.0.0/11, 192.160.0.0/13, 192.169.0.0/16, 192.170.0.0/15, 192.172.0.0/14, 192.176.0.0/12, 192.192.0.0/10, 193.0.0.0/8, 194.0.0.0/7, 196.0.0.0/6, 200.0.0.0/5, 208.0.0.0/4, 224.0.0.0/3, ::/0
```
---
```
dotnet run --allowed 198.18.0.0/15 --disallowed 198.18.15.0/24
```
```
198.18.0.0/21, 198.18.8.0/22, 198.18.12.0/23, 198.18.14.0/24, 198.18.16.0/20, 198.18.32.0/19, 198.18.64.0/18, 198.18.128.0/17, 198.19.0.0/16
```
---

You can mix `IPv4` and `IPv6` to your liking, the application will group them accordingly.
```
dotnet run --allowed 198.0.0.0/8,f000::/4 --disallowed fe80:abcd::/32,198.18.15.0/24
```
```
198.0.0.0/12, 198.16.0.0/15, 198.18.0.0/21, 198.18.8.0/22, 198.18.12.0/23, 198.18.14.0/24, 198.18.16.0/20, 198.18.32.0/19, 198.18.64.0/18, 198.18.128.0/17, 198.19.0.0/16, 198.20.0.0/14, 198.24.0.0/13, 198.32.0.0/11, 198.64.0.0/10, 198.128.0.0/9, f000::/5, f800::/6, fc00::/7, fe00::/9, fe80::/17, fe80:8000::/19, fe80:a000::/21, fe80:a800::/23, fe80:aa00::/24, fe80:ab00::/25, fe80:ab80::/26, fe80:abc0::/29, fe80:abc8::/30, fe80:abcc::/32, fe80:abce::/31, fe80:abd0::/28, fe80:abe0::/27, fe80:ac00::/22, fe80:b000::/20, fe80:c000::/18, fe81::/16, fe82::/15, fe84::/14, fe88::/13, fe90::/12, fea0::/11, fec0::/10, ff00::/8
```

## Usage within another C# application
The namespace `WireguardAllowedIPs.Core` ([Directory](/WireguardAllowedIPs/Core/)) contains all necessary
tools to calculate `AllowedIPs`.

Example Code: ([Demo.cs](/WireguardAllowedIPs/Demo.cs))
```cs
string[] allowedIPs = new[] {
    "0.0.0.0/0",
    "::/0"
};
string[] disallowedIPs = new[] {
    "10.0.0.0/8",
    "192.168.0.0/16",
    "172.16.0.0/12",
    "72.8.99.100/32"
};

// Calculate AllowedIPs based on the values above
IPNetwork[] result = Calculator.CalculateAllowedIPs(allowedIPs, disallowedIPs);

// The IP-Types have implementations for ToString
Console.WriteLine($"AllowedIPs = {string.Join<IPNetwork>(",", result)}");
```

## Motivation
I was not able to find straightforward code which shows how to replicate the results found on 
https://krasovs.ky/2021/07/04/wireguard-allowed-ips.html \
(The code is written in Go and uses the `go4.org/netipx` package internally)

I needed a way to compute `AllowedIPs` dynamically myself. And also within a C# application. So I decided to do some research and write a small library for exactly that purpose!

## License
This project is licensed under the MIT license.