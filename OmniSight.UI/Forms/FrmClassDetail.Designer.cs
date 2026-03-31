using System.Drawing;
using System.Windows.Forms;

namespace OmniSight.UI.Forms
{
    partial class FrmClassDetail
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
            txtPostContent = new MaterialSkin.Controls.MaterialMultiLineTextBox2();
            btnPost = new MaterialSkin.Controls.MaterialButton();
            flpStream = new FlowLayoutPanel();
            materialTabControl1 = new MaterialSkin.Controls.MaterialTabControl();
            tabPage1 = new TabPage();
            tabPage2 = new TabPage();
            lvwMembers = new MaterialSkin.Controls.MaterialListView();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            columnHeader3 = new ColumnHeader();
            materialTabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            SuspendLayout();
            // 
            // txtPostContent
            // 
            txtPostContent.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtPostContent.AnimateReadOnly = false;
            txtPostContent.BackgroundImageLayout = ImageLayout.None;
            txtPostContent.CharacterCasing = CharacterCasing.Normal;
            txtPostContent.Depth = 0;
            txtPostContent.HideSelection = true;
            txtPostContent.Hint = "Thông báo điều gì đó cho lớp học của bạn...";
            txtPostContent.Location = new Point(16, 76);
            txtPostContent.MaxLength = 32767;
            txtPostContent.MouseState = MaterialSkin.MouseState.OUT;
            txtPostContent.Name = "txtPostContent";
            txtPostContent.PasswordChar = '\0';
            txtPostContent.ReadOnly = false;
            txtPostContent.ScrollBars = ScrollBars.None;
            txtPostContent.SelectedText = "";
            txtPostContent.SelectionLength = 0;
            txtPostContent.SelectionStart = 0;
            txtPostContent.ShortcutsEnabled = true;
            txtPostContent.Size = new Size(653, 60);
            txtPostContent.TabIndex = 0;
            txtPostContent.TabStop = false;
            txtPostContent.TextAlign = HorizontalAlignment.Left;
            txtPostContent.UseSystemPasswordChar = false;
            // 
            // btnPost
            // 
            btnPost.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnPost.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnPost.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            btnPost.Depth = 0;
            btnPost.HighEmphasis = true;
            btnPost.Icon = null;
            btnPost.Location = new Point(679, 88);
            btnPost.Margin = new Padding(4, 6, 4, 6);
            btnPost.MouseState = MaterialSkin.MouseState.HOVER;
            btnPost.Name = "btnPost";
            btnPost.NoAccentTextColor = Color.Empty;
            btnPost.Size = new Size(88, 36);
            btnPost.TabIndex = 1;
            btnPost.Text = "Đăng bài";
            btnPost.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            btnPost.UseAccentColor = false;
            btnPost.UseVisualStyleBackColor = true;
            btnPost.Click += btnPost_Click;
            // 
            // flpStream
            // 
            flpStream.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            flpStream.AutoScroll = true;
            flpStream.Location = new Point(3, 6);
            flpStream.Name = "flpStream";
            flpStream.Size = new Size(738, 262);
            flpStream.TabIndex = 2;
            // 
            // materialTabControl1
            // 
            materialTabControl1.Controls.Add(tabPage1);
            materialTabControl1.Controls.Add(tabPage2);
            materialTabControl1.Depth = 0;
            materialTabControl1.Location = new Point(16, 142);
            materialTabControl1.MouseState = MaterialSkin.MouseState.HOVER;
            materialTabControl1.Multiline = true;
            materialTabControl1.Name = "materialTabControl1";
            materialTabControl1.SelectedIndex = 0;
            materialTabControl1.Size = new Size(758, 302);
            materialTabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(flpStream);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(750, 274);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Bảng Tin";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(lvwMembers);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(750, 274);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Mọi Người";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // lvwMembers
            // 
            lvwMembers.AutoSizeTable = false;
            lvwMembers.BackColor = Color.FromArgb(255, 255, 255);
            lvwMembers.BorderStyle = BorderStyle.None;
            lvwMembers.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3 });
            lvwMembers.Depth = 0;
            lvwMembers.FullRowSelect = true;
            lvwMembers.Location = new Point(6, 6);
            lvwMembers.MinimumSize = new Size(200, 100);
            lvwMembers.MouseLocation = new Point(-1, -1);
            lvwMembers.MouseState = MaterialSkin.MouseState.OUT;
            lvwMembers.Name = "lvwMembers";
            lvwMembers.OwnerDraw = true;
            lvwMembers.Size = new Size(738, 254);
            lvwMembers.TabIndex = 0;
            lvwMembers.UseCompatibleStateImageBehavior = false;
            lvwMembers.View = View.Details;
            lvwMembers.SelectedIndexChanged += lvwMembers_SelectedIndexChanged;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Họ Tên";
            columnHeader1.Width = 200;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Email";
            columnHeader2.Width = 200;
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "Vai Trò";
            columnHeader3.Width = 100;
            // 
            // FrmClassDetail
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(793, 450);
            Controls.Add(materialTabControl1);
            Controls.Add(btnPost);
            Controls.Add(txtPostContent);
            DrawerTabControl = materialTabControl1;
            Name = "FrmClassDetail";
            Text = "Bảng tin lớp học";
            Load += FrmClassDetail_Load;
            materialTabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MaterialSkin.Controls.MaterialMultiLineTextBox2 txtPostContent;
        private MaterialSkin.Controls.MaterialButton btnPost;
        private FlowLayoutPanel flpStream;
        private MaterialSkin.Controls.MaterialTabControl materialTabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private MaterialSkin.Controls.MaterialListView lvwMembers;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
    }
}