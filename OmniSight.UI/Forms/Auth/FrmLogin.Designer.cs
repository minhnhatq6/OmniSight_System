namespace OmniSight.UI.Forms.Auth
{
    partial class FrmLogin
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnLoginGoogle = new MaterialSkin.Controls.MaterialButton();
            this.SuspendLayout();
            // 
            // btnLoginGoogle
            // 
            this.btnLoginGoogle.AutoSize = false;
            this.btnLoginGoogle.Name = "btnLoginGoogle";
            this.btnLoginGoogle.Text = "Đăng nhập với Google";
            this.btnLoginGoogle.Location = new System.Drawing.Point(100, 100);
            this.btnLoginGoogle.Size = new System.Drawing.Size(200, 36);
            this.btnLoginGoogle.Click += new System.EventHandler(this.btnLoginGoogle_Click);
            // 
            // FrmLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 250);
            this.Controls.Add(this.btnLoginGoogle);
            this.Name = "FrmLogin";
            this.Text = "Đăng nhập";
            this.ResumeLayout(false);
        }

        #endregion

        private MaterialSkin.Controls.MaterialButton btnLoginGoogle;
    }
}
