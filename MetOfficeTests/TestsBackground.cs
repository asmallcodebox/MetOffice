using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Configuration;

namespace MODataPointTests
{ 
    public partial class LocationSpecificDataAccess
    {
        private WebAccess webAccess;
        private DataProcessing dataProcessing;
        private XmlDocument xmlInputDoc;
        private string inputDataFullFileName;
        private double uploadWaitTime;

        private static string baseUrl;
        private static string dataType;
        private static Dictionary<string, string> resources;
        internal static string BaseUrl { get { return baseUrl; } }
        internal static string DataType { get { return dataType; } }
        internal static Dictionary<string, string> Resources { get { return resources; } }

        internal void InitializeTest()
        {
            dataProcessing = new DataProcessing();
            resources = new Dictionary<string, string>();
            inputDataFullFileName = ConfigurationManager.AppSettings["InputFile"];
            LoadInputData(inputDataFullFileName);
            webAccess = new WebAccess();
            dataProcessing = new DataProcessing();
        }

        private void LoadInputData(string inputDataFullFileName)
        {
            xmlInputDoc = new XmlDocument();
            xmlInputDoc.Load(inputDataFullFileName);
            baseUrl = xmlInputDoc.SelectSingleNode("//Request/BaseUrl").InnerText.TrimEnd(new char[] { '/' });
            dataType = xmlInputDoc.SelectSingleNode("//Request/DataType").InnerText.Trim();
            XmlNodeList resourcesNodes = xmlInputDoc.SelectSingleNode("//Request/Resources").ChildNodes;
            foreach (XmlNode node in resourcesNodes)
                Resources.Add(node.Name, node.InnerText.Trim());
            uploadWaitTime = Double.Parse(xmlInputDoc.SelectSingleNode("//UploadWaitTime").InnerText.Trim());
        }
    }
}
