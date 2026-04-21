using System;
using System.Drawing;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using OmniSight.Services;

namespace OmniSight.UI.Forms
{
    public partial class FrmCreateClass : MaterialForm
    {
        private readonly ClassService _classService;
        private readonly int _currentTeacherId;

        // Khai báo các biến giao diện chuẩn Material
        private MaterialTextBox2 txtClassName;
        private MaterialComboBox cmbSubjects;
        private MaterialSwitch switchNewSubject;
        private MaterialTextBox2 txtNewSubject;
        private MaterialButton btnCreate;

        public FrmCreateClass(ClassService classService, int teacherId)
        {
            _classService = classService;
            _currentTeacherId = teacherId;

            // Xóa hàm InitializeComponent() cũ vì chúng ta sẽ tự vẽ lại toàn bộ
            BuildModernUI();
            MaterialSkinManager.Instance.AddFormToManage(this);

            this.Load += FrmCreateClass_Load;
        }

        private void BuildModernUI()
        {
            this.Text = "TẠO LỚP HỌC MỚI";
            this.Size = new Size(500, 480);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.Sizable = false;

            // 1. Tên lớp học
            txtClassName = new MaterialTextBox2
            {
                Hint = "Nhập tên lớp học (Ví dụ: 12A1)",
                Location = new Point(30, 80),
                Width = 440,
                UseTallSize = false
            };

            // 2. Chọn môn học có sẵn
            cmbSubjects = new MaterialComboBox
            {
                Hint = "Chọn môn học",
                Location = new Point(30, 140),
                Width = 440,
                UseTallSize = false,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // 3. Công tắc tạo môn mới
            switchNewSubject = new MaterialSwitch
            {
                Text = "Tạo môn học mới (Chưa có trong danh sách)",
                Location = new Point(30, 210),
                AutoSize = true
            };
            switchNewSubject.CheckedChanged += SwitchNewSubject_CheckedChanged;

            // 4. Ô nhập môn mới (Mặc định ẩn)
            txtNewSubject = new MaterialTextBox2
            {
                Hint = "Nhập tên môn học mới",
                Location = new Point(30, 270),
                Width = 440,
                UseTallSize = false,
                Enabled = false,
                Visible = false // Ẩn đi cho gọn, khi nào gạt switch mới hiện
            };

            // 5. Nút Tạo lớp
            btnCreate = new MaterialButton
            {
                Text = "TẠO LỚP HỌC",
                Location = new Point(310, 350),
                Size = new Size(160, 36),
                Type = MaterialButton.MaterialButtonType.Contained,
                UseAccentColor = true // Dùng màu nhấn (Cam/Đỏ) cho nổi bật
            };
            btnCreate.Click += BtnCreate_Click;

            // Gắn vào Form
            this.Controls.AddRange(new Control[] { txtClassName, cmbSubjects, switchNewSubject, txtNewSubject, btnCreate });
        }

        private async void FrmCreateClass_Load(object sender, EventArgs e)
        {
            try
            {
                // Lấy môn học riêng của giáo viên này
                var subjects = await _classService.GetSubjectsByTeacherAsync(_currentTeacherId);

                if (subjects != null && subjects.Count > 0)
                {
                    cmbSubjects.DataSource = subjects;
                    cmbSubjects.DisplayMember = "SubjectName";
                    cmbSubjects.ValueMember = "SubjectId";
                    cmbSubjects.SelectedIndex = 0;
                }
                else
                {
                    // Ép tạo môn mới nếu chưa có môn nào
                    switchNewSubject.Checked = true;
                    switchNewSubject.Enabled = false;
                    cmbSubjects.Enabled = false;
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải môn học: " + ex.Message); }
        }

        private void SwitchNewSubject_CheckedChanged(object sender, EventArgs e)
        {
            txtNewSubject.Enabled = switchNewSubject.Checked;
            txtNewSubject.Visible = switchNewSubject.Checked; // Hiệu ứng ẩn/hiện
            cmbSubjects.Enabled = !switchNewSubject.Checked;

            if (switchNewSubject.Checked) txtNewSubject.Focus();
        }

        private async void BtnCreate_Click(object sender, EventArgs e)
        {
            string className = txtClassName.Text.Trim();
            if (string.IsNullOrEmpty(className))
            {
                MessageBox.Show("Vui lòng nhập tên lớp học!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtClassName.Focus(); return;
            }

            int finalSubjectId = 0;

            try
            {
                btnCreate.Enabled = false; // Chống click nhiều lần

                if (switchNewSubject.Checked)
                {
                    string newSubName = txtNewSubject.Text.Trim();
                    if (string.IsNullOrEmpty(newSubName))
                    {
                        MessageBox.Show("Vui lòng nhập tên môn học mới!", "Cảnh báo");
                        txtNewSubject.Focus(); return;
                    }

                    var subject = await _classService.GetOrCreateSubjectAsync(newSubName, _currentTeacherId);
                    finalSubjectId = subject.SubjectId;
                }
                else
                {
                    if (cmbSubjects.SelectedValue == null)
                    {
                        MessageBox.Show("Vui lòng chọn môn học!"); return;
                    }
                    finalSubjectId = (int)cmbSubjects.SelectedValue;
                }

                var newClass = await _classService.CreateClassAsync(className, _currentTeacherId, finalSubjectId);

                MessageBox.Show($"Tạo lớp thành công!\nMã tham gia (Join Code): {newClass.JoinCode}", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi hệ thống");
                btnCreate.Enabled = true;
            }
        }
    }
}