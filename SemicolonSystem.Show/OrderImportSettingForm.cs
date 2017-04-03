using SemicolonSystem.Business;
using SemicolonSystem.Common;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SemicolonSystem.Show
{
    public partial class OrderImportSettingForm : Form
    {
        public OrderImportSettingForm()
        {
            InitializeComponent();
        }

        private static MainForm _mainForm;

        public OrderImportSettingForm(MainForm mianForm)
        {
            InitializeComponent();

            _mainForm = mianForm;
        }

        private void OrderImportSettingForm_Load(object sender, EventArgs e)
        {
            tbx_marginBottom.LostFocus += tbx_Offset_LostFocus;

            tbx_marginBottom.KeyPress += tbx_Offset_KeyPress;

            tbx_marginHeard.LostFocus += tbx_Offset_LostFocus;

            tbx_marginHeard.KeyPress += tbx_Offset_KeyPress;
        }

        private void tbx_Offset_LostFocus(object sender, EventArgs e)
        {
            TextBox tbx = (TextBox)sender;

            if (string.IsNullOrWhiteSpace(tbx.Text))
            {
                tbx.Text = "0";
            }
        }

        private void tbx_Offset_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !char.IsDigit(e.KeyChar) && e.KeyChar != 46)
            {
                e.Handled = true;
            }
        }



        private void btn_cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btn_confirm_Click(object sender, EventArgs e)
        {
            openFileDialog.FileName = string.Empty;

            DialogResult dialogResult = openFileDialog.ShowDialog();

            if (dialogResult != DialogResult.OK && dialogResult != DialogResult.Yes)
            {
                return;
            }

            int marginHeard = Convert.ToInt32(tbx_marginHeard.Text.Trim());

            int marginBottom = Convert.ToInt32(tbx_marginBottom.Text.Trim());

            string filePath = openFileDialog.FileName;

            DataResult dataResult = OrderService.ImportOrderExcel(filePath, marginHeard, marginBottom);

            if (dataResult.IsSuccess)
            {
                Close();

                MessageBox.Show("导入成功！");

                _mainForm.btn_ImportOrder.BackColor = Color.Green;
            }
            else
            {
                MessageBox.Show("导入失败！失败原因：" + dataResult.Message);
            }
        }
    }
}
