using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MODataPointTests
{
    internal class DataProcessing
    {
        internal virtual XmlDocument GetXmlFromResponse(WebContext webContext)
        {
            XmlDocument xmlResponseDoc = null;
            try
            {
                if (webContext.Response != null)
                {
                    xmlResponseDoc = new XmlDocument();
                    xmlResponseDoc.Load(webContext.Response.GetResponseStream());
                }
            }
            catch (XmlException ex)
            {
                throw new Exception("Error converting to xml.", ex);
            }

            return xmlResponseDoc;
        }

        internal bool AreCapabilitiesTSRelevant(WebContext webContext, double uploadWaitTime, int minNumberOfTimeSteps)
        {
            bool areTSNewlyUpdated = false;
            XmlNodeList dateTimeElements = ObtainXmlElement(webContext.XmlResponseDoc,"TS");
           
            if (dateTimeElements != null && dateTimeElements.Count > 0)
            {
                int noOfObtainedTimeSteps = dateTimeElements.Count;
                
                int i;
                switch (webContext.QueryTimeResolutionType)
                {
                    case QueryTimeResolutionType.Observation_hourly:
                        DateTime nowUTC = DateTime.Now.ToUniversalTime();
                        DateTime oldestTS = CustomConversions.ConvertToDateTime(dateTimeElements[0].InnerText);
                        DateTime newestTS = CustomConversions.ConvertToDateTime(dateTimeElements[noOfObtainedTimeSteps - 1].InnerText);
                        DateTime backMinHours = nowUTC.Subtract(TimeSpan.FromHours(minNumberOfTimeSteps));

                        if (dateTimeElements.Count >= minNumberOfTimeSteps
                        && nowUTC - newestTS <= TimeSpan.FromHours(1) && oldestTS - backMinHours <= TimeSpan.FromHours(1)
                        || nowUTC - newestTS <= TimeSpan.FromHours(1 + uploadWaitTime) && nowUTC - newestTS > TimeSpan.FromHours(1)
                            && backMinHours - oldestTS <= TimeSpan.FromHours(uploadWaitTime))
                            areTSNewlyUpdated = true;
                        break;
                    case QueryTimeResolutionType.Forecast_3hourly:
                        bool excludeCurrentFcstHour = false;
                        nowUTC = CustomConversions.ToForecastHourUTC(DateTime.Now);
                        nowUTC = excludeCurrentFcstHour ? nowUTC.AddHours(3) : nowUTC;
                        DateTime firstTS = CustomConversions.ConvertToDateTime(dateTimeElements[0].InnerText);

                        if (nowUTC >= firstTS)
                        {
                            i = 0;
                            do
                            {
                                ++i;
                            }
                            while (i < noOfObtainedTimeSteps
                            && CustomConversions.ConvertToDateTime(dateTimeElements[i].InnerText) < nowUTC);

                            if (noOfObtainedTimeSteps - i >= minNumberOfTimeSteps)
                                areTSNewlyUpdated = true;
                        }
                        break;
                    case QueryTimeResolutionType.Forecast_daily:
                        bool excludeCurrentFcstDay = false;
                        nowUTC = CustomConversions.ToForecastDayUTC(DateTime.Now);
                        nowUTC = excludeCurrentFcstDay ? nowUTC.AddDays(1) : nowUTC;
                        firstTS = CustomConversions.ConvertToDateTime(dateTimeElements[0].InnerText);

                        if (nowUTC >= firstTS)
                        {
                            i = 0;
                            do
                            {
                                ++i;
                            }
                            while (i < noOfObtainedTimeSteps
                            && CustomConversions.ConvertToDateTime(dateTimeElements[i].InnerText) < nowUTC);

                            if (noOfObtainedTimeSteps - i >= minNumberOfTimeSteps)
                                areTSNewlyUpdated = true;
                        }
                        break;
                    default:
                        throw new Exception("Not recognized query time resolution type.");
                }
            }
            return areTSNewlyUpdated;
        }

        internal bool IsRequestedTimeAmongCapabilitiesTS(WebContext webContext, DateTime requestedTime) 
        {
            bool isRequestedTimeAmongCapabilities = false;
            XmlNodeList dateTimeElements = ObtainXmlElement(webContext.XmlResponseDoc,"TS");
            
            if (dateTimeElements!=null && dateTimeElements.Count > 0)
            {
                DateTime requestedTimeRndHourUTC = CustomConversions.ToLastHour(requestedTime).ToUniversalTime();

                switch (webContext.QueryTimeResolutionType)
                {
                    case QueryTimeResolutionType.Observation_hourly:
                        DateTime oldestTS = CustomConversions.ConvertToDateTime(dateTimeElements[0].InnerText);
                        DateTime newestTS = CustomConversions.ConvertToDateTime(dateTimeElements[dateTimeElements.Count - 1].InnerText);
                        if (
                            requestedTimeRndHourUTC >= oldestTS
                            && requestedTimeRndHourUTC <= newestTS)
                                isRequestedTimeAmongCapabilities = true;
                        break;
                    case QueryTimeResolutionType.Forecast_3hourly:
                        DateTime firstTS = CustomConversions.ConvertToDateTime(dateTimeElements[0].InnerText);
                        DateTime lastTS = CustomConversions.ConvertToDateTime(dateTimeElements[dateTimeElements.Count - 1].InnerText);
                        if (requestedTimeRndHourUTC >= firstTS
                            && requestedTimeRndHourUTC < lastTS.AddHours(3))
                                isRequestedTimeAmongCapabilities = true;
                        break;
                    case QueryTimeResolutionType.Forecast_daily:
                        firstTS = CustomConversions.ConvertToDateTime(dateTimeElements[0].InnerText);
                        lastTS = CustomConversions.ConvertToDateTime(dateTimeElements[dateTimeElements.Count - 1].InnerText);
                        if (requestedTimeRndHourUTC >= firstTS 
                            && requestedTimeRndHourUTC < lastTS.AddDays(1))
                                isRequestedTimeAmongCapabilities = true;
                        break;
                    default:
                        throw new Exception("Not recognized query time resolution type.");
                }
            }
            return isRequestedTimeAmongCapabilities;
        }

        internal bool IsRequestedLocationObtained(WebContext webContext)
        {
            bool isRequestedLocationObtained = false;
            XmlNodeList locationElement = ObtainXmlElement(webContext.XmlResponseDoc, "Location");

            string requestedLocationID="";
            if (webContext.Query is ObservationsQuery)
                requestedLocationID = ((ObservationsQuery)webContext.Query).LocationID;
            else if (webContext.Query is ForecastQuery)
                requestedLocationID = ((ForecastQuery)webContext.Query).LocationID;

            string obtainedLocation="";
            if (locationElement != null && locationElement.Count == 1 && requestedLocationID.ToLower() != "all")
            {
                obtainedLocation = locationElement[0].Attributes["i"].Value;
            }
            else if (locationElement != null && locationElement.Count > 1 && requestedLocationID.ToLower() == "all")
            {
                obtainedLocation = "all";
            }

            if (obtainedLocation == requestedLocationID)
                isRequestedLocationObtained = true;
            return isRequestedLocationObtained;
        }

        internal bool IsRequestedTimeObtained(WebContext webContext) 
        {
            bool isRequestedTimeObtained = false;
            //if 'locationID'=all then it is assumed that the same time is returned for 
            //all locations and so time is checked only for the first location
            XmlNodeList dateElement = ObtainXmlElement(webContext.XmlResponseDoc, "Period");
            XmlNodeList timeElement = ObtainXmlElement(webContext.XmlResponseDoc, "Rep");
            
            if (dateElement!=null && dateElement.Count > 0 
                && timeElement!=null && timeElement.Count > 0)
            {
                string strObtainedDate = dateElement[0].Attributes["value"].Value;
                string strObtainedHour = timeElement[0].InnerText.Trim();
              
                DateTime obtainedDate = CustomConversions.ConvertToDate(strObtainedDate);
                int intObtainedHour = 0;
                if (webContext.QueryTimeResolutionType != QueryTimeResolutionType.Forecast_daily)
                {
                    intObtainedHour = Int32.Parse(strObtainedHour);
                    intObtainedHour = intObtainedHour / 60;
                }
                DateTime obtainedDateTime;
                DateTime requestedTimeRndUTC;
                switch (webContext.QueryTimeResolutionType)
                {
                    case QueryTimeResolutionType.Observation_hourly:
                        requestedTimeRndUTC = CustomConversions.ToObservationHourUTC(webContext.RequestedTime);
                        obtainedDateTime = obtainedDate.AddHours(intObtainedHour);
                        break;
                    case QueryTimeResolutionType.Forecast_3hourly:
                        requestedTimeRndUTC = CustomConversions.ToForecastHourUTC(webContext.RequestedTime);
                        obtainedDateTime = obtainedDate.AddHours(intObtainedHour);
                        break;
                    case QueryTimeResolutionType.Forecast_daily:
                        requestedTimeRndUTC = CustomConversions.ToForecastDayUTC(webContext.RequestedTime);
                        obtainedDateTime = obtainedDate;
                        break;
                    default:
                        throw new Exception("Not recognized query time resolution type.");
                }
                if (obtainedDateTime == requestedTimeRndUTC)
                    isRequestedTimeObtained = true;
                else return false;
            }
            return isRequestedTimeObtained;
        }

        private XmlNodeList ObtainXmlElement(XmlDocument xmlDoc, string xmlElementName)
        {
            XmlNodeList elementsList = null;
            try
            {
                elementsList = xmlDoc.GetElementsByTagName(xmlElementName);
                
            }
            catch (Exception ex)
            {
                throw new Exception("Error obtaining "+xmlElementName+" element(s) from xml document.", ex);
            }
            return elementsList;
        }
    }
}
