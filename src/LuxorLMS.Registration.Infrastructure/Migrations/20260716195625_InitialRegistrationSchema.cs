using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LuxorLMS.Registration.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialRegistrationSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CourseEnrollments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    SemesterId = table.Column<Guid>(type: "uuid", nullable: false),
                    RegistrationPeriodId = table.Column<Guid>(type: "uuid", nullable: true),
                    EnrollmentType = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreditHours = table.Column<int>(type: "integer", nullable: false),
                    GradeLetter = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    ApprovedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    WithdrawnAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RequestedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseEnrollments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegistrationPeriods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SemesterId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProgramId = table.Column<Guid>(type: "uuid", nullable: true),
                    AcademicYearId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LateRegistrationStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LateRegistrationEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MinCreditHours = table.Column<int>(type: "integer", nullable: false),
                    MaxCreditHours = table.Column<int>(type: "integer", nullable: false),
                    GpaCapForMax = table.Column<decimal>(type: "numeric", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationPeriods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentProgramEnrollments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProgramId = table.Column<Guid>(type: "uuid", nullable: false),
                    EnrollmentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentProgramEnrollments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrollments_SemesterId",
                table: "CourseEnrollments",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrollments_StudentId",
                table: "CourseEnrollments",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrollments_StudentId_CourseId_SemesterId",
                table: "CourseEnrollments",
                columns: new[] { "StudentId", "CourseId", "SemesterId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationPeriods_AcademicYearId",
                table: "RegistrationPeriods",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationPeriods_IsActive",
                table: "RegistrationPeriods",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationPeriods_SemesterId",
                table: "RegistrationPeriods",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentProgramEnrollments_StudentId_ProgramId",
                table: "StudentProgramEnrollments",
                columns: new[] { "StudentId", "ProgramId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseEnrollments");

            migrationBuilder.DropTable(
                name: "RegistrationPeriods");

            migrationBuilder.DropTable(
                name: "StudentProgramEnrollments");
        }
    }
}
