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
            var orderImportSettingForm = new OrderImportSettingForm(this);

            orderImportSettingForm.ShowDialog();
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

            if (!dataResult.IsSuccess)
            {
                MessageBox.Show("匹配失败！失败原因：" + dataResult.Message);
            }

            var tabs = new List<DataTable>();

            var isHasSex = false;

            foreach (var item in dataResult.Data)
            {
                isHasSex = item.MatchingRows.Any(a => !string.IsNullOrWhiteSpace(a.Sex));

                DataTable dt = new DataTable();

                dt.TableName = item.SheetName;

                var sumResults = Sort(item.MatchingRows.GroupBy(g => g.Model).Select(s => new KeyValuePair<string, int>(s.Key, s.Count())).ToList());

                if (isHasSex)
                {
                    dt.Columns.Add(new DataColumn("姓名(男)"));

                    dt.Columns.Add(new DataColumn("结果(男)"));

                    dt.Columns.Add(new DataColumn("匹配程度(男)"));

                    dt.Columns.Add(new DataColumn("号型(男)"));

                    dt.Columns.Add(new DataColumn("数量(男)"));

                    dt.Columns.Add(new DataColumn());

                    dt.Columns.Add(new DataColumn("姓名(女)"));

                    dt.Columns.Add(new DataColumn("结果(女)"));

                    dt.Columns.Add(new DataColumn("匹配程度(女)"));

                    dt.Columns.Add(new DataColumn("号型(女)"));

                    dt.Columns.Add(new DataColumn("数量(女)"));

                    dt.Columns.Add(new DataColumn());

                    dt.Columns.Add(new DataColumn("汇总"));

                    dt.Columns.Add(new DataColumn("号型"));

                    dt.Columns.Add(new DataColumn("数量"));

                    var boyList = item.MatchingRows.Where(w => w.Sex.Contains("男")).ToList();

                    var girlList = item.MatchingRows.Where(w => w.Sex.Contains("女")).ToList();

                    var sumBoyResults = Sort(boyList.GroupBy(g => g.Model).Select(s => new KeyValuePair<string, int>(s.Key, s.Count())).ToList());

                    var sumGirlResults = Sort(girlList.GroupBy(g => g.Model).Select(s => new KeyValuePair<string, int>(s.Key, s.Count())).ToList());

                    var count = Math.Max(boyList.Count, girlList.Count);

                    for (int i = 0; i < count; i++)
                    {
                        dt.Rows.Add(dt.NewRow());
                    }

                    for (int i = 0; i < boyList.Count; i++)
                    {
                        dt.Rows[i][0] = boyList[i].Name;

                        dt.Rows[i][1] = boyList[i].Model;

                        dt.Rows[i][2] = boyList[i].MatchingLevel.ToString();
                    }

                    for (int i = 0; i < sumBoyResults.Count; i++)
                    {
                        dt.Rows[i][3] = sumBoyResults[i].Key;

                        dt.Rows[i][4] = sumBoyResults[i].Value;
                    }

                    for (int i = 0; i < girlList.Count; i++)
                    {
                        dt.Rows[i][6] = girlList[i].Name;

                        dt.Rows[i][7] = girlList[i].Model;

                        dt.Rows[i][8] = girlList[i].MatchingLevel.ToString();
                    }

                    for (int i = 0; i < sumGirlResults.Count; i++)
                    {
                        dt.Rows[i][9] = sumGirlResults[i].Key;

                        dt.Rows[i][10] = sumGirlResults[i].Value;
                    }

                    for (int i = 0; i < sumResults.Count; i++)
                    {

                        dt.Rows[i][13] = sumResults[i].Key;

                        dt.Rows[i][14] = sumResults[i].Value;
                    }
                }
                else
                {
                    dt.Columns.Add(new DataColumn("姓名"));

                    dt.Columns.Add(new DataColumn("结果"));

                    dt.Columns.Add(new DataColumn("匹配程度"));

                    dt.Columns.Add(new DataColumn("号型"));

                    dt.Columns.Add(new DataColumn("数量"));

                    for (int i = 0; i < item.MatchingRows.Count; i++)
                    {
                        dt.Rows.Add(dt.NewRow());

                        dt.Rows[i][0] = item.MatchingRows[i].Name;

                        dt.Rows[i][1] = item.MatchingRows[i].Model;

                        dt.Rows[i][2] = item.MatchingRows[i].MatchingLevel.ToString();
                    }

                    for (int i = 0; i < sumResults.Count; i++)
                    {

                        dt.Rows[i][3] = sumResults[i].Key;

                        dt.Rows[i][4] = sumResults[i].Value;
                    }
                }

                tabs.Add(dt);
            }

            DataTable sumTab = new DataTable();

            sumTab.TableName = "汇总";

            sumTab.Columns.Add(new DataColumn("汇总(男)"));

            sumTab.Columns.Add(new DataColumn("号型(男)"));

            sumTab.Columns.Add(new DataColumn("数量(男)"));

            sumTab.Columns.Add(new DataColumn());

            sumTab.Columns.Add(new DataColumn("汇总(女)"));

            sumTab.Columns.Add(new DataColumn("号型(女)"));

            sumTab.Columns.Add(new DataColumn("数量(女)"));

            sumTab.Columns.Add(new DataColumn());

            sumTab.Columns.Add(new DataColumn("总汇总"));

            sumTab.Columns.Add(new DataColumn("号型"));

            sumTab.Columns.Add(new DataColumn("数量"));

            List<MatchingRowModel> sumData = new List<MatchingRowModel>();

            foreach (var item in dataResult.Data)
            {
                sumData.AddRange(item.MatchingRows);
            }

            var boyDataList = sumData.Where(w => w.Sex.Contains("男")).ToList();

            var girlDataList = sumData.Where(w => w.Sex.Contains("女")).ToList();

            var sumDataBoyResults = Sort(boyDataList.GroupBy(g => g.Model).Select(s => new KeyValuePair<string, int>(s.Key, s.Count())).ToList());

            var sumDataGirlResults = Sort(girlDataList.GroupBy(g => g.Model).Select(s => new KeyValuePair<string, int>(s.Key, s.Count())).ToList());

            var sumDataResults = Sort(sumData.GroupBy(g=>g.Model).Select(s => new KeyValuePair<string, int>(s.Key, s.Count())).ToList());

            for (int i = 0; i < sumDataResults.Count; i++)
            {
                sumTab.Rows.Add(sumTab.NewRow());
            }

            for (int i = 0; i < sumDataBoyResults.Count; i++)
            {
                sumTab.Rows[i][1] = sumDataBoyResults[i].Key;

                sumTab.Rows[i][2] = sumDataBoyResults[i].Value;
            }

            for (int i = 0; i < sumDataGirlResults.Count; i++)
            {
                sumTab.Rows[i][5] = sumDataGirlResults[i].Key;

                sumTab.Rows[i][6] = sumDataGirlResults[i].Value;
            }

            for (int i = 0; i < sumDataResults.Count; i++)
            {
                sumTab.Rows[i][9] = sumDataResults[i].Key;

                sumTab.Rows[i][10] = sumDataResults[i].Value;
            }

            tabs.Add(sumTab);

            saveResultFileDialog.FileName = "匹配结果";

            DialogResult dialogResult = saveResultFileDialog.ShowDialog();

            if (dialogResult != DialogResult.OK && dialogResult != DialogResult.Yes)
            {
                return;
            }

            string filePath = saveResultFileDialog.FileName;

            ExcelHelper.TableToExcelForXLS(tabs, filePath);

            MessageBox.Show("匹配成功！");
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

        private List<KeyValuePair<string, int>> Sort(List<KeyValuePair<string, int>> data)
        {
            var ruleCache = new Cache<List<SizeRuleModel>>();

            var ruleDataResult = ruleCache.GetCache("SizeRule");

            if (!ruleDataResult.IsSuccess)
            {
                MessageBox.Show("请导入规则信息！");
            }

            var models = ruleDataResult.Data.GroupBy(g => g.Model).Select(s => s.Key).ToList();

            var query = from r in models.AsQueryable()
                        join s in data.AsQueryable() on r equals s.Key into l
                        from a in l.DefaultIfEmpty(new KeyValuePair<string, int>(r, 0))
                        select new KeyValuePair<string, int>(r, a.Value);

            return query.Where(w=>w.Value != 0).ToList();
        }
    }
}
