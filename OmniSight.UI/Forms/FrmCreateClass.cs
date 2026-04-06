using System;
using System.Windows.Forms;
using MaterialSkin.Controls;
using OmniSight.Services;

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
            try
            {
                var subjects = await _classService.GetSubjectsAsync();

                if (subjects != null && subjects.Count > 0)
                {
                    cmbSubjects.DisplayMember = "SubjectName";
                    cmbSubjects.ValueMember = "SubjectId";
                    cmbSubjects.DataSource = subjects;
                    cmbSubjects.SelectedIndex = 0;

                    switchNewSubject.Checked = false; // Tắt mặc định
                }
                else
                {
                    // Nếu chưa có môn nào trong DB -> Ép người dùng tạo mới
                    switchNewSubject.Checked = true;
                    switchNewSubject.Enabled = false; // Khóa công tắc luôn
                    cmbSubjects.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách môn học: " + ex.Message);
            }
        }

        // Sự kiện khi bật/tắt công tắc Tạo môn mới
        private void switchNewSubject_CheckedChanged(object sender, EventArgs e)
        {
            // Nếu bật tạo mới -> Mở TextBox, khóa ComboBox
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
                // PHÂN NHÁNH LOGIC TÌM ID MÔN HỌC
                if (switchNewSubject.Checked)
                {
                    // 1. Nếu đang tạo môn mới
                    string newSubName = txtNewSubject.Text.Trim();
                    if (string.IsNullOrEmpty(newSubName))
                    {
                        MessageBox.Show("Vui lòng nhập tên môn học mới!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Gọi hàm sinh ra/tìm môn học rồi lấy ID
                    var subject = await _classService.GetOrCreateSubjectAsync(newSubName);
                    finalSubjectId = subject.SubjectId;
                }
                else
                {
                    // 2. Nếu chọn từ danh sách có sẵn
                    if (cmbSubjects.SelectedValue == null)
                    {
                        MessageBox.Show("Vui lòng chọn một môn học!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    finalSubjectId = (int)cmbSubjects.SelectedValue;
                }

                // TIẾN HÀNH TẠO LỚP
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