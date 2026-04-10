using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin.Controls;
using OmniSight.Services;
using OmniSight.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
namespace OmniSight.UI.Forms
{
    public class FrmExamManager : Form
    {
        private readonly ExamService _examService;
        private readonly ClassService _classService;

        private MaterialListView lvwExams;
        private MaterialComboBox cbClasses;
        private MaterialTextBox txtTitle;
        private NumericUpDown nudDuration;
        private MaterialButton btnCreate;

        public FrmExamManager(ExamService examService, ClassService classService)
        {
            _examService = examService;
            _classService = classService;
            InitializeComponent();
            LoadClassesAsync();
        }

        private async void LoadClassesAsync()
        {
            var classes = await _classService.GetOwnedClassesAsync(Program.ServiceProvider.GetService<AuthService>().CurrentUser.UserId);
            cbClasses.Items.Clear();
            foreach (var c in classes)
            {
                cbClasses.Items.Add(new ListViewItemWithId { Id = c.ClassId, Text = c.ClassName });
            }
            if (cbClasses.Items.Count > 0) cbClasses.SelectedIndex = 0;
        }

        private void InitializeComponent()
        {
            this.Text = "Quản lý đề thi";
            this.Width = 700;
            this.Height = 500;
            this.StartPosition = FormStartPosition.CenterParent;

            cbClasses = new MaterialComboBox { Left = 20, Top = 20, Width = 400 };
            txtTitle = new MaterialTextBox { Left = 20, Top = 70, Width = 400, Hint = "Tiêu đề đề thi" };
            nudDuration = new NumericUpDown { Left = 440, Top = 70, Width = 80, Minimum = 10, Maximum = 300, Value = 60 };
            btnCreate = new MaterialButton { Left = 540, Top = 70, Text = "Tạo đề" };
            btnCreate.Click += BtnCreate_Click;

            lvwExams = new MaterialListView { Left = 20, Top = 120, Width = 640, Height = 300, View = View.Details };
            lvwExams.Columns.Add("Tiêu đề", 400);
            lvwExams.Columns.Add("Thời lượng (phút)", 120);
            lvwExams.Columns.Add("Lớp", 120);

            this.Controls.Add(cbClasses);
            this.Controls.Add(txtTitle);
            this.Controls.Add(nudDuration);
            this.Controls.Add(btnCreate);
            this.Controls.Add(lvwExams);
        }

        private async void BtnCreate_Click(object sender, EventArgs e)
        {
            if (cbClasses.SelectedItem == null) return;
            var selected = cbClasses.SelectedItem as ListViewItemWithId;
            if (selected == null) return;

            string title = txtTitle.Text.Trim();
            int duration = (int)nudDuration.Value;
            if (string.IsNullOrEmpty(title)) { MessageBox.Show("Vui lòng nhập tiêu đề đề thi."); return; }

            try
            {
                var exam = await _examService.CreateExamAsync(selected.Id, title, duration);
                MessageBox.Show("Tạo đề thi thành công.");
                await RefreshExamsAsync(selected.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tạo đề thi: " + ex.Message);
            }
        }

        private async Task RefreshExamsAsync(int classId)
        {
            lvwExams.Items.Clear();
            var exams = await _examService.GetExamsByClassIdAsync(classId);
            foreach (var ex in exams)
            {
                var item = new ListViewItem(new[] { ex.Title, ex.DurationMinutes.ToString(), ex.Class?.ClassName ?? "" });
                item.Tag = ex.ExamId;
                lvwExams.Items.Add(item);
            }
        }

        private class ListViewItemWithId : ListViewItem
        {
            public int Id { get; set; }
            public ListViewItemWithId() : base() { }
            public override string ToString() => Text;
        }
    }
}
