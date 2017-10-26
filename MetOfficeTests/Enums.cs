using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODataPointTests
{
    internal enum DataType
    {
        Xml,
        //Json
    }

    internal enum ObsTimeRes
    {
        Hourly,
    }
    internal enum FcsTimeRes
    {
        ThreeHourly,
        Daily
    }

    public enum QueryTimeResolutionType
    {
        Observation_hourly,
        Forecast_3hourly,
        Forecast_daily
    }
}
