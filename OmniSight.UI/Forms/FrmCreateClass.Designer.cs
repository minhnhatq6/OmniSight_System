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
            txtClassName = new MaterialSkin.Controls.MaterialTextBox();
            btnCreate = new MaterialSkin.Controls.MaterialButton();
            cmbSubjects = new MaterialSkin.Controls.MaterialComboBox();
            SuspendLayout();
            // 
            // txtClassName
            // 
            txtClassName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtClassName.AnimateReadOnly = false;
            txtClassName.BorderStyle = BorderStyle.None;
            txtClassName.Depth = 0;
            txtClassName.Font = new Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            txtClassName.Hint = "Nhập tên lớp học...";
            txtClassName.LeadingIcon = null;
            txtClassName.Location = new Point(20, 90);
            txtClassName.MaxLength = 50;
            txtClassName.MouseState = MaterialSkin.MouseState.OUT;
            txtClassName.Multiline = false;
            txtClassName.Name = "txtClassName";
            txtClassName.Size = new Size(360, 50);
            txtClassName.TabIndex = 0;
            txtClassName.Text = "";
            txtClassName.TrailingIcon = null;
            // 
            // btnCreate
            // 
            btnCreate.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCreate.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnCreate.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            btnCreate.Depth = 0;
            btnCreate.HighEmphasis = true;
            btnCreate.Icon = null;
            btnCreate.Location = new Point(299, 213);
            btnCreate.Margin = new Padding(4, 6, 4, 6);
            btnCreate.MouseState = MaterialSkin.MouseState.HOVER;
            btnCreate.Name = "btnCreate";
            btnCreate.NoAccentTextColor = Color.Empty;
            btnCreate.Size = new Size(81, 36);
            btnCreate.TabIndex = 1;
            btnCreate.Text = "Tạo Lớp";
            btnCreate.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            btnCreate.UseAccentColor = false;
            btnCreate.UseVisualStyleBackColor = true;
            btnCreate.Click += btnCreate_Click;
            // 
            // cmbSubjects
            // 
            cmbSubjects.AutoResize = false;
            cmbSubjects.BackColor = Color.FromArgb(255, 255, 255);
            cmbSubjects.Depth = 0;
            cmbSubjects.DrawMode = DrawMode.OwnerDrawVariable;
            cmbSubjects.DropDownHeight = 174;
            cmbSubjects.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSubjects.DropDownWidth = 121;
            cmbSubjects.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            cmbSubjects.ForeColor = Color.FromArgb(222, 0, 0, 0);
            cmbSubjects.FormattingEnabled = true;
            cmbSubjects.IntegralHeight = false;
            cmbSubjects.ItemHeight = 43;
            cmbSubjects.Location = new Point(20, 146);
            cmbSubjects.MaxDropDownItems = 4;
            cmbSubjects.MouseState = MaterialSkin.MouseState.OUT;
            cmbSubjects.Name = "cmbSubjects";
            cmbSubjects.Size = new Size(360, 49);
            cmbSubjects.StartIndex = 0;
            cmbSubjects.TabIndex = 2;
            // 
            // FrmCreateClass
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(400, 283);
            Controls.Add(cmbSubjects);
            Controls.Add(btnCreate);
            Controls.Add(txtClassName);
            Name = "FrmCreateClass";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Tạo Lớp Học Mới";
            Load += FrmCreateClass_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MaterialSkin.Controls.MaterialTextBox txtClassName;
        private MaterialSkin.Controls.MaterialButton btnCreate;
        private MaterialSkin.Controls.MaterialComboBox cmbSubjects;
    }
}