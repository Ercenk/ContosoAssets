CREATE TABLE [dbo].[Assets] (
    [Id]             NVARCHAR (450)     NOT NULL,
    [CustomerName]   NVARCHAR (MAX)     NULL,
    [LastUpdateTime] DATETIMEOFFSET (7) NOT NULL,
    [Name]           NVARCHAR (MAX)     NULL,
    [Status]         INT                NOT NULL,
    CONSTRAINT [PK_Assets] PRIMARY KEY CLUSTERED ([Id] ASC)
);

