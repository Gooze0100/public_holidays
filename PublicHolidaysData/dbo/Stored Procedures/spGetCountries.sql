CREATE PROCEDURE [dbo].[spGetCountries]
AS
BEGIN
	SET NOCOUNT ON;
	--In theory need to add foreign keys to other tables to get data
	SELECT CountryCode, Country
	FROM dbo.Countries
	ORDER BY Country 
END