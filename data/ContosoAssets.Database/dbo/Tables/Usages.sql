CREATE TABLE [dbo].[Usages] (
    [Id]             UNIQUEIDENTIFIER   NOT NULL,
    [Company]        NVARCHAR (MAX)     NULL,
    [CustomerUserId] UNIQUEIDENTIFIER   NOT NULL,
    [Operation]      NVARCHAR (MAX)     NULL,
    [Timestamp]      DATETIMEOFFSET (7) NOT NULL,
    CONSTRAINT [PK_Usages] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Usages_CustomerUsers_CustomerUserId] FOREIGN KEY ([CustomerUserId]) REFERENCES [dbo].[CustomerUsers] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Usages_CustomerUserId]
    ON [dbo].[Usages]([CustomerUserId] ASC);

