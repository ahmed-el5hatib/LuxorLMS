using LuxorLMS.Identity.Application.Interfaces;
using LuxorLMS.Storage.Application.Interfaces;
using LuxorLMS.Storage.Application.Services;
using LuxorLMS.Storage.Domain.Interfaces;
using LuxorLMS.Storage.Infrastructure.Persistence;
using LuxorLMS.Storage.Infrastructure.Providers;
using LuxorLMS.Storage.Infrastructure.Repositories;
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
        Title = "LuxorLMS Storage API",
        Version = "v1",
        Description = "Storage Module (object storage, expiring signed URLs, file versioning)",
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

builder.Services.AddDbContext<LuxorLMSStorageDbContext>(options =>
{
    var __conn = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Host=localhost;Database=luxorlms_storage;Username=postgres;Password=postgres";
    if (!string.IsNullOrEmpty(__conn) && __conn.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase))
        options.UseSqlite(__conn);
    else
        options.UseNpgsql(__conn);
});

builder.Services.AddScoped<IAuthorizationService, LuxorLMS.Identity.Application.Services.AuthorizationService>();

builder.Services.AddScoped<IStoredFileRepository, StoredFileRepository>();
builder.Services.AddScoped<IFileVersionRepository, FileVersionRepository>();
builder.Services.AddScoped<IStorageProviderConfigRepository, StorageProviderConfigRepository>();
builder.Services.AddScoped<IStorageUnitOfWork, StorageUnitOfWork>();

builder.Services.AddScoped<IStorageProvider, LocalStorageProvider>();
builder.Services.AddScoped<IStorageProvider, S3StorageProvider>();
builder.Services.AddScoped<IStorageProvider, AzureBlobStorageProvider>();
builder.Services.AddScoped<IStorageProvider, MinioStorageProvider>();

builder.Services.AddScoped<IStorageService, StorageService>();

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
    var db = scope.ServiceProvider.GetRequiredService<LuxorLMSStorageDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "LuxorLMS Storage API v1"));
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
