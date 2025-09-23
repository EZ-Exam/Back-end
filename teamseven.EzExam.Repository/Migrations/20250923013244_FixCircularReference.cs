using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace teamseven.EzExam.Repository.Migrations
{
    /// <inheritdoc />
    public partial class FixCircularReference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_ai_test_recommendations_generated_exam_id",
                schema: "public",
                table: "ai_test_recommendations");

            migrationBuilder.DropIndex(
                name: "IX_ai_test_recommendations_GeneratedExamId",
                schema: "public",
                table: "ai_test_recommendations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ai_test_recommendations_GeneratedExamId",
                schema: "public",
                table: "ai_test_recommendations",
                column: "GeneratedExamId");

            migrationBuilder.AddForeignKey(
                name: "fk_ai_test_recommendations_generated_exam_id",
                schema: "public",
                table: "ai_test_recommendations",
                column: "GeneratedExamId",
                principalSchema: "public",
                principalTable: "exams",
                principalColumn: "Id");
        }
    }
}
