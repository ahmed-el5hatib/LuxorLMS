using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LuxorLMS.Forums.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialForumsSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "forums");

            migrationBuilder.CreateTable(
                name: "ForumTopics",
                schema: "forums",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseOfferingId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPinned = table.Column<bool>(type: "boolean", nullable: false),
                    IsLocked = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForumTopics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ForumPosts",
                schema: "forums",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TopicId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentPostId = table.Column<Guid>(type: "uuid", nullable: true),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Body = table.Column<string>(type: "text", nullable: false),
                    ModerationStatus = table.Column<int>(type: "integer", nullable: false),
                    ModeratedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForumPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForumPosts_ForumPosts_ParentPostId",
                        column: x => x.ParentPostId,
                        principalSchema: "forums",
                        principalTable: "ForumPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForumPosts_ForumTopics_TopicId",
                        column: x => x.TopicId,
                        principalSchema: "forums",
                        principalTable: "ForumTopics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ForumPosts_IsDeleted",
                schema: "forums",
                table: "ForumPosts",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ForumPosts_ModerationStatus",
                schema: "forums",
                table: "ForumPosts",
                column: "ModerationStatus");

            migrationBuilder.CreateIndex(
                name: "IX_ForumPosts_ParentPostId",
                schema: "forums",
                table: "ForumPosts",
                column: "ParentPostId");

            migrationBuilder.CreateIndex(
                name: "IX_ForumPosts_TopicId_CreatedAt",
                schema: "forums",
                table: "ForumPosts",
                columns: new[] { "TopicId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ForumTopics_CourseOfferingId_CreatedAt",
                schema: "forums",
                table: "ForumTopics",
                columns: new[] { "CourseOfferingId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ForumTopics_CourseOfferingId_IsPinned_CreatedAt",
                schema: "forums",
                table: "ForumTopics",
                columns: new[] { "CourseOfferingId", "IsPinned", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ForumTopics_IsDeleted",
                schema: "forums",
                table: "ForumTopics",
                column: "IsDeleted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ForumPosts",
                schema: "forums");

            migrationBuilder.DropTable(
                name: "ForumTopics",
                schema: "forums");
        }
    }
}
