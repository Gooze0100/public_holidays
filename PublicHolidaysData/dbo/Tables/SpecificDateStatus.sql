CREATE TABLE [dbo].[SpecificDateStatus]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Date] DATETIME2 NOT NULL, 
    [Status] NVARCHAR(60) NOT NULL, 
    [CreatedDate] DATETIME2 NOT NULL DEFAULT getutcdate(), 
    [UpdatedDate] DATETIME2 NULL , 
    [DeletedDate] DATETIME2 NULL
)
