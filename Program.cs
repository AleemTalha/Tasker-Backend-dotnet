using Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var ConnectionString = builder.Configuration.GetConnectionString("DB_URI");
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(ConnectionString);
});
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme
).AddJwtBearer(options =>
{

    var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new Exception("Jwt:Key is missing in configuration");
    var issuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new Exception("Jwt:Issuer is missing in configuration");

    var audience = builder.Configuration["Jwt:Audience"]
        ?? throw new Exception("Jwt:Audience is missing in configuration");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = issuer,
        ValidAudience = audience,

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey)
        )
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            Console.WriteLine("Authorization Header: " +
                context.Request.Headers.Authorization.ToString());

            return Task.CompletedTask;
        },

        OnTokenValidated = context =>
        {
            Console.WriteLine("✅ Token Validated");

            foreach (var claim in context.Principal!.Claims)
            {
                Console.WriteLine($"{claim.Type} = {claim.Value}");
            }

            return Task.CompletedTask;
        },

        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("Authentication Failed");
            Console.WriteLine(context.Exception.ToString());

            return Task.CompletedTask;
        },

        OnChallenge = context =>
        {
            Console.WriteLine("Challenge Triggered");
            Console.WriteLine(context.Error);
            Console.WriteLine(context.ErrorDescription);

            return Task.CompletedTask;
        }
    };
});
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"{ConsoleColors.Red}Exception: {ex.Message}{ConsoleColors.Reset}");
        if (!context.Response.HasStarted)
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new
            {
                message = "Internal Server Error"
            });
        }
    }
});
app.Use(async (context, next) =>
{
    var start = DateTime.UtcNow;
    await next();
    var duration = DateTime.UtcNow - start;

    string color = context.Response.StatusCode switch
    {
        >= 200 and < 300 => ConsoleColors.Green,
        >= 300 and < 400 => ConsoleColors.Yellow,
        >= 400 and < 500 => ConsoleColors.Red,
        >= 500 => ConsoleColors.Blue,
        _ => ConsoleColors.Reset
    };

    string logMessage = $"{ConsoleColors.Cyan}>>{ConsoleColors.Reset} " +
                        $"{ConsoleColors.Yellow}{context.Request.Method}{ConsoleColors.Reset} " +
                        $"{context.Request.Path} " +
                        $"{color}{context.Response.StatusCode}{ConsoleColors.Reset} " +
                        $"[{duration.TotalMilliseconds:F2}ms]" +
                        $" {ConsoleColors.Cyan}<<{ConsoleColors.Reset}";

    Console.WriteLine(logMessage);
});
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 404)
    {
        if (!context.Response.HasStarted)
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { message = "404 Route Not Found" });
        }
        return;
    }

});

app.Run();

public static class ConsoleColors
{
    public const string Reset = "\u001b[0m";
    public const string Red = "\u001b[31m";
    public const string Green = "\u001b[32m";
    public const string Yellow = "\u001b[33m";
    public const string Blue = "\u001b[34m";
    public const string Cyan = "\u001b[36m";
}