using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace teamseven.EzExam.Repository.Migrations
{
    /// <inheritdoc />
    public partial class RemoveNonExistentQuestionColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop columns that don't exist in the actual database but are referenced in the model
            migrationBuilder.Sql("ALTER TABLE questions DROP COLUMN IF EXISTS \"CorrectAnswer\";");
            migrationBuilder.Sql("ALTER TABLE questions DROP COLUMN IF EXISTS \"Explanation\";");
            migrationBuilder.Sql("ALTER TABLE questions DROP COLUMN IF EXISTS \"Formula\";");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Add back columns if needed
            migrationBuilder.AddColumn<string>(
                name: "CorrectAnswer",
                table: "questions",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Explanation",
                table: "questions", 
                type: "character varying(5000)",
                maxLength: 5000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Formula",
                table: "questions",
                type: "character varying(2000)", 
                maxLength: 2000,
                nullable: true);
        }
    }
}
