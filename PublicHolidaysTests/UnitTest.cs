using RequestsForData.Library;

namespace PublicHolidaysTests
{
    public class UnitTest
    {
        [Fact]
        public async void GetCountries()
        {
            Requests requests = new();
            if(await requests.GetCountriesList() == null)
            {
                throw new Exception("Country list is null");
            }

            List<dynamic> getData = await requests.GetCountriesList();
            if (getData.Count <= 0)
            {
                throw new Exception("No data was returned");
            }
        }

        [Fact]
        public async void GetAllHolidaysForYears()
        {
            Requests requests = new();
            if(await requests.GetAllHolidaysForYears("country", "year") == null)
            {
                throw new Exception("Returned null");
            }

            List<dynamic> getData = await requests.GetAllHolidaysForYears("ltu", "2023");
            if (getData.Count <= 0)
            {
                throw new Exception("Data list was returned with one or more records, for parameters CountryCode: ltu, Year: 2023");
            }
        }

        [Fact]
        public async void GetDayStatus()
        {
            Requests requests = new();
            if (await requests.GetDayStatus("country", "year") == null)
            {
                throw new Exception("Returned null");
            }

            List<dynamic> getData = await requests.GetDayStatus("ltu", "2023-04-07");
            if(getData.Count <= 0)
            {
                throw new Exception("Data list was returned with one or more records, for parameters CountryCode: ltu, Date: 2023-04-07");
            }

        }

        [Fact]
        public async void GetMaximumNumberOfFreeDays()
        {
            Requests requests = new();
            if (await requests.GetMaximumNumberOfFreeDays("country", "year") == null)
            {
                throw new Exception("Returned null");
            }

            List<dynamic> getData = await requests.GetMaximumNumberOfFreeDays("ltu", "2023");
            if (getData.Count <= 0)
            {
                throw new Exception("Data list was returned with one or more records, for parameters CountryCode: ltu, Year: 2023");
            }
        }
    }
}