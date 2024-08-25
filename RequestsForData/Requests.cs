using Newtonsoft.Json.Linq;

namespace RequestsForData
{
    public class Requests
    {
        // pagalvoti kaip su situo padaryti
        HttpClient httpClient = new HttpClient();
        // panaudoti singleton irasyti i db?
        public async Task<List<string>> GetCountriesList()
        {
            List<string> countries = new List<string>();
            try
            {
                string response = await httpClient.GetStringAsync("https://kayaposoft.com/enrico/json/v3.0/getSupportedCountries");
                JArray parsedCountriesData = JArray.Parse(response);
                foreach (JToken countryData in parsedCountriesData)
                {
                    if (countryData != null)
                    {
                        countries.Add(countryData["fullName"].ToString());
                    }
                }
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

            //Console.WriteLine("Holidays count: " + holidaysList.Count);


            int finalCounter = 0;
            int anotherCounter = 0;

            foreach (JToken holiday in holidaysList)
            {
                int month = (int)holiday["date"]["month"];
                int day = (int)holiday["date"]["day"];
                int daysInMonth = DateTime.DaysInMonth(year, month);
                DateOnly newDate = new DateOnly(year, month, day);
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
                        DateOnly newDateMinus = new DateOnly(year, month, day - 1);
                        if (newDateMinus != new DateOnly(year, monthAfter, dayAfter))
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
