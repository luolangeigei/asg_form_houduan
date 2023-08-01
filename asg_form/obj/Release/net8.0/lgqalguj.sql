BEGIN TRANSACTION;
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[F_game]') AND [c].[name] = N'closetime');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [F_game] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [F_game] DROP COLUMN [closetime];
GO

ALTER TABLE [F_news] ADD [time] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

ALTER TABLE [F_game] ADD [bilibiliuri] nvarchar(max) NULL;
GO

ALTER TABLE [F_game] ADD [commentary] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [F_game] ADD [referee] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [F_game] ADD [winteam] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [AspNetUsers] ADD [chinaname] nvarchar(max) NULL;
GO

ALTER TABLE [AspNetUsers] ADD [officium] nvarchar(max) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230731163702_newstime', N'7.0.5');
GO

COMMIT;
GO

