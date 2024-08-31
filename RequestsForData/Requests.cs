using Newtonsoft.Json.Linq;
using RequestsForData.Library.DataAccess;
using RequestsForData.Library.Helpers;

namespace RequestsForData.Library
{
    public class Requests
    {
        public async Task<List<dynamic>> GetCountriesList()
        {
            CountriesData countriesData = new();
            List<dynamic> listOfCountries = countriesData.GetCountries();

            if(listOfCountries.Count < 58)
            {
                try
                {
                    HttpClient httpClient = new();
                    string response = await httpClient.GetStringAsync("https://kayaposoft.com/enrico/json/v3.0/getSupportedCountries");
                    JArray parsedCountries = JArray.Parse(response);

                    if (parsedCountries.HasValues)
                    {
                        countriesData.DeleteCountries();

                        foreach (JToken countryData in parsedCountries)
                        {
                            if (countryData != null)
                            {
                                string countryCode = countryData["countryCode"].ToString();
                                string country = countryData["fullName"].ToString();
                                countriesData.SetCountry(countryCode, country);
                                listOfCountries.Add(new { countryCode, country});
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            return listOfCountries;
        }

        public async Task<List<dynamic>> GetAllHolidaysForYears(string countryCode, string year)
        {
            HolidaysData holidaysData = new HolidaysData();
            IEnumerable<dynamic> allHolidaysForYears = 
                            from holiday in holidaysData.GetHolidays(countryCode, year)
                            group holiday by holiday.Month into h
                            select new { Month = h.Key, Count = h.Count(), LastData = h.Select(c => new { c.Year, c.Day, c.Name_Origin, c.Name_ENG, c.HolidayType }) };

            if (allHolidaysForYears.Count() <= 0)
            {
                try
                {
                    HttpClient httpClient = new();
                    string response = await httpClient.GetStringAsync($"https://kayaposoft.com/enrico/json/v3.0/getHolidaysForYear?year={year}&country={countryCode}");
                    JArray parsedAllHolidaysForYears = JArray.Parse(response);

                    if(parsedAllHolidaysForYears.HasValues)
                    {
                        foreach (JToken holiday in parsedAllHolidaysForYears)
                        {
                            if (holiday != null)
                            {
                                string month = holiday["date"]["month"].ToString();
                                if (month.Count() == 1)
                                {
                                    month = "0" + month;
                                }
                                string day = holiday["date"]["day"].ToString();
                                if (day.Count() == 1)
                                {
                                    day = "0" + day;
                                }
                                string nameOrigin = holiday["name"][0]["text"].ToString();
                                string nameEng = holiday["name"][1]["text"].ToString();
                                string holidayType = holiday["holidayType"].ToString();

                                holidaysData.SetHolidays(countryCode, year, month, day, nameOrigin, nameEng, holidayType);
                            }
                        }

                        allHolidaysForYears = 
                            from holiday in holidaysData.GetHolidays(countryCode, year)
                            group holiday by holiday.Month into h
                            select new {Month = h.Key, Count = h.Count(), LastData = h.Select(c => new {c.Year, c.Day, c.Name_Origin, c.Name_ENG, c.HolidayType}) };
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            return allHolidaysForYears.ToList();
        }

        public async Task<List<dynamic>> GetDayStatus(string countryCode, string date)
        {
            List<dynamic> dayStatus = new();
            try
            {
                HolidaysData holidaysData = new HolidaysData();
                SpecificDateStatusData specificDateStatusData = new SpecificDateStatusData();

                string year = date.Substring(0, 4);
                List<dynamic> allHolidaysForYears = holidaysData.GetHolidays(countryCode, year);

                if (allHolidaysForYears.Count > 0)
                {
                    foreach (dynamic holiday in allHolidaysForYears)
                    {
                        string newDate = $"{year}-{holiday.Month}-{holiday.Day}";
                        if (date == newDate)
                        {
                            dayStatus.Add("Public Holiday");
                            return dayStatus;
                        }
                    }
                }
                else
                {
                    List<dynamic> specificDateStatus = specificDateStatusData.GetSpecificDateStatus(countryCode, date);

                    if (specificDateStatus.Count > 0)
                    {
                        dayStatus = specificDateStatus;
                        return dayStatus;
                    }
                }

                HttpClient httpClient = new();
                string responsePublicHoliday = await httpClient.GetStringAsync($"https://kayaposoft.com/enrico/json/v3.0/isPublicHoliday?date={date}&country={countryCode}");
                JObject publicHoliday = JObject.Parse(responsePublicHoliday);
                DateOnly.TryParse(date, out DateOnly parserdDate);
                string month = date.Substring(5, 2);
                string day = date.Substring(date.Length - 2);

                if (publicHoliday["isPublicHoliday"] != null && (bool)publicHoliday["isPublicHoliday"] == true)
                {
                    specificDateStatusData.SetSpecificDateStatus(countryCode, year, month, day, "Public Holiday");
                    dayStatus.Add("Public Holiday");
                }
                else if ((parserdDate.DayOfWeek == DayOfWeek.Saturday) || (parserdDate.DayOfWeek == DayOfWeek.Sunday))
                {
                    specificDateStatusData.SetSpecificDateStatus(countryCode, year, month, day, "Free day");
                    dayStatus.Add("Free day");
                }
                else
                {
                    specificDateStatusData.SetSpecificDateStatus(countryCode, year, month, day, "Workday");
                    dayStatus.Add("Workday");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return dayStatus;
        }

        public async Task<List<dynamic>> GetMaximumNumberOfFreeDays(string countryCode, string year)
        {
            List<dynamic> daysCounted = new();
            MaxNumberOfFreeDaysData maxNumberOfFreeDaysData = new();
            daysCounted = maxNumberOfFreeDaysData.GetMaxNumberOfFreeDays(countryCode, year);

            if(daysCounted.Count > 0)
            {
                return daysCounted;
            }
            else
            {
                List<DateOnly> listOfDatesFromDB = await GetListOfDates(countryCode, year);
                ExtractHolidayData extractHolidayData = new();
                int countedDays = extractHolidayData.GetMaximumNumberOfFreeDaysInRows(listOfDatesFromDB, year);
                if (countedDays > 0)
                {
                    maxNumberOfFreeDaysData.SetMaxNumberOfFreeDays(countryCode, year, countedDays);
                    daysCounted = maxNumberOfFreeDaysData.GetMaxNumberOfFreeDays(countryCode, year);
                }
                else
                {
                    throw new Exception("Days not counted.");
                }
            }

            return daysCounted;
        }
        
        private async Task<List<DateOnly>> GetListOfDates(string countryCode, string year)
        {
            bool isYearParsed = Int32.TryParse(year, out int parsedYear);
            List<dynamic> holidaysList = await GetAllHolidaysForYears(countryCode, year);
            List<DateOnly> parsedDates = new();

            if (isYearParsed)
            {
                foreach (dynamic data in holidaysList)
                {
                    Int32.TryParse(data.Month, out int parsedMonth);

                    foreach (dynamic lastData in data.LastData)
                    {
                        Int32.TryParse(lastData.Day, out int parsedDay);
                        parsedDates.Add(new DateOnly(parsedYear, parsedMonth, parsedDay));
                    }
                }
            }
            else
            {
                throw new ArgumentException("Year was not parsed from string.");
            }

            return parsedDates;
        }
    }
}
