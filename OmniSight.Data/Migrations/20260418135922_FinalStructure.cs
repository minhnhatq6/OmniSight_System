using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniSight.Data.Migrations
{
    /// <inheritdoc />
    public partial class FinalStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BaiNop_NguoiDung_UserId",
                table: "BaiNop");

            migrationBuilder.DropForeignKey(
                name: "FK_BaiTap_NguoiDung_UserId",
                table: "BaiTap");

            migrationBuilder.DropForeignKey(
                name: "FK_BangTin_NguoiDung_UserId",
                table: "BangTin");

            migrationBuilder.DropForeignKey(
                name: "FK_KetQuaThi_NguoiDung_UserId",
                table: "KetQuaThi");

            migrationBuilder.DropForeignKey(
                name: "FK_LopHoc_NguoiDung_MaGiaoVien",
                table: "LopHoc");

            migrationBuilder.DropForeignKey(
                name: "FK_MonHoc_NguoiDung_MaGiaoVien",
                table: "MonHoc");

            migrationBuilder.DropForeignKey(
                name: "FK_ThanhVienLop_NguoiDung_UserId",
                table: "ThanhVienLop");

            migrationBuilder.DropIndex(
                name: "IX_ThanhVienLop_UserId",
                table: "ThanhVienLop");

            migrationBuilder.DropIndex(
                name: "IX_KetQuaThi_UserId",
                table: "KetQuaThi");

            migrationBuilder.DropIndex(
                name: "IX_BangTin_UserId",
                table: "BangTin");

            migrationBuilder.DropIndex(
                name: "IX_BaiTap_UserId",
                table: "BaiTap");

            migrationBuilder.DropIndex(
                name: "IX_BaiNop_UserId",
                table: "BaiNop");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ThanhVienLop");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "KetQuaThi");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BangTin");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BaiTap");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BaiNop");

            migrationBuilder.AddForeignKey(
                name: "FK_LopHoc_NguoiDung_MaGiaoVien",
                table: "LopHoc",
                column: "MaGiaoVien",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung");

            migrationBuilder.AddForeignKey(
                name: "FK_MonHoc_NguoiDung_MaGiaoVien",
                table: "MonHoc",
                column: "MaGiaoVien",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LopHoc_NguoiDung_MaGiaoVien",
                table: "LopHoc");

            migrationBuilder.DropForeignKey(
                name: "FK_MonHoc_NguoiDung_MaGiaoVien",
                table: "MonHoc");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "ThanhVienLop",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "KetQuaThi",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "BangTin",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "BaiTap",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "BaiNop",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThanhVienLop_UserId",
                table: "ThanhVienLop",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_KetQuaThi_UserId",
                table: "KetQuaThi",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BangTin_UserId",
                table: "BangTin",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BaiTap_UserId",
                table: "BaiTap",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BaiNop_UserId",
                table: "BaiNop",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BaiNop_NguoiDung_UserId",
                table: "BaiNop",
                column: "UserId",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung");

            migrationBuilder.AddForeignKey(
                name: "FK_BaiTap_NguoiDung_UserId",
                table: "BaiTap",
                column: "UserId",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung");

            migrationBuilder.AddForeignKey(
                name: "FK_BangTin_NguoiDung_UserId",
                table: "BangTin",
                column: "UserId",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung");

            migrationBuilder.AddForeignKey(
                name: "FK_KetQuaThi_NguoiDung_UserId",
                table: "KetQuaThi",
                column: "UserId",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung");

            migrationBuilder.AddForeignKey(
                name: "FK_LopHoc_NguoiDung_MaGiaoVien",
                table: "LopHoc",
                column: "MaGiaoVien",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MonHoc_NguoiDung_MaGiaoVien",
                table: "MonHoc",
                column: "MaGiaoVien",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung");

            migrationBuilder.AddForeignKey(
                name: "FK_ThanhVienLop_NguoiDung_UserId",
                table: "ThanhVienLop",
                column: "UserId",
                principalTable: "NguoiDung",
                principalColumn: "MaNguoiDung");
        }
    }
}
