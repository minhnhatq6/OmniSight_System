using System;
using System.Windows.Forms;
using MaterialSkin.Controls;
using OmniSight.Services;

namespace OmniSight.UI.Forms
{
    public partial class FrmJoinClass : MaterialForm
    {
        private readonly ClassService _classService;
        private readonly int _currentStudentId;

        public FrmJoinClass(ClassService classService, int studentId)
        {
            InitializeComponent();
            _classService = classService;
            _currentStudentId = studentId;
        }

        private async void btnJoin_Click(object sender, EventArgs e)
        {
            string code = txtJoinCode.Text.Trim();

            if (string.IsNullOrEmpty(code) || code.Length != 6)
            {
                MessageBox.Show("Vui lòng nhập đúng mã code 6 ký tự!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Gọi hàm JoinClassAsync đã viết sẵn
                bool success = await _classService.JoinClassAsync(code, _currentStudentId);

                if (success)
                {
                    MessageBox.Show("Tham gia lớp học thành công!", "Tuyệt vời", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Mã lớp không tồn tại hoặc bạn đã ở trong lớp này rồi!", "Thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hệ thống: " + ex.Message);
            }
        }
    }
}