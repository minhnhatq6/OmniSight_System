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
            this.components = new System.ComponentModel.Container();
            this.materialTabControl1 = new MaterialSkin.Controls.MaterialTabControl();
            this.tabHome = new System.Windows.Forms.TabPage();
            this.lblHomeWelcome = new MaterialSkin.Controls.MaterialLabel();
            this.tabClasses = new System.Windows.Forms.TabPage();
            this.tabProfile = new System.Windows.Forms.TabPage();
            this.btnSaveProfile = new MaterialSkin.Controls.MaterialButton();
            this.switchTeacher = new MaterialSkin.Controls.MaterialSwitch();
            this.switchStudent = new MaterialSkin.Controls.MaterialSwitch();
            this.txtPhone = new MaterialSkin.Controls.MaterialTextBox();
            this.txtFullName = new MaterialSkin.Controls.MaterialTextBox();
            this.tabSettings = new System.Windows.Forms.TabPage();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.cmsUserMenu = new MaterialSkin.Controls.MaterialContextMenuStrip();
            this.tsmLogout = new MaterialSkin.Controls.MaterialToolStripMenuItem();
            this.btnUserAccount = new MaterialSkin.Controls.MaterialButton();
            this.materialTabControl1.SuspendLayout();
            this.tabHome.SuspendLayout();
            this.tabProfile.SuspendLayout();
            this.cmsUserMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // materialTabControl1
            // 
            this.materialTabControl1.Controls.Add(this.tabHome);
            this.materialTabControl1.Controls.Add(this.tabClasses);
            this.materialTabControl1.Controls.Add(this.tabProfile);
            this.materialTabControl1.Controls.Add(this.tabSettings);
            this.materialTabControl1.Depth = 0;
            this.materialTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.materialTabControl1.ImageList = this.imageList1;
            this.materialTabControl1.Location = new System.Drawing.Point(3, 64);
            this.materialTabControl1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialTabControl1.Multiline = true;
            this.materialTabControl1.Name = "materialTabControl1";
            this.materialTabControl1.SelectedIndex = 0;
            this.materialTabControl1.Size = new System.Drawing.Size(994, 533);
            this.materialTabControl1.TabIndex = 0;
            // 
            // tabHome
            // 
            this.tabHome.Controls.Add(this.lblHomeWelcome);
            this.tabHome.Location = new System.Drawing.Point(4, 39);
            this.tabHome.Name = "tabHome";
            this.tabHome.Padding = new System.Windows.Forms.Padding(3);
            this.tabHome.Size = new System.Drawing.Size(986, 490);
            this.tabHome.TabIndex = 0;
            this.tabHome.Text = "Trang chủ";
            this.tabHome.UseVisualStyleBackColor = true;
            // 
            // lblHomeWelcome
            // 
            this.lblHomeWelcome.AutoSize = true;
            this.lblHomeWelcome.Depth = 0;
            this.lblHomeWelcome.Font = new System.Drawing.Font("Roboto", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.lblHomeWelcome.FontType = MaterialSkin.MaterialSkinManager.fontType.H5;
            this.lblHomeWelcome.Location = new System.Drawing.Point(40, 40);
            this.lblHomeWelcome.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblHomeWelcome.Name = "lblHomeWelcome";
            this.lblHomeWelcome.Size = new System.Drawing.Size(1, 0);
            this.lblHomeWelcome.TabIndex = 0;
            // 
            // tabClasses
            // 
            this.tabClasses.Location = new System.Drawing.Point(4, 39);
            this.tabClasses.Name = "tabClasses";
            this.tabClasses.Padding = new System.Windows.Forms.Padding(3);
            this.tabClasses.Size = new System.Drawing.Size(986, 490);
            this.tabClasses.TabIndex = 1;
            this.tabClasses.Text = "Lớp học của tôi";
            this.tabClasses.UseVisualStyleBackColor = true;
            // 
            // tabProfile
            // 
            this.tabProfile.Controls.Add(this.btnSaveProfile);
            this.tabProfile.Controls.Add(this.switchTeacher);
            this.tabProfile.Controls.Add(this.switchStudent);
            this.tabProfile.Controls.Add(this.txtPhone);
            this.tabProfile.Controls.Add(this.txtFullName);
            this.tabProfile.Location = new System.Drawing.Point(4, 39);
            this.tabProfile.Name = "tabProfile";
            this.tabProfile.Size = new System.Drawing.Size(986, 490);
            this.tabProfile.TabIndex = 2;
            this.tabProfile.Text = "Hồ sơ cá nhân";
            this.tabProfile.UseVisualStyleBackColor = true;
            // 
            // btnSaveProfile
            // 
            this.btnSaveProfile.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSaveProfile.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnSaveProfile.Depth = 0;
            this.btnSaveProfile.HighEmphasis = true;
            this.btnSaveProfile.Icon = null;
            this.btnSaveProfile.Location = new System.Drawing.Point(40, 300);
            this.btnSaveProfile.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnSaveProfile.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnSaveProfile.Name = "btnSaveProfile";
            this.btnSaveProfile.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnSaveProfile.Size = new System.Drawing.Size(127, 36);
            this.btnSaveProfile.TabIndex = 4;
            this.btnSaveProfile.Text = "LƯU THAY ĐỔI";
            this.btnSaveProfile.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnSaveProfile.UseAccentColor = false;
            this.btnSaveProfile.UseVisualStyleBackColor = true;
            this.btnSaveProfile.Click += new System.EventHandler(this.btnSaveProfile_Click);
            // 
            // txtFullName
            // 
            this.txtFullName.AnimateReadOnly = false;
            this.txtFullName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtFullName.Depth = 0;
            this.txtFullName.Font = new System.Drawing.Font("Roboto", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtFullName.Hint = "Họ và tên";
            this.txtFullName.Location = new System.Drawing.Point(40, 40);
            this.txtFullName.MaxLength = 100;
            this.txtFullName.MouseState = MaterialSkin.MouseState.OUT;
            this.txtFullName.Multiline = false;
            this.txtFullName.Name = "txtFullName";
            this.txtFullName.Size = new System.Drawing.Size(350, 50);
            this.txtFullName.TabIndex = 0;
            this.txtFullName.Text = "";
            // 
            // txtPhone
            // 
            this.txtPhone.AnimateReadOnly = false;
            this.txtPhone.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtPhone.Depth = 0;
            this.txtPhone.Font = new System.Drawing.Font("Roboto", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtPhone.Hint = "Số điện thoại";
            this.txtPhone.Location = new System.Drawing.Point(40, 110);
            this.txtPhone.MaxLength = 15;
            this.txtPhone.MouseState = MaterialSkin.MouseState.OUT;
            this.txtPhone.Multiline = false;
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Size = new System.Drawing.Size(350, 50);
            this.txtPhone.TabIndex = 1;
            this.txtPhone.Text = "";
            // 
            // switchStudent
            // 
            this.switchStudent.AutoSize = true;
            this.switchStudent.Depth = 0;
            this.switchStudent.Location = new System.Drawing.Point(40, 180);
            this.switchStudent.Margin = new System.Windows.Forms.Padding(0);
            this.switchStudent.MouseLocation = new System.Drawing.Point(-1, -1);
            this.switchStudent.MouseState = MaterialSkin.MouseState.HOVER;
            this.switchStudent.Name = "switchStudent";
            this.switchStudent.Ripple = true;
            this.switchStudent.Size = new System.Drawing.Size(155, 37);
            this.switchStudent.TabIndex = 2;
            this.switchStudent.Text = "Vai trò Học sinh";
            this.switchStudent.UseVisualStyleBackColor = true;
            // 
            // switchTeacher
            // 
            this.switchTeacher.AutoSize = true;
            this.switchTeacher.Depth = 0;
            this.switchTeacher.Location = new System.Drawing.Point(40, 230);
            this.switchTeacher.Margin = new System.Windows.Forms.Padding(0);
            this.switchTeacher.MouseLocation = new System.Drawing.Point(-1, -1);
            this.switchTeacher.MouseState = MaterialSkin.MouseState.HOVER;
            this.switchTeacher.Name = "switchTeacher";
            this.switchTeacher.Ripple = true;
            this.switchTeacher.Size = new System.Drawing.Size(160, 37);
            this.switchTeacher.TabIndex = 3;
            this.switchTeacher.Text = "Vai trò Giáo viên";
            this.switchTeacher.UseVisualStyleBackColor = true;
            // 
            // tabSettings
            // 
            this.tabSettings.Location = new System.Drawing.Point(4, 39);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Size = new System.Drawing.Size(986, 490);
            this.tabSettings.TabIndex = 3;
            this.tabSettings.Text = "Cài đặt";
            this.tabSettings.UseVisualStyleBackColor = true;
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(32, 32);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // cmsUserMenu
            // 
            this.cmsUserMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.cmsUserMenu.Depth = 0;
            this.cmsUserMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmLogout});
            this.cmsUserMenu.MouseState = MaterialSkin.MouseState.HOVER;
            this.cmsUserMenu.Name = "cmsUserMenu";
            this.cmsUserMenu.Size = new System.Drawing.Size(130, 30);
            // 
            // tsmLogout
            // 
            this.tsmLogout.Name = "tsmLogout";
            this.tsmLogout.Size = new System.Drawing.Size(129, 24);
            this.tsmLogout.Text = "Đăng xuất";
            this.tsmLogout.Click += new System.EventHandler(this.tsmLogout_Click);
            // 
            // btnUserAccount
            // 
            this.btnUserAccount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUserAccount.AutoSize = false;
            this.btnUserAccount.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnUserAccount.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnUserAccount.Depth = 0;
            this.btnUserAccount.HighEmphasis = true;
            this.btnUserAccount.Icon = null;
            this.btnUserAccount.Location = new System.Drawing.Point(740, 27);
            this.btnUserAccount.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnUserAccount.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnUserAccount.Name = "btnUserAccount";
            this.btnUserAccount.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnUserAccount.Size = new System.Drawing.Size(250, 30);
            this.btnUserAccount.TabIndex = 1;
            this.btnUserAccount.Text = "TÀI KHOẢN";
            this.btnUserAccount.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnUserAccount.UseAccentColor = false;
            this.btnUserAccount.UseVisualStyleBackColor = true;
            this.btnUserAccount.Click += new System.EventHandler(this.btnUserAccount_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 600);
            this.Controls.Add(this.btnUserAccount);
            this.Controls.Add(this.materialTabControl1);
            this.DrawerShowIconsWhenHidden = true;
            this.DrawerTabControl = this.materialTabControl1;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OmniSight Dashboard";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.materialTabControl1.ResumeLayout(false);
            this.tabHome.ResumeLayout(false);
            this.tabHome.PerformLayout();
            this.tabProfile.ResumeLayout(false);
            this.tabProfile.PerformLayout();
            this.cmsUserMenu.ResumeLayout(false);
            this.ResumeLayout(false);

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
    }
}