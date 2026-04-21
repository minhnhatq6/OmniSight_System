namespace OmniSight.UI.Forms
{
    partial class MainForm
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

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            materialTabControl1 = new MaterialSkin.Controls.MaterialTabControl();
            tabHome = new TabPage();
            lblHomeWelcome = new MaterialSkin.Controls.MaterialLabel();
            tabClasses = new TabPage();
            btnOpenJoinClass = new MaterialSkin.Controls.MaterialButton();
            lvwClasses = new MaterialSkin.Controls.MaterialListView();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            btnOpenCreateClass = new MaterialSkin.Controls.MaterialButton();
            tabProfile = new TabPage();
            lblFaceIdTitle = new MaterialSkin.Controls.MaterialLabel();
            picCamera = new PictureBox();
            btnStartCamera = new MaterialSkin.Controls.MaterialButton();
            btnCaptureFace = new MaterialSkin.Controls.MaterialButton();
            btnSaveProfile = new MaterialSkin.Controls.MaterialButton();
            txtPhone = new MaterialSkin.Controls.MaterialTextBox();
            txtFullName = new MaterialSkin.Controls.MaterialTextBox();
            tabSettings = new TabPage();
            btnOpenExamManager = new MaterialSkin.Controls.MaterialButton();
            imageList1 = new ImageList(components);
            cmsUserMenu = new MaterialSkin.Controls.MaterialContextMenuStrip();
            tsmLogout = new ToolStripMenuItem();
            btnUserAccount = new MaterialSkin.Controls.MaterialButton();
            timerCamera = new System.Windows.Forms.Timer(components);
          
            materialTabControl1.SuspendLayout();
            tabHome.SuspendLayout();
            tabClasses.SuspendLayout();
            tabProfile.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picCamera).BeginInit();
            tabSettings.SuspendLayout();
            cmsUserMenu.SuspendLayout();
            SuspendLayout();
            // 
            // materialTabControl1
            // 
            materialTabControl1.Controls.Add(tabHome);
            materialTabControl1.Controls.Add(tabClasses);
            materialTabControl1.Controls.Add(tabProfile);
            materialTabControl1.Controls.Add(tabSettings);
            materialTabControl1.Depth = 0;
            materialTabControl1.Dock = DockStyle.Fill;
            materialTabControl1.ImageList = imageList1;
            materialTabControl1.Location = new Point(3, 48);
            materialTabControl1.Margin = new Padding(3, 2, 3, 2);
            materialTabControl1.MouseState = MaterialSkin.MouseState.HOVER;
            materialTabControl1.Multiline = true;
            materialTabControl1.Name = "materialTabControl1";
            materialTabControl1.SelectedIndex = 0;
            materialTabControl1.Size = new Size(1274, 670);
            materialTabControl1.TabIndex = 0;
            // 
            // tabHome
            // 
            tabHome.Controls.Add(lblHomeWelcome);
            tabHome.ImageKey = "Home";
            tabHome.Location = new Point(4, 31);
            tabHome.Margin = new Padding(3, 2, 3, 2);
            tabHome.Name = "tabHome";
            tabHome.Padding = new Padding(3, 2, 3, 2);
            tabHome.Size = new Size(1266, 635);
            tabHome.TabIndex = 0;
            tabHome.Text = "Trang chủ";
            tabHome.UseVisualStyleBackColor = true;
            // 
            // lblHomeWelcome
            // 
            lblHomeWelcome.AutoSize = true;
            lblHomeWelcome.Depth = 0;
            lblHomeWelcome.Font = new Font("Segoe UI", 24F, FontStyle.Bold, GraphicsUnit.Pixel);
            lblHomeWelcome.FontType = MaterialSkin.MaterialSkinManager.fontType.H5;
            lblHomeWelcome.Location = new Point(35, 30);
            lblHomeWelcome.MouseState = MaterialSkin.MouseState.HOVER;
            lblHomeWelcome.Name = "lblHomeWelcome";
            lblHomeWelcome.Size = new Size(1, 0);
            lblHomeWelcome.TabIndex = 0;
            // 
            // tabClasses
            // 
            tabClasses.BackColor = Color.White;
            tabClasses.ImageKey = "Class";
            tabClasses.Location = new Point(4, 31);
            tabClasses.Margin = new Padding(3, 2, 3, 2);
            tabClasses.Name = "tabClasses";
            tabClasses.Padding = new Padding(20); // Tạo lề 20px cho rộng rãi
            tabClasses.Size = new Size(1266, 635);
            tabClasses.TabIndex = 1;
            tabClasses.Text = "Lớp học của tôi";

            // TẠO THANH CÔNG CỤ (CHỨA TIÊU ĐỀ VÀ NÚT) BẰNG CODE
            Panel pnlClassToolbar = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.White };

            MaterialSkin.Controls.MaterialLabel lblClassTitle = new MaterialSkin.Controls.MaterialLabel
            {
                Text = "Danh sách lớp học của bạn",
                FontType = MaterialSkin.MaterialSkinManager.fontType.H6,
                Location = new Point(0, 5),
                AutoSize = true
            };

            // GOM NÚT VÀO GÓC PHẢI
            FlowLayoutPanel flpClassActions = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                Width = 400,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(0, 5, 0, 0)
            };

            flpClassActions.Controls.Add(btnOpenCreateClass);
            flpClassActions.Controls.Add(btnOpenJoinClass);

            pnlClassToolbar.Controls.Add(lblClassTitle);
            pnlClassToolbar.Controls.Add(flpClassActions);

            // TẠO KHUNG BỌC LISTVIEW ĐỂ CÁCH LỀ TRÊN 20PX
            Panel pnlListWrapper = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 20, 0, 0) };
            pnlListWrapper.Controls.Add(lvwClasses);

            tabClasses.Controls.Add(pnlListWrapper);
            tabClasses.Controls.Add(pnlClassToolbar); // Add sau cùng để neo lên Top chuẩn xác
            // 
            // btnOpenJoinClass
            // 
            btnOpenJoinClass.AutoSize = true;
            btnOpenJoinClass.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnOpenJoinClass.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            btnOpenJoinClass.Depth = 0;
            btnOpenJoinClass.HighEmphasis = true;
            btnOpenJoinClass.Icon = null;
            btnOpenJoinClass.MouseState = MaterialSkin.MouseState.HOVER;
            btnOpenJoinClass.Name = "btnOpenJoinClass";
            btnOpenJoinClass.NoAccentTextColor = Color.Empty;
            btnOpenJoinClass.TabIndex = 2;
            btnOpenJoinClass.Text = "THAM GIA LỚP";
            btnOpenJoinClass.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined; // Đổi thành dạng Viền
            btnOpenJoinClass.UseAccentColor = false;
            btnOpenJoinClass.UseVisualStyleBackColor = true;
            btnOpenJoinClass.Click += btnOpenJoinClass_Click;
            // 
            // lvwClasses
            // 
            lvwClasses.Dock = DockStyle.Fill; // Lấp đầy toàn bộ không gian còn lại
            lvwClasses.AutoSizeTable = false;
            lvwClasses.BackColor = Color.FromArgb(255, 255, 255);
            lvwClasses.BorderStyle = BorderStyle.None;
            lvwClasses.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2 });
            lvwClasses.Depth = 0;
            lvwClasses.FullRowSelect = true;
            lvwClasses.MinimumSize = new Size(200, 100);
            lvwClasses.MouseLocation = new Point(-1, -1);
            lvwClasses.MouseState = MaterialSkin.MouseState.OUT;
            lvwClasses.Name = "lvwClasses";
            lvwClasses.OwnerDraw = true;
            lvwClasses.TabIndex = 1;
            lvwClasses.UseCompatibleStateImageBehavior = false;
            lvwClasses.View = View.Details;
            lvwClasses.MouseDoubleClick += lvwClasses_MouseDoubleClick;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Tên lớp học";
            columnHeader1.Width = 600; // Nới rộng cột

            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Mã tham gia";
            columnHeader2.Width = 250; // Nới rộng cột
            // 
            // btnOpenCreateClass
            // 
            btnOpenCreateClass.AutoSize = true;
            btnOpenCreateClass.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnOpenCreateClass.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            btnOpenCreateClass.Depth = 0;
            btnOpenCreateClass.HighEmphasis = true;
            btnOpenCreateClass.Icon = null;
            btnOpenCreateClass.MouseState = MaterialSkin.MouseState.HOVER;
            btnOpenCreateClass.Name = "btnOpenCreateClass";
            btnOpenCreateClass.NoAccentTextColor = Color.Empty;
            btnOpenCreateClass.TabIndex = 0;
            btnOpenCreateClass.Text = "TẠO LỚP MỚI";
            btnOpenCreateClass.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained; // Đổ màu đặc
            btnOpenCreateClass.UseAccentColor = true; // Dùng màu nhấn (vàng/cam) cho nổi bật
            btnOpenCreateClass.UseVisualStyleBackColor = true;
            btnOpenCreateClass.Click += btnOpenCreateClass_Click;
            // 
            // tabProfile
            // 
            tabProfile.BackColor = Color.White;
            tabProfile.Controls.Add(lblFaceIdTitle);
            tabProfile.Controls.Add(picCamera);
            tabProfile.Controls.Add(btnStartCamera);
            tabProfile.Controls.Add(btnCaptureFace);
            tabProfile.Controls.Add(btnSaveProfile);
             // GIỮ LẠI CÔNG TẮC
            tabProfile.Controls.Add(txtPhone);
            tabProfile.Controls.Add(txtFullName);
            tabProfile.ImageKey = "Account";
            tabProfile.Location = new Point(4, 31);
            tabProfile.Margin = new Padding(3, 2, 3, 2);
            tabProfile.Name = "tabProfile";
            tabProfile.Size = new Size(1266, 635);
            tabProfile.TabIndex = 2;
            tabProfile.Text = "Hồ sơ cá nhân";

            // Tạo thêm Label "Thông Tin Cơ Bản" bằng code cho đẹp
            MaterialSkin.Controls.MaterialLabel lblProfileInfo = new MaterialSkin.Controls.MaterialLabel();
            lblProfileInfo.Text = "Thông Tin Cơ Bản";
            lblProfileInfo.FontType = MaterialSkin.MaterialSkinManager.fontType.H6;
            lblProfileInfo.Location = new Point(150, 50);
            lblProfileInfo.AutoSize = true;
            tabProfile.Controls.Add(lblProfileInfo);
            // 
            // lblFaceIdTitle
            // 
            lblFaceIdTitle.AutoSize = true;
            lblFaceIdTitle.Depth = 0;
            lblFaceIdTitle.FontType = MaterialSkin.MaterialSkinManager.fontType.H6;
            lblFaceIdTitle.Location = new Point(650, 50);
            lblFaceIdTitle.MouseState = MaterialSkin.MouseState.HOVER;
            lblFaceIdTitle.Name = "lblFaceIdTitle";
            lblFaceIdTitle.Size = new Size(200, 19);
            lblFaceIdTitle.TabIndex = 8;
            lblFaceIdTitle.Text = "Thiết Lập Face ID";
            // 
            // picCamera
            // 
            picCamera.BackColor = Color.Black;
            picCamera.BorderStyle = BorderStyle.FixedSingle;
            picCamera.Location = new Point(650, 100);
            picCamera.Name = "picCamera";
            picCamera.Size = new Size(480, 360);
            picCamera.SizeMode = PictureBoxSizeMode.Zoom;
            picCamera.TabIndex = 5;
            picCamera.TabStop = false;
            // 
            // btnStartCamera
            // 
            btnStartCamera.AutoSize = false;
            btnStartCamera.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnStartCamera.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            btnStartCamera.Depth = 0;
            btnStartCamera.HighEmphasis = true;
            btnStartCamera.Icon = null;
            btnStartCamera.Location = new Point(650, 480);
            btnStartCamera.MouseState = MaterialSkin.MouseState.HOVER;
            btnStartCamera.Name = "btnStartCamera";
            btnStartCamera.NoAccentTextColor = Color.Empty;
            btnStartCamera.Size = new Size(160, 40);
            btnStartCamera.TabIndex = 6;
            btnStartCamera.Text = "BẬT CAMERA";
            btnStartCamera.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined;
            btnStartCamera.UseAccentColor = false;
            btnStartCamera.UseVisualStyleBackColor = true;
            btnStartCamera.Click += btnStartCamera_Click;
            // 
            // btnCaptureFace
            // 
            btnCaptureFace.AutoSize = false;
            btnCaptureFace.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnCaptureFace.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            btnCaptureFace.Depth = 0;
            btnCaptureFace.HighEmphasis = true;
            btnCaptureFace.Icon = null;
            btnCaptureFace.Location = new Point(830, 480);
            btnCaptureFace.MouseState = MaterialSkin.MouseState.HOVER;
            btnCaptureFace.Name = "btnCaptureFace";
            btnCaptureFace.NoAccentTextColor = Color.Empty;
            btnCaptureFace.Size = new Size(300, 40);
            btnCaptureFace.TabIndex = 7;
            btnCaptureFace.Text = "QUÉT VÀ LƯU FACE ID";
            btnCaptureFace.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            btnCaptureFace.UseAccentColor = true;
            btnCaptureFace.UseVisualStyleBackColor = true;
            btnCaptureFace.Click += btnCaptureFace_Click;
            // 
            // btnSaveProfile
            // 
            btnSaveProfile.AutoSize = false;
            btnSaveProfile.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnSaveProfile.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            btnSaveProfile.Depth = 0;
            btnSaveProfile.HighEmphasis = true;
            btnSaveProfile.Icon = null;
            btnSaveProfile.Location = new Point(150, 350); // Đẩy nút lưu xuống dưới công tắc
            btnSaveProfile.MouseState = MaterialSkin.MouseState.HOVER;
            btnSaveProfile.Name = "btnSaveProfile";
            btnSaveProfile.NoAccentTextColor = Color.Empty;
            btnSaveProfile.Size = new Size(180, 40);
            btnSaveProfile.TabIndex = 4;
            btnSaveProfile.Text = "LƯU THAY ĐỔI";
            btnSaveProfile.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            btnSaveProfile.UseAccentColor = false;
            btnSaveProfile.UseVisualStyleBackColor = true;
            btnSaveProfile.Click += btnSaveProfile_Click;

            // 
            // txtPhone
            // 
            txtPhone.AnimateReadOnly = false;
            txtPhone.BorderStyle = BorderStyle.None;
            txtPhone.Depth = 0;
            txtPhone.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            txtPhone.Hint = "Số điện thoại";
            txtPhone.LeadingIcon = null;
            txtPhone.Location = new Point(150, 180);
            txtPhone.MaxLength = 15;
            txtPhone.MouseState = MaterialSkin.MouseState.OUT;
            txtPhone.Multiline = false;
            txtPhone.Name = "txtPhone";
            txtPhone.Size = new Size(400, 50);
            txtPhone.TabIndex = 1;
            // 
            // txtFullName
            // 
            txtFullName.AnimateReadOnly = false;
            txtFullName.BorderStyle = BorderStyle.None;
            txtFullName.Depth = 0;
            txtFullName.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            txtFullName.Hint = "Họ và tên";
            txtFullName.LeadingIcon = null;
            txtFullName.Location = new Point(150, 100);
            txtFullName.MaxLength = 100;
            txtFullName.MouseState = MaterialSkin.MouseState.OUT;
            txtFullName.Multiline = false;
            txtFullName.Name = "txtFullName";
            txtFullName.Size = new Size(400, 50);
            txtFullName.TabIndex = 0;
            // 
            // tabSettings
            // 
            tabSettings.Controls.Add(btnOpenExamManager);
            tabSettings.ImageKey = "Settings";
            tabSettings.Location = new Point(4, 31);
            tabSettings.Margin = new Padding(3, 2, 3, 2);
            tabSettings.Name = "tabSettings";
            tabSettings.Size = new Size(1266, 635);
            tabSettings.TabIndex = 3;
            tabSettings.Text = "Cài đặt";
            tabSettings.UseVisualStyleBackColor = true;
            // 
            // btnOpenExamManager
            // 
            btnOpenExamManager.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnOpenExamManager.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnOpenExamManager.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            btnOpenExamManager.Depth = 0;
            btnOpenExamManager.HighEmphasis = true;
            btnOpenExamManager.Icon = null;
            btnOpenExamManager.Location = new Point(609, 15);
            btnOpenExamManager.Margin = new Padding(4, 6, 4, 6);
            btnOpenExamManager.MouseState = MaterialSkin.MouseState.HOVER;
            btnOpenExamManager.Name = "btnOpenExamManager";
            btnOpenExamManager.NoAccentTextColor = Color.Empty;
            btnOpenExamManager.Size = new Size(131, 36);
            btnOpenExamManager.TabIndex = 10;
            btnOpenExamManager.Text = "Quản lý đề thi";
            btnOpenExamManager.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            btnOpenExamManager.UseAccentColor = false;
            btnOpenExamManager.UseVisualStyleBackColor = true;
            btnOpenExamManager.Click += btnOpenExamManager_Click;
            // 
            // imageList1
            // 
            imageList1.ColorDepth = ColorDepth.Depth32Bit;
            imageList1.ImageStream = (ImageListStreamer)resources.GetObject("imageList1.ImageStream");
            imageList1.TransparentColor = Color.Transparent;
            imageList1.Images.SetKeyName(0, "Account");
            imageList1.Images.SetKeyName(1, "Settings");
            imageList1.Images.SetKeyName(2, "Class");
            imageList1.Images.SetKeyName(3, "Home");
            // 
            // cmsUserMenu
            // 
            cmsUserMenu.BackColor = Color.FromArgb(242, 242, 242);
            cmsUserMenu.Depth = 0;
            cmsUserMenu.Items.AddRange(new ToolStripItem[] { tsmLogout });
            cmsUserMenu.MouseState = MaterialSkin.MouseState.HOVER;
            cmsUserMenu.Name = "cmsUserMenu";
            cmsUserMenu.Size = new Size(129, 26);
            // 
            // tsmLogout
            // 
            tsmLogout.Name = "tsmLogout";
            tsmLogout.Size = new Size(128, 22);
            tsmLogout.Text = "Đăng xuất";
            tsmLogout.Click += tsmLogout_Click;
            // 
            // btnUserAccount
            // 
            btnUserAccount.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnUserAccount.AutoSize = false;
            btnUserAccount.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnUserAccount.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            btnUserAccount.Depth = 0;
            btnUserAccount.HighEmphasis = true;
            btnUserAccount.Icon = null;
            btnUserAccount.Location = new Point(1053, 20);
            btnUserAccount.Margin = new Padding(4);
            btnUserAccount.MouseState = MaterialSkin.MouseState.HOVER;
            btnUserAccount.Name = "btnUserAccount";
            btnUserAccount.NoAccentTextColor = Color.Empty;
            btnUserAccount.Size = new Size(219, 22);
            btnUserAccount.TabIndex = 1;
            btnUserAccount.Text = "TÀI KHOẢN";
            btnUserAccount.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            btnUserAccount.UseAccentColor = false;
            btnUserAccount.UseVisualStyleBackColor = true;
            btnUserAccount.Click += btnUserAccount_Click;
            // 
            // timerCamera
            // 
            timerCamera.Interval = 30;
            timerCamera.Tick += timerCamera_Tick;

            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1280, 720);
            Controls.Add(btnUserAccount);
            Controls.Add(materialTabControl1);
            DrawerShowIconsWhenHidden = true;
            DrawerTabControl = materialTabControl1;
            Margin = new Padding(3, 2, 3, 2);
            Name = "MainForm";
            Padding = new Padding(3, 48, 3, 2);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "OmniSight Dashboard";
            FormClosed += MainForm_FormClosed;
            Load += MainForm_Load;
            materialTabControl1.ResumeLayout(false);
            tabHome.ResumeLayout(false);
            tabHome.PerformLayout();
            tabClasses.ResumeLayout(false);
            tabClasses.PerformLayout();
            tabProfile.ResumeLayout(false);
            tabProfile.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picCamera).EndInit();
            tabSettings.ResumeLayout(false);
            tabSettings.PerformLayout();
            cmsUserMenu.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private MaterialSkin.Controls.MaterialTabControl materialTabControl1;
        private System.Windows.Forms.TabPage tabHome;
        private System.Windows.Forms.TabPage tabClasses;
        private System.Windows.Forms.TabPage tabProfile;
        private System.Windows.Forms.TabPage tabSettings;
        private System.Windows.Forms.ImageList imageList1;
        private MaterialSkin.Controls.MaterialLabel lblHomeWelcome;
        private MaterialSkin.Controls.MaterialTextBox txtFullName;
        private MaterialSkin.Controls.MaterialTextBox txtPhone;
        private MaterialSkin.Controls.MaterialButton btnSaveProfile;
        private MaterialSkin.Controls.MaterialContextMenuStrip cmsUserMenu;
        private System.Windows.Forms.ToolStripMenuItem tsmLogout;
        private MaterialSkin.Controls.MaterialButton btnUserAccount;

        // --- FACE ID CONTROLS ---
        private System.Windows.Forms.PictureBox picCamera;
        private MaterialSkin.Controls.MaterialButton btnStartCamera;
        private MaterialSkin.Controls.MaterialButton btnCaptureFace;
        private MaterialSkin.Controls.MaterialLabel lblFaceIdTitle;
        private System.Windows.Forms.Timer timerCamera;
        private MaterialSkin.Controls.MaterialButton btnOpenCreateClass;
        private MaterialSkin.Controls.MaterialListView lvwClasses;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private MaterialSkin.Controls.MaterialButton btnOpenJoinClass;
        private MaterialSkin.Controls.MaterialButton btnOpenExamManager;
     
    }
}
