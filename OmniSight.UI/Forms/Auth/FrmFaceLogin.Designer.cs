namespace OmniSight.UI.Forms.Auth
{
    partial class FrmFaceLogin
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.PictureBox picFace;
        private MaterialSkin.Controls.MaterialLabel lblStatus;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.picFace = new System.Windows.Forms.PictureBox();
            this.lblStatus = new MaterialSkin.Controls.MaterialLabel();
            ((System.ComponentModel.ISupportInitialize)(this.picFace)).BeginInit();
            this.SuspendLayout();
            // picFace
            this.picFace.BackColor = System.Drawing.Color.Black;
            this.picFace.Location = new System.Drawing.Point(20, 80);
            this.picFace.Name = "picFace";
            this.picFace.Size = new System.Drawing.Size(460, 300);
            this.picFace.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picFace.TabIndex = 0;
            // lblStatus
            this.lblStatus.AutoSize = true;
            this.lblStatus.Depth = 0;
            this.lblStatus.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblStatus.Location = new System.Drawing.Point(20, 390);
            this.lblStatus.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblStatus.Text = "Đang quét khuôn mặt...";
            // FrmFaceLogin
            this.ClientSize = new System.Drawing.Size(500, 430);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.picFace);
            this.Name = "FrmFaceLogin";
            this.Text = "Xác thực khuôn mặt";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Load += new System.EventHandler(this.FrmFaceLogin_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picFace)).EndInit();
            this.ResumeLayout(false);
        }
    }
}