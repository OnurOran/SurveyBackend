using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSurveyIntroConsentOutroFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConsentText",
                table: "Surveys",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IntroText",
                table: "Surveys",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OutroText",
                table: "Surveys",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequireConsent",
                table: "Surveys",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConsentText",
                table: "Surveys");

            migrationBuilder.DropColumn(
                name: "IntroText",
                table: "Surveys");

            migrationBuilder.DropColumn(
                name: "OutroText",
                table: "Surveys");

            migrationBuilder.DropColumn(
                name: "RequireConsent",
                table: "Surveys");
        }
    }
}
