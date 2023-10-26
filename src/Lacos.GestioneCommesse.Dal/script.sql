IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    IF SCHEMA_ID(N'Docs') IS NULL EXEC(N'CREATE SCHEMA [Docs];');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    IF SCHEMA_ID(N'Registry') IS NULL EXEC(N'CREATE SCHEMA [Registry];');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    IF SCHEMA_ID(N'Security') IS NULL EXEC(N'CREATE SCHEMA [Security];');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE TABLE [Registry].[ActivityTypes] (
        [Id] bigint NOT NULL IDENTITY,
        [Name] nvarchar(200) NOT NULL,
        [PictureRequired] bit NOT NULL,
        [CreatedOn] datetimeoffset(3) NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [CreatedById] bigint NULL,
        [EditedOn] datetimeoffset(3) NULL,
        [EditedBy] nvarchar(max) NULL,
        [EditedById] bigint NULL,
        [DeletedOn] datetimeoffset(3) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedById] bigint NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_ActivityTypes] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE TABLE [Registry].[ProductTypes] (
        [Id] bigint NOT NULL IDENTITY,
        [Code] nvarchar(50) NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Description] nvarchar(max) NULL,
        [IsReiDoor] bit NOT NULL,
        [IsSparePart] bit NOT NULL,
        [CreatedOn] datetimeoffset(3) NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [CreatedById] bigint NULL,
        [EditedOn] datetimeoffset(3) NULL,
        [EditedBy] nvarchar(max) NULL,
        [EditedById] bigint NULL,
        [DeletedOn] datetimeoffset(3) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedById] bigint NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_ProductTypes] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE TABLE [Security].[Users] (
        [Id] bigint NOT NULL IDENTITY,
        [UserName] nvarchar(64) NOT NULL,
        [PasswordHash] nvarchar(64) NOT NULL,
        [Salt] nvarchar(64) NOT NULL,
        [AccessToken] nvarchar(64) NOT NULL,
        [Enabled] bit NOT NULL,
        [Role] int NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE TABLE [Registry].[Vehicles] (
        [Id] bigint NOT NULL IDENTITY,
        [Name] nvarchar(200) NOT NULL,
        [Plate] nvarchar(20) NULL,
        [Notes] nvarchar(max) NULL,
        [CreatedOn] datetimeoffset(3) NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [CreatedById] bigint NULL,
        [EditedOn] datetimeoffset(3) NULL,
        [EditedBy] nvarchar(max) NULL,
        [EditedById] bigint NULL,
        [DeletedOn] datetimeoffset(3) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedById] bigint NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Vehicles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE TABLE [Registry].[CheckLists] (
        [Id] bigint NOT NULL IDENTITY,
        [PictureFileName] nvarchar(50) NULL,
        [Description] nvarchar(max) NOT NULL,
        [ProductTypeId] bigint NOT NULL,
        [ActivityTypeId] bigint NOT NULL,
        [CreatedOn] datetimeoffset(3) NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [CreatedById] bigint NULL,
        [EditedOn] datetimeoffset(3) NULL,
        [EditedBy] nvarchar(max) NULL,
        [EditedById] bigint NULL,
        [DeletedOn] datetimeoffset(3) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedById] bigint NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_CheckLists] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CheckLists_ActivityTypes_ActivityTypeId] FOREIGN KEY ([ActivityTypeId]) REFERENCES [Registry].[ActivityTypes] ([Id]),
        CONSTRAINT [FK_CheckLists_ProductTypes_ProductTypeId] FOREIGN KEY ([ProductTypeId]) REFERENCES [Registry].[ProductTypes] ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE TABLE [Registry].[Customers] (
        [Id] bigint NOT NULL IDENTITY,
        [Name] nvarchar(200) NOT NULL,
        [Notes] nvarchar(max) NULL,
        [FiscalType] int NOT NULL,
        [CanGenerateTickets] bit NOT NULL,
        [UserId] bigint NULL,
        [CreatedOn] datetimeoffset(3) NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [CreatedById] bigint NULL,
        [EditedOn] datetimeoffset(3) NULL,
        [EditedBy] nvarchar(max) NULL,
        [EditedById] bigint NULL,
        [DeletedOn] datetimeoffset(3) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedById] bigint NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Customers] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Customers_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Security].[Users] ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE TABLE [Registry].[Operators] (
        [Id] bigint NOT NULL IDENTITY,
        [Email] nvarchar(200) NULL,
        [ColorHex] nvarchar(7) NULL,
        [Name] nvarchar(200) NOT NULL,
        [DefaultVehicleId] bigint NULL,
        [UserId] bigint NOT NULL,
        [CreatedOn] datetimeoffset(3) NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [CreatedById] bigint NULL,
        [EditedOn] datetimeoffset(3) NULL,
        [EditedBy] nvarchar(max) NULL,
        [EditedById] bigint NULL,
        [DeletedOn] datetimeoffset(3) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedById] bigint NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Operators] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Operators_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Security].[Users] ([Id]),
        CONSTRAINT [FK_Operators_Vehicles_DefaultVehicleId] FOREIGN KEY ([DefaultVehicleId]) REFERENCES [Registry].[Vehicles] ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE TABLE [Registry].[CheckListItems] (
        [Id] bigint NOT NULL IDENTITY,
        [Description] nvarchar(500) NOT NULL,
        [CheckListId] bigint NOT NULL,
        [CreatedOn] datetimeoffset(3) NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [CreatedById] bigint NULL,
        [EditedOn] datetimeoffset(3) NULL,
        [EditedBy] nvarchar(max) NULL,
        [EditedById] bigint NULL,
        [DeletedOn] datetimeoffset(3) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedById] bigint NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_CheckListItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CheckListItems_CheckLists_CheckListId] FOREIGN KEY ([CheckListId]) REFERENCES [Registry].[CheckLists] ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE TABLE [Registry].[CustomerAddresses] (
        [Id] bigint NOT NULL IDENTITY,
        [Description] nvarchar(200) NOT NULL,
        [City] nvarchar(200) NOT NULL,
        [StreetAddress] nvarchar(200) NOT NULL,
        [Province] nvarchar(200) NOT NULL,
        [ZipCode] nvarchar(5) NOT NULL,
        [Telephone] nvarchar(20) NULL,
        [Email] nvarchar(200) NULL,
        [IsMainAddress] bit NOT NULL,
        [Notes] nvarchar(max) NULL,
        [CustomerId] bigint NOT NULL,
        [CreatedOn] datetimeoffset(3) NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [CreatedById] bigint NULL,
        [EditedOn] datetimeoffset(3) NULL,
        [EditedBy] nvarchar(max) NULL,
        [EditedById] bigint NULL,
        [DeletedOn] datetimeoffset(3) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedById] bigint NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_CustomerAddresses] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CustomerAddresses_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Registry].[Customers] ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE TABLE [Docs].[Jobs] (
        [Id] bigint NOT NULL IDENTITY,
        [Status] int NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [CustomerId] bigint NOT NULL,
        [CreatedOn] datetimeoffset(3) NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [CreatedById] bigint NULL,
        [EditedOn] datetimeoffset(3) NULL,
        [EditedBy] nvarchar(max) NULL,
        [EditedById] bigint NULL,
        [DeletedOn] datetimeoffset(3) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedById] bigint NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Jobs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Jobs_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Registry].[Customers] ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE TABLE [Registry].[OperatorDocuments] (
        [Id] bigint NOT NULL IDENTITY,
        [Description] nvarchar(200) NOT NULL,
        [FileName] nvarchar(50) NOT NULL,
        [OperatorId] bigint NOT NULL,
        [CreatedOn] datetimeoffset(3) NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [CreatedById] bigint NULL,
        [EditedOn] datetimeoffset(3) NULL,
        [EditedBy] nvarchar(max) NULL,
        [EditedById] bigint NULL,
        [DeletedOn] datetimeoffset(3) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedById] bigint NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_OperatorDocuments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_OperatorDocuments_Operators_OperatorId] FOREIGN KEY ([OperatorId]) REFERENCES [Registry].[Operators] ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE TABLE [Registry].[Products] (
        [Id] bigint NOT NULL IDENTITY,
        [Code] nvarchar(50) NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Description] nvarchar(max) NULL,
        [PictureFileName] nvarchar(50) NULL,
        [QrCode] nvarchar(50) NULL,
        [CustomerId] bigint NULL,
        [CustomerAddressId] bigint NULL,
        [ProductTypeId] bigint NOT NULL,
        [CreatedOn] datetimeoffset(3) NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [CreatedById] bigint NULL,
        [EditedOn] datetimeoffset(3) NULL,
        [EditedBy] nvarchar(max) NULL,
        [EditedById] bigint NULL,
        [DeletedOn] datetimeoffset(3) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedById] bigint NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Products] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Products_CustomerAddresses_CustomerAddressId] FOREIGN KEY ([CustomerAddressId]) REFERENCES [Registry].[CustomerAddresses] ([Id]),
        CONSTRAINT [FK_Products_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Registry].[Customers] ([Id]),
        CONSTRAINT [FK_Products_ProductTypes_ProductTypeId] FOREIGN KEY ([ProductTypeId]) REFERENCES [Registry].[ProductTypes] ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE TABLE [Registry].[ProductDocuments] (
        [Id] bigint NOT NULL IDENTITY,
        [FileName] nvarchar(50) NOT NULL,
        [Description] nvarchar(200) NOT NULL,
        [ProductId] bigint NOT NULL,
        [CreatedOn] datetimeoffset(3) NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [CreatedById] bigint NULL,
        [EditedOn] datetimeoffset(3) NULL,
        [EditedBy] nvarchar(max) NULL,
        [EditedById] bigint NULL,
        [DeletedOn] datetimeoffset(3) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedById] bigint NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_ProductDocuments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ProductDocuments_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Registry].[Products] ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE TABLE [Docs].[Activities] (
        [Id] bigint NOT NULL IDENTITY,
        [Status] int NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [CustomerAddressId] bigint NOT NULL,
        [JobId] bigint NOT NULL,
        [TypeId] bigint NOT NULL,
        [SourceTicketId] bigint NULL,
        [SourcePuchaseOrderId] bigint NULL,
        [CreatedOn] datetimeoffset(3) NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [CreatedById] bigint NULL,
        [EditedOn] datetimeoffset(3) NULL,
        [EditedBy] nvarchar(max) NULL,
        [EditedById] bigint NULL,
        [DeletedOn] datetimeoffset(3) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedById] bigint NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Activities] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Activities_ActivityTypes_TypeId] FOREIGN KEY ([TypeId]) REFERENCES [Registry].[ActivityTypes] ([Id]),
        CONSTRAINT [FK_Activities_CustomerAddresses_CustomerAddressId] FOREIGN KEY ([CustomerAddressId]) REFERENCES [Registry].[CustomerAddresses] ([Id]),
        CONSTRAINT [FK_Activities_Jobs_JobId] FOREIGN KEY ([JobId]) REFERENCES [Docs].[Jobs] ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE TABLE [Docs].[Interventions] (
        [Id] bigint NOT NULL IDENTITY,
        [Start] datetimeoffset(3) NOT NULL,
        [End] datetimeoffset(3) NOT NULL,
        [Status] int NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [FinalNotes] nvarchar(max) NULL,
        [ReportFileName] nvarchar(50) NULL,
        [ReportGeneratedOn] datetimeoffset(3) NULL,
        [CustomerSignatureFileName] nvarchar(50) NULL,
        [VehicleId] bigint NULL,
        [ActivityId] bigint NULL,
        [CreatedOn] datetimeoffset(3) NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [CreatedById] bigint NULL,
        [EditedOn] datetimeoffset(3) NULL,
        [EditedBy] nvarchar(max) NULL,
        [EditedById] bigint NULL,
        [DeletedOn] datetimeoffset(3) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedById] bigint NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Interventions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Interventions_Activities_ActivityId] FOREIGN KEY ([ActivityId]) REFERENCES [Docs].[Activities] ([Id]),
        CONSTRAINT [FK_Interventions_Vehicles_VehicleId] FOREIGN KEY ([VehicleId]) REFERENCES [Registry].[Vehicles] ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE TABLE [Docs].[InterventionDisputes] (
        [Id] bigint NOT NULL IDENTITY,
        [Description] nvarchar(max) NOT NULL,
        [InterventionId] bigint NOT NULL,
        [CreatedOn] datetimeoffset(3) NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [CreatedById] bigint NULL,
        [EditedOn] datetimeoffset(3) NULL,
        [EditedBy] nvarchar(max) NULL,
        [EditedById] bigint NULL,
        [DeletedOn] datetimeoffset(3) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedById] bigint NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_InterventionDisputes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_InterventionDisputes_Interventions_InterventionId] FOREIGN KEY ([InterventionId]) REFERENCES [Docs].[Interventions] ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE TABLE [Docs].[InterventionNotes] (
        [Id] bigint NOT NULL IDENTITY,
        [PictureFileName] nvarchar(50) NULL,
        [Notes] nvarchar(max) NOT NULL,
        [OperatorId] bigint NOT NULL,
        [InterventionId] bigint NOT NULL,
        [CreatedOn] datetimeoffset(3) NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [CreatedById] bigint NULL,
        [EditedOn] datetimeoffset(3) NULL,
        [EditedBy] nvarchar(max) NULL,
        [EditedById] bigint NULL,
        [DeletedOn] datetimeoffset(3) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedById] bigint NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_InterventionNotes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_InterventionNotes_Interventions_InterventionId] FOREIGN KEY ([InterventionId]) REFERENCES [Docs].[Interventions] ([Id]),
        CONSTRAINT [FK_InterventionNotes_Operators_OperatorId] FOREIGN KEY ([OperatorId]) REFERENCES [Registry].[Operators] ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE TABLE [Docs].[InterventionOperators] (
        [InterventionsId] bigint NOT NULL,
        [OperatorsId] bigint NOT NULL,
        CONSTRAINT [PK_InterventionOperators] PRIMARY KEY ([InterventionsId], [OperatorsId]),
        CONSTRAINT [FK_InterventionOperators_Interventions_InterventionsId] FOREIGN KEY ([InterventionsId]) REFERENCES [Docs].[Interventions] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_InterventionOperators_Operators_OperatorsId] FOREIGN KEY ([OperatorsId]) REFERENCES [Registry].[Operators] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE TABLE [Docs].[InterventionProducts] (
        [Id] bigint NOT NULL IDENTITY,
        [ProductId] bigint NOT NULL,
        [InterventionId] bigint NOT NULL,
        [CreatedOn] datetimeoffset(3) NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [CreatedById] bigint NULL,
        [EditedOn] datetimeoffset(3) NULL,
        [EditedBy] nvarchar(max) NULL,
        [EditedById] bigint NULL,
        [DeletedOn] datetimeoffset(3) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedById] bigint NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_InterventionProducts] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_InterventionProducts_Interventions_InterventionId] FOREIGN KEY ([InterventionId]) REFERENCES [Docs].[Interventions] ([Id]),
        CONSTRAINT [FK_InterventionProducts_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Registry].[Products] ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE TABLE [Docs].[PurchaseOrders] (
        [Id] bigint NOT NULL IDENTITY,
        [Description] nvarchar(max) NOT NULL,
        [Status] int NOT NULL,
        [InterventionId] bigint NULL,
        [CustomerId] bigint NOT NULL,
        [CreatedOn] datetimeoffset(3) NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [CreatedById] bigint NULL,
        [EditedOn] datetimeoffset(3) NULL,
        [EditedBy] nvarchar(max) NULL,
        [EditedById] bigint NULL,
        [DeletedOn] datetimeoffset(3) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedById] bigint NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_PurchaseOrders] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PurchaseOrders_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Registry].[Customers] ([Id]),
        CONSTRAINT [FK_PurchaseOrders_Interventions_InterventionId] FOREIGN KEY ([InterventionId]) REFERENCES [Docs].[Interventions] ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE TABLE [Docs].[Tickets] (
        [Id] bigint NOT NULL IDENTITY,
        [Description] nvarchar(max) NOT NULL,
        [Status] int NOT NULL,
        [InterventionId] bigint NULL,
        [CustomerId] bigint NOT NULL,
        [CustomerAddressId] bigint NOT NULL,
        [CreatedOn] datetimeoffset(3) NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [CreatedById] bigint NULL,
        [EditedOn] datetimeoffset(3) NULL,
        [EditedBy] nvarchar(max) NULL,
        [EditedById] bigint NULL,
        [DeletedOn] datetimeoffset(3) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedById] bigint NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Tickets] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Tickets_CustomerAddresses_CustomerAddressId] FOREIGN KEY ([CustomerAddressId]) REFERENCES [Registry].[CustomerAddresses] ([Id]),
        CONSTRAINT [FK_Tickets_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Registry].[Customers] ([Id]),
        CONSTRAINT [FK_Tickets_Interventions_InterventionId] FOREIGN KEY ([InterventionId]) REFERENCES [Docs].[Interventions] ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE TABLE [Docs].[InterventionProductCheckLists] (
        [Id] bigint NOT NULL IDENTITY,
        [Description] nvarchar(500) NOT NULL,
        [CustomerSignatureFileName] nvarchar(50) NULL,
        [Notes] nvarchar(max) NULL,
        [InterventionProductId] bigint NOT NULL,
        [CreatedOn] datetimeoffset(3) NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [CreatedById] bigint NULL,
        [EditedOn] datetimeoffset(3) NULL,
        [EditedBy] nvarchar(max) NULL,
        [EditedById] bigint NULL,
        [DeletedOn] datetimeoffset(3) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedById] bigint NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_InterventionProductCheckLists] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_InterventionProductCheckLists_InterventionProducts_InterventionProductId] FOREIGN KEY ([InterventionProductId]) REFERENCES [Docs].[InterventionProducts] ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE TABLE [Docs].[InterventionProductPictures] (
        [Id] bigint NOT NULL IDENTITY,
        [FileName] nvarchar(50) NULL,
        [Type] int NOT NULL,
        [Notes] nvarchar(max) NULL,
        [OperatorId] bigint NOT NULL,
        [InterventionProductId] bigint NOT NULL,
        [CreatedOn] datetimeoffset(3) NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [CreatedById] bigint NULL,
        [EditedOn] datetimeoffset(3) NULL,
        [EditedBy] nvarchar(max) NULL,
        [EditedById] bigint NULL,
        [DeletedOn] datetimeoffset(3) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedById] bigint NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_InterventionProductPictures] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_InterventionProductPictures_InterventionProducts_InterventionProductId] FOREIGN KEY ([InterventionProductId]) REFERENCES [Docs].[InterventionProducts] ([Id]),
        CONSTRAINT [FK_InterventionProductPictures_Operators_OperatorId] FOREIGN KEY ([OperatorId]) REFERENCES [Registry].[Operators] ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE TABLE [Docs].[PurchaseOrderItems] (
        [Id] bigint NOT NULL IDENTITY,
        [ProductId] bigint NOT NULL,
        [Quantity] decimal(14,2) NOT NULL,
        [PurchaseOrderId] bigint NOT NULL,
        [CreatedOn] datetimeoffset(3) NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [CreatedById] bigint NULL,
        [EditedOn] datetimeoffset(3) NULL,
        [EditedBy] nvarchar(max) NULL,
        [EditedById] bigint NULL,
        [DeletedOn] datetimeoffset(3) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedById] bigint NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_PurchaseOrderItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PurchaseOrderItems_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Registry].[Products] ([Id]),
        CONSTRAINT [FK_PurchaseOrderItems_PurchaseOrders_PurchaseOrderId] FOREIGN KEY ([PurchaseOrderId]) REFERENCES [Docs].[PurchaseOrders] ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE TABLE [Docs].[TicketPictures] (
        [Id] bigint NOT NULL IDENTITY,
        [FileName] nvarchar(50) NOT NULL,
        [Description] nvarchar(max) NULL,
        [TicketId] bigint NOT NULL,
        [CreatedOn] datetimeoffset(3) NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [CreatedById] bigint NULL,
        [EditedOn] datetimeoffset(3) NULL,
        [EditedBy] nvarchar(max) NULL,
        [EditedById] bigint NULL,
        [DeletedOn] datetimeoffset(3) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedById] bigint NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_TicketPictures] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TicketPictures_Tickets_TicketId] FOREIGN KEY ([TicketId]) REFERENCES [Docs].[Tickets] ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE TABLE [InterventionProductCheckListItem] (
        [Id] bigint NOT NULL IDENTITY,
        [Description] nvarchar(max) NULL,
        [Outcome] int NULL,
        [Notes] nvarchar(max) NULL,
        [OperatorId] bigint NULL,
        [CheckListId] bigint NOT NULL,
        [CreatedOn] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [CreatedById] bigint NULL,
        [EditedOn] datetimeoffset NULL,
        [EditedBy] nvarchar(max) NULL,
        [EditedById] bigint NULL,
        [DeletedOn] datetimeoffset NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedById] bigint NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_InterventionProductCheckListItem] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_InterventionProductCheckListItem_InterventionProductCheckLists_CheckListId] FOREIGN KEY ([CheckListId]) REFERENCES [Docs].[InterventionProductCheckLists] ([Id]),
        CONSTRAINT [FK_InterventionProductCheckListItem_Operators_OperatorId] FOREIGN KEY ([OperatorId]) REFERENCES [Registry].[Operators] ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AccessToken', N'Enabled', N'PasswordHash', N'Role', N'Salt', N'UserName') AND [object_id] = OBJECT_ID(N'[Security].[Users]'))
        SET IDENTITY_INSERT [Security].[Users] ON;
    EXEC(N'INSERT INTO [Security].[Users] ([Id], [AccessToken], [Enabled], [PasswordHash], [Role], [Salt], [UserName])
    VALUES (CAST(1 AS bigint), N''a0f0a2ffd0f37c955fda023ed287c12fab375bfc0c3e58f96114c9eeb20066b0'', CAST(1 AS bit), N''5d389939f8207210ff712c13f7fe683928cb4c2a5ace9b00fcfe2040be96bbb2'', 0, N''f3064d73de0ca6b806ad24df65a59e1eb692393fc3f0b0297e37df522610b58b'', N''administrator'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AccessToken', N'Enabled', N'PasswordHash', N'Role', N'Salt', N'UserName') AND [object_id] = OBJECT_ID(N'[Security].[Users]'))
        SET IDENTITY_INSERT [Security].[Users] OFF;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE INDEX [IX_Activities_CustomerAddressId] ON [Docs].[Activities] ([CustomerAddressId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE INDEX [IX_Activities_JobId] ON [Docs].[Activities] ([JobId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Activities_SourcePuchaseOrderId] ON [Docs].[Activities] ([SourcePuchaseOrderId]) WHERE [SourcePuchaseOrderId] IS NOT NULL');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Activities_SourceTicketId] ON [Docs].[Activities] ([SourceTicketId]) WHERE [SourceTicketId] IS NOT NULL');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE INDEX [IX_Activities_TypeId] ON [Docs].[Activities] ([TypeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE INDEX [IX_CheckListItems_CheckListId] ON [Registry].[CheckListItems] ([CheckListId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE INDEX [IX_CheckLists_ActivityTypeId] ON [Registry].[CheckLists] ([ActivityTypeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE INDEX [IX_CheckLists_ProductTypeId] ON [Registry].[CheckLists] ([ProductTypeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE INDEX [IX_CustomerAddresses_CustomerId] ON [Registry].[CustomerAddresses] ([CustomerId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Customers_UserId] ON [Registry].[Customers] ([UserId]) WHERE [UserId] IS NOT NULL');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE INDEX [IX_InterventionDisputes_InterventionId] ON [Docs].[InterventionDisputes] ([InterventionId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE INDEX [IX_InterventionNotes_InterventionId] ON [Docs].[InterventionNotes] ([InterventionId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE INDEX [IX_InterventionNotes_OperatorId] ON [Docs].[InterventionNotes] ([OperatorId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE INDEX [IX_InterventionOperators_OperatorsId] ON [Docs].[InterventionOperators] ([OperatorsId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE INDEX [IX_InterventionProductCheckListItem_CheckListId] ON [InterventionProductCheckListItem] ([CheckListId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE INDEX [IX_InterventionProductCheckListItem_OperatorId] ON [InterventionProductCheckListItem] ([OperatorId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE UNIQUE INDEX [IX_InterventionProductCheckLists_InterventionProductId] ON [Docs].[InterventionProductCheckLists] ([InterventionProductId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE INDEX [IX_InterventionProductPictures_InterventionProductId] ON [Docs].[InterventionProductPictures] ([InterventionProductId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE INDEX [IX_InterventionProductPictures_OperatorId] ON [Docs].[InterventionProductPictures] ([OperatorId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE INDEX [IX_InterventionProducts_InterventionId] ON [Docs].[InterventionProducts] ([InterventionId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE INDEX [IX_InterventionProducts_ProductId] ON [Docs].[InterventionProducts] ([ProductId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE INDEX [IX_Interventions_ActivityId] ON [Docs].[Interventions] ([ActivityId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE INDEX [IX_Interventions_VehicleId] ON [Docs].[Interventions] ([VehicleId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE INDEX [IX_Jobs_CustomerId] ON [Docs].[Jobs] ([CustomerId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE INDEX [IX_OperatorDocuments_OperatorId] ON [Registry].[OperatorDocuments] ([OperatorId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE INDEX [IX_Operators_DefaultVehicleId] ON [Registry].[Operators] ([DefaultVehicleId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE UNIQUE INDEX [IX_Operators_UserId] ON [Registry].[Operators] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE INDEX [IX_ProductDocuments_ProductId] ON [Registry].[ProductDocuments] ([ProductId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE INDEX [IX_Products_CustomerAddressId] ON [Registry].[Products] ([CustomerAddressId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE INDEX [IX_Products_CustomerId] ON [Registry].[Products] ([CustomerId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE INDEX [IX_Products_ProductTypeId] ON [Registry].[Products] ([ProductTypeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE INDEX [IX_PurchaseOrderItems_ProductId] ON [Docs].[PurchaseOrderItems] ([ProductId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE INDEX [IX_PurchaseOrderItems_PurchaseOrderId] ON [Docs].[PurchaseOrderItems] ([PurchaseOrderId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE INDEX [IX_PurchaseOrders_CustomerId] ON [Docs].[PurchaseOrders] ([CustomerId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE INDEX [IX_PurchaseOrders_InterventionId] ON [Docs].[PurchaseOrders] ([InterventionId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE INDEX [IX_TicketPictures_TicketId] ON [Docs].[TicketPictures] ([TicketId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE INDEX [IX_Tickets_CustomerAddressId] ON [Docs].[Tickets] ([CustomerAddressId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE INDEX [IX_Tickets_CustomerId] ON [Docs].[Tickets] ([CustomerId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    CREATE INDEX [IX_Tickets_InterventionId] ON [Docs].[Tickets] ([InterventionId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    ALTER TABLE [Docs].[Activities] ADD CONSTRAINT [FK_Activities_PurchaseOrders_SourcePuchaseOrderId] FOREIGN KEY ([SourcePuchaseOrderId]) REFERENCES [Docs].[PurchaseOrders] ([Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    ALTER TABLE [Docs].[Activities] ADD CONSTRAINT [FK_Activities_Tickets_SourceTicketId] FOREIGN KEY ([SourceTicketId]) REFERENCES [Docs].[Tickets] ([Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230628162247_Initial')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230628162247_Initial', N'7.0.10');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230817132438_release1')
BEGIN
    DROP INDEX [IX_Operators_UserId] ON [Registry].[Operators];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230817132438_release1')
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Registry].[Operators]') AND [c].[name] = N'UserId');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Registry].[Operators] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [Registry].[Operators] ALTER COLUMN [UserId] bigint NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230817132438_release1')
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Operators_UserId] ON [Registry].[Operators] ([UserId]) WHERE [UserId] IS NOT NULL');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230817132438_release1')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230817132438_release1', N'7.0.10');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230818075037_release2')
BEGIN
    EXEC sp_rename N'[Registry].[OperatorDocuments].[Description]', N'OriginalFilename', N'COLUMN';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230818075037_release2')
BEGIN
    ALTER TABLE [Registry].[Customers] ADD [Email] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230818075037_release2')
BEGIN
    ALTER TABLE [Registry].[Customers] ADD [Telephone] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230818075037_release2')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230818075037_release2', N'7.0.10');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230818142131_release3')
BEGIN
    ALTER TABLE [Security].[Users] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230818142131_release3')
BEGIN
    EXEC(N'UPDATE [Security].[Users] SET [IsDeleted] = CAST(0 AS bit)
    WHERE [Id] = CAST(1 AS bigint);
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230818142131_release3')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230818142131_release3', N'7.0.10');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230818153257_release4')
BEGIN
    ALTER TABLE [Docs].[Jobs] ADD [JobDate] datetimeoffset(3) NOT NULL DEFAULT '0001-01-01T00:00:00.000+00:00';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230818153257_release4')
BEGIN
    ALTER TABLE [Docs].[Jobs] ADD [Number] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230818153257_release4')
BEGIN
    ALTER TABLE [Docs].[Jobs] ADD [Year] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230818153257_release4')
BEGIN
    ALTER TABLE [Docs].[Activities] ADD [RowNumber] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230818153257_release4')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230818153257_release4', N'7.0.10');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230821155713_release5')
BEGIN
    ALTER TABLE [Registry].[ProductDocuments] ADD [OriginalFilename] nvarchar(200) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230821155713_release5')
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Docs].[InterventionProducts]') AND [c].[name] = N'InterventionId');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Docs].[InterventionProducts] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [Docs].[InterventionProducts] ALTER COLUMN [InterventionId] bigint NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230821155713_release5')
BEGIN
    ALTER TABLE [Docs].[InterventionProducts] ADD [ActivityId] bigint NOT NULL DEFAULT CAST(0 AS bigint);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230821155713_release5')
BEGIN
    CREATE INDEX [IX_InterventionProducts_ActivityId] ON [Docs].[InterventionProducts] ([ActivityId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230821155713_release5')
BEGIN
    ALTER TABLE [Docs].[InterventionProducts] ADD CONSTRAINT [FK_InterventionProducts_Activities_ActivityId] FOREIGN KEY ([ActivityId]) REFERENCES [Docs].[Activities] ([Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230821155713_release5')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230821155713_release5', N'7.0.10');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230822130835_release6')
BEGIN
    ALTER TABLE [Registry].[Products] ADD [Constructor] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230822130835_release6')
BEGIN
    ALTER TABLE [Registry].[Products] ADD [HasPushBar] bit NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230822130835_release6')
BEGIN
    ALTER TABLE [Registry].[Products] ADD [Location] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230822130835_release6')
BEGIN
    ALTER TABLE [Registry].[Products] ADD [NumberOfDoors] int NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230822130835_release6')
BEGIN
    ALTER TABLE [Registry].[Products] ADD [REIType] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230822130835_release6')
BEGIN
    ALTER TABLE [Registry].[Products] ADD [SerialNumber] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230822130835_release6')
BEGIN
    ALTER TABLE [Registry].[Products] ADD [VOCType] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230822130835_release6')
BEGIN
    ALTER TABLE [Registry].[Products] ADD [Year] int NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230822130835_release6')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230822130835_release6', N'7.0.10');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230822131310_release7')
BEGIN
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Registry].[Products]') AND [c].[name] = N'VOCType');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Registry].[Products] DROP CONSTRAINT [' + @var2 + '];');
    ALTER TABLE [Registry].[Products] ALTER COLUMN [VOCType] nvarchar(50) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230822131310_release7')
BEGIN
    DECLARE @var3 sysname;
    SELECT @var3 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Registry].[Products]') AND [c].[name] = N'SerialNumber');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Registry].[Products] DROP CONSTRAINT [' + @var3 + '];');
    ALTER TABLE [Registry].[Products] ALTER COLUMN [SerialNumber] nvarchar(50) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230822131310_release7')
BEGIN
    DECLARE @var4 sysname;
    SELECT @var4 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Registry].[Products]') AND [c].[name] = N'REIType');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [Registry].[Products] DROP CONSTRAINT [' + @var4 + '];');
    ALTER TABLE [Registry].[Products] ALTER COLUMN [REIType] nvarchar(50) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230822131310_release7')
BEGIN
    DECLARE @var5 sysname;
    SELECT @var5 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Registry].[Products]') AND [c].[name] = N'Location');
    IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [Registry].[Products] DROP CONSTRAINT [' + @var5 + '];');
    ALTER TABLE [Registry].[Products] ALTER COLUMN [Location] nvarchar(200) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230822131310_release7')
BEGIN
    DECLARE @var6 sysname;
    SELECT @var6 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Registry].[Products]') AND [c].[name] = N'Constructor');
    IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [Registry].[Products] DROP CONSTRAINT [' + @var6 + '];');
    ALTER TABLE [Registry].[Products] ALTER COLUMN [Constructor] nvarchar(200) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230822131310_release7')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230822131310_release7', N'7.0.10');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230822132138_release8')
BEGIN
    EXEC sp_rename N'[Registry].[Products].[VOCType]', N'VocType', N'COLUMN';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230822132138_release8')
BEGIN
    EXEC sp_rename N'[Registry].[Products].[REIType]', N'ReiType', N'COLUMN';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230822132138_release8')
BEGIN
    EXEC sp_rename N'[Registry].[Products].[Constructor]', N'ConstructorName', N'COLUMN';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230822132138_release8')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230822132138_release8', N'7.0.10');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230824125334_release9')
BEGIN
    ALTER TABLE [Docs].[InterventionProducts] DROP CONSTRAINT [FK_InterventionProducts_Activities_ActivityId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230824125334_release9')
BEGIN
    ALTER TABLE [Docs].[InterventionProducts] DROP CONSTRAINT [FK_InterventionProducts_Products_ProductId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230824125334_release9')
BEGIN
    DROP INDEX [IX_InterventionProducts_ActivityId] ON [Docs].[InterventionProducts];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230824125334_release9')
BEGIN
    DECLARE @var7 sysname;
    SELECT @var7 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Docs].[Jobs]') AND [c].[name] = N'Status');
    IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [Docs].[Jobs] DROP CONSTRAINT [' + @var7 + '];');
    ALTER TABLE [Docs].[Jobs] DROP COLUMN [Status];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230824125334_release9')
BEGIN
    DECLARE @var8 sysname;
    SELECT @var8 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Docs].[InterventionProducts]') AND [c].[name] = N'ActivityId');
    IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [Docs].[InterventionProducts] DROP CONSTRAINT [' + @var8 + '];');
    ALTER TABLE [Docs].[InterventionProducts] DROP COLUMN [ActivityId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230824125334_release9')
BEGIN
    DECLARE @var9 sysname;
    SELECT @var9 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Docs].[Activities]') AND [c].[name] = N'Status');
    IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [Docs].[Activities] DROP CONSTRAINT [' + @var9 + '];');
    ALTER TABLE [Docs].[Activities] DROP COLUMN [Status];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230824125334_release9')
BEGIN
    EXEC sp_rename N'[Docs].[InterventionProducts].[ProductId]', N'ActivityProductId', N'COLUMN';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230824125334_release9')
BEGIN
    EXEC sp_rename N'[Docs].[InterventionProducts].[IX_InterventionProducts_ProductId]', N'IX_InterventionProducts_ActivityProductId', N'INDEX';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230824125334_release9')
BEGIN
    DROP INDEX [IX_Interventions_ActivityId] ON [Docs].[Interventions];
    DECLARE @var10 sysname;
    SELECT @var10 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Docs].[Interventions]') AND [c].[name] = N'ActivityId');
    IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [Docs].[Interventions] DROP CONSTRAINT [' + @var10 + '];');
    EXEC(N'UPDATE [Docs].[Interventions] SET [ActivityId] = CAST(0 AS bigint) WHERE [ActivityId] IS NULL');
    ALTER TABLE [Docs].[Interventions] ALTER COLUMN [ActivityId] bigint NOT NULL;
    ALTER TABLE [Docs].[Interventions] ADD DEFAULT CAST(0 AS bigint) FOR [ActivityId];
    CREATE INDEX [IX_Interventions_ActivityId] ON [Docs].[Interventions] ([ActivityId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230824125334_release9')
BEGIN
    DROP INDEX [IX_InterventionProducts_InterventionId] ON [Docs].[InterventionProducts];
    DECLARE @var11 sysname;
    SELECT @var11 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Docs].[InterventionProducts]') AND [c].[name] = N'InterventionId');
    IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [Docs].[InterventionProducts] DROP CONSTRAINT [' + @var11 + '];');
    EXEC(N'UPDATE [Docs].[InterventionProducts] SET [InterventionId] = CAST(0 AS bigint) WHERE [InterventionId] IS NULL');
    ALTER TABLE [Docs].[InterventionProducts] ALTER COLUMN [InterventionId] bigint NOT NULL;
    ALTER TABLE [Docs].[InterventionProducts] ADD DEFAULT CAST(0 AS bigint) FOR [InterventionId];
    CREATE INDEX [IX_InterventionProducts_InterventionId] ON [Docs].[InterventionProducts] ([InterventionId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230824125334_release9')
BEGIN
    CREATE TABLE [Docs].[ActivityProducts] (
        [Id] bigint NOT NULL IDENTITY,
        [ProductId] bigint NOT NULL,
        [ActivityId] bigint NOT NULL,
        [CreatedOn] datetimeoffset(3) NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [CreatedById] bigint NULL,
        [EditedOn] datetimeoffset(3) NULL,
        [EditedBy] nvarchar(max) NULL,
        [EditedById] bigint NULL,
        [DeletedOn] datetimeoffset(3) NULL,
        [DeletedBy] nvarchar(max) NULL,
        [DeletedById] bigint NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_ActivityProducts] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ActivityProducts_Activities_ActivityId] FOREIGN KEY ([ActivityId]) REFERENCES [Docs].[Activities] ([Id]),
        CONSTRAINT [FK_ActivityProducts_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Registry].[Products] ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230824125334_release9')
BEGIN
    CREATE INDEX [IX_ActivityProducts_ActivityId] ON [Docs].[ActivityProducts] ([ActivityId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230824125334_release9')
BEGIN
    CREATE INDEX [IX_ActivityProducts_ProductId] ON [Docs].[ActivityProducts] ([ProductId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230824125334_release9')
BEGIN
    ALTER TABLE [Docs].[InterventionProducts] ADD CONSTRAINT [FK_InterventionProducts_ActivityProducts_ActivityProductId] FOREIGN KEY ([ActivityProductId]) REFERENCES [Docs].[ActivityProducts] ([Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230824125334_release9')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230824125334_release9', N'7.0.10');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230829085323_release10')
BEGIN
    DECLARE @var12 sysname;
    SELECT @var12 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Registry].[Products]') AND [c].[name] = N'QrCode');
    IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [Registry].[Products] DROP CONSTRAINT [' + @var12 + '];');
    ALTER TABLE [Registry].[Products] DROP COLUMN [QrCode];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230829085323_release10')
BEGIN
    ALTER TABLE [Registry].[Products] ADD [QrCodeNumber] int NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230829085323_release10')
BEGIN
    ALTER TABLE [Registry].[Products] ADD [QrCodePrefix] nvarchar(10) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230829085323_release10')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230829085323_release10', N'7.0.10');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231006083312_release11')
BEGIN
    ALTER TABLE [Docs].[Tickets] ADD [Number] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231006083312_release11')
BEGIN
    ALTER TABLE [Docs].[Tickets] ADD [TicketDate] datetimeoffset(3) NOT NULL DEFAULT '0001-01-01T00:00:00.000+00:00';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231006083312_release11')
BEGIN
    ALTER TABLE [Docs].[Tickets] ADD [Year] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231006083312_release11')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20231006083312_release11', N'7.0.10');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231016113304_release12')
BEGIN
    ALTER TABLE [InterventionProductCheckListItem] DROP CONSTRAINT [FK_InterventionProductCheckListItem_InterventionProductCheckLists_CheckListId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231016113304_release12')
BEGIN
    ALTER TABLE [InterventionProductCheckListItem] DROP CONSTRAINT [FK_InterventionProductCheckListItem_Operators_OperatorId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231016113304_release12')
BEGIN
    ALTER TABLE [InterventionProductCheckListItem] DROP CONSTRAINT [PK_InterventionProductCheckListItem];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231016113304_release12')
BEGIN
    EXEC sp_rename N'[InterventionProductCheckListItem]', N'InterventionProductCheckListItems';
    ALTER SCHEMA [Docs] TRANSFER [InterventionProductCheckListItems];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231016113304_release12')
BEGIN
    EXEC sp_rename N'[Docs].[InterventionProductCheckListItems].[IX_InterventionProductCheckListItem_OperatorId]', N'IX_InterventionProductCheckListItems_OperatorId', N'INDEX';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231016113304_release12')
BEGIN
    EXEC sp_rename N'[Docs].[InterventionProductCheckListItems].[IX_InterventionProductCheckListItem_CheckListId]', N'IX_InterventionProductCheckListItems_CheckListId', N'INDEX';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231016113304_release12')
BEGIN
    ALTER TABLE [Registry].[ActivityTypes] ADD [ColorHex] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231016113304_release12')
BEGIN
    ALTER TABLE [Registry].[ActivityTypes] ADD [IsInternal] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231016113304_release12')
BEGIN
    DECLARE @var13 sysname;
    SELECT @var13 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Docs].[InterventionProductCheckListItems]') AND [c].[name] = N'EditedOn');
    IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [Docs].[InterventionProductCheckListItems] DROP CONSTRAINT [' + @var13 + '];');
    ALTER TABLE [Docs].[InterventionProductCheckListItems] ALTER COLUMN [EditedOn] datetimeoffset(3) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231016113304_release12')
BEGIN
    DECLARE @var14 sysname;
    SELECT @var14 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Docs].[InterventionProductCheckListItems]') AND [c].[name] = N'Description');
    IF @var14 IS NOT NULL EXEC(N'ALTER TABLE [Docs].[InterventionProductCheckListItems] DROP CONSTRAINT [' + @var14 + '];');
    EXEC(N'UPDATE [Docs].[InterventionProductCheckListItems] SET [Description] = N'''' WHERE [Description] IS NULL');
    ALTER TABLE [Docs].[InterventionProductCheckListItems] ALTER COLUMN [Description] nvarchar(500) NOT NULL;
    ALTER TABLE [Docs].[InterventionProductCheckListItems] ADD DEFAULT N'' FOR [Description];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231016113304_release12')
BEGIN
    DECLARE @var15 sysname;
    SELECT @var15 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Docs].[InterventionProductCheckListItems]') AND [c].[name] = N'DeletedOn');
    IF @var15 IS NOT NULL EXEC(N'ALTER TABLE [Docs].[InterventionProductCheckListItems] DROP CONSTRAINT [' + @var15 + '];');
    ALTER TABLE [Docs].[InterventionProductCheckListItems] ALTER COLUMN [DeletedOn] datetimeoffset(3) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231016113304_release12')
BEGIN
    DECLARE @var16 sysname;
    SELECT @var16 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Docs].[InterventionProductCheckListItems]') AND [c].[name] = N'CreatedOn');
    IF @var16 IS NOT NULL EXEC(N'ALTER TABLE [Docs].[InterventionProductCheckListItems] DROP CONSTRAINT [' + @var16 + '];');
    ALTER TABLE [Docs].[InterventionProductCheckListItems] ALTER COLUMN [CreatedOn] datetimeoffset(3) NOT NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231016113304_release12')
BEGIN
    ALTER TABLE [Docs].[InterventionProductCheckListItems] ADD CONSTRAINT [PK_InterventionProductCheckListItems] PRIMARY KEY ([Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231016113304_release12')
BEGIN
    CREATE TABLE [Registry].[OperatorsActivityTypes] (
        [ActivityTypesId] bigint NOT NULL,
        [OperatorsId] bigint NOT NULL,
        CONSTRAINT [PK_OperatorsActivityTypes] PRIMARY KEY ([ActivityTypesId], [OperatorsId]),
        CONSTRAINT [FK_OperatorsActivityTypes_ActivityTypes_ActivityTypesId] FOREIGN KEY ([ActivityTypesId]) REFERENCES [Registry].[ActivityTypes] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_OperatorsActivityTypes_Operators_OperatorsId] FOREIGN KEY ([OperatorsId]) REFERENCES [Registry].[Operators] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231016113304_release12')
BEGIN
    CREATE INDEX [IX_OperatorsActivityTypes_OperatorsId] ON [Registry].[OperatorsActivityTypes] ([OperatorsId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231016113304_release12')
BEGIN
    ALTER TABLE [Docs].[InterventionProductCheckListItems] ADD CONSTRAINT [FK_InterventionProductCheckListItems_InterventionProductCheckLists_CheckListId] FOREIGN KEY ([CheckListId]) REFERENCES [Docs].[InterventionProductCheckLists] ([Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231016113304_release12')
BEGIN
    ALTER TABLE [Docs].[InterventionProductCheckListItems] ADD CONSTRAINT [FK_InterventionProductCheckListItems_Operators_OperatorId] FOREIGN KEY ([OperatorId]) REFERENCES [Registry].[Operators] ([Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231016113304_release12')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20231016113304_release12', N'7.0.10');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231017151834_Add_Status_To_Activity')
BEGIN
    ALTER TABLE [Docs].[Activities] ADD [Status] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231017151834_Add_Status_To_Activity')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20231017151834_Add_Status_To_Activity', N'7.0.10');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231018075252_Add_ExpirationDate_To_Activity')
BEGIN
    ALTER TABLE [Docs].[Activities] ADD [ExpirationDate] datetimeoffset(3) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231018075252_Add_ExpirationDate_To_Activity')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20231018075252_Add_ExpirationDate_To_Activity', N'7.0.10');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231026084953_Modify_Product_QrCodeNumber_int_to_string')
BEGIN
    DECLARE @var17 sysname;
    SELECT @var17 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Registry].[Products]') AND [c].[name] = N'QrCodeNumber');
    IF @var17 IS NOT NULL EXEC(N'ALTER TABLE [Registry].[Products] DROP CONSTRAINT [' + @var17 + '];');
    ALTER TABLE [Registry].[Products] ALTER COLUMN [QrCodeNumber] nvarchar(10) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231026084953_Modify_Product_QrCodeNumber_int_to_string')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20231026084953_Modify_Product_QrCodeNumber_int_to_string', N'7.0.10');
END;
GO

COMMIT;
GO

