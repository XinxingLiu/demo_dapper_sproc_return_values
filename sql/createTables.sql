DROP TABLE IF EXISTS [dbo].[EnumValue];
DROP TABLE IF EXISTS [dbo].[DataType];

CREATE TABLE [dbo].[DataType] (
    [Id]                 INT             IDENTITY (1, 1) NOT NULL,
    [Name]               NVARCHAR (50)   NOT NULL,
    [ElementTypeId]      INT NULL,
    CONSTRAINT [PK_DataType] PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[EnumValue] (
    [Id]                 INT             IDENTITY (1, 1) NOT NULL,
    [Name]               NVARCHAR (255)   NOT NULL,
    [Value]              INT NOT NULL,  -- integer interpretation of the enum value, currently based on position but extensible to explicit specification later
    [DataTypeId]         INT NOT NULL,
    CONSTRAINT [PK_EnumValue] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EnumValue_DataTypeId] FOREIGN KEY ([DataTypeId]) REFERENCES [dbo].[DataType] ([Id])
);

INSERT INTO [dbo].[DataType] ([Name])
VALUES ('integer');

INSERT INTO [dbo].[EnumValue] ([Name],[Value],[DataTypeId])
VALUES ('state', 12, 1);