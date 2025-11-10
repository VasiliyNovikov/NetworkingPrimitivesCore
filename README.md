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
| Format_IPAnyAddress |  0.65 |         - |
|                     |       |           |
| Parse_IPAddress     |  1.00 |    1000 B |
| Parse_IPAnyAddress  |  0.45 |         - |

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
| Parse_IPv6_IPv6Address  |  0.46 |         - |

## IPAnyNetwork
| Method                | Ratio | Allocated |
|---------------------- |------:|----------:|
| Contains_IPNetwork    |  1.00 |         - |
| Contains_IPAnyNetwork |  0.87 |         - |
|                       |       |           |
| Format_IPNetwork      |  1.00 |         - |
| Format_IPAnyNetwork   |  0.78 |         - |
|                       |       |           |
| Parse_IPNetwork       |  1.00 |     480 B |
| Parse_IPAnyNetwork    |  0.63 |         - |

## IPv4Network
| Method               | Ratio | Allocated |
|--------------------- |------:|----------:|
| Contains_IPNetwork   |  1.00 |         - |
| Contains_IPv4Network |  0.37 |         - |
|                      |       |           |
| Format_IPNetwork     |  1.00 |         - |
| Format_IPv4Network   |  0.60 |         - |
|                      |       |           |
| Parse_IPNetwork      |  1.00 |     160 B |
| Parse_IPv4Network    |  0.70 |         - |

## IPv6Network
| Method               | Ratio | Allocated |
|--------------------- |------:|----------:|
| Contains_IPNetwork   |  1.00 |         - |
| Contains_IPv6Network |  0.23 |         - |
|                      |       |           |
| Format_IPNetwork     |  1.00 |         - |
| Format_IPv6Network   |  0.87 |         - |
|                      |       |           |
| Parse_IPNetwork      |  1.00 |     320 B |
| Parse_IPv6Network    |  0.65 |         - |