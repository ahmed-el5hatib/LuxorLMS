using AcademicApp = LuxorLMS.Academic.Application;
using LuxorLMS.Academic.Application.Interfaces;
using LuxorLMS.Academic.Application.Services;
using LuxorLMS.Academic.Infrastructure.Persistence;
using LuxorLMS.Identity.Application.Interfaces;
using LuxorLMS.Quizzes.Api.Filters;
using LuxorLMS.Quizzes.Application.Interfaces;
using LuxorLMS.Quizzes.Infrastructure.Adapters;
using LuxorLMS.Quizzes.Infrastructure.Persistence;
using LuxorLMS.Quizzes.Infrastructure.Repositories;
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
        Title = "LuxorLMS Quizzes API",
        Version = "v1",
        Description = "Quizzes Module (visual quiz builder, timed attempts, auto-grading, essay grading)",
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
builder.Services.AddDbContext<LuxorLMSQuizzesDbContext>(options =>
    var __conn = builder.Configuration.GetConnectionString("DefaultConnection"); if (__conn != null && __conn.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase)) options.UseSqlite(__conn); else options.UseNpgsql(__conn));
builder.Services.AddDbContext<LuxorLMSAcademicDbContext>(options =>
    var __academicConn = builder.Configuration.GetConnectionString("AcademicConnection") ?? connString; if (__academicConn != null && __academicConn.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase)) options.UseSqlite(__academicConn); else options.UseNpgsql(__academicConn));

builder.Services.AddScoped<IAuthorizationService, LuxorLMS.Identity.Application.Services.AuthorizationService>();

// Repositories + UnitOfWork (Quizzes)
builder.Services.AddScoped<LuxorLMS.Quizzes.Domain.Interfaces.IQuizRepository, QuizRepository>();
builder.Services.AddScoped<LuxorLMS.Quizzes.Domain.Interfaces.IQuizQuestionRepository, QuizQuestionRepository>();
builder.Services.AddScoped<LuxorLMS.Quizzes.Domain.Interfaces.IQuizOptionRepository, QuizOptionRepository>();
builder.Services.AddScoped<LuxorLMS.Quizzes.Domain.Interfaces.IQuizAttemptRepository, QuizAttemptRepository>();
builder.Services.AddScoped<LuxorLMS.Quizzes.Domain.Interfaces.IQuizAnswerRepository, QuizAnswerRepository>();
builder.Services.AddScoped<LuxorLMS.Quizzes.Domain.Interfaces.IUnitOfWork, UnitOfWork>();

// Academic services + repositories (course-offering catalog reads)
builder.Services.AddScoped<ICourseOfferingService, CourseOfferingService>();
builder.Services.AddScoped<LuxorLMS.Academic.Domain.Interfaces.ICourseOfferingRepository, LuxorLMS.Academic.Infrastructure.Repositories.CourseOfferingRepository>();
builder.Services.AddScoped<LuxorLMS.Academic.Domain.Interfaces.ICourseRepository, LuxorLMS.Academic.Infrastructure.Repositories.CourseRepository>();
builder.Services.AddScoped<LuxorLMS.Academic.Domain.Interfaces.ISemesterRepository, LuxorLMS.Academic.Infrastructure.Repositories.SemesterRepository>();
builder.Services.AddScoped<LuxorLMS.Academic.Domain.Interfaces.IUnitOfWork, LuxorLMS.Academic.Infrastructure.Repositories.UnitOfWork>();

// Application services (Quizzes)
builder.Services.AddScoped<IQuizService, LuxorLMS.Quizzes.Application.Services.QuizService>();
builder.Services.AddScoped<IQuizAttemptService, LuxorLMS.Quizzes.Application.Services.QuizAttemptService>();
builder.Services.AddScoped<IQuizAnswerService, LuxorLMS.Quizzes.Application.Services.QuizAnswerService>();
builder.Services.AddScoped<IAcademicQuizGateway, AcademicQuizGateway>();

builder.Services.AddValidatorsFromAssembly(typeof(LuxorLMS.Quizzes.Application.DTOs.QuizDto).Assembly);

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
    var quizzesDb = scope.ServiceProvider.GetRequiredService<LuxorLMSQuizzesDbContext>();
    quizzesDb.Database.Migrate();
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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "LuxorLMS Quizzes API v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

