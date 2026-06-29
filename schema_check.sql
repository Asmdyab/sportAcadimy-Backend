CREATE SEQUENCE [FamilyCodeSequence] AS int START WITH 1 INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 NO CYCLE;
GO


CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO


CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [IsBanned] bit NOT NULL DEFAULT CAST(0 AS bit),
    [CreatedAt] datetime2 NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [UpdatedAt] datetime2 NULL,
    [UpdatedBy] nvarchar(max) NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    [DeletedAt] datetime2 NULL,
    [DeletedBy] nvarchar(max) NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);
GO


CREATE TABLE [Branches] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [City] nvarchar(50) NOT NULL,
    [Country] nvarchar(50) NOT NULL,
    [PhoneNumber] nvarchar(13) NOT NULL,
    [Email] nvarchar(50) NULL,
    [CoX] nvarchar(50) NOT NULL,
    [CoY] nvarchar(50) NOT NULL,
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
    CONSTRAINT [PK_Branches] PRIMARY KEY ([Id])
);
GO


CREATE TABLE [ChatConversations] (
    [Id] uniqueidentifier NOT NULL,
    [Title] nvarchar(100) NULL,
    [UserId] nvarchar(450) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_ChatConversations] PRIMARY KEY ([Id])
);
GO


CREATE TABLE [Families] (
    [Id] int NOT NULL DEFAULT (NEXT VALUE FOR FamilyCodeSequence),
    [FamilyCode] int NOT NULL,
    [LastMemberNumber] int NOT NULL,
    CONSTRAINT [PK_Families] PRIMARY KEY ([Id])
);
GO


CREATE TABLE [NationalityCategories] (
    [Id] int NOT NULL IDENTITY,
    [Code] nvarchar(3) NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    CONSTRAINT [PK_NationalityCategories] PRIMARY KEY ([Id])
);
GO


CREATE TABLE [Notification] (
    [Id] int NOT NULL IDENTITY,
    [Message] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
    [GroupName] nvarchar(30) NULL,
    CONSTRAINT [PK_Notification] PRIMARY KEY ([Id])
);
GO


CREATE TABLE [Sports] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(50) NOT NULL,
    [Description] nvarchar(500) NULL,
    [Category] nvarchar(max) NOT NULL,
    [IsRequireHealthTest] bit NOT NULL DEFAULT CAST(1 AS bit),
    CONSTRAINT [PK_Sports] PRIMARY KEY ([Id])
);
GO


CREATE TABLE [SubscriptionTypes] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [DaysPerMonth] int NOT NULL,
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
    [IsOffer] bit NOT NULL DEFAULT CAST(0 AS bit),
    CONSTRAINT [PK_SubscriptionTypes] PRIMARY KEY ([Id])
);
GO


CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [Profiles] (
    [AppUserId] nvarchar(450) NOT NULL,
    [ProfileImageUrl] nvarchar(255) NULL,
    [Bio] nvarchar(300) NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Profiles] PRIMARY KEY ([AppUserId]),
    CONSTRAINT [FK_Profiles_AspNetUsers_AppUserId] FOREIGN KEY ([AppUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [VideoAnalyses] (
    [Id] uniqueidentifier NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    [LandmarksJson] nvarchar(max) NOT NULL,
    [MovementType] nvarchar(max) NOT NULL,
    [AiAnalysisResult] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_VideoAnalyses] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_VideoAnalyses_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [Employees] (
    [Id] int NOT NULL IDENTITY,
    [Salary] decimal(18,2) NOT NULL,
    [HireDate] datetime2 NOT NULL,
    [Position] nvarchar(max) NOT NULL,
    [IsWork] bit NOT NULL DEFAULT CAST(1 AS bit),
    [BranchId] int NOT NULL,
    [AppUserId] nvarchar(450) NULL,
    [FirstName] nvarchar(50) NOT NULL,
    [LastName] nvarchar(50) NOT NULL,
    [SSN] nvarchar(14) NOT NULL,
    [Email] nvarchar(200) NOT NULL,
    [BirthDate] date NOT NULL,
    [Gender] nvarchar(max) NOT NULL,
    [Nationality] nvarchar(max) NOT NULL,
    [Street] nvarchar(70) NOT NULL,
    [City] nvarchar(50) NOT NULL,
    [PhoneNumber] nvarchar(12) NOT NULL,
    [SecondPhoneNumber] nvarchar(12) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [UpdatedAt] datetime2 NULL,
    [UpdatedBy] nvarchar(max) NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    [DeletedAt] datetime2 NULL,
    [DeletedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_Employees] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Employees_AspNetUsers_AppUserId] FOREIGN KEY ([AppUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Employees_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION
);
GO


CREATE TABLE [Payments] (
    [PaymentNumber] nvarchar(50) NOT NULL,
    [Method] nvarchar(max) NOT NULL,
    [PaidDate] datetime2 NOT NULL,
    [BranchId] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [UpdatedAt] datetime2 NULL,
    [UpdatedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_Payments] PRIMARY KEY ([PaymentNumber]),
    CONSTRAINT [FK_Payments_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION
);
GO


CREATE TABLE [ChatMessages] (
    [Id] uniqueidentifier NOT NULL,
    [ChatConversationId] uniqueidentifier NOT NULL,
    [Role] nvarchar(max) NOT NULL,
    [Content] nvarchar(2000) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_ChatMessages] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ChatMessages_ChatConversations_ChatConversationId] FOREIGN KEY ([ChatConversationId]) REFERENCES [ChatConversations] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [Trainees] (
    [Id] int NOT NULL IDENTITY,
    [TraineeCode] nvarchar(25) NOT NULL,
    [JoinDate] date NOT NULL DEFAULT (GETDATE()),
    [IsSubscribed] bit NOT NULL,
    [ParentNumber] nvarchar(13) NULL,
    [GuardianName] nvarchar(50) NULL,
    [AppUserId] nvarchar(450) NULL,
    [BranchId] int NOT NULL,
    [FamilyId] int NOT NULL,
    [NationalityCategoryId] int NOT NULL,
    [FirstName] nvarchar(50) NOT NULL,
    [LastName] nvarchar(50) NOT NULL,
    [SSN] nvarchar(14) NOT NULL,
    [Email] nvarchar(200) NOT NULL,
    [BirthDate] date NOT NULL,
    [Gender] nvarchar(max) NOT NULL,
    [Nationality] nvarchar(max) NOT NULL,
    [Street] nvarchar(70) NOT NULL,
    [City] nvarchar(50) NOT NULL,
    [PhoneNumber] nvarchar(12) NOT NULL,
    [SecondPhoneNumber] nvarchar(12) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [UpdatedAt] datetime2 NULL,
    [UpdatedBy] nvarchar(max) NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    [DeletedAt] datetime2 NULL,
    [DeletedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_Trainees] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Trainees_AspNetUsers_AppUserId] FOREIGN KEY ([AppUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Trainees_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Trainees_Families_FamilyId] FOREIGN KEY ([FamilyId]) REFERENCES [Families] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Trainees_NationalityCategories_NationalityCategoryId] FOREIGN KEY ([NationalityCategoryId]) REFERENCES [NationalityCategories] ([Id]) ON DELETE NO ACTION
);
GO


CREATE TABLE [NotificationRecipiens] (
    [NotificationId] int NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    [IsRead] bit NOT NULL DEFAULT CAST(0 AS bit),
    CONSTRAINT [PK_NotificationRecipiens] PRIMARY KEY ([UserId], [NotificationId]),
    CONSTRAINT [FK_NotificationRecipiens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_NotificationRecipiens_Notification_NotificationId] FOREIGN KEY ([NotificationId]) REFERENCES [Notification] ([Id]) ON DELETE NO ACTION
);
GO


CREATE TABLE [SportBranches] (
    [SportId] int NOT NULL,
    [BranchId] int NOT NULL,
    CONSTRAINT [PK_SportBranches] PRIMARY KEY ([SportId], [BranchId]),
    CONSTRAINT [FK_SportBranches_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_SportBranches_Sports_SportId] FOREIGN KEY ([SportId]) REFERENCES [Sports] ([Id]) ON DELETE NO ACTION
);
GO


CREATE TABLE [SportSubscriptionTypes] (
    [SportId] int NOT NULL,
    [SubscriptionTypeId] int NOT NULL,
    CONSTRAINT [PK_SportSubscriptionTypes] PRIMARY KEY ([SportId], [SubscriptionTypeId]),
    CONSTRAINT [FK_SportSubscriptionTypes_Sports_SportId] FOREIGN KEY ([SportId]) REFERENCES [Sports] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_SportSubscriptionTypes_SubscriptionTypes_SubscriptionTypeId] FOREIGN KEY ([SubscriptionTypeId]) REFERENCES [SubscriptionTypes] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [Coaches] (
    [EmployeeId] int NOT NULL,
    [SkillLevel] nvarchar(max) NOT NULL,
    [Rate] int NOT NULL DEFAULT 2,
    [SportId] int NOT NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    [DeletedAt] datetime2 NULL,
    [DeletedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_Coaches] PRIMARY KEY ([EmployeeId]),
    CONSTRAINT [FK_Coaches_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Coaches_Sports_SportId] FOREIGN KEY ([SportId]) REFERENCES [Sports] ([Id]) ON DELETE NO ACTION
);
GO


CREATE TABLE [SportTrainees] (
    [SportId] int NOT NULL,
    [TraineeId] int NOT NULL,
    [SkillLevel] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_SportTrainees] PRIMARY KEY ([SportId], [TraineeId]),
    CONSTRAINT [FK_SportTrainees_Sports_SportId] FOREIGN KEY ([SportId]) REFERENCES [Sports] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_SportTrainees_Trainees_TraineeId] FOREIGN KEY ([TraineeId]) REFERENCES [Trainees] ([Id]) ON DELETE NO ACTION
);
GO


CREATE TABLE [TraineeCodesHistory] (
    [Id] int NOT NULL IDENTITY,
    [TraineeId] int NOT NULL,
    [OldTraineeCode] nvarchar(50) NOT NULL,
    [ChangedAt] datetime2 NOT NULL DEFAULT (SYSUTCDATETIME()),
    [Reason] nvarchar(500) NULL,
    CONSTRAINT [PK_TraineeCodesHistory] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TraineeCodesHistory_Trainees_TraineeId] FOREIGN KEY ([TraineeId]) REFERENCES [Trainees] ([Id]) ON DELETE NO ACTION
);
GO


CREATE TABLE [SportPrices] (
    [SportId] int NOT NULL,
    [BranchId] int NOT NULL,
    [SubsTypeId] int NOT NULL,
    [Price] decimal(10,2) NOT NULL,
    CONSTRAINT [PK_SportPrices] PRIMARY KEY ([SportId], [BranchId], [SubsTypeId]),
    CONSTRAINT [FK_SportPrices_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_SportPrices_SportSubscriptionTypes_SportId_SubsTypeId] FOREIGN KEY ([SportId], [SubsTypeId]) REFERENCES [SportSubscriptionTypes] ([SportId], [SubscriptionTypeId]) ON DELETE CASCADE
);
GO


CREATE TABLE [TraineeGroups] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(200) NOT NULL DEFAULT N'Trainee Group',
    [SkillLevel] nvarchar(max) NOT NULL,
    [MaximumCapacity] int NOT NULL DEFAULT 15,
    [DurationInMinutes] int NOT NULL DEFAULT 55,
    [Gender] nvarchar(max) NOT NULL,
    [BranchId] int NOT NULL,
    [CoachId] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [UpdatedAt] datetime2 NULL,
    [UpdatedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_TraineeGroups] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TraineeGroups_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_TraineeGroups_Coaches_CoachId] FOREIGN KEY ([CoachId]) REFERENCES [Coaches] ([EmployeeId]) ON DELETE NO ACTION
);
GO


CREATE TABLE [SubscriptionDetails] (
    [Id] int NOT NULL IDENTITY,
    [StartDate] date NOT NULL,
    [EndDate] date NOT NULL,
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
    [PaymentNumber] nvarchar(50) NOT NULL,
    [TraineeId] int NOT NULL,
    [SubscriptionTypeId] int NOT NULL,
    [SportId] int NOT NULL,
    [BranchId] int NOT NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    [DeletedAt] datetime2 NULL,
    [DeletedBy] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [UpdatedAt] datetime2 NULL,
    [UpdatedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_SubscriptionDetails] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_SubscriptionDetails_Payments_PaymentNumber] FOREIGN KEY ([PaymentNumber]) REFERENCES [Payments] ([PaymentNumber]) ON DELETE CASCADE,
    CONSTRAINT [FK_SubscriptionDetails_SportPrices_SportId_BranchId_SubscriptionTypeId] FOREIGN KEY ([SportId], [BranchId], [SubscriptionTypeId]) REFERENCES [SportPrices] ([SportId], [BranchId], [SubsTypeId]) ON DELETE CASCADE,
    CONSTRAINT [FK_SubscriptionDetails_Trainees_TraineeId] FOREIGN KEY ([TraineeId]) REFERENCES [Trainees] ([Id]) ON DELETE NO ACTION
);
GO


CREATE TABLE [GroupSchedules] (
    [Id] int NOT NULL IDENTITY,
    [TraineeGroupId] int NOT NULL,
    [Day] nvarchar(max) NOT NULL,
    [StartTime] time NOT NULL,
    CONSTRAINT [PK_GroupSchedules] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_GroupSchedules_TraineeGroups_TraineeGroupId] FOREIGN KEY ([TraineeGroupId]) REFERENCES [TraineeGroups] ([Id]) ON DELETE NO ACTION
);
GO


CREATE TABLE [Enrollments] (
    [Id] int NOT NULL IDENTITY,
    [EnrollmentDate] datetime2 NOT NULL,
    [ExpiryDate] datetime2 NOT NULL,
    [SessionAllowed] int NOT NULL,
    [SessionRemaining] int NOT NULL,
    [IsActive] bit NOT NULL,
    [TraineeId] int NOT NULL,
    [TraineeGroupId] int NOT NULL,
    [SubscriptionDetailsId] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [UpdatedAt] datetime2 NULL,
    [UpdatedBy] nvarchar(max) NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    [DeletedAt] datetime2 NULL,
    [DeletedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_Enrollments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Enrollments_SubscriptionDetails_SubscriptionDetailsId] FOREIGN KEY ([SubscriptionDetailsId]) REFERENCES [SubscriptionDetails] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Enrollments_TraineeGroups_TraineeGroupId] FOREIGN KEY ([TraineeGroupId]) REFERENCES [TraineeGroups] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Enrollments_Trainees_TraineeId] FOREIGN KEY ([TraineeId]) REFERENCES [Trainees] ([Id]) ON DELETE NO ACTION
);
GO


CREATE TABLE [SessionOccurrences] (
    [Id] int NOT NULL IDENTITY,
    [GroupScheduleId] int NOT NULL,
    [StartDateTime] datetime2 NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [UpdatedAt] datetime2 NULL,
    [UpdatedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_SessionOccurrences] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_SessionOccurrences_GroupSchedules_GroupScheduleId] FOREIGN KEY ([GroupScheduleId]) REFERENCES [GroupSchedules] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [Attendances] (
    [Id] int NOT NULL IDENTITY,
    [AttendanceDate] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [AttendanceStatus] nvarchar(max) NOT NULL,
    [CheckInTime] time NOT NULL DEFAULT (CONVERT(TIME, GETUTCDATE())),
    [CoachNote] nvarchar(500) NOT NULL,
    [EnrollmentId] int NOT NULL,
    [SessionOccurrenceId] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [UpdatedAt] datetime2 NULL,
    [UpdatedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_Attendances] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Attendances_Enrollments_EnrollmentId] FOREIGN KEY ([EnrollmentId]) REFERENCES [Enrollments] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Attendances_SessionOccurrences_SessionOccurrenceId] FOREIGN KEY ([SessionOccurrenceId]) REFERENCES [SessionOccurrences] ([Id]) ON DELETE CASCADE
);
GO


IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Code', N'Name') AND [object_id] = OBJECT_ID(N'[NationalityCategories]'))
    SET IDENTITY_INSERT [NationalityCategories] ON;
INSERT INTO [NationalityCategories] ([Id], [Code], [Name])
VALUES (1, N'AM', N'American'),
(2, N'EU', N'European'),
(3, N'AS', N'Asian'),
(4, N'AF', N'African'),
(5, N'AG', N'Arab Gulf'),
(6, N'AR', N'Arab'),
(7, N'OC', N'Oceanian');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Code', N'Name') AND [object_id] = OBJECT_ID(N'[NationalityCategories]'))
    SET IDENTITY_INSERT [NationalityCategories] OFF;
GO


CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
GO


CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO


CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
GO


CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
GO


CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
GO


CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
GO


CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO


CREATE INDEX [IX_Attendances_EnrollmentId] ON [Attendances] ([EnrollmentId]);
GO


CREATE INDEX [IX_Attendances_SessionOccurrenceId] ON [Attendances] ([SessionOccurrenceId]);
GO


CREATE UNIQUE INDEX [IX_Branch_Coordinates] ON [Branches] ([CoX], [CoY]);
GO


CREATE UNIQUE INDEX [IX_Branch_Email] ON [Branches] ([Email]) WHERE [Email] IS NOT NULL;
GO


CREATE UNIQUE INDEX [IX_Branch_Name] ON [Branches] ([Name]);
GO


CREATE INDEX [IX_ChatMessages_ChatConversationId] ON [ChatMessages] ([ChatConversationId]);
GO


CREATE INDEX [IX_Coaches_SportId] ON [Coaches] ([SportId]);
GO


CREATE UNIQUE INDEX [IX_Employees_AppUserId] ON [Employees] ([AppUserId]) WHERE [AppUserId] IS NOT NULL;
GO


CREATE INDEX [IX_Employees_BranchId] ON [Employees] ([BranchId]);
GO


CREATE UNIQUE INDEX [IX_Enrollments_SubscriptionDetailsId] ON [Enrollments] ([SubscriptionDetailsId]);
GO


CREATE INDEX [IX_Enrollments_TraineeGroupId] ON [Enrollments] ([TraineeGroupId]);
GO


CREATE INDEX [IX_Enrollments_TraineeId] ON [Enrollments] ([TraineeId]);
GO


CREATE INDEX [IX_GroupSchedules_TraineeGroupId] ON [GroupSchedules] ([TraineeGroupId]);
GO


CREATE UNIQUE INDEX [IX_NationalityCategories_Code] ON [NationalityCategories] ([Code]);
GO


CREATE UNIQUE INDEX [IX_NationalityCategories_Name] ON [NationalityCategories] ([Name]);
GO


CREATE INDEX [IX_NotificationRecipiens_NotificationId] ON [NotificationRecipiens] ([NotificationId]);
GO


CREATE INDEX [IX_Payments_BranchId] ON [Payments] ([BranchId]);
GO


CREATE INDEX [IX_SessionOccurrences_GroupScheduleId] ON [SessionOccurrences] ([GroupScheduleId]);
GO


CREATE INDEX [IX_SportBranches_BranchId] ON [SportBranches] ([BranchId]);
GO


CREATE INDEX [IX_SportPrices_BranchId] ON [SportPrices] ([BranchId]);
GO


CREATE INDEX [IX_SportPrices_SportId_SubsTypeId] ON [SportPrices] ([SportId], [SubsTypeId]);
GO


CREATE INDEX [IX_SportSubscriptionTypes_SubscriptionTypeId] ON [SportSubscriptionTypes] ([SubscriptionTypeId]);
GO


CREATE INDEX [IX_SportTrainees_TraineeId] ON [SportTrainees] ([TraineeId]);
GO


CREATE UNIQUE INDEX [IX_SubscriptionDetails_PaymentNumber] ON [SubscriptionDetails] ([PaymentNumber]);
GO


CREATE INDEX [IX_SubscriptionDetails_SportId_BranchId_SubscriptionTypeId] ON [SubscriptionDetails] ([SportId], [BranchId], [SubscriptionTypeId]);
GO


CREATE INDEX [IX_SubscriptionDetails_TraineeId] ON [SubscriptionDetails] ([TraineeId]);
GO


CREATE INDEX [IX_TraineeCodesHistory_TraineeId] ON [TraineeCodesHistory] ([TraineeId]);
GO


CREATE INDEX [IX_TraineeGroups_BranchId] ON [TraineeGroups] ([BranchId]);
GO


CREATE INDEX [IX_TraineeGroups_CoachId] ON [TraineeGroups] ([CoachId]);
GO


CREATE UNIQUE INDEX [IX_Trainees_AppUserId] ON [Trainees] ([AppUserId]) WHERE [AppUserId] IS NOT NULL;
GO


CREATE INDEX [IX_Trainees_BranchId] ON [Trainees] ([BranchId]);
GO


CREATE INDEX [IX_Trainees_FamilyId] ON [Trainees] ([FamilyId]) WHERE [FamilyId] IS NOT NULL;
GO


CREATE INDEX [IX_Trainees_NationalityCategoryId] ON [Trainees] ([NationalityCategoryId]);
GO


CREATE UNIQUE INDEX [IX_Trainees_TraineeCode] ON [Trainees] ([TraineeCode]) WHERE [TraineeCode] IS NOT NULL;
GO


CREATE INDEX [IX_VideoAnalyses_UserId] ON [VideoAnalyses] ([UserId]);
GO


