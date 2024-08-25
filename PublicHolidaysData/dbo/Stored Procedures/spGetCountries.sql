CREATE PROCEDURE [dbo].[spGetCountries]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT CountryCode, Country
	FROM dbo.Countries
END