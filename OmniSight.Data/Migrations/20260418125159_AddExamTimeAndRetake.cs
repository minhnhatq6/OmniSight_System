using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniSight.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddExamTimeAndRetake : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TrangThaiLamLai",
                table: "KetQuaThi",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ThoiGianDongDe",
                table: "DeThi",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ThoiGianMoDe",
                table: "DeThi",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrangThaiLamLai",
                table: "KetQuaThi");

            migrationBuilder.DropColumn(
                name: "ThoiGianDongDe",
                table: "DeThi");

            migrationBuilder.DropColumn(
                name: "ThoiGianMoDe",
                table: "DeThi");
        }
    }
}
