#region License
// Copyright (c) 2019 Anderson Ribeiro
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SOAPDatasetHell
{
    public class SoapToDataSet
    {

        public Uri SoapEndpoint { get; private set; }
        public List<string> NameSpaces { get; private set; }
        public string SoapBody { get; private set; }
        public NetworkCredential Credentials { get; private set; } = null;

        public SoapToDataSet()
        {
            this.NameSpaces = new List<string>();
        }


        public SoapToDataSet(Uri soapEndpoint) : this() => SoapEndpoint = soapEndpoint;
       
        
        public void AddNamespace(string soapNamespace)
        {
            if (NameSpaces.IndexOf(soapNamespace) > -1) throw new ArgumentException("Namespace already exists ");
            NameSpaces.Add(soapNamespace);
        }

        public void AddNamespaces(IList<string> soapNamespaces)
        {
            if (soapNamespaces == null || soapNamespaces.Count == 0 ) throw new ArgumentNullException($"Invalid argument  {nameof(soapNamespaces)}");
            NameSpaces.AddRange(soapNamespaces);
        }

        public void RemoveNamespace(string soapNamespace)
        {
            if (NameSpaces.IndexOf(soapNamespace) > -1) throw new ArgumentException("Namespace not exists ");
            NameSpaces.Remove(soapNamespace);
        }

        public void SetCredential(NetworkCredential credential) => Credentials = credential;
     
        public void SetSoapEndpoint(Uri soapEndpoint) => SoapEndpoint = soapEndpoint;
               
        public void SetSoapBody(string soapBody) => SoapBody = soapBody;
      
        XmlDocument createSoapEnvelope()
        {
            StringBuilder xmlContent = new StringBuilder();
            xmlContent.AppendLine(@"<?xml version=""1.0""?>");
            xmlContent.AppendLine(@"<soap:Envelope");
            xmlContent.AppendLine(@"xmlns:soap=""http://www.w3.org/2003/05/soap-envelope""");

            foreach (var nameSpace in NameSpaces)
            {
                xmlContent.AppendLine(nameSpace);
            }
            xmlContent.Append(@">");
            xmlContent.AppendLine(@"<soap:Header>");
            xmlContent.AppendLine(@"</soap:Header>");
            xmlContent.AppendLine(@"<soap:Body>");
            xmlContent.AppendLine(Regex.Replace(this.SoapBody, @"\s+", ""));
            xmlContent.AppendLine(@"</soap:Body>");
            xmlContent.AppendLine(@"</soap:Envelope>");

            XmlDocument soapEnvelopXml = new XmlDocument();
            soapEnvelopXml.LoadXml(xmlContent.ToString());
            return soapEnvelopXml;
        }

       
        HttpWebRequest CreateWebRequest()
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(SoapEndpoint);

            webRequest.ContentType = "text/xml; encoding='utf-8'";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";

            if (this.Credentials != null)
            {
                webRequest.Credentials = this.Credentials;
            }

            return webRequest;
        }

        async Task InsertSoapEnvelopIntoWebRequestAsync(XmlDocument envelop, WebRequest webRequest)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(envelop.OuterXml);
            webRequest.ContentLength = bytes.Length;
            using (Stream stream = await webRequest.GetRequestStreamAsync())
            {
                await stream.WriteAsync(bytes, 0, bytes.Length);
            }
        }

        void InsertSoapEnvelopIntoWebRequest(XmlDocument envelop, WebRequest webRequest)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(envelop.OuterXml);
            webRequest.ContentLength = bytes.Length;
            using (Stream stream = webRequest.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }
        }


        /// <summary>
        /// Make a request to WCF SOAP URL in asynchronous way.
        /// </summary>
        /// <returns>A XElement</returns>
        public async Task<XElement> GetSoapResponseAsync()
        {
            XmlDocument soapEnvelopXml = createSoapEnvelope();
            HttpWebRequest webRequest = CreateWebRequest();
            await InsertSoapEnvelopIntoWebRequestAsync(soapEnvelopXml, webRequest);
            string soapResult = string.Empty;
            using (HttpWebResponse response = (HttpWebResponse)await webRequest.GetResponseAsync())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                    {
                        soapResult = await rd.ReadToEndAsync();
                    }
                }
            }

            XElement document = XElement.Load(new StringReader(soapResult));
            return document;
        }

        /// <summary>
        /// Make a request to WCF SOAP URL in synchronous way.
        /// </summary>
        /// <returns>A XElement</returns>
        public XElement GetSoapResponse()
        {
            XmlDocument soapEnvelopXml = createSoapEnvelope();
            HttpWebRequest webRequest = CreateWebRequest();
            InsertSoapEnvelopIntoWebRequest(soapEnvelopXml, webRequest);
            string soapResult = string.Empty;
            using (HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                    {
                        soapResult = rd.ReadToEnd();
                    }
                }
            }

            XElement document = XElement.Load(new StringReader(soapResult));
            return document;
        }

        /// <summary>
        /// Converts a Diffgram into a DataSet
        /// </summary>
        /// <param name="diffgram">A Diffgram: DataSet schema and data represented in XML.</param>
        /// <returns>A System.Data.DataSet</returns>
        public DataSet ExtractDataSet(XElement diffgram)
        {
            DataSet dataSet = new DataSet();
            dataSet.ReadXml(new StringReader(diffgram.ToString()), XmlReadMode.ReadSchema);
            dataSet.AcceptChanges();
            return dataSet;
        }


    }


}
