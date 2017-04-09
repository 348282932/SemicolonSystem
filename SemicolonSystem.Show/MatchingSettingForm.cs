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
using System.Windows.Forms;

namespace SemicolonSystem.Show
{
    public partial class MatchingSettingForm : Form
    {
        public MatchingSettingForm()
        {
            InitializeComponent();
        }

        private void MatchingSettingForm_Load(object sender, EventArgs e)
        {
            var dataResult = new DataResult<List<SizeRuleModel>>();

            var ruleCache = new Cache<List<SizeRuleModel>>();

            var ruletDataResult = ruleCache.GetCache("SizeRule");

            if (!ruletDataResult.IsSuccess)
            {
                MessageBox.Show("请导入规则信息！");

                Close();

                return;
            }

            int top = 20;

            foreach (var item in ruletDataResult.Data)
            {
                var cache = new Cache<List<WeightModel>>();

                var cofigDataResult = cache.GetCache("WeightCofig_" + item.Name);

                Button btn_SizeRule = new Button();

                btn_SizeRule.Text = item.Name;

                btn_SizeRule.Top = top + 10;

                btn_SizeRule.Left = 72;

                btn_SizeRule.Width = 80;

                btn_SizeRule.Height = 40;

                btn_SizeRule.Name = "btn_SizeRule";

                if (cofigDataResult.IsSuccess)
                {
                    btn_SizeRule.BackColor = Color.Green;
                }

                btn_SizeRule.Click += btn_SizeRule_Click;

                top += 60;

                Controls.Add(btn_SizeRule);
            }

            Height = top + 60;
        }

        private void btn_SizeRule_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            MatchingForm matchingForm = new MatchingForm(btn.Text);

            matchingForm.ShowDialog();
        }
    }
}
