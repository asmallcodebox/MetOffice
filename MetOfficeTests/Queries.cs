using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODataPointTests
{
    internal class Query
    {
    }
    internal class ObservationsQuery : Query
    {
        public ObservationsQuery(string locationID, ObsTimeRes timeRes, DateTime dateTime)
        {
            LocationID = locationID;
            TimeRes = timeRes;
            DateTime = CustomConversions.ToObservationHourUTC(
                (dateTime));
        }
        public string LocationID { get; set; }
        public ObsTimeRes TimeRes { get; set; }
        public DateTime DateTime { get; set; }
    }
    internal class ForecastQuery : Query
    {
        public ForecastQuery(string locationID, FcsTimeRes timeRes, DateTime dateTime)
        {
            LocationID = locationID;
            TimeRes = timeRes;
            DateTime = TimeRes == FcsTimeRes.ThreeHourly ? CustomConversions.ToForecastHourUTC((dateTime))
                : CustomConversions.ToForecastDayUTC((dateTime));
        }
        public string LocationID { get; set; }
        public FcsTimeRes TimeRes { get; set; }
        public DateTime DateTime { get; set; }
    }

    internal class ObservationsCapabilitiesQuery : Query
    {
        public ObservationsCapabilitiesQuery(ObsTimeRes timeRes)
        {
            TimeRes = timeRes;
        }
        public ObsTimeRes TimeRes { get; set; }
    }

    internal class ForecastCapabilitiesQuery : Query
    {
        public ForecastCapabilitiesQuery(FcsTimeRes timeRes)
        {
            TimeRes = timeRes;
        }
        public FcsTimeRes TimeRes { get; set; }
    }
}

