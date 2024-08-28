using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

//using System.Text.Json;
using System.Text.Json.Nodes;

namespace JsonDataTest
{
    public class Requests
    {
        HttpClient httpClient = new HttpClient();
        // panaudoti singleton irasyti i db?
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
        public async Task<string> GetMaximumNumberOfFreeDays(int year, string country)
        {
            int lastCounter = 0;
            int innerCounter = 0;

            JArray holidaysList = await GetAllHolidaysForYears(year.ToString(), country);
  
            //var newholidaysList = from holiday in holidaysList orderby holiday["date"]["month"] select holiday;

            int finalCounter = 0;
            int anotherCounter = 0;

            List<DateOnly> listOfDates = new();


            for (int i = 0; i < holidaysList.Count; i++)
            {
                int month = (int)holidaysList[i]["date"]["month"];
                int day = (int)holidaysList[i]["date"]["day"];
                DateOnly combinedDate = new DateOnly(year, month, day);

                int monthAfter = 0;
                int dayAfter = 0;

                if ((i + 1) < holidaysList.Count)
                {
                    monthAfter = (int)holidaysList[i + 1]["date"]["month"];
                    dayAfter = (int)holidaysList[i + 1]["date"]["day"];
                }

                if (monthAfter > 0)
                {
                    int daysInMonthAfter = DateTime.DaysInMonth(year, monthAfter);
                    if (dayAfter < daysInMonthAfter && dayAfter > 1)
                    {
                        if (new DateOnly(year, month, day + 1) == new DateOnly(year, monthAfter, dayAfter))
                        {
                            // padaryti grupavimus ir t.t.
                            listOfDates.Add(combinedDate);
                            listOfDates.Add(new DateOnly(year, monthAfter, dayAfter));

                        }
                    }
                }
            }

            // teoriskai turejo grazinti kaip is SQL tipo sugrupuota pagal men ir tada kiekvienam men skaiciuoti kiek yra
            //var data = listOfDates.Distinct().ToList().GroupBy(d => d.Month);
            // kai sugrupuji tada tik vienas menuo ir lieka :D o days turi sukti 
            var distinctDates = from d in listOfDates.Distinct() 
                                group d by d.Month into e 
                                select new { Month = e.Key, Count = e.Count(), LastData = e.Select(c => new { c.Day, c.Year }) };

            //var distinctDates = from date in listOfDates.Distinct() select date;
            int cnt = 0;
            //foreach (var date in listOfDates.GroupBy(d => d.Month))
            //{
            //    Console.WriteLine("{0} {1}", date.Key, date.Count());
            //    //Console.WriteLine(date);
            //}
            //Console.WriteLine(distinctDates.Max(d => d.Count));

            foreach (var item in distinctDates)
            {
                if (cnt < item.Count)
                {
                    cnt = item.Count;
                }
            }

            List<DateOnly> datesonly = new();
            foreach (var item in distinctDates)
            {
                if(cnt == item.Count)
                {
                    foreach (var item1 in item.LastData)
                    {
                        // tipo ta pati padarius su DB tai stored funkcija grazinti ir tada su linq galima paimti menesi ir jame ziureti ar dienos eina viena po kitos ir tada jei taip count ir supusti i lista ir ji order by ir tada paimti first ir last value ir prideti ziureti ar yra weekend vienas po kito
                        datesonly.Add(new DateOnly(item1.Year, item.Month, item1.Day));
                        //Console.WriteLine(new DateOnly(item1.Year, item.Month, item1.Day));
                    }
                }
            }

            var dates = datesonly.OrderBy(x => x.Day);

            var counterofdays = dates.Count();
            var cntner = 0;

            for (int i = 1; i <= 7; i++)
            {

                if (dates.First().AddDays(-i).DayOfWeek == DayOfWeek.Saturday || dates.First().AddDays(-i).DayOfWeek == DayOfWeek.Sunday)
                {
                    cntner++;
                }
                else if(dates.Last().AddDays(i).DayOfWeek == DayOfWeek.Saturday || dates.Last().AddDays(i).DayOfWeek == DayOfWeek.Sunday)
                {
                    cntner++;
                }
                else
                {
                    break;
                }

                counterofdays += cntner;
            }



            Console.WriteLine("counterofdays: " + counterofdays);


            // =========================================================


            foreach (JToken holiday in holidaysList) 
            {
                int month = (int)holiday["date"]["month"];
                int day = (int)holiday["date"]["day"];
                int daysInMonth = DateTime.DaysInMonth(year, month);
                DateOnly combinedDate = new DateOnly(year, month, day);
                //anotherCounter++;
                //Console.WriteLine(combinedDate);

                // cia yra pradine date tai su ja reiktu tikrinti 
                //Console.WriteLine(combinedDate.AddDays(-1));
                //if ((combinedDate.AddDays(-1).DayOfWeek == DayOfWeek.Saturday) || (combinedDate.AddDays(-1).DayOfWeek == DayOfWeek.Sunday))
                //{
                //    Console.WriteLine("ateina--");
                //    anotherCounter++;
                //}
                //if ((combinedDate.AddDays(1).DayOfWeek == DayOfWeek.Saturday) || (combinedDate.AddDays(1).DayOfWeek == DayOfWeek.Sunday))
                //{
                //    //Console.WriteLine("ateina++");
                //    anotherCounter++;
                //}

                //for (int j = 1; j <= 2; j++)
                //{
                //    // jis prides jei bus kita data tipo jei bus is kitos savaitgalio o ne in sequence
                //    // reikia pasiimti new date ir ji valdyti nes kitaip niekaip tipo dienas keisti nes 
                //    if ((combinedDate.AddDays(-j).DayOfWeek == DayOfWeek.Saturday) || (combinedDate.AddDays(-j).DayOfWeek == DayOfWeek.Sunday))
                //    {
                //        Console.WriteLine("ateina--");
                //        anotherCounter++;
                //    }
                //    else
                //    {
                //        break;
                //    }

                //}

                //if(combinedDate == new DateOnly())

                //Console.WriteLine(combinedDate);


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
                        // cia saugoti i list
                        // saugo tik 26d. nes dubliuojasi
                        //Console.WriteLine(new DateOnly(year, month, day));
                        //listOfDates.Add(new DateOnly(year, month, day));



                        // kazkaip issiaiskinti last in sequence and first
                        // reiktu gal pagalvoti kad skippintu tipo ta data su continue
                        if (anotherCounter > 0)
                        {
                            // realiai kaip ir veikia tik reikia su sitais pasizaisti tipo kad jei pirmas tai irgi counter daryti nu nes prideda dienu bereikalingai o turetu tiesiog pirma data paimti ir su ja zaisti ir su paskutine
                            if ((combinedDate.AddDays(i).DayOfWeek == DayOfWeek.Saturday) || (combinedDate.AddDays(i).DayOfWeek == DayOfWeek.Sunday))
                            {
                                //Console.WriteLine("ateina++");
                                //anotherCounter++;
                            }

                            if ((combinedDate.AddDays(-i).DayOfWeek == DayOfWeek.Saturday) || (combinedDate.AddDays(-i).DayOfWeek == DayOfWeek.Sunday))
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
                            //if (day < daysInMonth)
                            //{
                            //    if ((combinedDate.AddDays(i).DayOfWeek == DayOfWeek.Saturday) || (combinedDate.AddDays(i).DayOfWeek == DayOfWeek.Sunday))
                            //    {
                            //        //Console.WriteLine("ateina++");
                            //        ///anotherCounter++;
                            //    }
                            //}

                            //if (day > 1)
                            //{
                            //    if ((combinedDate.AddDays(-i).DayOfWeek == DayOfWeek.Saturday) || (combinedDate.AddDays(-i).DayOfWeek == DayOfWeek.Sunday))
                            //    {
                            //        //Console.WriteLine("ateina--");
                            //        //anotherCounter++;
                            //    }
                            //}
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
                        //listOfDates.Clear();
                        //for (int k = 1; k <= 7; k++)
                        //{
                        //    if ((combinedDate.AddDays(k).DayOfWeek == DayOfWeek.Saturday) || (combinedDate.AddDays(k).DayOfWeek == DayOfWeek.Sunday))
                        //    {
                        //        //Console.WriteLine("ateina++");
                        //        //anotherCounter++;
                        //    }
                        //    else
                        //    {
                        //        break;
                        //    }
                        //}




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

                    if(lastCounter < innerCounter) lastCounter = innerCounter;
                }
                else
                {
                    innerCounter = 0;
                }
            }

            //Console.WriteLine("Max number of free: " + lastCounter);
            //the maximum number of free(free day + holiday) days in a row, which will be by a given country and year

            return lastCounter.ToString();
        }
    }
}
