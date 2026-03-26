namespace OmniSight.UI.Forms.Auth
{
    partial class FrmLogin
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
            this.txtEmail = new MaterialSkin.Controls.MaterialTextBox();
            this.txtPassword = new MaterialSkin.Controls.MaterialTextBox();
            this.btnLoginEmail = new MaterialSkin.Controls.MaterialButton();
            this.btnLoginGoogle = new MaterialSkin.Controls.MaterialButton();
            this.lnkRegister = new MaterialSkin.Controls.MaterialLabel();
            this.SuspendLayout();
            // 
            // txtEmail
            // 
            this.txtEmail.AnimateReadOnly = false;
            this.txtEmail.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtEmail.Depth = 0;
            this.txtEmail.Font = new System.Drawing.Font("Roboto", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtEmail.Hint = "Email";
            this.txtEmail.LeadingIcon = null;
            this.txtEmail.Location = new System.Drawing.Point(40, 100);
            this.txtEmail.MaxLength = 50;
            this.txtEmail.MouseState = MaterialSkin.MouseState.OUT;
            this.txtEmail.Multiline = false;
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(350, 50);
            this.txtEmail.TabIndex = 0;
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
            this.txtPassword.Location = new System.Drawing.Point(40, 170);
            this.txtPassword.MaxLength = 50;
            this.txtPassword.MouseState = MaterialSkin.MouseState.OUT;
            this.txtPassword.Multiline = false;
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Password = true;
            this.txtPassword.Size = new System.Drawing.Size(350, 50);
            this.txtPassword.TabIndex = 1;
            this.txtPassword.Text = "";
            this.txtPassword.TrailingIcon = null;
            // 
            // btnLoginEmail
            // 
            this.btnLoginEmail.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnLoginEmail.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnLoginEmail.Depth = 0;
            this.btnLoginEmail.HighEmphasis = true;
            this.btnLoginEmail.Icon = null;
            this.btnLoginEmail.Location = new System.Drawing.Point(40, 240);
            this.btnLoginEmail.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnLoginEmail.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnLoginEmail.Name = "btnLoginEmail";
            this.btnLoginEmail.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnLoginEmail.Size = new System.Drawing.Size(350, 36);
            this.btnLoginEmail.TabIndex = 2;
            this.btnLoginEmail.Text = "ĐĂNG NHẬP";
            this.btnLoginEmail.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnLoginEmail.UseAccentColor = false;
            this.btnLoginEmail.UseVisualStyleBackColor = true;
            this.btnLoginEmail.Click += new System.EventHandler(this.btnLoginEmail_Click);
            // 
            // btnLoginGoogle
            // 
            this.btnLoginGoogle.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnLoginGoogle.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnLoginGoogle.Depth = 0;
            this.btnLoginGoogle.HighEmphasis = true;
            this.btnLoginGoogle.Icon = null;
            this.btnLoginGoogle.Location = new System.Drawing.Point(40, 290);
            this.btnLoginGoogle.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnLoginGoogle.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnLoginGoogle.Name = "btnLoginGoogle";
            this.btnLoginGoogle.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnLoginGoogle.Size = new System.Drawing.Size(350, 36);
            this.btnLoginGoogle.TabIndex = 3;
            this.btnLoginGoogle.Text = "ĐĂNG NHẬP VỚI GOOGLE";
            this.btnLoginGoogle.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined;
            this.btnLoginGoogle.UseAccentColor = false;
            this.btnLoginGoogle.UseVisualStyleBackColor = true;
            this.btnLoginGoogle.Click += new System.EventHandler(this.btnLoginGoogle_Click);
            // 
            // lnkRegister
            // 
            this.lnkRegister.AutoSize = true;
            this.lnkRegister.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lnkRegister.Depth = 0;
            this.lnkRegister.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lnkRegister.Location = new System.Drawing.Point(40, 350);
            this.lnkRegister.MouseState = MaterialSkin.MouseState.HOVER;
            this.lnkRegister.Name = "lnkRegister";
            this.lnkRegister.Size = new System.Drawing.Size(176, 19);
            this.lnkRegister.TabIndex = 4;
            this.lnkRegister.Text = "Chưa có tài khoản? Đăng ký";
            this.lnkRegister.Click += new System.EventHandler(this.lnkRegister_Click);
            // 
            // FrmLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(430, 420);
            this.Controls.Add(this.lnkRegister);
            this.Controls.Add(this.btnLoginGoogle);
            this.Controls.Add(this.btnLoginEmail);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtEmail);
            this.Name = "FrmLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Đăng nhập";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private MaterialSkin.Controls.MaterialTextBox txtEmail;
        private MaterialSkin.Controls.MaterialTextBox txtPassword;
        private MaterialSkin.Controls.MaterialButton btnLoginEmail;
        private MaterialSkin.Controls.MaterialButton btnLoginGoogle;
        private MaterialSkin.Controls.MaterialLabel lnkRegister;
    }
}