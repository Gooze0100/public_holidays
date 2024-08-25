using RequestsForData.Library.Internal.DataAccess;

namespace RequestsForData.Library.DataAccess
{
    public class HolidaysData
    {
        public List<string> GetHolidays(string countryCode, string year)
        {
            SqlDataAccess sqlAccess = new SqlDataAccess();

            var p = new { countryCode, year };

            // greiciausiai reikes sugrupuoti cia
            List<string> output = sqlAccess.LoadData<string, dynamic>("dbo.spGetHolidays", p, "DefaultConnection");

            return output;
        }

        public void SetHolidays(string countryCode, string year, string month, string day, string name_origin, string name_eng, string holidayType)
        {
            SqlDataAccess sqlAccess = new SqlDataAccess();

            var p = new { countryCode, year, month, day, name_origin, name_eng, @holidayType };

            sqlAccess.SaveData<dynamic>("dbo.spInsertIntoHolidays", p, "DefaultConnection");
        }
    }
}
