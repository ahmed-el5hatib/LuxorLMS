# Milestone 4: Grading & Transcripts — Execution Plan

> Extracted from the master plan: `## 1. Project Analysis LuxorLMS.txt` → "Milestone 4: Grading & Transcripts (Weeks 10-11)".

## Overview
Fourth milestone in the LuxorLMS modular monolith. Introduces the **Grading** module: configurable weighted grade schemas per Course Offering, a **GPA engine** (Semester GPA + Cumulative GPA on the standard 4.0 scale), a **grade publishing workflow** (Draft → Department Head approval → Academic Affairs publish), and a **7-day student appeal window** after publishing. Builds on Milestone 1 (Identity/RBAC), Milestone 2 (Academic hierarchy: Courses, Credit Hours, Students, CourseOfferings, Sections) and Milestone 3 (Registration & Course Offerings/Sections).

## Objectives (from master plan)
- **M4.1** Grade Management: `GradeCategories`, `GradeComponents`, weighted schemas.
- **M4.2** GPA Engine: Semester GPA and CGPA calculation, standard 4.0 scaling.
- **M4.3** Grade Publishing: Draft → Department Head approval → Academic Affairs publish workflow.
- **M4.4** Appeals: 7-day student appeal window after publishing.

## Dependencies
- Milestone 1 (Identity, JWT, RBAC) — `IAuthorizationService` reused for permission checks; `UserRole` for role gating (DepartmentManager / AcademicAffairs).
- Milestone 2 (Academic) — `Course` (`CreditHours`), `CourseOffering`, `Section`, `SectionMember`, `Student` (`Gpa` = CGPA store) read/updated via Academic.Application/Infrastructure.
- Milestone 3 (Registration/Course Offerings) — offerings & sections are the grading unit.
- .NET 8 SDK, PostgreSQL, `LuxorLMS.Kernel` (`Result<T>`/`Error`).

## Integration Approach
- New projects `LuxorLMS.Grading.{Domain,Application,Infrastructure,Api}` added to `LuxorLMS.slnx`.
- References mirror Registration: Domain→Kernel; Application→Domain,Kernel,Academic.Application,Identity.Application; Infrastructure→all + Academic.Infrastructure,Academic.Application; Api→all + Academic.Application,Academic.Infrastructure,Identity.Application.
- Grading has its own DbContext (`LuxorLMSGradingDbContext`, DB `luxorlms_grading`).
- Grading.Api also registers Academic's `LuxorLMSAcademicDbContext` (connection `AcademicConnection`) to read catalog/offering data and update `Student.Gpa` (same pattern as Registration.Api → Academic).
- Cross-module reads use an anti-corruption adapter (`IAcademicGradingGateway` implemented by `AcademicGradingGateway` over Academic services/repositories) — same pattern as Registration's `CourseCatalogAdapter`.

## Domain Model (new entities in `LuxorLMS.Grading.Domain`)
- `GradeCategory` (Id, CourseOfferingId, Name, Weight[0..1], DisplayOrder, audit).
- `GradeComponent` (Id, GradeCategoryId, Title, MaxPoints, audit).
- `StudentGrade` (Id, CourseOfferingId, StudentId, CourseId, SemesterId, CreditHours, RawScore[0..100], GradeLetter, GradePoints, PublishStatus, DeptHeadApprovedBy/At, PublishedBy/At, AppealDeadline, audit).
- `GradeAppeal` (Id, StudentGradeId, StudentId, Reason, Status, Resolution, ResolvedBy/At, audit).
- Enums: `GradePublishStatus` (Draft, PendingDeptHead, DeptHeadApproved, Published, Rejected), `AppealStatus` (Open, Approved, Rejected).

### GPA Engine (M4.2)
- Standard 4.0 letter scale: A=4.0, A-=3.7, B+=3.3, B=3.0, B-=2.7, C+=2.3, C=2.0, C-=1.7, D+=1.3, D=1.0, F=0.0. 'W' excluded from GPA.
- Semester GPA = Σ(gradePoints × creditHours) / Σ(creditHours) over published grades in a semester.
- CGPA = same over all published grades; written back to Academic `Student.Gpa`.

## Tasks Breakdown

### Task 4.1: Grading Domain Layer (M4.1/M4.3/M4.4)
- Entities: `GradeCategory`, `GradeComponent`, `StudentGrade`, `GradeAppeal`.
- Enums: `GradePublishStatus`, `AppealStatus`.
- Interfaces: `IGradeCategoryRepository`, `IGradeComponentRepository`, `IStudentGradeRepository`, `IGradeAppealRepository`, `IUnitOfWork`.

### Task 4.2: Grading Infrastructure
- `LuxorLMSGradingDbContext`, repositories, `UnitOfWork`, `IDesignTimeDbContextFactory`.
- `AcademicGradingGateway` (adapter over Academic to read course credit-hours/offerings and update `Student.Gpa`).
- Migration `InitialGradingSchema`.

### Task 4.3: Grading Application (M4.1/M4.2)
- DTOs + FluentValidation validators.
- `GradeSchemaService` (categories/components CRUD + weight-sum validation).
- `StudentGradeService` (enter/update raw scores → letter/points, submit for approval, dept-head approve, academic-affairs publish).
- `GpaService` (semester GPA, CGPA, write-back), `GradeAppealService` (7-day window enforcement + resolution).
- `IGradeScaleService` (4.0 scale mapping). `IAcademicGradingGateway` interface.

### Task 4.4: Grading API + RBAC (M4.1/M4.3/M4.4)
- Controllers: `GradeSchemaController`, `StudentGradesController`, `GpaController`, `GradeAppealsController`.
- `GradingPermissions` + `RequirePermissionFilter` (reuse Identity `IAuthorizationService`).
- `Program.cs`: JWT, Swagger, versioning, ProblemDetails, both DbContexts.

## Acceptance Criteria
1. `dotnet build LuxorLMS.slnx` succeeds, 0 errors.
2. Migration `InitialGradingSchema` generated; tables created with correct relationships/indexes.
3. RBAC: unauthorized → 401/403; students limited to own grades/appeals.
4. Business rules enforced: category weights sum to 1.0; publish workflow order (Draft → DeptHead → Published); appeals rejected after 7-day window; 'W' excluded from GPA.
5. GPA engine computes Semester GPA + CGPA on 4.0 scale and writes CGPA back to `Student.Gpa`.
6. Swagger UI functional with JWT-protected routes.

## Estimated Timeline
- 4.1 Domain: 2h
- 4.2 Infrastructure: 3h
- 4.3 Application (incl. GPA engine): 5h
- 4.4 API + RBAC: 3h
- Build/validate/migration: 2h
- Total: ~15h (≈2 working days)
