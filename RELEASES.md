# Releases

This document provides detailed release notes for the Betfair library.

## 🚀 Version 9.0.1-alpha-5 (2025-07-01)

**Ultra-Low-Latency MarketCache, Raw Stream Access & Code Quality**

A feature-rich release introducing a zero-allocation market cache for stream processing, raw byte-level stream access, comprehensive test coverage improvements (90%+), mutation testing, and full SonarCloud compliance.

### 🎯 **What's New**

#### Ultra-Low-Latency MarketCache
- **Zero-Allocation Processing**: New `MarketCacheProcessor` reads raw stream bytes directly via `Utf8JsonReader` — no intermediate objects, no GC pressure on steady-state deltas
- **In-Memory Market State**: `MarketCache`, `RunnerCache`, `PriceLadder`, and `PositionLadder` maintain live market state updated in-place
- **Market Definitions**: `MarketDefinitionCache` uses cached UTF-8 bytes for zero-allocation string comparison — only allocates when values actually change
- **First-Byte Dispatch**: Runner property parsing uses single-byte switching for minimal branching on hot paths
- **Deferred Ladder Updates**: Batches price/size updates into a reusable buffer, applied once the runner is resolved

#### Raw Stream Access
- **`Subscription.ReadRawLines`**: New method provides direct access to raw byte lines from the stream pipeline
- **Custom Pipelines**: Enables building zero-copy processing pipelines outside the built-in JSON deserializer

#### Benchmarks
- Full BenchmarkDotNet suite for stream processing (`MarketStreamPipelineBenchmarks`)
- JSON parsing comparison benchmarks (`ChangeMessageReader`, `JsonReadBenchmarks`)
- Python benchmark for cross-language comparison
- 8,000+ line real market stream sample data included

#### Mutation Testing
- Stryker.NET integration with `--since` for incremental mutation analysis
- API key configuration for dashboard reporting
- Optimized workflow running only against changed files

### 🐛 **Bug Fixes**

- **img/rc ordering bug**: Image flag now correctly clears runners regardless of JSON property order
- **IdleWatchdog race condition**: Fixed missed stall detection when watchdog poll and message arrival interleaved
- **Session token refresh**: Now triggers on exception type (`InvalidSessionInformationException`) instead of brittle exact message match
- **Flaky watchdog test**: Widened timing margins for CI reliability

### 🔧 **Code Quality**

#### SonarCloud — All Issues Resolved
- Refactored 5 methods exceeding Cognitive Complexity threshold (max was 57 → all now under 15)
- Replaced non-generic assertion overloads with generic equivalents (9 occurrences)
- Extracted repeated arrays to `static readonly` fields
- Replaced `FirstOrDefault` with `Array.Find` on arrays
- Removed unused private members

#### Local Analyzer Parity
- Added 20+ Sonar rules to `.editorconfig` — local builds now catch everything SonarCloud would flag
- Zero SA1204, CA1502, and S-prefixed warnings from all modified files

#### Dependencies Updated
- `SonarAnalyzer.CSharp` 10.21.0 → 10.27.0
- `System.IO.Pipelines` 10.0.5 → 10.0.9
- `DotNet.ReproducibleBuilds` 2.0.2 → 2.0.5
- Test packages updated (xunit, FluentAssertions, coverlet, etc.)

### 📚 **Documentation**

- New `docs/MarketCache.md` — architecture guide for the low-latency cache
- New `docs/BetfairApiClient.md` — comprehensive API client reference
- Updated Subscription, Authentication, and other existing docs

### 🧪 **Testing**

- **1,684 tests** — all passing on net8.0, net9.0, and net10.0
- **90% coverage** — up from ~75% in alpha-4
- New test suites: `MarketCacheProcessorTests`, `MarketCacheTests`, `MarketDefinitionCacheTests`, `PriceLadderTests`, `PositionLadderTests`, `RunnerCacheTests`, `ClearedOrdersQueryTests`, `BetfairExceptionFactoryTests`, `MarketFilterAdditionalTests`

### 📦 **Installation**

```bash
dotnet add package Betfair --version 9.0.1-alpha-5
```

### 🔗 **Compatibility**

- **Backward Compatible**: No breaking changes from alpha-4
- **Target Frameworks**: net8.0, net9.0, net10.0

---

## 🚀 Version 9.0.1-alpha (2025-06-19)

**Multi-Framework Support & NuGet Improvements**

This release adds multi-framework targeting support and enhances the NuGet package experience while maintaining full backward compatibility.

### 🎯 **What's New**

#### Multi-Framework Targeting
- **Dual Framework Support**: Library now targets both .NET 8.0 and .NET 9.0
- **Future-Ready**: Enables consumption from both current (.NET 8) and next-generation (.NET 9) projects
- **Seamless Migration**: Users can upgrade to .NET 9 without changing library dependencies
- **Comprehensive Testing**: All 2,848 tests pass on both target frameworks

#### Enhanced NuGet Package
- **Embedded Documentation**: README.md now included in NuGet package
- **Better Discoverability**: Full documentation, badges, and examples visible on nuget.org
- **Installation Instructions**: Clear `dotnet add package` and Package Manager commands
- **Code Examples**: Immediate access to usage patterns and API examples

### 🔧 **Technical Improvements**

#### Build System
- **Clean Builds**: Suppressed IL2026 warnings from generated JSON serialization code
- **AOT Compatibility**: Maintained ahead-of-time compilation support
- **Warning-Free**: Zero build warnings on both target frameworks
- **CI/CD Ready**: GitHub Actions workflows remain fully compatible

#### Quality Assurance
- **Cross-Framework Testing**: Automated testing on both .NET 8.0 and .NET 9.0
- **Performance Consistency**: No performance degradation across frameworks
- **Compatibility Verified**: All existing functionality works identically on both frameworks

### 📦 **Package Information**

- **Version**: 9.0.1-alpha
- **Target Frameworks**: net8.0, net9.0
- **Dependencies**: System.IO.Pipelines 9.0.6
- **Package Size**: Optimized for both frameworks

### 🚀 **Benefits for Developers**

#### Immediate Benefits
- **Framework Flexibility**: Choose .NET 8 or .NET 9 based on project requirements
- **Better Package Discovery**: Rich documentation directly on NuGet.org
- **Simplified Onboarding**: Installation and usage examples readily available

#### Future Benefits
- **Smooth Upgrades**: Seamless transition when upgrading projects to .NET 9
- **Latest Features**: Access to .NET 9 performance improvements and new APIs
- **Long-term Support**: Continued support for both LTS (.NET 8) and current (.NET 9) versions

### 📋 **Installation**

```bash
# .NET CLI
dotnet add package Betfair --version 9.0.1-alpha

# Package Manager Console
PM> Install-Package Betfair -Version 9.0.1-alpha
```

### 🔗 **Compatibility**

- **Backward Compatible**: No breaking changes from 9.0.0-alpha
- **GitHub Actions**: Existing CI/CD workflows continue to work unchanged
- **Dependencies**: All existing dependencies remain compatible

---

## 🚀 Version 9.0.0-alpha (2025-06-19)

**Major Release - Complete Library Overhaul**

This is a major release that represents a complete evolution of the Betfair library with significant architectural improvements, comprehensive API coverage, and enhanced developer experience.

### 🎯 **What's New**

#### Complete Betfair API Coverage
- **Account API**: Full support for AccountDetails, AccountFunds, AccountStatement, and CurrencyRates
- **Betting API**: Added Competitions, Countries, CurrentOrders, MarketTypes, TimeRanges, and Venues
- **Enhanced Endpoints**: Improved MarketBook, MarketCatalogue with better serialization support

#### Advanced Error Handling
- **29 Specific Exception Classes**: Distinct exceptions for each Betfair error code
- **Detailed Error Information**: Preserves requestUUID, errorCode, and errorDetails from Betfair
- **Smart Error Mapping**: Automatically detects Betting vs Account API errors

#### Fluent API Design
- **AccountStatementBuilder**: Build complex account statement queries with ease
- **MarketBookQuery**: Fluent interface for market book requests
- **ClearedOrdersQuery**: Simplified cleared orders filtering
- **PriceProjectionBuilder**: Easy price projection configuration

### 🏗️ **Architecture Improvements**

#### Complete Code Organization
- **Domain-Specific Folders**: APIs organized by functionality (Account, Betting)
- **Request/Response Separation**: Clear separation of request and response models
- **Centralized Enums**: All enums moved to dedicated folder structure

#### Enhanced Type System
- **New Enums**: MarketBettingType, MarketProjection, MarketSort, MarketStatus, OrderProjection, RunnerStatus
- **Rich Response Models**: KeyLineDescription, KeyLineSelection, Match, Matches, Order, Runner, TimeRange
- **Proper Serialization**: All models use init accessors for JSON compatibility

### 🧪 **Quality & Testing**

#### Massive Test Suite Expansion
- **1,300+ New Tests**: Comprehensive coverage of all functionality
- **Serialization Tests**: Every endpoint tested for JSON compatibility
- **Edge Case Coverage**: Null handling, error scenarios, boundary conditions
- **Future-Proof**: Reflection-based tests automatically discover new types

#### Code Quality
- **Zero Warnings**: Fixed all 186 build warnings
- **Enhanced Analysis**: Improved StyleCop, SonarQube, and Code Analysis compliance
- **Better Patterns**: TheoryData<T> usage, proper resource disposal

### 📚 **Developer Experience**

#### Documentation
- **XML Documentation**: Comprehensive docs for all public properties
- **API Specifications**: Based on official Betfair interface documentation
- **Better IntelliSense**: Rich tooltips and parameter guidance

#### Testability & Mocking
- **Virtual Methods**: Easy to mock and test in consuming applications
- **Public Properties**: Builder state accessible for testing
- **Clean Interfaces**: Simplified API surface for better usability

### ⚠️ **Breaking Changes**

This is a major version release with significant breaking changes:

1. **Namespace Changes**: Complete API reorganization requires updating using statements
2. **Response Objects**: Now use `init` accessors instead of internal setters
3. **Exception Types**: New specific exception classes replace generic exceptions
4. **Enum Serialization**: Standardized JSON converter usage across all enums
5. **MarketStatus**: Changed from response class to enum type

### 🔧 **Migration Guide**

#### Updating Namespaces
```csharp
// Old
using Betfair.Api.Responses;

// New
using Betfair.Api.Betting.ListMarketBook.Responses;
using Betfair.Api.Accounts.GetAccountFunds.Responses;
```

#### Response Object Changes
```csharp
// Old - internal setters
var response = new AccountFundsResponse { AvailableToBetBalance = 100 };

// New - init accessors
var response = new AccountFundsResponse { AvailableToBetBalance = 100 };
```

#### Exception Handling
```csharp
// Old - generic exceptions
catch (HttpRequestException ex)

// New - specific exceptions
catch (InvalidSessionInformationException ex)
catch (TooManyRequestsException ex)
```

### 📦 **Installation**

```bash
dotnet add package Betfair --version 9.0.0-alpha
```

### 🔗 **Resources**

- [GitHub Repository](https://github.com/KelvinVail/Betfair)
- [API Documentation](docs/)
- [Migration Guide](docs/Migration.md)

---

## 📋 Previous Releases

### Version 8.3.5 and Earlier
- Basic API functionality with limited endpoint coverage
- Foundation for the major architectural improvements in 9.0.0
- See git history for detailed changes in earlier versions
