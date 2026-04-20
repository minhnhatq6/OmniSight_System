namespace OmniSight.UI.Forms.Auth
{
    partial class FrmSetPassword
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing) { if (disposing && (components != null)) components.Dispose(); base.Dispose(disposing); }

        private void InitializeComponent()
        {
            this.lblInfo = new System.Windows.Forms.Label();
            this.txtNewPassword = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // lblInfo
            this.lblInfo.Font = new System.Drawing.Font("Roboto", 9F, System.Drawing.FontStyle.Regular);
            this.lblInfo.Location = new System.Drawing.Point(25, 80);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(350, 45);
            this.lblInfo.TabIndex = 0;
            this.lblInfo.Text = "Bạn vừa đăng nhập bằng Google. Hãy thiết lập mật khẩu để đăng nhập bằng Email sau này.";
            // txtNewPassword
            this.txtNewPassword.Font = new System.Drawing.Font("Roboto", 11F);
            this.txtNewPassword.Location = new System.Drawing.Point(25, 140);
            this.txtNewPassword.MaxLength = 50;
            this.txtNewPassword.Name = "txtNewPassword";
            this.txtNewPassword.PasswordChar = '●';
            this.txtNewPassword.PlaceholderText = "Nhập mật khẩu mới";
            this.txtNewPassword.Size = new System.Drawing.Size(350, 50);
            this.txtNewPassword.TabIndex = 1;
            this.txtNewPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            // btnSave
            this.btnSave.Location = new System.Drawing.Point(25, 210);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(350, 36);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "LƯU MẬT KHẨU VÀ VÀO TRANG CHỦ";
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(33, 150, 243);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.Font = new System.Drawing.Font("Roboto", 10F, System.Drawing.FontStyle.Bold);
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
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

        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.TextBox txtNewPassword;
        private System.Windows.Forms.Button btnSave;
    }
}