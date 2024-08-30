using Microsoft.OpenApi.Models;
using PublicHolidaysApp;
using RequestsForData.Library;
using System.Reflection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json");
builder.Configuration.AddJsonFile("appsettings.development.json");
builder.Configuration.AddUserSecrets<Program>();
WebApplication app = builder.Build();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Documentation For Holidays Data",
        Version = "v1",
        Description = "Program uses kayaposoft API to write data in MSSQL database and reuse it. Use this link for ISO-3 country codes: https://www.nationsonline.org/oneworld/country_code_list.htm",
    });
    options.SchemaFilter<RemoveSchemas>();

    string xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});



app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

Requests requests = new Requests();

// Check caching and rate limiting
// Countries
app.MapGet("api/countries",
    async () => await requests.GetCountriesList()
).WithOpenApi(
   operation => new(operation)
   {
       Summary = "Here you can get supported Countries list",
       Description = "This endpoint returns Json array of countries full names",
   });

// All Holidays
app.MapGet("api/allHolidays/country/{country}/year/{year}",
    async (string country, string year) => await requests.GetAllHolidaysForYears(country, year)
).WithOpenApi(
    operation => 
    {
        operation.Summary = "You can get all holidays for specific country and year";
        operation.Description = "This endpoint returns all holidays";
        OpenApiParameter country = operation.Parameters[0];
        country.Description = "Enter Country name with ISO-3 standard";
        OpenApiParameter year = operation.Parameters[1];
        year.Description = "Enter Year \"yyyy\"";
        return operation;
    });

// Day Status
app.MapGet("api/dayStatus/country/{country}/date/{date}",
    async (string country, string date) => await requests.GetDayStatus(country, date)
).WithOpenApi(
    operation =>
    {
        operation.Summary = "Here you can get Day status";
        operation.Description = "This endpoint returns day status";
        OpenApiParameter country = operation.Parameters[0];
        country.Description = "Enter Country name with ISO-3 standard";
        OpenApiParameter date = operation.Parameters[1];
        date.Description = "Enter date with type \"yyyy-mm-dd\"";
        return operation;
    });

// Maximum Number of Free Days
app.MapGet("api/maximumNumberofFreeDays/country/{country}/year/{year}",
    async (string country, string year) => await requests.GetMaximumNumberOfFreeDays(country, year)
).WithOpenApi(
    operation =>
    {
        operation.Summary = "Here you can get maximum number of free days in row";
        operation.Description = "This endpoint maximum number of free days in row";
        OpenApiParameter country = operation.Parameters[0];
        country.Description = "Enter Country name with ISO-3 standard";
        OpenApiParameter year = operation.Parameters[1];
        year.Description = "Enter Year \"yyyy\"";
        return operation;
    });

app.Run();