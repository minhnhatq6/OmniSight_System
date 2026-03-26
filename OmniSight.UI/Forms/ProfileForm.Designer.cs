namespace OmniSight.UI.Forms
{
    partial class ProfileForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtFullName = new MaterialSkin.Controls.MaterialTextBox();
            this.txtPhone = new MaterialSkin.Controls.MaterialTextBox();
            this.switchStudent = new MaterialSkin.Controls.MaterialSwitch();
            this.switchTeacher = new MaterialSkin.Controls.MaterialSwitch();
            this.btnSave = new MaterialSkin.Controls.MaterialButton();
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
            this.txtFullName.MaxLength = 100;
            this.txtFullName.MouseState = MaterialSkin.MouseState.OUT;
            this.txtFullName.Multiline = false;
            this.txtFullName.Name = "txtFullName";
            this.txtFullName.Size = new System.Drawing.Size(350, 50);
            this.txtFullName.TabIndex = 0;
            this.txtFullName.Text = "";
            this.txtFullName.TrailingIcon = null;
            // 
            // txtPhone
            // 
            this.txtPhone.AnimateReadOnly = false;
            this.txtPhone.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtPhone.Depth = 0;
            this.txtPhone.Font = new System.Drawing.Font("Roboto", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtPhone.Hint = "Số điện thoại";
            this.txtPhone.LeadingIcon = null;
            this.txtPhone.Location = new System.Drawing.Point(40, 170);
            this.txtPhone.MaxLength = 15;
            this.txtPhone.MouseState = MaterialSkin.MouseState.OUT;
            this.txtPhone.Multiline = false;
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Size = new System.Drawing.Size(350, 50);
            this.txtPhone.TabIndex = 1;
            this.txtPhone.Text = "";
            this.txtPhone.TrailingIcon = null;
            // 
            // switchStudent
            // 
            this.switchStudent.AutoSize = true;
            this.switchStudent.Depth = 0;
            this.switchStudent.Location = new System.Drawing.Point(40, 240);
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
            this.switchTeacher.Location = new System.Drawing.Point(40, 290);
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
            // btnSave
            // 
            this.btnSave.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSave.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnSave.Depth = 0;
            this.btnSave.HighEmphasis = true;
            this.btnSave.Icon = null;
            this.btnSave.Location = new System.Drawing.Point(40, 350);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnSave.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnSave.Name = "btnSave";
            this.btnSave.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnSave.Size = new System.Drawing.Size(127, 36);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "LƯU THAY ĐỔI";
            this.btnSave.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnSave.UseAccentColor = false;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // ProfileForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(430, 430);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.switchTeacher);
            this.Controls.Add(this.switchStudent);
            this.Controls.Add(this.txtPhone);
            this.Controls.Add(this.txtFullName);
            this.Name = "ProfileForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Chỉnh sửa thông tin";
            this.Load += new System.EventHandler(this.ProfileForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MaterialSkin.Controls.MaterialTextBox txtFullName;
        private MaterialSkin.Controls.MaterialTextBox txtPhone;
        private MaterialSkin.Controls.MaterialSwitch switchStudent;
        private MaterialSkin.Controls.MaterialSwitch switchTeacher;
        private MaterialSkin.Controls.MaterialButton btnSave;
    }
}