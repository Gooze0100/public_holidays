CREATE PROCEDURE [dbo].[spGetHolidays]
	@countryCode nvarchar(3),
	@year nvarchar(4)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT Year, Month, Day, Name_Origin, Name_ENG
	FROM dbo.Holidays
	WHERE CountryCode = @countryCode AND Year = @year
	--GROUP BY Month nera agreagated todel neapima
END