using Emgu.CV;
using Emgu.CV.Structure;
using MaterialSkin;
using MaterialSkin.Controls;
using Microsoft.Extensions.DependencyInjection;
using OmniSight.Services;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace OmniSight.UI.Forms
{
    public partial class MainForm : MaterialForm
    {
        private readonly FaceAiService _faceAiService;
        private readonly AuthService _authService;
        private readonly IUserService _userService;
        private readonly IServiceProvider _serviceProvider;
        private bool _isLoggingOut = false;
        private readonly List<(string DisplayName, string JoinCode, int ClassId)> _allClassItems = new();
        private TextBox? _txtClassSearch;
        private float _currentZoom = 1.0f;
        private const float MIN_ZOOM = 0.7f;
        private const float MAX_ZOOM = 2.0f;
        private const float ZOOM_STEP = 0.1f;
        public bool IsFirstTimeLogin { get; set; }
        public MainForm(AuthService authService, IUserService userService, IServiceProvider serviceProvider, FaceAiService faceAiService)
        {
            InitializeComponent();
            _authService = authService;
            _userService = userService;
            _serviceProvider = serviceProvider;
            _faceAiService = faceAiService;
            this.Shown += MainForm_Shown;
            this.Activated += MainForm_Activated;
            // X�"A LoadClassList() khỏi constructor - gọi sau khi form load xong

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);

            // Load cài �'ặt từ file
            var uiSettings = LoadUiSettings();
            materialSkinManager.Theme = uiSettings.isDark ? MaterialSkinManager.Themes.DARK : MaterialSkinManager.Themes.LIGHT;
            ApplyColorByIndex(uiSettings.colorIndex);
            ConfigureClassListView();
        
        }   

        private void ConfigureClassListView()
        {
            lvwClasses.OwnerDraw = true;
            lvwClasses.FullRowSelect = true;
            lvwClasses.HideSelection = false;
            lvwClasses.GridLines = false;
            lvwClasses.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lvwClasses.Font = new Font("Segoe UI", 11f, FontStyle.Regular);

            // Increase row height for a cleaner, less cramped table look.
            lvwClasses.SmallImageList = new ImageList { ImageSize = new Size(1, 36) };

            var dbProp = typeof(Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            dbProp?.SetValue(lvwClasses, true, null);

            lvwClasses.DrawColumnHeader += lvwClasses_DrawColumnHeader;
            lvwClasses.DrawItem += lvwClasses_DrawItem;
            lvwClasses.DrawSubItem += lvwClasses_DrawSubItem;
            lvwClasses.Resize += (s, e) => UpdateClassListColumnWidths();
            tabClasses.Resize += (s, e) => UpdateClassListColumnWidths();

            SetupClassSearchBox();
            UpdateClassListColumnWidths();
        }

        private void SetupClassSearchBox()
        {
            if (_txtClassSearch != null) return;

            var searchPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 56,
                Padding = new Padding(0, 10, 0, 10),
                BackColor = Color.Transparent,
                Name = "pnlClassSearch"
            };

            _txtClassSearch = new TextBox
            {
                Name = "txtClassSearch",
                PlaceholderText = "Tìm lớp theo tên hoặc mã tham gia...",
                Font = new Font("Segoe UI", 10.5f, FontStyle.Regular),
                BorderStyle = BorderStyle.FixedSingle,
                Width = 420,
                Height = 34,
                Location = new Point(0, 10)
            };
            _txtClassSearch.TextChanged += (s, e) => ApplyClassListFilter();

            searchPanel.Controls.Add(_txtClassSearch);
            tabClasses.Controls.Add(searchPanel);
            tabClasses.Controls.SetChildIndex(searchPanel, 1);
        }

        private void UpdateClassListColumnWidths()
        {
            if (lvwClasses.Columns.Count < 2) return;

            var width = Math.Max(300, lvwClasses.ClientSize.Width - 4);
            var joinCodeWidth = 220;
            var classNameWidth = Math.Max(260, width - joinCodeWidth);

            lvwClasses.Columns[0].Width = classNameWidth;
            lvwClasses.Columns[1].Width = joinCodeWidth;
        }

        private void lvwClasses_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            using var bg = new SolidBrush(Color.FromArgb(245, 247, 250));
            using var line = new Pen(Color.FromArgb(224, 229, 236));
            e.Graphics.FillRectangle(bg, e.Bounds);
            e.Graphics.DrawLine(line, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);

            var textRect = new Rectangle(e.Bounds.Left + 10, e.Bounds.Top, e.Bounds.Width - 12, e.Bounds.Height);
            TextRenderer.DrawText(
                e.Graphics,
                e.Header.Text,
                new Font("Segoe UI Semibold", 10.5f),
                textRect,
                Color.FromArgb(66, 66, 66),
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        private void lvwClasses_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            // DrawSubItem handles the full row drawing.
        }

        private void lvwClasses_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            var rowColor = Color.White;

            using var bg = new SolidBrush(rowColor);
            using var grid = new Pen(Color.FromArgb(236, 239, 244));
            e.Graphics.FillRectangle(bg, e.Bounds);
            e.Graphics.DrawLine(grid, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);

            var textColor = Color.FromArgb(33, 33, 33);
            var textRect = new Rectangle(e.Bounds.Left + 10, e.Bounds.Top, e.Bounds.Width - 14, e.Bounds.Height);
            TextRenderer.DrawText(
                e.Graphics,
                e.SubItem.Text,
                new Font("Segoe UI", 10.5f, FontStyle.Regular),
                textRect,
                textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        private void ApplyClassListFilter()
        {
            var keyword = _txtClassSearch?.Text?.Trim();
            var source = _allClassItems.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                source = source.Where(i =>
                    i.DisplayName.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                    || i.JoinCode.Contains(keyword, StringComparison.OrdinalIgnoreCase));
            }

            lvwClasses.BeginUpdate();
            lvwClasses.Items.Clear();

            foreach (var item in source)
            {
                var row = new ListViewItem(item.DisplayName);
                row.SubItems.Add(item.JoinCode);
                row.Tag = item.ClassId;
                lvwClasses.Items.Add(row);
            }

            lvwClasses.EndUpdate();
        }

        private void btnOpenExamManager_Click(object sender, EventArgs e)
        {
            // 1. Ki�fm tra quyền giáo viên trư�>c
            var user = _authService.CurrentUser;
            if (user == null) return;

            if (!user.IsTeacher)
            {
                MessageBox.Show("Chỉ giáo viên mới có quyền tạo đề thi.", "Truy cập bị từ chối",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Lấy services cần thiết
            var examService = _serviceProvider.GetRequiredService<ExamService>();
            var classService = _serviceProvider.GetRequiredService<ClassService>();

            // 3. Hi�fn th�< Form quản lý bài thi
            using (var frm = new FrmExamManagement(examService, classService, user.UserId))
            {
                frm.ShowDialog(this);
            }
        }
        public bool StartAtProfile { get; set; } = false;
        private void MainForm_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(">> [MAIN_LOAD] B1: Thiết lập UI ban đầu");

            this.DrawerTabControl = this.materialTabControl1;
            this.KeyPreview = true;
            this.KeyDown += MainForm_KeyDown;

            var user = _authService.CurrentUser;

            // 1. Nếu là �'�fng nhập lần �'ầu, ch�? chuy�fn Tab (không hi�?n MessageBox �Y �'ây)
            if (StartAtProfile)
            {
                materialTabControl1.SelectedTab = tabProfile;
            }

            // 2. Đ�. dữ li�?u User lên giao di�?n
            if (user != null)
            {
                lblHomeWelcome.Text = $"Chào mừng {user.FullName} đến với OmniSight!";
                btnUserAccount.Text = user.FullName.ToUpper();
                txtFullName.Text = user.FullName ?? "";
                txtPhone.Text = user.Phone ?? "";

                SetupHomeTab();
                SetupSettingsTab();

                switchStudent.Checked = user.IsStudent;
                switchTeacher.Checked = user.IsTeacher;
            }

            // 3. Chạy các tác vụ ngầm (Dashboard, AI) mà không chặn UI
            BeginInvoke(new Action(() =>
            {
                _ = Task.Run(() => LoadDashboardDataAsync());

                _ = Task.Run(() =>
                {
                    try
                    {
                        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                        string detector = Path.Combine(baseDir, "Models", "face_detection_yunet_2023mar.onnx");
                        string recognizer = Path.Combine(baseDir, "Models", "face_recognition_sface_2021dec.onnx");
                        _faceAiService.InitializeModels(detector, recognizer);
                    }
                    catch (Exception ex) { System.Diagnostics.Debug.WriteLine("Lỗi AI: " + ex.Message); }
                });

                _ = LoadClassList();
            }));
        }

        // H�?M N�?Y SẼ CHẠY SAU KHI FORM HI�?N L�SN HO�?N TO�?N
        private void MainForm_Shown(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(">> [MAIN_SHOWN] Form đã hiển thị xong");

            if (StartAtProfile)
            {
                // Lúc này Form �'ã vẽ xong hoàn toàn, gọi MessageBox sẽ không b�< treo app
                MessageBox.Show(this,
                    "Chào mừng thành viên mới!\n\nBạn hãy dành ít giây để:\n1. Chọn vai trò.\n2. Cài đặt Face ID.",
                    "Hoàn tất hồ sơ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            if (!IsHandleCreated) return;
            BeginInvoke(new MethodInvoker(NormalizeDashboardCardsAppearance));
        }

        private void NormalizeDashboardCardsAppearance()
        {
            if (flpDashboardCards == null) return;

            foreach (Panel card in flpDashboardCards.Controls.OfType<Panel>())
            {
                if (card.Tag is Color originalColor)
                {
                    card.BackColor = originalColor;
                }

                foreach (Label label in card.Controls.OfType<Label>())
                {
                    if ((label.Tag as string) == "dashboard-title")
                    {
                        label.Font = new Font("Segoe UI", 11, FontStyle.Bold);
                    }
                    else if ((label.Tag as string) == "dashboard-value")
                    {
                        label.Font = new Font("Segoe UI", 32, FontStyle.Bold);
                    }

                    label.ForeColor = Color.White;
                    label.BackColor = Color.Transparent;
                    label.AutoSize = true;
                }

                foreach (PictureBox pictureBox in card.Controls.OfType<PictureBox>())
                {
                    pictureBox.BackColor = Color.Transparent;
                }

                card.Invalidate();
            }
        }

        private void btnUserAccount_Click(object sender, EventArgs e)
        {
            cmsUserMenu.Show(btnUserAccount, new Point(0, btnUserAccount.Height));
        }

        private void tsmLogout_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                _isLoggingOut = true;
                _authService.Logout();
                Application.Restart();
                Environment.Exit(0);
            }
        }

        private async void btnSaveProfile_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text)) return;
            var user = _authService.CurrentUser;
            if (user == null) return;

            bool success = await _userService.UpdateProfileAsync(user.UserId, txtFullName.Text, txtPhone.Text, switchStudent.Checked, switchTeacher.Checked);

            if (success)
            {
                // Reload user from DB to ensure switches and other fields reflect persisted state
                var updatedUser = await _userService.GetUserByIdAsync(user.UserId);
                if (updatedUser != null)
                {
                    _authService.SetCurrentUser(updatedUser);
                    btnUserAccount.Text = (updatedUser.FullName ?? string.Empty).ToUpper();

                    // Update UI fields to reflect saved state
                    txtFullName.Text = updatedUser.FullName ?? string.Empty;
                    txtPhone.Text = updatedUser.Phone ?? string.Empty;
                    switchStudent.Checked = updatedUser.IsStudent;
                    switchTeacher.Checked = updatedUser.IsTeacher;

                    MessageBox.Show("Cập nhật thành công!");
                }
                else
                {
                    // Fallback: update minimal UI
                    user.FullName = txtFullName.Text;
                    btnUserAccount.Text = txtFullName.Text.ToUpper();
                    MessageBox.Show("Cập nhật thành công! (Không thể tải lại dữ liệu người dùng)");
                }
            }
        }

        private void btnStartCamera_Click(object sender, EventArgs e)
        {
            if (_faceAiService.StartCamera(0))
            {
                timerCamera.Start();
                btnStartCamera.Enabled = false;
            }
            else MessageBox.Show("Không thể mở camera!");
        }

        private void timerCamera_Tick(object sender, EventArgs e)
        {
            using (var frame = _faceAiService.GetFrame())
            {
                if (frame != null && !frame.IsEmpty)
                {
                    var oldImage = picCamera.Image;
                    picCamera.Image = frame.ToBitmap();
                    oldImage?.Dispose();
                }
            }
        }

        private async void btnCaptureFace_Click(object sender, EventArgs e)
        {
            using (var frame = _faceAiService.GetFrame())
            {
                if (frame == null || frame.IsEmpty) return;

                var embedding = _faceAiService.ExtractEmbedding(frame);
                if (embedding != null)
                {
                    string embStr = string.Join(";", embedding.Select(f => f.ToString(CultureInfo.InvariantCulture)));
                    var user = _authService.CurrentUser;
                    if (user != null)
                    {
                        await _userService.UpdateFaceEmbeddingAsync(user.UserId, embStr);
                        user.FaceEmbedding = embStr;
                        MessageBox.Show("Thiết lập Face ID thành công!");
                    }

                    timerCamera.Stop();
                    _faceAiService.StopCamera();
                    picCamera.Image?.Dispose();
                    picCamera.Image = null;
                    btnStartCamera.Enabled = true;
                }
                else MessageBox.Show("Không nhận diện được khuôn mặt!");
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _faceAiService.StopCamera();
            if (!_isLoggingOut) Application.Exit();
        }

        private void btnOpenCreateClass_Click(object sender, EventArgs e)
        {
            var classService = _serviceProvider.GetRequiredService<ClassService>();
            int currentTeacherId = _authService.CurrentUser?.UserId ?? 0;

            using (var frmCreate = new FrmCreateClass(classService, currentTeacherId))
            {
                var result = frmCreate.ShowDialog();
                if (result == DialogResult.OK)
                {
                    _=LoadClassList();
                }
            }
        }

        private async Task LoadClassList()
        {
            System.Diagnostics.Debug.WriteLine(">> [CLASSLIST] B1: Bắt đầu");
            try
            {
                _allClassItems.Clear();
                var _currentUser = _authService.CurrentUser;
                if (_currentUser == null)
                {
                    System.Diagnostics.Debug.WriteLine(">> [CLASSLIST] B2: user null, return");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($">> [CLASSLIST] B3: IsTeacher={_currentUser.IsTeacher}, IsStudent={_currentUser.IsStudent}");
                var _classService = _serviceProvider.GetRequiredService<ClassService>();

                if (_currentUser.IsTeacher)
                {
                    System.Diagnostics.Debug.WriteLine(">> [CLASSLIST] B4: Lấy owned classes...");
                    var ownedClasses = await _classService.GetOwnedClassesAsync(_currentUser.UserId);
                    System.Diagnostics.Debug.WriteLine($">> [CLASSLIST] B5: Có {ownedClasses.Count} lớp dạy");
                    foreach (var c in ownedClasses)
                    {
                        string fullDisplayName = $"{c.ClassName} - {c.Subject?.SubjectName ?? "N/A"}";
                        _allClassItems.Add((fullDisplayName, c.JoinCode, c.ClassId));
                    }
                }

                if (_currentUser.IsStudent)
                {
                    System.Diagnostics.Debug.WriteLine(">> [CLASSLIST] B6: Lấy joined classes...");
                    var joinedClasses = await _classService.GetJoinedClassesAsync(_currentUser.UserId);
                    System.Diagnostics.Debug.WriteLine($">> [CLASSLIST] B7: Có {joinedClasses.Count} lớp học");
                    foreach (var c in joinedClasses)
                    {
                        if (_allClassItems.Any(i => i.ClassId == c.ClassId)) continue;
                        string fullDisplayName = $"{c.ClassName} - {c.Subject?.SubjectName ?? "N/A"}";
                        _allClassItems.Add((fullDisplayName, "******", c.ClassId));
                    }
                }

                ApplyClassListFilter();
                System.Diagnostics.Debug.WriteLine(">> [CLASSLIST] B8: Hoàn tất");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(">> [CLASSLIST] LỖI: " + ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void btnOpenJoinClass_Click(object sender, EventArgs e)
        {
            var classService = _serviceProvider.GetRequiredService<ClassService>();
            int currentUserId = _authService.CurrentUser?.UserId ?? 0;

            using (var frmJoin = new FrmJoinClass(classService, currentUserId))
            {
                if (frmJoin.ShowDialog() == DialogResult.OK)
                {
                    _=LoadClassList();
                }
            }
        }

        // Double-click on class list
        private async void lvwClasses_MouseDoubleClick(object sender, EventArgs e)
        {
            var _currentUser = _authService.CurrentUser;
            if (lvwClasses.SelectedItems.Count == 0) return;

            var selectedItem = lvwClasses.SelectedItems[0];
            string className = selectedItem.Text;
            int classId = (int)(selectedItem.Tag ?? 0); 

            var streamService = _serviceProvider.GetRequiredService<StreamService>();
            var classService = _serviceProvider.GetRequiredService<ClassService>();
            var assignmentService = _serviceProvider.GetRequiredService<AssignmentService>();
            var examService = _serviceProvider.GetRequiredService<ExamService>();
            int currentUserId = _currentUser?.UserId ?? 0;
            // KI�,M TRA XEM C�" PHẢI GIÁO VI�SN CỦA L�sP N�?Y KH�"NG
            bool isTeacherOfThisClass = await classService.IsTeacherOfClassAsync(classId, currentUserId);

            // M�Y chi tiết l�>p dư�>i dạng form riêng (không append thêm tab/sidebar trong MainForm)
            OpenClassDetailForm(classId, className, streamService, classService, assignmentService, examService, currentUserId, isTeacherOfThisClass);
        }

        private void OpenClassDetailForm(int classId, string className, StreamService streamService, ClassService classService, AssignmentService assignmentService, ExamService examService, int currentUserId, bool isTeacherOfThisClass)
        {
            using (var detailForm = new MaterialForm())
            {
                detailForm.Text = "Lớp: " + className;
                detailForm.StartPosition = FormStartPosition.CenterParent;
                detailForm.Size = new Size(1250, 760);
                detailForm.MinimumSize = new Size(1000, 650);

                MaterialSkinManager.Instance.AddFormToManage(detailForm);

                var ucDetail = new UcClassDetail(streamService, classService, _authService, currentUserId, classId, isTeacherOfThisClass);
                ucDetail.SetAssignmentService(assignmentService);
                ucDetail.SetExamService(examService);
                ucDetail.Dock = DockStyle.Fill;

                detailForm.Controls.Add(ucDetail);
                detailForm.ShowDialog(this);
            }
        }

        private void OpenClassDetailTab(int classId, string className, StreamService streamService, ClassService classService, AssignmentService assignmentService, ExamService examService, int currentUserId, bool isTeacherOfThisClass)
        {
            string tabName = "tabClass_" + classId;
            TabPage tabDetail = materialTabControl1.TabPages[tabName];

            if (tabDetail == null)
            {
                tabDetail = new TabPage();
                tabDetail.Name = tabName;
                tabDetail.Text = "Lớp: " + className;
                tabDetail.BackColor = Color.White;

                var ucDetail = new UcClassDetail(streamService, classService, _authService, currentUserId, classId, isTeacherOfThisClass);
                ucDetail.SetAssignmentService(assignmentService);
                ucDetail.SetExamService(examService);
                ucDetail.Dock = DockStyle.Fill;

                tabDetail.Controls.Add(ucDetail);
                materialTabControl1.TabPages.Add(tabDetail);
            }

            materialTabControl1.SelectedTab = tabDetail;
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            // Ctrl + Scroll wheel simulation with + and - keys
            if (e.Control)
            {
                if (e.KeyCode == Keys.Oemplus || e.KeyCode == Keys.Add)
                {
                    ZoomIn();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.OemMinus || e.KeyCode == Keys.Subtract)
                {
                    ZoomOut();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.D0)
                {
                    ResetZoom();
                    e.Handled = true;
                }
            }
        }

        // 1. Khai báo khung chứa các thẻ th�'ng kê
        private FlowLayoutPanel flpDashboardCards;

        // 2. Hàm Setup giao di�?n Trang Chủ
        private void SetupHomeTab()
        {
            // Thêm chữ "T�.ng quan hoạt �'�Tng" dư�>i dòng Chào mừng
            MaterialLabel lblSubtitle = new MaterialLabel
            {
                Text = "Tổng quan hoạt động của bạn",
                FontType = MaterialSkinManager.fontType.Subtitle1,
                Location = new Point(35, 75),
                AutoSize = true,
                ForeColor = Color.Gray
            };
            tabHome.Controls.Add(lblSubtitle);

            // Khung chứa các thẻ màu sắc
            flpDashboardCards = new FlowLayoutPanel
            {
                Location = new Point(35, 120),
                Width = tabHome.Width - 70,
                Height = 180, // T�,NG L�SN 180 Đ�, CÁC THẺ KH�"NG B�S CH�^N �?P
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                WrapContents = true
            };
            tabHome.Controls.Add(flpDashboardCards);

            // Tiêu �'ề Hành �'�Tng nhanh
            MaterialLabel lblQuickActions = new MaterialLabel
            {
                Text = "Hành động nhanh",
                FontType = MaterialSkinManager.fontType.H6,
                Location = new Point(35, 300),
                AutoSize = true
            };
            tabHome.Controls.Add(lblQuickActions);

            // Khung chứa các nút bấm
            FlowLayoutPanel flpActions = new FlowLayoutPanel
            {
                Location = new Point(35, 340),
                Width = tabHome.Width - 70,
                Height = 100,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            MaterialButton btnGoToClasses = new MaterialButton { Text = "MỞ DANH SÁCH LỚP", AutoSize = true, UseAccentColor = true };
            btnGoToClasses.Click += (s, e) => materialTabControl1.SelectedTab = tabClasses;

            MaterialButton btnGoToProfile = new MaterialButton { Text = "CẬP NHẬT HỒ SƠ", AutoSize = true, Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined };
            btnGoToProfile.Click += (s, e) => materialTabControl1.SelectedTab = tabProfile;

            flpActions.Controls.Add(btnGoToClasses);
            flpActions.Controls.Add(btnGoToProfile);

            // Nếu là giáo viên, thêm nút Quản lý �'ề thi
            var user = _authService.CurrentUser;
            if (user != null && user.IsTeacher)
            {
                MaterialButton btnGoToExams = new MaterialButton { Text = "QUẢN LÝ ĐỀ THI", AutoSize = true, Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined };
                btnGoToExams.Click += btnOpenExamManager_Click;
                flpActions.Controls.Add(btnGoToExams);
            }

            tabHome.Controls.Add(flpActions);
        }

        // 3. Hàm vẽ ra từng thẻ màu sắc (Card)
        private Panel CreateDashboardCard(string title, string value, Color bgColor, string iconKind)
        {
            Panel card = new Panel
            {
                Width = 300,
                Height = 130,
                Margin = new Padding(0, 0, 20, 20),
                BackColor = bgColor,
                Tag = bgColor,
                BorderStyle = BorderStyle.None
            };

            card.Paint += (s, e) => { e.Graphics.Clear(bgColor); };

            Label lblTitle = new Label { Text = title.ToUpper(), Tag = "dashboard-title", Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = Color.White, BackColor = Color.Transparent, AutoSize = true, Location = new Point(20, 20) };
            Label lblValue = new Label { Text = value, Tag = "dashboard-value", Font = new Font("Segoe UI", 32, FontStyle.Bold), ForeColor = Color.White, BackColor = Color.Transparent, AutoSize = true, Location = new Point(20, 50) };

            PictureBox picIcon = new PictureBox
            {
                Size = new Size(36, 36),
                Location = new Point(248, 16),
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Image = CreateDashboardIcon(iconKind)
            };

            card.Controls.Add(lblTitle);
            card.Controls.Add(lblValue);
            card.Controls.Add(picIcon);

            return card;
        }

        private Bitmap CreateDashboardIcon(string iconKind)
        {
            var bmp = new Bitmap(36, 36);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                using var pen = new Pen(Color.FromArgb(230, 255, 255, 255), 2f);
                using var fill = new SolidBrush(Color.FromArgb(50, 255, 255, 255));

                switch ((iconKind ?? string.Empty).ToLowerInvariant())
                {
                    case "teacher":
                        g.FillEllipse(fill, 9, 4, 18, 18);
                        g.DrawEllipse(pen, 9, 4, 18, 18);
                        g.FillRectangle(fill, 6, 22, 24, 10);
                        g.DrawRectangle(pen, 6, 22, 24, 10);
                        break;
                    case "assignment":
                        g.FillRectangle(fill, 8, 5, 20, 26);
                        g.DrawRectangle(pen, 8, 5, 20, 26);
                        g.DrawLine(pen, 11, 12, 25, 12);
                        g.DrawLine(pen, 11, 18, 25, 18);
                        g.DrawLine(pen, 11, 24, 20, 24);
                        break;
                    case "student":
                        g.FillEllipse(fill, 11, 5, 14, 14);
                        g.DrawEllipse(pen, 11, 5, 14, 14);
                        g.FillPie(fill, 6, 16, 24, 16, 200, 140);
                        g.DrawArc(pen, 6, 16, 24, 16, 200, 140);
                        break;
                    case "account":
                        g.FillEllipse(fill, 10, 5, 16, 16);
                        g.DrawEllipse(pen, 10, 5, 16, 16);
                        g.DrawEllipse(pen, 6, 18, 24, 13);
                        break;
                    default:
                        g.FillEllipse(fill, 8, 8, 20, 20);
                        g.DrawEllipse(pen, 8, 8, 20, 20);
                        break;
                }
            }

            return bmp;
        }

        // 4. Hàm nạp dữ li�?u thật vào các thẻ
       private async Task LoadDashboardDataAsync()
{
    System.Diagnostics.Debug.WriteLine(">> [DASHBOARD] B1: Bắt đầu");
    try
    {
        var user = _authService.CurrentUser;
        if (user == null || flpDashboardCards == null)
        {
            System.Diagnostics.Debug.WriteLine(">> [DASHBOARD] B2: null, return");
            return;
        }

        int classCount = 0;
        int pendingActionCount = 0;

        using (var scope = _serviceProvider.CreateScope())
        {
            var classService = scope.ServiceProvider.GetRequiredService<ClassService>();
            var assignmentService = scope.ServiceProvider.GetRequiredService<AssignmentService>();

            if (user.IsTeacher)
            {
                var classes = await classService.GetOwnedClassesAsync(user.UserId).ConfigureAwait(false);
                classCount = classes.Count;
                foreach (var c in classes)
                {
                    var assignments = await assignmentService.GetAssignmentsByClassIdAsync(c.ClassId).ConfigureAwait(false);
                    foreach (var a in assignments)
                        if (a.Submissions != null)
                            pendingActionCount += a.Submissions.Count(s => s.Score == null);
                }
            }
            else
            {
                var classes = await classService.GetJoinedClassesAsync(user.UserId).ConfigureAwait(false);
                classCount = classes.Count;
                foreach (var c in classes)
                {
                    var assignments = await assignmentService.GetAssignmentsByClassIdAsync(c.ClassId).ConfigureAwait(false);
                    foreach (var a in assignments)
                    {
                        bool isSubmitted = a.Submissions != null && a.Submissions.Any(s => s.StudentId == user.UserId);
                        if (!isSubmitted) pendingActionCount++;
                    }
                }
            }
        }

        System.Diagnostics.Debug.WriteLine(">> [DASHBOARD] B4: Xong DB");

        if (!this.IsHandleCreated)
        {
            System.Diagnostics.Debug.WriteLine(">> [DASHBOARD] B4b: Handle chưa có, đợi...");
            await Task.Delay(300).ConfigureAwait(false);
        }

        System.Diagnostics.Debug.WriteLine(">> [DASHBOARD] B4c: Gọi BeginInvoke");
        this.BeginInvoke(new MethodInvoker(() =>
        {
            System.Diagnostics.Debug.WriteLine(">> [DASHBOARD] B5: Vẽ UI");
            flpDashboardCards.Controls.Clear();

            if (user.IsTeacher)
            {
                flpDashboardCards.Controls.Add(CreateDashboardCard("LỚP ĐANG DẠY", $"{classCount} lớp", Color.FromArgb(33, 150, 243), "teacher"));
                flpDashboardCards.Controls.Add(CreateDashboardCard("BÀI NỘP CHỜ CHẤM", $"{pendingActionCount} bài", Color.FromArgb(255, 152, 0), "assignment"));
            }
            else
            {
                flpDashboardCards.Controls.Add(CreateDashboardCard("LỚP ĐANG HỌC", $"{classCount} lớp", Color.FromArgb(76, 175, 80), "student"));
                flpDashboardCards.Controls.Add(CreateDashboardCard("BÀI TẬP CHỜ NỘP", $"{pendingActionCount} bài", Color.FromArgb(244, 67, 54), "assignment"));
            }

            flpDashboardCards.Controls.Add(CreateDashboardCard("VAI TRÒ TÀI KHOẢN", user.IsTeacher ? "Giáo viên" : "Học sinh", Color.FromArgb(156, 39, 176), "account"));
            NormalizeDashboardCardsAppearance();
            System.Diagnostics.Debug.WriteLine(">> [DASHBOARD] B6: Hoàn tất");
        }));
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine(">> [DASHBOARD] LỖI: " + ex.Message + "\n" + ex.StackTrace);
        if (this.IsHandleCreated)
        {
            this.BeginInvoke(new MethodInvoker(() =>
                MessageBox.Show("Lỗi Dashboard: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error)));
        }
    }
}

        // 1. Hàm thiết lập giao di�?n trang Cài �'ặt
       // 1. Hàm thiết lập giao di�?n trang Cài �'ặt (Chuẩn MaterialSkin)
        private void SetupSettingsTab()
        {
            tabSettings.Controls.Clear();
            
            // X�"A HẾT CÁC L�?NH �?P M�?U N�?N (BackColor = White/Transparent) ĐI
            // Hãy �'�f MaterialSkin tự �'�Tng lo màu nền cho Tab này!

            MaterialLabel lblTitle = new MaterialLabel
            {
                Text = "CÀI ĐẶT GIAO DIỆN ỨNG DỤNG",
                FontType = MaterialSkinManager.fontType.H6,
                Location = new Point(35, 30),
                AutoSize = true
            };

            var currentSettings = LoadUiSettings();

            MaterialSwitch switchDarkMode = new MaterialSwitch
            {
                Text = "Chế độ nền tối (Dark Mode)",
                Location = new Point(35, 80),
                AutoSize = true,
                Checked = currentSettings.isDark
            };
            
            switchDarkMode.CheckedChanged += (s, e) =>
            {
                MaterialSkinManager.Instance.Theme = switchDarkMode.Checked ? MaterialSkinManager.Themes.DARK : MaterialSkinManager.Themes.LIGHT;
                
                int currentColorIndex = LoadUiSettings().colorIndex;
                SaveUiSettings(switchDarkMode.Checked, currentColorIndex);

                this.Invalidate(true);
                this.Refresh();
                _ = LoadDashboardDataAsync();
            };

            MaterialLabel lblTheme = new MaterialLabel
            {
                Text = "Chọn màu chủ đạo:",
                FontType = MaterialSkinManager.fontType.Subtitle1,
                Location = new Point(35, 160),
                AutoSize = true
            };

            FlowLayoutPanel flpColors = new FlowLayoutPanel
            {
                Location = new Point(35, 200),
                Width = 800,
                Height = 100,
                WrapContents = true
            };

            // Tạo các Ảnh màu (Dùng ID từ 0 �'ến 4)
            flpColors.Controls.Add(CreateColorOption("Xanh Dương", Color.FromArgb(30, 136, 229), 0));
            flpColors.Controls.Add(CreateColorOption("Xanh Lá", Color.FromArgb(67, 160, 71), 1));
            flpColors.Controls.Add(CreateColorOption("Cam", Color.FromArgb(251, 140, 0), 2));
            flpColors.Controls.Add(CreateColorOption("Tím", Color.FromArgb(142, 36, 170), 3));
            flpColors.Controls.Add(CreateColorOption("Hồng", Color.FromArgb(216, 27, 96), 4));

            tabSettings.Controls.Add(lblTitle);
            tabSettings.Controls.Add(switchDarkMode);
            tabSettings.Controls.Add(lblTheme);
            tabSettings.Controls.Add(flpColors);
        }

        // 2. TUY�?T CHI�SU: Biến cái nút thành m�Tt bức ảnh (PictureBox)
        private PictureBox CreateColorOption(string name, Color bgColor, int colorIndex)
        {
            // Tạo m�Tt bức ảnh kích thư�>c 130x45
            Bitmap bmp = new Bitmap(130, 45);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                // Tô màu nền cho bức ảnh
                g.Clear(bgColor);
                
                // Vẽ chữ màu trắng lên giữa bức ảnh
                using (Font font = new Font("Segoe UI", 11, FontStyle.Bold))
                using (Brush brush = new SolidBrush(Color.White))
                {
                    StringFormat sf = new StringFormat();
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    g.DrawString(name, font, brush, new RectangleF(0, 0, 130, 45), sf);
                }
            }

            // Bỏ bức ảnh vào PictureBox (MaterialSkin không th�f �'ụng vào Image)
            PictureBox pic = new PictureBox
            {
                Image = bmp,
                Size = new Size(130, 45),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 15, 15)
            };

            pic.Click += (s, e) =>
            {
                ApplyColorByIndex(colorIndex);
                
                bool isDark = MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK;
                SaveUiSettings(isDark, colorIndex);

                this.Refresh(); 
                _ = LoadDashboardDataAsync();
            };

            return pic;
        }

        private void ZoomIn()
        {
            if (_currentZoom < MAX_ZOOM)
            {
                _currentZoom = Math.Min(_currentZoom + ZOOM_STEP, MAX_ZOOM);
                ApplyZoom();
            }
        }

        private void ZoomOut()
        {
            if (_currentZoom > MIN_ZOOM)
            {
                _currentZoom = Math.Max(_currentZoom - ZOOM_STEP, MIN_ZOOM);
                ApplyZoom();
            }
        }

        private void ResetZoom()
        {
            _currentZoom = 1.0f;
            ApplyZoom();
        }

        private void ApplyZoom()
        {
            // Áp dụng zoom cho tất cả các tab
            foreach (TabPage tab in materialTabControl1.TabPages)
            {
                foreach (Control ctrl in tab.Controls)
                {
                    if (ctrl is UcClassDetail ucDetail)
                    {
                        // Phóng to/thu nhỏ font và size của các control
                        ScaleControl(ucDetail, _currentZoom);
                    }
                }
            }

            // Phóng to/thu nhỏ form chính
            this.Font = new Font(this.Font.FontFamily, 8 * _currentZoom, this.Font.Style);

            // Update title bar to show zoom level
            this.Text = $"OmniSight Dashboard ({(int)(_currentZoom * 100)}%)";
        }

        private void ScaleControl(Control control, float zoomLevel)
        {
            if (control == null) return;

            try
            {
                // Scale font
                float newFontSize = 8 * zoomLevel;
                control.Font = new Font(control.Font?.FontFamily ?? new FontFamily("Roboto"), newFontSize, control.Font?.Style ?? FontStyle.Regular);

                // Scale child controls
                foreach (Control child in control.Controls)
                {
                    ScaleControl(child, zoomLevel);
                }
            }
            catch
            {
                // Ignore errors for controls that don't support scaling
            }
        }
        // --- H�? THỐNG LƯU TRỮ C�?I ĐẶT GIAO DI�?N ---
        private string GetUiSettingsFilePath()
        {
            // Lưu file cấu hình ui_settings.txt vào cùng ch�- v�>i session.json
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OmniSight", "ui_settings.txt");
        }

        private void SaveUiSettings(bool isDark, int colorIndex)
        {
            try
            {
                string path = GetUiSettingsFilePath();
                File.WriteAllText(path, $"{isDark}|{colorIndex}");
            }
            catch { }
        }

        private (bool isDark, int colorIndex) LoadUiSettings()
        {
            try
            {
                string path = GetUiSettingsFilePath();
                if (File.Exists(path))
                {
                    var parts = File.ReadAllText(path).Split('|');
                    if (parts.Length == 2)
                    {
                        return (bool.Parse(parts[0]), int.Parse(parts[1]));
                    }
                }
            }
            catch { }
            return (false, 0); // Mặc �'�<nh: Nền sáng, Màu Xanh dương
        }

        private void ApplyColorByIndex(int index)
        {
            var msm = MaterialSkinManager.Instance;
            switch (index)
            {
                case 1: msm.ColorScheme = new ColorScheme(Primary.Green600, Primary.Green700, Primary.Green200, Accent.LightGreen200, TextShade.WHITE); break;
                case 2: msm.ColorScheme = new ColorScheme(Primary.Orange600, Primary.Orange700, Primary.Orange200, Accent.DeepOrange200, TextShade.WHITE); break;
                case 3: msm.ColorScheme = new ColorScheme(Primary.Purple600, Primary.Purple700, Primary.Purple200, Accent.DeepPurple200, TextShade.WHITE); break;
                case 4: msm.ColorScheme = new ColorScheme(Primary.Pink600, Primary.Pink700, Primary.Pink200, Accent.Pink200, TextShade.WHITE); break;
                default: msm.ColorScheme = new ColorScheme(Primary.Blue600, Primary.Blue700, Primary.Blue200, Accent.LightBlue200, TextShade.WHITE); break;
            }
        }
    }
}
