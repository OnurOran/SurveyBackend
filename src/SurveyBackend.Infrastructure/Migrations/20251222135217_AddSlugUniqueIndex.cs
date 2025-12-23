using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSlugUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Survey_Slug",
                table: "Survey",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Survey_Slug",
                table: "Survey");
        }
    }
}
