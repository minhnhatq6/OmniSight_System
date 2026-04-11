using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniSight.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAnswersDataToExamResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AnswersData",
                table: "ExamResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnswersData",
                table: "ExamResults");
        }
    }
}
