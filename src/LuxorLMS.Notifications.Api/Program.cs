using Hangfire;
using Hangfire.PostgreSql;
using LuxorLMS.Academic.Infrastructure.Persistence;
using LuxorLMS.Identity.Application.Interfaces;
using LuxorLMS.Identity.Infrastructure.Persistence;
using LuxorLMS.Notifications.Application.Interfaces;
using LuxorLMS.Notifications.Application.Services;
using LuxorLMS.Notifications.Domain.Interfaces;
using LuxorLMS.Notifications.Infrastructure.Gateways;
using LuxorLMS.Notifications.Infrastructure.Jobs;
using LuxorLMS.Notifications.Infrastructure.Persistence;
using LuxorLMS.Notifications.Infrastructure.Repositories;
using LuxorLMS.Notifications.Infrastructure.Senders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "LuxorLMS Notifications API",
        Version = "v1",
        Description = "Notifications Module (Multi-channel dispatch, Hangfire async processing, fallback routing)",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact { Name = "LuxorLMS Team" }
    });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using Bearer scheme.",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var connString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Host=localhost;Database=luxorlms_notifications;Username=postgres;Password=postgres";

builder.Services.AddDbContext<LuxorLMSNotificationsDbContext>(options => var __anyConn = connString; if (__anyConn != null && __anyConn.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase)) options.UseSqlite(__anyConn); else options.UseNpgsql(__anyConn));
builder.Services.AddDbContext<LuxorLMSIdentityDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("IdentityConnection") ?? connString));
builder.Services.AddDbContext<LuxorLMSAcademicDbContext>(options => var __academicConn = builder.Configuration.GetConnectionString("AcademicConnection") ?? connString; if (__academicConn != null && __academicConn.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase)) options.UseSqlite(__academicConn); else options.UseNpgsql(__academicConn));

builder.Services.AddScoped<IAuthorizationService, LuxorLMS.Identity.Application.Services.AuthorizationService>();

builder.Services.AddScoped<INotificationTemplateRepository, NotificationTemplateRepository>();
builder.Services.AddScoped<INotificationMessageRepository, NotificationMessageRepository>();
builder.Services.AddScoped<INotificationPreferenceRepository, NotificationPreferenceRepository>();
builder.Services.AddScoped<INotificationsUnitOfWork, NotificationsUnitOfWork>();

builder.Services.AddScoped<IChannelSender, InAppSender>();
builder.Services.AddScoped<IChannelSender, EmailSender>();
builder.Services.AddScoped<IChannelSender, SmsSender>();
builder.Services.AddScoped<IChannelSender, PushSender>();

builder.Services.AddScoped<IUserNotificationGateway, UserNotificationGateway>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<SendNotificationJob>();

// Hangfire setup
builder.Services.AddHangfire(config => config.UsePostgreSqlStorage(c => c.UseNpgsqlConnection(connString)));
builder.Services.AddHangfireServer();

var jwtSection = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSection["SecretKey"] ?? "ChangeThisToAStrongRandomSecretKeyAtLeast32CharactersLong!";
var issuer = jwtSection["Issuer"] ?? "LuxorLMS";
var audience = jwtSection["Audience"] ?? "LuxorLMSUsers";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseCors(policy =>
{
    policy.WithOrigins("http://localhost:3000", "http://localhost:5173", "http://localhost:8080")
          .AllowAnyMethod()
          .AllowAnyHeader()
          .AllowCredentials();
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LuxorLMSNotificationsDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "LuxorLMS Notifications API v1"));
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseHangfireDashboard("/hangfire");
app.MapControllers();

app.Run();

