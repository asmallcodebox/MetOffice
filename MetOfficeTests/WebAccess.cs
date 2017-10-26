using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MODataPointTests
{
    internal class WebAccess
    {
        internal WebRequest CreateRequest(WebContext webContext)
        {
            try
            {
                WebRequest request = WebRequest.Create(CreateStrRequest(webContext));
                request.Method = WebRequestMethods.Http.Get;
                return request;
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating a web request.", ex);
            }
        }

        private string CreateStrRequest(WebContext webContext)
        {
            string rsrcs = "";
            string locationID = "";
            string resolution = "";
            string time = "";

            if (webContext.Query is ObservationsQuery)
            {
                rsrcs = LocationSpecificDataAccess.Resources["Obs24H"];
                locationID = ((ObservationsQuery)webContext.Query).LocationID;
                resolution = ((ObservationsQuery)webContext.Query).TimeRes.ToString().ToLower();
                time = CustomConversions.ToDateTimeQueryString(((ObservationsQuery)webContext.Query).DateTime);
            }
            else if (webContext.Query is ForecastQuery)
            {
                rsrcs = LocationSpecificDataAccess.Resources["Fcs5D"];
                locationID = ((ForecastQuery)webContext.Query).LocationID;
                FcsTimeRes timeRes = ((ForecastQuery)webContext.Query).TimeRes;
                resolution = timeRes == FcsTimeRes.ThreeHourly ? timeRes.ToString().Replace("Three", "3").ToLower()
                    : timeRes.ToString().ToLower();
                time = CustomConversions.ToDateTimeQueryString(((ForecastQuery)webContext.Query).DateTime);
            }
            else if (webContext.Query is ObservationsCapabilitiesQuery)
            {
                rsrcs = LocationSpecificDataAccess.Resources["ObsCpbs"];
                resolution = ((ObservationsCapabilitiesQuery)webContext.Query).TimeRes.ToString().ToLower();
            }
            else if (webContext.Query is ForecastCapabilitiesQuery)
            {
                rsrcs = LocationSpecificDataAccess.Resources["FcsCpbs"];
                FcsTimeRes timeRes = ((ForecastCapabilitiesQuery)webContext.Query).TimeRes;
                resolution = timeRes == FcsTimeRes.ThreeHourly ? timeRes.ToString().Replace("Three", "3").ToLower()
                    : timeRes.ToString().ToLower();
            }
            else
                throw new Exception("Not recognized query type.");

            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(LocationSpecificDataAccess.BaseUrl);
            strBuilder.Append("/" + rsrcs.Replace("datatype", LocationSpecificDataAccess.DataType.ToString().ToLower()));
            if (locationID != "")
                strBuilder.Append("/" + locationID);
            strBuilder.Append("?res=" + resolution);
            if (time != "")
                strBuilder.Append(("&time=" + time));
            strBuilder.Append("&key=");
            strBuilder.Append(webContext.RegisteredUser.AccessKey);

            return strBuilder.ToString();
        }

        internal virtual WebResponse GetResponse(WebContext webContext)
        {
            try
            {
                WebResponse response = webContext.Request.GetResponse();
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("Error obtaining a response from the server.", ex);
            }
        }
    }
}
