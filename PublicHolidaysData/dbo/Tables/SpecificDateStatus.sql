CREATE TABLE [dbo].[SpecificDateStatus]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CountryCode] NVARCHAR(3) NOT NULL, 
    [Year] NVARCHAR(4) NOT NULL,
    [Month] NVARCHAR(2) NOT NULL, 
    [Day] NVARCHAR(2) NOT NULL, 
    [Status] NVARCHAR(60) NOT NULL, 
    [CreatedDate] DATETIME2 NOT NULL DEFAULT getutcdate(), 
    [UpdatedDate] DATETIME2 NULL , 
    [DeletedDate] DATETIME2 NULL
)
