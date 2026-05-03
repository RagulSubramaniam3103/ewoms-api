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

CREATE TABLE [EWO_AdminAuditLog] (
    [Id] int NOT NULL IDENTITY,
    [AdminId] nvarchar(max) NOT NULL,
    [AdminName] nvarchar(max) NOT NULL,
    [Action] nvarchar(max) NOT NULL,
    [TargetId] nvarchar(max) NULL,
    [TargetName] nvarchar(max) NULL,
    [Details] nvarchar(max) NULL,
    [Timestamp] datetime2 NOT NULL,
    [IpAddress] nvarchar(max) NULL,
    CONSTRAINT [PK_EWO_AdminAuditLog] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [EWO_ChatGroup] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [CreatedByUserId] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_EWO_ChatGroup] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [EWO_Conversation] (
    [Id] int NOT NULL IDENTITY,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_EWO_Conversation] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [EWO_DeleteUserPost] (
    [SNo] int NOT NULL IDENTITY,
    [UId] int NOT NULL,
    [UserId] nvarchar(max) NOT NULL,
    [ProfileImage] varbinary(max) NOT NULL,
    [Caption] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [DeletedBy] nvarchar(max) NOT NULL,
    [DeletedAt] datetime2 NOT NULL,
    [Reason] nvarchar(max) NULL,
    CONSTRAINT [PK_EWO_DeleteUserPost] PRIMARY KEY ([SNo])
);
GO

CREATE TABLE [EWO_MasterNotification] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(max) NOT NULL,
    [Message] nvarchar(max) NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [IsRead] bit NOT NULL,
    CONSTRAINT [PK_EWO_MasterNotification] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [EWO_MasterRole] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_EWO_MasterRole] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [EWO_MasterUser] (
    [Id] nvarchar(450) NOT NULL,
    [FullName] nvarchar(max) NULL,
    [CreatedUser] datetime2 NULL,
    [IsActive] bit NOT NULL,
    [ProfileImage] varbinary(max) NULL,
    [IsPrivate] bit NOT NULL,
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
    CONSTRAINT [PK_EWO_MasterUser] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [EWO_MasterUserBackup] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(max) NOT NULL,
    [UserName] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [FullName] nvarchar(max) NULL,
    [UserRole] nvarchar(max) NULL,
    [CreatedUser] datetime2 NULL,
    [ProfileImage] varbinary(max) NULL,
    [DeletedBy] nvarchar(max) NOT NULL,
    [DeletedAt] datetime2 NOT NULL,
    [Reason] nvarchar(max) NULL,
    CONSTRAINT [PK_EWO_MasterUserBackup] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [EWO_PostComment] (
    [Id] int NOT NULL IDENTITY,
    [PostId] int NOT NULL,
    [UserId] nvarchar(max) NOT NULL,
    [Content] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_EWO_PostComment] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [EWO_PostLike] (
    [Id] int NOT NULL IDENTITY,
    [PostId] int NOT NULL,
    [UserId] nvarchar(max) NOT NULL,
    [LikedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_EWO_PostLike] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [EWO_SavedPost] (
    [Id] int NOT NULL IDENTITY,
    [PostId] int NOT NULL,
    [UserId] nvarchar(max) NOT NULL,
    [SavedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_EWO_SavedPost] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [EWO_UserPost] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(max) NOT NULL,
    [profileimage] varbinary(max) NOT NULL,
    [Caption] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [IsActive] bit NOT NULL,
    [IsBlurred] bit NOT NULL,
    [IsDeleted] bit NOT NULL,
    [ModeratedBy] nvarchar(max) NULL,
    [ModeratedAt] datetime2 NULL,
    CONSTRAINT [PK_EWO_UserPost] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [EWO_UserStory] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(max) NOT NULL,
    [StoryImage] varbinary(max) NOT NULL,
    [Caption] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [ExpiresAt] datetime2 NOT NULL,
    [IsActive] bit NOT NULL,
    [IsBlurred] bit NOT NULL,
    [ViewCount] int NOT NULL,
    CONSTRAINT [PK_EWO_UserStory] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [EWOMS_FriendRequests] (
    [Id] int NOT NULL IDENTITY,
    [SenderId] nvarchar(max) NOT NULL,
    [ReceiverId] nvarchar(max) NOT NULL,
    [RequestDate] datetime2 NOT NULL,
    [Status] int NOT NULL,
    CONSTRAINT [PK_EWOMS_FriendRequests] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Master_UserPasswordLogs] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(max) NULL,
    [PasswordHash] nvarchar(max) NULL,
    [CreatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_Master_UserPasswordLogs] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [EWO_ChatMessage] (
    [Id] int NOT NULL IDENTITY,
    [SenderId] nvarchar(max) NOT NULL,
    [ReceiverId] nvarchar(max) NULL,
    [GroupId] int NULL,
    [Message] nvarchar(max) NOT NULL,
    [SentAt] datetime2 NOT NULL,
    [IsDelivered] bit NOT NULL,
    [DeliveredAt] datetime2 NULL,
    [IsRead] bit NOT NULL,
    [ReadAt] datetime2 NULL,
    [Image] nvarchar(max) NULL,
    [Video] nvarchar(max) NULL,
    [Document] nvarchar(max) NULL,
    [FileName] nvarchar(max) NULL,
    [ConversationId] int NULL,
    CONSTRAINT [PK_EWO_ChatMessage] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EWO_ChatMessage_EWO_Conversation_ConversationId] FOREIGN KEY ([ConversationId]) REFERENCES [EWO_Conversation] ([Id])
);
GO

CREATE TABLE [EWO_RoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_EWO_RoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EWO_RoleClaims_EWO_MasterRole_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [EWO_MasterRole] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [EWO_ChatGroupMember] (
    [Id] int NOT NULL IDENTITY,
    [GroupId] int NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    [JoinedAt] datetime2 NOT NULL,
    [IsAdmin] bit NOT NULL,
    CONSTRAINT [PK_EWO_ChatGroupMember] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EWO_ChatGroupMember_EWO_ChatGroup_GroupId] FOREIGN KEY ([GroupId]) REFERENCES [EWO_ChatGroup] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_EWO_ChatGroupMember_EWO_MasterUser_UserId] FOREIGN KEY ([UserId]) REFERENCES [EWO_MasterUser] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [EWO_ConversationMember] (
    [Id] int NOT NULL IDENTITY,
    [ConversationId] int NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_EWO_ConversationMember] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EWO_ConversationMember_EWO_Conversation_ConversationId] FOREIGN KEY ([ConversationId]) REFERENCES [EWO_Conversation] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_EWO_ConversationMember_EWO_MasterUser_UserId] FOREIGN KEY ([UserId]) REFERENCES [EWO_MasterUser] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [EWO_Followers] (
    [Id] int NOT NULL IDENTITY,
    [FollowerId] nvarchar(450) NOT NULL,
    [FollowingId] nvarchar(450) NOT NULL,
    [FollowedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_EWO_Followers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EWO_Followers_EWO_MasterUser_FollowerId] FOREIGN KEY ([FollowerId]) REFERENCES [EWO_MasterUser] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_EWO_Followers_EWO_MasterUser_FollowingId] FOREIGN KEY ([FollowingId]) REFERENCES [EWO_MasterUser] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [EWO_MasterAdmin] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NULL,
    [UserName] nvarchar(max) NULL,
    [FullName] nvarchar(max) NULL,
    [Email] nvarchar(max) NULL,
    [Password] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [Address1] nvarchar(max) NULL,
    [Address2] nvarchar(max) NULL,
    [City] nvarchar(max) NULL,
    [State] nvarchar(max) NULL,
    [PostalCode] nvarchar(max) NULL,
    [Country] nvarchar(max) NULL,
    CONSTRAINT [PK_EWO_MasterAdmin] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EWO_MasterAdmin_EWO_MasterUser_UserId] FOREIGN KEY ([UserId]) REFERENCES [EWO_MasterUser] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [EWO_MasterManager] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NULL,
    [UserName] nvarchar(max) NULL,
    [FullName] nvarchar(max) NULL,
    [Email] nvarchar(max) NULL,
    [Password] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [Address1] nvarchar(max) NULL,
    [Address2] nvarchar(max) NULL,
    [City] nvarchar(max) NULL,
    [State] nvarchar(max) NULL,
    [PostalCode] nvarchar(max) NULL,
    [Country] nvarchar(max) NULL,
    CONSTRAINT [PK_EWO_MasterManager] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EWO_MasterManager_EWO_MasterUser_UserId] FOREIGN KEY ([UserId]) REFERENCES [EWO_MasterUser] ([Id])
);
GO

CREATE TABLE [EWO_MasterUserDetails] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NULL,
    [UserName] nvarchar(max) NULL,
    [FullName] nvarchar(max) NULL,
    [Email] nvarchar(max) NULL,
    [Password] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [Address1] nvarchar(max) NULL,
    [Address2] nvarchar(max) NULL,
    [City] nvarchar(max) NULL,
    [State] nvarchar(max) NULL,
    [PostalCode] nvarchar(max) NULL,
    [Country] nvarchar(max) NULL,
    CONSTRAINT [PK_EWO_MasterUserDetails] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EWO_MasterUserDetails_EWO_MasterUser_UserId] FOREIGN KEY ([UserId]) REFERENCES [EWO_MasterUser] ([Id])
);
GO

CREATE TABLE [EWO_UserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_EWO_UserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EWO_UserClaims_EWO_MasterUser_UserId] FOREIGN KEY ([UserId]) REFERENCES [EWO_MasterUser] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [EWO_UserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_EWO_UserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_EWO_UserLogins_EWO_MasterUser_UserId] FOREIGN KEY ([UserId]) REFERENCES [EWO_MasterUser] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [EWO_UserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_EWO_UserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_EWO_UserRoles_EWO_MasterRole_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [EWO_MasterRole] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_EWO_UserRoles_EWO_MasterUser_UserId] FOREIGN KEY ([UserId]) REFERENCES [EWO_MasterUser] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [EWO_UserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_EWO_UserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_EWO_UserTokens_EWO_MasterUser_UserId] FOREIGN KEY ([UserId]) REFERENCES [EWO_MasterUser] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [EWO_UserStoryView] (
    [Id] int NOT NULL IDENTITY,
    [StoryId] int NOT NULL,
    [ViewerId] nvarchar(max) NOT NULL,
    [ViewedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_EWO_UserStoryView] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EWO_UserStoryView_EWO_UserStory_StoryId] FOREIGN KEY ([StoryId]) REFERENCES [EWO_UserStory] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_EWO_ChatGroupMember_GroupId] ON [EWO_ChatGroupMember] ([GroupId]);
GO

CREATE INDEX [IX_EWO_ChatGroupMember_UserId] ON [EWO_ChatGroupMember] ([UserId]);
GO

CREATE INDEX [IX_EWO_ChatMessage_ConversationId] ON [EWO_ChatMessage] ([ConversationId]);
GO

CREATE INDEX [IX_EWO_ConversationMember_ConversationId] ON [EWO_ConversationMember] ([ConversationId]);
GO

CREATE INDEX [IX_EWO_ConversationMember_UserId] ON [EWO_ConversationMember] ([UserId]);
GO

CREATE INDEX [IX_EWO_Followers_FollowerId] ON [EWO_Followers] ([FollowerId]);
GO

CREATE INDEX [IX_EWO_Followers_FollowingId] ON [EWO_Followers] ([FollowingId]);
GO

CREATE INDEX [IX_EWO_MasterAdmin_UserId] ON [EWO_MasterAdmin] ([UserId]);
GO

CREATE INDEX [IX_EWO_MasterManager_UserId] ON [EWO_MasterManager] ([UserId]);
GO

CREATE UNIQUE INDEX [RoleNameIndex] ON [EWO_MasterRole] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO

CREATE INDEX [EmailIndex] ON [EWO_MasterUser] ([NormalizedEmail]);
GO

CREATE UNIQUE INDEX [UserNameIndex] ON [EWO_MasterUser] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO

CREATE INDEX [IX_EWO_MasterUserDetails_UserId] ON [EWO_MasterUserDetails] ([UserId]);
GO

CREATE INDEX [IX_EWO_RoleClaims_RoleId] ON [EWO_RoleClaims] ([RoleId]);
GO

CREATE INDEX [IX_EWO_UserClaims_UserId] ON [EWO_UserClaims] ([UserId]);
GO

CREATE INDEX [IX_EWO_UserLogins_UserId] ON [EWO_UserLogins] ([UserId]);
GO

CREATE INDEX [IX_EWO_UserRoles_RoleId] ON [EWO_UserRoles] ([RoleId]);
GO

CREATE INDEX [IX_EWO_UserStoryView_StoryId] ON [EWO_UserStoryView] ([StoryId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260503055312_InitialSqlServerMigration', N'8.0.4');
GO

COMMIT;
GO

