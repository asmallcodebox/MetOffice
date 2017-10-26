using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MODataPointTests
{
    internal class WebContext
    {
        public RegisteredDataPointUser RegisteredUser { get; set; }
        public Query Query { get; set; }
        public WebRequest Request { get; set; }
        public WebResponse Response { get; set; }
        public QueryTimeResolutionType QueryTimeResolutionType { get; set; }
        public DateTime RequestedTime { get; set; }
        public XmlDocument XmlResponseDoc { get; set; }

        public void Dispose()
        {
            if(Response!=null)
                Response.Dispose();
        }
    }
}
