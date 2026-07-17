# Milestone 1: Identity & Core Security — Execution Plan

## Overview
This milestone establishes the foundational security layer for LuxorLMS. It implements the Identity Module with Claims-Based Access Control (CBAC), secure authentication flows, multi-factor authentication, password policies, rate limiting, and immutable audit logging. All subsequent modules depend on this milestone.

---

## Objectives
1. Establish a secure, production-ready authentication and authorization foundation.
2. Implement 8 distinct system roles with fine-grained permission enforcement.
3. Provide resilient session management with refresh tokens and device tracking.
4. Enforce organizational security standards (MFA, lockout, OWASP protections).
5. Create an immutable audit trail for all mutating operations.

---

## Tasks Breakdown

### Task 1.1: Solution Scaffolding & Shared Kernel
**Objective:** Create the Visual Studio solution structure following Clean Architecture principles with a Modular Monolith layout.

**Actions:**
- Create solution `LuxorLMS.sln`
- Create shared `LuxorLMS.Kernel` project for cross-cutting concerns (results, exceptions, constants)
- Create `LuxorLMS.Identity` module project with Clean Architecture layers:
  - `LuxorLMS.Identity.Domain` (entities, enums, interfaces)
  - `LuxorLMS.Identity.Application` (use cases, validators, DTOs)
  - `LuxorLMS.Identity.Infrastructure` (EF Core, repositories, external services)
  - `LuxorLMS.Identity.Api` (Minimal API endpoints, middleware)
- Configure solution-level `Directory.Build.props` for consistent versioning and analyzers.
- Add global `Usings.cs` files per project.

**Deliverables:**
- `LuxorLMS.sln` with 5 projects.
- Build passes with zero warnings.

---

### Task 1.2: Domain Layer — Identity Entities
**Objective:** Define the core domain entities, enums, and interfaces for the Identity module.

**Actions:**
- Create `User` entity with fields:
  - `Id` (Guid), `Username`, `Email`, `PasswordHash`, `PasswordSalt`
  - `FirstNameAr`, `LastNameAr`, `FirstNameEn`, `LastNameEn`
  - `Role` (enum: SystemAdmin, AcademicAffairs, QAOfficer, FacultyManager, DepartmentManager, Doctor, TeachingAssistant, Student)
  - `MfaEnabled`, `MfaSecret`
  - `IsLocked`, `FailedLoginAttempts`, `LockoutEnd`
  - `CreatedAt`, `UpdatedAt`, `LastLogin`, `CreatedBy`
- Create `RefreshToken` entity:
  - `Id`, `UserId`, `Token` (hashed), `ExpiresAt`, `CreatedAt`, `RevokedAt`, `ReplacedByToken`, `DeviceInfo`, `IpAddress`
- Create `DeviceSession` entity:
  - `Id`, `UserId`, `DeviceName`, `UserAgent`, `IpAddress`, `LastActiveAt`, `CreatedAt`
- Create `AuditLog` entity:
  - `Id`, `UserId`, `Action`, `EntityName`, `EntityId`, `OldValues` (JSON), `NewValues` (JSON), `Timestamp`, `IpAddress`, `UserAgent`
- Define enums: `UserRole`, `AttendanceStatus` (for future use)
- Define interfaces: `IUserRepository`, `IRefreshTokenRepository`, `IAuditLogRepository`, `IUnitOfWork`
- Configure EF Core entity relationships and value conversions.

**Deliverables:**
- Complete Domain layer with all entities, enums, and repository interfaces.
- Domain layer has zero dependencies on Infrastructure or API layers.

---

### Task 1.3: Infrastructure Layer — Persistence & Security
**Objective:** Implement EF Core DbContext, repositories, and security utilities.

**Actions:**
- Create `LuxorLMSIdentityDbContext` inheriting from `DbContext`.
- Configure all entity mappings via Fluent API (no data annotations on domain entities).
- Implement `UserRepository`, `RefreshTokenRepository`, `AuditLogRepository`, `UnitOfWork`.
- Implement password hashing using `BCrypt.Net-Next` (or `Microsoft.AspNetCore.Identity`-compatible PBKDF2).
- Implement `IJwtTokenService` for generating and validating JWT access tokens.
- Implement `ITokenCleanupService` for periodic expired token purging.
- Configure PostgreSQL connection string injection.
- Configure global query filters for soft-delete patterns if applicable.

**Deliverables:**
- Fully functional DbContext with migrations infrastructure.
- Repository implementations with async methods.
- JWT token generation and validation services.
- Database migration scripts for Identity tables.

---

### Task 1.4: Authentication — JWT, Refresh Tokens & Device Sessions
**Objective:** Implement secure authentication with access/refresh token rotation and device session tracking.

**Actions:**
- Implement `AuthService` use case with methods:
  - `RegisterAsync` (with email/username uniqueness checks)
  - `LoginAsync` (validate credentials, check lockout, generate tokens)
  - `RefreshTokenAsync` (rotate refresh tokens, revoke old token)
  - `RevokeTokenAsync` (logout from specific device)
  - `RevokeAllTokensAsync` (logout from all devices)
- Configure JWT settings:
  - Access token lifetime: 15 minutes
  - Refresh token lifetime: 7 days
  - Signing algorithm: RS256 (or HS256 with secure key)
  - Claims: `sub`, `email`, `role`, `name`, `jti`
- Implement device session tracking on login.
- Create `AuthController` (Minimal API) with endpoints:
  - `POST /api/v1/auth/register`
  - `POST /api/v1/auth/login`
  - `POST /api/v1/auth/refresh`
  - `POST /api/v1/auth/revoke`
  - `POST /api/v1/auth/revoke-all`
  - `GET /api/v1/auth/me` (current user profile)
  - `GET /api/v1/auth/sessions` (list active devices)

**Deliverables:**
- Fully functional authentication flow with token rotation.
- Device session management endpoints.
- Swagger/OpenAPI documentation with JWT bearer auth configured.

---

### Task 1.5: Multi-Factor Authentication (MFA)
**Objective:** Implement TOTP-based MFA for privileged roles.

**Actions:**
- Implement `MfaService` with methods:
  - `EnableMfaAsync` (generate TOTP secret, return QR code URI)
  - `VerifyMfaAsync` (validate TOTP code during setup)
  - `DisableMfaAsync` (require password re-confirmation)
  - `ValidateMfaCodeAsync` (validate during login)
- Integrate MFA into login flow:
  - If user has MFA enabled, return `mfaRequired: true` and `mfaToken` (short-lived, single-use)
  - Client submits `mfaToken` + TOTP code to `/api/v1/auth/mfa/validate`
- Enforce MFA requirement for roles: SystemAdmin, AcademicAffairs, Doctor.
- Store MFA recovery codes (optional but recommended).
- Use `OtpNet` library for TOTP generation/validation.

**Deliverables:**
- MFA enrollment and validation endpoints.
- Login flow with MFA challenge.
- Recovery code generation and validation (optional).

---

### Task 1.6: Authorization — RBAC & Policy Enforcement
**Objective:** Implement Claims-Based Access Control (CBAC) with 8 roles and permission policies.

**Actions:**
- Define permission claims as constants or enum flags.
- Create `IAuthorizationService` with methods:
  - `AuthorizeAsync(userId, requiredPermissions)`
  - `HasRoleAsync(userId, role)`
- Implement custom `AuthorizationHandler` for permission-based policies.
- Create authorization middleware or endpoint filters for:
  - `RequirePermission` attribute/filter
  - `RequireRole` attribute/filter
- Define role-permission matrix:
  - SystemAdmin: All permissions
  - AcademicAffairs: Registration management, grade publishing, calendar config
  - QAOfficer: Read-only access to reports and analytics
  - FacultyManager: Faculty-level settings, quotas, staff allocation
  - DepartmentManager: Program/study plan management, grade review
  - Doctor: Course content, quizzes, assignments, attendance QR, grading
  - TeachingAssistant: Attendance marking, restricted grading, forum moderation
  - Student: Course registration, material download, submissions, grade view
- Implement permission caching in Redis for performance.

**Deliverables:**
- Complete RBAC enforcement infrastructure.
- Permission constants and role-permission matrix.
- Authorization middleware/filters applied to API endpoints.

---

### Task 1.7: Security Hardening — Password Policies, Lockout & Rate Limiting
**Objective:** Enforce organizational security standards.

**Actions:**
- Implement password policy validator:
  - Minimum 10 characters
  - Requires uppercase, lowercase, symbol, number
  - Checks against common passwords (optional)
- Implement account lockout policy:
  - 5 failed login attempts → 15-minute lockout
  - Reset on successful login
- Implement rate limiting middleware:
  - IP-based token bucket for `/api/v1/auth/login` (5 attempts per minute)
  - IP-based for `/api/v1/auth/register` (3 per hour)
  - User-based for refresh token endpoint (10 per minute)
  - Use Redis-backed distributed rate limiter.
- Configure security headers middleware:
  - HSTS, X-Content-Type-Options, X-Frame-Options, CSP
- Configure CORS with strict origin allowlist.
- Implement OWASP protections:
  - BOLA/IDOR: Ensure all repository methods validate `UserId` ownership or admin role.
  - SQL Injection: Use parameterized queries exclusively (EF Core handles this).
  - XSS: Enforce content-type validation, sanitize inputs at API boundaries.
  - CSRF: Enable Antiforgery for state-changing requests if using cookies.

**Deliverables:**
- Password policy enforcement.
- Account lockout mechanism.
- Distributed rate limiting with Redis.
- Security headers and CORS configuration.

---

### Task 1.8: Audit Logging — Immutable Action Tracking
**Objective:** Create an immutable audit trail for all mutating operations.

**Actions:**
- Create `IAuditLogger` service with method `LogAsync(action, entityName, entityId, oldValues, newValues)`.
- Implement `AuditLogMiddleware` or use MediatR pipeline behavior to automatically log:
  - Every HTTP request that mutates state (POST, PUT, PATCH, DELETE)
  - Capture: UserId, Action, Endpoint, Old Values (if applicable), New Values (if applicable), Timestamp, IP, UserAgent
- Implement database-level immutability:
  - Database trigger to prevent UPDATE/DELETE on `AuditLogs` table.
  - Application-level guard: throw exception if audit log modification is attempted.
- Create `AuditLogController` with endpoints:
  - `GET /api/v1/audit-logs` (SystemAdmin only, with pagination)
  - `GET /api/v1/audit-logs/{id}`
  - `GET /api/v1/audit-logs/user/{userId}`
  - `GET /api/v1/audit-logs/entity/{entityName}/{entityId}`

**Deliverables:**
- Immutable AuditLogs table with database trigger.
- Automatic audit logging via middleware/pipeline.
- Audit log query endpoints with pagination.

---

### Task 1.9: API Configuration & Cross-Cutting Concerns
**Objective:** Wire up the API layer with all enterprise configurations.

**Actions:**
- Configure Program.cs / Startup.cs:
  - Add controllers / minimal APIs
  - Add EF Core with PostgreSQL
  - Add Redis with RedLock (for future use)
  - Add JWT authentication and authorization
  - Add rate limiting middleware
  - Add security headers
  - Add Swagger/OpenAPI with JWT auth support
  - Add CORS
  - Add response compression (Gzip/Brotli)
  - Add health checks
- Configure FluentValidation for all request DTOs.
- Implement global exception handling with `ProblemDetails` (RFC 7807).
- Implement structured logging with Serilog (or built-in logging).
- Add API versioning (`/api/v1/...`).
- Add idempotency middleware for registration/login endpoints.

**Deliverables:**
- Fully configured API pipeline.
- Swagger UI with JWT support.
- Global exception handling and validation.

---

## Expected Deliverables Summary

| Deliverable | Description |
|:---|:---|
| Solution Structure | `LuxorLMS.sln` with Kernel + Identity module (4 projects: Domain, Application, Infrastructure, Api) |
| Database Schema | EF Core migrations for Users, RefreshTokens, DeviceSessions, AuditLogs |
| Authentication API | Register, Login, Refresh, Revoke, MFA endpoints |
| RBAC Engine | 8-role permission matrix with policy enforcement |
| Security Middleware | Rate limiting, lockout, password policy, security headers |
| Audit Logging | Immutable audit trail with automatic interception |
| Documentation | Swagger/OpenAPI, README for Identity module |

---

## Acceptance Criteria
1. **Build:** `dotnet build` succeeds with zero warnings across all projects.
2. **Tests:** Unit tests cover `AuthService`, `MfaService`, `PasswordPolicy`, `RateLimiter`, `AuditLogger`.
3. **Security:** OWASP Top 10 mitigations verified (no SQLi, XSS, CSRF, BOLA in auth flows).
4. **Performance:** JWT validation < 5ms, token refresh < 50ms, rate limiter < 2ms overhead.
5. **Documentation:** Swagger UI functional with all endpoints documented and JWT-protected routes requiring auth.

---

## Execution Order
1. Scaffold solution and project structure (Task 1.1)
2. Build Domain layer entities and interfaces (Task 1.2)
3. Implement Infrastructure layer (Task 1.3)
4. Implement Authentication (Task 1.4)
5. Implement MFA (Task 1.5)
6. Implement Authorization (Task 1.6)
7. Implement Security Hardening (Task 1.7)
8. Implement Audit Logging (Task 1.8)
9. Configure API and cross-cutting concerns (Task 1.9)

---

## Estimated Timeline
- **Task 1.1:** 2 hours
- **Task 1.2:** 3 hours
- **Task 1.3:** 4 hours
- **Task 1.4:** 4 hours
- **Task 1.5:** 3 hours
- **Task 1.6:** 4 hours
- **Task 1.7:** 3 hours
- **Task 1.8:** 3 hours
- **Task 1.9:** 2 hours
- **Total:** ~28 hours (3-4 working days)

---

## Dependencies
- .NET 8 SDK
- PostgreSQL (local or Docker)
- Redis (local or Docker)
- `OtpNet` NuGet package
- `BCrypt.Net-Next` NuGet package
- `StackExchange.Redis` NuGet package
- `RedLock.net` NuGet package
- `FluentValidation` NuGet package
- `Microsoft.AspNetCore.Authentication.JwtBearer` NuGet package
