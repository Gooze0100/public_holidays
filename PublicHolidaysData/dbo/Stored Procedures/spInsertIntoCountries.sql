CREATE PROCEDURE [dbo].[spInsertIntoCountries]
	@countryCode nvarchar(3),
	@country nvarchar(MAX)
AS
BEGIN
	INSERT INTO dbo.Countries (CountryCode, Country)
	VALUES (@countryCode, @country)
END