CREATE PROCEDURE [dbo].[spGetDayStatus]
	@countryCode nvarchar(3),
	@date nvarchar(10)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT Status
	FROM dbo.SpecificDateStatus
	WHERE FORMATMESSAGE('%s-%s-%s', Year, Month, Day) = @date AND CountryCode = @countryCode
END