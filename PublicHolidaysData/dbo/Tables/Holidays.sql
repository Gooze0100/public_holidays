CREATE TABLE [dbo].[Holidays]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
    [Country] NVARCHAR(250) NOT NULL, 
    [Date] DATETIME2 NOT NULL, 
    [Name_Origin] NVARCHAR(250) NOT NULL, 
    [Name_ENG] NVARCHAR(250) NOT NULL,
    [HolidayType] NVARCHAR(250) NOT NULL,
    [CreatedDate] DATETIME2 NOT NULL DEFAULT getutcdate(), 
    [UpdatedDate] DATETIME2, 
    [DeletedDate] DATETIME2

)
