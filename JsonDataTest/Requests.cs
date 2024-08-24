using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

//using System.Text.Json;
using System.Text.Json.Nodes;

namespace JsonDataTest
{
    public class Requests
    {
        HttpClient httpClient = new HttpClient();
        public async Task<JArray> GetCountriesList()
        {
            JArray countries = null;
            try
            {
                string response = await httpClient.GetStringAsync("https://kayaposoft.com/enrico/json/v3.0/getSupportedCountries");
                countries = JArray.Parse(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return countries;
        }

        public async Task<JArray> GetAllHolidaysForYears(string year, string country)
        {
            JArray allHolidaysForYears = null;
            try
            {
                string response = await httpClient.GetStringAsync($"https://kayaposoft.com/enrico/json/v3.0/getHolidaysForYear?year={year}&country={country}");
                allHolidaysForYears = JArray.Parse(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return allHolidaysForYears;
        }

        public async Task<string> GetDayStatus(string date, string country)
        {
            string dayStatus = String.Empty;
            try
            {
                // padaryti kad netirkintu sito jei jau surado kaip free day true
                string responsePublicHoliday = await httpClient.GetStringAsync($"https://kayaposoft.com/enrico/json/v3.0/isPublicHoliday?date={date}&country={country}");
                JObject publicHoliday = JObject.Parse(responsePublicHoliday);

                DateOnly.TryParse(date, out DateOnly parserdDate);
                // galima sukeisti vietomis su dayofweek ir tada viduje dar vieno if padaryti jei nera nieko tada siuncia uzklausa, jei yra free day tada ja ir grazinti 
                if (publicHoliday["isPublicHoliday"] != null && (bool)publicHoliday["isPublicHoliday"] == true)
                {
                    dayStatus = "Public Holiday";
                }
                else if ((parserdDate.DayOfWeek == DayOfWeek.Saturday) || (parserdDate.DayOfWeek == DayOfWeek.Sunday))
                {
                    dayStatus = "Free day";
                }
                else
                {
                    dayStatus = "Workday";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return dayStatus;
        }

        // perdaryti year to string
        public async Task<int> GetMaximumNumberOfFreeDays(int year, string country)
        {
            int lastCounter = 0;
            int innerCounter = 0;

            JArray holidaysList = await GetAllHolidaysForYears(year.ToString(), country);
            //bool isYearParsed = Int32.TryParse(year, null, out int parsedYear);

            Console.WriteLine("Holidays count: " + holidaysList.Count);

            //foreach (JToken holiday in holidaysList)
            for (int i = 0; i < holidaysList.Count; i++)
            {
                int month = (int)holidaysList[i]["date"]["month"];
                int day = (int)holidaysList[i]["date"]["day"];

                // cia irgi padaryti kad kelis kartus to paties menesio netikrintu
                int daysInMonth = DateTime.DaysInMonth(year, month);
                int monthAfter = 0;
                int dayAfter = 0;
                DateOnly dateOnly = new DateOnly(year, month, day);
                //dateOnly.AddDays(1);
                //string dayStatus = await GetDayStatus(dateOnly.ToString("yyyy-MM-dd"), country);

                //Console.WriteLine(dateOnly);

                if ((i + 1) < holidaysList.Count)
                {
                    monthAfter = (int)holidaysList[i + 1]["date"]["month"];  
                    dayAfter = (int)holidaysList[i + 1]["date"]["day"];
                }

                innerCounter++;

                // pasidometi padaryti su linq
                if ($"{month}-{day + 1}" == $"{monthAfter}-{dayAfter}")
                {
                    //Console.WriteLine("vienas po kito");
                    innerCounter++;

                    if (day < daysInMonth)
                    {
                        // patikrinti del sito ar nereikia prasukti dar daugiau
                        DateOnly newDatePlus = new DateOnly(year, month, day + 1);
                        if (newDatePlus != new DateOnly(year, monthAfter, dayAfter))
                        {
                            Console.WriteLine(newDatePlus);
                            //Console.WriteLine(new DateOnly(year, monthAfter, dayAfter));
                            // ai nu jo cia prideda nes paima ta pati tipo jei lygus yra nes gi sventines daznai buna savaitgaliai
                            if ((newDatePlus.DayOfWeek == DayOfWeek.Saturday) || (newDatePlus.DayOfWeek == DayOfWeek.Sunday))
                            {
                                innerCounter++;
                            }
                            else
                            {
                                //innerCounter = 0;
                            }

                        }

                    }

                    if (day > 1)
                    {
                        DateOnly newDateMinus = new DateOnly(year, month, day - 1);
                        if (newDateMinus != new DateOnly(year, monthAfter, dayAfter))
                        {
                            Console.WriteLine(newDateMinus);
                            //Console.WriteLine(new DateOnly(year, monthAfter, dayAfter));
                            //dayStatus = await GetDayStatus(newDateMinus.ToString("yyyy-MM-dd"), country);
                            //Console.WriteLine("Minus Day: " + dayStatus);

                            if ((newDateMinus.DayOfWeek == DayOfWeek.Saturday) || (newDateMinus.DayOfWeek == DayOfWeek.Sunday))
                            {
                                innerCounter++;
                            }
                            else
                            {
                                //innerCounter = 0;
                            }

                        }
                    }

                    if(lastCounter < innerCounter) lastCounter = innerCounter;
                }
                else
                {
                    innerCounter = 0;
                }
            }

            Console.WriteLine("Max number of free: " + lastCounter);
            //the maximum number of free(free day + holiday) days in a row, which will be by a given country and year

            return lastCounter;
        }
    }
}
