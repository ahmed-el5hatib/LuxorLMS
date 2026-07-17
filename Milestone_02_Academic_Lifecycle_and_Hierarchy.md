# Milestone 2: Academic Lifecycle & Hierarchy — Execution Plan

## Overview
This milestone implements the Academic Module, establishing the organizational hierarchy required to support the full academic lifecycle of Luxor University. It builds on the Identity and security foundation from Milestone 1 and introduces the core domain entities that all subsequent academic modules (Registration, Course Management, Grading, Attendance, etc.) will depend on.

---

## Objectives
1. Model the academic organizational hierarchy: Faculty → Department → Program → StudyPlan → Course.
2. Implement temporal entities: AcademicYear and Semester with strict date validation.
3. Support curriculum definition with course prerequisites and study-plan mappings.
4. Enforce RBAC permissions for academic affairs, department managers, and faculty managers.
5. Provide a clean, repository-pattern API for querying and managing academic structures.

---

## Tasks Breakdown

### Task 2.1: Solution Scaffolding — Academic Module Projects
**Objective:** Create the Academic module project structure following Clean Architecture and Modular Monolith boundaries.

**Actions:**
- Create `LuxorLMS.Academic.Domain` (entities, enums, interfaces)
- Create `LuxorLMS.Academic.Application` (DTOs, service interfaces, business logic)
- Create `LuxorLMS.Academic.Infrastructure` (EF Core, repositories, external integrations)
- Create `LuxorLMS.Academic.Api` (Minimal API controllers, DI wiring)
- Add all projects to `LuxorLMS.slnx`
- Configure project references:
  - Domain → Kernel
  - Application → Domain, Kernel
  - Infrastructure → Application, Domain, Kernel
  - Api → Infrastructure, Application, Domain, Kernel

**Deliverables:**
- 4 new projects in the solution.
- `dotnet build` succeeds.

---

### Task 2.2: Domain Layer — Academic Entities
**Objective:** Define the core domain entities, enums, value objects, and repository interfaces for the Academic module.

**Actions:**
- Create `Faculty` entity:
  - `Id` (Guid), `NameAr`, `NameEn`, `Code` (unique), `IsActive`, `CreatedAt`, `CreatedBy`
- Create `Department` entity:
  - `Id` (Guid), `FacultyId` (FK), `NameAr`, `NameEn`, `Code` (unique), `HeadId` (FK to Users), `IsActive`, `CreatedAt`, `CreatedBy`
- Create `Program` entity:
  - `Id` (Guid), `DepartmentId` (FK), `NameAr`, `NameEn`, `Code` (unique), `DegreeLevel` (enum: Bachelor, Master, PhD), `TotalCreditsRequired`, `IsActive`, `CreatedAt`, `CreatedBy`
- Create `StudyPlan` entity:
  - `Id` (Guid), `ProgramId` (FK), `VersionCode` (e.g., "PLAN-2026"), `EffectiveFrom` (DateTime), `EffectiveTo` (DateTime, nullable), `MinimumCredits`, `IsActive`, `CreatedAt`, `CreatedBy`
- Create `StudyPlanCourse` entity (junction):
  - `Id` (Guid), `StudyPlanId` (FK), `CourseId` (FK), `SemesterNumber` (int, e.g., 1-8), `IsRequired` (bool), `CreatedAt`
- Create `AcademicYear` entity:
  - `Id` (Guid), `Label` (e.g., "2026/2027"), `StartDate`, `EndDate`, `IsActive`, `CreatedAt`, `CreatedBy`
- Create `Semester` entity:
  - `Id` (Guid), `AcademicYearId` (FK), `Name` (e.g., "First Semester"), `Code` (e.g., "FALL"), `StartDate`, `EndDate`, `RegistrationStart`, `RegistrationEnd`, `IsActive`, `CreatedAt`, `CreatedBy`
- Create `Course` entity:
  - `Id` (Guid), `DepartmentId` (FK), `CourseCode` (unique), `NameAr`, `NameEn`, `CreditHours`, `Description`, `IsActive`, `CreatedAt`, `CreatedBy`
- Create `CoursePrerequisite` entity:
  - `Id` (Guid), `CourseId` (FK), `PrerequisiteCourseId` (FK), `IsActive`, `CreatedAt`
- Create enums: `DegreeLevel`
- Create interfaces: `IFacultyRepository`, `IDepartmentRepository`, `IProgramRepository`, `IStudyPlanRepository`, `ICourseRepository`, `IAcademicYearRepository`, `ISemesterRepository`, `IUnitOfWork`
- Configure EF Core Fluent API mappings, unique indexes, relationships.

**Deliverables:**
- Complete Domain layer with all entities, enums, and repository interfaces.
- Zero dependencies on Infrastructure or API.

---

### Task 2.3: Infrastructure Layer — Persistence & Repositories
**Objective:** Implement EF Core DbContext, repositories, and database migrations for the Academic module.

**Actions:**
- Create `LuxorLMSAcademicDbContext` inheriting from `DbContext`.
- Configure all entity mappings via Fluent API:
  - Unique indexes on `Code` fields (Faculty, Department, Program, Course)
  - Foreign keys with proper delete behaviors
  - Composite unique index on `StudyPlanCourse` (StudyPlanId + CourseId)
- Implement repositories:
  - `FacultyRepository`
  - `DepartmentRepository`
  - `ProgramRepository`
  - `StudyPlanRepository`
  - `CourseRepository`
  - `AcademicYearRepository`
  - `SemesterRepository`
- Implement `UnitOfWork` pattern.
- Add database migration infrastructure.

**Deliverables:**
- Fully functional DbContext with all Academic entities mapped.
- Repository implementations with async CRUD methods.
- EF Core migrations.

---

### Task 2.4: Application Layer — DTOs & Services
**Objective:** Implement DTOs, service interfaces, and business logic for the Academic module.

**Actions:**
- Create DTOs:
  - `FacultyDto`, `CreateFacultyRequest`, `UpdateFacultyRequest`
  - `DepartmentDto`, `CreateDepartmentRequest`, `UpdateDepartmentRequest`
  - `ProgramDto`, `CreateProgramRequest`, `UpdateProgramRequest`
  - `StudyPlanDto`, `CreateStudyPlanRequest`, `UpdateStudyPlanRequest`
  - `CourseDto`, `CreateCourseRequest`, `UpdateCourseRequest`
  - `AcademicYearDto`, `CreateAcademicYearRequest`
  - `SemesterDto`, `CreateSemesterRequest`, `UpdateSemesterRequest`
- Create service interfaces:
  - `IFacultyService`
  - `IDepartmentService`
  - `IProgramService`
  - `IStudyPlanService`
  - `ICourseService`
  - `IAcademicYearService`
  - `ISemesterService`
- Implement services with validation logic:
  - Faculty/Department/Program CRUD with uniqueness checks
  - StudyPlan management with effective date validation
  - Course CRUD with prerequisite validation
  - Semester date validation (StartDate < EndDate, Registration within semester bounds)
  - AcademicYear date validation

**Deliverables:**
- Complete Application layer with DTOs, interfaces, and service implementations.
- Validation logic for all entities.

---

### Task 2.5: API Layer — Controllers & Endpoints
**Objective:** Expose Academic module functionality via REST API endpoints.

**Actions:**
- Create controllers:
  - `FacultiesController` — `/api/v1/academic/faculties`
  - `DepartmentsController` — `/api/v1/academic/departments`
  - `ProgramsController` — `/api/v1/academic/programs`
  - `StudyPlansController` — `/api/v1/academic/study-plans`
  - `CoursesController` — `/api/v1/academic/courses`
  - `AcademicYearsController` — `/api/v1/academic/years`
  - `SemestersController` — `/api/v1/academic/semesters`
- Implement CRUD endpoints for each entity.
- Add query endpoints:
  - `GET /api/v1/academic/faculties/{id}/departments`
  - `GET /api/v1/academic/departments/{id}/programs`
  - `GET /api/v1/academic/programs/{id}/study-plans`
  - `GET /api/v1/academic/study-plans/{id}/courses`
- Apply authorization policies:
  - `SystemAdmin`: Full access
  - `FacultyManager`: Read faculty/department data, manage quotas
  - `DepartmentManager`: Manage programs, study plans, courses within department
  - `AcademicAffairs`: Manage academic years, semesters, registration periods

**Deliverables:**
- RESTful API endpoints for all Academic entities.
- Authorization enforcement via Identity module's RBAC.

---

### Task 2.6: RBAC Integration & Policy Enforcement
**Objective:** Integrate Academic module with Identity module's authorization system.

**Actions:**
- Create authorization policies in Academic.Api:
  - `AcademicPolicy` requiring specific permissions
- Apply `[RequirePermission]` or endpoint filters to controllers.
- Ensure all mutating endpoints validate user role against entity ownership (e.g., Department Manager can only modify courses in their department).
- Implement soft delete pattern where applicable.

**Deliverables:**
- All Academic API endpoints protected by RBAC.
- Cross-module authorization working (Academic module uses Identity's `IAuthorizationService`).

---

### Task 2.7: API Configuration & Cross-Cutting Concerns
**Objective:** Wire up the Academic API with enterprise configurations.

**Actions:**
- Create `LuxorLMS.Academic.Api/Program.cs` with:
  - Controllers
  - EF Core with PostgreSQL (shared database or separate schema)
  - Swagger/OpenAPI
  - CORS
  - Authentication/Authorization (JWT from Identity module)
  - Global exception handling with `ProblemDetails`
- Configure FluentValidation for request DTOs.
- Add API versioning (`/api/v1/...`).

**Deliverables:**
- Fully configured Academic API pipeline.
- Swagger UI with JWT auth.

---

## Expected Deliverables Summary

| Deliverable | Description |
|:---|:---|
| Solution Structure | 4 new Academic module projects added to `LuxorLMS.slnx` |
| Database Schema | EF Core migrations for Faculty, Department, Program, StudyPlan, Course, AcademicYear, Semester |
| Domain Model | Complete entity graph with relationships and value objects |
| Application Layer | DTOs, validation logic, and service interfaces |
| API Layer | REST endpoints for Academic hierarchy with RBAC enforcement |
| Documentation | Swagger/OpenAPI with all endpoints documented |

---

## Acceptance Criteria
1. **Build:** `dotnet build LuxorLMS.slnx` succeeds with zero errors.
2. **Database:** EF Core migrations apply cleanly, all tables created with correct relationships.
3. **RBAC:** Unauthorized users receive 403/401; authorized users can only access permitted resources.
4. **Validation:** Business rules enforced (unique codes, date ranges, prerequisite checks).
5. **API:** Swagger UI functional with JWT-protected routes.

---

## Execution Order
1. Scaffold Academic module projects (Task 2.1)
2. Build Domain layer (Task 2.2)
3. Implement Infrastructure layer (Task 2.3)
4. Implement Application layer (Task 2.4)
5. Implement API controllers (Task 2.5)
6. Integrate RBAC (Task 2.6)
7. Configure API pipeline (Task 2.7)
8. Build and validate

---

## Estimated Timeline
- **Task 2.1:** 1 hour
- **Task 2.2:** 3 hours
- **Task 2.3:** 3 hours
- **Task 2.4:** 3 hours
- **Task 2.5:** 3 hours
- **Task 2.6:** 2 hours
- **Task 2.7:** 1 hour
- **Total:** ~16 hours (2 working days)

---

## Dependencies
- Milestone 1 completed (Identity module, JWT auth, RBAC)
- .NET 8 SDK
- PostgreSQL
- Existing `LuxorLMS.Kernel` shared library
