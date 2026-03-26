namespace OmniSight.UI.Forms.Auth
{
    partial class FrmSetPassword
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing) { if (disposing && (components != null)) components.Dispose(); base.Dispose(disposing); }

        private void InitializeComponent()
        {
            this.lblInfo = new MaterialSkin.Controls.MaterialLabel();
            this.txtNewPassword = new MaterialSkin.Controls.MaterialTextBox();
            this.btnSave = new MaterialSkin.Controls.MaterialButton();
            this.SuspendLayout();
            // lblInfo
            this.lblInfo.Depth = 0;
            this.lblInfo.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblInfo.Location = new System.Drawing.Point(25, 80);
            this.lblInfo.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(350, 45);
            this.lblInfo.TabIndex = 0;
            this.lblInfo.Text = "Bạn vừa đăng nhập bằng Google. Hãy thiết lập mật khẩu để đăng nhập bằng Email sau này.";
            // txtNewPassword
            this.txtNewPassword.AnimateReadOnly = false;
            this.txtNewPassword.Depth = 0;
            this.txtNewPassword.Hint = "Nhập mật khẩu mới";
            this.txtNewPassword.Location = new System.Drawing.Point(25, 140);
            this.txtNewPassword.MaxLength = 50;
            this.txtNewPassword.MouseState = MaterialSkin.MouseState.OUT;
            this.txtNewPassword.Name = "txtNewPassword";
            this.txtNewPassword.Password = true;
            this.txtNewPassword.Size = new System.Drawing.Size(350, 50);
            this.txtNewPassword.TabIndex = 1;
            // btnSave
            this.btnSave.AutoSize = false;
            this.btnSave.Depth = 0;
            this.btnSave.HighEmphasis = true;
            this.btnSave.Location = new System.Drawing.Point(25, 210);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(350, 36);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "LƯU MẬT KHẨU VÀ VÀO TRANG CHỦ";
            this.btnSave.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // Form
            this.ClientSize = new System.Drawing.Size(400, 280);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtNewPassword);
            this.Controls.Add(this.lblInfo);
            this.Name = "FrmSetPassword";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Thiết lập mật khẩu";
            this.ResumeLayout(false);
        }
        private MaterialSkin.Controls.MaterialLabel lblInfo;
        private MaterialSkin.Controls.MaterialTextBox txtNewPassword;
        private MaterialSkin.Controls.MaterialButton btnSave;
    }
}