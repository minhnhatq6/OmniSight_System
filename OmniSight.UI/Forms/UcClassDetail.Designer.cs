namespace OmniSight.UI.Forms
{
    partial class UcClassDetail
    {
        private System.ComponentModel.IContainer components = null;

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
            lblGradingTitle = new MaterialSkin.Controls.MaterialLabel();
            lvwSubmissions = new MaterialSkin.Controls.MaterialListView();
            colStudent = new ColumnHeader();
            tabMembers = new TabPage();
            flpMembers = new FlowLayoutPanel();
            lblMembersTitle = new MaterialSkin.Controls.MaterialLabel();
            colAssignment = new ColumnHeader();
            colLink = new ColumnHeader();
            colScore = new ColumnHeader();
            materialTabSelector1 = new MaterialSkin.Controls.MaterialTabSelector();
            materialTabControl1.SuspendLayout();
            tabStream.SuspendLayout();
            panelPost.SuspendLayout();
            tabAssignments.SuspendLayout();
            panelCreateAssignment.SuspendLayout();
            tabGrading.SuspendLayout();
            tabMembers.SuspendLayout();
            SuspendLayout();
            // 
            // materialTabControl1
            // 
            materialTabControl1.Controls.Add(tabStream);
            materialTabControl1.Controls.Add(tabAssignments);
            materialTabControl1.Controls.Add(tabGrading);
            materialTabControl1.Controls.Add(tabMembers);
            materialTabControl1.Depth = 0;
            materialTabControl1.Location = new Point(0, 0);
            materialTabControl1.Margin = new Padding(3, 4, 3, 4);
            materialTabControl1.MouseState = MaterialSkin.MouseState.HOVER;
            materialTabControl1.Multiline = true;
            materialTabControl1.Name = "materialTabControl1";
            materialTabControl1.SelectedIndex = 0;
            materialTabControl1.Size = new Size(971, 667);
            materialTabControl1.TabIndex = 0;
            // 
            // tabStream
            // 
            tabStream.BackColor = Color.White;
            tabStream.Controls.Add(flpStream);
            tabStream.Controls.Add(panelPost);
            tabStream.Location = new Point(4, 29);
            tabStream.Margin = new Padding(3, 4, 3, 4);
            tabStream.Name = "tabStream";
            tabStream.Padding = new Padding(3, 4, 3, 4);
            tabStream.Size = new Size(963, 634);
            tabStream.TabIndex = 0;
            tabStream.Text = "Bảng Tin";
            // 
            // flpStream
            // 
            flpStream.AutoScroll = true;
            flpStream.Dock = DockStyle.Fill;
            flpStream.Location = new Point(3, 179);
            flpStream.Margin = new Padding(3, 4, 3, 4);
            flpStream.Name = "flpStream";
            flpStream.Size = new Size(957, 451);
            flpStream.TabIndex = 1;
            // 
            // panelPost
            // 
            panelPost.Controls.Add(btnPost);
            panelPost.Controls.Add(txtPostContent);
            panelPost.Dock = DockStyle.Top;
            panelPost.Location = new Point(3, 4);
            panelPost.Margin = new Padding(3, 4, 3, 4);
            panelPost.Name = "panelPost";
            panelPost.Size = new Size(957, 175);
            panelPost.TabIndex = 0;
            // 
            // btnPost
            // 
            btnPost.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnPost.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnPost.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            btnPost.Depth = 0;
            btnPost.HighEmphasis = true;
            btnPost.Icon = null;
            btnPost.Location = new Point(808, 95);
            btnPost.Margin = new Padding(5, 8, 5, 8);
            btnPost.MouseState = MaterialSkin.MouseState.HOVER;
            btnPost.Name = "btnPost";
            btnPost.NoAccentTextColor = Color.Empty;
            btnPost.Size = new Size(88, 36);
            btnPost.TabIndex = 1;
            btnPost.Text = "ĐĂNG BÀI";
            btnPost.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            btnPost.UseAccentColor = false;
            btnPost.UseVisualStyleBackColor = true;
            btnPost.Click += btnPost_Click;
            // 
            // txtPostContent
            // 
            txtPostContent.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtPostContent.AnimateReadOnly = false;
            txtPostContent.BackgroundImageLayout = ImageLayout.None;
            txtPostContent.CharacterCasing = CharacterCasing.Normal;
            txtPostContent.Depth = 0;
            txtPostContent.HideSelection = true;
            txtPostContent.Hint = "Đăng thông báo cho lớp học...";
            txtPostContent.Location = new Point(0, 80);
            txtPostContent.Margin = new Padding(3, 4, 3, 4);
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
            txtPostContent.Size = new Size(777, 80);
            txtPostContent.TabIndex = 0;
            txtPostContent.TabStop = false;
            txtPostContent.TextAlign = HorizontalAlignment.Left;
            txtPostContent.UseSystemPasswordChar = false;
            // 
            // tabAssignments
            // 
            tabAssignments.BackColor = Color.White;
            tabAssignments.Controls.Add(flpAssignments);
            tabAssignments.Controls.Add(panelCreateAssignment);
            tabAssignments.Location = new Point(4, 29);
            tabAssignments.Margin = new Padding(3, 4, 3, 4);
            tabAssignments.Name = "tabAssignments";
            tabAssignments.Padding = new Padding(3, 4, 3, 4);
            tabAssignments.Size = new Size(963, 634);
            tabAssignments.TabIndex = 1;
            tabAssignments.Text = "Bài Tập & Nộp Bài";
            // 
            // flpAssignments
            // 
            flpAssignments.AutoScroll = true;
            flpAssignments.Dock = DockStyle.Fill;
            flpAssignments.Location = new Point(3, 304);
            flpAssignments.Margin = new Padding(3, 4, 3, 4);
            flpAssignments.Name = "flpAssignments";
            flpAssignments.Size = new Size(957, 326);
            flpAssignments.TabIndex = 1;
            // 
            // panelCreateAssignment
            // 
            panelCreateAssignment.Controls.Add(btnActionAssignment);
            panelCreateAssignment.Controls.Add(dtpDueDate);
            panelCreateAssignment.Controls.Add(lblDueDate);
            panelCreateAssignment.Controls.Add(txtAssignmentDescription);
            panelCreateAssignment.Controls.Add(txtDriveLink);
            panelCreateAssignment.Controls.Add(txtAssignmentName);
            panelCreateAssignment.Controls.Add(lblAssignmentTitle);
            panelCreateAssignment.Dock = DockStyle.Top;
            panelCreateAssignment.Location = new Point(3, 4);
            panelCreateAssignment.Margin = new Padding(3, 4, 3, 4);
            panelCreateAssignment.Name = "panelCreateAssignment";
            panelCreateAssignment.Size = new Size(957, 300);
            panelCreateAssignment.TabIndex = 0;
            // 
            // btnActionAssignment
            // 
            btnActionAssignment.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnActionAssignment.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnActionAssignment.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            btnActionAssignment.Depth = 0;
            btnActionAssignment.HighEmphasis = true;
            btnActionAssignment.Icon = null;
            btnActionAssignment.Location = new Point(870, 200);
            btnActionAssignment.Margin = new Padding(5, 8, 5, 8);
            btnActionAssignment.MouseState = MaterialSkin.MouseState.HOVER;
            btnActionAssignment.Name = "btnActionAssignment";
            btnActionAssignment.NoAccentTextColor = Color.Empty;
            btnActionAssignment.Size = new Size(98, 36);
            btnActionAssignment.TabIndex = 3;
            btnActionAssignment.Text = "THỰC HIỆN";
            btnActionAssignment.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            btnActionAssignment.UseAccentColor = false;
            btnActionAssignment.UseVisualStyleBackColor = true;
            btnActionAssignment.Click += btnActionAssignment_Click;
            // 
            // dtpDueDate
            // 
            dtpDueDate.Location = new Point(100, 193);
            dtpDueDate.Margin = new Padding(3, 4, 3, 4);
            dtpDueDate.Name = "dtpDueDate";
            dtpDueDate.Size = new Size(200, 27);
            dtpDueDate.TabIndex = 6;
            // 
            // lblDueDate
            // 
            lblDueDate.AutoSize = true;
            lblDueDate.Depth = 0;
            lblDueDate.Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
            lblDueDate.Location = new Point(11, 198);
            lblDueDate.MouseState = MaterialSkin.MouseState.HOVER;
            lblDueDate.Name = "lblDueDate";
            lblDueDate.Size = new Size(141, 19);
            lblDueDate.TabIndex = 5;
            lblDueDate.Text = "Hạn nộp (tùy chọn):";
            // 
            // txtAssignmentDescription
            // 
            txtAssignmentDescription.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtAssignmentDescription.AnimateReadOnly = false;
            txtAssignmentDescription.BackgroundImageLayout = ImageLayout.None;
            txtAssignmentDescription.CharacterCasing = CharacterCasing.Normal;
            txtAssignmentDescription.Depth = 0;
            txtAssignmentDescription.HideSelection = true;
            txtAssignmentDescription.Hint = "Mô tả bài tập (tùy chọn)...";
            txtAssignmentDescription.Location = new Point(11, 147);
            txtAssignmentDescription.Margin = new Padding(3, 4, 3, 4);
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
            txtAssignmentDescription.Size = new Size(900, 40);
            txtAssignmentDescription.TabIndex = 4;
            txtAssignmentDescription.TabStop = false;
            txtAssignmentDescription.TextAlign = HorizontalAlignment.Left;
            txtAssignmentDescription.UseSystemPasswordChar = false;
            // 
            // txtDriveLink
            // 
            txtDriveLink.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDriveLink.AnimateReadOnly = false;
            txtDriveLink.BorderStyle = BorderStyle.None;
            txtDriveLink.Depth = 0;
            txtDriveLink.Font = new Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            txtDriveLink.Hint = "Dán Link Google Drive sản phẩm vào đây...";
            txtDriveLink.LeadingIcon = null;
            txtDriveLink.Location = new Point(11, 97);
            txtDriveLink.Margin = new Padding(3, 4, 3, 4);
            txtDriveLink.MaxLength = 500;
            txtDriveLink.MouseState = MaterialSkin.MouseState.OUT;
            txtDriveLink.Multiline = false;
            txtDriveLink.Name = "txtDriveLink";
            txtDriveLink.Size = new Size(900, 50);
            txtDriveLink.TabIndex = 2;
            txtDriveLink.Text = "";
            txtDriveLink.TrailingIcon = null;
            // 
            // txtAssignmentName
            // 
            txtAssignmentName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtAssignmentName.AnimateReadOnly = false;
            txtAssignmentName.BorderStyle = BorderStyle.None;
            txtAssignmentName.Depth = 0;
            txtAssignmentName.Font = new Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            txtAssignmentName.Hint = "Tên bài tập...";
            txtAssignmentName.LeadingIcon = null;
            txtAssignmentName.Location = new Point(11, 47);
            txtAssignmentName.Margin = new Padding(3, 4, 3, 4);
            txtAssignmentName.MaxLength = 50;
            txtAssignmentName.MouseState = MaterialSkin.MouseState.OUT;
            txtAssignmentName.Multiline = false;
            txtAssignmentName.Name = "txtAssignmentName";
            txtAssignmentName.Size = new Size(900, 50);
            txtAssignmentName.TabIndex = 1;
            txtAssignmentName.Text = "";
            txtAssignmentName.TrailingIcon = null;
            // 
            // lblAssignmentTitle
            // 
            lblAssignmentTitle.AutoSize = true;
            lblAssignmentTitle.Depth = 0;
            lblAssignmentTitle.Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
            lblAssignmentTitle.Location = new Point(11, 11);
            lblAssignmentTitle.MouseState = MaterialSkin.MouseState.HOVER;
            lblAssignmentTitle.Name = "lblAssignmentTitle";
            lblAssignmentTitle.Size = new Size(120, 19);
            lblAssignmentTitle.TabIndex = 0;
            lblAssignmentTitle.Text = "Khu vực giao bài";
            // 
            // tabGrading
            // 
            tabGrading.BackColor = Color.White;
            tabGrading.Controls.Add(flpGrading);
            tabGrading.Location = new Point(4, 29);
            tabGrading.Margin = new Padding(3, 4, 3, 4);
            tabGrading.Name = "tabGrading";
            tabGrading.Padding = new Padding(3, 4, 3, 4);
            tabGrading.Size = new Size(963, 634);
            tabGrading.TabIndex = 2;
            tabGrading.Text = "Chấm Điểm";
            tabGrading.UseVisualStyleBackColor = true;
            // 
            // flpGrading
            // 
            flpGrading.AutoScroll = true;
            flpGrading.Dock = DockStyle.Fill;
            flpGrading.Location = new Point(3, 4);
            flpGrading.Margin = new Padding(3, 4, 3, 4);
            flpGrading.Name = "flpGrading";
            flpGrading.Size = new Size(957, 626);
            flpGrading.TabIndex = 0;
            // 
            // lblGradingTitle
            // 
            lblGradingTitle.AutoSize = true;
            lblGradingTitle.Depth = 0;
            lblGradingTitle.Font = new Font("Roboto", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            lblGradingTitle.Location = new Point(10, 10);
            lblGradingTitle.MouseState = MaterialSkin.MouseState.HOVER;
            lblGradingTitle.Name = "lblGradingTitle";
            lblGradingTitle.Size = new Size(200, 19);
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
            lvwSubmissions.Dock = DockStyle.Fill;
            lvwSubmissions.Font = new Font("Microsoft Sans Serif", 24F, FontStyle.Regular, GraphicsUnit.Pixel);
            lvwSubmissions.FullRowSelect = true;
            lvwSubmissions.Location = new Point(0, 0);
            lvwSubmissions.Margin = new Padding(3, 4, 3, 4);
            lvwSubmissions.MinimumSize = new Size(229, 133);
            lvwSubmissions.MouseLocation = new Point(-1, -1);
            lvwSubmissions.MouseState = MaterialSkin.MouseState.OUT;
            lvwSubmissions.Name = "lvwSubmissions";
            lvwSubmissions.OwnerDraw = true;
            lvwSubmissions.Size = new Size(963, 634);
            lvwSubmissions.TabIndex = 0;
            lvwSubmissions.UseCompatibleStateImageBehavior = false;
            lvwSubmissions.View = View.Details;
            // 
            // colStudent
            // 
            colStudent.Text = "Học sinh";
            colStudent.Width = 200;
            // 
            // colAssignment
            // 
            colAssignment.Text = "Bài tập";
            colAssignment.Width = 200;
            // 
            // colLink
            // 
            colLink.Text = "Link Drive";
            colLink.Width = 300;
            // 
            // colScore
            // 
            colScore.Text = "Điểm";
            colScore.Width = 100;
            // 
            // tabMembers
            // 
            tabMembers.BackColor = Color.White;
            tabMembers.Controls.Add(flpMembers);
            tabMembers.Location = new Point(4, 29);
            tabMembers.Margin = new Padding(3, 4, 3, 4);
            tabMembers.Name = "tabMembers";
            tabMembers.Padding = new Padding(3, 4, 3, 4);
            tabMembers.Size = new Size(963, 634);
            tabMembers.TabIndex = 3;
            tabMembers.Text = "Danh sách học sinh";
            tabMembers.UseVisualStyleBackColor = true;
            // 
            // flpMembers
            // 
            flpMembers.AutoScroll = true;
            flpMembers.Dock = DockStyle.Fill;
            flpMembers.Location = new Point(3, 4);
            flpMembers.Margin = new Padding(3, 4, 3, 4);
            flpMembers.Name = "flpMembers";
            flpMembers.Size = new Size(957, 626);
            flpMembers.TabIndex = 0;
            // 
            // lblMembersTitle
            // 
            lblMembersTitle.AutoSize = true;
            lblMembersTitle.Depth = 0;
            lblMembersTitle.Font = new Font("Roboto", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            lblMembersTitle.Location = new Point(10, 10);
            lblMembersTitle.MouseState = MaterialSkin.MouseState.HOVER;
            lblMembersTitle.Name = "lblMembersTitle";
            lblMembersTitle.Size = new Size(200, 19);
            lblMembersTitle.TabIndex = 0;
            lblMembersTitle.Text = "Danh sách thành viên lớp học";
            // 
            // materialTabSelector1
            // 
            materialTabSelector1.BaseTabControl = materialTabControl1;
            materialTabSelector1.CharacterCasing = MaterialSkin.Controls.MaterialTabSelector.CustomCharacterCasing.Normal;
            materialTabSelector1.Depth = 0;
            materialTabSelector1.Dock = DockStyle.Top;
            materialTabSelector1.Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
            materialTabSelector1.Location = new Point(0, 0);
            materialTabSelector1.Margin = new Padding(3, 4, 3, 4);
            materialTabSelector1.MouseState = MaterialSkin.MouseState.HOVER;
            materialTabSelector1.Name = "materialTabSelector1";
            materialTabSelector1.Size = new Size(971, 56);
            materialTabSelector1.TabIndex = 1;
            materialTabSelector1.Click += materialTabSelector1_Click;
            // 
            // UcClassDetail
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            Controls.Add(materialTabSelector1);
            Controls.Add(materialTabControl1);
            Margin = new Padding(3, 4, 3, 4);
            Name = "UcClassDetail";
            Size = new Size(971, 667);
            VisibleChanged += UcClassDetail_VisibleChanged;
            materialTabControl1.ResumeLayout(false);
            tabStream.ResumeLayout(false);
            panelPost.ResumeLayout(false);
            panelPost.PerformLayout();
            tabAssignments.ResumeLayout(false);
            panelCreateAssignment.ResumeLayout(false);
            panelCreateAssignment.PerformLayout();
            tabGrading.ResumeLayout(false);
            tabMembers.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

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
        private System.Windows.Forms.TabPage tabMembers;
        private System.Windows.Forms.FlowLayoutPanel flpMembers;
        private MaterialSkin.Controls.MaterialLabel lblMembersTitle;
    }
}