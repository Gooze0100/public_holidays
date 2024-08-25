using Microsoft.OpenApi.Models;
using RequestsForData.Library;
using System.Reflection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Documentation For Holidays Data",
        Version = "v1",
        Description = "Program uses kayaposoft API to write data in MSSQL database and reuse it"
    });

    string xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

WebApplication app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
Requests requests = new Requests();
// caching ir rate limmiting pasiziureti nes tipo kad neuzlaustu programos

app.MapGet("api/countries",
    async () => await requests.GetCountriesList()
).WithOpenApi(
   operation => new(operation)
   {
       Summary = "Here you can get supported Countries list",
       Description = "This endpoint returns Json array of countries full names",
   });

app.MapGet("api/holidaysList/country/{country}/year/{year}",
    (string country, string year) => new { Message = "hello world: " + year, country }
).WithOpenApi(
    operation => 
    {
        operation.Summary = "You can get holidays list for specific country and year";
        operation.Description = "This endpoint returns holidays list";
        OpenApiParameter country = operation.Parameters[0];
        country.Description = "Enter Country name with ISO-3 standard";
        OpenApiParameter year = operation.Parameters[1];
        year.Description = "Enter Year \"yyyy\"";
        return operation;
    });

app.MapGet("api/dayStatus/date/{date}",
    (string date) => new { Message = "hello world: " + date }
).WithOpenApi(
    operation =>
    {
        operation.Summary = "Here you can get Day status";
        operation.Description = "This endpoint returns day status";
        OpenApiParameter date = operation.Parameters[0];
        date.Description = "Enter date with type \"yyyy-mm-dd\"";
        return operation;
    });

app.MapGet("api/maximumNumberofFreeDays/country/{country}/year/{year}",
    (string country, string year) => new { Message = "hello world: " + country }
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