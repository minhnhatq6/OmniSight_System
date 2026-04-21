using MaterialSkin;
using MaterialSkin.Controls;
using Microsoft.Extensions.DependencyInjection;
using OmniSight.Core.Entities;
using OmniSight.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OmniSight.UI.Forms
{
    public class FrmAdminDashboard : MaterialForm
    {
        private readonly IUserService _userService;
        private readonly IServiceProvider _serviceProvider;

        // Dữ liệu
        private List<User> _allUsers = new List<User>();

        // UI Components
        private DataGridView dgvUsers;
        private MaterialTextBox txtSearch;
        private MaterialComboBox cmbRoleFilter;
        private MaterialTextBox txtEmail, txtFullName;
        private TextBox txtPassword; // Dùng TextBox thuần để an toàn như bạn yêu cầu

        public FrmAdminDashboard(IUserService userService, IServiceProvider serviceProvider)
        {
            _userService = userService;
            _serviceProvider = serviceProvider;

            // Cấu hình Form
            this.Text = "HỆ THỐNG QUẢN TRỊ TẬP TRUNG - OMNISIGHT";
            this.Size = new Size(1300, 800);
            this.MinimumSize = new Size(1000, 600); // Giới hạn thu nhỏ
            this.StartPosition = FormStartPosition.CenterScreen;
            MaterialSkinManager.Instance.AddFormToManage(this);

            BuildModernUI();
            _ = LoadUsersAsync();
        }

        private void BuildModernUI()
        {
            // --- 1. SIDEBAR (BÊN TRÁI - CỐ ĐỊNH CHIỀU RỘNG) ---
            Panel pnlSidebar = new Panel { Dock = DockStyle.Left, Width = 320, BackColor = Color.FromArgb(242, 242, 242), Padding = new Padding(20) };

            MaterialLabel lblHeader = new MaterialLabel { Text = "TẠO GIÁO VIÊN MỚI", FontType = MaterialSkinManager.fontType.H6, Dock = DockStyle.Top, Height = 40 };

            txtFullName = new MaterialTextBox { Hint = "Họ tên đầy đủ", Dock = DockStyle.Top, Margin = new Padding(0, 20, 0, 0) };
            txtEmail = new MaterialTextBox { Hint = "Địa chỉ Email", Dock = DockStyle.Top };

            // Nhóm Mật khẩu (Dùng panel nhỏ để bọc label + textbox thuần)
            Panel pnlPass = new Panel { Dock = DockStyle.Top, Height = 80, Padding = new Padding(0, 10, 0, 0) };
            Label lblPassTitle = new Label { Text = "Mật khẩu khởi tạo:", Dock = DockStyle.Top, Font = new Font("Roboto", 9) };
            txtPassword = new TextBox { Dock = DockStyle.Top, PasswordChar = '•', Font = new Font("Roboto", 12) };
            pnlPass.Controls.Add(txtPassword); pnlPass.Controls.Add(lblPassTitle);

            MaterialButton btnCreate = new MaterialButton { Text = "TẠO TÀI KHOẢN", Dock = DockStyle.Top, Height = 50, UseAccentColor = true, Type = MaterialButton.MaterialButtonType.Contained };
            btnCreate.Click += BtnCreateTeacher_Click;

            MaterialButton btnLogout = new MaterialButton { Text = "ĐĂNG XUẤT", Dock = DockStyle.Bottom, Type = MaterialButton.MaterialButtonType.Outlined };
            btnLogout.Click += (s, e) => { this.Hide(); _serviceProvider.GetRequiredService<Auth.FrmLogin>().Show(); };

            pnlSidebar.Controls.Add(btnCreate);
            pnlSidebar.Controls.Add(pnlPass);
            pnlSidebar.Controls.Add(txtEmail);
            pnlSidebar.Controls.Add(txtFullName);
            pnlSidebar.Controls.Add(lblHeader);
            pnlSidebar.Controls.Add(btnLogout);

            // --- 2. VÙNG NỘI DUNG CHÍNH (BÊN PHẢI - TỰ CO GIÃN) ---
            Panel pnlMain = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20), BackColor = Color.White };

            // THANH CÔNG CỤ (Tìm kiếm + Lọc) - Dùng TableLayoutPanel để chia tỉ lệ
            TableLayoutPanel tlpToolBar = new TableLayoutPanel { Dock = DockStyle.Top, Height = 80, ColumnCount = 2 };
            tlpToolBar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70)); // Search chiếm 70%
            tlpToolBar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30)); // Filter chiếm 30%

            txtSearch = new MaterialTextBox { Hint = "🔍 Tìm theo tên hoặc email...", Dock = DockStyle.Fill };
            txtSearch.TextChanged += (s, e) => ApplyFilters();

            cmbRoleFilter = new MaterialComboBox { Dock = DockStyle.Fill, Hint = "Lọc vai trò" };
            cmbRoleFilter.Items.AddRange(new object[] { "Tất cả người dùng", "Chỉ Giáo viên", "Chỉ Học sinh" });
            cmbRoleFilter.SelectedIndex = 0;
            cmbRoleFilter.SelectedIndexChanged += (s, e) => ApplyFilters();

            tlpToolBar.Controls.Add(txtSearch, 0, 0);
            tlpToolBar.Controls.Add(cmbRoleFilter, 1, 0);

            // BẢNG DỮ LIỆU
            dgvUsers = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowTemplate = { Height = 50 },
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 50
            };
            // Style cho bảng
            dgvUsers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgvUsers.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvUsers.ColumnHeadersDefaultCellStyle.Font = new Font("Roboto", 10, FontStyle.Bold);

            dgvUsers.Columns.AddRange(
                new DataGridViewTextBoxColumn { Name = "UserId", DataPropertyName = "UserId", HeaderText = "ID", Width = 60 },
                new DataGridViewTextBoxColumn { Name = "FullName", DataPropertyName = "FullName", HeaderText = "Họ và Tên" },
                new DataGridViewTextBoxColumn { Name = "Email", DataPropertyName = "Email", HeaderText = "Email / Tên đăng nhập" },
                new DataGridViewTextBoxColumn { Name = "RoleDisplay", DataPropertyName = "RoleDisplay", HeaderText = "Vai Trò", Width = 120 },
                new DataGridViewButtonColumn
                {
                    Name = "ActionCol",
                    DataPropertyName = "ActionText",
                    HeaderText = "Phân quyền",
                    FlatStyle = FlatStyle.Flat,
                    Width = 150
                }
            );
            dgvUsers.CellContentClick += DgvUsers_CellContentClick;

            pnlMain.Controls.Add(dgvUsers);
            pnlMain.Controls.Add(tlpToolBar);

            // Add các vùng vào Form
            this.Controls.Add(pnlMain);
            this.Controls.Add(pnlSidebar);
        }

        private async Task LoadUsersAsync()
        {
            _allUsers = await _userService.GetAllUsersAsync();
            ApplyFilters(); // Nạp dữ liệu xong thì chạy lọc luôn
        }

        private void ApplyFilters()
        {
            string keyword = txtSearch.Text.ToLower().Trim();
            int filterIdx = cmbRoleFilter.SelectedIndex;

            var filtered = _allUsers.Where(u =>
                (string.IsNullOrEmpty(keyword) || u.FullName.ToLower().Contains(keyword) || u.Email.ToLower().Contains(keyword)) &&
                (filterIdx == 0 || (filterIdx == 1 && u.IsTeacher) || (filterIdx == 2 && u.IsStudent))
            ).Select(u => new {
                u.UserId,
                u.FullName,
                u.Email,
                RoleDisplay = u.IsTeacher ? "⭐ Giáo viên" : "👤 Học sinh",
                ActionText = u.IsTeacher ? "✅ Đã là GV" : "Cấp quyền GV"
            }).ToList();

            dgvUsers.DataSource = filtered;
        }

        private async void BtnCreateTeacher_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtFullName.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Vui lòng không để trống thông tin!"); return;
            }

            var result = await _userService.CreateTeacherAccountAsync(txtEmail.Text.Trim(), txtFullName.Text.Trim(), txtPassword.Text);
            if (result.success)
            {
                MessageBox.Show(result.message, "Thành công");
                txtEmail.Clear(); txtFullName.Clear(); txtPassword.Clear();
                await LoadUsersAsync();
            }
            else MessageBox.Show(result.message, "Lỗi");
        }

        private async void DgvUsers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvUsers.Columns[e.ColumnIndex].Name == "ActionCol")
            {
                var actionText = dgvUsers.Rows[e.RowIndex].Cells["ActionCol"].Value?.ToString();
                if (actionText == "Cấp quyền GV")
                {
                    int userId = (int)dgvUsers.Rows[e.RowIndex].Cells["UserId"].Value;
                    if (MessageBox.Show("Xác nhận nâng cấp tài khoản này lên Giáo viên?", "Phân quyền", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        await _userService.GrantTeacherRoleAsync(userId);
                        await LoadUsersAsync();
                    }
                }
            }
        }
    }
}