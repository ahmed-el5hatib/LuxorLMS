using AcademicApp = LuxorLMS.Academic.Application;
using LuxorLMS.Academic.Application.Interfaces;
using LuxorLMS.Academic.Application.Services;
using LuxorLMS.Academic.Infrastructure.Persistence;
using LuxorLMS.Identity.Application.Interfaces;
using LuxorLMS.Grading.Api.Filters;
using LuxorLMS.Grading.Application.Interfaces;
using LuxorLMS.Grading.Infrastructure.Adapters;
using LuxorLMS.Grading.Infrastructure.Persistence;
using LuxorLMS.Grading.Infrastructure.Repositories;
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
        Title = "LuxorLMS Grading API",
        Version = "v1",
        Description = "Grading & Transcripts Module (weighted schemas, GPA engine, publish workflow, appeals)",
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
builder.Services.AddDbContext<LuxorLMSGradingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<LuxorLMSAcademicDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AcademicConnection")));

builder.Services.AddScoped<IAuthorizationService, LuxorLMS.Identity.Application.Services.AuthorizationService>();

// Repositories + UnitOfWork (Grading)
builder.Services.AddScoped<LuxorLMS.Grading.Domain.Interfaces.IGradeCategoryRepository, GradeCategoryRepository>();
builder.Services.AddScoped<LuxorLMS.Grading.Domain.Interfaces.IGradeComponentRepository, GradeComponentRepository>();
builder.Services.AddScoped<LuxorLMS.Grading.Domain.Interfaces.IStudentGradeRepository, StudentGradeRepository>();
builder.Services.AddScoped<LuxorLMS.Grading.Domain.Interfaces.IGradeAppealRepository, GradeAppealRepository>();
builder.Services.AddScoped<LuxorLMS.Grading.Domain.Interfaces.IUnitOfWork, UnitOfWork>();

// Academic services + repositories (catalog reads + CGPA write-back)
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ICourseOfferingService, CourseOfferingService>();
builder.Services.AddScoped<LuxorLMS.Academic.Domain.Interfaces.ICourseRepository, LuxorLMS.Academic.Infrastructure.Repositories.CourseRepository>();
builder.Services.AddScoped<LuxorLMS.Academic.Domain.Interfaces.ICourseOfferingRepository, LuxorLMS.Academic.Infrastructure.Repositories.CourseOfferingRepository>();
builder.Services.AddScoped<LuxorLMS.Academic.Domain.Interfaces.ISemesterRepository, LuxorLMS.Academic.Infrastructure.Repositories.SemesterRepository>();
builder.Services.AddScoped<LuxorLMS.Academic.Domain.Interfaces.IDepartmentRepository, LuxorLMS.Academic.Infrastructure.Repositories.DepartmentRepository>();
builder.Services.AddScoped<LuxorLMS.Academic.Domain.Interfaces.IStudentRepository, LuxorLMS.Academic.Infrastructure.Repositories.StudentRepository>();
builder.Services.AddScoped<LuxorLMS.Academic.Domain.Interfaces.IUnitOfWork, LuxorLMS.Academic.Infrastructure.Repositories.UnitOfWork>();

// Application services (Grading)
builder.Services.AddScoped<IGradeScaleService, LuxorLMS.Grading.Application.Services.GradeScaleService>();
builder.Services.AddScoped<IGradeSchemaService, LuxorLMS.Grading.Application.Services.GradeSchemaService>();
builder.Services.AddScoped<IGpaService, LuxorLMS.Grading.Application.Services.GpaService>();
builder.Services.AddScoped<IStudentGradeService, LuxorLMS.Grading.Application.Services.StudentGradeService>();
builder.Services.AddScoped<IGradeAppealService, LuxorLMS.Grading.Application.Services.GradeAppealService>();
builder.Services.AddScoped<IAcademicGradingGateway, AcademicGradingGateway>();

builder.Services.AddValidatorsFromAssembly(typeof(LuxorLMS.Grading.Application.DTOs.GradeCategoryDto).Assembly);

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
    var gradingDb = scope.ServiceProvider.GetRequiredService<LuxorLMSGradingDbContext>();
    gradingDb.Database.Migrate();
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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "LuxorLMS Grading API v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
