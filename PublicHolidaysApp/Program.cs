using Microsoft.OpenApi.Models;
using RequestsForData;
using System.Reflection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
OpenApiInfo info = new()
{
    Title = "API Documentation For Holidays Data",
    Version = "v1",
    Description = "Program uses kayaposoft API to write data in MSSQL database reuse it"
};

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    options =>
    {
        options.SwaggerDoc("v1", info);
        string xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    });

WebApplication app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
Requests requests = new Requests();
// caching ir rate limmiting pasiziureti nes tipo kad neuzlaustu programos

//nepaima summary :(
/// <summary>
/// Here you can get supported Countries list.
/// </summary>
/// <returns>This endpoint returns Json array of countries full names</returns>
app.MapGet("api/Countries",
    async () => await requests.GetCountriesList()
);

/// <summary>
/// You can get holidays list for specific country and year.
/// </summary>
/// <param name="country"></params>
/// <param name="year"></params>
/// <returns>This endpoint returns holidays list</returns>
app.MapGet("api/HolidaysList/{country}/{year}",
    (string country, string year) => new { Message = "hello world: " + year, country }
);

/// <summary>
/// Here you can get Day status.
/// </summary>
/// <param name="date"></params>
/// <returns>This endpoint returns day status</returns>
app.MapGet("api/DayStatus/{date}",
    (string date) => new { Message = "hello world: " + date }
);

/// <summary>
/// Here you can get maximum number of free days in row.
/// </summary>
/// <param name="country"></params>
/// <param name="year"></params>
/// <returns>This endpoint maximum number of free days in row</returns>
app.MapGet("api/MaximumNumberofFreeDays/{country}/{year}",
    (string country, string year) => new { Message = "hello world: " + country }
)

//.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();