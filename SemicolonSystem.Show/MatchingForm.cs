using SemicolonSystem.Business;
using SemicolonSystem.Common;
using SemicolonSystem.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                MessageBox.Show("初始化权重配置异常！请尝试重新导入订单！异常原因：" + dataResult.Message);

                return;
            }

            int top = 60;

            List<KeyValuePair<string,int>> cbxData = new List<KeyValuePair<string,int>>()
            {
                new KeyValuePair<string,int>("无",-1),
                new KeyValuePair<string,int>("1",1),
                new KeyValuePair<string,int>("2",2),
                new KeyValuePair<string,int>("3",3)
            };

            foreach (var item in dataResult.Data)
            {
                Label lbl_Position = new Label();

                lbl_Position.Text = item.Position;

                lbl_Position.Top = top;

                lbl_Position.Left = 39;

                lbl_Position.Width = 60;

                Label lbl_Offset = new Label();

                lbl_Offset.Text = "±";

                lbl_Offset.Top = top + 3;

                lbl_Offset.Left = 127;

                lbl_Offset.Width = 15;

                TextBox tbx_Offset = new TextBox();

                tbx_Offset.Text = "0";

                tbx_Offset.Top = top;

                tbx_Offset.Left = 142;

                tbx_Offset.Width = 40;

                ComboBox cbx_PriorityLevel = new ComboBox();

                cbx_PriorityLevel.DataSource = cbxData;

                cbx_PriorityLevel.DisplayMember = "Key";

                cbx_PriorityLevel.ValueMember = "Value";

                cbx_PriorityLevel.SelectedValue = -1;

                cbx_PriorityLevel.Top = top;

                cbx_PriorityLevel.Left = 224;

                cbx_PriorityLevel.Width = 40;

                cbx_PriorityLevel.Name = "cbx_PriorityLevel";

                this.Controls.Add(lbl_Position);

                this.Controls.Add(lbl_Offset);

                this.Controls.Add(tbx_Offset);

                this.Controls.Add(cbx_PriorityLevel);

                top += 40;
            }

            this.Height = top + 100;

            this.btn_confirm.Top = top + 10;
        }

        private void btn_confirm_Click(object sender, EventArgs e)
        {
            MessageBox.Show("配置成功！");
        }
    }
}
