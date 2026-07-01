# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [9.0.1-alpha-5] - 2025-07-01

### Added
- **Ultra-Low-Latency MarketCache**
  - New `MarketCacheProcessor` for zero-allocation stream processing using `Utf8JsonReader`
  - `MarketCache`, `RunnerCache`, `PriceLadder`, and `PositionLadder` types for in-memory market state
  - `MarketDefinitionCache` and `RunnerDefinitionCache` for mutable market definition tracking
  - First-byte dispatch pattern for minimal branching on hot paths
  - Deferred ladder update buffer to batch runner price/size updates
- **Raw Stream Access**
  - Added `ReadRawLines` method to `Subscription` for direct raw byte stream access
  - Enables custom zero-copy processing pipelines outside the built-in deserializer
- **Benchmarks**
  - Added `MarketStreamPipelineBenchmarks` for end-to-end stream processing measurement
  - Added `ChangeMessageReader` benchmark for JSON parsing comparison
  - Added `QuickTimer` and `SanityCheck` utilities
  - Added Python benchmark for cross-language performance comparison
  - Included 8,000+ line `MarketStream.txt` sample data file
- **Documentation**
  - New `docs/MarketCache.md` covering the low-latency cache architecture
  - New `docs/BetfairApiClient.md` with comprehensive API client documentation
  - Updated `docs/Subscription.md`, `docs/Authentication.md`, and other docs
- **Test Coverage**
  - Added `MarketCacheProcessorTests` (504 lines), `MarketCacheTests`, `MarketDefinitionCacheTests`
  - Added `PriceLadderTests`, `PositionLadderTests`, `RunnerCacheTests`
  - Added `ClearedOrdersQueryTests`, `BetfairExceptionFactoryTests`, `MarketFilterAdditionalTests`
  - Increased overall test coverage to 90%
  - Total test count: 1,684 tests (all passing)
- **Mutation Testing**
  - Added Stryker mutation testing workflow with API key integration
  - Optimized mutation testing to run only on changed files via `--since`
- **SonarCloud Analyzer Parity**
  - Added Sonar rule configuration to `.editorconfig` (S3776, S1144, S6602, S6603, S6608, S6610, S6618, S6640, and others)
  - Local builds now surface the same issues SonarCloud reports

### Changed
- **Performance Optimizations**
  - `MarketCacheProcessor` uses `ReadOnlySpan<byte>` property names with `ValueTextEquals` for zero-allocation property matching
  - Cached UTF-8 bytes in `MarketDefinitionCache` to avoid repeated string allocations on unchanged values
  - `Pipeline` enhanced to support raw line callbacks via `ReadOnlySpanAction`
- **Session Token Refresh**
  - Token refresh now triggers on exception type (`InvalidSessionInformationException`) instead of exact message match
  - More robust against Betfair API message wording changes
- **Refactored for Cognitive Complexity**
  - `IdleWatchdogExtensions.RunWatchdogAsync` decomposed into focused helper methods
  - `MarketCacheProcessor.Process`, `ReadSingleMarketChange`, `ReadSingleRunnerChange` extracted into dispatch methods
  - `MarketDefinitionCache.ReadFrom` split into grouped property readers
- **Code Quality**
  - Replaced `.Be(typeof(T))` with generic `.Be<T>()` across test files (9 occurrences)
  - Replaced `.BeAssignableTo(typeof(T))` with generic overload
  - Used `string.Empty` instead of `""` literal
  - Extracted repeated inline arrays to `static readonly` fields
  - Replaced `FirstOrDefault` with `Array.Find` on array types
  - Removed unused private properties from `MarketCacheProcessor`
  - Fixed SA1204 member ordering in `MarketCacheProcessor` and `MarketDefinitionCache`
  - Removed duplicate switch expressions in `BetfairExceptionFactory`
- **Dependencies Updated**
  - `SonarAnalyzer.CSharp` 10.21.0 → 10.27.0
  - `System.IO.Pipelines` 10.0.5 → 10.0.9
  - `DotNet.ReproducibleBuilds` 2.0.2 → 2.0.5
  - Test packages updated (xunit, FluentAssertions, coverlet, etc.)
- **CI/CD**
  - Optimized mutation testing workflow (Stryker `--since` with branch refs)
  - Fixed security warnings in quality pipeline

### Fixed
- **MarketCache img/rc ordering bug** — image flag now correctly clears runners regardless of property order in JSON
- **Race condition in IdleWatchdog** — missed stall detection when watchdog check and message arrival interleaved
- **Flaky watchdog test** — widened timing margins for CI reliability
- **Build warnings** — resolved all warnings without disabling analyzers
- **`SnakeCaseEnumJsonConverter`** — added to `LadderType` enum for proper serialization

## [9.0.1-alpha-3] - 2025-08-27

### Fixed
- **Critical Stream Reconnection Issue**
  - Fixed ObjectDisposedException when stream reconnection attempts to use disposed socket
  - Changed _tcpClient from readonly to mutable field to allow replacement during reconnection
  - ReconnectAndResubscribe now disposes old TcpClient and creates new instance for each reconnection
  - Prevents "Cannot access a disposed object. Object name: 'System.Net.Sockets.Socket'" error
  - Ensures robust and reliable streaming connections with multiple reconnection attempts
- **Test Coverage**
  - Added test to verify _tcpClient field mutability enabling reconnection fix
  - Total test count increased to 2890 tests (all passing)

## [9.0.1-alpha-2] - 2025-08-21

### Added
- **Comprehensive Subscription Retry Logic Tests**
  - Added 18 unit tests covering all retry scenarios in Subscription class
  - Tests verify two-tier retry system behavior:
    * When reader not active: Direct message writing, no retry logic
    * When reader active: In-band status waiting with retry logic
  - Coverage includes edge cases: status failures, wrong IDs, null responses, timeouts
  - Added PipelineStubWithRetry test double for reliable retry behavior mocking
- **Stream Enhancements**
  - Added StrategyMatchedData class for dynamic strategy deserialization in OrderRunnerChange
  - Enhanced OrderRunnerChange.StrategyMatchedChange to use Dictionary<string, StrategyMatchedData>
  - Added StrategyMatchedData to SerializerContext for proper JSON serialization support
- **Stream Reconnection Improvements**
  - Simplified reconnection logic with explicit ownership flag
  - Consolidated last-subscription state management
  - Added IdleWatchdogExtensions for connection monitoring
  - Enhanced StyleCop compliance and code organization

### Changed
- **Test Suite Expansion**
  - Total test count increased to 2890 tests (all passing)
  - Enhanced test coverage for stream functionality
  - Improved test reliability and maintainability
- **Code Quality**
  - Improved StyleCop ordering and compliance
  - Enhanced code organization and documentation

### Fixed
- **Stream Reliability**
  - Improved connection stability and retry mechanisms
  - Better handling of edge cases in subscription management
  - Enhanced error recovery in stream operations

## [9.0.1-alpha] - 2025-06-19

### Added
- **Multi-Framework Targeting**
  - Added support for .NET 9.0 alongside existing .NET 8.0 support
  - Library now targets both `net8.0` and `net9.0` frameworks
  - Test project updated to run on both target frameworks
- **NuGet Package Enhancements**
  - Added README.md to NuGet package for better discoverability
  - Package now displays full documentation, installation instructions, and code examples on nuget.org
  - Improved developer experience with comprehensive package information

### Changed
- **Build Configuration**
  - Suppressed IL2026 warnings from generated JSON serialization code
  - These warnings are expected when using AOT compilation with System.Text.Json
  - Maintained AOT compatibility while ensuring clean builds

### Fixed
- **Build Warnings**
  - Eliminated IL2026 warnings related to `Exception.TargetSite` usage in generated code
  - Clean builds with no warnings on both .NET 8.0 and .NET 9.0 frameworks

## [9.0.0-alpha] - 2025-06-19

### Added
- **Complete Betfair API Coverage**
  - Account API endpoints: AccountDetails, AccountFunds, AccountStatement, CurrencyRates
  - Betting API endpoints: Competitions, Countries, CurrentOrders, MarketTypes, TimeRanges, Venues
  - Enhanced existing endpoints: MarketBook, MarketCatalogue, and others
- **Advanced Error Handling System**
  - 29 distinct exception classes (16 Betting + 13 Account API exceptions)
  - Proper error code mapping with descriptive error messages
  - Preservation of Betfair error details (requestUUID, errorCode, errorDetails)
- **Fluent API Builders**
  - `AccountStatementBuilder` for account statement queries
  - `MarketBookQuery` for market book requests
  - `ClearedOrdersQuery` for cleared orders
  - `PriceProjectionBuilder` for price projections
- **Enhanced Type System**
  - New enums: MarketBettingType, MarketProjection, MarketSort, MarketStatus, OrderProjection, RunnerStatus
  - New response models: KeyLineDescription, KeyLineSelection, Match, Matches, Order, Runner, TimeRange
- **Comprehensive Test Suite**
  - 1,300+ new tests covering all API endpoints
  - Request/response serialization tests
  - Edge cases, error conditions, and real-world usage patterns
  - Reflection-based tests for future-proof coverage
- **XML Documentation**
  - Comprehensive XML docs for all public properties using official Betfair API specifications

### Changed
- **Complete API Structure Reorganization**
  - Separated Requests and Responses into dedicated subfolders
  - Moved all enums into centralized Enums folder
  - Organized filters into logical groupings
- **Enhanced JSON Serialization**
  - Response objects now use init accessors instead of internal setters
  - Fixed MarketBook array deserialization
  - Comprehensive enum JSON converter validation
- **Code Quality Improvements**
  - Fixed all 186 build warnings → 0 warnings
  - Enhanced StyleCop compliance and code analysis
  - Improved SonarQube compliance with TheoryData<T> usage
- **Package Updates**
  - Updated all dependencies to latest versions
  - Fixed cross-platform compatibility issues

### Removed
- **Experimental Classes Cleanup**
  - Removed experimental BetfairStreamDeserializer and related classes
  - Cleaned up unused deserializer components
  - Removed 2,000+ lines of experimental code

### Breaking Changes
- **API Structure**: Complete namespace reorganization requires updating using statements
- **Response Objects**: Now use init accessors instead of internal setters
- **Exception Hierarchy**: Completely redesigned with specific exception classes
- **Enum Serialization**: Standardized across all types with proper JSON converters
- **MarketStatus**: Changed from response class to enum type

### Fixed
- MarketBook array deserialization issues
- GitHub Actions analyzer compatibility in Linux environments
- Cross-platform build issues with Microsoft.CodeAnalysis.Analyzers

## [8.3.5] - Previous Release
- Last stable release before major architectural changes
- Basic API functionality with limited endpoint coverage
