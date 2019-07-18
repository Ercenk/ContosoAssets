CREATE TABLE [dbo].[CustomerUsers] (
    [Id]             UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]    DATETIMEOFFSET (7) NOT NULL,
    [CustomerId]     UNIQUEIDENTIFIER   NOT NULL,
    [CustomerName]   NVARCHAR (MAX)     NULL,
    [CustomerRegion] INT                NULL,
    [FullName]       NVARCHAR (MAX)     NULL,
    [UserName]       NVARCHAR (MAX)     NULL,
    CONSTRAINT [PK_CustomerUsers] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CustomerUsers_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_CustomerUsers_CustomerId]
    ON [dbo].[CustomerUsers]([CustomerId] ASC);

