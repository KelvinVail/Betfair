# Authentication
The Credentials object is used to store all information needed to authenticate to Betfair. 
The same object can be reused by any client making a connection to Betfair.
The Credentials object has no publicly available methods. It is used exclusively by other internal processes in the library.

## Create a Credentials object

```csharp
var credentials = new Credentials([USERNAME], [PASSWORD], [APPKEY]);
```
It is recommended by Betfair that you use a certificate for non-interactive bot login.
[Learn more here](https://docs.developer.betfair.com/display/1smk3cen4v3lu3yomq5qye0ni/Non-Interactive+%28bot%29+login).

To use a certificate to login:
```csharp
var cert = X509Certificate2.CreateFrom...;
var credentials = new Credentials([USERNAME], [PASSWORD], [APPKEY], cert);
```

## Class
You can view the Credentials class [here](/src/Betfair/Core/Authentication/Credentials.cs)