using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LuxorLMS.Analytics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialAnalyticsSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "analytics");

            migrationBuilder.CreateTable(
                name: "AnalyticsKpis",
                schema: "analytics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Value = table.Column<decimal>(type: "numeric", nullable: false),
                    Unit = table.Column<string>(type: "text", nullable: true),
                    MetricType = table.Column<int>(type: "integer", nullable: false),
                    TimeRange = table.Column<int>(type: "integer", nullable: false),
                    CourseOfferingId = table.Column<Guid>(type: "uuid", nullable: true),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProgramId = table.Column<Guid>(type: "uuid", nullable: true),
                    CalculatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Metadata = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalyticsKpis", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GpaTrends",
                schema: "analytics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseOfferingId = table.Column<Guid>(type: "uuid", nullable: true),
                    SemesterGpa = table.Column<decimal>(type: "numeric", nullable: false),
                    CumulativeGpa = table.Column<decimal>(type: "numeric", nullable: false),
                    SemesterNumber = table.Column<int>(type: "integer", nullable: false),
                    CalculatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GpaTrends", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GradeDistributions",
                schema: "analytics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseOfferingId = table.Column<Guid>(type: "uuid", nullable: false),
                    GradeLetter = table.Column<string>(type: "text", nullable: false),
                    StudentCount = table.Column<int>(type: "integer", nullable: false),
                    Percentage = table.Column<decimal>(type: "numeric", nullable: false),
                    MinScore = table.Column<decimal>(type: "numeric", nullable: false),
                    MaxScore = table.Column<decimal>(type: "numeric", nullable: false),
                    AverageScore = table.Column<decimal>(type: "numeric", nullable: false),
                    CalculatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GradeDistributions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServerHealthMetrics",
                schema: "analytics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MetricName = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<decimal>(type: "numeric", nullable: false),
                    Unit = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    RecordedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Details = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerHealthMetrics", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnalyticsKpis_DepartmentId_ProgramId_MetricType_PeriodStart",
                schema: "analytics",
                table: "AnalyticsKpis",
                columns: new[] { "DepartmentId", "ProgramId", "MetricType", "PeriodStart" });

            migrationBuilder.CreateIndex(
                name: "IX_AnalyticsKpis_Key_CourseOfferingId_TimeRange_PeriodStart",
                schema: "analytics",
                table: "AnalyticsKpis",
                columns: new[] { "Key", "CourseOfferingId", "TimeRange", "PeriodStart" });

            migrationBuilder.CreateIndex(
                name: "IX_GpaTrends_CourseOfferingId",
                schema: "analytics",
                table: "GpaTrends",
                column: "CourseOfferingId");

            migrationBuilder.CreateIndex(
                name: "IX_GpaTrends_StudentId_SemesterNumber",
                schema: "analytics",
                table: "GpaTrends",
                columns: new[] { "StudentId", "SemesterNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_GradeDistributions_CourseOfferingId",
                schema: "analytics",
                table: "GradeDistributions",
                column: "CourseOfferingId");

            migrationBuilder.CreateIndex(
                name: "IX_GradeDistributions_GradeLetter",
                schema: "analytics",
                table: "GradeDistributions",
                column: "GradeLetter");

            migrationBuilder.CreateIndex(
                name: "IX_ServerHealthMetrics_RecordedAt",
                schema: "analytics",
                table: "ServerHealthMetrics",
                column: "RecordedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ServerHealthMetrics_Status",
                schema: "analytics",
                table: "ServerHealthMetrics",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnalyticsKpis",
                schema: "analytics");

            migrationBuilder.DropTable(
                name: "GpaTrends",
                schema: "analytics");

            migrationBuilder.DropTable(
                name: "GradeDistributions",
                schema: "analytics");

            migrationBuilder.DropTable(
                name: "ServerHealthMetrics",
                schema: "analytics");
        }
    }
}
