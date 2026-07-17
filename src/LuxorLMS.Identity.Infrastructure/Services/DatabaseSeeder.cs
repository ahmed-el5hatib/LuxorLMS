using LuxorLMS.Identity.Domain.Entities;
using LuxorLMS.Identity.Domain.Enums;
using LuxorLMS.Identity.Domain.Interfaces;
using LuxorLMS.Identity.Infrastructure.Persistence;
using LuxorLMS.Identity.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Identity.Infrastructure.Services;

public class DatabaseSeeder
{
    private readonly LuxorLMSIdentityDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public DatabaseSeeder(LuxorLMSIdentityDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (!_context.Users.Any())
        {
            var adminUser = new User
            {
                Id = Guid.NewGuid(),
                Username = "admin",
                Email = "admin@luxorlms.com",
                PasswordHash = _passwordHasher.Hash("Admin@123456"),
                PasswordSalt = string.Empty,
                FirstNameAr = "مدير",
                LastNameAr = "النظام",
                FirstNameEn = "System",
                LastNameEn = "Administrator",
                Role = UserRole.SystemAdmin,
                MfaEnabled = false,
                IsLocked = false,
                FailedLoginAttempts = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = Guid.Empty
            };

            await _context.Users.AddAsync(adminUser, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
