CREATE PROCEDURE [dbo].[spInsertIntoMaxNumberOfFreeDays]
	@countryCode nvarchar(3),
	@year nvarchar(4),
	@numberOfMaxDays tinyint
AS
BEGIN
	INSERT INTO dbo.MaxNumbersOfFreeDays(CountryCode, Year, NumberOfMaxDays)
	VALUES (@countryCode, @year, @numberOfMaxDays)
END