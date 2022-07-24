# HBD.AzProxy.Core

## 1. AcquireTokenAsync with Client Secret

```csharp
var credentials = new AppCredentials("https://login.microsoftonline.com/[TenantId]", "ClientId", "ClientSecret");
var auth = new AuthContext(credentials);
var token = await auth.AcquireTokenAsync(/*Optionals Scope*/ new[] { "https://graph.microsoft.com/.default" });
```

## 2. AcquireTokenAsync with UserName and Password

```csharp
var credentials = new AppCredentials("https://login.microsoftonline.com/[TenantId]", "ClientId", "ClientSecret");
var userCredentials = new UserCredentials("UserName", "Password");
var auth = new AuthContext(credentials);
var token = await auth.AcquireTokenByUsernamePassword(userCredentials,/*Optionals Scope*/  new[] { "https://graph.microsoft.com/.default" });
```