using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin.Controls;
using OmniSight.Services;

namespace OmniSight.UI.Forms
{
    public partial class UcClassDetail : UserControl
    {
        private readonly StreamService _streamService;
        private readonly ClassService _classService;
        private readonly AuthService _authService;
        private AssignmentService _assignmentService;
        private readonly int _classId;
        private readonly int _currentUserId;

        public UcClassDetail(StreamService streamService, ClassService classService, AuthService authService, int currentUserId, int classId)
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
        }

        // Setter cho AssignmentService (gọi từ MainForm)
        public void SetAssignmentService(AssignmentService assignmentService)
        {
            _assignmentService = assignmentService;
        }

        private bool _streamLoaded = false;
        private bool _assignmentLoaded = false;
        private bool _gradingLoaded = false;
        private bool _membersLoaded = false;

        private async void UcClassDetail_VisibleChanged(object sender, EventArgs e)
        {
            // Chỉ tải một lần duy nhất khi UserControl trở nên hiển thị
            if (this.Visible && !_streamLoaded)
            {
                _streamLoaded = true;
                await LoadStreamAsync();
                await LoadAssignmentsAsync();
                await LoadGradingAsync();
                await LoadMembersAsync();
            }
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

                var assignments = await _assignmentService.GetAssignmentsByClassIdAsync(_classId);

                flpAssignments.Controls.Clear();

                if (assignments == null || assignments.Count == 0)
                {
                    flpAssignments.Controls.Add(new Label
                    {
                        Text = "Chưa có bài tập nào.",
                        AutoSize = true,
                        Padding = new Padding(10),
                        ForeColor = Color.Gray
                    });
                    return;
                }

                // Nếu là giáo viên, cho phép tạo bài tập
                if (user.IsTeacher)
                {
                    ConfigureTeacherAssignmentUI();
                }
                else
                {
                    ConfigureStudentAssignmentUI();
                }

                // Hiển thị danh sách bài tập
                foreach (var assignment in assignments)
                {
                    var assignmentCard = CreateAssignmentCard(assignment, user.IsTeacher);
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

                if (user.IsTeacher)
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
            Panel card = new Panel
            {
                Width = flpGrading.Width - 30,
                AutoSize = true,
                Padding = new Padding(15),
                Margin = new Padding(5, 5, 5, 15),
                BackColor = Color.FromArgb(240, 248, 255),
                BorderStyle = BorderStyle.FixedSingle
            };

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
                AutoSize = true,
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
                Width = flpAssignments.Width - 30,
                AutoSize = true,
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
            string dueText = assignment.DueDate.HasValue ? $"Hạn chót: {assignment.DueDate:dd/MM/yyyy}" : "Không có hạn chót";
            Label lblDueDate = new Label
            {
                Text = dueText,
                Font = new Font("Roboto", 9, FontStyle.Italic),
                ForeColor = assignment.DueDate.HasValue && assignment.DueDate < DateTime.Now ? Color.Red : Color.Gray,
                AutoSize = true,
                Location = new Point(10, lblDescription.Bottom + 10)
            };

            int controlHeight = lblDueDate.Bottom + 15;

            // Nếu có link tài nguyên, thêm nút mở
            if (!string.IsNullOrEmpty(assignment.AttachmentUrl))
            {
                Button btnOpenLink = new Button
                {
                    Text = "📎 Mở tài nguyên",
                    AutoSize = false,
                    Width = 120,
                    Height = 30,
                    Location = new Point(10, controlHeight),
                    BackColor = Color.FromArgb(33, 150, 243),
                    ForeColor = Color.White,
                    Font = new Font("Roboto", 9, FontStyle.Regular),
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand
                };
                btnOpenLink.Click += (sender, e) => OpenLink(assignment.AttachmentUrl);
                card.Controls.Add(btnOpenLink);
                controlHeight += 40;
            }

            // Nếu là giáo viên, thêm nút xoá
            if (isTeacher)
            {
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
            else
            {
                // Học sinh có thể nộp bài
                Button btnSubmit = new Button
                {
                    Text = "📤 Nộp bài",
                    AutoSize = false,
                    Width = 100,
                    Height = 30,
                    Location = new Point(10, controlHeight),
                    BackColor = Color.FromArgb(76, 175, 80),
                    ForeColor = Color.White,
                    Font = new Font("Roboto", 9, FontStyle.Regular),
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand
                };
                btnSubmit.Click += (sender, e) => SubmitAssignment(assignment.AssignmentId);
                card.Controls.Add(btnSubmit);
                controlHeight += 40;
            }

            card.Controls.Add(lblTitle);
            card.Controls.Add(lblDescription);
            card.Controls.Add(lblDueDate);
            card.Height = controlHeight;

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
            // Mở dialog nhập link nộp bài
            string submissionLink = PromptForSubmission();
            if (!string.IsNullOrEmpty(submissionLink))
            {
                SubmitAssignmentAsync(assignmentId, submissionLink);
            }
        }

        private async void SubmitAssignmentAsync(int assignmentId, string fileOrLinkUrl)
        {
            try
            {
                await _assignmentService.SubmitAssignmentAsync(assignmentId, _currentUserId, fileOrLinkUrl);
                MessageBox.Show("Nộp bài thành công!");
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
                Width = 400,
                Height = 150,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            Label label = new Label() { Left = 20, Top = 20, Text = "Nhập link Google Drive (hoặc URL):", Width = 350 };
            TextBox textBox = new TextBox() { Left = 20, Top = 50, Width = 350 };
            Button okButton = new Button() { Text = "OK", Left = 200, Width = 80, Top = 90, DialogResult = DialogResult.OK };
            Button cancelButton = new Button() { Text = "Hủy", Left = 290, Width = 80, Top = 90, DialogResult = DialogResult.Cancel };

            prompt.Controls.Add(label);
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(okButton);
            prompt.Controls.Add(cancelButton);
            prompt.AcceptButton = okButton;
            prompt.CancelButton = cancelButton;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : null;
        }

        private async void btnActionAssignment_Click(object sender, EventArgs e)
        {
            var user = _authService.CurrentUser;
            if (user == null || !user.IsTeacher) return;

            string title = txtAssignmentName.Text.Trim();
            string description = txtAssignmentDescription.Text.Trim();
            string attachmentUrl = txtDriveLink.Text.Trim();
            DateTime? dueDate = dtpDueDate.Checked ? (DateTime?)dtpDueDate.Value : null;

            if (string.IsNullOrEmpty(title))
            {
                MessageBox.Show("Vui lòng nhập tên bài tập!");
                return;
            }

            try
            {
                await _assignmentService.CreateAssignmentAsync(_classId, _currentUserId, title, description, attachmentUrl, dueDate);
                MessageBox.Show("Thêm bài tập thành công!");
                txtAssignmentName.Clear();
                txtAssignmentDescription.Clear();
                txtDriveLink.Clear();
                dtpDueDate.Checked = false;
                await LoadAssignmentsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thêm bài tập: " + ex.Message);
            }
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
                AutoSize = true,
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

        private void materialTabSelector1_Click(object sender, EventArgs e)
        {

        }
    }
}
