CREATE TABLE [dbo].[Users] (
    [Id]         NVARCHAR (50) NOT NULL,
    [FirstName]  NVARCHAR (50) NOT NULL,
    [LastName]   NVARCHAR (50) NOT NULL,
    [Email]      NVARCHAR (50) NOT NULL,
    [Password]   NVARCHAR (64) NOT NULL,
    [DOB]        NVARCHAR (50) NOT NULL,
    [Image]      NVARCHAR (50) NULL,
    [CardNumber] NVARCHAR (50) NOT NULL,
    [CardExpiry] NVARCHAR (50) NOT NULL,
    [CardCVV]    NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

