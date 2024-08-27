using RequestsForData.Library.Internal.DataAccess;

namespace RequestsForData.Library.DataAccess
{
    public class SpecificDateStatusData
    {
        public List<dynamic> GetSpecificDateStatus(string countryCode, string date)
        {
            SqlDataAccess sqlAccess = new SqlDataAccess();

            var p = new { countryCode, date };

            List<dynamic> output = sqlAccess.LoadData<dynamic>("dbo.spGetDayStatus", p, "DefaultConnection");

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
