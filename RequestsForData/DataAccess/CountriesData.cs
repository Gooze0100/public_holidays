using RequestsForData.Library.Internal.DataAccess;

namespace RequestsForData.Library.DataAccess
{
    public class CountriesData
    {
        public List<dynamic> GetCountries()
        {
            SqlDataAccess sqlAccess = new();

            List<dynamic> output = sqlAccess.LoadData<dynamic>("dbo.spGetCountries", new {}, "DefaultConnection");

            return output;
        }

        public void SetCountry(string countryCode, string country)
        {
            SqlDataAccess sqlAccess = new();

            var p = new { countryCode,  country };

            sqlAccess.SaveData<dynamic>("dbo.spInsertIntoCountries", p, "DefaultConnection");
        }

        public void DeleteCountries()
        {
            SqlDataAccess sqlAccess = new();

            sqlAccess.DeleteData("dbo.spDeleteCountries", "DefaultConnection");
        }
    }
}
