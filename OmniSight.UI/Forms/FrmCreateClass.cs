using System;
using System.Windows.Forms;
using MaterialSkin.Controls;
using OmniSight.Services;
using OmniSight.Data; // Để gọi DbContext nếu không dùng DI

namespace OmniSight.UI.Forms
{
    public partial class FrmCreateClass : MaterialForm
    {
        private readonly ClassService _classService;
        private readonly int _currentTeacherId;

        // Constructor yêu cầu truyền vào Service và ID của giáo viên đang đăng nhập
        public FrmCreateClass(ClassService classService, int teacherId)
        {
            InitializeComponent();
            _classService = classService;
            _currentTeacherId = teacherId;
        }

        private async void btnCreate_Click(object sender, EventArgs e)
        {
            string className = txtClassName.Text.Trim();

            // 1. Kiểm tra tên lớp
            if (string.IsNullOrEmpty(className))
            {
                MessageBox.Show("Vui lòng nhập tên lớp học!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Kiểm tra môn học (SelectedValue không được null)
            if (cmbSubjects.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn môn học cho lớp!\n(Nếu danh sách trống, vui lòng thêm môn học vào Database trước)", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int subjectId = (int)cmbSubjects.SelectedValue;

            try
            {
                // Gọi xuống Service để lưu vào Database
                var newClass = await _classService.CreateClassAsync(className, _currentTeacherId, subjectId);

                MessageBox.Show($"Tạo lớp thành công!\nMã tham gia (Join Code) của lớp là: {newClass.JoinCode}",
                                "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Đóng form và báo hiệu thành công về cho Form chính
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tạo lớp: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void FrmCreateClass_Load(object sender, EventArgs e)
        {

            var subjects = await _classService.GetSubjectsAsync(); // Giả sử bạn đã viết hàm này trong ClassService để lấy danh sách môn học từ database
            cmbSubjects.DataSource = subjects;
            cmbSubjects.DisplayMember = "SubjectName"; // Hiển thị tên môn
            cmbSubjects.ValueMember = "SubjectId";     // Giá trị ẩn là ID
        }
    }
}