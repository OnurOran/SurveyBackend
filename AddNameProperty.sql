BEGIN TRANSACTION;
ALTER TABLE [Parameters] ADD [Name] nvarchar(100) NOT NULL DEFAULT N'';

UPDATE [Parameters] SET [Name] = N'Parameters'
WHERE [Id] = 1;
SELECT @@ROWCOUNT;


UPDATE [Parameters] SET [Name] = N'SurveyAccessTypes'
WHERE [Id] = 2;
SELECT @@ROWCOUNT;


UPDATE [Parameters] SET [Name] = N'Internal'
WHERE [Id] = 3;
SELECT @@ROWCOUNT;


UPDATE [Parameters] SET [Name] = N'Public'
WHERE [Id] = 4;
SELECT @@ROWCOUNT;


UPDATE [Parameters] SET [Name] = N'QuestionTypes'
WHERE [Id] = 5;
SELECT @@ROWCOUNT;


UPDATE [Parameters] SET [Name] = N'SingleSelect'
WHERE [Id] = 6;
SELECT @@ROWCOUNT;


UPDATE [Parameters] SET [Name] = N'MultiSelect'
WHERE [Id] = 7;
SELECT @@ROWCOUNT;


UPDATE [Parameters] SET [Name] = N'OpenText'
WHERE [Id] = 8;
SELECT @@ROWCOUNT;


UPDATE [Parameters] SET [Name] = N'FileUpload'
WHERE [Id] = 9;
SELECT @@ROWCOUNT;


UPDATE [Parameters] SET [Name] = N'Conditional'
WHERE [Id] = 10;
SELECT @@ROWCOUNT;


INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251217133429_AddNamePropertyToParameter', N'9.0.0');

COMMIT;
GO

