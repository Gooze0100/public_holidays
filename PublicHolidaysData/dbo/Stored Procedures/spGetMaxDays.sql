CREATE PROCEDURE [dbo].[spGetMaxDays]
	@countryCode nvarchar(3),
	@year nvarchar(4)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT NumberOfMaxDays
	FROM dbo.MaxNumbersOfFreeDays
	WHERE CountryCode = @countryCode AND Year = @year
END