# Milestone 6: Storage, Notifications & Forums — Execution Plan

> Extracted from the master plan: `LuxorLMS_Specification_README.md` → "## 1. Project Analysis LuxorLMS" → Milestone 6 (Weeks 14-16).
> Mirrors the structure and conventions of Milestones 1–5.

## Overview
Sixth milestone in the LuxorLMS modular monolith. Introduces three cross-cutting modules that many prior and future milestones depend on:

- **Storage Module** (spec module #13): Cloud-agnostic object storage (S3 / Azure Blob / MinIO) behind signed URLs, file versioning, and an abstract provider binding. Required by Assignments (submissions), Course Materials, and Reporting (exported documents).
- **Notifications Module** (spec module #9): Template-driven, asynchronous dispatcher across In-App, Email (SMTP/SendGrid), SMS (Twilio), and Mobile Push, with scheduled dispatch and channel fallback. Required by Attendance alerts, Grading publish, and Registration windows.
- **Forums Module** (spec module #8): Threaded social communication with moderation rights for Doctors/TAs.

Builds on Milestone 1 (Identity/RBAC, `IAuthorizationService`, user contact channels), Milestone 2 (Academic hierarchy for course-scoped forums), Milestone 3 (CourseOfferings/Sections for scoping), and Milestone 5 (Assignments/Quizzes for storage integration hooks).

## Objectives (from master plan)
- **M6.1 Storage:** Abstract `IStorageProvider` with S3 / Azure Blob / MinIO adapters; signed (expiring) URL generation; file versioning on re-upload.
- **M6.2 Notifications:** Templated dispatch engine across In-App/Email/SMS/Push; immediate + scheduled send; Push→Email fallback rule; Hangfire-backed async queue.
- **M6.3 Forums:** Threaded topics/posts scoped to CourseOffering; Doctor/TA moderation; student participation under RBAC.

## Dependencies
- Milestone 1 (Identity, JWT, RBAC) — `IAuthorizationService` reused; `User` email/phone/push-token channels for notification targeting.
- Milestone 2 (Academic) — read `Course`, `CourseOffering`, `Section` via Academic.Application for scoping forums and resolving course audiences.
- Milestone 3 (Registration) — `CourseOffering`/`Section` membership context for forum visibility and notification audiences.
- Milestone 5 (Assignments) — Storage provider consumed by the submission/file pipeline (anti-corruption adapter over Storage).
- .NET 8 SDK, PostgreSQL, `LuxorLMS.Kernel` (`Result<T>`/`Error`), Redis + Hangfire (already planned), `AWSSDK.S3`, `Azure.Storage.Blobs`, `Minio` (or `MinIO.AspNetCore`), `SendGrid`, `Twilio`, `FluentValidation`.

## Integration Approach
- New projects `LuxorLMS.Storage.{Domain,Application,Infrastructure,Api}`, `LuxorLMS.Notifications.{Domain,Application,Infrastructure,Api}`, `LuxorLMS.Forums.{Domain,Application,Infrastructure,Api}` added to `LuxorLMS.slnx`.
- Each module owns its own DbContext and (logical) database:
  - `LuxorLMSStorageDbContext` → `luxorlms_storage`
  - `LuxorLMSNotificationsDbContext` → `luxorlms_notifications`
  - `LuxorLMSForumsDbContext` → `luxorlms_forums`
- Object blobs themselves are **not** stored in PostgreSQL; only metadata (`StoredFile` records with version, provider key, content hash) lives in the Storage DB. Physical bytes live in the configured provider bucket/container.
- Cross-module reads use anti-corruption adapters:
  - `IAcademicForumGateway` (Forums → Academic) — resolve `CourseOffering`/section membership.
  - `IUserNotificationGateway` (Notifications → Identity) — resolve recipient channels (email/phone/push token).
  - `IStorageService` (Assignments → Storage) — already referenced in M5 as a placeholder adapter; M6 provides the real implementation.
- Notification dispatch is decoupled via Hangfire background jobs; the API layer enqueues `SendNotificationJob`, workers execute via provider clients.
- Authorization centralized in Identity; each module defines its own `*Permissions` static class.

## Domain Models

### Storage Module
- `StoredFile` (Id, OwnerId, CourseOfferingId Nullable, Container, ObjectKey, Provider [enum], FileName, ContentType, SizeBytes, Version, ContentHash, IsCurrent, CreatedBy, CreatedAt).
- `FileVersion` (Id, StoredFileId, Version, ObjectKey, SizeBytes, ContentHash, CreatedBy, CreatedAt).
- `StorageProviderConfig` (Id, ProviderType [enum S3/Azure/MinIO], BucketOrContainer, Region, Endpoint Nullable, IsActive).
- Enums: `StorageProviderType` (S3, AzureBlob, MinIO).

### Notifications Module
- `NotificationTemplate` (Id, Code, Channel [enum], Subject, BodyTemplate, Culture, CreatedBy, CreatedAt).
- `NotificationMessage` (Id, TemplateId Nullable, RecipientUserId, Channel [enum], Title, Body, Status [enum Queued/Sent/Failed/Cancelled], ScheduledAt Nullable, SentAt Nullable, Error Nullable, CreatedBy, CreatedAt).
- `NotificationPreference` (Id, UserId, Channel [enum], Enabled).
- Enums: `NotificationChannel` (InApp, Email, Sms, Push), `NotificationStatus` (Queued, Sent, Failed, Cancelled).

### Forums Module
- `ForumTopic` (Id, CourseOfferingId, Title, AuthorId, IsPinned, IsLocked, CreatedAt, CreatedBy).
- `ForumPost` (Id, TopicId, ParentPostId Nullable, AuthorId, Body, IsModerated, ModeratedBy Nullable, CreatedAt, CreatedBy).
- Enums: `ForumModerationStatus` (None, Flagged, Removed).

## Tasks Breakdown

### Task 6.1: Storage Domain + Infrastructure + Application + API (M6.1)
- Entities, Enums, Repositories, DbContext, `UnitOfWork`, `IDesignTimeDbContextFactory`.
- `IStorageProvider` abstraction + `S3StorageProvider`, `AzureBlobStorageProvider`, `MinioStorageProvider` implementations.
- `IStorageService`: `UploadAsync`, `GetSignedUrlAsync(objectKey, expiresIn)`, `DownloadAsync`, `DeleteAsync`, `UploadNewVersionAsync` (file versioning + `IsCurrent` rotation + `FileVersion` append).
- Signed URL expiry default 15 minutes (`exp` claim enforced server-side on download).
- Provider selection via `StorageProviderConfig` (active provider) + `IOptions<StorageSettings>`.
- Controllers: `FilesController` (`POST /api/v1/storage/files`, `GET /api/v1/storage/files/{id}/url`, `GET /api/v1/storage/files/{id}/versions`, `DELETE`). `StoragePermissions` + `RequirePermissionFilter`.

### Task 6.2: Notifications Domain + Infrastructure + Application + API (M6.2)
- Entities, Enums, Repositories, DbContext, `UnitOfWork`, `IDesignTimeDbContextFactory`.
- `INotificationTemplateRepository` + seed templates (registration-open, grade-published, attendance-alert, assignment-due).
- `INotificationService`: `SendNowAsync`, `ScheduleAsync(message, at)`, `SendToCourseOfferingAsync(templateCode, courseOfferingId, model)` resolving recipients via `IUserNotificationGateway`.
- Channel dispatch: `IChannelSender` implementations `InAppSender`, `EmailSender` (SendGrid/SMTP), `SmsSender` (Twilio), `PushSender`.
- Fallback rule: Push fail/disabled → Email (configurable per preference).
- Hangfire `SendNotificationJob` enqueued by API; worker resolves template → renders body (structured placeholders) → picks channel senders → updates `NotificationMessage.Status`.
- Controllers: `NotificationTemplatesController`, `NotificationsController` (send/schedule/status), `NotificationPreferencesController`. `NotificationsPermissions` + filter.

### Task 6.3: Forums Domain + Infrastructure + Application + API (M6.3)
- Entities, Enums, Repositories, DbContext, `UnitOfWork`, `IDesignTimeDbContextFactory`.
- `IForumService`: topic CRUD (scoped to `CourseOffering`, visibility via `IAcademicForumGateway`), threaded post CRUD with `ParentPostId`, moderation (Doctor/TA flag/remove), pin/lock.
- Pagination: cursor (keyset) for infinite-scroll topic/post feeds (per spec §5).
- Controllers: `ForumTopicsController`, `ForumPostsController`. `ForumsPermissions` + filter (Doctor/TA moderate; Student post only in enrolled offerings).

### Task 6.4: Cross-Module Wiring (M6 integration)
- Storage: register `IStorageService` real implementation; update Assignments' placeholder adapter to consume it (M5.4 file upload/versioning now persists via Storage).
- Notifications: wire Attendance absence alert (M5.2), Grade publish (M4.3), and Registration window open (M3.1) to enqueue `NotificationMessage` via `INotificationService`.
- Forums: scope visibility to `SectionMember`/`CourseEnrollment` membership from Academic/Registration.
- Hangfire dashboard registered in Notifications.Api (or shared gateway host) with Authorization-only access.

## Acceptance Criteria
1. `dotnet build LuxorLMS.slnx` succeeds, 0 errors.
2. Migrations generated for all three modules; tables created with correct relationships/indexes (including `FileVersion`, `NotificationMessage`, `ForumPost.ParentPostId`).
3. RBAC: unauthorized → 401/403; students limited to own files/notifications and to enrolled-offering forums.
4. Storage business rules: signed URLs expire (re-download after expiry rejected); `IsCurrent` always exactly one per `StoredFile`; re-upload creates a new `FileVersion`; provider abstraction swappable via config without code change.
5. Notifications: channel send executed async via Hangfire; Push→Email fallback verified; scheduled messages dispatch at `ScheduledAt`.
6. Forums: threads scoped to `CourseOffering`; moderation restricted to Doctor/TA; cursor pagination returns stable feeds.
7. Swagger UI functional with JWT-protected routes across all three module APIs.

## Estimated Timeline
- 6.1 Storage (Domain+Infra+App+API): 6h
- 6.2 Notifications (Domain+Infra+App+API+Hangfire): 7h
- 6.3 Forums (Domain+Infra+App+API): 5h
- 6.4 Cross-module wiring (Storage/Notifications/Forums integration): 3h
- Build/validate/migrations: 3h
- Total: ~24h (≈3 working days)

## Execution Order
1. Scaffold the three module projects (Task structure mirrors M2/M3).
2. Storage Domain → Infrastructure → Application → API (Task 6.1).
3. Notifications Domain → Infrastructure → Application → API + Hangfire jobs (Task 6.2).
4. Forums Domain → Infrastructure → Application → API (Task 6.3).
5. Cross-module wiring & provider/config plumbing (Task 6.4).
6. Build, migrate, validate against acceptance criteria.
