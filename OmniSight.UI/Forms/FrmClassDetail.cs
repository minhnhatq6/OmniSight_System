using System;
using System.Drawing;
using System.Windows.Forms;
using MaterialSkin.Controls;
using OmniSight.Services;

namespace OmniSight.UI.Forms
{
    public partial class FrmClassDetail : MaterialForm
    {
        private readonly StreamService _streamService;
        private readonly int _currentUserId;
        private readonly int _classId;
        private readonly ClassService _classService;

        // Constructor yêu cầu truyền vào Service, ID người dùng và ID của Lớp học đang mở
        public FrmClassDetail(StreamService streamService, ClassService classService, int currentUserId, int classId, string className)
        {
            InitializeComponent();
            _streamService = streamService;
            _classService = classService;
            _currentUserId = currentUserId;
            _classId = classId;

            this.Text = "Lớp: " + className; // Đổi tiêu đề Form thành tên lớp
        }

        private async void FrmClassDetail_Load(object sender, EventArgs e)
        {
            await LoadMembersAsync();
            await LoadStreamAsync();
            
        }

        private async Task LoadMembersAsync()
        {
            try
            {
                // Giả sử bạn đã kéo lvwMembers vào tab "Mọi người"
                lvwMembers.Items.Clear();
                var members = await _classService.GetClassMembersAsync(_classId);
                foreach (var m in members)
                {
                    var item = new ListViewItem(m.Student.Username); // Cột 1: Họ tên
                    item.SubItems.Add(m.Student.Email);              // Cột 2: Email
                    if (m.Student.IsTeacher)                   // Cột 3: Vai trò (Student/Teacher)
                        item.SubItems.Add("Giáo viên");
                    else
                        item.SubItems.Add("Học sinh");

                    lvwMembers.Items.Add(item);
                }
            }
            catch (Exception)
            {
                // Nếu lvwMembers chưa có cột, bạn có thể khởi tạo nhanh bằng code:
                // lvwMembers.Columns.Add("Họ tên", 200);
            }
        }

        // Hàm lấy dữ liệu và hiển thị lên FlowLayoutPanel
        private async System.Threading.Tasks.Task LoadStreamAsync()
        {
            flpStream.Controls.Clear(); // Xóa bài cũ đi
            var posts = await _streamService.GetPostsByClassIdAsync(_classId);

            foreach (var post in posts)
            {
                // Gọi hàm "vẽ" giao diện cho từng bài đăng
                var postCard = CreatePostCard(post.Author.FullName, post.CreatedAt, post.Content);
                flpStream.Controls.Add(postCard);
            }
        }

        // Hàm "chế tạo" một khung chứa bài viết bằng code
        private Panel CreatePostCard(string authorName, DateTime time, string content)
        {
            Panel card = new Panel
            {
                Width = flpStream.Width - 25, // Trừ hao thanh cuộn
                AutoSize = true,
                Padding = new Padding(10),
                Margin = new Padding(5, 5, 5, 15),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblHeader = new Label
            {
                Text = $"{authorName} - {time:dd/MM/yyyy HH:mm}",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(10, 10)
            };

            Label lblContent = new Label
            {
                Text = content,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                AutoSize = true,
                MaximumSize = new Size(card.Width - 20, 0), // Tự động xuống dòng nếu văn bản dài
                Location = new Point(10, 35)
            };

            card.Controls.Add(lblHeader);
            card.Controls.Add(lblContent);
            return card;
        }

        private async void btnPost_Click(object sender, EventArgs e)
        {
            string content = txtPostContent.Text.Trim();
            if (string.IsNullOrEmpty(content)) return;

            try
            {
                // Lưu vào Database
                await _streamService.CreatePostAsync(_classId, _currentUserId, content);

                txtPostContent.Clear(); // Xóa ô nhập
                await LoadStreamAsync(); // Load lại bảng tin ngay lập tức
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi đăng bài: " + ex.Message);
            }
        }

        private void lvwMembers_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}