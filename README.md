
Create an instance of SoapToDatasetClient and set properties:

    Credentials, if aplicable:
    SOAP Endpoint URL
    SOAP Namespaces ( Mandatory )
    SOAP Body ( Mandatory )

After, call SoapToDatasetClient.GetDataSet() to retrieve Dataset from WCF Webservice.

Something like this:

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
