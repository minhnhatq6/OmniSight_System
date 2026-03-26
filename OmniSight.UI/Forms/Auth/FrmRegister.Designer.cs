namespace OmniSight.UI.Forms.Auth
{
    partial class FrmRegister
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
            this.txtFullName = new MaterialSkin.Controls.MaterialTextBox();
            this.txtEmail = new MaterialSkin.Controls.MaterialTextBox();
            this.txtPassword = new MaterialSkin.Controls.MaterialTextBox();
            this.txtConfirmPassword = new MaterialSkin.Controls.MaterialTextBox();
            this.btnRegister = new MaterialSkin.Controls.MaterialButton();
            this.lnkLogin = new MaterialSkin.Controls.MaterialLabel();
            this.SuspendLayout();
            // 
            // txtFullName
            // 
            this.txtFullName.AnimateReadOnly = false;
            this.txtFullName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtFullName.Depth = 0;
            this.txtFullName.Font = new System.Drawing.Font("Roboto", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtFullName.Hint = "Họ và tên";
            this.txtFullName.LeadingIcon = null;
            this.txtFullName.Location = new System.Drawing.Point(40, 100);
            this.txtFullName.MaxLength = 50;
            this.txtFullName.MouseState = MaterialSkin.MouseState.OUT;
            this.txtFullName.Multiline = false;
            this.txtFullName.Name = "txtFullName";
            this.txtFullName.Size = new System.Drawing.Size(350, 50);
            this.txtFullName.TabIndex = 0;
            this.txtFullName.Text = "";
            this.txtFullName.TrailingIcon = null;
            // 
            // txtEmail
            // 
            this.txtEmail.AnimateReadOnly = false;
            this.txtEmail.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtEmail.Depth = 0;
            this.txtEmail.Font = new System.Drawing.Font("Roboto", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtEmail.Hint = "Email";
            this.txtEmail.LeadingIcon = null;
            this.txtEmail.Location = new System.Drawing.Point(40, 170);
            this.txtEmail.MaxLength = 50;
            this.txtEmail.MouseState = MaterialSkin.MouseState.OUT;
            this.txtEmail.Multiline = false;
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(350, 50);
            this.txtEmail.TabIndex = 1;
            this.txtEmail.Text = "";
            this.txtEmail.TrailingIcon = null;
            // 
            // txtPassword
            // 
            this.txtPassword.AnimateReadOnly = false;
            this.txtPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtPassword.Depth = 0;
            this.txtPassword.Font = new System.Drawing.Font("Roboto", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtPassword.Hint = "Mật khẩu";
            this.txtPassword.LeadingIcon = null;
            this.txtPassword.Location = new System.Drawing.Point(40, 240);
            this.txtPassword.MaxLength = 50;
            this.txtPassword.MouseState = MaterialSkin.MouseState.OUT;
            this.txtPassword.Multiline = false;
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Password = true;
            this.txtPassword.Size = new System.Drawing.Size(350, 50);
            this.txtPassword.TabIndex = 2;
            this.txtPassword.Text = "";
            this.txtPassword.TrailingIcon = null;
            // 
            // txtConfirmPassword
            // 
            this.txtConfirmPassword.AnimateReadOnly = false;
            this.txtConfirmPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtConfirmPassword.Depth = 0;
            this.txtConfirmPassword.Font = new System.Drawing.Font("Roboto", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtConfirmPassword.Hint = "Xác nhận mật khẩu";
            this.txtConfirmPassword.LeadingIcon = null;
            this.txtConfirmPassword.Location = new System.Drawing.Point(40, 310);
            this.txtConfirmPassword.MaxLength = 50;
            this.txtConfirmPassword.MouseState = MaterialSkin.MouseState.OUT;
            this.txtConfirmPassword.Multiline = false;
            this.txtConfirmPassword.Name = "txtConfirmPassword";
            this.txtConfirmPassword.Password = true;
            this.txtConfirmPassword.Size = new System.Drawing.Size(350, 50);
            this.txtConfirmPassword.TabIndex = 3;
            this.txtConfirmPassword.Text = "";
            this.txtConfirmPassword.TrailingIcon = null;
            // 
            // btnRegister
            // 
            this.btnRegister.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnRegister.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnRegister.Depth = 0;
            this.btnRegister.HighEmphasis = true;
            this.btnRegister.Icon = null;
            this.btnRegister.Location = new System.Drawing.Point(40, 380);
            this.btnRegister.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnRegister.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnRegister.Size = new System.Drawing.Size(78, 36);
            this.btnRegister.TabIndex = 4;
            this.btnRegister.Text = "ĐĂNG KÝ";
            this.btnRegister.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnRegister.UseAccentColor = false;
            this.btnRegister.UseVisualStyleBackColor = true;
            this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click);
            // 
            // lnkLogin
            // 
            this.lnkLogin.AutoSize = true;
            this.lnkLogin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lnkLogin.Depth = 0;
            this.lnkLogin.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lnkLogin.Location = new System.Drawing.Point(40, 440);
            this.lnkLogin.MouseState = MaterialSkin.MouseState.HOVER;
            this.lnkLogin.Name = "lnkLogin";
            this.lnkLogin.Size = new System.Drawing.Size(193, 19);
            this.lnkLogin.TabIndex = 5;
            this.lnkLogin.Text = "Đã có tài khoản? Đăng nhập";
            this.lnkLogin.Click += new System.EventHandler(this.lnkLogin_Click);
            // 
            // FrmRegister
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(430, 500);
            this.Controls.Add(this.lnkLogin);
            this.Controls.Add(this.btnRegister);
            this.Controls.Add(this.txtConfirmPassword);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.txtFullName);
            this.Name = "FrmRegister";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Đăng ký tài khoản";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private MaterialSkin.Controls.MaterialTextBox txtFullName;
        private MaterialSkin.Controls.MaterialTextBox txtEmail;
        private MaterialSkin.Controls.MaterialTextBox txtPassword;
        private MaterialSkin.Controls.MaterialTextBox txtConfirmPassword;
        private MaterialSkin.Controls.MaterialButton btnRegister;
        private MaterialSkin.Controls.MaterialLabel lnkLogin;
    }
}