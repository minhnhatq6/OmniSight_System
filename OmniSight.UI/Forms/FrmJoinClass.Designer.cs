using System.Drawing;
using System.Windows.Forms;

namespace OmniSight.UI.Forms
{
    partial class FrmJoinClass
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
            txtJoinCode = new MaterialSkin.Controls.MaterialTextBox();
            btnJoin = new MaterialSkin.Controls.MaterialButton();
            SuspendLayout();
            // 
            // txtJoinCode
            // 
            // GIM TOẠ ĐỘ: Tự động kéo dài ra hai bên
            txtJoinCode.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtJoinCode.AnimateReadOnly = false;
            txtJoinCode.BorderStyle = BorderStyle.None;
            txtJoinCode.Depth = 0;
            txtJoinCode.Font = new Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            txtJoinCode.Hint = "Nhập mã lớp (Ví dụ: ABC123)";
            txtJoinCode.LeadingIcon = null;
            txtJoinCode.Location = new Point(20, 90); // Căn lề đồng bộ với form Tạo Lớp
            txtJoinCode.MaxLength = 6; // Đã giới hạn: Chỉ cho phép nhập đúng 6 ký tự
            txtJoinCode.MouseState = MaterialSkin.MouseState.OUT;
            txtJoinCode.Multiline = false;
            txtJoinCode.Name = "txtJoinCode";
            txtJoinCode.Size = new Size(360, 50);
            txtJoinCode.TabIndex = 0;
            txtJoinCode.Text = "";
            txtJoinCode.TrailingIcon = null;
            // 
            // btnJoin
            // 
            // GIM TOẠ ĐỘ: Ép nút bấm luôn nằm ở góc dưới cùng bên phải
            btnJoin.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnJoin.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnJoin.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            btnJoin.Depth = 0;
            btnJoin.HighEmphasis = true;
            btnJoin.Icon = null;
            btnJoin.Location = new Point(280, 180); // Nằm ở góc dưới cùng bên phải
            btnJoin.Margin = new Padding(4, 6, 4, 6);
            btnJoin.MouseState = MaterialSkin.MouseState.HOVER;
            btnJoin.Name = "btnJoin";
            btnJoin.NoAccentTextColor = Color.Empty;
            btnJoin.Size = new Size(100, 36);
            btnJoin.TabIndex = 1;
            btnJoin.Text = "Tham Gia";
            btnJoin.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            btnJoin.UseAccentColor = false;
            btnJoin.UseVisualStyleBackColor = true;
            btnJoin.Click += btnJoin_Click;
            // 
            // FrmJoinClass
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(400, 250); // Thu nhỏ Form lại bằng đúng kích thước form Tạo Lớp
            Controls.Add(btnJoin);
            Controls.Add(txtJoinCode);
            Name = "FrmJoinClass";
            StartPosition = FormStartPosition.CenterParent; // Bật lên ở chính giữa màn hình
            Text = "Tham Gia Lớp Học"; // Đổi tiêu đề rõ ràng
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MaterialSkin.Controls.MaterialTextBox txtJoinCode;
        private MaterialSkin.Controls.MaterialButton btnJoin;
    }
}