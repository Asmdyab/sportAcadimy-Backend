using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportAcademy.Infrastructure.Migrations
{
    public partial class MakeTraineeIdIdentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                -- Drop foreign keys referencing Trainees
                ALTER TABLE [SportTrainees] DROP CONSTRAINT [FK_SportTrainees_Trainees_TraineeId];
                ALTER TABLE [TraineeCodesHistory] DROP CONSTRAINT [FK_TraineeCodesHistory_Trainees_TraineeId];
                ALTER TABLE [SubscriptionDetails] DROP CONSTRAINT [FK_SubscriptionDetails_Trainees_TraineeId];
                ALTER TABLE [Enrollments] DROP CONSTRAINT [FK_Enrollments_Trainees_TraineeId];

                -- Create new Trainees table with IDENTITY on Id
                CREATE TABLE [Trainees_new] (
                    [Id] int NOT NULL IDENTITY(1,1),
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
                    [IsDeleted] bit NOT NULL DEFAULT 0,
                    [DeletedAt] datetime2 NULL,
                    [DeletedBy] nvarchar(max) NULL,
                    CONSTRAINT [PK_Trainees_new] PRIMARY KEY ([Id]),
                    CONSTRAINT [FK_TAU_AppUserId_temp] FOREIGN KEY ([AppUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
                    CONSTRAINT [FK_TB_BranchId_temp] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
                    CONSTRAINT [FK_TF_FamilyId_temp] FOREIGN KEY ([FamilyId]) REFERENCES [Families] ([Id]) ON DELETE NO ACTION,
                    CONSTRAINT [FK_TN_NationalityCategoryId_temp] FOREIGN KEY ([NationalityCategoryId]) REFERENCES [NationalityCategories] ([Id]) ON DELETE NO ACTION
                );

                -- Copy existing data preserving Ids
                SET IDENTITY_INSERT [Trainees_new] ON;
                INSERT INTO [Trainees_new] (
                    [Id], [TraineeCode], [JoinDate], [IsSubscribed],
                    [ParentNumber], [GuardianName], [AppUserId],
                    [BranchId], [FamilyId], [NationalityCategoryId],
                    [FirstName], [LastName], [SSN], [Email], [BirthDate],
                    [Gender], [Nationality], [Street], [City],
                    [PhoneNumber], [SecondPhoneNumber],
                    [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy],
                    [IsDeleted], [DeletedAt], [DeletedBy]
                )
                SELECT * FROM [Trainees];
                SET IDENTITY_INSERT [Trainees_new] OFF;

                -- Drop old table (removes all its constraints) and rename
                DROP TABLE [Trainees];
                EXEC sp_rename 'Trainees_new', 'Trainees';

                -- Recreate indexes
                CREATE UNIQUE INDEX [IX_Trainees_AppUserId] ON [Trainees] ([AppUserId]) WHERE [AppUserId] IS NOT NULL;
                CREATE INDEX [IX_Trainees_BranchId] ON [Trainees] ([BranchId]);
                CREATE INDEX [IX_Trainees_FamilyId] ON [Trainees] ([FamilyId]) WHERE [FamilyId] IS NOT NULL;
                CREATE INDEX [IX_Trainees_NationalityCategoryId] ON [Trainees] ([NationalityCategoryId]);
                CREATE UNIQUE INDEX [IX_Trainees_TraineeCode] ON [Trainees] ([TraineeCode]) WHERE [TraineeCode] IS NOT NULL;

                -- Recreate foreign keys referencing Trainees
                ALTER TABLE [SportTrainees] ADD CONSTRAINT [FK_SportTrainees_Trainees_TraineeId]
                    FOREIGN KEY ([TraineeId]) REFERENCES [Trainees] ([Id]) ON DELETE NO ACTION;
                ALTER TABLE [TraineeCodesHistory] ADD CONSTRAINT [FK_TraineeCodesHistory_Trainees_TraineeId]
                    FOREIGN KEY ([TraineeId]) REFERENCES [Trainees] ([Id]) ON DELETE CASCADE;
                ALTER TABLE [SubscriptionDetails] ADD CONSTRAINT [FK_SubscriptionDetails_Trainees_TraineeId]
                    FOREIGN KEY ([TraineeId]) REFERENCES [Trainees] ([Id]) ON DELETE NO ACTION;
                ALTER TABLE [Enrollments] ADD CONSTRAINT [FK_Enrollments_Trainees_TraineeId]
                    FOREIGN KEY ([TraineeId]) REFERENCES [Trainees] ([Id]) ON DELETE NO ACTION;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                -- Drop foreign keys referencing Trainees
                ALTER TABLE [SportTrainees] DROP CONSTRAINT [FK_SportTrainees_Trainees_TraineeId];
                ALTER TABLE [TraineeCodesHistory] DROP CONSTRAINT [FK_TraineeCodesHistory_Trainees_TraineeId];
                ALTER TABLE [SubscriptionDetails] DROP CONSTRAINT [FK_SubscriptionDetails_Trainees_TraineeId];
                ALTER TABLE [Enrollments] DROP CONSTRAINT [FK_Enrollments_Trainees_TraineeId];

                CREATE TABLE [Trainees_old] (
                    [Id] int NOT NULL,
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
                    [IsDeleted] bit NOT NULL DEFAULT 0,
                    [DeletedAt] datetime2 NULL,
                    [DeletedBy] nvarchar(max) NULL,
                    CONSTRAINT [PK_Trainees_old] PRIMARY KEY ([Id]),
                    CONSTRAINT [FK_Trainees_AspNetUsers_AppUserId] FOREIGN KEY ([AppUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
                    CONSTRAINT [FK_Trainees_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
                    CONSTRAINT [FK_Trainees_Families_FamilyId] FOREIGN KEY ([FamilyId]) REFERENCES [Families] ([Id]) ON DELETE NO ACTION,
                    CONSTRAINT [FK_Trainees_NationalityCategories_NationalityCategoryId] FOREIGN KEY ([NationalityCategoryId]) REFERENCES [NationalityCategories] ([Id]) ON DELETE NO ACTION
                );

                INSERT INTO [Trainees_old] SELECT * FROM [Trainees];

                DROP TABLE [Trainees];
                EXEC sp_rename 'Trainees_old', 'Trainees';

                -- Recreate indexes
                CREATE UNIQUE INDEX [IX_Trainees_AppUserId] ON [Trainees] ([AppUserId]) WHERE [AppUserId] IS NOT NULL;
                CREATE INDEX [IX_Trainees_BranchId] ON [Trainees] ([BranchId]);
                CREATE INDEX [IX_Trainees_FamilyId] ON [Trainees] ([FamilyId]) WHERE [FamilyId] IS NOT NULL;
                CREATE INDEX [IX_Trainees_NationalityCategoryId] ON [Trainees] ([NationalityCategoryId]);
                CREATE UNIQUE INDEX [IX_Trainees_TraineeCode] ON [Trainees] ([TraineeCode]) WHERE [TraineeCode] IS NOT NULL;

                -- Recreate foreign keys referencing Trainees
                ALTER TABLE [SportTrainees] ADD CONSTRAINT [FK_SportTrainees_Trainees_TraineeId]
                    FOREIGN KEY ([TraineeId]) REFERENCES [Trainees] ([Id]) ON DELETE NO ACTION;
                ALTER TABLE [TraineeCodesHistory] ADD CONSTRAINT [FK_TraineeCodesHistory_Trainees_TraineeId]
                    FOREIGN KEY ([TraineeId]) REFERENCES [Trainees] ([Id]) ON DELETE CASCADE;
                ALTER TABLE [SubscriptionDetails] ADD CONSTRAINT [FK_SubscriptionDetails_Trainees_TraineeId]
                    FOREIGN KEY ([TraineeId]) REFERENCES [Trainees] ([Id]) ON DELETE NO ACTION;
                ALTER TABLE [Enrollments] ADD CONSTRAINT [FK_Enrollments_Trainees_TraineeId]
                    FOREIGN KEY ([TraineeId]) REFERENCES [Trainees] ([Id]) ON DELETE NO ACTION;
            ");
        }
    }
}
