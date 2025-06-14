# Account Management

Complete guide to managing your Betfair account through the API.

## Contents
- [Account Funds](#account-funds)
- [Account Details](#account-details)
- [Account Statement](#account-statement)
- [Currency Operations](#currency-operations)
- [Transfer Funds](#transfer-funds)
- [Profit and Loss](#profit-and-loss)
- [Best Practices](#best-practices)

## Account Funds

### Check Available Balance

```csharp
var funds = await client.AccountFunds();

Console.WriteLine($"Available to Bet: £{funds.AvailableToBetBalance:F2}");
Console.WriteLine($"Current Exposure: £{funds.Exposure:F2}");
Console.WriteLine($"Retained Commission: £{funds.RetainedCommission:F2}");
Console.WriteLine($"Exposure Limit: £{funds.ExposureLimit:F2}");
Console.WriteLine($"Discount Rate: {funds.DiscountRate:P2}");
Console.WriteLine($"Points Balance: {funds.PointsBalance}");
```

### Check Different Wallets

```csharp
// UK Wallet (default)
var ukFunds = await client.AccountFunds(Wallet.Uk);

// Australian Wallet
var auFunds = await client.AccountFunds(Wallet.Australian);

Console.WriteLine($"UK Balance: £{ukFunds.AvailableToBetBalance:F2}");
Console.WriteLine($"AU Balance: A${auFunds.AvailableToBetBalance:F2}");
```

### Monitor Balance Changes

```csharp
async Task MonitorBalance(CancellationToken cancellationToken)
{
    var lastBalance = 0.0;
    
    while (!cancellationToken.IsCancellationRequested)
    {
        var funds = await client.AccountFunds();
        
        if (Math.Abs(funds.AvailableToBetBalance - lastBalance) > 0.01)
        {
            Console.WriteLine($"Balance changed: £{funds.AvailableToBetBalance:F2}");
            lastBalance = funds.AvailableToBetBalance;
        }
        
        await Task.Delay(5000, cancellationToken); // Check every 5 seconds
    }
}
```

## Account Details

### Get Account Information

```csharp
var details = await client.AccountDetails();

Console.WriteLine($"Name: {details.FirstName} {details.LastName}");
Console.WriteLine($"Currency: {details.CurrencyCode}");
Console.WriteLine($"Locale: {details.LocaleCode}");
Console.WriteLine($"Region: {details.Region}");
Console.WriteLine($"Timezone: {details.Timezone}");
Console.WriteLine($"Country: {details.CountryCode}");
Console.WriteLine($"Discount Rate: {details.DiscountRate:P2}");
Console.WriteLine($"Points Balance: {details.PointsBalance}");
```

### Account Validation

```csharp
async Task<bool> ValidateAccount()
{
    try
    {
        var details = await client.AccountDetails();
        var funds = await client.AccountFunds();
        
        // Check if account is properly set up
        if (string.IsNullOrEmpty(details.CurrencyCode))
        {
            Console.WriteLine("Account currency not set");
            return false;
        }
        
        if (funds.AvailableToBetBalance <= 0)
        {
            Console.WriteLine("No funds available");
            return false;
        }
        
        Console.WriteLine("Account validation successful");
        return true;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Account validation failed: {ex.Message}");
        return false;
    }
}
```

## Account Statement

### Get Recent Transactions

```csharp
var query = new AccountStatementQuery()
    .LastWeek()
    .ExchangeOnly()
    .UkWallet()
    .Take(100);

var statement = await client.AccountStatement(query);

foreach (var item in statement.AccountStatement ?? [])
{
    Console.WriteLine($"Date: {item.ItemDate:yyyy-MM-dd HH:mm}");
    Console.WriteLine($"Amount: £{item.Amount:F2}");
    Console.WriteLine($"Balance: £{item.Balance:F2}");
    
    if (item.LegacyData != null)
    {
        Console.WriteLine($"Market: {item.LegacyData.FullMarketName}");
        Console.WriteLine($"Selection: {item.LegacyData.SelectionName}");
        Console.WriteLine($"Bet Type: {item.LegacyData.BetType}");
        Console.WriteLine($"Transaction: {item.LegacyData.TransactionType}");
    }
    
    Console.WriteLine();
}
```

### Filter by Date Range

```csharp
var query = new AccountStatementQuery()
    .WithDateRange(
        DateTimeOffset.UtcNow.AddDays(-30),
        DateTimeOffset.UtcNow)
    .ExchangeOnly()
    .Take(500);

var statement = await client.AccountStatement(query);
```

### Filter by Transaction Type

```csharp
// Exchange transactions only
var exchangeQuery = new AccountStatementQuery().ExchangeOnly();

// Poker room transactions only
var pokerQuery = new AccountStatementQuery().PokerRoomOnly();

// All transactions
var allQuery = new AccountStatementQuery().AllTransactions();
```

### Calculate Daily P&L

```csharp
async Task<Dictionary<DateTime, double>> CalculateDailyPnL(int days = 30)
{
    var query = new AccountStatementQuery()
        .WithDateRange(
            DateTimeOffset.UtcNow.AddDays(-days),
            DateTimeOffset.UtcNow)
        .ExchangeOnly()
        .Take(1000);

    var statement = await client.AccountStatement(query);
    var dailyPnL = new Dictionary<DateTime, double>();

    foreach (var item in statement.AccountStatement ?? [])
    {
        var date = item.ItemDate.Date;
        
        if (!dailyPnL.ContainsKey(date))
            dailyPnL[date] = 0;
            
        // Only count betting-related transactions
        if (item.LegacyData?.TransactionType?.Contains("RESULT") == true)
        {
            dailyPnL[date] += item.Amount;
        }
    }

    return dailyPnL;
}
```

## Currency Operations

### Get Currency Rates

```csharp
var rates = await client.CurrencyRates("GBP");

foreach (var rate in rates)
{
    Console.WriteLine($"{rate.CurrencyCode}: {rate.Rate:F4}");
}
```

### Convert Currency

```csharp
async Task<double> ConvertCurrency(double amount, string fromCurrency, string toCurrency)
{
    var rates = await client.CurrencyRates(fromCurrency);
    var targetRate = rates.FirstOrDefault(r => r.CurrencyCode == toCurrency);
    
    if (targetRate == null)
        throw new ArgumentException($"Currency {toCurrency} not found");
    
    return amount * targetRate.Rate;
}

// Example usage
var gbpAmount = 100.0;
var eurAmount = await ConvertCurrency(gbpAmount, "GBP", "EUR");
Console.WriteLine($"£{gbpAmount} = €{eurAmount:F2}");
```

## Transfer Funds

### Transfer Between Wallets

```csharp
// Transfer £100 from UK to Australian wallet
var transfer = await client.TransferFunds(
    from: Wallet.Uk,
    toWallet: Wallet.Australian,
    amount: 100.0);

if (transfer.TransactionId != null)
{
    Console.WriteLine($"Transfer successful: {transfer.TransactionId}");
}
else
{
    Console.WriteLine("Transfer failed");
}
```

### Automated Fund Management

```csharp
async Task MaintainMinimumBalance(double minimumBalance = 100.0)
{
    var ukFunds = await client.AccountFunds(Wallet.Uk);
    var auFunds = await client.AccountFunds(Wallet.Australian);
    
    if (ukFunds.AvailableToBetBalance < minimumBalance && 
        auFunds.AvailableToBetBalance > minimumBalance * 2)
    {
        var transferAmount = minimumBalance - ukFunds.AvailableToBetBalance;
        
        var transfer = await client.TransferFunds(
            Wallet.Australian,
            Wallet.Uk,
            transferAmount);
            
        Console.WriteLine($"Transferred £{transferAmount:F2} from AU to UK wallet");
    }
}
```

## Profit and Loss

### Market P&L

```csharp
var marketIds = new List<string> { "1.123456789", "1.987654321" };

var pnl = await client.MarketProfitAndLoss(
    marketIds,
    includeSettledBets: true,
    includeBspBets: true,
    netOfCommission: true);

foreach (var market in pnl)
{
    Console.WriteLine($"Market: {market.MarketId}");
    Console.WriteLine($"Commission Applied: £{market.CommissionApplied:F2}");
    
    foreach (var runner in market.ProfitAndLosses ?? [])
    {
        Console.WriteLine($"  Selection {runner.SelectionId}: ");
        Console.WriteLine($"    If Wins: £{runner.IfWin:F2}");
        Console.WriteLine($"    If Loses: £{runner.IfLose:F2}");
        Console.WriteLine($"    If Places: £{runner.IfPlace:F2}");
    }
    Console.WriteLine();
}
```

### Calculate Total Exposure

```csharp
async Task<double> CalculateTotalExposure(List<string> marketIds)
{
    var pnl = await client.MarketProfitAndLoss(marketIds);
    var totalExposure = 0.0;
    
    foreach (var market in pnl)
    {
        foreach (var runner in market.ProfitAndLosses ?? [])
        {
            // Worst case scenario for this runner
            var worstCase = Math.Min(runner.IfWin ?? 0, runner.IfLose ?? 0);
            if (worstCase < 0)
            {
                totalExposure += Math.Abs(worstCase);
            }
        }
    }
    
    return totalExposure;
}
```

## Best Practices

### 1. Regular Balance Monitoring

```csharp
async Task MonitorAccountHealth()
{
    var funds = await client.AccountFunds();
    var exposureRatio = funds.Exposure / funds.ExposureLimit;
    
    if (exposureRatio > 0.8)
    {
        Console.WriteLine("WARNING: High exposure ratio");
    }
    
    if (funds.AvailableToBetBalance < 50.0)
    {
        Console.WriteLine("WARNING: Low balance");
    }
}
```

### 2. Transaction Logging

```csharp
async Task LogAccountActivity()
{
    var query = new AccountStatementQuery()
        .Today()
        .ExchangeOnly()
        .Take(100);
        
    var statement = await client.AccountStatement(query);
    
    foreach (var item in statement.AccountStatement ?? [])
    {
        // Log to your preferred logging system
        Console.WriteLine($"[{item.ItemDate}] {item.Amount:F2} - {item.LegacyData?.TransactionType}");
    }
}
```

### 3. Error Handling

```csharp
async Task<AccountFundsResponse?> GetAccountFundsSafely()
{
    try
    {
        return await client.AccountFunds();
    }
    catch (HttpRequestException ex)
    {
        Console.WriteLine($"Failed to get account funds: {ex.Message}");
        return null;
    }
}
```

### 4. Rate Limiting

```csharp
private readonly SemaphoreSlim _accountSemaphore = new(1, 1);

async Task<AccountFundsResponse> GetAccountFundsThrottled()
{
    await _accountSemaphore.WaitAsync();
    try
    {
        await Task.Delay(100); // Minimum delay between calls
        return await client.AccountFunds();
    }
    finally
    {
        _accountSemaphore.Release();
    }
}
```

### 5. Caching Account Data

```csharp
private AccountDetailsResponse? _cachedDetails;
private DateTime _detailsCacheTime;

async Task<AccountDetailsResponse> GetAccountDetailsWithCache()
{
    if (_cachedDetails == null || 
        DateTime.UtcNow - _detailsCacheTime > TimeSpan.FromHours(1))
    {
        _cachedDetails = await client.AccountDetails();
        _detailsCacheTime = DateTime.UtcNow;
    }
    
    return _cachedDetails;
}
```
