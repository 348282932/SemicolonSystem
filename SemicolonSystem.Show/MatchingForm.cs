using SemicolonSystem.Business;
using SemicolonSystem.Model;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SemicolonSystem.Show
{
    public partial class MatchingForm : Form
    {
        public MatchingForm()
        {
            InitializeComponent();
        }

        private void MatchingForm_Load(object sender, EventArgs e)
        {
            var dataResult = OrderService.GetWeightConfig();

            if (!dataResult.IsSuccess)
            {
                MessageBox.Show("初始化权重配置异常！请尝试重新导入订单！");

                return;
            }

            int top = 60;

            foreach (var item in dataResult.Data)
            {
                List<KeyValuePair<string, int>> cbxData = new List<KeyValuePair<string, int>>()
                {
                    new KeyValuePair<string,int>("无", 999)
                };

                for (int i = 1; i <= 30; i++)
                {
                    cbxData.Add(new KeyValuePair<string, int>(i.ToString(), i));
                }

                Label lbl_Position = new Label();

                lbl_Position.Text = item.Position;

                lbl_Position.Top = top + 4;

                lbl_Position.Left = 39;

                lbl_Position.Width = 60;

                lbl_Position.Name = "lbl_Position";

                Label lbl_Offset = new Label();

                lbl_Offset.Text = "±";

                lbl_Offset.Top = top + 3;

                lbl_Offset.Left = 127;

                lbl_Offset.Width = 15;

                lbl_Offset.Name = "lbl_Offset";

                TextBox tbx_Offset = new TextBox();

                tbx_Offset.Text = item.Offset.ToString();

                tbx_Offset.Top = top;

                tbx_Offset.Left = 145;

                tbx_Offset.Width = 40;

                tbx_Offset.Name = "tbx_Offset";

                tbx_Offset.LostFocus += tbx_Offset_LostFocus;

                tbx_Offset.KeyPress += tbx_Offset_KeyPress;

                ComboBox cbx_PriorityLevel = new ComboBox();

                cbx_PriorityLevel.DataSource = cbxData;

                cbx_PriorityLevel.DisplayMember = "Key";

                cbx_PriorityLevel.ValueMember = "Value";

                cbx_PriorityLevel.DropDownHeight = 80;

                cbx_PriorityLevel.Top = top;

                cbx_PriorityLevel.Left = 224;

                cbx_PriorityLevel.Width = 40;

                cbx_PriorityLevel.Name = "cbx_PriorityLevel";

                Controls.Add(lbl_Position);

                Controls.Add(lbl_Offset);

                Controls.Add(tbx_Offset);

                Controls.Add(cbx_PriorityLevel);

                cbx_PriorityLevel.SelectedValue = (int)item.PriorityLevel;

                top += 40;
            }

            Height = top + 100;

            btn_confirm.Top = top + 10;
        }

        private void tbx_Offset_LostFocus(object sender, EventArgs e)
        {
            TextBox tbx = (TextBox)sender;

            if (String.IsNullOrWhiteSpace(tbx.Text))
            {
                tbx.Text = "0";
            }
        }

        private void tbx_Offset_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar) && e.KeyChar != 46)
            {
                e.Handled = true;
            }
        }

        private void btn_confirm_Click(object sender, EventArgs e)
        {
            List<WeightModel> list = new List<WeightModel>();

            var positionArr = Controls.Find("lbl_Position", false);

            var offsetArr = Controls.Find("tbx_Offset", false);

            var priorityLevelArr = Controls.Find("cbx_PriorityLevel", false);

            for (int i = 0; i < positionArr.Length; i++)
            {
                var priorityLevel = Convert.ToInt16(((ComboBox)priorityLevelArr[i]).SelectedValue);

                var offset = Convert.ToDecimal(offsetArr[i].Text.Trim());

                list.Add(new WeightModel
                {
                    Position = positionArr[i].Text.ToString(),
                    Offset = offset,
                    PriorityLevel = priorityLevel
                });
            }

            var dataResult = OrderService.ImportWeightCofig(list);

            if (dataResult.IsSuccess)
            {
                MessageBox.Show("配置成功！");

                this.Close();
            }
            else
            {
                MessageBox.Show("配置失败！请尝试重新配置权重！");
            }
        }
    }
}
