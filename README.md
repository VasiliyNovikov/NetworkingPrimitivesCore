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

