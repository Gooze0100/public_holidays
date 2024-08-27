using Newtonsoft.Json.Linq;
using RequestsForData.Library.DataAccess;

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
            List<dynamic> allHolidaysForYears = holidaysData.GetHolidays(countryCode, year);

            if (allHolidaysForYears.Count <= 0)
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
                                // Very bad approach need to update DB types.
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

                        IEnumerable<IGrouping<dynamic, dynamic>> groupedByMonth = holidaysData.GetHolidays(countryCode, year).GroupBy(h => h.Month);
                        allHolidaysForYears = groupedByMonth as List<dynamic>;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            return allHolidaysForYears;
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

        // perdaryti year to string
        public async Task<int> GetMaximumNumberOfFreeDays(string country, string year)
        {
            HttpClient httpClient = new();
            Int32.TryParse(year, out int parsedYear);

            int lastCounter = 0;
            int innerCounter = 0;

            List<dynamic> holidaysList = await GetAllHolidaysForYears(year.ToString(), country);
            //bool isYearParsed = Int32.TryParse(year, null, out int parsedYear);

            //Console.WriteLine("Holidays count: " + holidaysList.Count);


            int finalCounter = 0;
            int anotherCounter = 0;

            foreach (JToken holiday in holidaysList)
            {
                int month = (int)holiday["date"]["month"];
                int day = (int)holiday["date"]["day"];
                int daysInMonth = DateTime.DaysInMonth(parsedYear, month);
                DateOnly newDate = new DateOnly(parsedYear, month, day);
                //anotherCounter++;
                Console.WriteLine(newDate);

                // cia yra pradine date tai su ja reiktu tikrinti 
                //Console.WriteLine(newDate.AddDays(-1));
                //if ((newDate.AddDays(-1).DayOfWeek == DayOfWeek.Saturday) || (newDate.AddDays(-1).DayOfWeek == DayOfWeek.Sunday))
                //{
                //    Console.WriteLine("ateina--");
                //    anotherCounter++;
                //}
                //if ((newDate.AddDays(1).DayOfWeek == DayOfWeek.Saturday) || (newDate.AddDays(1).DayOfWeek == DayOfWeek.Sunday))
                //{
                //    //Console.WriteLine("ateina++");
                //    anotherCounter++;
                //}

                for (int j = 1; j <= 7; j++)
                {
                    // jis prides jei bus kita data tipo jei bus is kitos savaitgalio o ne in sequence
                    // reikia pasiimti new date ir ji valdyti nes kitaip niekaip tipo dienas keisti nes 
                    if ((newDate.AddDays(-j).DayOfWeek == DayOfWeek.Saturday) || (newDate.AddDays(-j).DayOfWeek == DayOfWeek.Sunday))
                    {
                        //Console.WriteLine("ateina--");
                        anotherCounter++;
                    }
                    else
                    {
                        break;
                    }

                }


                for (int i = 0; i <= holidaysList.Count; i++)
                {
                    int monthAfter = month;
                    int dayAfter = day;
                    if ((i + 1) < holidaysList.Count)
                    {
                        monthAfter = (int)holidaysList[i + 1]["date"]["month"];
                        dayAfter = (int)holidaysList[i + 1]["date"]["day"];
                    }

                    if (month == monthAfter && day == dayAfter)
                    {
                        anotherCounter++;

                        // kazkaip issiaiskinti last in sequence and first
                        // reiktu gal pagalvoti kad skippintu tipo ta data su continue
                        if (anotherCounter > 0)
                        {
                            // realiai kaip ir veikia tik reikia su sitais pasizaisti tipo kad jei pirmas tai irgi counter daryti nu nes prideda dienu bereikalingai o turetu tiesiog pirma data paimti ir su ja zaisti ir su paskutine
                            if ((newDate.AddDays(i).DayOfWeek == DayOfWeek.Saturday) || (newDate.AddDays(i).DayOfWeek == DayOfWeek.Sunday))
                            {
                                //Console.WriteLine("ateina++");
                                //anotherCounter++;
                            }

                            if ((newDate.AddDays(-i).DayOfWeek == DayOfWeek.Saturday) || (newDate.AddDays(-i).DayOfWeek == DayOfWeek.Sunday))
                            {
                                //Console.WriteLine("ateina--");
                                //anotherCounter++;
                            }

                        }


                        //if (finalCounter < anotherCounter) finalCounter = anotherCounter;

                        //if (finalCounter < anotherCounter) finalCounter = anotherCounter;
                        // o tai gal sitam padaryti dar viena cikla? tipo uz situ dvieju ciklu tiesiog grazintu pirma ir paskutine data kur is eiles eina


                        // 
                        //break;
                        if (day < daysInMonth)
                        {
                            if ((newDate.AddDays(i).DayOfWeek == DayOfWeek.Saturday) || (newDate.AddDays(i).DayOfWeek == DayOfWeek.Sunday))
                            {
                                //Console.WriteLine("ateina++");
                                ///anotherCounter++;
                            }
                        }

                        if (day > 1)
                        {
                            if ((newDate.AddDays(-i).DayOfWeek == DayOfWeek.Saturday) || (newDate.AddDays(-i).DayOfWeek == DayOfWeek.Sunday))
                            {
                                //Console.WriteLine("ateina--");
                                //anotherCounter++;
                            }
                        }
                        //if (finalCounter < anotherCounter) finalCounter = anotherCounter;

                        //cia vos ne kinda reikia atskiro ciklo
                        // sitas pakeicia todel ir tiek nebesuskaiciuoja dienu
                        //DateOnly newDatePlus = new DateOnly(year, month, day);
                        //if ((newDatePlus.DayOfWeek == DayOfWeek.Saturday) || (newDatePlus.DayOfWeek == DayOfWeek.Sunday))
                        //{
                        //anotherCounter++;
                        //}

                        //if (finalCounter < anotherCounter) finalCounter = anotherCounter;
                    }
                    else
                    {

                        for (int k = 1; k <= 7; k++)
                        {
                            if ((newDate.AddDays(k).DayOfWeek == DayOfWeek.Saturday) || (newDate.AddDays(k).DayOfWeek == DayOfWeek.Sunday))
                            {
                                Console.WriteLine("ateina++");
                                anotherCounter++;
                            }
                            else
                            {
                                break;
                            }
                        }




                        //continue;
                    }
                    //anotherCounter = 0;

                }





                if (finalCounter < anotherCounter) finalCounter = anotherCounter;
                anotherCounter = 0;
            }


            Console.WriteLine("Counted days: " + finalCounter);

            for (int i = 0; i < holidaysList.Count; i++)
            {
                int month = (int)holidaysList[i]["date"]["month"];
                int day = (int)holidaysList[i]["date"]["day"];

                // cia irgi padaryti kad kelis kartus to paties menesio netikrintu
                int daysInMonth = DateTime.DaysInMonth(parsedYear, month);
                int monthAfter = 0;
                int dayAfter = 0;
                DateOnly dateOnly = new DateOnly(parsedYear, month, day);
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
                        DateOnly newDatePlus = new DateOnly(parsedYear, month, day + 1);
                        if (newDatePlus != new DateOnly(parsedYear, monthAfter, dayAfter))
                        {
                            //Console.WriteLine(newDatePlus);
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
                        DateOnly newDateMinus = new DateOnly(parsedYear, month, day - 1);
                        if (newDateMinus != new DateOnly(parsedYear, monthAfter, dayAfter))
                        {
                            //Console.WriteLine(newDateMinus);
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

                    if (lastCounter < innerCounter) lastCounter = innerCounter;
                }
                else
                {
                    innerCounter = 0;
                }
            }

            //Console.WriteLine("Max number of free: " + lastCounter);
            //the maximum number of free(free day + holiday) days in a row, which will be by a given country and year

            return lastCounter;
        }

    }
}
