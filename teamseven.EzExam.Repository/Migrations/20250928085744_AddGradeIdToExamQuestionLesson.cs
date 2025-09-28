using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace teamseven.EzExam.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddGradeIdToExamQuestionLesson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CorrectAnswer",
                schema: "public",
                table: "questions",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Explanation",
                schema: "public",
                table: "questions",
                type: "character varying(5000)",
                maxLength: 5000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Formula",
                schema: "public",
                table: "questions",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GradeId",
                schema: "public",
                table: "questions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Options",
                schema: "public",
                table: "questions",
                type: "character varying(5000)",
                maxLength: 5000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GradeId",
                schema: "public",
                table: "lessons",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GradeId",
                schema: "public",
                table: "exams",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_questions_GradeId",
                schema: "public",
                table: "questions",
                column: "GradeId");

            migrationBuilder.CreateIndex(
                name: "IX_lessons_GradeId",
                schema: "public",
                table: "lessons",
                column: "GradeId");

            migrationBuilder.CreateIndex(
                name: "IX_exams_GradeId",
                schema: "public",
                table: "exams",
                column: "GradeId");

            migrationBuilder.AddForeignKey(
                name: "FK_exams_grades_GradeId",
                schema: "public",
                table: "exams",
                column: "GradeId",
                principalSchema: "public",
                principalTable: "grades",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_lessons_grades_GradeId",
                schema: "public",
                table: "lessons",
                column: "GradeId",
                principalSchema: "public",
                principalTable: "grades",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_questions_grades_GradeId",
                schema: "public",
                table: "questions",
                column: "GradeId",
                principalSchema: "public",
                principalTable: "grades",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_exams_grades_GradeId",
                schema: "public",
                table: "exams");

            migrationBuilder.DropForeignKey(
                name: "FK_lessons_grades_GradeId",
                schema: "public",
                table: "lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_questions_grades_GradeId",
                schema: "public",
                table: "questions");

            migrationBuilder.DropIndex(
                name: "IX_questions_GradeId",
                schema: "public",
                table: "questions");

            migrationBuilder.DropIndex(
                name: "IX_lessons_GradeId",
                schema: "public",
                table: "lessons");

            migrationBuilder.DropIndex(
                name: "IX_exams_GradeId",
                schema: "public",
                table: "exams");

            migrationBuilder.DropColumn(
                name: "CorrectAnswer",
                schema: "public",
                table: "questions");

            migrationBuilder.DropColumn(
                name: "Explanation",
                schema: "public",
                table: "questions");

            migrationBuilder.DropColumn(
                name: "Formula",
                schema: "public",
                table: "questions");

            migrationBuilder.DropColumn(
                name: "GradeId",
                schema: "public",
                table: "questions");

            migrationBuilder.DropColumn(
                name: "Options",
                schema: "public",
                table: "questions");

            migrationBuilder.DropColumn(
                name: "GradeId",
                schema: "public",
                table: "lessons");

            migrationBuilder.DropColumn(
                name: "GradeId",
                schema: "public",
                table: "exams");
        }
    }
}
