# Milestone 5: Attendance & Quizzes — Execution Plan

> Extracted from the master plan: `## 1. Project Analysis LuxorLMS.txt` → "Milestone 5: Attendance & Quizzes (Weeks 12-13)".

## Overview
Fifth milestone in the LuxorLMS modular monolith. Introduces three new modules: **Attendance** (QR token + geolocation verification, absence alerting), **Quizzes** (question bank, visual builder, timed exam execution), and **Assignments** (rubrics, submission pipeline, file versioning, plagiarism integration). Builds on Milestone 1 (Identity/RBAC), Milestone 2 (Academic hierarchy), and Milestone 3 (Registration/Course Offerings/Sections).

## Objectives (from master plan)
- **M5.1** Attendance: QR token generation, geolocation verification, status types (Present/Late/Absent/Excused).
- **M5.2** Attendance alerts: >25% absence threshold flagging.
- **M5.3** Quizzes Module: Question bank, visual quiz builder, timed exam execution.
- **M5.4** Assignments Module: Rubrics, submission pipeline, file versioning, plagiarism check integration.

## Dependencies
- Milestone 1 (Identity, JWT, RBAC) — `IAuthorizationService` reused for permission checks.
- Milestone 2 (Academic) — `Course`, `CourseOffering`, `Section`, `SectionMember`, `Student` read/updated via Academic.Application/Infrastructure.
- Milestone 3 (Registration) — `CourseEnrollment`, `SectionMember` for enrollment context.
- .NET 8 SDK, PostgreSQL, `LuxorLMS.Kernel` (`Result<T>`/`Error`).

## Integration Approach
- New projects `LuxorLMS.Attendance.{Domain,Application,Infrastructure,Api}`, `LuxorLMS.Quizzes.{Domain,Application,Infrastructure,Api}`, `LuxorLMS.Assignments.{Domain,Application,Infrastructure,Api}` added to `LuxorLMS.slnx`.
- Each module has its own DbContext and database.
- Cross-module reads use anti-corruption adapters over Academic services/repositories.
- Authorization centralized in Identity; each module defines its own `*Permissions` static class.

## Domain Models

### Attendance Module
- `AttendanceSession` (Id, CourseOfferingId, SectionId, SessionDate, SessionType, TokenHash, ExpiresAt, CreatedBy).
- `AttendanceRecord` (Id, AttendanceSessionId, StudentId, Status, Latitude, Longitude, CheckedInAt, Notes, CreatedBy).
- Enums: `AttendanceStatus` (Present, Late, Absent, Excused), `AttendanceSessionType` (InPerson, Online, Hybrid).

### Quizzes Module
- `Quiz` (Id, CourseOfferingId, Title, Description, TimeLimitMinutes, IsPublished, AvailableFrom, AvailableTo, CreatedBy).
- `QuizQuestion` (Id, QuizId, QuestionType, Text, Points, DisplayOrder, CreatedBy).
- `QuizOption` (Id, QuizQuestionId, Text, IsCorrect, DisplayOrder, CreatedBy).
- `QuizAttempt` (Id, QuizId, StudentId, StartedAt, SubmittedAt, Score, Status, CreatedBy).
- `QuizAnswer` (Id, QuizAttemptId, QuizQuestionId, SelectedOptionId, TextAnswer, IsCorrect, CreatedBy).
- Enums: `QuestionType` (MultipleChoice, TrueFalse, Essay), `QuizAttemptStatus` (InProgress, Submitted, Graded, Expired).

### Assignments Module
- `Assignment` (Id, CourseOfferingId, Title, Description, DueDate, MaxScore, IsPublished, CreatedBy).
- `AssignmentRubric` (Id, AssignmentId, Criteria, MaxPoints, Description, CreatedBy).
- `AssignmentSubmission` (Id, AssignmentId, StudentId, SubmittedAt, Score, Feedback, Status, CreatedBy).
- `AssignmentFile` (Id, AssignmentSubmissionId, FileName, FileUrl, Version, UploadedAt, CreatedBy).
- Enums: `AssignmentStatus` (Draft, Published, Submitted, Graded, Late, Excused), `SubmissionStatus` (Pending, Submitted, Returned, Late).

## Tasks Breakdown

### Task 5.1: Attendance Domain + Application + Infrastructure + API
- Entities, Enums, Repositories, DbContext, Services, DTOs, Validators, Controllers, RBAC.
- QR token generation service.
- Geolocation verification (distance calculation).
- Attendance percentage alerting service (>25% absence).

### Task 5.2: Quizzes Domain + Application + Infrastructure + API
- Entities, Enums, Repositories, DbContext, Services, DTOs, Validators, Controllers, RBAC.
- Question bank CRUD.
- Quiz builder (questions + options).
- Timed exam execution with auto-submit on expiry.
- RedLock for concurrent quiz submission prevention.

### Task 5.3: Assignments Domain + Application + Infrastructure + API
- Entities, Enums, Repositories, DbContext, Services, DTOs, Validators, Controllers, RBAC.
- Rubric CRUD.
- Submission pipeline (upload + versioning).
- Plagiarism check integration (placeholder adapter).
- File versioning on re-submission.

## Acceptance Criteria
1. `dotnet build LuxorLMS.slnx` succeeds, 0 errors.
2. Migrations generated for all three modules; tables created with correct relationships/indexes.
3. RBAC: unauthorized → 401/403; role-based permissions enforced.
4. Business rules enforced: QR token expiry, geolocation radius, quiz time limits, rubric score ≤ max points, assignment due dates.
5. Attendance alerts computed when absence >25% threshold.
6. Swagger UI functional with JWT-protected routes.

## Estimated Timeline
- Domain (3 modules): 3h
- Infrastructure (3 modules): 4h
- Application (3 modules): 6h
- API + RBAC (3 modules): 4h
- Build/validate/migrations: 3h
- Total: ~20h
