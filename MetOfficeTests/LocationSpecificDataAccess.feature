Feature: MetOffice DataPoint location-specific data access
	In order to be able to obtain data from various UK sites
	As an online MetOffice DataPoint user
	I want to be able to access it by defining the queries as URLs

Scenario Outline: Obtain capabilities
Given I am a registered DataPoint user
When I send a capabilities <queryTimeResolutionType> request
Then I should obtain at least <minNumberOf> relevant time steps

Examples:
| No | queryTimeResolutionType | minNumberOf |
| 0  | observation_hourly      | 24          |
| 1  | forecast_3hourly        | 40          |
| 2  | forecast_daily          | 5           |

Scenario Outline: Obtain data for a given location and time
Given I am a registered DataPoint user
And <queryTimeResolutionType> data for a given time <requestedTime> is available
When I send a <queryTimeResolutionType> request for a given location <locationID> and time <requestedTime>
Then I should obtain data for the requested location and point in time

Examples:
| No | queryTimeResolutionType | locationID | requestedTime   |
| 0  | observation_hourly      | 3066       | now             |
| 1  | observation_hourly      | 3068       | 1 hour ago      |
| 2  | forecast_3hourly        | 3075       | now             |
| 3  | forecast_3hourly        | 3002       | 3 hours ago     |
| 4  | forecast_3hourly        | 3005       | 3 hours later   |
| 5  | forecast_3hourly        | 3005       | 4 hours later   |
| 6  | forecast_3hourly        | 3008       | 120 hours later |
| 7  | forecast_daily          | 3014       | now             |
| 8  | forecast_daily          | 3034       | 1 days ago      |
| 9  | forecast_daily          | 3044       | 5 days later    |
| 10 | forecast_daily          | all        | 5 days later    |





