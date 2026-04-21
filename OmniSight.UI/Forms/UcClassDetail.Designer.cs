using System;
using System.Drawing;
using System.Windows.Forms;

namespace OmniSight.UI.Forms
{
    partial class UcClassDetail
    {
        private System.ComponentModel.IContainer components = null;

        private TableLayoutPanel tlpPost;
        private TableLayoutPanel tlpAssignment;
        private FlowLayoutPanel flpExamToolbar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            materialTabControl1 = new MaterialSkin.Controls.MaterialTabControl();
            tabStream = new TabPage();
            flpStream = new FlowLayoutPanel();
            panelPost = new Panel();
            btnPost = new MaterialSkin.Controls.MaterialButton();
            txtPostContent = new MaterialSkin.Controls.MaterialMultiLineTextBox2();
            tabAssignments = new TabPage();
            flpAssignments = new FlowLayoutPanel();
            panelCreateAssignment = new Panel();
            btnActionAssignment = new MaterialSkin.Controls.MaterialButton();
            dtpDueDate = new DateTimePicker();
            lblDueDate = new MaterialSkin.Controls.MaterialLabel();
            txtAssignmentDescription = new MaterialSkin.Controls.MaterialMultiLineTextBox2();
            txtDriveLink = new MaterialSkin.Controls.MaterialTextBox();
            txtAssignmentName = new MaterialSkin.Controls.MaterialTextBox();
            lblAssignmentTitle = new MaterialSkin.Controls.MaterialLabel();
            tabGrading = new TabPage();
            flpGrading = new FlowLayoutPanel();
            tabExams = new TabPage();
            flpExams = new FlowLayoutPanel();
            tabMembers = new TabPage();
            flpMembers = new FlowLayoutPanel();
            lblGradingTitle = new MaterialSkin.Controls.MaterialLabel();
            lvwSubmissions = new MaterialSkin.Controls.MaterialListView();
            colStudent = new ColumnHeader();
            colAssignment = new ColumnHeader();
            colLink = new ColumnHeader();
            colScore = new ColumnHeader();
            lblExamsTitle = new MaterialSkin.Controls.MaterialLabel();
            lblMembersTitle = new MaterialSkin.Controls.MaterialLabel();
            materialTabSelector1 = new MaterialSkin.Controls.MaterialTabSelector();
            panelExamTeacher = new Panel();
            btnDownloadTemplate = new MaterialSkin.Controls.MaterialButton();
            btnImportWord = new MaterialSkin.Controls.MaterialButton();
            btnManageExams = new MaterialSkin.Controls.MaterialButton();
            tlpPost = new TableLayoutPanel();
            tlpAssignment = new TableLayoutPanel();
            flpExamToolbar = new FlowLayoutPanel();
            materialTabControl1.SuspendLayout();
            tabStream.SuspendLayout();
            panelPost.SuspendLayout();
            tlpPost.SuspendLayout();
            tabAssignments.SuspendLayout();
            panelCreateAssignment.SuspendLayout();
            tlpAssignment.SuspendLayout();
            tabGrading.SuspendLayout();
            tabExams.SuspendLayout();
            panelExamTeacher.SuspendLayout();
            flpExamToolbar.SuspendLayout();
            tabMembers.SuspendLayout();
            SuspendLayout();
            // 
            // materialTabControl1
            // 
            materialTabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            materialTabControl1.Controls.Add(tabStream);
            materialTabControl1.Controls.Add(tabAssignments);
            materialTabControl1.Controls.Add(tabGrading);
            materialTabControl1.Controls.Add(tabExams);
            materialTabControl1.Controls.Add(tabMembers);
            materialTabControl1.Depth = 0;
            materialTabControl1.Location = new Point(0, 42);
            materialTabControl1.MouseState = MaterialSkin.MouseState.HOVER;
            materialTabControl1.Multiline = true;
            materialTabControl1.Name = "materialTabControl1";
            materialTabControl1.SelectedIndex = 0;
            materialTabControl1.Size = new Size(850, 458);
            materialTabControl1.TabIndex = 0;
            // 
            // tabStream
            // 
            tabStream.BackColor = Color.White;
            tabStream.Controls.Add(flpStream);
            tabStream.Controls.Add(panelPost);
            tabStream.Location = new Point(4, 24);
            tabStream.Name = "tabStream";
            tabStream.Padding = new Padding(3);
            tabStream.Size = new Size(842, 430);
            tabStream.TabIndex = 0;
            tabStream.Text = "Bảng Tin";
            // 
            // panelPost
            // 
            panelPost.Controls.Add(tlpPost);
            panelPost.Dock = DockStyle.Top;
            panelPost.Location = new Point(3, 3);
            panelPost.Name = "panelPost";
            panelPost.Size = new Size(836, 84);
            panelPost.TabIndex = 0;
            // 
            // tlpPost
            // 
            tlpPost.ColumnCount = 2;
            tlpPost.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpPost.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 125F));
            tlpPost.Controls.Add(txtPostContent, 0, 0);
            tlpPost.Controls.Add(btnPost, 1, 0);
            tlpPost.Dock = DockStyle.Fill;
            tlpPost.Location = new Point(0, 0);
            tlpPost.Name = "tlpPost";
            tlpPost.Padding = new Padding(15, 12, 15, 12);
            tlpPost.RowCount = 1;
            tlpPost.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpPost.Size = new Size(836, 84);
            tlpPost.TabIndex = 0;
            // 
            // txtPostContent
            // 
            txtPostContent.AnimateReadOnly = false;
            txtPostContent.BackgroundImageLayout = ImageLayout.None;
            txtPostContent.CharacterCasing = CharacterCasing.Normal;
            txtPostContent.Depth = 0;
            txtPostContent.Dock = DockStyle.Fill;
            txtPostContent.HideSelection = true;
            txtPostContent.Hint = "Đăng thông báo cho lớp học...";
            txtPostContent.Margin = new Padding(0, 0, 10, 0);
            txtPostContent.MaxLength = 32767;
            txtPostContent.MouseState = MaterialSkin.MouseState.OUT;
            txtPostContent.Name = "txtPostContent";
            txtPostContent.PasswordChar = '\0';
            txtPostContent.ReadOnly = false;
            txtPostContent.ScrollBars = ScrollBars.None;
            txtPostContent.SelectedText = "";
            txtPostContent.SelectionLength = 0;
            txtPostContent.SelectionStart = 0;
            txtPostContent.ShortcutsEnabled = true;
            txtPostContent.Size = new Size(671, 60);
            txtPostContent.TabIndex = 0;
            txtPostContent.TabStop = false;
            txtPostContent.TextAlign = HorizontalAlignment.Left;
            txtPostContent.UseSystemPasswordChar = false;
            // 
            // btnPost
            // 
            btnPost.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnPost.AutoSize = false;
            btnPost.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnPost.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            btnPost.Depth = 0;
            btnPost.HighEmphasis = true;
            btnPost.Icon = null;
            btnPost.Location = new Point(706, 12);
            btnPost.Margin = new Padding(0, 12, 0, 12);
            btnPost.MouseState = MaterialSkin.MouseState.HOVER;
            btnPost.Name = "btnPost";
            btnPost.NoAccentTextColor = Color.Empty;
            btnPost.Size = new Size(100, 36);
            btnPost.TabIndex = 1;
            btnPost.Text = "ĐĂNG BÀI";
            btnPost.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            btnPost.UseAccentColor = false;
            btnPost.UseVisualStyleBackColor = true;
            btnPost.Click += btnPost_Click;
            // 
            // flpStream
            // 
            flpStream.AutoScroll = true;
            flpStream.Dock = DockStyle.Fill;
            flpStream.Location = new Point(3, 87);
            flpStream.Margin = new Padding(3, 4, 3, 4);
            flpStream.Name = "flpStream";
            flpStream.Padding = new Padding(8);
            flpStream.Size = new Size(836, 340);
            flpStream.TabIndex = 1;
            // 
            // tabAssignments
            // 
            tabAssignments.BackColor = Color.White;
            tabAssignments.Controls.Add(flpAssignments);
            tabAssignments.Controls.Add(panelCreateAssignment);
            tabAssignments.Location = new Point(4, 24);
            tabAssignments.Name = "tabAssignments";
            tabAssignments.Padding = new Padding(3);
            tabAssignments.Size = new Size(842, 430);
            tabAssignments.TabIndex = 1;
            tabAssignments.Text = "Bài Tập & Nộp Bài";
            // 
            // panelCreateAssignment
            // 
            panelCreateAssignment.Controls.Add(tlpAssignment);
            panelCreateAssignment.Dock = DockStyle.Top;
            panelCreateAssignment.Location = new Point(3, 3);
            panelCreateAssignment.Name = "panelCreateAssignment";
            panelCreateAssignment.Size = new Size(836, 262);
            panelCreateAssignment.TabIndex = 0;
            // 
            // tlpAssignment
            // 
            tlpAssignment.ColumnCount = 3;
            tlpAssignment.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
            tlpAssignment.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpAssignment.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            tlpAssignment.Controls.Add(lblAssignmentTitle, 0, 0);
            tlpAssignment.Controls.Add(txtAssignmentName, 0, 1);
            tlpAssignment.Controls.Add(txtDriveLink, 0, 2);
            tlpAssignment.Controls.Add(txtAssignmentDescription, 0, 3);
            tlpAssignment.Controls.Add(lblDueDate, 0, 4);
            tlpAssignment.Controls.Add(dtpDueDate, 1, 4);
            tlpAssignment.Controls.Add(btnActionAssignment, 2, 4);
            tlpAssignment.Dock = DockStyle.Fill;
            tlpAssignment.Location = new Point(0, 0);
            tlpAssignment.Name = "tlpAssignment";
            tlpAssignment.Padding = new Padding(18, 10, 18, 10);
            tlpAssignment.RowCount = 5;
            tlpAssignment.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            tlpAssignment.RowStyles.Add(new RowStyle(SizeType.Absolute, 55F));
            tlpAssignment.RowStyles.Add(new RowStyle(SizeType.Absolute, 55F));
            tlpAssignment.RowStyles.Add(new RowStyle(SizeType.Absolute, 85F));
            tlpAssignment.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F));
            panelCreateAssignment.Height = 320;
            tlpAssignment.TabIndex = 0;
            tlpAssignment.SetColumnSpan(lblAssignmentTitle, 3);
            tlpAssignment.SetColumnSpan(txtAssignmentName, 3);
            tlpAssignment.SetColumnSpan(txtDriveLink, 3);
            tlpAssignment.SetColumnSpan(txtAssignmentDescription, 3);
            // 
            // lblAssignmentTitle
            // 
            lblAssignmentTitle.AutoSize = true;
            lblAssignmentTitle.Depth = 0;
            lblAssignmentTitle.Dock = DockStyle.Fill;
            lblAssignmentTitle.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
            lblAssignmentTitle.Location = new Point(21, 10);
            lblAssignmentTitle.MouseState = MaterialSkin.MouseState.HOVER;
            lblAssignmentTitle.Name = "lblAssignmentTitle";
            lblAssignmentTitle.Size = new Size(776, 25);
            lblAssignmentTitle.TabIndex = 0;
            lblAssignmentTitle.Text = "Khu vực giao bài";
            lblAssignmentTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtAssignmentName
            // 
            txtAssignmentName.AnimateReadOnly = false;
            txtAssignmentName.BorderStyle = BorderStyle.None;
            txtAssignmentName.Depth = 0;
            txtAssignmentName.Dock = DockStyle.Fill;
            txtAssignmentName.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            txtAssignmentName.Hint = "Tên bài tập...";
            txtAssignmentName.LeadingIcon = null;
            txtAssignmentName.Location = new Point(21, 38);
            txtAssignmentName.MaxLength = 50;
            txtAssignmentName.MouseState = MaterialSkin.MouseState.OUT;
            txtAssignmentName.Multiline = false;
            txtAssignmentName.Name = "txtAssignmentName";
            txtAssignmentName.Size = new Size(776, 49);
            txtAssignmentName.TabIndex = 1;
            txtAssignmentName.Text = "";
            txtAssignmentName.TrailingIcon = null;
            // 
            // txtDriveLink
            // 
            txtDriveLink.AnimateReadOnly = false;
            txtDriveLink.BorderStyle = BorderStyle.None;
            txtDriveLink.Depth = 0;
            txtDriveLink.Dock = DockStyle.Fill;
            txtDriveLink.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            txtDriveLink.Hint = "Dán Link Google Drive sản phẩm vào đây...";
            txtDriveLink.LeadingIcon = null;
            txtDriveLink.Location = new Point(21, 93);
            txtDriveLink.MaxLength = 500;
            txtDriveLink.MouseState = MaterialSkin.MouseState.OUT;
            txtDriveLink.Multiline = false;
            txtDriveLink.Name = "txtDriveLink";
            txtDriveLink.Size = new Size(776, 49);
            txtDriveLink.TabIndex = 2;
            txtDriveLink.Text = "";
            txtDriveLink.TrailingIcon = null;
            // 
            // txtAssignmentDescription
            // 
            txtAssignmentDescription.AnimateReadOnly = false;
            txtAssignmentDescription.BackgroundImageLayout = ImageLayout.None;
            txtAssignmentDescription.CharacterCasing = CharacterCasing.Normal;
            txtAssignmentDescription.Depth = 0;
            txtAssignmentDescription.Dock = DockStyle.Fill;
            txtAssignmentDescription.HideSelection = true;
            txtAssignmentDescription.Hint = "Mô tả bài tập (tùy chọn)...";
            txtAssignmentDescription.Location = new Point(21, 148);
            txtAssignmentDescription.MaxLength = 32767;
            txtAssignmentDescription.MouseState = MaterialSkin.MouseState.OUT;
            txtAssignmentDescription.Name = "txtAssignmentDescription";
            txtAssignmentDescription.PasswordChar = '\0';
            txtAssignmentDescription.ReadOnly = false;
            txtAssignmentDescription.ScrollBars = ScrollBars.Vertical;
            txtAssignmentDescription.SelectedText = "";
            txtAssignmentDescription.SelectionLength = 0;
            txtAssignmentDescription.SelectionStart = 0;
            txtAssignmentDescription.ShortcutsEnabled = true;
            txtAssignmentDescription.Size = new Size(776, 79);
            txtAssignmentDescription.TabIndex = 4;
            txtAssignmentDescription.TabStop = false;
            txtAssignmentDescription.TextAlign = HorizontalAlignment.Left;
            txtAssignmentDescription.UseSystemPasswordChar = false;
            // 
            // lblDueDate
            // 
            lblDueDate.AutoSize = true;
            lblDueDate.Depth = 0;
            lblDueDate.Dock = DockStyle.Fill;
            lblDueDate.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
            lblDueDate.Location = new Point(21, 233);
            lblDueDate.MouseState = MaterialSkin.MouseState.HOVER;
            lblDueDate.Name = "lblDueDate";
            lblDueDate.Size = new Size(134, 50);
            lblDueDate.TabIndex = 5;
            lblDueDate.Text = "Hạn nộp (tùy chọn):";
            lblDueDate.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // dtpDueDate
            // 
            dtpDueDate.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            dtpDueDate.Location = new Point(161, 246);
            dtpDueDate.Name = "dtpDueDate";
            dtpDueDate.Size = new Size(534, 23);
            dtpDueDate.TabIndex = 6;
            // 
            // btnActionAssignment
            // 
            btnActionAssignment.Anchor = AnchorStyles.Right;
            btnActionAssignment.AutoSize = false;
            btnActionAssignment.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnActionAssignment.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            btnActionAssignment.Depth = 0;
            btnActionAssignment.HighEmphasis = true;
            btnActionAssignment.Icon = null;
            btnActionAssignment.Location = new Point(700, 240);
            btnActionAssignment.Margin = new Padding(4, 6, 0, 6);
            btnActionAssignment.MouseState = MaterialSkin.MouseState.HOVER;
            btnActionAssignment.Name = "btnActionAssignment";
            btnActionAssignment.NoAccentTextColor = Color.Empty;
            btnActionAssignment.Size = new Size(100, 36);
            btnActionAssignment.TabIndex = 3;
            btnActionAssignment.Text = "THỰC HIỆN";
            btnActionAssignment.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            btnActionAssignment.UseAccentColor = false;
            btnActionAssignment.UseVisualStyleBackColor = true;
            btnActionAssignment.Click += btnActionAssignment_Click;
            // 
            // flpAssignments
            // 
            flpAssignments.AutoScroll = true;
            flpAssignments.Dock = DockStyle.Fill;
            flpAssignments.FlowDirection = FlowDirection.TopDown;
            flpAssignments.WrapContents = false;
            flpAssignments.Location = new Point(3, 265);
            flpAssignments.Name = "flpAssignments";
            flpAssignments.Padding = new Padding(8);
            flpAssignments.Size = new Size(836, 162);
            flpAssignments.TabIndex = 1;
            // 
            // tabGrading
            // 
            tabGrading.BackColor = Color.White;
            tabGrading.Controls.Add(flpGrading);
            tabGrading.Location = new Point(4, 24);
            tabGrading.Name = "tabGrading";
            tabGrading.Padding = new Padding(3);
            tabGrading.Size = new Size(842, 430);
            tabGrading.TabIndex = 2;
            tabGrading.Text = "Chấm Điểm";
            // 
            // flpGrading
            // 
            flpGrading.AutoScroll = true;
            flpGrading.Controls.Add(lblGradingTitle);
            flpGrading.Controls.Add(lvwSubmissions);
            flpGrading.Dock = DockStyle.Fill;
            flpGrading.FlowDirection = FlowDirection.TopDown;
            flpGrading.Location = new Point(3, 3);
            flpGrading.Name = "flpGrading";
            flpGrading.Padding = new Padding(10);
            flpGrading.Size = new Size(836, 424);
            flpGrading.TabIndex = 0;
            flpGrading.WrapContents = false;
            // 
            // lblGradingTitle
            // 
            lblGradingTitle.AutoSize = true;
            lblGradingTitle.Depth = 0;
            lblGradingTitle.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            lblGradingTitle.Location = new Point(13, 10);
            lblGradingTitle.MouseState = MaterialSkin.MouseState.HOVER;
            lblGradingTitle.Name = "lblGradingTitle";
            lblGradingTitle.Size = new Size(265, 19);
            lblGradingTitle.TabIndex = 0;
            lblGradingTitle.Text = "Danh sách bài nộp chờ chấm điểm";
            // 
            // lvwSubmissions
            // 
            lvwSubmissions.AutoSizeTable = false;
            lvwSubmissions.BackColor = Color.FromArgb(255, 255, 255);
            lvwSubmissions.BorderStyle = BorderStyle.None;
            lvwSubmissions.Columns.AddRange(new ColumnHeader[] { colStudent, colAssignment, colLink, colScore });
            lvwSubmissions.Depth = 0;
            lvwSubmissions.Font = new Font("Microsoft Sans Serif", 24F, FontStyle.Regular, GraphicsUnit.Pixel);
            lvwSubmissions.FullRowSelect = true;
            lvwSubmissions.Location = new Point(13, 42);
            lvwSubmissions.Margin = new Padding(3, 13, 3, 3);
            lvwSubmissions.MinimumSize = new Size(229, 133);
            lvwSubmissions.MouseLocation = new Point(-1, -1);
            lvwSubmissions.MouseState = MaterialSkin.MouseState.OUT;
            lvwSubmissions.Name = "lvwSubmissions";
            lvwSubmissions.OwnerDraw = true;
            lvwSubmissions.Size = new Size(800, 360);
            lvwSubmissions.TabIndex = 1;
            lvwSubmissions.UseCompatibleStateImageBehavior = false;
            lvwSubmissions.View = View.Details;
            lvwSubmissions.Resize += lvwSubmissions_Resize;
            // 
            // colStudent
            // 
            colStudent.Text = "Học sinh";
            colStudent.Width = 180;
            // 
            // colAssignment
            // 
            colAssignment.Text = "Bài tập";
            colAssignment.Width = 180;
            // 
            // colLink
            // 
            colLink.Text = "Link Drive";
            colLink.Width = 320;
            // 
            // colScore
            // 
            colScore.Text = "Điểm";
            colScore.Width = 100;
            // 
            // tabExams
            // 
            tabExams.BackColor = Color.White;
            tabExams.Controls.Add(flpExams);
            tabExams.Controls.Add(panelExamTeacher);
            tabExams.Location = new Point(4, 24);
            tabExams.Name = "tabExams";
            tabExams.Padding = new Padding(3);
            tabExams.Size = new Size(842, 430);
            tabExams.TabIndex = 3;
            tabExams.Text = "Đề Thi";
            // 
            // panelExamTeacher
            // 
            panelExamTeacher.Controls.Add(flpExamToolbar);
            panelExamTeacher.Dock = DockStyle.Top;
            panelExamTeacher.Location = new Point(3, 3);
            panelExamTeacher.Name = "panelExamTeacher";
            panelExamTeacher.Size = new Size(836, 64);
            panelExamTeacher.TabIndex = 1;
            // 
            // flpExamToolbar
            // 
            flpExamToolbar.AutoScroll = true;
            flpExamToolbar.Controls.Add(btnDownloadTemplate);
            flpExamToolbar.Controls.Add(btnImportWord);
            flpExamToolbar.Controls.Add(btnManageExams);
            flpExamToolbar.Dock = DockStyle.Fill;
            flpExamToolbar.FlowDirection = FlowDirection.LeftToRight;
            flpExamToolbar.Location = new Point(0, 0);
            flpExamToolbar.Name = "flpExamToolbar";
            flpExamToolbar.Padding = new Padding(10, 10, 10, 10);
            flpExamToolbar.Size = new Size(836, 64);
            flpExamToolbar.TabIndex = 0;
            flpExamToolbar.WrapContents = true;
            // 
            // btnDownloadTemplate
            // 
            btnDownloadTemplate.AutoSize = false;
            btnDownloadTemplate.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnDownloadTemplate.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            btnDownloadTemplate.Depth = 0;
            btnDownloadTemplate.HighEmphasis = true;
            btnDownloadTemplate.Icon = null;
            btnDownloadTemplate.Margin = new Padding(0, 0, 10, 10);
            btnDownloadTemplate.MouseState = MaterialSkin.MouseState.HOVER;
            btnDownloadTemplate.Name = "btnDownloadTemplate";
            btnDownloadTemplate.NoAccentTextColor = Color.Empty;
            btnDownloadTemplate.Size = new Size(130, 36);
            btnDownloadTemplate.TabIndex = 0;
            btnDownloadTemplate.Text = "TẢI FILE MẪU";
            btnDownloadTemplate.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined;
            btnDownloadTemplate.UseAccentColor = false;
            btnDownloadTemplate.UseVisualStyleBackColor = true;
            btnDownloadTemplate.Click += new EventHandler(btnDownloadTemplate_Click);
            // 
            // btnImportWord
            // 
            btnImportWord.AutoSize = false;
            btnImportWord.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnImportWord.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            btnImportWord.Depth = 0;
            btnImportWord.HighEmphasis = true;
            btnImportWord.Icon = null;
            btnImportWord.Margin = new Padding(0, 0, 10, 10);
            btnImportWord.MouseState = MaterialSkin.MouseState.HOVER;
            btnImportWord.Name = "btnImportWord";
            btnImportWord.NoAccentTextColor = Color.Empty;
            btnImportWord.Size = new Size(160, 36);
            btnImportWord.TabIndex = 1;
            btnImportWord.Text = "NHẬP ĐỀ TỪ WORD";
            btnImportWord.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            btnImportWord.UseAccentColor = false;
            btnImportWord.UseVisualStyleBackColor = true;
            btnImportWord.Click += new EventHandler(btnImportWord_Click);
            // 
            // btnManageExams
            // 
            btnManageExams.AutoSize = false;
            btnManageExams.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnManageExams.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            btnManageExams.Depth = 0;
            btnManageExams.HighEmphasis = true;
            btnManageExams.Icon = null;
            btnManageExams.Margin = new Padding(0, 0, 10, 10);
            btnManageExams.MouseState = MaterialSkin.MouseState.HOVER;
            btnManageExams.Name = "btnManageExams";
            btnManageExams.NoAccentTextColor = Color.Empty;
            btnManageExams.Size = new Size(160, 36);
            btnManageExams.TabIndex = 2;
            btnManageExams.Text = "QUẢN LÝ BÀI THI";
            btnManageExams.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            btnManageExams.UseAccentColor = true;
            btnManageExams.UseVisualStyleBackColor = true;
            btnManageExams.Click += new EventHandler(btnManageExams_Click);
            // 
            // flpExams
            // 
            flpExams.AutoScroll = true;
            flpExams.Dock = DockStyle.Fill;
            flpExams.Location = new Point(3, 67);
            flpExams.Name = "flpExams";
            flpExams.Padding = new Padding(8);
            flpExams.Size = new Size(836, 360);
            flpExams.TabIndex = 0;
            // 
            // tabMembers
            // 
            tabMembers.BackColor = Color.White;
            tabMembers.Controls.Add(flpMembers);
            tabMembers.Location = new Point(4, 24);
            tabMembers.Name = "tabMembers";
            tabMembers.Padding = new Padding(3);
            tabMembers.Size = new Size(842, 430);
            tabMembers.TabIndex = 4;
            tabMembers.Text = "Danh sách học sinh";
            tabMembers.UseVisualStyleBackColor = true;
            // 
            // flpMembers
            // 
            flpMembers.AutoScroll = true;
            flpMembers.Dock = DockStyle.Fill;
            flpMembers.Location = new Point(3, 3);
            flpMembers.Name = "flpMembers";
            flpMembers.Padding = new Padding(8);
            flpMembers.Size = new Size(836, 424);
            flpMembers.TabIndex = 0;
            // 
            // lblExamsTitle
            // 
            lblExamsTitle.AutoSize = true;
            lblExamsTitle.Depth = 0;
            lblExamsTitle.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            lblExamsTitle.Location = new Point(10, 10);
            lblExamsTitle.MouseState = MaterialSkin.MouseState.HOVER;
            lblExamsTitle.Name = "lblExamsTitle";
            lblExamsTitle.Size = new Size(110, 19);
            lblExamsTitle.TabIndex = 0;
            lblExamsTitle.Text = "Danh sách đề thi";
            // 
            // lblMembersTitle
            // 
            lblMembersTitle.AutoSize = true;
            lblMembersTitle.Depth = 0;
            lblMembersTitle.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            lblMembersTitle.Location = new Point(10, 10);
            lblMembersTitle.MouseState = MaterialSkin.MouseState.HOVER;
            lblMembersTitle.Name = "lblMembersTitle";
            lblMembersTitle.Size = new Size(189, 19);
            lblMembersTitle.TabIndex = 0;
            lblMembersTitle.Text = "Danh sách thành viên lớp học";
            // 
            // materialTabSelector1
            // 
            materialTabSelector1.BaseTabControl = materialTabControl1;
            materialTabSelector1.CharacterCasing = MaterialSkin.Controls.MaterialTabSelector.CustomCharacterCasing.Normal;
            materialTabSelector1.Depth = 0;
            materialTabSelector1.Dock = DockStyle.Top;
            materialTabSelector1.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
            materialTabSelector1.Location = new Point(0, 0);
            materialTabSelector1.MouseState = MaterialSkin.MouseState.HOVER;
            materialTabSelector1.Name = "materialTabSelector1";
            materialTabSelector1.Size = new Size(850, 42);
            materialTabSelector1.TabIndex = 1;
            // 
            // UcClassDetail
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            Controls.Add(materialTabSelector1);
            Controls.Add(materialTabControl1);
            Name = "UcClassDetail";
            Size = new Size(850, 500);
            VisibleChanged += UcClassDetail_VisibleChanged;
            Resize += UcClassDetail_Resize;
            materialTabControl1.ResumeLayout(false);
            tabStream.ResumeLayout(false);
            panelPost.ResumeLayout(false);
            tlpPost.ResumeLayout(false);
            tabAssignments.ResumeLayout(false);
            panelCreateAssignment.ResumeLayout(false);
            tlpAssignment.ResumeLayout(false);
            tlpAssignment.PerformLayout();
            tabGrading.ResumeLayout(false);
            tabExams.ResumeLayout(false);
            panelExamTeacher.ResumeLayout(false);
            flpExamToolbar.ResumeLayout(false);
            tabMembers.ResumeLayout(false);
            ResumeLayout(false);

            AdjustSubmissionColumns();
        }

        #endregion

        private void UcClassDetail_Resize(object sender, EventArgs e)
        {
            AdjustSubmissionColumns();
            ResizeAssignmentCards();
            ResizeGradingCards();
            ResizeExamCards();
            ResizeMemberCards();
            ResizeMemberSearchBox();
        }

        private void lvwSubmissions_Resize(object sender, EventArgs e)
        {
            AdjustSubmissionColumns();
        }

        private void AdjustSubmissionColumns()
        {
            if (lvwSubmissions == null || lvwSubmissions.Columns.Count < 4)
            {
                return;
            }

            int totalWidth = lvwSubmissions.ClientSize.Width;
            if (totalWidth <= 0)
            {
                return;
            }

            int scoreWidth = 90;
            int studentWidth = (int)(totalWidth * 0.22);
            int assignmentWidth = (int)(totalWidth * 0.22);
            int linkWidth = totalWidth - studentWidth - assignmentWidth - scoreWidth - 5;

            if (studentWidth < 120) studentWidth = 120;
            if (assignmentWidth < 120) assignmentWidth = 120;
            if (linkWidth < 150) linkWidth = 150;

            colStudent.Width = studentWidth;
            colAssignment.Width = assignmentWidth;
            colLink.Width = linkWidth;
            colScore.Width = scoreWidth;
        }

        private MaterialSkin.Controls.MaterialTabControl materialTabControl1;
        private MaterialSkin.Controls.MaterialTabSelector materialTabSelector1;
        private System.Windows.Forms.TabPage tabStream;
        private System.Windows.Forms.FlowLayoutPanel flpStream;
        private System.Windows.Forms.Panel panelPost;
        private MaterialSkin.Controls.MaterialMultiLineTextBox2 txtPostContent;
        private MaterialSkin.Controls.MaterialButton btnPost;
        private System.Windows.Forms.TabPage tabAssignments;
        private System.Windows.Forms.FlowLayoutPanel flpAssignments;
        private System.Windows.Forms.Panel panelCreateAssignment;
        private MaterialSkin.Controls.MaterialLabel lblAssignmentTitle;
        private MaterialSkin.Controls.MaterialTextBox txtAssignmentName;
        private MaterialSkin.Controls.MaterialTextBox txtDriveLink;
        private MaterialSkin.Controls.MaterialButton btnActionAssignment;
        private MaterialSkin.Controls.MaterialMultiLineTextBox2 txtAssignmentDescription;
        private MaterialSkin.Controls.MaterialLabel lblDueDate;
        private System.Windows.Forms.DateTimePicker dtpDueDate;
        private System.Windows.Forms.TabPage tabGrading;
        private System.Windows.Forms.FlowLayoutPanel flpGrading;
        private MaterialSkin.Controls.MaterialLabel lblGradingTitle;
        private MaterialSkin.Controls.MaterialListView lvwSubmissions;
        private System.Windows.Forms.ColumnHeader colStudent;
        private System.Windows.Forms.ColumnHeader colAssignment;
        private System.Windows.Forms.ColumnHeader colLink;
        private System.Windows.Forms.ColumnHeader colScore;
        private System.Windows.Forms.TabPage tabExams;
        private System.Windows.Forms.FlowLayoutPanel flpExams;
        private MaterialSkin.Controls.MaterialLabel lblExamsTitle;
        private System.Windows.Forms.TabPage tabMembers;
        private System.Windows.Forms.FlowLayoutPanel flpMembers;
        private MaterialSkin.Controls.MaterialLabel lblMembersTitle;
        private System.Windows.Forms.Panel panelExamTeacher;
        private MaterialSkin.Controls.MaterialButton btnDownloadTemplate;
        private MaterialSkin.Controls.MaterialButton btnImportWord;
        private MaterialSkin.Controls.MaterialButton btnManageExams;
    }
}