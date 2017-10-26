using System;
using TechTalk.SpecFlow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;

namespace MODataPointTests
{
    [Binding]
    public partial class LocationSpecificDataAccess
    {
        private WebContext webContext;

        internal LocationSpecificDataAccess(WebContext context)
        {
            webContext = context;
        }

        [BeforeScenario]
        public void Initialize()
        {
            InitializeTest();
        }

        [Given(@"I am a registered DataPoint user")]
        public void GivenIAmARegisteredDataPointUser()
        {
            webContext.RegisteredUser = new RegisteredDataPointUser();
        }

        [When(@"I send a capabilities (.*) request")]
        public void WhenISendACapabilitiesRequest(QueryTimeResolutionType queryTimeResolutionType)
        {
            webContext.QueryTimeResolutionType = queryTimeResolutionType;
            webContext.Query = CreateCapabilitiesQuery(queryTimeResolutionType);
            webContext.Request = webAccess.CreateRequest(webContext);
            webContext.Response = webAccess.GetResponse(webContext);
        }
        private Query CreateCapabilitiesQuery(QueryTimeResolutionType queryTimeResolutionType)
        {
            Query query = null;
            switch (queryTimeResolutionType)
            {
                case QueryTimeResolutionType.Observation_hourly:
                    query = new ObservationsCapabilitiesQuery(ObsTimeRes.Hourly);
                    break;
                case QueryTimeResolutionType.Forecast_3hourly:
                    query = new ForecastCapabilitiesQuery(FcsTimeRes.ThreeHourly);
                    break;
                case QueryTimeResolutionType.Forecast_daily:
                    query = new ForecastCapabilitiesQuery(FcsTimeRes.Daily);
                    break;
                default:
                    throw new Exception("Not recognized query time resolution type.");
            }
            return query;
        }

        [Then(@"I should obtain at least (.*) relevant time steps")]
        public void ThenIShouldObtainAtLeastNumberOfRelevantCapabilitiesTS(int minNumberOfTimeSteps)
        {
            webContext.XmlResponseDoc = dataProcessing.GetXmlFromResponse(webContext);
            bool areCapabilitiesRelevant = dataProcessing.AreCapabilitiesTSRelevant(webContext, uploadWaitTime, minNumberOfTimeSteps); 
            Assert.IsTrue(areCapabilitiesRelevant);
        }

        [Given(@"(.*) data for a given time (.*) is available")]
        public void GivenDataForAGivenTimeIsAvailable(QueryTimeResolutionType queryTimeResolutionType, DateTime requestedTime)
        {
            WhenISendACapabilitiesRequest(queryTimeResolutionType);
            ThenRequestedTimeShouldBeAmongCapabilitiesTS(requestedTime);
        }

        private void ThenRequestedTimeShouldBeAmongCapabilitiesTS(DateTime requestedTime)
        {
            webContext.XmlResponseDoc = dataProcessing.GetXmlFromResponse(webContext);
            bool isRequestedTimeAmongCapabilities = dataProcessing.IsRequestedTimeAmongCapabilitiesTS(webContext, requestedTime);
            Assert.IsTrue(isRequestedTimeAmongCapabilities);
        }

        [When(@"I send a (.*) request for a given location (.*) and time (.*)")]
        public void WhenISendARequestForAGivenLocationAndTime(QueryTimeResolutionType queryTimeResolutionType, string locationID, DateTime requestedTime)
        {
            webContext.QueryTimeResolutionType = queryTimeResolutionType;
            webContext.RequestedTime = requestedTime;
            webContext.Query = CreateLocationSpecificQuery(queryTimeResolutionType, locationID, requestedTime);
            webContext.Request = webAccess.CreateRequest(webContext);
            webContext.Response = webAccess.GetResponse(webContext);
        }

        private Query CreateLocationSpecificQuery(QueryTimeResolutionType queryTimeResolutionType, string locationID, DateTime requestedTime)
        {
            Query query = null;
            switch (queryTimeResolutionType)
            {
                case QueryTimeResolutionType.Observation_hourly:
                    query = new ObservationsQuery(locationID, ObsTimeRes.Hourly, requestedTime);
                    break;
                case QueryTimeResolutionType.Forecast_3hourly:
                    query = new ForecastQuery(locationID, FcsTimeRes.ThreeHourly, requestedTime);
                    break;
                case QueryTimeResolutionType.Forecast_daily:
                    query = new ForecastQuery(locationID, FcsTimeRes.Daily, requestedTime);
                    break;
                default:
                    throw new Exception("Not recognized query time resolution type.");
            }
            return query;
        }

        [Then(@"I should obtain data for the requested location and point in time")]
        public void ThenIShouldObtainDataForTheRequestedLocationAndPointInTime()
        {
            webContext.XmlResponseDoc = dataProcessing.GetXmlFromResponse(webContext);
            bool isRequestedLocationObtained = dataProcessing.IsRequestedLocationObtained(webContext);
            bool isRequestedTimeObtained = dataProcessing.IsRequestedTimeObtained(webContext);
            Assert.IsTrue(isRequestedLocationObtained && isRequestedTimeObtained);           
        }
    }
}
