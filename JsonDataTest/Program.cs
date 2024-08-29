using JsonDataTest;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text.Json;

try
{
    
    Requests requests = new Requests();

    JArray supportedCountries = await requests.GetCountriesList();

    foreach (JToken country in supportedCountries)
    {
        //Console.WriteLine(country["fullName"]);
    }

    JArray holidaysForYear = await requests.GetAllHolidaysForYears("2024", "ltu");

    foreach (JToken holiday in holidaysForYear)
    {
        //Console.WriteLine(holiday["date"]);
        //Console.WriteLine(item["name"]);

        foreach (JToken name in holiday["name"])
        {
            //Console.WriteLine(name["text"]);
        }
        //Console.WriteLine(item["holidayType"]);
    }

    string dayStatus = await requests.GetDayStatus("2024-08-15", "ltu");
    //Console.WriteLine(dayStatus);

    List<int> getNumOfMaxDays = await requests.GetMaximumNumberOfFreeDays("ltu", "2023");
    Console.WriteLine(getNumOfMaxDays[0]);
    //Console.WriteLine(getNumOfMaxDays);

}
catch(Exception ex)
{
    Console.WriteLine(ex);
}


