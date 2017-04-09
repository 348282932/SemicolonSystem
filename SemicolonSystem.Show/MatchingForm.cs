using SemicolonSystem.Business;
using SemicolonSystem.Common;
using SemicolonSystem.Model;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SemicolonSystem.Show
{
    public partial class MatchingForm : Form
    {
        private static string _sizeRuleName;

        public MatchingForm(string sizeRuleName)
        {
            _sizeRuleName = sizeRuleName;

            InitializeComponent();
        }

        private void MatchingForm_Load(object sender, EventArgs e)
        {
            var dataResult = new DataResult<List<WeightModel>>();

            try
            {
                dataResult = OrderService.GetWeightConfig(_sizeRuleName);

                if (!dataResult.IsSuccess)
                {
                    MessageBox.Show(dataResult.Message);

                    Close();

                    return;
                }
            }
            catch
            {
                MessageBox.Show("初始化权重配置异常！请尝试重新导入订单！");
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


                Label lbl_Offset_negative = new Label();

                lbl_Offset_negative.Text = "-";

                lbl_Offset_negative.Top = top + 3;

                lbl_Offset_negative.Left = 120;

                lbl_Offset_negative.Width = 15;

                lbl_Offset_negative.Name = "lbl_Offset_negative";


                TextBox tbx_Offset_left = new TextBox();

                tbx_Offset_left.Text = item.OffsetLeft.ToString();

                tbx_Offset_left.Top = top;

                tbx_Offset_left.Left = 137;

                tbx_Offset_left.Width = 40;

                tbx_Offset_left.Name = "tbx_Offset_left";

                tbx_Offset_left.LostFocus += tbx_Offset_left_LostFocus;

                tbx_Offset_left.KeyPress += tbx_Offset_left_KeyPress;


                Label lbl_Offset = new Label();

                lbl_Offset.Text = "~";

                lbl_Offset.Top = top + 3;

                lbl_Offset.Left = 180;

                lbl_Offset.Width = 15;

                lbl_Offset.Name = "lbl_Offset";


                TextBox tbx_Offset_right = new TextBox();

                tbx_Offset_right.Text = item.OffsetRight.ToString();

                tbx_Offset_right.Top = top;

                tbx_Offset_right.Left = 198;

                tbx_Offset_right.Width = 40;

                tbx_Offset_right.Name = "tbx_Offset_right";

                tbx_Offset_right.LostFocus += tbx_Offset_right_LostFocus;

                tbx_Offset_right.KeyPress += tbx_Offset_right_KeyPress;


                ComboBox cbx_PriorityLevel = new ComboBox();

                cbx_PriorityLevel.DataSource = cbxData;

                cbx_PriorityLevel.DisplayMember = "Key";

                cbx_PriorityLevel.ValueMember = "Value";

                cbx_PriorityLevel.DropDownHeight = 80;

                cbx_PriorityLevel.Top = top;

                cbx_PriorityLevel.Left = 290;

                cbx_PriorityLevel.Width = 40;

                cbx_PriorityLevel.Name = "cbx_PriorityLevel";

                Controls.Add(lbl_Position);

                Controls.Add(lbl_Offset_negative);

                Controls.Add(tbx_Offset_left);

                Controls.Add(lbl_Offset);

                Controls.Add(tbx_Offset_right);

                Controls.Add(cbx_PriorityLevel);

                cbx_PriorityLevel.SelectedValue = (int)item.PriorityLevel;

                top += 40;
            }

            Height = top + 100;

            btn_confirm.Top = top + 10;
        }

        private void tbx_Offset_right_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !char.IsDigit(e.KeyChar) && e.KeyChar != 46)
            {
                e.Handled = true;
            }
        }

        private void tbx_Offset_right_LostFocus(object sender, EventArgs e)
        {
            TextBox tbx = (TextBox)sender;

            if (string.IsNullOrWhiteSpace(tbx.Text))
            {
                tbx.Text = "0";
            }
        }

        private void tbx_Offset_left_LostFocus(object sender, EventArgs e)
        {
            TextBox tbx = (TextBox)sender;

            if (string.IsNullOrWhiteSpace(tbx.Text))
            {
                tbx.Text = "0";
            }
        }

        private void tbx_Offset_left_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !char.IsDigit(e.KeyChar) && e.KeyChar != 46)
            {
                e.Handled = true;
            }
        }

        private void btn_confirm_Click(object sender, EventArgs e)
        {
            List<WeightModel> list = new List<WeightModel>();

            var positionArr = Controls.Find("lbl_Position", false);

            var offsetLeftArr = Controls.Find("tbx_Offset_left", false);

            var offsetRightArr = Controls.Find("tbx_Offset_right", false);

            var priorityLevelArr = Controls.Find("cbx_PriorityLevel", false);

            for (int i = 0; i < positionArr.Length; i++)
            {
                var priorityLevel = Convert.ToInt16(((ComboBox)priorityLevelArr[i]).SelectedValue);

                var offsetLeft = Convert.ToDecimal(offsetLeftArr[i].Text.Trim());

                var offsetRight = Convert.ToDecimal(offsetRightArr[i].Text.Trim());

                list.Add(new WeightModel
                {
                    Position = positionArr[i].Text.ToString(),
                    OffsetLeft = offsetLeft,
                    OffsetRight = offsetRight,
                    PriorityLevel = priorityLevel
                });
            }

            var dataResult = OrderService.ImportWeightCofig(list, _sizeRuleName);

            if (dataResult.IsSuccess)
            {
                MessageBox.Show("配置成功！");

                Close();
            }
            else
            {
                MessageBox.Show("配置失败！请尝试重新配置权重！");
            }
        }
    }
}
