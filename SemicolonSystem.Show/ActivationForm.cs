using SemicolonSystem.Common;
using System;
using System.Windows.Forms;

namespace SemicolonSystem.Show
{
    public partial class ActivationForm : Form
    {
        public ActivationForm()
        {
            InitializeComponent();
        }

        private void btn_Activation_Click(object sender, EventArgs e)
        {
            if (tbx_ActivationCode.Text.Trim() == Encryption.MD5(lbl_key.Text, Global.Key))
            {
                Global.IsAuthorization = true;

                DataResult dataResult = new Cache<string>().SetCache("SecretKey", tbx_ActivationCode.Text.Trim());

                if (!dataResult.IsSuccess)
                {
                    MessageBox.Show(dataResult.Message);
                }
                else
                {
                    MessageBox.Show("激活成功！");

                    Hide();

                    MainForm mainForm = new MainForm();

                    mainForm.Show();
                }
            }
            else
            {
                MessageBox.Show("激活码错误！");
            }
        }

        private void ActivationForm_Load(object sender, EventArgs e)
        {
            lbl_key.Text = Global.FingerPrint;
        }

        private void btn_Copy_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(lbl_key.Text);

            MessageBox.Show("复制成功！");
        }
    }
}
