
# SoapDatasetHell

Software engineers used to share datasets throught WCF Webservices. Nowadays, it's not a good practice, but, there are many serivces written in this way.

In .Net Core, there isn't an easy way to read these legacy datasets.

This lib get XML result and converts it into a DataSet object.


### Nuget

| | |
|-|-|
| downloads | ![](https://img.shields.io/nuget/dt/dev.andersonribeiro.SOAPDatasetHell) |
| stable | [![Nuget](https://img.shields.io/nuget/v/dev.andersonribeiro.SOAPDatasetHell)](https://www.nuget.org/packages/dev.andersonribeiro.SOAPDatasetHell) |

### Get help

[![Gitter](https://badges.gitter.im/SoapDatasetHell/community.svg)](https://gitter.im/SoapDatasetHell/community?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)

 
Follow me on Twitter: [@andersonribeiro][1]

  [1]: https://twitter.com/andersonribeiro

# Install with nuget

```
Install-Package dev.andersonribeiro.SOAPDatasetHell -Version 1.0.0
```

# Install with .NET CLI
```
dotnet add package dev.andersonribeiro.SOAPDatasetHell --version 1.0.0
```

# How to use

Create an instance of SoapToDatasetClient and set properties:

    * Credentials;
    * SOAP Endpoint URL;
    * SOAP Namespaces;
    * SOAP Body;

After, call SoapToDatasetClient.GetDataSet() to retrieve Dataset from WCF Webservice.

Something like this:

```csharp
SoapToDatasetClient soapClient = new SoapToDatasetClient("http://wssupplier/wsPortal/ws_listPriority.asmx");
soapClient.Credentials = new NetworkCredential("username", "password");
soapClient.NameSpaces.Add(@"xmlns:ran=""http://www.ranger.com/""");
soapClient.SoapBody = $@"
<ran:WS_Priority_Call>
<ran:in_Param>
<ran:IntegrationSystem>2</ran:IntegrationSystem>
</ran:in_Param>
</ran:WS_WS_Priority_Call>"
DataSet dataSet = await soapClient.GetDataSet();
```
