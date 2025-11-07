# NetworkingPrimitivesCore
Networking primitives like NetInt, IPv4/IPv6 address/network, MAC address

# Benchmarks

## NetInt
| Method                | Ratio | Allocated |
|---------------------- |------:|----------:|
| U128_BinaryPrimitives |  1.02 |         - |
| U128_NetInt           |  0.85 |         - |
|                       |       |           |
| U16_BinaryPrimitives  |  1.00 |         - |
| U16_NetInt            |  1.00 |         - |
|                       |       |           |
| U32_BinaryPrimitives  |  1.00 |         - |
| U32_NetInt            |  0.98 |         - |
|                       |       |           |
| U64_BinaryPrimitives  |  1.00 |         - |
| U64_NetInt            |  0.99 |         - |

## MACAddress
| Method                 | Ratio | Allocated |
|----------------------- |------:|----------:|
| Format_PhysicalAddress |  1.00 |     240 B |
| Format_MACAddress      |  0.85 |         - |
|                        |       |           |
| Parse_PhysicalAddress  |  1.00 |     320 B |
| Parse_MACAddress       |  0.72 |         - |

## IPAnyAddress
| Method              | Ratio | Allocated |
|-------------------- |------:|----------:|
| Format_IPAddress    |  1.00 |         - |
| Format_IPAnyAddress |  0.69 |         - |
|                     |       |           |
| Parse_IPAddress     |  1.00 |    1000 B |
| Parse_IPAnyAddress  |  0.46 |         - |

## IPv4Address
| Method                  | Ratio | Allocated |
|------------------------ |------:|----------:|
| Format_IPv4_IPAddress   |  1.00 |         - |
| Format_IPv4_IPv4Address |  0.78 |         - |
|                         |       |           |
| Parse_IPv4_IPAddress    |  1.00 |     280 B |
| Parse_IPv4_IPv4Address  |  0.44 |         - |

## IPv6Address
| Method                  | Ratio | Allocated |
|------------------------ |------:|----------:|
| Format_IPv6_IPAddress   |  1.00 |         - |
| Format_IPv6_IPv6Address |  0.71 |         - |
|                         |       |           |
| Parse_IPv6_IPAddress    |  1.00 |     720 B |
| Parse_IPv6_IPv6Address  |  0.48 |         - |

## IPAnyNetwork
| Method              | Ratio | Allocated |
|-------------------- |------:|----------:|
| Format_IPNetwork    |  1.00 |         - |
| Format_IPAnyNetwork |  0.92 |         - |
|                     |       |           |
| Parse_IPNetwork     |  1.00 |     480 B |
| Parse_IPAnyNetwork  |  0.63 |         - |
