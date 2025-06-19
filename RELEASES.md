# Releases

This document provides detailed release notes for the Betfair library.

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
