CREATE PROCEDURE [dbo].[spInsertIntoHolidays]
	@countryCode nvarchar(3),
	@year nvarchar(4),
	@month nvarchar(2),
	@day nvarchar(2),
	@name_origin nvarchar(250),
	@name_eng nvarchar(250),
	@holidayType nvarchar(250)
AS
BEGIN
	INSERT INTO dbo.Holidays(CountryCode, Year, Month, Day, Name_Origin, Name_ENG, HolidayType)
	VALUES (@countryCode, @year, @month, @day, @name_origin, @name_eng, @holidayType)
END