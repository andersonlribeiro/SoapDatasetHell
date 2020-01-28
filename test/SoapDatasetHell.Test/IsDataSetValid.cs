using NUnit.Framework;
using SOAPDatasetHell;
using System.Xml.Linq;
using System.IO;
using System.Data;

namespace SoapDatasetHellTest
{
    public class Tests
    {

        SoapToDataSet soapClient;
        XElement diffGram;
        
        [SetUp]
        public void Setup()
        {
            soapClient = new SoapToDataSet(new System.Uri("https://www.foo.bar/service.asmx"));
            soapClient.NameSpaces.Add(@"xmlns:myservice=""http://www.foo.bar/""");
            soapClient.SetSoapBody(GetSoapParams());

            string fileContent = File.ReadAllText("DiffGram.xml");
            diffGram = XElement.Parse(fileContent);
            string GetSoapParams()
            {
                return $@"
						<ran:WS_TipoPrioridade_Call>
						 <ran:in_Param>
							<ran:IntegrationSystem>2</ran:IntegrationSystem>
						 </ran:in_Param>
					  </ran:WS_TipoPrioridade_Call>";
            }

        }

        [Test]
        public void HasDataSet()
        {
            DataSet ds  =  this.soapClient.ExtractDataSet(this.diffGram);
            Assert.GreaterOrEqual(ds.Tables.Count, 1);
        }


        [Test]
        public void HasRows()
        {
            DataSet ds = this.soapClient.ExtractDataSet(this.diffGram);
            Assert.GreaterOrEqual(ds.Tables[0].Rows.Count, 1);
        }
    }
}