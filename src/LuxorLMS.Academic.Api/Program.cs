using LuxorLMS.Academic.Application.Interfaces;
using LuxorLMS.Academic.Infrastructure.Persistence;
using LuxorLMS.Academic.Infrastructure.Repositories;
using LuxorLMS.Identity.Application.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Controllers + camelCase JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

// API versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "LuxorLMS Academic API",
        Version = "v1",
        Description = "Academic Lifecycle & Hierarchy Module",
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

// Database (separate Academic schema/database)
builder.Services.AddDbContext<LuxorLMSAcademicDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity module (for RBAC authorization service)
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

// Repositories + UnitOfWork
builder.Services.AddScoped<LuxorLMS.Academic.Domain.Interfaces.IFacultyRepository, FacultyRepository>();
builder.Services.AddScoped<LuxorLMS.Academic.Domain.Interfaces.IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<LuxorLMS.Academic.Domain.Interfaces.IProgramRepository, ProgramRepository>();
builder.Services.AddScoped<LuxorLMS.Academic.Domain.Interfaces.IStudyPlanRepository, StudyPlanRepository>();
builder.Services.AddScoped<LuxorLMS.Academic.Domain.Interfaces.ICourseRepository, CourseRepository>();
builder.Services.AddScoped<LuxorLMS.Academic.Domain.Interfaces.IAcademicYearRepository, AcademicYearRepository>();
builder.Services.AddScoped<LuxorLMS.Academic.Domain.Interfaces.ISemesterRepository, SemesterRepository>();
builder.Services.AddScoped<LuxorLMS.Academic.Domain.Interfaces.ICourseOfferingRepository, CourseOfferingRepository>();
builder.Services.AddScoped<LuxorLMS.Academic.Domain.Interfaces.ISectionRepository, SectionRepository>();
builder.Services.AddScoped<LuxorLMS.Academic.Domain.Interfaces.ISectionMemberRepository, SectionMemberRepository>();
builder.Services.AddScoped<LuxorLMS.Academic.Domain.Interfaces.IStudentRepository, StudentRepository>();
builder.Services.AddScoped<LuxorLMS.Academic.Domain.Interfaces.IUnitOfWork, UnitOfWork>();

// Application services
builder.Services.AddScoped<IFacultyService, LuxorLMS.Academic.Application.Services.FacultyService>();
builder.Services.AddScoped<IDepartmentService, LuxorLMS.Academic.Application.Services.DepartmentService>();
builder.Services.AddScoped<IProgramService, LuxorLMS.Academic.Application.Services.ProgramService>();
builder.Services.AddScoped<IStudyPlanService, LuxorLMS.Academic.Application.Services.StudyPlanService>();
builder.Services.AddScoped<ICourseService, LuxorLMS.Academic.Application.Services.CourseService>();
builder.Services.AddScoped<ICourseOfferingService, LuxorLMS.Academic.Application.Services.CourseOfferingService>();
builder.Services.AddScoped<ISectionService, LuxorLMS.Academic.Application.Services.SectionService>();
builder.Services.AddScoped<IAcademicYearService, LuxorLMS.Academic.Application.Services.AcademicYearService>();
builder.Services.AddScoped<ISemesterService, LuxorLMS.Academic.Application.Services.SemesterService>();
builder.Services.AddScoped<IStudentService, LuxorLMS.Academic.Application.Services.StudentService>();

// Identity Authorization (RBAC)
builder.Services.AddScoped<IAuthorizationService, LuxorLMS.Identity.Application.Services.AuthorizationService>();

// FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(LuxorLMS.Academic.Application.DTOs.FacultyDto).Assembly);

// Authentication (validate tokens issued by Identity module)
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

// Ensure database schema is migrated
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<LuxorLMSAcademicDbContext>();
    dbContext.Database.Migrate();
}

// Global exception handling via ProblemDetails
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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "LuxorLMS Academic API v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
