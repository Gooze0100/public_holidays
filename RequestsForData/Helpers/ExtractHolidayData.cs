using Newtonsoft.Json.Linq;

namespace RequestsForData.Library.Helpers
{
    public class ExtractHolidayData
    {
        private async Task<List<DateOnly>> GetListOfDates(string countryCode, string year)
        {
            JArray holidaysList = await GetAllHolidaysForYears(year, countryCode);
            bool isYearParsed = Int32.TryParse(year, out int parsedYear);
            List<DateOnly> parsedDates = new();

            if (isYearParsed)
            {
                foreach (JToken item in holidaysList)
                {
                    parsedDates.Add(new DateOnly(parsedYear, (int)item["date"]["month"], (int)item["date"]["day"]));
                }
            }
            else
            {
                throw new ArgumentException("Year was not parsed from string.");
            }

            return parsedDates;
        }


        public async Task<List<int>> GetMaximumNumberOfFreeDaysInRows(List<DateOnly> listOfDatesToCheck, string year)
        {
            bool isYearParsed = Int32.TryParse(year, out int parsedYear);
            List<DateOnly> parsedDates = listOfDatesToCheck;
            List<DateOnly> listOfDates = new();

            if (isYearParsed)
            {
                for (int i = 0; i < parsedDates.Count; i++)
                {
                    int month = parsedDates[i].Month;
                    int day = parsedDates[i].Day;
                    int monthAfter = 0;
                    int dayAfter = 0;

                    if ((i + 1) < parsedDates.Count)
                    {
                        monthAfter = parsedDates[i + 1].Month;
                        dayAfter = parsedDates[i + 1].Day;
                    }

                    if (monthAfter > 0)
                    {
                        int daysInMonthAfter = DateTime.DaysInMonth(parsedYear, monthAfter);

                        if (dayAfter < daysInMonthAfter && dayAfter > 1)
                        {
                            if (new DateOnly(parsedYear, month, day + 1) == new DateOnly(parsedYear, monthAfter, dayAfter))
                            {
                                listOfDates.Add(parsedDates[i]);
                                listOfDates.Add(new DateOnly(parsedYear, monthAfter, dayAfter));

                            }
                        }
                    }
                }
            }
            else
            {
                throw new ArgumentException("Year was not parsed from string.");
            }

            // First group then check if goes one after another.
            var distinctDates = from d in listOfDates.Distinct()
                                group d by d.Month into e
                                select new { Month = e.Key, Count = e.Count(), LastData = e.Select(c => new { c.Day, c.Year }) };

            int cnt = 0;
            foreach (var date in distinctDates)
            {
                if (cnt < date.Count)
                {
                    cnt = date.Count;
                }
            }

            List<DateOnly> datesonly = new();
            foreach (var date in distinctDates)
            {
                if (cnt == date.Count)
                {
                    foreach (var yearDay in date.LastData)
                    {
                        datesonly.Add(new DateOnly(yearDay.Year, date.Month, yearDay.Day));
                    }
                }
            }

            IOrderedEnumerable<DateOnly> dates = datesonly.OrderBy(x => x.Day);
            int maximumNumberOfFreeDaysPerYearInRow = dates.Count();
            int counter = 0;

            for (int i = 1; i <= 7; i++)
            {

                if (dates.First().AddDays(-i).DayOfWeek == DayOfWeek.Saturday || dates.First().AddDays(-i).DayOfWeek == DayOfWeek.Sunday)
                {
                    counter++;
                }
                else if (dates.Last().AddDays(i).DayOfWeek == DayOfWeek.Saturday || dates.Last().AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                {
                    counter++;
                }
                else
                {
                    break;
                }

                maximumNumberOfFreeDaysPerYearInRow += counter;
            }

            return new List<int>() { maximumNumberOfFreeDaysPerYearInRow };
        }

    }
}
