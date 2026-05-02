using IdentityWebAPI_User.Data;
using IdentityWebAPI_User.MainModel;
using IdentityWebAPI_User.MainModel.UserEndModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<IdentityDBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<CustomerDetails, IdentityRole>(options =>
{
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.AllowedForNewUsers = true;
}).AddEntityFrameworkStores<IdentityDBContext>().AddDefaultTokenProviders();

builder.Services.AddScoped<GenerateToken>();

#region Authentication - JWT Token

/*---------------------------------------------------------------------------------------------------
                                   Authentication Started - JWT Token
---------------------------------------------------------------------------------------------------*/

var jwtsecretkey = "thisis)(thesecret@#key@#$0jkn(#@";


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

     
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtsecretkey))
    };
});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter the Token like this: Bearer {your token here}"
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

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
});



/*---------------------------------------------------------------------------------------------------
                                   Authentication Ended - JWT Token
---------------------------------------------------------------------------------------------------*/

#endregion

var app = builder.Build();

#region Middleware Explanation

/*---------------------------------------------------------------------------------------------------
                                   Middleware Started
---------------------------------------------------------------------------------------------------*/



app.Use(async (context, next) =>
{
    Console.WriteLine("Middleware Calling : Request Raise - " + DateTime.UtcNow);
    var getstatus = context.Response.StatusCode;
    Console.WriteLine("Response Status Code : " + getstatus + " - " + DateTime.UtcNow);
    await next();
});


/*---------------------------------------------------------------------------------------------------
                                   Middleware Ended
---------------------------------------------------------------------------------------------------*/

#endregion

#region Minimal API Explanation


/*---------------------------------------------------------------------------------------------------
                                   Minimal API Started
---------------------------------------------------------------------------------------------------*/

var logger = app.Services.GetRequiredService<ILogger<Program>>();

logger.LogInformation("Application starting up at {Time}", DateTime.UtcNow);

app.MapGet("/help", () => new
{
    Message = "Test Help Modules",
    DateTime = DateTime.UtcNow.Date,
    Parameter = "Without Parameter"
});

app.MapGet("/help/{getid}", (int getid, ILogger<Program> logger) => 
{
    logger.LogInformation("Received request for help with getid: {GetId}", getid);
    var result = getid switch
    {

        1 => new { Message = "Test Help Modules 1", DateTime = DateTime.UtcNow.Date },
        2 => new { Message = "Test Help Modules 2", DateTime = DateTime.UtcNow.Date },
        _ => new { Message = "Test Help Modules Default", DateTime = DateTime.UtcNow.Date }
    };
    logger.LogInformation("Returning result for getid {GetId}: {Result}", getid, result);
    return Results.Json(result);
});



app.MapPost("/Register",async(UserManager<CustomerDetails> userManager,RoleManager<IdentityRole> roleManager, CustomerRegister customerregister)=>{
    var existingrecords = await userManager.FindByEmailAsync(customerregister.Email);
    if(existingrecords != null)
    {
        return Results.BadRequest(new { Message = "User Already Exists" });
    }
    var user = new CustomerDetails
    {
        UserName = customerregister.UserName,
        CustomerName = customerregister.CustomerName,
        Email = customerregister.Email
    };
    var result = await userManager.CreateAsync(user, customerregister.Password);
    if (!result.Succeeded)
    {
        return Results.BadRequest(new { Message = "User Creation Failed", Errors = result.Errors });
    }
    else
    {
        if (!await roleManager.RoleExistsAsync(customerregister.Role))
        {
            await roleManager.CreateAsync(new IdentityRole(customerregister.Role));
        }
        await userManager.AddToRoleAsync(user, customerregister.Role);
        return Results.Ok(new
        {
            Message = "User created successfully",
            Username = user.UserName,
            Role = customerregister.Role
        });
    }
}).RequireAuthorization("AdminOnly");


app.MapPost("/Login", async (UserManager<CustomerDetails> userManager, GenerateToken GenerateToken, CustomerLogin cusomterlogin) =>
{
    var existinguser = await userManager.FindByEmailAsync(cusomterlogin.Email);
    if (existinguser == null)
    {
        return Results.BadRequest(new { Message = "Invalid Email" });
    }

    if (await userManager.IsLockedOutAsync(existinguser))
    {
        return Results.BadRequest(new
        {
            Message = "Your account is locked due to multiple failed login attempts. Try again later."
        });
    }


    var passwordcheck = await userManager.CheckPasswordAsync(existinguser, cusomterlogin.Password);
    if (!passwordcheck)
    {
        await userManager.AccessFailedAsync(existinguser);

        var attempts = await userManager.GetAccessFailedCountAsync(existinguser);
        return Results.BadRequest(new { Message = "Invalid Email or Password" });
    }
    var tokenservice = GenerateToken.TokenGenerate(existinguser, await userManager.GetRolesAsync(existinguser));
    var finalresult = new
    {
        Username = existinguser.UserName,
        UserEmail = existinguser.Email,
        Token = tokenservice,
    };
    return Results.Ok(finalresult);
});

app.MapGet("/ResetAttempt/{emailid}", async (string emailid, UserManager<CustomerDetails> userManager) =>
{
    var existinguser = await userManager.FindByEmailAsync(emailid);

    if (existinguser == null)
    {
        return Results.BadRequest(new { Message = "Invalid Email" });
    }

    var islocked = await userManager.IsLockedOutAsync(existinguser);

    if (islocked)
    {
        await userManager.SetLockoutEndDateAsync(existinguser, null);

        var result = await userManager.ResetAccessFailedCountAsync(existinguser);

        if (result.Succeeded)
        {
            return Results.Ok(new { Message = "Access Failed Count Reset Successfully" });
        }
    }

    return Results.BadRequest(new
    {
        Message = "Failed to Reset Access Failed Count"
    });
});

app.MapGet("/AdminDashboard", () =>
{
    return Results.Ok(new
    {
        Message = "Admin Dashboard"
    });
}).RequireAuthorization("AdminOnly");

app.MapGet("/UserDashboard", () =>
{
    return Results.Ok(new
    {
        Message = "User Dashboard"
    });
}).RequireAuthorization("UserOnly");

/*---------------------------------------------------------------------------------------------------
                                   Minimal API Ended
---------------------------------------------------------------------------------------------------*/


#endregion

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
