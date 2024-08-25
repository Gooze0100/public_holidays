CREATE TABLE [dbo].[MaxNumbersOfFreeDays]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Country] NVARCHAR(3) NOT NULL,
    [Year] INT NOT NULL,
    [NumberOfMaxDays] INT NOT NULL,
    [CreatedDate] DATETIME2 NOT NULL DEFAULT getutcdate(), 
    [UpdatedDate] DATETIME2 NULL , 
    [DeletedDate] DATETIME2 NULL
)
