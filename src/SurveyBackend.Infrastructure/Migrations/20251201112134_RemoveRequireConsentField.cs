using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRequireConsentField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequireConsent",
                table: "Surveys");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RequireConsent",
                table: "Surveys",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
