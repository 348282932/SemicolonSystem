using SemicolonSystem.Business;
using SemicolonSystem.Common;
using SemicolonSystem.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
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
            openFileDialog.FileName = string.Empty;

            DialogResult dialogResult = openFileDialog.ShowDialog();

            if(dialogResult != DialogResult.OK && dialogResult != DialogResult.Yes)
            {
                return;
            }

            string filePath = openFileDialog.FileName;

            DataResult dataResult = RuleService.ImportRuleExcel(filePath);

            if (dataResult.IsSuccess)
            {
                MessageBox.Show("导入成功！");

                btn_ImportRule.BackColor = Color.Green;

                btn_ImportOrder.BackColor = Control.DefaultBackColor;
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
            openFileDialog.FileName = string.Empty;

            DialogResult dialogResult = openFileDialog.ShowDialog();

            if (dialogResult != DialogResult.OK && dialogResult != DialogResult.Yes)
            {
                return;
            }

            string filePath = openFileDialog.FileName;

            DataResult dataResult = OrderService.ImportOrderExcel(filePath);

            if (dataResult.IsSuccess)
            {
                MessageBox.Show("导入成功！");

                btn_ImportOrder.BackColor = Color.Green;
            }
            else
            {
                MessageBox.Show("导入失败！失败原因：" + dataResult.Message);
            }
        }

        /// <summary>
        /// 设置权重
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_SetWeight_Click(object sender, EventArgs e)
        {
            MatchingForm matchingForm = new MatchingForm();

            matchingForm.ShowDialog();
        }

        /// <summary>
        /// 匹配
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Matching_Click(object sender, EventArgs e)
        {
            var dataResult = OrderService.GetMatchingResult();

            if (dataResult.IsSuccess)
            {
                DataTable dt = new DataTable();

                dt.Columns.Add(new DataColumn("名称"));

                dt.Columns.Add(new DataColumn("匹配结果"));

                dt.Columns.Add(new DataColumn(""));

                dt.Columns.Add(new DataColumn("尺寸型号"));

                dt.Columns.Add(new DataColumn("数量"));

                for (int i = 0; i < dataResult.Data.Count; i++)
                {
                    dt.Rows.Add(dt.NewRow());

                    dt.Rows[i][0] = dataResult.Data[i].Name;

                    dt.Rows[i][1] = dataResult.Data[i].Model;

                    dt.Rows[i][2] = dataResult.Data[i].MatchingLevel.ToString();
                }

                saveResultFileDialog.FileName = "匹配结果";

                DialogResult dialogResult = saveResultFileDialog.ShowDialog();

                if (dialogResult != DialogResult.OK && dialogResult != DialogResult.Yes)
                {
                    return;
                }

                string filePath = saveResultFileDialog.FileName;

                var sumResults = dataResult.Data.GroupBy(g => g.Model).Select(s => new KeyValuePair<string, int>(s.Key, s.Count())).ToList();

                var ruleCache = new Cache<List<SizeRuleModel>>();

                var ruleDataResult = ruleCache.GetCache("SizeRule");

                if (!ruleDataResult.IsSuccess)
                {
                    MessageBox.Show("请导入规则信息！");
                }

                var models = ruleDataResult.Data.GroupBy(g => g.Model).Select(s => s.Key).ToList();

                var query = from r in models.AsQueryable()
                            join s in sumResults.AsQueryable() on r equals s.Key into l
                            from a in l.DefaultIfEmpty(new KeyValuePair<string, int>(r, 0))
                            select new KeyValuePair<string, int>(r, a.Value);

                sumResults = query.ToList();

                ExcelHelper.TableToExcelForXLS(dt, filePath, sumResults);

                MessageBox.Show("匹配成功！");
            }
            else
            {
                MessageBox.Show("匹配失败！失败原因：" + dataResult.Message);
            }
        }

        /// <summary>
        /// 下载规则模板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbl_DownRule_Click(object sender, EventArgs e)
        {
            saveTemplateFileDialog.FileName = "规则模版";

            DialogResult dialogResult = saveTemplateFileDialog.ShowDialog();

            if (dialogResult != DialogResult.OK && dialogResult != DialogResult.Yes)
            {
                return;
            }

            string filePath = saveTemplateFileDialog.FileName;

            string path = string.Empty;

#if DEBUG
            path = Application.StartupPath + "\\Template\\规则模版.xlsx";
#else
            path = Global.InstallPath + "\\Template\\规则模版.xlsx";
#endif

            try
            {
                File.Copy(path, filePath);
            }
            catch(Exception ex)
            {
                MessageBox.Show(String.Format("获取规则模板异常！异常原因：{0}，请尝试在该路径下取出模版！{1}{2}", ex.Message, Environment.NewLine, path));

                return;
            }

            MessageBox.Show("导出模版成功！");
        }

        /// <summary>
        /// 下载订单模板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbl_DownOrder_Click(object sender, EventArgs e)
        {
            saveTemplateFileDialog.FileName = "订单模版";

            DialogResult dialogResult = saveTemplateFileDialog.ShowDialog();

            if (dialogResult != DialogResult.OK && dialogResult != DialogResult.Yes)
            {
                return;
            }

            string filePath = saveTemplateFileDialog.FileName;

            string path = string.Empty;

#if DEBUG
            path = Application.StartupPath + "\\Template\\订单模版.xlsx";
#else
            path = Global.InstallPath + "\\Template\\订单模版.xlsx";
#endif

            try
            {
                File.Copy(path, filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("获取订单模板异常！异常原因：{0}，请尝试在该路径下取出模板！{1}{2}", ex.Message, Environment.NewLine, path));

                return;
            }

            MessageBox.Show("导出模板成功！");
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (Global.IsAuthorization)
            {

            }
        }
    }
}
