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
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE TABLE [Department] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(100) NOT NULL,
        [ExternalIdentifier] nvarchar(100) NOT NULL,
        [CreateEmployeeId] int NULL,
        [CreateDate] datetime2 NOT NULL,
        [UpdateEmployeeId] int NULL,
        [UpdateDate] datetime2 NULL,
        [IsDelete] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_Department] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE TABLE [Parameter] (
        [Id] int NOT NULL,
        [Code] nvarchar(20) NULL,
        [GroupName] nvarchar(100) NOT NULL,
        [DisplayName] nvarchar(200) NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [Description] nvarchar(500) NULL,
        [ParentId] int NOT NULL,
        [LevelNo] int NOT NULL,
        [Symbol] nvarchar(10) NULL,
        [OrderNo] int NOT NULL,
        [CreateEmployeeId] int NULL,
        [CreateDate] datetime2 NOT NULL,
        [UpdateEmployeeId] int NULL,
        [UpdateDate] datetime2 NULL,
        [IsDelete] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_Parameter] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE TABLE [Participant] (
        [Id] int NOT NULL IDENTITY,
        [ExternalId] uniqueidentifier NULL,
        [LdapUsername] nvarchar(100) NULL,
        [CreateEmployeeId] int NULL,
        [CreateDate] datetime2 NOT NULL,
        [UpdateEmployeeId] int NULL,
        [UpdateDate] datetime2 NULL,
        [IsDelete] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_Participant] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE TABLE [Permission] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(50) NOT NULL,
        [Description] nvarchar(200) NOT NULL,
        [CreateEmployeeId] int NULL,
        [CreateDate] datetime2 NOT NULL,
        [UpdateEmployeeId] int NULL,
        [UpdateDate] datetime2 NULL,
        [IsDelete] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_Permission] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE TABLE [Role] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(50) NOT NULL,
        [Description] nvarchar(200) NOT NULL,
        [CreateEmployeeId] int NULL,
        [CreateDate] datetime2 NOT NULL,
        [UpdateEmployeeId] int NULL,
        [UpdateDate] datetime2 NULL,
        [IsDelete] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_Role] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE TABLE [User] (
        [Id] int NOT NULL IDENTITY,
        [Username] nvarchar(50) NOT NULL,
        [Email] nvarchar(254) NOT NULL,
        [DepartmentId] int NOT NULL,
        [PasswordHash] nvarchar(512) NULL,
        [IsSuperAdmin] bit NOT NULL,
        [CreateEmployeeId] int NULL,
        [CreateDate] datetime2 NOT NULL,
        [UpdateEmployeeId] int NULL,
        [UpdateDate] datetime2 NULL,
        [IsDelete] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_User] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_User_Department_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Department] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE TABLE [Survey] (
        [Id] int NOT NULL IDENTITY,
        [Slug] nvarchar(100) NOT NULL,
        [Title] nvarchar(200) NOT NULL,
        [Description] nvarchar(2000) NULL,
        [IntroText] nvarchar(256) NULL,
        [ConsentText] nvarchar(1000) NULL,
        [OutroText] nvarchar(150) NULL,
        [DepartmentId] int NOT NULL,
        [IsPublished] bit NOT NULL DEFAULT CAST(0 AS bit),
        [AccessType] int NOT NULL,
        [StartDate] datetime2 NULL,
        [EndDate] datetime2 NULL,
        [TypeId] int NULL,
        [CreateEmployeeId] int NULL,
        [CreateDate] datetime2 NOT NULL,
        [UpdateEmployeeId] int NULL,
        [UpdateDate] datetime2 NULL,
        [IsDelete] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_Survey] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Survey_Department_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Department] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Survey_Parameter_TypeId] FOREIGN KEY ([TypeId]) REFERENCES [Parameter] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE TABLE [RolePermission] (
        [RoleId] int NOT NULL,
        [PermissionId] int NOT NULL,
        [CreateEmployeeId] int NULL,
        [CreateDate] datetime2 NOT NULL,
        [UpdateEmployeeId] int NULL,
        [UpdateDate] datetime2 NULL,
        [IsDelete] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_RolePermission] PRIMARY KEY ([RoleId], [PermissionId]),
        CONSTRAINT [FK_RolePermission_Permission_PermissionId] FOREIGN KEY ([PermissionId]) REFERENCES [Permission] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_RolePermission_Role_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Role] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE TABLE [UserRefreshToken] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [Token] nvarchar(512) NOT NULL,
        [ExpiresAt] datetime2 NOT NULL,
        [RevokedAt] datetime2 NULL,
        [CreateEmployeeId] int NULL,
        [CreateDate] datetime2 NOT NULL,
        [UpdateEmployeeId] int NULL,
        [UpdateDate] datetime2 NULL,
        [IsDelete] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_UserRefreshToken] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserRefreshToken_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [User] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE TABLE [UserRole] (
        [UserId] int NOT NULL,
        [RoleId] int NOT NULL,
        [DepartmentId] int NOT NULL,
        [CreateEmployeeId] int NULL,
        [CreateDate] datetime2 NOT NULL,
        [UpdateEmployeeId] int NULL,
        [UpdateDate] datetime2 NULL,
        [IsDelete] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_UserRole] PRIMARY KEY ([UserId], [RoleId], [DepartmentId]),
        CONSTRAINT [FK_UserRole_Department_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Department] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_UserRole_Role_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Role] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_UserRole_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [User] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE TABLE [Participation] (
        [Id] int NOT NULL IDENTITY,
        [SurveyId] int NOT NULL,
        [ParticipantId] int NOT NULL,
        [StartedAt] datetime2 NOT NULL,
        [CompletedAt] datetime2 NULL,
        [IpAddress] nvarchar(50) NULL,
        [CreateEmployeeId] int NULL,
        [CreateDate] datetime2 NOT NULL,
        [UpdateEmployeeId] int NULL,
        [UpdateDate] datetime2 NULL,
        [IsDelete] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_Participation] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Participation_Participant_ParticipantId] FOREIGN KEY ([ParticipantId]) REFERENCES [Participant] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Participation_Survey_SurveyId] FOREIGN KEY ([SurveyId]) REFERENCES [Survey] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE TABLE [Question] (
        [Id] int NOT NULL IDENTITY,
        [SurveyId] int NOT NULL,
        [Text] nvarchar(1000) NOT NULL,
        [Description] nvarchar(500) NULL,
        [Order] int NOT NULL,
        [Type] int NOT NULL,
        [IsRequired] bit NOT NULL,
        [AllowedAttachmentContentTypes] nvarchar(512) NULL,
        [TypeId] int NULL,
        [CreateEmployeeId] int NULL,
        [CreateDate] datetime2 NOT NULL,
        [UpdateEmployeeId] int NULL,
        [UpdateDate] datetime2 NULL,
        [IsDelete] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_Question] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Question_Parameter_TypeId] FOREIGN KEY ([TypeId]) REFERENCES [Parameter] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Question_Survey_SurveyId] FOREIGN KEY ([SurveyId]) REFERENCES [Survey] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE TABLE [Answer] (
        [Id] int NOT NULL IDENTITY,
        [ParticipationId] int NOT NULL,
        [QuestionId] int NOT NULL,
        [TextValue] nvarchar(2000) NULL,
        [CreateEmployeeId] int NULL,
        [CreateDate] datetime2 NOT NULL,
        [UpdateEmployeeId] int NULL,
        [UpdateDate] datetime2 NULL,
        [IsDelete] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_Answer] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Answer_Participation_ParticipationId] FOREIGN KEY ([ParticipationId]) REFERENCES [Participation] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Answer_Question_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [Question] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE TABLE [QuestionOption] (
        [Id] int NOT NULL IDENTITY,
        [QuestionId] int NOT NULL,
        [Text] nvarchar(256) NOT NULL,
        [Order] int NOT NULL,
        [Value] int NULL,
        [CreateEmployeeId] int NULL,
        [CreateDate] datetime2 NOT NULL,
        [UpdateEmployeeId] int NULL,
        [UpdateDate] datetime2 NULL,
        [IsDelete] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_QuestionOption] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_QuestionOption_Question_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [Question] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE TABLE [AnswerAttachment] (
        [Id] int NOT NULL IDENTITY,
        [AnswerId] int NOT NULL,
        [SurveyId] int NOT NULL,
        [DepartmentId] int NOT NULL,
        [FileName] nvarchar(128) NOT NULL,
        [ContentType] nvarchar(256) NOT NULL,
        [SizeBytes] bigint NOT NULL,
        [StoragePath] nvarchar(1024) NOT NULL,
        [CreateEmployeeId] int NULL,
        [CreateDate] datetime2 NOT NULL,
        [UpdateEmployeeId] int NULL,
        [UpdateDate] datetime2 NULL,
        [IsDelete] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_AnswerAttachment] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AnswerAttachment_Answer_AnswerId] FOREIGN KEY ([AnswerId]) REFERENCES [Answer] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE TABLE [AnswerOption] (
        [Id] int NOT NULL IDENTITY,
        [AnswerId] int NOT NULL,
        [QuestionOptionId] int NOT NULL,
        [CreateEmployeeId] int NULL,
        [CreateDate] datetime2 NOT NULL,
        [UpdateEmployeeId] int NULL,
        [UpdateDate] datetime2 NULL,
        [IsDelete] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_AnswerOption] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AnswerOption_Answer_AnswerId] FOREIGN KEY ([AnswerId]) REFERENCES [Answer] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AnswerOption_QuestionOption_QuestionOptionId] FOREIGN KEY ([QuestionOptionId]) REFERENCES [QuestionOption] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE TABLE [Attachment] (
        [Id] int NOT NULL IDENTITY,
        [OwnerType] int NOT NULL,
        [DepartmentId] int NOT NULL,
        [SurveyId] int NULL,
        [QuestionId] int NULL,
        [QuestionOptionId] int NULL,
        [FileName] nvarchar(128) NOT NULL,
        [ContentType] nvarchar(256) NOT NULL,
        [SizeBytes] bigint NOT NULL,
        [StoragePath] nvarchar(1024) NOT NULL,
        [CreateEmployeeId] int NULL,
        [CreateDate] datetime2 NOT NULL,
        [UpdateEmployeeId] int NULL,
        [UpdateDate] datetime2 NULL,
        [IsDelete] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_Attachment] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_Attachment_SingleOwner] CHECK (
                    (
                        (CASE WHEN SurveyId IS NOT NULL THEN 1 ELSE 0 END) +
                        (CASE WHEN QuestionId IS NOT NULL THEN 1 ELSE 0 END) +
                        (CASE WHEN QuestionOptionId IS NOT NULL THEN 1 ELSE 0 END)
                    ) = 1),
        CONSTRAINT [FK_Attachment_QuestionOption_QuestionOptionId] FOREIGN KEY ([QuestionOptionId]) REFERENCES [QuestionOption] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Attachment_Question_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [Question] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Attachment_Survey_SurveyId] FOREIGN KEY ([SurveyId]) REFERENCES [Survey] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE TABLE [DependentQuestion] (
        [Id] int NOT NULL IDENTITY,
        [ParentQuestionOptionId] int NOT NULL,
        [ChildQuestionId] int NOT NULL,
        [CreateEmployeeId] int NULL,
        [CreateDate] datetime2 NOT NULL,
        [UpdateEmployeeId] int NULL,
        [UpdateDate] datetime2 NULL,
        [IsDelete] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_DependentQuestion] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DependentQuestion_QuestionOption_ParentQuestionOptionId] FOREIGN KEY ([ParentQuestionOptionId]) REFERENCES [QuestionOption] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_DependentQuestion_Question_ChildQuestionId] FOREIGN KEY ([ChildQuestionId]) REFERENCES [Question] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Code', N'CreateDate', N'CreateEmployeeId', N'Description', N'DisplayName', N'GroupName', N'IsActive', N'IsDelete', N'LevelNo', N'Name', N'OrderNo', N'ParentId', N'Symbol', N'UpdateDate', N'UpdateEmployeeId') AND [object_id] = OBJECT_ID(N'[Parameter]'))
        SET IDENTITY_INSERT [Parameter] ON;
    EXEC(N'INSERT INTO [Parameter] ([Id], [Code], [CreateDate], [CreateEmployeeId], [Description], [DisplayName], [GroupName], [IsActive], [IsDelete], [LevelNo], [Name], [OrderNo], [ParentId], [Symbol], [UpdateDate], [UpdateEmployeeId])
    VALUES (1, NULL, ''0001-01-01T00:00:00.0000000'', NULL, NULL, N''Parameters'', N''Parameters'', CAST(1 AS bit), CAST(0 AS bit), 0, N''Parameters'', 0, 0, NULL, NULL, NULL),
    (2, NULL, ''0001-01-01T00:00:00.0000000'', NULL, N''Anket erişim tipi parametreleri'', N''Anket Erişim Türleri'', N''SurveyAccessTypes'', CAST(1 AS bit), CAST(0 AS bit), 1, N''SurveyAccessTypes'', 0, 1, NULL, NULL, NULL),
    (3, N''INTERNAL'', ''0001-01-01T00:00:00.0000000'', NULL, N''Sadece kurum içi erişim'', N''Dahili'', N''SurveyAccessTypes'', CAST(1 AS bit), CAST(0 AS bit), 2, N''Internal'', 0, 2, NULL, NULL, NULL),
    (4, N''PUBLIC'', ''0001-01-01T00:00:00.0000000'', NULL, N''Herkese açık erişim'', N''Halka Açık'', N''SurveyAccessTypes'', CAST(1 AS bit), CAST(0 AS bit), 2, N''Public'', 1, 2, NULL, NULL, NULL),
    (5, NULL, ''0001-01-01T00:00:00.0000000'', NULL, N''Anket soru tipi parametreleri'', N''Soru Türleri'', N''QuestionTypes'', CAST(1 AS bit), CAST(0 AS bit), 1, N''QuestionTypes'', 1, 1, NULL, NULL, NULL),
    (6, N''SINGLE_SELECT'', ''0001-01-01T00:00:00.0000000'', NULL, N''Tek seçenek seçilebilen soru tipi'', N''Tekli Seçim'', N''QuestionTypes'', CAST(1 AS bit), CAST(0 AS bit), 2, N''SingleSelect'', 0, 5, NULL, NULL, NULL),
    (7, N''MULTI_SELECT'', ''0001-01-01T00:00:00.0000000'', NULL, N''Birden fazla seçenek seçilebilen soru tipi'', N''Çoklu Seçim'', N''QuestionTypes'', CAST(1 AS bit), CAST(0 AS bit), 2, N''MultiSelect'', 1, 5, NULL, NULL, NULL),
    (8, N''OPEN_TEXT'', ''0001-01-01T00:00:00.0000000'', NULL, N''Serbest metin girişi yapılabilen soru tipi'', N''Açık Metin'', N''QuestionTypes'', CAST(1 AS bit), CAST(0 AS bit), 2, N''OpenText'', 2, 5, NULL, NULL, NULL),
    (9, N''FILE_UPLOAD'', ''0001-01-01T00:00:00.0000000'', NULL, N''Dosya yüklemesi yapılabilen soru tipi'', N''Dosya Yükleme'', N''QuestionTypes'', CAST(1 AS bit), CAST(0 AS bit), 2, N''FileUpload'', 3, 5, NULL, NULL, NULL),
    (10, N''CONDITIONAL'', ''0001-01-01T00:00:00.0000000'', NULL, N''Koşula bağlı olarak gösterilen soru tipi'', N''Koşullu'', N''QuestionTypes'', CAST(1 AS bit), CAST(0 AS bit), 2, N''Conditional'', 4, 5, NULL, NULL, NULL)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Code', N'CreateDate', N'CreateEmployeeId', N'Description', N'DisplayName', N'GroupName', N'IsActive', N'IsDelete', N'LevelNo', N'Name', N'OrderNo', N'ParentId', N'Symbol', N'UpdateDate', N'UpdateEmployeeId') AND [object_id] = OBJECT_ID(N'[Parameter]'))
        SET IDENTITY_INSERT [Parameter] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AnswerAttachment_AnswerId] ON [AnswerAttachment] ([AnswerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE INDEX [IX_AnswerOption_AnswerId] ON [AnswerOption] ([AnswerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE INDEX [IX_AnswerOption_QuestionOptionId] ON [AnswerOption] ([QuestionOptionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE INDEX [IX_Answer_ParticipationId] ON [Answer] ([ParticipationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE INDEX [IX_Answer_QuestionId] ON [Answer] ([QuestionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Attachment_QuestionId] ON [Attachment] ([QuestionId]) WHERE [QuestionId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Attachment_QuestionOptionId] ON [Attachment] ([QuestionOptionId]) WHERE [QuestionOptionId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Attachment_SurveyId] ON [Attachment] ([SurveyId]) WHERE [SurveyId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Department_ExternalIdentifier] ON [Department] ([ExternalIdentifier]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE INDEX [IX_DependentQuestion_ChildQuestionId] ON [DependentQuestion] ([ChildQuestionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE INDEX [IX_DependentQuestion_ParentQuestionOptionId] ON [DependentQuestion] ([ParentQuestionOptionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Participant_ExternalId] ON [Participant] ([ExternalId]) WHERE [ExternalId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Participant_LdapUsername] ON [Participant] ([LdapUsername]) WHERE [LdapUsername] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE INDEX [IX_Participation_ParticipantId] ON [Participation] ([ParticipantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE INDEX [IX_Participation_SurveyId] ON [Participation] ([SurveyId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE INDEX [IX_QuestionOption_QuestionId] ON [QuestionOption] ([QuestionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE INDEX [IX_Question_SurveyId] ON [Question] ([SurveyId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE INDEX [IX_Question_TypeId] ON [Question] ([TypeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE INDEX [IX_RolePermission_PermissionId] ON [RolePermission] ([PermissionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE INDEX [IX_Survey_DepartmentId] ON [Survey] ([DepartmentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE INDEX [IX_Survey_TypeId] ON [Survey] ([TypeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_UserRefreshToken_Token] ON [UserRefreshToken] ([Token]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE INDEX [IX_UserRefreshToken_UserId] ON [UserRefreshToken] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE INDEX [IX_UserRole_DepartmentId] ON [UserRole] ([DepartmentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE INDEX [IX_UserRole_RoleId] ON [UserRole] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    CREATE INDEX [IX_User_DepartmentId] ON [User] ([DepartmentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251219110645_Initial'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251219110645_Initial', N'9.0.0');
END;

COMMIT;
GO

