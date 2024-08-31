# Public Holidays App

This project was created with [ASP.NET](https://dotnet.microsoft.com/en-us/apps/aspnet) version of .NET 8.0.
This app saves public holidays data from https://kayaposoft.com/enrico/ into SQL server.
It uses four endpoints for:

- countries list
- grouped by a month holidays list for a given country and year
- specific day status(workday, free day, holiday)
- the maximum number of free(free day + holiday) days in a row, which will be by a given country and year

## Installation

- Add MSSQL database "DefaultConnection" string to user secrets in RequestForData.Library, then publish PublicHolidaysData project using that same string.
- Start the application.

## User Guide

In Swagger when application is deployed you can see four endpoints and what parameters to enter to get data. Examples are writtten what data should be entered.

## Configuration

Add user secrets in RequestForData.Library "DefaultConnection" to connect to SQL server and publish PublicHolidaysData project.

## Guidlines for future improvements

- Use foreign keys for countries and other tables because it is same data.
- Implement better unit tests. Unit smoke tests with selenium.
- SQL write stored procedures for updating (Countries), not just wiping information.
- SQL table column types could be changed, to more useful, for year to tinyInt and etc.
- GetMaximumNumberOfFreeDays functions can be better written with grouped months, less loops, etc.
- Use Models to control and get proper data, not just dynamic type.

## License

MIT License

## Learning part

- Need to learn more about Docker.
- Need to learn about cloud services.
