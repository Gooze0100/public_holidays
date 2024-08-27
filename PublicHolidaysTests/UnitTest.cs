using RequestsForData.Library;

namespace PublicHolidaysTests
{
    public class UnitTest
    {
        // Very poor tests, requires improvment
        [Fact]
        public async void GetCountries()
        {
            Requests requests = new Requests();
            if(await requests.GetCountriesList() == null)
            {
                throw new Exception("Country list is null");
            }
        }

        [Fact]
        public async void GetAllHolidaysForYears()
        {
            Requests requests = new Requests();
            if(requests.GetAllHolidaysForYears("country", "year") == null)
            {
                throw new Exception("Returned null");
            }
        }

        [Fact]
        public async void GetDayStatus()
        {
            Requests requests = new Requests();
            if (requests.GetDayStatus("country", "year") == null)
            {
                throw new Exception("Returned null");
            }
        }

        [Fact]
        public async void GetMaximumNumberOfFreeDays()
        {
            Requests requests = new Requests();
            if (requests.GetMaximumNumberOfFreeDays("country", "year") == null)
            {
                throw new Exception("Returned null");
            }
        }

        // TODO: add tests to load and save data
        // add website in github
    }
}