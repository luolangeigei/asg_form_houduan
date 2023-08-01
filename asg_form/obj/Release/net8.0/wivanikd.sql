BEGIN TRANSACTION;
GO

ALTER TABLE [AspNetUsers] ADD [UserBase64] nvarchar(max) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230801094752_userbase', N'7.0.5');
GO

COMMIT;
GO

