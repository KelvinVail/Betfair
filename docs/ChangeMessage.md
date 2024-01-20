# Change Message
The change message is the parent class used for all messages received from Betfair. The key property that determines the change type is the ```Operation``` value.  

Possible ```Operation``` values are:
- connection - [See Connection Message](#connection-message)
- status - [See Status Message](#status-message)
- mcm - [See Market Change Message](#market-change-message)
- ocm

 ## Connection Message
This is received when a successful connection is made to the Betfair server. It is the first message you will receive in your stream.  
The only populated properties on the change message will be ```Operation``` and ```ConnectionId```.  

```Operation``` will be "connection".  
```ConnectionId``` will hold a unique identifier, e.g. "002-230915140112-174". If you ever need to contact Betfair support about a connection issue you will need to supply this unique ConnectionId to them.

## Status Message
Every message sent to Betfair will receive a status message as a response. You will also receive a status message if Betfair closes your connection.  
The only properties that will always be populated on the status message will be ```Operation``` and ```StatusCode```.  Additional properties may be populated depending on the request made or if the connection has been closed by Betfair. See below for details.

```Operation``` will be "status".  
```StatusCode``` will be either "SUCCESS" or "FAILURE".  
```Id``` will appear on most responses and is an incrementing integer used to link request and response messages together. i.e. the first message you send to Betfair will receive responses with an Id of 1, the second 2 and so on.

### Authentication response
This will be the first status message you receive from Betfair, indicating if you have successfully authenticated or not.

```Operation``` will be "status".  
```StatusCode``` will be either "SUCCESS" or "FAILURE".  
```Id``` will be 1. An authentication request is always the first message that must be sent after connecting to Betfair so will always be 1.
```ConnectionClosed``` will be either true or false. False if the connection is open or true if it is closed.  
```ConnectionsAvailable``` an integer indicating the number of additional connections you can open. These are limited globally by Betfair, not by the ```StreamClient```.

### Authentication failures
If ```StatusCode``` is "FAILURE" you will receive the properties ```ErrorCode``` and ```ErrorMessage```.  

ErrorCode                     | Description
---------                     | -----------
NOT_AUTHORIZED                | Returned when you are not authorized to perform the operation.
MAX_CONNECTION_LIMIT_EXCEEDED | Returned when you try to create more connections than allowed to.
TOO_MANY_REQUESTS             | Returned when you make too many requests within a short time period.

### Subscription response 
You will receive a separate status message on the stream for each call you make to either ```StreamClient.Subscribe()``` or ```StreamClient.SubscribeToOrders()```.  

```Operation``` will be "status".  
```StatusCode``` will be either "SUCCESS" or "FAILURE".  
```Id``` starts at 2, then increments by 1 each time you have called either ```StreamClient.Subscribe()``` or ```StreamClient.SubscribeToOrders()```. Responses to the first of these method calls will receive and Id of 2 the next 3 and so on.  
```ConnectionClosed``` will be either true or false. False if the connection is open or true if it is closed.  

### Subscription failures
If ```StatusCode``` is "FAILURE" you will receive the properties ```ErrorCode``` and ```ErrorMessage```.  

ErrorCode                   | Description
---------                   | -----------
SUBSCRIPTION_LIMIT_EXCEEDED | Returned when attempting to subscribe to more markets 200 markets.

### Timeout response
If you fail to call either ```StreamClient.Subscribe()``` or ```StreamClient.SubscribeToOrders()``` with 15 seconds of creating the ```StreamClient``` you will receive a timeout response from Betfair.  

```Operation``` will be "status".  
```StatusCode``` will be "FAILURE".  
```ConnectionClose``` will be "true".
```ConnectionId``` will be the unique ConnectionId of the connection that has timed out.  
```ErrorCode``` will be "TIMEOUT".  
```ErrorMessage``` will be "Connection is not subscribed and is idle: 15000 ms".

## Market Change Message