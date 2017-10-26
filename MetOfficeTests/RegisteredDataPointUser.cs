using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODataPointTests
{
    internal class RegisteredDataPointUser
    {
        private string accessKey;
        public string AccessKey { get { return accessKey; } }
        public RegisteredDataPointUser()
        {
            ObtainAccessKey();
        }
        private void ObtainAccessKey()
        {
            accessKey = ConfigurationManager.AppSettings["AccessKey"];
            if(accessKey.Trim() == "")
                throw new Exception("Error obtaining AccessKey from the App.config file." +
                    "Please make sure your Met Office DataPoint API key is provided in AccessKey variable in the App.config file.");
        }
    }
}
