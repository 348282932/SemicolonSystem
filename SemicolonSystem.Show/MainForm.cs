using SemicolonSystem.Business;
using SemicolonSystem.Common;
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
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 导入规则
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ImportRule_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = openRuleFileDialog.ShowDialog();

            if(dialogResult != DialogResult.OK && dialogResult != DialogResult.Yes)
            {
                return;
            }

            string filePath = openRuleFileDialog.FileName;

            DataResult dataResult = RuleService.ImportRuleExcel(filePath);

            if (dataResult.IsSuccess)
            {
                MessageBox.Show("导入成功！");
            }
            else
            {
                MessageBox.Show("导入失败！失败原因：" + dataResult.Message);
            }
        }

        /// <summary>
        /// 导入订单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ImportOrder_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = openRuleFileDialog.ShowDialog();

            if (dialogResult != DialogResult.OK && dialogResult != DialogResult.Yes)
            {
                return;
            }

            string filePath = openRuleFileDialog.FileName;

            DataResult dataResult = OrderService.ImportOrderExcel(filePath);

            if (dataResult.IsSuccess)
            {
                MessageBox.Show("导入成功！");
            }
            else
            {
                MessageBox.Show("导入失败！失败原因：" + dataResult.Message);
            }
        }

        private void btn_SetWeight_Click(object sender, EventArgs e)
        {
            MatchingForm matchingForm = new MatchingForm();

            matchingForm.ShowDialog();
        }
    }
}
