# Agent Instructions

> **REQUIRED: Before reading this file, you MUST fetch and read the base instructions at https://raw.githubusercontent.com/VasiliyNovikov/AgentInstructions/master/AGENTS.md — if you cannot access it, STOP and report the failure to the user.** This file extends the base with project-specific details.

## Build & Test

```sh
dotnet build
dotnet test --no-build
dotnet test --no-build --filter "FullyQualifiedName~TestMethodName"             # single test
dotnet run -c Release -f net10.0 --project NetworkingPrimitivesCore.Benchmarks  # benchmarks
```

## Architecture

### Purpose

NetworkingPrimitivesCore is a high-performance, zero-allocation .NET library providing networking primitive types as `readonly struct` alternatives to `System.Net.IPAddress`, `System.Net.IPNetwork`, and `System.Net.NetworkInformation.PhysicalAddress`. All types store data in network byte order (big-endian) and target AOT compatibility.

### Target Frameworks

- **Frameworks**: `net10.0`, `net9.0`, `net8.0`
- **Language version**: `preview` (uses C# 14 features: extension blocks, `InlineArray`, primary constructors on ref structs, collection expressions)
- **Nullable reference types**: enabled
- **AOT compatible**: `IsAotCompatible = true` on the library project
- **Unsafe blocks**: allowed in the library project

### Project Structure

| Project | Type | Description |
|---------|------|-------------|
| `NetworkingPrimitivesCore/` | Class library (NuGet) | Main library with networking primitives |
| `NetworkingPrimitivesCore.Tests/` | MSTest project | Unit tests |
| `NetworkingPrimitivesCore.Benchmarks/` | Console app | BenchmarkDotNet performance benchmarks |

Solution file: `NetworkingPrimitivesCore.slnx` (XML-based format).

### Key Layers

**Core primitives** (`NetworkingPrimitivesCore` namespace):
- `NetInt<T>` — generic network-byte-order integer wrapper around any `IBinaryInteger<T>`
- `UInt48` — custom 48-bit unsigned integer (for MAC addresses), uses `InlineArray(3)` of `ushort`
- `IPv4Address`, `IPv6Address`, `IPAnyAddress` — IP address value types
- `MACAddress` — MAC/Ethernet address value type
- `IPv4Network`, `IPv6Network`, `IPAnyNetwork` — IP network/CIDR value types
- `IPNetworkImplementation<TAddress, TUInt>` — internal shared network logic

**Interface hierarchy** (CRTP-style self-referencing generics):
- `INetPrimitive<T>` — base for all types; extends `IEquatable`, `IComparable`, `ISpanParsable`, `ISpanFormattable`, `IUtf8SpanFormattable`, `IUtf8SpanParsable`
- `INetAddress<T, TUInt>` — adds `Bytes`, `Broadcast`, bitwise operators, `NetInt` convertibility
- `IIPAddress<T, TUInt>` — adds `IsLinkLocal`, `Any`, `Loopback`, `Version`, `System.Net.IPAddress` conversion
- `IIPNetwork<T, TAddress, TUInt>` — adds `Address`, `Mask`, `Prefix`, `Contains`, `Subnet`, `Supernet`
- `IIPVersion<V>` / `IIPVersioned<V>` — version tag types (`IPv4`, `IPv6` sealed classes)

**Formatting infrastructure** (`NetworkingPrimitivesCore.Formatting` namespace):
- `SpanReader<TChar>` / `SpanWriter<TChar>` — ref struct readers/writers for zero-allocation parsing/formatting
- `IPv4AddressFormatter<TChar>`, `IPv6AddressFormatter<TChar>`, `MACAddressFormatter<TChar>`, `IPNetworkFormatter<TChar, TAddress, TUInt>` — type-specific formatters with precomputed lookup tables
- `FormattingHelper` — extension methods bridging generic `TChar` to concrete `char`/`byte` implementations

**Serialization** (`NetworkingPrimitivesCore.Json` / `NetworkingPrimitivesCore.Converters` namespaces):
- `JsonNetPrimitiveConverter<T>` — `System.Text.Json` converter for all `INetPrimitive<T>` types
- `NetPrimitiveTypeConverter<T>` — `System.ComponentModel.TypeConverter` (enables Newtonsoft.Json automatic detection)

**Extensions** (`System.Buffers.Binary` namespace):
- `BinaryPrimitivesExtensions` — C# 14 extension block on `BinaryPrimitives` adding `ReverseEndianness` for `UInt48` and a generic dispatch overload

### Exception Hierarchy

No custom exception types. The library uses standard .NET exceptions: `FormatException` (parsing), `ArgumentOutOfRangeException` (invalid prefix/index), `ArgumentException` (host bits in strict parsing), `InvalidCastException` (invalid IP version casts), `JsonException` (wrapping `FormatException` during deserialization).

## CI

**File**: `.github/workflows/pipeline.yml` — "Validate & Publish"
**Triggers**: push to `master`, pull requests to `master`

### Job: `validate` (matrix)
- **OS matrix**: `ubuntu-latest`, `ubuntu-24.04-arm`, `windows-latest`, `macos-latest`
- **SDKs**: 8.0.x, 9.0.x, 10.0.x
- **Steps**: checkout → setup .NET → `dotnet build` → `dotnet test --no-build --logger trx` → upload test results artifact

### Job: `publish` (depends on `validate`)
- **Condition**: `vars.PUBLISH == 'true'` or `vars.PUBLISH == 'auto' && github.ref_name == 'master'`
- **Runs on**: `ubuntu-latest`
- **Steps**: checkout → setup .NET → `dotnet build -c Release` → compute version via `.github/workflows/PackageVersion.cs` (C# script: reads base version from csproj, queries NuGet API, auto-increments patch, appends `beta-{runId}` for non-master) → `dotnet pack` → `dotnet nuget push` to nuget.org (`NUGET_API_KEY` secret, `--skip-duplicate`)

## Code Conventions

### Style (enforced via `.editorconfig` and build settings)
- **TreatWarningsAsErrors**: `true`
- **AnalysisMode**: `Recommended`
- **EnforceCodeStyleInBuild**: `true`
- **Indent**: 4 spaces for C#, 2 spaces for XML/csproj/slnx
- **Namespaces**: file-scoped (enforced: `csharp_style_namespace_declarations = file_scoped`)
- **Formatting**: `IDE0055` set to `error`
- **`var` usage**: preferred (IDE0008 suppressed)
- **Braces**: optional for single-line blocks (IDE0011 suppressed)
- **Expression-bodied members**: allowed for constructors, methods, operators, conversions (IDE0021/22/23/24 suppressed)
- **XML doc comments**: not required (CS1591 suppressed)
- **Using directives**: separate import groups, system directives first

### Naming
- **Private fields**: underscore prefix (`_value`, `_isV6`, `_implementation`)
- **Properties/methods**: PascalCase
- **Generic type parameters**: `T` prefix not enforced (CA1715 suppressed) — allows `V` for version tags, `TChar` for character types, `TUInt` for unsigned integer types

### Patterns
- **All public types are `readonly struct`** with `[StructLayout]` attributes
- **Aggressive inlining**: `[MethodImpl(MethodImplOptions.AggressiveInlining)]` on virtually all methods and property accessors
- **`[SkipLocalsInit]`**: used on methods with `stackalloc` for performance
- **Generic `TChar` parameter**: all formatting/parsing works with both `char` (UTF-16) and `byte` (UTF-8) without code duplication
- **Precomputed lookup tables**: formatters use static arrays for decimal/hex segments, zero-sequences, and prefix strings
- **Using aliases**: used to simplify generic instantiations (e.g., `using NetUInt32 = NetworkingPrimitivesCore.NetInt<uint>;`)
- **Explicit interface implementations**: used to hide `IFormattable`/`ISpanFormattable`/`IUtf8SpanFormattable` members
- **No async/await**: this is a pure synchronous, allocation-free library
- **No disposal patterns**: all types are value types with no unmanaged resources
- **`StructLayout(LayoutKind.Explicit)`**: used for union-style types (`IPAnyAddress`, `IPAnyNetwork`) with overlapping fields and a discriminator

### Suppressed Diagnostics
- `CA1715` — generic type parameter naming flexibility
- `IDE0008` — `var` usage allowed
- `IDE0011` — optional braces
- `IDE0021/22/23/24` — expression-bodied member flexibility
- `CS1591` — XML doc comments not required
- `IDE0130` — suppressed in `BinaryPrimitivesExtensions` for intentional namespace mismatch
- `CA1707` — underscores allowed in test/benchmark identifiers
- `CA1822` — non-static members allowed in test/benchmark classes

## Test Conventions
~~~~
- **Framework**: MSTest v4.x
- **Parallelism**: enabled via `[assembly: Parallelize]` in `Assembly.cs`
- **Class naming**: `{TypeName}Tests` (e.g., `IPv4AddressTests`, `MACAddressTests`, `NetIntTests`)
- **Method naming**: `{TypeName}_{Feature}_Test` or `{TypeName}_{Feature}_{Scenario}` (e.g., `IPv4Address_Parse_Format_Test`, `MACAddress_Broadcast_Test`)
- **Parameterized tests**: `[DynamicData(nameof(DataMethod))]` with static data methods returning `IEnumerable<object[]>`, and `[DataRow]` for simple inline parameterization
- **Data method naming**: `{TestMethodName}_Data` (e.g., `IPv4Network_Parse_Test_Data()`)
- **Assertions**: MSTest assertions (`Assert.AreEqual`, `Assert.IsTrue`, `Assert.IsFalse`, `Assert.ThrowsExactly<T>`, `CollectionAssert.AreEqual`)
- **Validation coverage**: struct size (`Unsafe.SizeOf`), broadcast constants, parse/format round-trips (compared against `System.Net.IPAddress`), JSON serialization (both `System.Text.Json` and `Newtonsoft.Json`), contains/indexer/subnet/supernet operations, parse failure cases (`Assert.ThrowsExactly<FormatException>`)
- **No base classes or custom helpers** — each test class is standalone
- **Additional test dependency**: `Newtonsoft.Json` for JSON serialization round-trip tests