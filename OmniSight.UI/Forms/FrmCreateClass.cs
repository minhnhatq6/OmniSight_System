using System;
using System.Windows.Forms;
using MaterialSkin.Controls;
using OmniSight.Services;
using Microsoft.Extensions.DependencyInjection;

namespace OmniSight.UI.Forms
{
    public partial class FrmCreateClass : MaterialForm
    {
        private readonly ClassService _classService;
        private readonly int _currentTeacherId;

        public FrmCreateClass(ClassService classService, int teacherId)
        {
            InitializeComponent();
            _classService = classService;
            _currentTeacherId = teacherId;
        }

        private async void FrmCreateClass_Load(object sender, EventArgs e)
        {
            // --- KIỂM TRA QUYỀN GIÁO VIÊN NGAY KHI MỞ ---
            var authService = Program.ServiceProvider.GetRequiredService<AuthService>();
            if (authService.CurrentUser == null || !authService.CurrentUser.IsTeacher)
            {
                MessageBox.Show("Chỉ giáo viên mới có quyền tạo lớp học!", "Từ chối truy cập",
                                MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.Close();
                return;
            }

            try
            {
                // Gọi hàm lấy môn học riêng của giáo viên này
                var subjects = await _classService.GetSubjectsByTeacherAsync(_currentTeacherId);

                if (subjects != null && subjects.Count > 0)
                {
                    cmbSubjects.DisplayMember = "SubjectName";
                    cmbSubjects.ValueMember = "SubjectId";
                    cmbSubjects.DataSource = subjects;
                    cmbSubjects.SelectedIndex = 0;
                    switchNewSubject.Checked = false;
                }
                else
                {
                    switchNewSubject.Checked = true;
                    switchNewSubject.Enabled = false;
                    cmbSubjects.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách môn học: " + ex.Message);
            }
        }

        private void switchNewSubject_CheckedChanged(object sender, EventArgs e)
        {
            txtNewSubject.Enabled = switchNewSubject.Checked;
            cmbSubjects.Enabled = !switchNewSubject.Checked;
            if (switchNewSubject.Checked) txtNewSubject.Focus();
        }

        private async void btnCreate_Click(object sender, EventArgs e)
        {
            string className = txtClassName.Text.Trim();
            if (string.IsNullOrEmpty(className))
            {
                MessageBox.Show("Vui lòng nhập tên lớp học!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int finalSubjectId = 0;

            try
            {
                if (switchNewSubject.Checked)
                {
                    string newSubName = txtNewSubject.Text.Trim();
                    if (string.IsNullOrEmpty(newSubName))
                    {
                        MessageBox.Show("Vui lòng nhập tên môn học mới!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // SỬA: Truyền thêm TeacherId để tạo môn riêng
                    var subject = await _classService.GetOrCreateSubjectAsync(newSubName, _currentTeacherId);
                    finalSubjectId = subject.SubjectId;
                }
                else
                {
                    if (cmbSubjects.SelectedValue == null)
                    {
                        MessageBox.Show("Vui lòng chọn một môn học!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    finalSubjectId = (int)cmbSubjects.SelectedValue;
                }

                var newClass = await _classService.CreateClassAsync(className, _currentTeacherId, finalSubjectId);

                MessageBox.Show($"Tạo lớp thành công!\nMã tham gia (Join Code): {newClass.JoinCode}",
                                "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tạo lớp: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}