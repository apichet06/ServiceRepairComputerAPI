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

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230815040136_AddnewDatabase')
BEGIN
    CREATE TABLE [Categories] (
        [Id] int NOT NULL IDENTITY,
        [C_ID] nvarchar(10) NULL,
        [Name] nvarchar(300) NULL,
        CONSTRAINT [PK_Categories] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230815040136_AddnewDatabase')
BEGIN
    CREATE TABLE [Comments] (
        [Id] int NOT NULL IDENTITY,
        [CM_ID] nvarchar(100) NULL,
        [Content] nvarchar(100) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [TechnicianId] nvarchar(10) NULL,
        [I_ID] nvarchar(10) NULL,
        CONSTRAINT [PK_Comments] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230815040136_AddnewDatabase')
BEGIN
    CREATE TABLE [Computers] (
        [Id] int NOT NULL IDENTITY,
        [ComputerId] nvarchar(20) NULL,
        [Name] nvarchar(250) NULL,
        [SerialNumber] nvarchar(100) NULL,
        [Description] nvarchar(500) NULL,
        CONSTRAINT [PK_Computers] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230815040136_AddnewDatabase')
BEGIN
    CREATE TABLE [Divisions] (
        [Id] int NOT NULL IDENTITY,
        [DV_ID] nvarchar(10) NULL,
        [DV_Name] nvarchar(150) NULL,
        [P_ID] nvarchar(10) NULL,
        CONSTRAINT [PK_Divisions] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230815040136_AddnewDatabase')
BEGIN
    CREATE TABLE [Employees] (
        [Id] int NOT NULL IDENTITY,
        [EmployeeId] nvarchar(10) NULL,
        [Title] nvarchar(10) NULL,
        [FirstName] nvarchar(100) NULL,
        [LastName] nvarchar(100) NULL,
        [Username] nvarchar(100) NULL,
        [Email] nvarchar(100) NULL,
        [Password] nvarchar(50) NULL,
        [DV_ID] nvarchar(10) NULL,
        [P_ID] nvarchar(10) NULL,
        [Status] int NOT NULL,
        CONSTRAINT [PK_Employees] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230815040136_AddnewDatabase')
BEGIN
    CREATE TABLE [Issues] (
        [Id] int NOT NULL IDENTITY,
        [I_ID] nvarchar(10) NULL,
        [Title] nvarchar(200) NULL,
        [Description] nvarchar(300) NULL,
        [Status] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [ResolvedAt] datetime2 NULL,
        [Path_Images] nvarchar(150) NULL,
        [ComputerId] nvarchar(10) NULL,
        [CategoryId] nvarchar(10) NULL,
        [EmployeeId] nvarchar(10) NULL,
        [TechnicianId] nvarchar(10) NULL,
        CONSTRAINT [PK_Issues] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230815040136_AddnewDatabase')
BEGIN
    CREATE TABLE [Positions] (
        [Id] int NOT NULL IDENTITY,
        [P_ID] nvarchar(10) NULL,
        [P_Name] nvarchar(150) NULL,
        CONSTRAINT [PK_Positions] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230815040136_AddnewDatabase')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230815040136_AddnewDatabase', N'7.0.9');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230815043249_AddnewDatabasenew')
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Employees]') AND [c].[name] = N'Status');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Employees] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [Employees] ALTER COLUMN [Status] nvarchar(50) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230815043249_AddnewDatabasenew')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230815043249_AddnewDatabasenew', N'7.0.9');
END;
GO

COMMIT;
GO

