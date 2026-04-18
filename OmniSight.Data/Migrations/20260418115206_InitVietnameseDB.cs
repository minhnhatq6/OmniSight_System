using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmniSight.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitVietnameseDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NguoiDung",
                columns: table => new
                {
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDangNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AnhDaiDien = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LaHocSinh = table.Column<bool>(type: "bit", nullable: false),
                    LaGiaoVien = table.Column<bool>(type: "bit", nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    MaGoogle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DaXacNhanEmail = table.Column<bool>(type: "bit", nullable: false),
                    DuLieuKhuonMat = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiDung", x => x.MaNguoiDung);
                });

            migrationBuilder.CreateTable(
                name: "MonHoc",
                columns: table => new
                {
                    MaMonHoc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenMonHoc = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaGiaoVien = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonHoc", x => x.MaMonHoc);
                    table.ForeignKey(
                        name: "FK_MonHoc_NguoiDung_MaGiaoVien",
                        column: x => x.MaGiaoVien,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung");
                });

            migrationBuilder.CreateTable(
                name: "TokenXacThuc",
                columns: table => new
                {
                    MaToken = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    ChuoiToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiToken = table.Column<int>(type: "int", nullable: false),
                    NgayHetHan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DaSuDung = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenXacThuc", x => x.MaToken);
                    table.ForeignKey(
                        name: "FK_TokenXacThuc_NguoiDung_MaNguoiDung",
                        column: x => x.MaNguoiDung,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LopHoc",
                columns: table => new
                {
                    MaLop = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenLop = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaThamGia = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MaGiaoVien = table.Column<int>(type: "int", nullable: false),
                    MaMonHoc = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LopHoc", x => x.MaLop);
                    table.ForeignKey(
                        name: "FK_LopHoc_MonHoc_MaMonHoc",
                        column: x => x.MaMonHoc,
                        principalTable: "MonHoc",
                        principalColumn: "MaMonHoc",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LopHoc_NguoiDung_MaGiaoVien",
                        column: x => x.MaGiaoVien,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BaiTap",
                columns: table => new
                {
                    MaBaiTap = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaLop = table.Column<int>(type: "int", nullable: false),
                    TieuDe = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LinkDinhKem = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    HanNop = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaNguoiTao = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaiTap", x => x.MaBaiTap);
                    table.ForeignKey(
                        name: "FK_BaiTap_LopHoc_MaLop",
                        column: x => x.MaLop,
                        principalTable: "LopHoc",
                        principalColumn: "MaLop",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BaiTap_NguoiDung_MaNguoiTao",
                        column: x => x.MaNguoiTao,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung");
                    table.ForeignKey(
                        name: "FK_BaiTap_NguoiDung_UserId",
                        column: x => x.UserId,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung");
                });

            migrationBuilder.CreateTable(
                name: "BangTin",
                columns: table => new
                {
                    MaBaiDang = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaLop = table.Column<int>(type: "int", nullable: false),
                    MaNguoiDang = table.Column<int>(type: "int", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BangTin", x => x.MaBaiDang);
                    table.ForeignKey(
                        name: "FK_BangTin_LopHoc_MaLop",
                        column: x => x.MaLop,
                        principalTable: "LopHoc",
                        principalColumn: "MaLop",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BangTin_NguoiDung_MaNguoiDang",
                        column: x => x.MaNguoiDang,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung");
                    table.ForeignKey(
                        name: "FK_BangTin_NguoiDung_UserId",
                        column: x => x.UserId,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung");
                });

            migrationBuilder.CreateTable(
                name: "DeThi",
                columns: table => new
                {
                    MaDeThi = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaLop = table.Column<int>(type: "int", nullable: false),
                    TieuDe = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ThoiGianLamBai = table.Column<int>(type: "int", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeThi", x => x.MaDeThi);
                    table.ForeignKey(
                        name: "FK_DeThi_LopHoc_MaLop",
                        column: x => x.MaLop,
                        principalTable: "LopHoc",
                        principalColumn: "MaLop",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThanhVienLop",
                columns: table => new
                {
                    MaLop = table.Column<int>(type: "int", nullable: false),
                    MaHocSinh = table.Column<int>(type: "int", nullable: false),
                    NgayThamGia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThanhVienLop", x => new { x.MaLop, x.MaHocSinh });
                    table.ForeignKey(
                        name: "FK_ThanhVienLop_LopHoc_MaLop",
                        column: x => x.MaLop,
                        principalTable: "LopHoc",
                        principalColumn: "MaLop",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThanhVienLop_NguoiDung_MaHocSinh",
                        column: x => x.MaHocSinh,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung");
                    table.ForeignKey(
                        name: "FK_ThanhVienLop_NguoiDung_UserId",
                        column: x => x.UserId,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung");
                });

            migrationBuilder.CreateTable(
                name: "BaiNop",
                columns: table => new
                {
                    MaBaiNop = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaBaiTap = table.Column<int>(type: "int", nullable: false),
                    MaHocSinh = table.Column<int>(type: "int", nullable: false),
                    LinkBaiLam = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NgayNop = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DiemSo = table.Column<float>(type: "real", nullable: true),
                    NhanXet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaiNop", x => x.MaBaiNop);
                    table.ForeignKey(
                        name: "FK_BaiNop_BaiTap_MaBaiTap",
                        column: x => x.MaBaiTap,
                        principalTable: "BaiTap",
                        principalColumn: "MaBaiTap",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BaiNop_NguoiDung_MaHocSinh",
                        column: x => x.MaHocSinh,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung");
                    table.ForeignKey(
                        name: "FK_BaiNop_NguoiDung_UserId",
                        column: x => x.UserId,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung");
                });

            migrationBuilder.CreateTable(
                name: "CauHoi",
                columns: table => new
                {
                    MaCauHoi = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDeThi = table.Column<int>(type: "int", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DapAnA = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DapAnB = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DapAnC = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DapAnD = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DapAnDung = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauHoi", x => x.MaCauHoi);
                    table.ForeignKey(
                        name: "FK_CauHoi_DeThi_MaDeThi",
                        column: x => x.MaDeThi,
                        principalTable: "DeThi",
                        principalColumn: "MaDeThi",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KetQuaThi",
                columns: table => new
                {
                    MaKetQua = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDeThi = table.Column<int>(type: "int", nullable: false),
                    MaHocSinh = table.Column<int>(type: "int", nullable: false),
                    DiemSo = table.Column<float>(type: "real", nullable: true),
                    ThoiGianBatDau = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ThoiGianKetThuc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DuLieuDapAn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KetQuaThi", x => x.MaKetQua);
                    table.ForeignKey(
                        name: "FK_KetQuaThi_DeThi_MaDeThi",
                        column: x => x.MaDeThi,
                        principalTable: "DeThi",
                        principalColumn: "MaDeThi",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KetQuaThi_NguoiDung_MaHocSinh",
                        column: x => x.MaHocSinh,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung");
                    table.ForeignKey(
                        name: "FK_KetQuaThi_NguoiDung_UserId",
                        column: x => x.UserId,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung");
                });

            migrationBuilder.CreateTable(
                name: "NhatKyViPham",
                columns: table => new
                {
                    MaViPham = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKetQua = table.Column<int>(type: "int", nullable: false),
                    LoaiViPham = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AnhViPham = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ThoiGianViPham = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhatKyViPham", x => x.MaViPham);
                    table.ForeignKey(
                        name: "FK_NhatKyViPham_KetQuaThi_MaKetQua",
                        column: x => x.MaKetQua,
                        principalTable: "KetQuaThi",
                        principalColumn: "MaKetQua");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BaiNop_MaBaiTap",
                table: "BaiNop",
                column: "MaBaiTap");

            migrationBuilder.CreateIndex(
                name: "IX_BaiNop_MaHocSinh",
                table: "BaiNop",
                column: "MaHocSinh");

            migrationBuilder.CreateIndex(
                name: "IX_BaiNop_UserId",
                table: "BaiNop",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BaiTap_MaLop",
                table: "BaiTap",
                column: "MaLop");

            migrationBuilder.CreateIndex(
                name: "IX_BaiTap_MaNguoiTao",
                table: "BaiTap",
                column: "MaNguoiTao");

            migrationBuilder.CreateIndex(
                name: "IX_BaiTap_UserId",
                table: "BaiTap",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BangTin_MaLop",
                table: "BangTin",
                column: "MaLop");

            migrationBuilder.CreateIndex(
                name: "IX_BangTin_MaNguoiDang",
                table: "BangTin",
                column: "MaNguoiDang");

            migrationBuilder.CreateIndex(
                name: "IX_BangTin_UserId",
                table: "BangTin",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CauHoi_MaDeThi",
                table: "CauHoi",
                column: "MaDeThi");

            migrationBuilder.CreateIndex(
                name: "IX_DeThi_MaLop",
                table: "DeThi",
                column: "MaLop");

            migrationBuilder.CreateIndex(
                name: "IX_KetQuaThi_MaDeThi",
                table: "KetQuaThi",
                column: "MaDeThi");

            migrationBuilder.CreateIndex(
                name: "IX_KetQuaThi_MaHocSinh",
                table: "KetQuaThi",
                column: "MaHocSinh");

            migrationBuilder.CreateIndex(
                name: "IX_KetQuaThi_UserId",
                table: "KetQuaThi",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LopHoc_MaGiaoVien",
                table: "LopHoc",
                column: "MaGiaoVien");

            migrationBuilder.CreateIndex(
                name: "IX_LopHoc_MaMonHoc",
                table: "LopHoc",
                column: "MaMonHoc");

            migrationBuilder.CreateIndex(
                name: "IX_MonHoc_MaGiaoVien",
                table: "MonHoc",
                column: "MaGiaoVien");

            migrationBuilder.CreateIndex(
                name: "IX_NhatKyViPham_MaKetQua",
                table: "NhatKyViPham",
                column: "MaKetQua");

            migrationBuilder.CreateIndex(
                name: "IX_ThanhVienLop_MaHocSinh",
                table: "ThanhVienLop",
                column: "MaHocSinh");

            migrationBuilder.CreateIndex(
                name: "IX_ThanhVienLop_UserId",
                table: "ThanhVienLop",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TokenXacThuc_MaNguoiDung",
                table: "TokenXacThuc",
                column: "MaNguoiDung");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BaiNop");

            migrationBuilder.DropTable(
                name: "BangTin");

            migrationBuilder.DropTable(
                name: "CauHoi");

            migrationBuilder.DropTable(
                name: "NhatKyViPham");

            migrationBuilder.DropTable(
                name: "ThanhVienLop");

            migrationBuilder.DropTable(
                name: "TokenXacThuc");

            migrationBuilder.DropTable(
                name: "BaiTap");

            migrationBuilder.DropTable(
                name: "KetQuaThi");

            migrationBuilder.DropTable(
                name: "DeThi");

            migrationBuilder.DropTable(
                name: "LopHoc");

            migrationBuilder.DropTable(
                name: "MonHoc");

            migrationBuilder.DropTable(
                name: "NguoiDung");
        }
    }
}
