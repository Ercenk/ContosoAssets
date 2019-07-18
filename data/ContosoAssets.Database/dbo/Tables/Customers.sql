CREATE TABLE [dbo].[Customers] (
    [Id]           UNIQUEIDENTIFIER NOT NULL,
    [CustomerName] NVARCHAR (MAX)   NULL,
    [SkuId]        UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Customers_Skus_SkuId] FOREIGN KEY ([SkuId]) REFERENCES [dbo].[Skus] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Customers_SkuId]
    ON [dbo].[Customers]([SkuId] ASC);

