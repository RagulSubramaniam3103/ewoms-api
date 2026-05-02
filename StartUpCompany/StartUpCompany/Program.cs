using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using StartUpCompany.MainModel.Data_DB;
using StartUpCompany.MainModel;
using System.Threading.RateLimiting;
using StartUpCompany.CQRSMethod.Command.UserCreate;
using StartUpCompany.CQRSMethod.Queries.Usersabstract;
using StartUpCompany.CQRSMethod.Queries.Users;
using StartUpCompany.MainModel.Data_AutoMapper.UsersEdit;
using StartUpCompany.FactoryDI;
using Microsoft.Extensions.DependencyInjection;
using StartUpCompany.CQRSMethod.Queries.UserAllDetails;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using StartUpCompany.Services.GenerateToken;
using StartUpCompany.CQRSMethod.Queries.UserControlled;
using StartUpCompany.Services.UserControlled;
using Microsoft.OpenApi.Models;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DataDBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<MasterUsers, IdentityRole>().AddEntityFrameworkStores<DataDBContext>().AddDefaultTokenProviders();

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter(policyName: "Fixed", options =>
    {
        options.PermitLimit = 5;
        options.Window = TimeSpan.FromSeconds(10);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 2;
    });
});

//builder.Services.AddMediatR(typeof(Program));

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

//builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
//builder.Services.AddAutoMapper(typeof(AutoMappingUserEdit));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<UserDetailsCommandsHandler>();
builder.Services.AddScoped<UserQueryHandler>();
builder.Services.AddScoped<IUserAbstract, HandlerUserAbstract>();
builder.Services.AddScoped<AbstractUserIDRole, HanndlerUserAdminAbstract>();
builder.Services.AddScoped<AbstractUserIDRole, HanndlerUserStudentAbstract>();
builder.Services.AddScoped<AbstractUserIDRole, HanndlerUserStaffAbstract>();

//Factory DI

builder.Services.AddScoped<HanndlerUserAdminAbstract>();
builder.Services.AddScoped<HanndlerUserStudentAbstract>();
builder.Services.AddScoped<HanndlerUserStaffAbstract>();

builder.Services.AddScoped<IFAbstractUserDetails, FAbstractUserDetails>();
builder.Services.AddScoped<IFAbstractAllUserDetails, FAbstractAlluserDetails>();

builder.Services.AddScoped<UserGetallQueryHandlerClass>();
builder.Services.AddScoped<HandlerAllUserDetails_Admin>();
builder.Services.AddScoped<HandlerAllUserDetails_Staff>();
builder.Services.AddScoped<HandlerAllUserDetails_Student>();

builder.Services.AddScoped<IUserLoginServices, UsertLoginServices>();
builder.Services.AddScoped<UserControlled_QueryCommandHandler>();
builder.Services.AddScoped<UserControlled_Login>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyOrigin().AllowAnyHeader();
    });
});


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";
    options.DefaultScheme = "Bearer";
})
.AddJwtBearer("Bearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
        ),

        RoleClaimType = ClaimTypes.Role,
        NameClaimType = ClaimTypes.Email
    };
});


builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter the Token Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
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
            new string[] {}
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseRateLimiter();

app.MapControllers();

app.Run();
