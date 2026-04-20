using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin.Controls;
using OmniSight.Services;
using OmniSight.Core.Entities;

namespace OmniSight.UI.Forms
{
    /// <summary>
    /// Deprecated: Use FrmExamManagement instead
    /// This form is kept as a redirect for backward compatibility
    /// It automatically opens the new FrmExamManagement form
    /// </summary>
    public class FrmExamManager : Form
    {
        private readonly ExamService _examService;
        private readonly ClassService _classService;
        private readonly AuthService _authService;
        private readonly int _teacherId;

        private MaterialLabel lblInfo;
        private MaterialButton btnOpen;

        public FrmExamManager(ExamService examService, ClassService classService, AuthService authService, int teacherId)
        {
            _examService = examService;
            _classService = classService;
            _authService = authService;
            _teacherId = teacherId;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Quản Lý Đề Thi - Chuyển Hướng";
            this.Width = 500;
            this.Height = 250;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Panel
            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20), AutoScroll = true };

            // Header
            var lblHeader = new MaterialLabel
            {
                Text = "Chuyển Hướng Giao Diện",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 10)
            };

            // Info label
            lblInfo = new MaterialLabel
            {
                Text = "Giao diện quản lý bài thi đã được cập nhật.\n\nVui lòng nhấp nút dưới để mở giao diện mới với đầy đủ tính năng:\n• Danh sách bài thi\n• Quản lý câu hỏi\n• Xem kết quả học sinh",
                AutoSize = true,
                MaximumSize = new Size(400, 0),
                Margin = new Padding(0, 10, 0, 20),
                ForeColor = Color.Gray
            };

            // Redirect button
            btnOpen = new MaterialButton
            {
                Text = "Mở Giao Diện Quản Lý Bài Thi",
                Width = 200,
                Height = 40,
                Margin = new Padding(0, 0, 10, 0),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            btnOpen.Click += async (s, e) => await OpenExamManagement();

            // Cancel button
            var btnCancel = new MaterialButton
            {
                Text = "Đóng",
                Width = 100,
                Height = 40,
                Margin = new Padding(10, 0, 0, 0),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            btnCancel.Click += (s, e) => this.Close();

            // Button panel
            var btnPanel = new Panel
            {
                Height = 50,
                Dock = DockStyle.Bottom,
                Padding = new Padding(10)
            };
            btnPanel.Controls.Add(btnOpen);
            btnPanel.Controls.Add(btnCancel);

            panel.Controls.Add(lblHeader);
            panel.Controls.Add(lblInfo);

            this.Controls.Add(btnPanel);
            this.Controls.Add(panel);

            this.Load += (s, e) => AutoOpenNewForm();
        }

        private async void AutoOpenNewForm()
        {
            // Auto-open the new form after short delay
            await Task.Delay(1000);
            OpenExamManagement();
        }

        private async Task OpenExamManagement()
        {
            try
            {
                // Open the new comprehensive exam management form
                using (var frm = new FrmExamManagement(_examService, _classService, _teacherId))
                {
                    frm.ShowDialog(this);
                }
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi mở giao diện quản lý bài thi:\n\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    /// <summary>
    /// Helper class for storing ID in ListViewItem
    /// </summary>
    public class ListViewItemWithId
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public override string ToString() => Text;
    }
}

