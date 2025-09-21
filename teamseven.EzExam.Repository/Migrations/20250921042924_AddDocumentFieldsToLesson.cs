using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace teamseven.EzExam.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentFieldsToLesson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE lessons ADD COLUMN IF NOT EXISTS \"Document\" character varying(5000);");
            migrationBuilder.Sql("ALTER TABLE lessons ADD COLUMN IF NOT EXISTS \"DocumentType\" character varying(50);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Document",
                schema: "public",
                table: "lessons");

            migrationBuilder.DropColumn(
                name: "DocumentType",
                schema: "public",
                table: "lessons");
        }
    }
}
