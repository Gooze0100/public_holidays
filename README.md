# Public Holidays App

This project was created with [ASP.NET](https://dotnet.microsoft.com/en-us/apps/aspnet) version of .NET 8.0.
This app saves public holidays data from https://kayaposoft.com/enrico/ into SQL server.
It uses three endpoints for:

- countries list
- grouped by a month holidays list for a given country and year
- specific day status(workday, free day, holiday)
- the maximum number of free(free day + holiday) days in a row, which will be by a given country and year

## Installation

- Add MSSQL database "DefaultConnection" string to user secrets then Publish PublicHolidaysData project.
- Start the application in Docker.

## User Guide

In Swagger when application is deployes you can see four endpoints and what parameters to enter to get that data.

## Configuration

MSSQL server is used localy. Not implemented in Docker as it should be.

## Guidlines for future improvements

- Use foreign keys to countries and other tables because it is same data.
- Implement better unit tests.
- SQL write stored procedures for updates, and update values if not found for countries.
- SQL table column types could be changed, to more useful, for year to tinyInt and etc.
- GetMaximumNumberOfFreeDays functions can be better written with grouped months, less loops, etc.

My worst parts is that I don't know Docker a lot and how to work with. :(

## License

MIT License
