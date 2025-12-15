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
CREATE TABLE [Departments] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(256) NOT NULL,
    [ExternalIdentifier] nvarchar(256) NOT NULL,
    CONSTRAINT [PK_Departments] PRIMARY KEY ([Id])
);

CREATE TABLE [Participants] (
    [Id] uniqueidentifier NOT NULL,
    [ExternalId] uniqueidentifier NULL,
    [LdapUsername] nvarchar(100) NULL,
    [CreatedAt] datetimeoffset NOT NULL,
    CONSTRAINT [PK_Participants] PRIMARY KEY ([Id])
);

CREATE TABLE [Permissions] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(200) NOT NULL,
    [Description] nvarchar(500) NOT NULL,
    CONSTRAINT [PK_Permissions] PRIMARY KEY ([Id])
);

CREATE TABLE [Roles] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(200) NOT NULL,
    [Description] nvarchar(500) NOT NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
);

CREATE TABLE [Surveys] (
    [Id] uniqueidentifier NOT NULL,
    [Title] nvarchar(200) NOT NULL,
    [Description] nvarchar(2000) NULL,
    [CreatedBy] nvarchar(100) NOT NULL,
    [DepartmentId] uniqueidentifier NOT NULL,
    [CreatedAt] datetimeoffset NOT NULL,
    [IsActive] bit NOT NULL DEFAULT CAST(0 AS bit),
    [AccessType] int NOT NULL,
    [StartDate] datetimeoffset NULL,
    [EndDate] datetimeoffset NULL,
    CONSTRAINT [PK_Surveys] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Surveys_Departments_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Departments] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Users] (
    [Id] uniqueidentifier NOT NULL,
    [Username] nvarchar(256) NOT NULL,
    [Email] nvarchar(256) NOT NULL,
    [DepartmentId] uniqueidentifier NOT NULL,
    [PasswordHash] nvarchar(512) NULL,
    [IsSuperAdmin] bit NOT NULL,
    [CreatedAt] datetimeoffset NOT NULL,
    [UpdatedAt] datetimeoffset NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Users_Departments_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Departments] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [RolePermissions] (
    [RoleId] uniqueidentifier NOT NULL,
    [PermissionId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_RolePermissions] PRIMARY KEY ([RoleId], [PermissionId]),
    CONSTRAINT [FK_RolePermissions_Permissions_PermissionId] FOREIGN KEY ([PermissionId]) REFERENCES [Permissions] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_RolePermissions_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Participations] (
    [Id] uniqueidentifier NOT NULL,
    [SurveyId] uniqueidentifier NOT NULL,
    [ParticipantId] uniqueidentifier NOT NULL,
    [StartedAt] datetimeoffset NOT NULL,
    [CompletedAt] datetimeoffset NULL,
    [IpAddress] nvarchar(50) NULL,
    CONSTRAINT [PK_Participations] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Participations_Participants_ParticipantId] FOREIGN KEY ([ParticipantId]) REFERENCES [Participants] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Participations_Surveys_SurveyId] FOREIGN KEY ([SurveyId]) REFERENCES [Surveys] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Questions] (
    [Id] uniqueidentifier NOT NULL,
    [SurveyId] uniqueidentifier NOT NULL,
    [Text] nvarchar(1000) NOT NULL,
    [Description] nvarchar(500) NULL,
    [Order] int NOT NULL,
    [Type] int NOT NULL,
    [IsRequired] bit NOT NULL,
    [AllowedAttachmentContentTypes] nvarchar(512) NULL,
    CONSTRAINT [PK_Questions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Questions_Surveys_SurveyId] FOREIGN KEY ([SurveyId]) REFERENCES [Surveys] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [UserRefreshTokens] (
    [Id] uniqueidentifier NOT NULL,
    [UserId] uniqueidentifier NOT NULL,
    [Token] nvarchar(512) NOT NULL,
    [ExpiresAt] datetimeoffset NOT NULL,
    [CreatedAt] datetimeoffset NOT NULL,
    [RevokedAt] datetimeoffset NULL,
    CONSTRAINT [PK_UserRefreshTokens] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserRefreshTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [UserRoles] (
    [UserId] uniqueidentifier NOT NULL,
    [RoleId] uniqueidentifier NOT NULL,
    [DepartmentId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_UserRoles] PRIMARY KEY ([UserId], [RoleId], [DepartmentId]),
    CONSTRAINT [FK_UserRoles_Departments_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Departments] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_UserRoles_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserRoles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Answers] (
    [Id] uniqueidentifier NOT NULL,
    [ParticipationId] uniqueidentifier NOT NULL,
    [QuestionId] uniqueidentifier NOT NULL,
    [TextValue] nvarchar(max) NULL,
    CONSTRAINT [PK_Answers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Answers_Participations_ParticipationId] FOREIGN KEY ([ParticipationId]) REFERENCES [Participations] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Answers_Questions_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [Questions] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [QuestionOptions] (
    [Id] uniqueidentifier NOT NULL,
    [QuestionId] uniqueidentifier NOT NULL,
    [Text] nvarchar(500) NOT NULL,
    [Order] int NOT NULL,
    [Value] int NULL,
    CONSTRAINT [PK_QuestionOptions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_QuestionOptions_Questions_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [Questions] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AnswerAttachments] (
    [Id] uniqueidentifier NOT NULL,
    [AnswerId] uniqueidentifier NOT NULL,
    [SurveyId] uniqueidentifier NOT NULL,
    [DepartmentId] uniqueidentifier NOT NULL,
    [FileName] nvarchar(512) NOT NULL,
    [ContentType] nvarchar(256) NOT NULL,
    [SizeBytes] bigint NOT NULL,
    [StoragePath] nvarchar(1024) NOT NULL,
    [CreatedAt] datetimeoffset NOT NULL,
    CONSTRAINT [PK_AnswerAttachments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AnswerAttachments_Answers_AnswerId] FOREIGN KEY ([AnswerId]) REFERENCES [Answers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AnswerOptions] (
    [Id] uniqueidentifier NOT NULL,
    [AnswerId] uniqueidentifier NOT NULL,
    [QuestionOptionId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_AnswerOptions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AnswerOptions_Answers_AnswerId] FOREIGN KEY ([AnswerId]) REFERENCES [Answers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AnswerOptions_QuestionOptions_QuestionOptionId] FOREIGN KEY ([QuestionOptionId]) REFERENCES [QuestionOptions] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Attachments] (
    [Id] uniqueidentifier NOT NULL,
    [OwnerType] int NOT NULL,
    [DepartmentId] uniqueidentifier NOT NULL,
    [SurveyId] uniqueidentifier NULL,
    [QuestionId] uniqueidentifier NULL,
    [QuestionOptionId] uniqueidentifier NULL,
    [FileName] nvarchar(512) NOT NULL,
    [ContentType] nvarchar(256) NOT NULL,
    [SizeBytes] bigint NOT NULL,
    [StoragePath] nvarchar(1024) NOT NULL,
    [CreatedAt] datetimeoffset NOT NULL,
    CONSTRAINT [PK_Attachments] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_Attachments_SingleOwner] CHECK (
                (
                    (CASE WHEN SurveyId IS NOT NULL THEN 1 ELSE 0 END) +
                    (CASE WHEN QuestionId IS NOT NULL THEN 1 ELSE 0 END) +
                    (CASE WHEN QuestionOptionId IS NOT NULL THEN 1 ELSE 0 END)
                ) = 1),
    CONSTRAINT [FK_Attachments_QuestionOptions_QuestionOptionId] FOREIGN KEY ([QuestionOptionId]) REFERENCES [QuestionOptions] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Attachments_Questions_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [Questions] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Attachments_Surveys_SurveyId] FOREIGN KEY ([SurveyId]) REFERENCES [Surveys] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [DependentQuestions] (
    [Id] uniqueidentifier NOT NULL,
    [ParentQuestionOptionId] uniqueidentifier NOT NULL,
    [ChildQuestionId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_DependentQuestions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DependentQuestions_QuestionOptions_ParentQuestionOptionId] FOREIGN KEY ([ParentQuestionOptionId]) REFERENCES [QuestionOptions] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_DependentQuestions_Questions_ChildQuestionId] FOREIGN KEY ([ChildQuestionId]) REFERENCES [Questions] ([Id]) ON DELETE NO ACTION
);

CREATE UNIQUE INDEX [IX_AnswerAttachments_AnswerId] ON [AnswerAttachments] ([AnswerId]);

CREATE INDEX [IX_AnswerOptions_AnswerId] ON [AnswerOptions] ([AnswerId]);

CREATE INDEX [IX_AnswerOptions_QuestionOptionId] ON [AnswerOptions] ([QuestionOptionId]);

CREATE INDEX [IX_Answers_ParticipationId] ON [Answers] ([ParticipationId]);

CREATE INDEX [IX_Answers_QuestionId] ON [Answers] ([QuestionId]);

CREATE UNIQUE INDEX [IX_Attachments_QuestionId] ON [Attachments] ([QuestionId]) WHERE [QuestionId] IS NOT NULL;

CREATE UNIQUE INDEX [IX_Attachments_QuestionOptionId] ON [Attachments] ([QuestionOptionId]) WHERE [QuestionOptionId] IS NOT NULL;

CREATE UNIQUE INDEX [IX_Attachments_SurveyId] ON [Attachments] ([SurveyId]) WHERE [SurveyId] IS NOT NULL;

CREATE UNIQUE INDEX [IX_Departments_ExternalIdentifier] ON [Departments] ([ExternalIdentifier]);

CREATE INDEX [IX_DependentQuestions_ChildQuestionId] ON [DependentQuestions] ([ChildQuestionId]);

CREATE INDEX [IX_DependentQuestions_ParentQuestionOptionId] ON [DependentQuestions] ([ParentQuestionOptionId]);

CREATE UNIQUE INDEX [IX_Participants_ExternalId] ON [Participants] ([ExternalId]) WHERE [ExternalId] IS NOT NULL;

CREATE UNIQUE INDEX [IX_Participants_LdapUsername] ON [Participants] ([LdapUsername]) WHERE [LdapUsername] IS NOT NULL;

CREATE INDEX [IX_Participations_ParticipantId] ON [Participations] ([ParticipantId]);

CREATE INDEX [IX_Participations_SurveyId] ON [Participations] ([SurveyId]);

CREATE INDEX [IX_QuestionOptions_QuestionId] ON [QuestionOptions] ([QuestionId]);

CREATE INDEX [IX_Questions_SurveyId] ON [Questions] ([SurveyId]);

CREATE INDEX [IX_RolePermissions_PermissionId] ON [RolePermissions] ([PermissionId]);

CREATE INDEX [IX_Surveys_DepartmentId] ON [Surveys] ([DepartmentId]);

CREATE UNIQUE INDEX [IX_UserRefreshTokens_Token] ON [UserRefreshTokens] ([Token]);

CREATE INDEX [IX_UserRefreshTokens_UserId] ON [UserRefreshTokens] ([UserId]);

CREATE INDEX [IX_UserRoles_DepartmentId] ON [UserRoles] ([DepartmentId]);

CREATE INDEX [IX_UserRoles_RoleId] ON [UserRoles] ([RoleId]);

CREATE INDEX [IX_Users_DepartmentId] ON [Users] ([DepartmentId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251128112305_AddSurveyDepartment', N'9.0.0');

ALTER TABLE [Surveys] ADD [ConsentText] nvarchar(max) NULL;

ALTER TABLE [Surveys] ADD [IntroText] nvarchar(max) NULL;

ALTER TABLE [Surveys] ADD [OutroText] nvarchar(max) NULL;

ALTER TABLE [Surveys] ADD [RequireConsent] bit NOT NULL DEFAULT CAST(0 AS bit);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251201111716_AddSurveyIntroConsentOutroFields', N'9.0.0');

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Surveys]') AND [c].[name] = N'RequireConsent');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Surveys] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [Surveys] DROP COLUMN [RequireConsent];

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251201112134_RemoveRequireConsentField', N'9.0.0');

COMMIT;
GO

