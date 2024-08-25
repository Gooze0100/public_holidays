CREATE PROCEDURE [dbo].[spInsertIntoSpecificDateStatus]
	@countryCode nvarchar(3),
	@year nvarchar(4),
	@month nvarchar(2),
	@day nvarchar(2),
	@status nvarchar(60)
AS
BEGIN
	INSERT INTO dbo.SpecificDateStatus(CountryCode, Year, Month, Day, Status)
	VALUES (@countryCode, @year, @month, @day, @status)
END