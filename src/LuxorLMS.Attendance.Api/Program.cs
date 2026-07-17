using LuxorLMS.Academic.Application.Interfaces;
using LuxorLMS.Academic.Application.Services;
using LuxorLMS.Academic.Infrastructure.Persistence;
using LuxorLMS.Identity.Application.Interfaces;
using LuxorLMS.Attendance.Api.Authorization;
using LuxorLMS.Attendance.Api.Filters;
using LuxorLMS.Attendance.Application.Interfaces;
using LuxorLMS.Attendance.Infrastructure.Adapters;
using LuxorLMS.Attendance.Infrastructure.Persistence;
using LuxorLMS.Attendance.Infrastructure.Repositories;
using FluentValidation;
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
        Title = "LuxorLMS Attendance API",
        Version = "v1",
        Description = "Attendance Module (QR sessions, geolocation check-in, absence alerts)",
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
builder.Services.AddDbContext<LuxorLMSAttendanceDbContext>(options =>
    var __conn = builder.Configuration.GetConnectionString("DefaultConnection"); if (__conn != null && __conn.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase)) options.UseSqlite(__conn); else options.UseNpgsql(__conn));
builder.Services.AddDbContext<LuxorLMSAcademicDbContext>(options =>
    var __academicConn = builder.Configuration.GetConnectionString("AcademicConnection") ?? connString; if (__academicConn != null && __academicConn.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase)) options.UseSqlite(__academicConn); else options.UseNpgsql(__academicConn));

builder.Services.AddScoped<IAuthorizationService, LuxorLMS.Identity.Application.Services.AuthorizationService>();

// Repositories + UnitOfWork (Attendance)
builder.Services.AddScoped<LuxorLMS.Attendance.Domain.Interfaces.IAttendanceSessionRepository, AttendanceSessionRepository>();
builder.Services.AddScoped<LuxorLMS.Attendance.Domain.Interfaces.IAttendanceRecordRepository, AttendanceRecordRepository>();
builder.Services.AddScoped<LuxorLMS.Attendance.Domain.Interfaces.IAttendanceAlertRepository, AttendanceAlertRepository>();
builder.Services.AddScoped<LuxorLMS.Attendance.Domain.Interfaces.IUnitOfWork, UnitOfWork>();

// Academic services + repositories (catalog reads)
builder.Services.AddScoped<ICourseOfferingService, CourseOfferingService>();
builder.Services.AddScoped<ISectionService, LuxorLMS.Academic.Application.Services.SectionService>();
builder.Services.AddScoped<LuxorLMS.Academic.Domain.Interfaces.ISectionRepository, LuxorLMS.Academic.Infrastructure.Repositories.SectionRepository>();
builder.Services.AddScoped<LuxorLMS.Academic.Domain.Interfaces.IStudentRepository, LuxorLMS.Academic.Infrastructure.Repositories.StudentRepository>();
builder.Services.AddScoped<LuxorLMS.Academic.Domain.Interfaces.IUnitOfWork, LuxorLMS.Academic.Infrastructure.Repositories.UnitOfWork>();

// Application services (Attendance)
builder.Services.AddScoped<IAttendanceSessionService, LuxorLMS.Attendance.Application.Services.AttendanceSessionService>();
builder.Services.AddScoped<IAttendanceRecordService, LuxorLMS.Attendance.Application.Services.AttendanceRecordService>();
builder.Services.AddScoped<IAttendanceAlertService, LuxorLMS.Attendance.Application.Services.AttendanceAlertService>();
builder.Services.AddScoped<IQrTokenService, LuxorLMS.Attendance.Application.Services.QrTokenService>();
builder.Services.AddScoped<IAcademicAttendanceGateway, AcademicAttendanceGateway>();

builder.Services.AddValidatorsFromAssembly(typeof(LuxorLMS.Attendance.Application.DTOs.AttendanceSessionDto).Assembly);

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
    var attendanceDb = scope.ServiceProvider.GetRequiredService<LuxorLMSAttendanceDbContext>();
    attendanceDb.Database.Migrate();
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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "LuxorLMS Attendance API v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

