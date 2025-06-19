# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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
