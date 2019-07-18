CREATE TABLE [dbo].[Skus] (
    [Id]           UNIQUEIDENTIFIER NOT NULL,
    [Description]  NVARCHAR (MAX)   NULL,
    [MonthlyCost]  FLOAT (53)       NOT NULL,
    [MonthlyLimit] INT              NOT NULL,
    [Name]         NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_Skus] PRIMARY KEY CLUSTERED ([Id] ASC)
);

