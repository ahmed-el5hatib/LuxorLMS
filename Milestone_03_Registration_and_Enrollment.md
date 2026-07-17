# Milestone 3: Registration & Course Management — Execution Plan

> Extracted from the master plan: `## 1. Project Analysis LuxorLMS.txt` → "Milestone 3: Registration & Course Management (Weeks 7-9)".

## Overview
Third milestone in the LuxorLMS modular monolith. Combines the **Registration** module (enrollment windows, prerequisite engine, GPA-based credit-hour validation, add/drop/withdraw, approval workflow) with the start of **Course Management** (Course Offerings & Sections: Lecture/Lab/Tutorial partitioning, capacity, staff assignment). Builds on Milestone 1 (Identity/RBAC) and Milestone 2 (Academic hierarchy).

## Objectives (from master plan)
- **M3.1** Registration Module: Registration windows, prerequisite engine, credit hour validation (GPA-based).
- **M3.2** Add/Drop/Withdraw workflows with 'W' grade recording.
- **M3.3** Course Offerings & Sections: Lecture/Lab/Tutorial partitioning, capacity management, staff assignment.
- **M3.4** Concurrency: RedLock seat booking during registration.
- **M3.5** Enrollment Approval: Academic Affairs override dashboard.

## Dependencies
- Milestone 1 (Identity, JWT, RBAC) — `IAuthorizationService` reused.
- Milestone 2 (Academic) — `Course`, `StudyPlan`, `CoursePrerequisite`, `Semester`, `Program` read via Academic.Application.
- .NET 8 SDK, PostgreSQL, `LuxorLMS.Kernel` (`Result<T>`/`Error`).
- Redis + RedLock.net (for M3.4 distributed seat booking).

## Integration Approach
- Registration projects (`LuxorLMS.Registration.{Domain,Application,Infrastructure,Api}`) added to `LuxorLMS.slnx`.
- References mirror M2: Domain→Kernel; Application→Domain,Kernel,Academic.Application,Identity.Application; Infrastructure→all + Academic.Infrastructure; Api→all + Academic.Application,Identity.Application.
- Registration has its own DbContext (`LuxorLMSRegistrationDbContext`, DB `luxorlms_registration`).
- Registration.Api also registers Academic's `LuxorLMSAcademicDbContext` (connection `luxorlms_academic`) to read catalog/prerequisite data (same pattern as Academic.Api→Identity.Application).
- **M3.3 Course Offerings & Sections** extend the Academic module (new entities in `LuxorLMS.Academic.Domain`: `CourseOffering`, `Section`, `SectionMember`) since they belong to the Academic lifecycle per the spec domain model and reuse Academic's DB.
- **M3.4 RedLock** uses `RedLock.net` (already a planned dependency) with a distributed lock around seat/capacity booking during registration.

## Tasks Breakdown

### Task 3.1: Registration Domain Layer (M3.1/M3.2/M3.5)
- Entities: `RegistrationPeriod`, `StudentProgramEnrollment`, `CourseEnrollment`.
- Enums: `EnrollmentStatus`, `StudentProgramStatus`, `EnrollmentType`.
- Interfaces: `IRegistrationPeriodRepository`, `IStudentProgramEnrollmentRepository`, `ICourseEnrollmentRepository`, `IUnitOfWork`.

### Task 3.2: Registration Infrastructure
- `LuxorLMSRegistrationDbContext`, repositories, `UnitOfWork`, `IDesignTimeDbContextFactory`.
- Migration `InitialRegistrationSchema`.

### Task 3.3: Registration Application
- DTOs + FluentValidation validators.
- `RegistrationPeriodService`, `StudentProgramEnrollmentService`, `CourseEnrollmentService` (prerequisite engine, GPA credit-hour limits, add/drop/withdraw with 'W', approval).
- `ICourseCatalogService` adapter over Academic's `ICourseService`.

### Task 3.4: Registration API + RBAC (M3.1/M3.2/M3.5)
- Controllers: `RegistrationPeriodsController`, `StudentProgramEnrollmentsController`, `CourseEnrollmentsController`.
- `RegistrationPermissions` + `RequirePermissionFilter` (reuse Identity `IAuthorizationService`).
- `Program.cs`: JWT, Swagger, versioning, ProblemDetails, both DbContexts.

### Task 3.5: Course Offerings & Sections (M3.3)
- Academic.Domain entities: `CourseOffering` (CourseId, SemesterId, PrimaryTeacherId, Capacity, RegistrationStart/End, Status), `Section` (CourseOfferingId, SectionType enum Lecture/Lab/Tutorial, AssignedStaffId, Capacity), `SectionMember` (SectionId, StudentId).
- Academic.Infrastructure: mappings + repositories.
- Academic.Application: DTOs, validators, services (offering/section CRUD, capacity checks).
- Academic.Api: `CourseOfferingsController`, `SectionsController` with RBAC.

### Task 3.6: RedLock Concurrency (M3.4)
- Add `RedLock.net` to Registration.Infrastructure.
- `ISeatBookingLock` / `RedLockSeatBookingService` acquiring a distributed lock around capacity booking during `CourseEnrollmentService.RegisterAsync`.

## Acceptance Criteria
1. `dotnet build LuxorLMS.slnx` succeeds, 0 errors.
2. Migrations apply; tables created with correct relationships.
3. RBAC: unauthorized → 401/403; students limited to own enrollments.
4. Business rules enforced: period windows, prerequisites, GPA credit-hour limits, withdraw → 'W', offering capacity.
5. RedLock prevents double-booking under concurrency.
6. Swagger UI functional with JWT-protected routes.

## Estimated Timeline
- 3.1 Domain: 2h
- 3.2 Infrastructure: 2h
- 3.3 Application: 4h
- 3.4 API + RBAC: 2h
- 3.5 Course Offerings & Sections: 4h
- 3.6 RedLock concurrency: 2h
- Build/validate: 1h
- Total: ~17h (≈2.5 working days)
