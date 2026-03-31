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
            switchTeacher = new MaterialSkin.Controls.MaterialSwitch();
            switchStudent = new MaterialSkin.Controls.MaterialSwitch();
            txtPhone = new MaterialSkin.Controls.MaterialTextBox();
            txtFullName = new MaterialSkin.Controls.MaterialTextBox();
            tabSettings = new TabPage();
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
            materialTabControl1.Size = new Size(869, 400);
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
            tabHome.Size = new Size(861, 365);
            tabHome.TabIndex = 0;
            tabHome.Text = "Trang chủ";
            tabHome.UseVisualStyleBackColor = true;
            // 
            // lblHomeWelcome
            // 
            lblHomeWelcome.AutoSize = true;
            lblHomeWelcome.Depth = 0;
            lblHomeWelcome.Font = new Font("Roboto", 24F, FontStyle.Bold, GraphicsUnit.Pixel);
            lblHomeWelcome.FontType = MaterialSkin.MaterialSkinManager.fontType.H5;
            lblHomeWelcome.Location = new Point(35, 30);
            lblHomeWelcome.MouseState = MaterialSkin.MouseState.HOVER;
            lblHomeWelcome.Name = "lblHomeWelcome";
            lblHomeWelcome.Size = new Size(1, 0);
            lblHomeWelcome.TabIndex = 0;
            // 
            // tabClasses
            // 
            tabClasses.Controls.Add(btnOpenJoinClass);
            tabClasses.Controls.Add(lvwClasses);
            tabClasses.Controls.Add(btnOpenCreateClass);
            tabClasses.ImageKey = "Class";
            tabClasses.Location = new Point(4, 31);
            tabClasses.Margin = new Padding(3, 2, 3, 2);
            tabClasses.Name = "tabClasses";
            tabClasses.Padding = new Padding(15);
            tabClasses.Size = new Size(861, 365);
            tabClasses.TabIndex = 1;
            tabClasses.Text = "Lớp học của tôi";
            tabClasses.UseVisualStyleBackColor = true;
            // 
            // btnOpenJoinClass
            // 
            btnOpenJoinClass.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnOpenJoinClass.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnOpenJoinClass.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            btnOpenJoinClass.Depth = 0;
            btnOpenJoinClass.HighEmphasis = true;
            btnOpenJoinClass.Icon = null;
            btnOpenJoinClass.Location = new Point(598, 15);
            btnOpenJoinClass.Margin = new Padding(4, 6, 4, 6);
            btnOpenJoinClass.MouseState = MaterialSkin.MouseState.HOVER;
            btnOpenJoinClass.Name = "btnOpenJoinClass";
            btnOpenJoinClass.NoAccentTextColor = Color.Empty;
            btnOpenJoinClass.Size = new Size(122, 36);
            btnOpenJoinClass.TabIndex = 2;
            btnOpenJoinClass.Text = "Tham Gia Lớp";
            btnOpenJoinClass.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            btnOpenJoinClass.UseAccentColor = false;
            btnOpenJoinClass.UseVisualStyleBackColor = true;
            btnOpenJoinClass.Click += btnOpenJoinClass_Click;
            // 
            // lvwClasses
            // 
            lvwClasses.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lvwClasses.AutoSizeTable = false;
            lvwClasses.BackColor = Color.FromArgb(255, 255, 255);
            lvwClasses.BorderStyle = BorderStyle.None;
            lvwClasses.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2 });
            lvwClasses.Depth = 0;
            lvwClasses.FullRowSelect = true;
            lvwClasses.Location = new Point(15, 60);
            lvwClasses.MinimumSize = new Size(200, 100);
            lvwClasses.MouseLocation = new Point(-1, -1);
            lvwClasses.MouseState = MaterialSkin.MouseState.OUT;
            lvwClasses.Name = "lvwClasses";
            lvwClasses.OwnerDraw = true;
            lvwClasses.Size = new Size(825, 290);
            lvwClasses.TabIndex = 1;
            lvwClasses.UseCompatibleStateImageBehavior = false;
            lvwClasses.View = View.Details;
            lvwClasses.MouseDoubleClick += lvwClasses_MouseDoubleClick;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Tên lớp học";
            columnHeader1.Width = 500;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Mã tham gia";
            columnHeader2.Width = 200;
            // 
            // btnOpenCreateClass
            // 
            btnOpenCreateClass.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnOpenCreateClass.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnOpenCreateClass.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            btnOpenCreateClass.Depth = 0;
            btnOpenCreateClass.HighEmphasis = true;
            btnOpenCreateClass.Icon = null;
            btnOpenCreateClass.Location = new Point(730, 15);
            btnOpenCreateClass.Margin = new Padding(4, 6, 4, 6);
            btnOpenCreateClass.MouseState = MaterialSkin.MouseState.HOVER;
            btnOpenCreateClass.Name = "btnOpenCreateClass";
            btnOpenCreateClass.NoAccentTextColor = Color.Empty;
            btnOpenCreateClass.Size = new Size(112, 36);
            btnOpenCreateClass.TabIndex = 0;
            btnOpenCreateClass.Text = "Tạo lớp mới";
            btnOpenCreateClass.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            btnOpenCreateClass.UseAccentColor = false;
            btnOpenCreateClass.UseVisualStyleBackColor = true;
            btnOpenCreateClass.Click += btnOpenCreateClass_Click;
            // 
            // tabProfile
            // 
            tabProfile.Controls.Add(lblFaceIdTitle);
            tabProfile.Controls.Add(picCamera);
            tabProfile.Controls.Add(btnStartCamera);
            tabProfile.Controls.Add(btnCaptureFace);
            tabProfile.Controls.Add(btnSaveProfile);
            tabProfile.Controls.Add(switchTeacher);
            tabProfile.Controls.Add(switchStudent);
            tabProfile.Controls.Add(txtPhone);
            tabProfile.Controls.Add(txtFullName);
            tabProfile.ImageKey = "Account";
            tabProfile.Location = new Point(4, 31);
            tabProfile.Margin = new Padding(3, 2, 3, 2);
            tabProfile.Name = "tabProfile";
            tabProfile.Size = new Size(861, 365);
            tabProfile.TabIndex = 2;
            tabProfile.Text = "Hồ sơ cá nhân";
            tabProfile.UseVisualStyleBackColor = true;
            // 
            // lblFaceIdTitle
            // 
            lblFaceIdTitle.AutoSize = true;
            lblFaceIdTitle.Depth = 0;
            lblFaceIdTitle.Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
            lblFaceIdTitle.Location = new Point(394, 11);
            lblFaceIdTitle.MouseState = MaterialSkin.MouseState.HOVER;
            lblFaceIdTitle.Name = "lblFaceIdTitle";
            lblFaceIdTitle.Size = new Size(120, 19);
            lblFaceIdTitle.TabIndex = 8;
            lblFaceIdTitle.Text = "Thiết lập Face ID";
            // 
            // picCamera
            // 
            picCamera.BackColor = Color.Black;
            picCamera.BorderStyle = BorderStyle.FixedSingle;
            picCamera.Location = new Point(394, 30);
            picCamera.Margin = new Padding(3, 2, 3, 2);
            picCamera.Name = "picCamera";
            picCamera.Size = new Size(420, 203);
            picCamera.SizeMode = PictureBoxSizeMode.StretchImage;
            picCamera.TabIndex = 5;
            picCamera.TabStop = false;
            // 
            // btnStartCamera
            // 
            btnStartCamera.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnStartCamera.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            btnStartCamera.Depth = 0;
            btnStartCamera.HighEmphasis = true;
            btnStartCamera.Icon = null;
            btnStartCamera.Location = new Point(394, 244);
            btnStartCamera.Margin = new Padding(4);
            btnStartCamera.MouseState = MaterialSkin.MouseState.HOVER;
            btnStartCamera.Name = "btnStartCamera";
            btnStartCamera.NoAccentTextColor = Color.Empty;
            btnStartCamera.Size = new Size(112, 36);
            btnStartCamera.TabIndex = 6;
            btnStartCamera.Text = "BẬT CAMERA";
            btnStartCamera.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            btnStartCamera.UseAccentColor = false;
            btnStartCamera.UseVisualStyleBackColor = true;
            btnStartCamera.Click += btnStartCamera_Click;
            // 
            // btnCaptureFace
            // 
            btnCaptureFace.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnCaptureFace.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            btnCaptureFace.Depth = 0;
            btnCaptureFace.HighEmphasis = true;
            btnCaptureFace.Icon = null;
            btnCaptureFace.Location = new Point(508, 244);
            btnCaptureFace.Margin = new Padding(4);
            btnCaptureFace.MouseState = MaterialSkin.MouseState.HOVER;
            btnCaptureFace.Name = "btnCaptureFace";
            btnCaptureFace.NoAccentTextColor = Color.Empty;
            btnCaptureFace.Size = new Size(160, 36);
            btnCaptureFace.TabIndex = 7;
            btnCaptureFace.Text = "QUÉT & LƯU FACE ID";
            btnCaptureFace.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            btnCaptureFace.UseAccentColor = true;
            btnCaptureFace.UseVisualStyleBackColor = true;
            btnCaptureFace.Click += btnCaptureFace_Click;
            // 
            // btnSaveProfile
            // 
            btnSaveProfile.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnSaveProfile.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            btnSaveProfile.Depth = 0;
            btnSaveProfile.HighEmphasis = true;
            btnSaveProfile.Icon = null;
            btnSaveProfile.Location = new Point(35, 225);
            btnSaveProfile.Margin = new Padding(4);
            btnSaveProfile.MouseState = MaterialSkin.MouseState.HOVER;
            btnSaveProfile.Name = "btnSaveProfile";
            btnSaveProfile.NoAccentTextColor = Color.Empty;
            btnSaveProfile.Size = new Size(119, 36);
            btnSaveProfile.TabIndex = 4;
            btnSaveProfile.Text = "LƯU THAY ĐỔI";
            btnSaveProfile.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            btnSaveProfile.UseAccentColor = false;
            btnSaveProfile.UseVisualStyleBackColor = true;
            btnSaveProfile.Click += btnSaveProfile_Click;
            // 
            // switchTeacher
            // 
            switchTeacher.AutoSize = true;
            switchTeacher.Depth = 0;
            switchTeacher.Location = new Point(35, 172);
            switchTeacher.Margin = new Padding(0);
            switchTeacher.MouseLocation = new Point(-1, -1);
            switchTeacher.MouseState = MaterialSkin.MouseState.HOVER;
            switchTeacher.Name = "switchTeacher";
            switchTeacher.Ripple = true;
            switchTeacher.Size = new Size(174, 37);
            switchTeacher.TabIndex = 3;
            switchTeacher.Text = "Vai trò Giáo viên";
            switchTeacher.UseVisualStyleBackColor = true;
            // 
            // switchStudent
            // 
            switchStudent.AutoSize = true;
            switchStudent.Depth = 0;
            switchStudent.Location = new Point(35, 135);
            switchStudent.Margin = new Padding(0);
            switchStudent.MouseLocation = new Point(-1, -1);
            switchStudent.MouseState = MaterialSkin.MouseState.HOVER;
            switchStudent.Name = "switchStudent";
            switchStudent.Ripple = true;
            switchStudent.Size = new Size(170, 37);
            switchStudent.TabIndex = 2;
            switchStudent.Text = "Vai trò Học sinh";
            switchStudent.UseVisualStyleBackColor = true;
            // 
            // txtPhone
            // 
            txtPhone.AnimateReadOnly = false;
            txtPhone.BorderStyle = BorderStyle.None;
            txtPhone.Depth = 0;
            txtPhone.Font = new Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            txtPhone.Hint = "Số điện thoại";
            txtPhone.LeadingIcon = null;
            txtPhone.Location = new Point(35, 82);
            txtPhone.Margin = new Padding(3, 2, 3, 2);
            txtPhone.MaxLength = 15;
            txtPhone.MouseState = MaterialSkin.MouseState.OUT;
            txtPhone.Multiline = false;
            txtPhone.Name = "txtPhone";
            txtPhone.Size = new Size(306, 50);
            txtPhone.TabIndex = 1;
            txtPhone.Text = "";
            txtPhone.TrailingIcon = null;
            // 
            // txtFullName
            // 
            txtFullName.AnimateReadOnly = false;
            txtFullName.BorderStyle = BorderStyle.None;
            txtFullName.Depth = 0;
            txtFullName.Font = new Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            txtFullName.Hint = "Họ và tên";
            txtFullName.LeadingIcon = null;
            txtFullName.Location = new Point(35, 30);
            txtFullName.Margin = new Padding(3, 2, 3, 2);
            txtFullName.MaxLength = 100;
            txtFullName.MouseState = MaterialSkin.MouseState.OUT;
            txtFullName.Multiline = false;
            txtFullName.Name = "txtFullName";
            txtFullName.Size = new Size(306, 50);
            txtFullName.TabIndex = 0;
            txtFullName.Text = "";
            txtFullName.TrailingIcon = null;
            // 
            // tabSettings
            // 
            tabSettings.ImageKey = "Settings";
            tabSettings.Location = new Point(4, 31);
            tabSettings.Margin = new Padding(3, 2, 3, 2);
            tabSettings.Name = "tabSettings";
            tabSettings.Size = new Size(861, 365);
            tabSettings.TabIndex = 3;
            tabSettings.Text = "Cài đặt";
            tabSettings.UseVisualStyleBackColor = true;
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
            btnUserAccount.Location = new Point(648, 20);
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
            ClientSize = new Size(875, 450);
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
        private MaterialSkin.Controls.MaterialSwitch switchStudent;
        private MaterialSkin.Controls.MaterialSwitch switchTeacher;
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
    }
}