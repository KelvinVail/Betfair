# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [9.0.1-alpha-5] - 2026-04-25

### Added
- **IHttpClientFactory support in BetfairApiClient**
  - New `BetfairApiClient(Credentials, IHttpClientFactory)` constructor allows consumers to supply their own HTTP pipeline via DI
  - Enables integration with `AddExtendedHttpClientLogging`, `AddHttpClientLatencyTelemetry`, and custom `DelegatingHandler` implementations without any changes to calling code
  - The factory manages the `HttpClient` lifetime; `BetfairApiClient` will not dispose it
  - The named client `"Betfair"` is resolved from the factory on construction
- **BetfairClientHandler made public**
  - Consumers can now register `BetfairClientHandler` as the primary handler when configuring a named client with `IHttpClientFactory`
  - Preserves the same transport settings (certificate revocation checking, GZip decompression, proxy disabled) used by the default constructor
  - Accepts an optional `X509Certificate2` for certificate-based authentication
- **HTTP client logging documentation**
  - New `docs/HttpClientLogging.md` covering basic factory setup, extended HTTP logging, phase-level latency telemetry, log consumption (console, Application Insights, Seq), and custom log enrichers
  - Includes full registration examples with correct ordering for `AddHttpClientLatencyTelemetry` and `AddExtendedHttpClientLogging`
  - Documents the `LatencyInfo` log field format and how to export phase timings as metrics via a custom `DelegatingHandler`

### Changed
- **Internal HTTP pipeline loosened**
  - `HttpDeserializer` and `TokenProvider` now accept `HttpClient` instead of `BetfairHttpClient`, enabling the factory-based path without duplicating pipeline logic
  - `BetfairHttpFactory` gains a second `Create` overload accepting `HttpClient`
- **Dependencies**
  - Added `Microsoft.Extensions.Http 9.0.0`

### Fixed
- **BetfairExceptionFactory duplicate switch expressions**
  - Each error code was previously matched in two separate switch expressions — once to create a default exception and read its message, then again to create the final exception. The first switch was immediately discarded.
  - Collapsed to a single switch per domain using `GetDefaultBettingMessage` / `GetDefaultAccountMessage` helpers. No behaviour change.

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
