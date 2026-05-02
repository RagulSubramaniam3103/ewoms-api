using EWOMS_Application_CQRS.Commands;
using EWOMS_Application_CQRS.Commands.UpdateUser;
using EWOMS_Application_CQRS.Commands.UserPost;
using EWOMS_Application_CQRS.Queries.LockoutUser;
using EWOMS_Application_CQRS.Queries.LoginQueries;
using EWOMS_Application_CQRS.Queries.PasswordReset;
using EWOMS_Application_CQRS.Queries.RegisteredUser;
using EWOMS_Application_CQRS.Queries.Audit;
using EWOMS_ExternalClassLibrary_DTO.UserData_DTO;
using EWOMS_Application_CQRS.Commands.UserDeletion;
using EWOMS_Application_CQRS.Queries.DeletedData;
using EWOMS_ClassLibrary.DataControlled;
using EWOMS_ClassLibrary.DataIntegration;
using EWOMS_ClassLibrary.EmailSending;
using EWOMS_ClassLibrary.JWTToken;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text;
using EWOMS_ClassLibrary.Services;

var builder = WebApplication.CreateBuilder(args);

//DB Connection

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

//-----------//


//---------Dependency Injection----------//

builder.Services.AddScoped<MasterUser_LoginHandler>();
builder.Services.AddScoped<LoginAuthentication_Token>();
builder.Services.AddScoped<MasterUser_LockoutHandler>();
builder.Services.AddScoped<MasterUser_RegisterHandler>();
builder.Services.AddScoped<MasterUser_ForgotPwdHandler>();
builder.Services.AddScoped<MasterGetUserDetails_RoleHandler>();
builder.Services.AddScoped<MasterUser_GetLockoutHandler>();
builder.Services.AddScoped<MasterForgotpassprofilehandler>();
builder.Services.AddScoped<MasterUser_NewNotificationHandler>();
builder.Services.AddScoped<MasterUser_MarkNotificationsReadHandler>();
builder.Services.AddScoped<MasterUser_MarkSingleNotificationReadHandler>();
builder.Services.AddScoped<MasterAdminUpdateHandler>();
builder.Services.AddScoped<MasterManagerUpdateHandler>();
builder.Services.AddScoped<MasterUserUpdateHandler>();
builder.Services.AddScoped<MasterUserDeletePostHandler>();
builder.Services.AddScoped<MasterUser_DeleteFullDataHandler>();

builder.Services.AddScoped<MasterUserPostHandler>();
builder.Services.AddScoped<MasterUserPostGet_Handler>();
builder.Services.AddScoped<MasterUserArchivePost>();
builder.Services.AddScoped<MasterUserBlurPostHandler>();
builder.Services.AddScoped<TogglePostLikeHandler>();
builder.Services.AddScoped<ToggleSavePostHandler>();
builder.Services.AddScoped<GetSavedPostsHandler>();
builder.Services.AddScoped<GetDashboardStatsHandler>();
builder.Services.AddScoped<AddCommentHandler>();
builder.Services.AddScoped<GetCommentsHandler>();
builder.Services.AddScoped<MasterUserStoryHandler>();
builder.Services.AddScoped<SeedStoryHandler>();
builder.Services.AddScoped<GetDeletedUsersHandler>();
builder.Services.AddScoped<GetDeletedPostsHandler>();
builder.Services.AddScoped<GetAdminAuditLogsHandler>();

builder.Services.AddScoped<ForgotPasswordTokenGenerate>();
builder.Services.AddScoped<ForgotPassword_EmailSentHandler>();




// Interface intergrated DI

builder.Services.AddScoped<IEmailService, SendingEmail>();

//---------------------------------------//

builder.Services.AddIdentity<MasterUser,IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();




//-----Attempt lockout-----//

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
});

//-------------------------//


//-----JWT Swagger Authorize-----//

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "EWOMS API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter Token -> {Token}"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

//-------------------------------//

//-----JWT Authentication-----//

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        RoleClaimType = System.Security.Claims.ClaimTypes.Role
    };

    // Allow SignalR to receive JWT from query string (WebSocket connections can't set headers)
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) &&
                path.StartsWithSegments("/chatHub"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

//----------------------------//


//builder.Services.AddAuthorization();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Adminaccess", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("Manageraccess", policy =>
        policy.RequireRole("Manager"));

    options.AddPolicy("Useraccess", policy =>
        policy.RequireRole("User"));

    options.AddPolicy("Administration", policy =>
        policy.RequireRole("Admin", "Manager"));

    options.AddPolicy("AllowAll", policy =>
        policy.RequireRole("Admin", "Manager", "User"));
});

builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => {
        policy.WithOrigins("http://localhost:54950", "http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Required for SignalR WebSocket connections
    });
});



// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// SignalR with CamelCase JSON serialization and increased message size for images
builder.Services.AddSignalR(options => {
    options.EnableDetailedErrors = true;
    options.MaximumReceiveMessageSize = 1024 * 1024 * 100; // 100MB limit for base64 images
    options.StreamBufferCapacity = 1024 * 1024 * 100;
}).AddJsonProtocol(options => {
    options.PayloadSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});
builder.Services.AddSingleton<UserConnectionManager>();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    // Ensure database is created and migrations are applied
    await context.Database.MigrateAsync();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = { "Admin", "User", "Manager" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Ensure Backup Tables Exist (SQLite Syntax)
    string createBackupTablesSql = @"
        CREATE TABLE IF NOT EXISTS [EWO_MasterUserBackup] (
            [Id] INTEGER PRIMARY KEY AUTOINCREMENT,
            [UserId] TEXT NOT NULL,
            [UserName] TEXT NOT NULL,
            [Email] TEXT NOT NULL,
            [FullName] TEXT NULL,
            [UserRole] TEXT NULL,
            [CreatedUser] TEXT NULL,
            [ProfileImage] BLOB NULL,
            [DeletedBy] TEXT NOT NULL,
            [DeletedAt] TEXT NOT NULL,
            [Reason] TEXT NULL
        );

        CREATE TABLE IF NOT EXISTS [EWO_DeleteUserPost] (
            [SNo] INTEGER PRIMARY KEY AUTOINCREMENT,
            [UId] INTEGER NOT NULL,
            [UserId] TEXT NOT NULL,
            [ProfileImage] BLOB NULL,
            [Caption] TEXT NULL,
            [CreatedAt] TEXT NOT NULL,
            [DeletedBy] TEXT NOT NULL,
            [DeletedAt] TEXT NOT NULL,
            [Reason] TEXT NULL
        );

        CREATE TABLE IF NOT EXISTS [EWO_AdminAuditLog] (
            [Id] INTEGER PRIMARY KEY AUTOINCREMENT,
            [AdminId] TEXT NOT NULL,
            [AdminName] TEXT NOT NULL,
            [Action] TEXT NOT NULL,
            [TargetId] TEXT NULL,
            [TargetName] TEXT NULL,
            [Details] TEXT NULL,
            [Timestamp] TEXT NOT NULL,
            [IpAddress] TEXT NULL
        );";
    
    await context.Database.ExecuteSqlRawAsync(createBackupTablesSql);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.MapHub<EWOMS_CoreAPI.Hubber.ChatHub>("/chatHub");

app.Run();
