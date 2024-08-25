CREATE TABLE [dbo].[MaxNumbersOfFreeDays]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CountryCode] NVARCHAR(3) NOT NULL,
    [Year] NVARCHAR(4) NOT NULL,
    [NumberOfMaxDays] TINYINT NOT NULL,
    [CreatedDate] DATETIME2 NOT NULL DEFAULT getutcdate(), 
    [UpdatedDate] DATETIME2 NULL , 
    [DeletedDate] DATETIME2 NULL
)
