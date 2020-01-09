using System;
using System.Data;
using System.Net;
using System.Threading.Tasks;
using SOAPDatasetHell;

namespace SoaToDataSetSample
{
    class Program
    {
        static void Main(string[] args)
        {

            MainAsync().Wait();
            
        }


        static async Task MainAsync()
        {
            SoapToDatasetClient soapClient = new SoapToDatasetClient("http://smsmont03/wsPortal/ws_TipoPrioridade.asmx");
            soapClient.Credentials = new NetworkCredential("portalservices", "57axWTN7=AS#3B");
            soapClient.NameSpaces.Add(@"xmlns:ran=""http://www.rangel_CGD.com/""");
            soapClient.SoapBody = GetSoapParams();
            DataSet dataSet = await soapClient.GetDataSet();

            foreach (DataTable table in dataSet.Tables)
            {
                for (int rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
                {
                    for (int colIndex = 0; colIndex < table.Columns.Count; colIndex++)
                    {
                        Console.WriteLine(table.Rows[rowIndex][colIndex]);
                    }
                }

                Console.ReadLine();
            }

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

    }
}
