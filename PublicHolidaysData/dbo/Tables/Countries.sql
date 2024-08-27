CREATE TABLE [dbo].[Countries]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CountryCode] NVARCHAR(3) NOT NULL, 
    [Country] NVARCHAR(MAX) NOT NULL, 
    [CreatedDate] DATETIME2 NOT NULL DEFAULT getutcdate(), 
    [UpdatedDate] DATETIME2 NULL , 
    [DeletedDate] DATETIME2 NULL
    --In theory we need to create Foreign key for other tables that information do not duplicate
)
