namespace RequestsForData.Library.Helpers
{
    public class ExtractHolidayData
    {
        public int GetMaximumNumberOfFreeDaysInRows(List<DateOnly> listOfDatesToCheck, string year)
        {
            bool isYearParsed = Int32.TryParse(year, out int parsedYear);
            List<DateOnly> listOfDates = new();

            if (isYearParsed)
            {
                for (int i = 0; i < listOfDatesToCheck.Count; i++)
                {
                    int month = listOfDatesToCheck[i].Month;
                    int day = listOfDatesToCheck[i].Day;
                    int monthAfter = 0;
                    int dayAfter = 0;

                    if ((i + 1) < listOfDatesToCheck.Count)
                    {
                        monthAfter = listOfDatesToCheck[i + 1].Month;
                        dayAfter = listOfDatesToCheck[i + 1].Day;
                    }

                    if (monthAfter > 0)
                    {
                        int daysInMonthAfter = DateTime.DaysInMonth(parsedYear, monthAfter);

                        if (dayAfter < daysInMonthAfter && dayAfter > 1)
                        {
                            if (new DateOnly(parsedYear, month, day + 1) == new DateOnly(parsedYear, monthAfter, dayAfter))
                            {
                                listOfDates.Add(listOfDatesToCheck[i]);
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
                if (dates.FirstOrDefault().AddDays(-i).DayOfWeek == DayOfWeek.Saturday || dates.FirstOrDefault().AddDays(-i).DayOfWeek == DayOfWeek.Sunday)
                {
                    counter++;
                }
                else if (dates.LastOrDefault().AddDays(i).DayOfWeek == DayOfWeek.Saturday || dates.LastOrDefault().AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                {
                    counter++;
                }
                else
                {
                    break;
                }

                maximumNumberOfFreeDaysPerYearInRow += counter;
            }

            return maximumNumberOfFreeDaysPerYearInRow;
        }
    }
}
