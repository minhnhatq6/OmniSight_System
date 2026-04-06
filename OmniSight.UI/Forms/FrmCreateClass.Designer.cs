using System.Drawing;
using System.Windows.Forms;

namespace OmniSight.UI.Forms
{
    partial class FrmCreateClass
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
            this.txtClassName = new MaterialSkin.Controls.MaterialTextBox();
            this.cmbSubjects = new MaterialSkin.Controls.MaterialComboBox();
            this.switchNewSubject = new MaterialSkin.Controls.MaterialSwitch();
            this.txtNewSubject = new MaterialSkin.Controls.MaterialTextBox();
            this.btnCreate = new MaterialSkin.Controls.MaterialButton();
            this.SuspendLayout();
            // 
            // txtClassName
            // 
            this.txtClassName.AnimateReadOnly = false;
            this.txtClassName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtClassName.Depth = 0;
            this.txtClassName.Font = new System.Drawing.Font("Roboto", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtClassName.Hint = "Nhập tên lớp học...";
            this.txtClassName.LeadingIcon = null;
            this.txtClassName.Location = new System.Drawing.Point(20, 80);
            this.txtClassName.MaxLength = 50;
            this.txtClassName.MouseState = MaterialSkin.MouseState.OUT;
            this.txtClassName.Multiline = false;
            this.txtClassName.Name = "txtClassName";
            this.txtClassName.Size = new System.Drawing.Size(360, 50);
            this.txtClassName.TabIndex = 0;
            this.txtClassName.Text = "";
            this.txtClassName.TrailingIcon = null;
            // 
            // cmbSubjects
            // 
            this.cmbSubjects.AutoResize = false;
            this.cmbSubjects.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmbSubjects.Depth = 0;
            this.cmbSubjects.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cmbSubjects.DropDownHeight = 174;
            this.cmbSubjects.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSubjects.DropDownWidth = 121;
            this.cmbSubjects.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.cmbSubjects.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.cmbSubjects.FormattingEnabled = true;
            this.cmbSubjects.Hint = "Chọn môn học";
            this.cmbSubjects.IntegralHeight = false;
            this.cmbSubjects.ItemHeight = 43;
            this.cmbSubjects.Location = new System.Drawing.Point(20, 140);
            this.cmbSubjects.MaxDropDownItems = 4;
            this.cmbSubjects.MouseState = MaterialSkin.MouseState.OUT;
            this.cmbSubjects.Name = "cmbSubjects";
            this.cmbSubjects.Size = new System.Drawing.Size(360, 49);
            this.cmbSubjects.StartIndex = 0;
            this.cmbSubjects.TabIndex = 1;
            // 
            // switchNewSubject
            // 
            this.switchNewSubject.AutoSize = true;
            this.switchNewSubject.Depth = 0;
            this.switchNewSubject.Location = new System.Drawing.Point(20, 200);
            this.switchNewSubject.Margin = new System.Windows.Forms.Padding(0);
            this.switchNewSubject.MouseLocation = new System.Drawing.Point(-1, -1);
            this.switchNewSubject.MouseState = MaterialSkin.MouseState.HOVER;
            this.switchNewSubject.Name = "switchNewSubject";
            this.switchNewSubject.Ripple = true;
            this.switchNewSubject.Size = new System.Drawing.Size(193, 37);
            this.switchNewSubject.TabIndex = 2;
            this.switchNewSubject.Text = "Tạo môn học mới";
            this.switchNewSubject.UseVisualStyleBackColor = true;
            this.switchNewSubject.CheckedChanged += new System.EventHandler(this.switchNewSubject_CheckedChanged);
            // 
            // txtNewSubject
            // 
            this.txtNewSubject.AnimateReadOnly = false;
            this.txtNewSubject.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtNewSubject.Depth = 0;
            this.txtNewSubject.Enabled = false;
            this.txtNewSubject.Font = new System.Drawing.Font("Roboto", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtNewSubject.Hint = "Nhập tên môn học mới...";
            this.txtNewSubject.LeadingIcon = null;
            this.txtNewSubject.Location = new System.Drawing.Point(20, 250);
            this.txtNewSubject.MaxLength = 50;
            this.txtNewSubject.MouseState = MaterialSkin.MouseState.OUT;
            this.txtNewSubject.Multiline = false;
            this.txtNewSubject.Name = "txtNewSubject";
            this.txtNewSubject.Size = new System.Drawing.Size(360, 50);
            this.txtNewSubject.TabIndex = 3;
            this.txtNewSubject.Text = "";
            this.txtNewSubject.TrailingIcon = null;
            // 
            // btnCreate
            // 
            this.btnCreate.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnCreate.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnCreate.Depth = 0;
            this.btnCreate.HighEmphasis = true;
            this.btnCreate.Icon = null;
            this.btnCreate.Location = new System.Drawing.Point(299, 320);
            this.btnCreate.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnCreate.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnCreate.Size = new System.Drawing.Size(81, 36);
            this.btnCreate.TabIndex = 4;
            this.btnCreate.Text = "TẠO LỚP";
            this.btnCreate.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnCreate.UseAccentColor = false;
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // FrmCreateClass
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 380);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.txtNewSubject);
            this.Controls.Add(this.switchNewSubject);
            this.Controls.Add(this.cmbSubjects);
            this.Controls.Add(this.txtClassName);
            this.Name = "FrmCreateClass";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tạo Lớp Học Mới";
            this.Load += new System.EventHandler(this.FrmCreateClass_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private MaterialSkin.Controls.MaterialTextBox txtClassName;
        private MaterialSkin.Controls.MaterialComboBox cmbSubjects;
        private MaterialSkin.Controls.MaterialSwitch switchNewSubject;
        private MaterialSkin.Controls.MaterialTextBox txtNewSubject;
        private MaterialSkin.Controls.MaterialButton btnCreate;
    }
}