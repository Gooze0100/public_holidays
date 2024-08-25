﻿
using RequestsForData.Library.Internal.DataAccess;

namespace RequestsForData.Library.DataAccess
{
    public class MaxNumberOfFreeDaysData
    {
        public List<string> GetMaxNumberOfFreeDays(string countryCode, string year)
        {
            SqlDataAccess sqlAccess = new SqlDataAccess();

            var p = new { countryCode, year };

            List<string> output = sqlAccess.LoadData<string, dynamic>("dbo.spGetMaxDays", p, "DefaultConnection");

            return output;
        }

        public void SetMaxNumberOfFreeDays(string countryCode, string year, int @numberOfMaxDays)
        {
            SqlDataAccess sqlAccess = new SqlDataAccess();

            var p = new { countryCode, year, numberOfMaxDays };

            sqlAccess.SaveData<dynamic>("dbo.spInsertIntoMaxNumberOfFreeDays", p, "DefaultConnection");
        }
    }
}