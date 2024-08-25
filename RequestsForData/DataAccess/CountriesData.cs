using Microsoft.Extensions.Configuration;
using RequestsForData.Library.Internal.DataAccess;
using System.Configuration;

namespace RequestsForData.Library.DataAccess
{
    public class CountriesData
    {
        public List<string> GetCountries()
        {
            SqlDataAccess sqlAccess = new();

            // unit testing could benefit for this
            List<string> output = sqlAccess.LoadData<string, dynamic>("dbo.spGetCountries", new {}, "DefaultConnection");

            return output;
        }

        public void SetCountry(string countryCode, string country)
        {
            SqlDataAccess sqlAccess = new();

            // pabandyti nutrinti country code ir viena palikti
            var p = new { countryCode = countryCode, country = country };

            sqlAccess.SaveData<dynamic>("dbo.spInsertIntoCountries", p, "DefaultConnection");
        }
    }
}
