using RequestsForData.Library.Internal.DataAccess;

namespace RequestsForData.Library.DataAccess
{
    public class SpecificDateStatusData
    {
        public List<string> GetSpecificDateStatus(string countryCode, string date)
        {
            SqlDataAccess sqlAccess = new SqlDataAccess();

            var p = new { countryCode, date };

            List<string> output = sqlAccess.LoadData<string, dynamic>("dbo.spGetDayStatus", p, "DefaultConnection");

            return output;
        }

        public void SetSpecificDateStatus(string countryCode, string year, string month, string day, string status)
        {
            SqlDataAccess sqlAccess = new SqlDataAccess();

            var p = new { countryCode, year, month, day, status };

            sqlAccess.SaveData<dynamic>("dbo.spInsertIntoSpecificDateStatus", p, "DefaultConnection");
        }
    }
}
