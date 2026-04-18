using MaterialSkin.Controls;
using Microsoft.Extensions.DependencyInjection;
using OmniSight.Core.Entities;
using OmniSight.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xceed.Words.NET;
namespace OmniSight.UI.Forms
{
    public partial class UcClassDetail : UserControl
    {
        private readonly StreamService _streamService;
        private readonly ClassService _classService;
        private readonly AuthService _authService;
        private AssignmentService _assignmentService;
        private ExamService _examService;
        private readonly int _classId;
        private readonly int _currentUserId;
        private readonly bool _isTeacherOfThisClass;
        private int? _editingAssignmentId = null;
        private bool _isDataLoading = false; 
        public UcClassDetail(StreamService streamService, ClassService classService, AuthService authService, int currentUserId, int classId, bool isTeacherOfThisClass)
        {
            InitializeComponent();

            // Debug: In ra số tabs sau InitializeComponent
            System.Diagnostics.Debug.WriteLine($"[After InitializeComponent] Total tabs: {materialTabControl1.TabPages.Count}");
            foreach (TabPage tab in materialTabControl1.TabPages)
            {
                System.Diagnostics.Debug.WriteLine($"  - {tab.Name}: {tab.Text}");
            }

            _streamService = streamService;
            _classService = classService;
            _authService = authService;
            _assignmentService = null; // Sẽ được set từ MainForm
            _currentUserId = currentUserId;
            _classId = classId;
            this.VisibleChanged += UcClassDetail_VisibleChanged;
            _isTeacherOfThisClass = isTeacherOfThisClass;
            dtpDueDate.Format = DateTimePickerFormat.Custom;
            dtpDueDate.CustomFormat = "dd/MM/yyyy HH:mm";
           

        }

        // Setter cho AssignmentService (gọi từ MainForm)
        public void SetAssignmentService(AssignmentService assignmentService)
        {
            _assignmentService = assignmentService;
        }

        public void SetExamService(ExamService examService)
        {
            _examService = examService;
        }

        private bool _streamLoaded = false;
        private bool _assignmentLoaded = false;
        private bool _gradingLoaded = false;
        private bool _membersLoaded = false;
        private bool _examsLoaded = false;

        private async void UcClassDetail_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible && !_streamLoaded)
            {
                if (_isDataLoading) return; // Nếu đang load thì thôi

                try
                {
                    _isDataLoading = true;
                    _streamLoaded = true;

                    // PHẢI dùng await từng cái một, không để chúng chạy song song
                    await LoadStreamAsync();
                    await LoadAssignmentsAsync();
                    await LoadGradingAsync();
                    await LoadExamsAsync();
                    await LoadMembersAsync();
                }
                finally
                {
                    _isDataLoading = false;
                }
            }
        }

        private async Task LoadExamsAsync()
        {
            if (this.InvokeRequired) { /*...*/ return; }
            if (_examService == null || flpExams == null) return;

            try
            {
                // === SỬA ĐOẠN NÀY ===
                // Đảm bảo bật Panel cho Giáo viên và Tắt Panel cho Học sinh
                if (panelExamTeacher != null)
                {
                    panelExamTeacher.Visible = _isTeacherOfThisClass;

                    // Rất quan trọng: Đưa nó lên trên cùng (phòng trường hợp nó bị đè bởi flpExams)
                    if (_isTeacherOfThisClass)
                    {
                        panelExamTeacher.BringToFront();
                    }
                }
                // ====================

                flpExams.Controls.Clear();
                flpExams.Padding = new Padding(0, 70, 0, 0);
                var user = _authService.CurrentUser;
                if (user == null) return;

                // Tiêu đề
                flpExams.Controls.Add(new Label { Text = "📖 Đề thi", Font = new Font("Roboto", 14, FontStyle.Bold), AutoSize = true, Padding = new Padding(10) });

                // Chỉ lấy đề của LỚP NÀY
                var exams = await _examService.GetExamsByClassIdAsync(_classId);

                if (exams == null || exams.Count == 0)
                {
                    flpExams.Controls.Add(new Label { Text = "Chưa có đề thi trong lớp này.", AutoSize = true, ForeColor = Color.Gray, Padding = new Padding(10) });
                    return;
                }

                foreach (var exam in exams)
                {
                    var card = CreateExamCard(exam, _isTeacherOfThisClass);
                    flpExams.Controls.Add(card);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải đề thi: " + ex.Message);
            }
        }

        private Panel CreateExamCard(OmniSight.Core.Entities.Exam exam, bool isTeacher)
        {
            Panel card = new Panel
            {
                // Lấy chiều rộng bằng Flp trừ đi một chút margin cho khỏi sát viền
                Width = flpExams.Width > 50 ? flpExams.Width - 30 : 800,
                AutoSize = false,
                Padding = new Padding(15),
                Margin = new Padding(5, 5, 5, 15),
                BackColor = Color.FromArgb(245, 245, 245),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Đảm bảo khi Form to ra/nhỏ lại, card tự động co giãn theo
            card.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            Label lblTitle = new Label
            {
                Text = $"📚 {exam.Title}",
                Font = new Font("Roboto", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(10, 10)
            };
            card.Controls.Add(lblTitle);

            int currentY = lblTitle.Bottom + 10;

            Label lblInfo = new Label
            {
                Text = $"Thời lượng: {exam.DurationMinutes} phút | Tạo: {exam.CreatedAt:dd/MM/yyyy}",
                Font = new Font("Roboto", 9),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(10, currentY)
            };
            card.Controls.Add(lblInfo);

            currentY = lblInfo.Bottom + 15;

            // --- XỬ LÝ NÚT BẤM (CĂN LỀ PHẢI) ---
            if (isTeacher)
            {
                Button btnManage = new Button
                {
                    Text = "🔧 Quản lý",
                    AutoSize = false,
                    Width = 100,
                    Height = 35,
                    BackColor = Color.FromArgb(33, 150, 243),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    // Neo nút này vào bên Phải (Right) của Panel
                    Anchor = AnchorStyles.Top | AnchorStyles.Right
                };
                // Đặt Location dựa trên chiều rộng hiện tại của card (trừ đi chiều rộng nút và margin 15)
                btnManage.Location = new Point(card.Width - btnManage.Width - 15, 10);

                btnManage.Click += async (s, e) =>
                {
                    try
                    {
                        using (var frm = new FrmExamManagement(_examService, _classService, _currentUserId))
                        {
                            if (frm.ShowDialog(this.ParentForm) == DialogResult.OK)
                            {
                                await LoadExamsAsync();
                            }
                        }
                    }
                    catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
                };
                card.Controls.Add(btnManage);
            }
            else // Dành cho HỌC SINH
            {
                DateTime now = DateTime.Now;
                bool isWaiting = exam.StartTime.HasValue && now < exam.StartTime.Value;
                bool isExpired = exam.EndTime.HasValue && now > exam.EndTime.Value;

                var myResult = exam.ExamResults?.FirstOrDefault(r => r.StudentId == _currentUserId);

                Button btnTake = new Button
                {
                    AutoSize = false,
                    Width = 120, // Tăng độ rộng để chứa đủ chữ "Đang chờ duyệt"
                    Height = 35,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    Anchor = AnchorStyles.Top | AnchorStyles.Right // Neo lề phải
                };
                // Căn phải như nút Quản lý
                btnTake.Location = new Point(card.Width - btnTake.Width - 15, 10);

                if (isWaiting)
                {
                    btnTake.Text = "⏳ Chưa mở";
                    btnTake.BackColor = Color.Gray; btnTake.ForeColor = Color.White;
                    btnTake.Enabled = false;
                }
                else if (myResult != null && myResult.CompletedAt != null)
                {
                    if (myResult.RetakeStatus == "Pending")
                    {
                        btnTake.Text = "Đang chờ duyệt";
                        btnTake.BackColor = Color.Orange; btnTake.ForeColor = Color.White;
                        btnTake.Enabled = false;
                    }
                    else if (myResult.RetakeStatus == "Approved")
                    {
                        btnTake.Text = "🔄 Làm lại ngay";
                        btnTake.BackColor = Color.Blue; btnTake.ForeColor = Color.White;
                        btnTake.Click += async (s, e) => { await OpenTakeExamForm(exam); };
                    }
                    else
                    {
                        btnTake.Text = "✋ Xin làm lại";
                        btnTake.BackColor = Color.Red; btnTake.ForeColor = Color.White;
                        btnTake.Click += async (s, e) => {
                            if (MessageBox.Show("Bạn có muốn gửi yêu cầu xin làm lại bài thi này cho giáo viên?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                await _examService.RequestRetakeAsync(exam.ExamId, _currentUserId);
                                MessageBox.Show("Đã gửi yêu cầu, vui lòng chờ giáo viên duyệt!");
                                await LoadExamsAsync();
                            }
                        };
                    }
                }
                else if (isExpired)
                {
                    btnTake.Text = "🛑 Đã đóng";
                    btnTake.BackColor = Color.DarkRed; btnTake.ForeColor = Color.White;
                    btnTake.Enabled = false;
                }
                else
                {
                    btnTake.Text = "🚀 Vào thi";
                    btnTake.BackColor = Color.FromArgb(76, 175, 80); btnTake.ForeColor = Color.White;
                    btnTake.Click += async (s, e) => { await OpenTakeExamForm(exam); };
                }

                card.Controls.Add(btnTake);
            }

            card.Height = currentY;
            return card;
        }
        private async Task OpenTakeExamForm(Exam exam)
        {
            try
            {
                var result = await _examService.StartExamAsync(exam.ExamId, _currentUserId);
                var antiCheatService = Program.ServiceProvider.GetRequiredService<AntiCheatService>();
                using (var frm = new FrmTakeExam(exam, result, _examService, antiCheatService))
                {
                    frm.ShowDialog(this.ParentForm);
                }
                await LoadExamsAsync();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        private async Task LoadStreamAsync()
        {
            // Đảm bảo không bị lỗi Cross-thread nếu hàm này bị gọi từ luồng khác
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(async () => await LoadStreamAsync()));
                return;
            }

            if (flpStream == null || _streamService == null) return;

            flpStream.Controls.Clear();
            // Hiện một thông báo đang tải (tùy chọn)
            flpStream.Controls.Add(new Label { Text = "Đang tải bảng tin...", AutoSize = true, Padding = new Padding(10) });

            try
            {
                var posts = await _streamService.GetPostsByClassIdAsync(_classId);

                flpStream.Controls.Clear(); // Xóa chữ "Đang tải"

                if (posts == null || posts.Count == 0)
                {
                    flpStream.Controls.Add(new Label { Text = "Chưa có thông báo nào trong lớp này.", AutoSize = true, Padding = new Padding(10), ForeColor = Color.Gray });
                    return;
                }

                foreach (var post in posts)
                {
                    string author = post.Author?.FullName ?? "Người dùng ẩn danh";
                    string content = post.Content ?? "";

                    var postCard = CreatePostCard(author, post.CreatedAt, content);
                    flpStream.Controls.Add(postCard);
                }

                // CỰC KỲ QUAN TRỌNG: Ép giao diện sắp xếp lại các bài đăng
                flpStream.PerformLayout();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải bảng tin: " + ex.Message);
            }
        }

        private Panel CreatePostCard(string author, DateTime time, string content)
        {
            Panel card = new Panel
            {
                Width = flpStream.Width - 30,
                AutoSize = true,
                Padding = new Padding(15),
                Margin = new Padding(5, 5, 5, 15),
                BackColor = Color.FromArgb(245, 245, 245),
                BorderStyle = BorderStyle.None
            };

            Label lblHeader = new Label { Text = $"{author} • {time:dd/MM HH:mm}", Font = new Font("Roboto", 9, FontStyle.Bold), AutoSize = true, Location = new Point(10, 10) };
            Label lblText = new Label { Text = content, Font = new Font("Roboto", 11), AutoSize = true, MaximumSize = new Size(card.Width - 20, 0), Location = new Point(10, 35) };

            card.Controls.Add(lblHeader);
            card.Controls.Add(lblText);
            return card;
        }

        private async void btnPost_Click(object sender, EventArgs e)
        {
            string content = txtPostContent.Text.Trim();
            if (string.IsNullOrEmpty(content)) return;

            await _streamService.CreatePostAsync(_classId, _currentUserId, content);
            txtPostContent.Clear();
            await LoadStreamAsync();
        }

        private async Task LoadAssignmentsAsync()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(async () => await LoadAssignmentsAsync()));
                return;
            }

            if (_assignmentService == null || flpAssignments == null) return;

            try
            {
                var user = _authService.CurrentUser;
                if (user == null) return;

                // --- BƯỚC 1: QUAN TRỌNG - PHÂN QUYỀN TRƯỚC ---
                // Phải ẩn/hiện bảng giao bài ngay lập tức cho dù có bài tập hay không
                if (_isTeacherOfThisClass)
                {
                    ConfigureTeacherAssignmentUI(); // Hiện bảng giao bài
                }
                else
                {
                    ConfigureStudentAssignmentUI(); // Ẩn bảng giao bài
                }

                // Sau đó mới đi lấy dữ liệu từ Database
                var assignments = await _assignmentService.GetAssignmentsByClassIdAsync(_classId);

                flpAssignments.Controls.Clear();

                // --- BƯỚC 2: KIỂM TRA TRỐNG SAU ---
                if (assignments == null || assignments.Count == 0)
                {
                    flpAssignments.Controls.Add(new Label
                    {
                        Text = "Chưa có bài tập nào.",
                        AutoSize = true,
                        Padding = new Padding(10),
                        ForeColor = Color.Gray
                    });
                    return; // Thoát ở đây là an toàn vì panel đã được ẩn ở Bước 1 rồi
                }

                // Hiển thị danh sách bài tập (giữ nguyên code cũ)
                foreach (var assignment in assignments)
                {
                    var assignmentCard = CreateAssignmentCard(assignment, _isTeacherOfThisClass);
                    flpAssignments.Controls.Add(assignmentCard);
                }

                flpAssignments.PerformLayout();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải bài tập: " + ex.Message);
            }
        }

        private void ConfigureTeacherAssignmentUI()
        {
            // Bản đồ: Thực hiện = Tạo bài tập
            btnActionAssignment.Text = "THÊM BÀI TẬP";
            panelCreateAssignment.Visible = true;
            lblAssignmentTitle.Text = "Khu vực giao bài";
            txtAssignmentName.Hint = "Tên bài tập...";
            txtDriveLink.Hint = "Link Google Drive (hoặc tài nguyên)...";
        }

        private void ConfigureStudentAssignmentUI()
        {
            // Học sinh chỉ xem, không tạo bài tập
            panelCreateAssignment.Visible = false;
        }

        private async Task LoadGradingAsync()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(async () => await LoadGradingAsync()));
                return;
            }

            if (_assignmentService == null || flpGrading == null) return;

            try
            {
                var user = _authService.CurrentUser;
                if (user == null) return;

                var assignments = await _assignmentService.GetAssignmentsByClassIdAsync(_classId);

                flpGrading.Controls.Clear();
                flpGrading.FlowDirection = FlowDirection.TopDown; // Xếp từ trên xuống
                flpGrading.WrapContents = false; // Không cho phép tràn sang hàng ngang

                if (_isTeacherOfThisClass)
                {
                    // View cho giáo viên: chấm điểm
                    flpGrading.Controls.Add(lblGradingTitle);

                    if (assignments == null || assignments.Count == 0)
                    {
                        flpGrading.Controls.Add(new Label
                        {
                            Text = "Chưa có bài tập nào.",
                            AutoSize = true,
                            Padding = new Padding(10),
                            ForeColor = Color.Gray,
                            Margin = new Padding(10)
                        });
                        flpGrading.PerformLayout();
                        return;
                    }

                    // Hiển thị từng bài tập với danh sách nộp bài
                    foreach (var assignment in assignments)
                    {
                        var gradingCard = CreateGradingCard(assignment);
                        flpGrading.Controls.Add(gradingCard);
                    }
                }
                else
                {
                    // View cho học sinh: xem điểm và phản hồi
                    Label lblStudentTitle = new Label
                    {
                        Text = "📊 Kết quả chấm điểm của bạn",
                        Font = new Font("Roboto", 14, FontStyle.Bold),
                        AutoSize = true,
                        Padding = new Padding(10),
                        Margin = new Padding(5, 5, 5, 15)
                    };
                    flpGrading.Controls.Add(lblStudentTitle);

                    if (assignments == null || assignments.Count == 0)
                    {
                        flpGrading.Controls.Add(new Label
                        {
                            Text = "Chưa có bài tập nào.",
                            AutoSize = true,
                            Padding = new Padding(10),
                            ForeColor = Color.Gray,
                            Margin = new Padding(10)
                        });
                        flpGrading.PerformLayout();
                        return;
                    }

                    // Hiển thị từng bài tập với điểm của học sinh
                    foreach (var assignment in assignments)
                    {
                        var studentGradeCard = CreateStudentGradeCard(assignment, user.UserId);
                        flpGrading.Controls.Add(studentGradeCard);
                    }
                }

                flpGrading.PerformLayout();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách chấm điểm: " + ex.Message);
            }
        }

        private Panel CreateGradingCard(OmniSight.Core.Entities.Assignment assignment)
        {
            // Lấy chiều rộng của UserControl, nếu quá nhỏ thì lấy mặc định 800
            int cardWidth = this.Width > 100 ? this.Width - 50 : 780;

            Panel card = new Panel
            {
                Width = cardWidth,
                Height = 150, // Cho một chiều cao mặc định
                AutoSize = true, // Để true nhưng phải kết hợp với FlowLayoutPanel đúng
                Padding = new Padding(15),
                Margin = new Padding(10, 10, 10, 15),
                BackColor = Color.FromArgb(240, 248, 255),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Đảm bảo card không bao giờ nhỏ hơn cardWidth
            card.MinimumSize = new Size(cardWidth, 50);

            // Tiêu đề bài tập
            Label lblTitle = new Label
            {
                Text = $"📋 {assignment.Title}",
                Font = new Font("Roboto", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(10, 10)
            };
            card.Controls.Add(lblTitle);

            int currentY = lblTitle.Bottom + 10;

            // Tổng số bài nộp
            int totalSubmissions = assignment.Submissions?.Count ?? 0;
            Label lblSubmissionCount = new Label
            {
                Text = $"Tổng nộp: {totalSubmissions} bài",
                Font = new Font("Roboto", 10),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(10, currentY)
            };
            card.Controls.Add(lblSubmissionCount);
            currentY = lblSubmissionCount.Bottom + 10;

            // Hiển thị danh sách người nộp
            if (assignment.Submissions == null || assignment.Submissions.Count == 0)
            {
                Label lblNoSubmission = new Label
                {
                    Text = "Chưa có ai nộp bài.",
                    Font = new Font("Roboto", 9, FontStyle.Italic),
                    ForeColor = Color.Gray,
                    AutoSize = true,
                    Location = new Point(10, currentY)
                };
                card.Controls.Add(lblNoSubmission);
                currentY = lblNoSubmission.Bottom + 10;
            }
            else
            {
                // Hiển thị danh sách các bài nộp dưới dạng bảng nhỏ
                foreach (var submission in assignment.Submissions)
                {
                    string studentName = submission.Student?.FullName ?? "Không xác định";
                    string statusText = submission.Score.HasValue ? $"✓ Đã chấm: {submission.Score}/10" : "⏳ Chưa chấm";
                    string statusColor = submission.Score.HasValue ? "Green" : "Orange";

                    var submissionPanel = new Panel
                    {
                        Width = card.Width - 40,
                        Height = 80,
                        Location = new Point(10, currentY),
                        BackColor = Color.White,
                        BorderStyle = BorderStyle.None,
                        Margin = new Padding(0, 0, 0, 5)
                    };

                    // Tên học sinh
                    Label lblStudent = new Label
                    {
                        Text = $"👤 {studentName}",
                        Font = new Font("Roboto", 10, FontStyle.Regular),
                        AutoSize = true,
                        Location = new Point(5, 5)
                    };
                    submissionPanel.Controls.Add(lblStudent);

                    // Trạng thái chấm điểm
                    Label lblStatus = new Label
                    {
                        Text = statusText,
                        Font = new Font("Roboto", 9),
                        ForeColor = submission.Score.HasValue ? Color.Green : Color.Orange,
                        AutoSize = true,
                        Location = new Point(5, lblStudent.Bottom + 3)
                    };
                    submissionPanel.Controls.Add(lblStatus);

                    // Nút mở link
                    Button btnOpenSubmission = new Button
                    {
                        Text = "📎 Xem",
                        AutoSize = false,
                        Width = 70,
                        Height = 28,
                        Location = new Point(submissionPanel.Width - 150, 5),
                        BackColor = Color.FromArgb(33, 150, 243),
                        ForeColor = Color.White,
                        Font = new Font("Roboto", 8),
                        FlatStyle = FlatStyle.Flat,
                        Cursor = Cursors.Hand
                    };
                    btnOpenSubmission.Click += (sender, e) => OpenLink(submission.FileOrLinkUrl);
                    submissionPanel.Controls.Add(btnOpenSubmission);

                    // Nút chấm điểm
                    Button btnGrade = new Button
                    {
                        Text = "⭐ Chấm",
                        AutoSize = false,
                        Width = 70,
                        Height = 28,
                        Location = new Point(submissionPanel.Width - 75, 5),
                        BackColor = Color.FromArgb(76, 175, 80),
                        ForeColor = Color.White,
                        Font = new Font("Roboto", 8),
                        FlatStyle = FlatStyle.Flat,
                        Cursor = Cursors.Hand
                    };
                    btnGrade.Click += (sender, e) => GradeSubmission(submission.SubmissionId);
                    submissionPanel.Controls.Add(btnGrade);

                    card.Controls.Add(submissionPanel);
                    currentY += 90;
                }
            }

            card.Height = currentY + 10;
            return card;
        }

        private Panel CreateStudentGradeCard(OmniSight.Core.Entities.Assignment assignment, int studentId)
        {
            Panel card = new Panel
            {
                Width = flpGrading.Width - 30,
                AutoSize = false,
                Padding = new Padding(15),
                Margin = new Padding(5, 5, 5, 15),
                BackColor = Color.FromArgb(245, 245, 245),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Tiêu đề bài tập
            Label lblTitle = new Label
            {
                Text = $"📝 {assignment.Title}",
                Font = new Font("Roboto", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(10, 10)
            };
            card.Controls.Add(lblTitle);

            int currentY = lblTitle.Bottom + 10;

            // Mô tả bài tập
            if (!string.IsNullOrEmpty(assignment.Description))
            {
                Label lblDescription = new Label
                {
                    Text = assignment.Description,
                    Font = new Font("Roboto", 9),
                    AutoSize = true,
                    MaximumSize = new Size(card.Width - 40, 0),
                    Location = new Point(10, currentY),
                    ForeColor = Color.DarkGray
                };
                card.Controls.Add(lblDescription);
                currentY = lblDescription.Bottom + 10;
            }

            // Hạn chót
            if (assignment.DueDate.HasValue)
            {
                string dueText = $"Hạn chót: {assignment.DueDate:dd/MM/yyyy HH:mm}";
                Label lblDueDate = new Label
                {
                    Text = dueText,
                    Font = new Font("Roboto", 9, FontStyle.Italic),
                    ForeColor = assignment.DueDate < DateTime.Now ? Color.Red : Color.Gray,
                    AutoSize = true,
                    Location = new Point(10, currentY)
                };
                card.Controls.Add(lblDueDate);
                currentY = lblDueDate.Bottom + 15;
            }

            // Tìm bài nộp của học sinh này
            var studentSubmission = assignment.Submissions?.FirstOrDefault(s => s.StudentId == studentId);

            if (studentSubmission == null)
            {
                // Chưa nộp bài
                Label lblNoSubmission = new Label
                {
                    Text = "❌ Bạn chưa nộp bài này",
                    Font = new Font("Roboto", 10, FontStyle.Italic),
                    ForeColor = Color.Red,
                    AutoSize = true,
                    Location = new Point(10, currentY)
                };
                card.Controls.Add(lblNoSubmission);
                currentY = lblNoSubmission.Bottom + 10;
            }
            else
            {
                // Đã nộp bài
                Label lblSubmitted = new Label
                {
                    Text = $"✅ Đã nộp: {studentSubmission.SubmittedAt:dd/MM/yyyy HH:mm}",
                    Font = new Font("Roboto", 10),
                    ForeColor = Color.Green,
                    AutoSize = true,
                    Location = new Point(10, currentY)
                };
                card.Controls.Add(lblSubmitted);
                currentY = lblSubmitted.Bottom + 10;

                // Nút xem bài nộp
                if (!string.IsNullOrEmpty(studentSubmission.FileOrLinkUrl))
                {
                    Button btnViewSubmission = new Button
                    {
                        Text = "📎 Xem bài nộp",
                        AutoSize = false,
                        Width = 120,
                        Height = 30,
                        Location = new Point(10, currentY),
                        BackColor = Color.FromArgb(33, 150, 243),
                        ForeColor = Color.White,
                        Font = new Font("Roboto", 9),
                        FlatStyle = FlatStyle.Flat,
                        Cursor = Cursors.Hand
                    };
                    btnViewSubmission.Click += (sender, e) => OpenLink(studentSubmission.FileOrLinkUrl);
                    card.Controls.Add(btnViewSubmission);
                    currentY += 40;
                }

                // Hiển thị điểm và phản hồi nếu đã chấm
                if (studentSubmission.Score.HasValue)
                {
                    currentY += 5;

                    // Panel điểm
                    Panel scorePanel = new Panel
                    {
                        Width = card.Width - 40,
                        Height = 60,
                        Location = new Point(10, currentY),
                        BackColor = studentSubmission.Score >= 7 ? Color.FromArgb(200, 230, 201) : Color.FromArgb(255, 243, 224),
                        BorderStyle = BorderStyle.FixedSingle
                    };

                    // Điểm
                    Label lblScore = new Label
                    {
                        Text = $"⭐ Điểm: {studentSubmission.Score}/10",
                        Font = new Font("Roboto", 14, FontStyle.Bold),
                        ForeColor = studentSubmission.Score >= 7 ? Color.Green : Color.Orange,
                        AutoSize = true,
                        Location = new Point(10, 8)
                    };
                    scorePanel.Controls.Add(lblScore);

                    // Xếp loại
                    string gradeLevel = GetGradeLevel(studentSubmission.Score.Value);
                    Label lblGradeLevel = new Label
                    {
                        Text = gradeLevel,
                        Font = new Font("Roboto", 10),
                        ForeColor = Color.Gray,
                        AutoSize = true,
                        Location = new Point(lblScore.Right + 20, 12)
                    };
                    scorePanel.Controls.Add(lblGradeLevel);

                    card.Controls.Add(scorePanel);
                    currentY += 70;

                    // Phản hồi nếu có
                    if (!string.IsNullOrEmpty(studentSubmission.Feedback))
                    {
                        Label lblFeedbackTitle = new Label
                        {
                            Text = "💬 Phản hồi từ giáo viên:",
                            Font = new Font("Roboto", 10, FontStyle.Bold),
                            AutoSize = true,
                            Location = new Point(10, currentY)
                        };
                        card.Controls.Add(lblFeedbackTitle);
                        currentY = lblFeedbackTitle.Bottom + 5;

                        Label lblFeedback = new Label
                        {
                            Text = studentSubmission.Feedback,
                            Font = new Font("Roboto", 9),
                            AutoSize = true,
                            MaximumSize = new Size(card.Width - 40, 0),
                            Location = new Point(10, currentY),
                            ForeColor = Color.DarkBlue,
                            BackColor = Color.FromArgb(230, 245, 255),
                            Padding = new Padding(10)
                        };
                        card.Controls.Add(lblFeedback);
                        currentY = lblFeedback.Bottom + 10;
                    }
                }
                else
                {
                    // Chưa được chấm
                    Label lblNotGraded = new Label
                    {
                        Text = "⏳ Bài của bạn chưa được chấm điểm",
                        Font = new Font("Roboto", 10, FontStyle.Italic),
                        ForeColor = Color.Orange,
                        AutoSize = true,
                        Location = new Point(10, currentY)
                    };
                    card.Controls.Add(lblNotGraded);
                    currentY = lblNotGraded.Bottom + 10;
                }
            }

            card.Height = currentY + 10;
            return card;
        }

        private string GetGradeLevel(float score)
        {
            if (score >= 9) return "(Xuất sắc)";
            if (score >= 8) return "(Tốt)";
            if (score >= 7) return "(Khá)";
            if (score >= 6) return "(Trung bình khá)";
            if (score >= 5) return "(Trung bình)";
            if (score >= 4) return "(Yếu)";
            return "(Kém)";
        }

        private void GradeSubmission(int submissionId)
        {
            // Mở dialog để nhập điểm và phản hồi
            Form gradeForm = new Form()
            {
                Text = "Chấm điểm bài tập",
                Width = 450,
                Height = 280,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            Label lblScore = new Label() { Left = 20, Top = 20, Text = "Điểm (0-10):", Width = 200 };
            NumericUpDown nudScore = new NumericUpDown()
            {
                Left = 150,
                Top = 20,
                Width = 100,
                Maximum = 10,
                Minimum = 0,
                DecimalPlaces = 1,
                Increment = 0.5m
            };

            Label lblFeedback = new Label() { Left = 20, Top = 70, Text = "Phản hồi (tùy chọn):", Width = 200 };
            TextBox txtFeedback = new TextBox()
            {
                Left = 20,
                Top = 100,
                Width = 400,
                Height = 80,
                Multiline = true
            };

            Button okButton = new Button()
            {
                Text = "Lưu",
                Left = 200,
                Width = 80,
                Top = 200,
                DialogResult = DialogResult.OK
            };
            Button cancelButton = new Button()
            {
                Text = "Hủy",
                Left = 290,
                Width = 80,
                Top = 200,
                DialogResult = DialogResult.Cancel
            };

            gradeForm.Controls.Add(lblScore);
            gradeForm.Controls.Add(nudScore);
            gradeForm.Controls.Add(lblFeedback);
            gradeForm.Controls.Add(txtFeedback);
            gradeForm.Controls.Add(okButton);
            gradeForm.Controls.Add(cancelButton);
            gradeForm.AcceptButton = okButton;
            gradeForm.CancelButton = cancelButton;

            if (gradeForm.ShowDialog() == DialogResult.OK)
            {
                SaveGrade(submissionId, (float)nudScore.Value, txtFeedback.Text);
            }
        }

        private async void SaveGrade(int submissionId, float score, string feedback)
        {
            try
            {
                await _assignmentService.GradeSubmissionAsync(submissionId, score, feedback);
                MessageBox.Show("Chấm điểm thành công!");
                await LoadGradingAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi chấm điểm: " + ex.Message);
            }
        }

        private Panel CreateAssignmentCard(OmniSight.Core.Entities.Assignment assignment, bool isTeacher)
        {
            Panel card = new Panel
            {
                Width = flpAssignments.Width - 60,
                AutoSize = false,
                Padding = new Padding(15),
                Margin = new Padding(5, 5, 5, 15),
                BackColor = Color.FromArgb(245, 245, 245),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Tiêu đề bài tập
            Label lblTitle = new Label
            {
                Text = assignment.Title ?? "Không có tiêu đề",
                Font = new Font("Roboto", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(10, 10)
            };

            // Mô tả
            Label lblDescription = new Label
            {
                Text = assignment.Description ?? "Không có mô tả",
                Font = new Font("Roboto", 10),
                AutoSize = true,
                MaximumSize = new Size(card.Width - 20, 0),
                Location = new Point(10, 35)
            };

            // Hạn chót
            string dueText = assignment.DueDate.HasValue ? $"Hạn chót: {assignment.DueDate:dd/MM/yyyy HH:mm}" : "Không có hạn chót";
            Label lblDueDate = new Label
            {
                Text = dueText,
                Font = new Font("Roboto", 9, FontStyle.Italic),
                ForeColor = assignment.DueDate.HasValue && assignment.DueDate < DateTime.Now ? Color.Red : Color.Gray,
                AutoSize = true,
                Location = new Point(10, lblDescription.Bottom + 10)
            };

            // Biến lưu trữ tọa độ Y hiện tại để vẽ tiếp các nút bấm
            int currentY = lblDueDate.Bottom + 15;

            // Nếu có link tài nguyên, thêm nút mở
            if (!string.IsNullOrEmpty(assignment.AttachmentUrl))
            {
                Button btnOpenLink = new Button
                {
                    Text = "📎 Mở tài nguyên",
                    AutoSize = false,
                    Width = 120,
                    Height = 30,
                    Location = new Point(10, currentY),
                    BackColor = Color.FromArgb(33, 150, 243),
                    ForeColor = Color.White,
                    Font = new Font("Roboto", 9, FontStyle.Regular),
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand
                };
                btnOpenLink.Click += (sender, e) => OpenLink(assignment.AttachmentUrl);
                card.Controls.Add(btnOpenLink);
                currentY += 40; // Tăng Y để nút dưới không đè lên
            }

            // Nếu là giáo viên, thêm nút xoá / sửa
            if (isTeacher)
            {
                Button btnEdit = new Button
                {
                    Text = "✏️ Sửa",
                    Width = 70,
                    Height = 30,
                    Location = new Point(card.Width - 180, lblDueDate.Top),
                    BackColor = Color.FromArgb(255, 193, 7), // Màu vàng
                    FlatStyle = FlatStyle.Flat
                };
                btnEdit.Click += (s, e) => {
                    _editingAssignmentId = assignment.AssignmentId;
                    txtAssignmentName.Text = assignment.Title;
                    txtAssignmentDescription.Text = assignment.Description;
                    txtDriveLink.Text = assignment.AttachmentUrl;
                    if (assignment.DueDate.HasValue) dtpDueDate.Value = assignment.DueDate.Value;
                    btnActionAssignment.Text = "LƯU THAY ĐỔI";
                    txtAssignmentName.Focus();
                };
                card.Controls.Add(btnEdit);

                Button btnDelete = new Button
                {
                    Text = "🗑️ Xoá",
                    AutoSize = false,
                    Width = 80,
                    Height = 30,
                    Location = new Point(card.Width - 100, lblDueDate.Top),
                    BackColor = Color.FromArgb(244, 67, 54),
                    ForeColor = Color.White,
                    Font = new Font("Roboto", 9, FontStyle.Regular),
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand
                };
                btnDelete.Click += async (sender, e) => await DeleteAssignment(assignment.AssignmentId);
                card.Controls.Add(btnDelete);
            }
            else // NHÁNH DÀNH CHO HỌC SINH
            {
                var mySubmission = assignment.Submissions?.FirstOrDefault(s => s.StudentId == _currentUserId);

                if (mySubmission != null)
                {
                    // ĐÃ NỘP BÀI
                    Label lblStatus = new Label
                    {
                        Text = $"✅ Đã nộp: {mySubmission.SubmittedAt:dd/MM HH:mm}",
                        ForeColor = Color.Green,
                        Font = new Font("Roboto", 9, FontStyle.Bold),
                        AutoSize = true,
                        Location = new Point(10, currentY)
                    };
                    card.Controls.Add(lblStatus);

                    Button btnResubmit = new Button
                    {
                        Text = "Nộp lại",
                        Size = new Size(80, 25),
                        Location = new Point(card.Width - 100, currentY - 5),
                        BackColor = Color.LightSalmon,
                        FlatStyle = FlatStyle.Flat,
                        Cursor = Cursors.Hand
                    };
                    btnResubmit.Click += (s, e) => SubmitAssignment(assignment.AssignmentId);
                    card.Controls.Add(btnResubmit);

                    currentY += 35; // Cộng thêm chiều cao cho khung
                }
                else
                {
                    // CHƯA NỘP BÀI
                    if (assignment.DueDate.HasValue && DateTime.Now > assignment.DueDate.Value)
                    {
                        // QUÁ HẠN
                        Label lblExpired = new Label
                        {
                            Text = "🔴 Đã hết hạn nộp bài",
                            ForeColor = Color.Red,
                            AutoSize = true,
                            Location = new Point(10, currentY)
                        };
                        card.Controls.Add(lblExpired);
                        currentY += 30; // Cộng thêm chiều cao
                    }
                    else
                    {
                        // CÒN HẠN -> HIỆN NÚT NỘP BÀI
                        Button btnSubmit = new Button
                        {
                            Text = "📤 NỘP BÀI",
                            Width = 120,
                            Height = 35,
                            Location = new Point(10, currentY),
                            BackColor = Color.FromArgb(76, 175, 80),
                            ForeColor = Color.White,
                            FlatStyle = FlatStyle.Flat,
                            Cursor = Cursors.Hand
                        };
                        btnSubmit.Click += (s, e) => SubmitAssignment(assignment.AssignmentId);
                        card.Controls.Add(btnSubmit);

                        currentY += 45; // ĐÂY LÀ DÒNG QUAN TRỌNG ĐỂ NÚT KHÔNG BỊ CHE MẤT
                    }
                }
            }

            card.Controls.Add(lblTitle);
            card.Controls.Add(lblDescription);
            card.Controls.Add(lblDueDate);

            // Gán chiều cao cuối cùng cho khung card
            card.Height = currentY + 10;

            return card;
        }

        

        private void OpenLink(string url)
        {
            try
            {
                if (!string.IsNullOrEmpty(url))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi mở link: " + ex.Message);
            }
        }

        private async Task DeleteAssignment(int assignmentId)
        {
            if (MessageBox.Show("Bạn chắc chắn muốn xoá bài tập này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    await _assignmentService.DeleteAssignmentAsync(assignmentId);
                    await LoadAssignmentsAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xoá bài tập: " + ex.Message);
                }
            }
        }

        private void SubmitAssignment(int assignmentId)
        {
            string submissionLink = PromptForSubmission();

            // Kiểm tra nếu người dùng không nhập gì hoặc bấm Hủy
            if (string.IsNullOrWhiteSpace(submissionLink)) return;

            SubmitAssignmentAsync(assignmentId, submissionLink);
        }

        private async void SubmitAssignmentAsync(int assignmentId, string fileOrLinkUrl)
        {
            try
            {
                // Gọi Service lưu vào Database
                await _assignmentService.SubmitAssignmentAsync(assignmentId, _currentUserId, fileOrLinkUrl);

                MessageBox.Show("Nộp bài thành công!", "Thông báo");

                // QUAN TRỌNG: Tải lại danh sách để cập nhật trạng thái "Đã nộp" trên màn hình
                await LoadAssignmentsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nộp bài: " + ex.Message);
            }
        }

        private string PromptForSubmission()
        {
            // Tạo form nhỏ để nhập link nộp bài
            Form prompt = new Form()
            {
                Text = "Nộp bài tập",
                Width = 450,
                Height = 200,
                StartPosition = FormStartPosition.CenterParent, // Sẽ nằm giữa MainForm
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.White
            };

            Label label = new Label() { Left = 20, Top = 20, Text = "Dán link bài làm (Google Drive/URL):", Width = 350, Font = new Font("Roboto", 10) };
            TextBox textBox = new TextBox() { Left = 20, Top = 50, Width = 390, Font = new Font("Roboto", 10) };

            Button okButton = new Button()
            {
                Text = "NỘP BÀI",
                Left = 220,
                Width = 100,
                Top = 100,
                Height = 40,
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };

            Button cancelButton = new Button()
            {
                Text = "HỦY",
                Left = 330,
                Width = 80,
                Top = 100,
                Height = 40,
                DialogResult = DialogResult.Cancel
            };

            prompt.Controls.AddRange(new Control[] { label, textBox, okButton, cancelButton });
            prompt.AcceptButton = okButton;

            // QUAN TRỌNG: Truyền 'this' vào ShowDialog để nó bắt đúng Parent
            return prompt.ShowDialog(this) == DialogResult.OK ? textBox.Text.Trim() : null;
        }

        private async void btnActionAssignment_Click(object sender, EventArgs e)
        {
            if (!_isTeacherOfThisClass) return;

            string title = txtAssignmentName.Text.Trim();
            if (string.IsNullOrEmpty(title))
            {
                MessageBox.Show("Vui lòng nhập tên bài tập!"); return;
            }

            try
            {
                DateTime? dueDate = dtpDueDate.Value;

                if (_editingAssignmentId == null)
                {
                    // CHẾ ĐỘ THÊM MỚI
                    await _assignmentService.CreateAssignmentAsync(_classId, _currentUserId, title,
                        txtAssignmentDescription.Text, txtDriveLink.Text, dueDate);
                    MessageBox.Show("Thêm bài tập thành công!");
                }
                else
                {
                    // CHẾ ĐỘ CẬP NHẬT (Bạn cần thêm hàm Update này vào AssignmentService)
                    await _assignmentService.UpdateAssignmentAsync(_editingAssignmentId.Value, title,
                        txtAssignmentDescription.Text, txtDriveLink.Text, dueDate);
                    MessageBox.Show("Cập nhật bài tập thành công!");
                }

                // Reset form
                _editingAssignmentId = null;
                btnActionAssignment.Text = "THÊM BÀI TẬP";
                txtAssignmentName.Clear();
                txtAssignmentDescription.Clear();
                txtDriveLink.Clear();

                await LoadAssignmentsAsync(); // Tải lại danh sách
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private async Task LoadMembersAsync()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(async () => await LoadMembersAsync()));
                return;
            }

            if (_classService == null || flpMembers == null) return;

            try
            {
                flpMembers.Controls.Clear();

                // Add title
                Label lblTitle = new Label
                {
                    Text = "👥 Danh sách thành viên lớp học",
                    Font = new Font("Roboto", 14, FontStyle.Bold),
                    AutoSize = true,
                    Padding = new Padding(10),
                    Margin = new Padding(5, 5, 5, 15)
                };
                flpMembers.Controls.Add(lblTitle);

                var members = await _classService.GetClassMembersAsync(_classId);

                if (members == null || members.Count == 0)
                {
                    flpMembers.Controls.Add(new Label
                    {
                        Text = "Chưa có thành viên nào trong lớp này.",
                        AutoSize = true,
                        Padding = new Padding(10),
                        ForeColor = Color.Gray
                    });
                    flpMembers.PerformLayout();
                    return;
                }

                // Display members
                foreach (var member in members)
                {
                    var memberCard = CreateMemberCard(member);
                    flpMembers.Controls.Add(memberCard);
                }

                flpMembers.PerformLayout();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách thành viên: " + ex.Message);
            }
        }

        private Panel CreateMemberCard(OmniSight.Core.Entities.ClassMember member)
        {
            Panel card = new Panel
            {
                Width = flpMembers.Width - 30,
                AutoSize = false,
                Padding = new Padding(15),
                Margin = new Padding(5, 5, 5, 15),
                BackColor = Color.FromArgb(245, 245, 245),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Student name
            string studentName = member.Student?.FullName ?? "Không xác định";
            Label lblName = new Label
            {
                Text = $"👤 {studentName}",
                Font = new Font("Roboto", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(10, 10)
            };
            card.Controls.Add(lblName);

            int currentY = lblName.Bottom + 10;

            // Email
            string email = member.Student?.Email ?? "Không có email";
            Label lblEmail = new Label
            {
                Text = $"📧 {email}",
                Font = new Font("Roboto", 9),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(10, currentY)
            };
            card.Controls.Add(lblEmail);
            currentY = lblEmail.Bottom + 10;

            // Join date
            Label lblJoinDate = new Label
            {
                Text = $"📅 Tham gia: {member.JoinedAt:dd/MM/yyyy}",
                Font = new Font("Roboto", 9),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(10, currentY)
            };
            card.Controls.Add(lblJoinDate);
            currentY = lblJoinDate.Bottom + 10;

            card.Height = currentY + 5;
            return card;
        }

        // --- SỰ KIỆN 1: TẢI FILE MẪU ---
        private void btnDownloadTemplate_Click(object sender, EventArgs e)
        {
            // Dùng using để giải phóng RAM ngay khi tắt hộp thoại
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Word Document|*.docx";
                sfd.FileName = "Mau_De_Thi_OmniSight.docx";
                sfd.Title = "Chọn nơi lưu file mẫu";

                // QUAN TRỌNG: Truyền this.ParentForm vào để tránh bị treo (Not Responding)
                if (sfd.ShowDialog(this.ParentForm) == DialogResult.OK)
                {
                    try
                    {
                        using (var doc = DocX.Create(sfd.FileName))
                        {
                            doc.InsertParagraph("MẪU ĐỀ THI OMNISIGHT").Bold().FontSize(16d).Alignment = Xceed.Document.NET.Alignment.center;
                            doc.InsertParagraph("\n(Lưu ý: Không đổi định dạng các chữ 'Câu x:', 'A.', 'B.', 'C.', 'D.', 'Đáp án:')\n").Italic();

                            doc.InsertParagraph("Câu 1: Thủ đô của Việt Nam là gì?");
                            doc.InsertParagraph("A. Đà Nẵng");
                            doc.InsertParagraph("B. Hà Nội");
                            doc.InsertParagraph("C. Hồ Chí Minh");
                            doc.InsertParagraph("D. Cần Thơ");
                            doc.InsertParagraph("Đáp án: B\n");

                            doc.InsertParagraph("Câu 2: Trái đất có hình gì?");
                            doc.InsertParagraph("A. Hình vuông");
                            doc.InsertParagraph("B. Hình tam giác");
                            doc.InsertParagraph("C. Hình cầu");
                            doc.InsertParagraph("D. Hình nón");
                            doc.InsertParagraph("Đáp án: C");

                            doc.Save();
                        }
                        MessageBox.Show(this.ParentForm, "Đã tải file mẫu thành công! Hãy mở file lên để xem định dạng.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this.ParentForm, "Lỗi tạo file Word: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // --- SỰ KIỆN 2: NHẬP FILE WORD ---
        private async void btnImportWord_Click(object sender, EventArgs e)
        {
            if (_examService == null)
            {
                MessageBox.Show(this.ParentForm, "Dịch vụ bài thi chưa được khởi tạo!", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Word Document|*.docx";
                ofd.Title = "Chọn đề thi Word cần nhập";

                // QUAN TRỌNG: Truyền this.ParentForm vào để tránh bị treo
                if (ofd.ShowDialog(this.ParentForm) == DialogResult.OK)
                {
                    // Hiện popup nhỏ để hỏi Tên đề thi và Thời gian
                    var (title, duration) = PromptForExamDetails();

                    if (string.IsNullOrEmpty(title)) return; // Người dùng bấm Hủy

                    try
                    {
                        btnImportWord.Text = "ĐANG XỬ LÝ...";
                        btnImportWord.Enabled = false;

                        var exam = await _examService.ImportExamFromWordAsync(_classId, title, duration, ofd.FileName);

                        MessageBox.Show(this.ParentForm, $"Nhập đề thi thành công!\nĐã trích xuất được {exam.Questions.Count} câu hỏi.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        await LoadExamsAsync();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this.ParentForm, "Lỗi khi đọc file Word: Định dạng không đúng hoặc file đang bị mở bởi phần mềm khác.\n\nChi tiết: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        btnImportWord.Text = "NHẬP ĐỀ TỪ WORD";
                        btnImportWord.Enabled = true;
                    }
                }
            }
        }

        // Hàm hỗ trợ: Hiển thị popup hỏi tên đề thi và thời gian
        private (string Title, int Duration) PromptForExamDetails()
        {
            using (Form prompt = new Form())
            {
                prompt.Width = 400;
                prompt.Height = 200;
                prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
                prompt.Text = "Thông tin đề thi";
                prompt.StartPosition = FormStartPosition.CenterParent;
                prompt.MaximizeBox = false;
                prompt.MinimizeBox = false;

                Label textLabel = new Label() { Left = 20, Top = 20, Text = "Tên đề thi:" };
                TextBox inputBox = new TextBox() { Left = 20, Top = 45, Width = 340 };

                Label timeLabel = new Label() { Left = 20, Top = 80, Text = "Thời gian làm bài (phút):" };
                NumericUpDown timeBox = new NumericUpDown() { Left = 180, Top = 78, Width = 80, Minimum = 5, Maximum = 180, Value = 45 };

                Button confirmation = new Button() { Text = "Xác nhận", Left = 260, Width = 100, Top = 120, DialogResult = DialogResult.OK };

                prompt.Controls.Add(textLabel);
                prompt.Controls.Add(inputBox);
                prompt.Controls.Add(timeLabel);
                prompt.Controls.Add(timeBox);
                prompt.Controls.Add(confirmation);
                prompt.AcceptButton = confirmation;

                // QUAN TRỌNG: Truyền this.ParentForm vào để tránh bị treo
                if (prompt.ShowDialog(this.ParentForm) == DialogResult.OK)
                {
                    return (inputBox.Text.Trim(), (int)timeBox.Value);
                }

                return (null, 0);
            }
        }
        private async void btnManageExams_Click(object sender, EventArgs e)
        {
            try
            {
                // Sử dụng các Service đã được tiêm (Inject) vào UcClassDetail từ trước
                using (var frm = new FrmExamManagement(_examService, _classService, _currentUserId))
                {
                    // Mở form dưới dạng Dialog
                    if (frm.ShowDialog(this.ParentForm) == DialogResult.OK)
                    {
                        // Nếu trong lúc quản lý có Xóa/Sửa đề, khi đóng lại sẽ load lại danh sách ở ngoài
                        await LoadExamsAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi mở trình quản lý: " + ex.Message);
            }
        }
    }
}
