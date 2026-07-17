using FluentValidation;
using LuxorLMS.Academic.Application.Interfaces;
using LuxorLMS.Academic.Infrastructure.Persistence;
using LuxorLMS.Academic.Application.Services;
using LuxorLMS.Identity.Application.Interfaces;
using LuxorLMS.Registration.Api.Authorization;
using LuxorLMS.Registration.Api.Filters;
using LuxorLMS.Registration.Application.Interfaces;
using LuxorLMS.Registration.Infrastructure.Adapters;
using LuxorLMS.Registration.Infrastructure.Persistence;
using LuxorLMS.Registration.Infrastructure.Repositories;
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
        Title = "LuxorLMS Registration API",
        Version = "v1",
        Description = "Registration & Enrollment Validation Module",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact { Name = "LuxorLMS Team" }
    });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
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

// Databases
builder.Services.AddDbContext<LuxorLMSRegistrationDbContext>(options =>
{
    var __conn = builder.Configuration.GetConnectionString("DefaultConnection");
    if (!string.IsNullOrEmpty(__conn) && __conn.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase))
        options.UseSqlite(__conn);
    else
        options.UseNpgsql(__conn);
});
builder.Services.AddDbContext<LuxorLMSAcademicDbContext>(options =>
{
    var __academicConn = builder.Configuration.GetConnectionString("AcademicConnection") ?? builder.Configuration.GetConnectionString("DefaultConnection");
    if (!string.IsNullOrEmpty(__academicConn) && __academicConn.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase))
        options.UseSqlite(__academicConn);
    else
        options.UseNpgsql(__academicConn);
});

// RedLock (distributed seat-booking lock during registration)
builder.Services.AddSingleton<LuxorLMS.Registration.Application.Concurrency.ISeatBookingLockFactory>(sp =>
    new LuxorLMS.Registration.Infrastructure.Concurrency.RedLockSeatBookingLockFactory(
        builder.Configuration.GetConnectionString("Redis") ?? builder.Configuration["Redis:ConnectionString"] ?? "localhost:6379"));
builder.Services.AddScoped<IAuthorizationService, LuxorLMS.Identity.Application.Services.AuthorizationService>();

// Repositories + UnitOfWork (Registration)
builder.Services.AddScoped<LuxorLMS.Registration.Domain.Interfaces.IRegistrationPeriodRepository, RegistrationPeriodRepository>();
builder.Services.AddScoped<LuxorLMS.Registration.Domain.Interfaces.IStudentProgramEnrollmentRepository, StudentProgramEnrollmentRepository>();
builder.Services.AddScoped<LuxorLMS.Registration.Domain.Interfaces.ICourseEnrollmentRepository, CourseEnrollmentRepository>();
builder.Services.AddScoped<LuxorLMS.Registration.Domain.Interfaces.IUnitOfWork, UnitOfWork>();

// Academic services (catalog reads)
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<LuxorLMS.Academic.Domain.Interfaces.IStudentRepository, LuxorLMS.Academic.Infrastructure.Repositories.StudentRepository>();
builder.Services.AddScoped<LuxorLMS.Academic.Application.Interfaces.IStudentService, LuxorLMS.Academic.Application.Services.StudentService>();

// Application services (Registration)
builder.Services.AddScoped<IRegistrationPeriodService, LuxorLMS.Registration.Application.Services.RegistrationPeriodService>();
builder.Services.AddScoped<IStudentProgramEnrollmentService, LuxorLMS.Registration.Application.Services.StudentProgramEnrollmentService>();
builder.Services.AddScoped<ICourseEnrollmentService, LuxorLMS.Registration.Application.Services.CourseEnrollmentService>();
builder.Services.AddScoped<ICourseCatalogService, CourseCatalogAdapter>();

builder.Services.AddValidatorsFromAssembly(typeof(LuxorLMS.Registration.Application.DTOs.RegistrationPeriodDto).Assembly);

// JWT (validate tokens issued by Identity module)
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
    var registrationDb = scope.ServiceProvider.GetRequiredService<LuxorLMSRegistrationDbContext>();
    registrationDb.Database.Migrate();
}

app.UseExceptionHandler(appBuilder =>
{
    appBuilder.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(new
        {
            type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            title = "An unexpected error occurred.",
            status = 500
        });
    });
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "LuxorLMS Registration API v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


