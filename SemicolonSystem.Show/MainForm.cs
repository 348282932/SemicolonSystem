using SemicolonSystem.Business;
using SemicolonSystem.Common;
using SemicolonSystem.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading;
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
            var tabs = new List<DataTable>();

            ShowLoadingForm("正在匹配，请稍等片刻！", this, (obj) => 
            {
                var dataResult = OrderService.GetMatchingResult();

                if (!dataResult.IsSuccess)
                {
                    MessageBox.Show("匹配失败！失败原因：" + dataResult.Message);
                }

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

                DataTable sumTabA = new DataTable();

                sumTabA.TableName = "汇总1";

                sumTabA.Columns.Add(new DataColumn("汇总(男)"));

                sumTabA.Columns.Add(new DataColumn("号型(男)"));

                sumTabA.Columns.Add(new DataColumn("数量(男)"));

                sumTabA.Columns.Add(new DataColumn());

                sumTabA.Columns.Add(new DataColumn("汇总(女)"));

                sumTabA.Columns.Add(new DataColumn("号型(女)"));

                sumTabA.Columns.Add(new DataColumn("数量(女)"));

                sumTabA.Columns.Add(new DataColumn());

                sumTabA.Columns.Add(new DataColumn("总汇总"));

                sumTabA.Columns.Add(new DataColumn("号型"));

                sumTabA.Columns.Add(new DataColumn("数量"));

                List<MatchingRowModel> sumData = new List<MatchingRowModel>();

                foreach (var item in dataResult.Data)
                {
                    sumData.AddRange(item.MatchingRows);
                }

                var boyDataList = sumData.Where(w => w.Sex.Contains("男")).ToList();

                var girlDataList = sumData.Where(w => w.Sex.Contains("女")).ToList();

                var sumDataBoyResults = Sort(boyDataList.GroupBy(g => g.Model).Select(s => new KeyValuePair<string, int>(s.Key, s.Count())).ToList());

                var sumDataGirlResults = Sort(girlDataList.GroupBy(g => g.Model).Select(s => new KeyValuePair<string, int>(s.Key, s.Count())).ToList());

                var sumDataResults = Sort(sumData.GroupBy(g => g.Model).Select(s => new KeyValuePair<string, int>(s.Key, s.Count())).ToList());

                for (int i = 0; i < sumDataResults.Count; i++)
                {
                    sumTabA.Rows.Add(sumTabA.NewRow());
                }

                for (int i = 0; i < sumDataBoyResults.Count; i++)
                {
                    sumTabA.Rows[i][1] = sumDataBoyResults[i].Key;

                    sumTabA.Rows[i][2] = sumDataBoyResults[i].Value;
                }

                for (int i = 0; i < sumDataGirlResults.Count; i++)
                {
                    sumTabA.Rows[i][5] = sumDataGirlResults[i].Key;

                    sumTabA.Rows[i][6] = sumDataGirlResults[i].Value;
                }

                for (int i = 0; i < sumDataResults.Count; i++)
                {
                    sumTabA.Rows[i][9] = sumDataResults[i].Key;

                    sumTabA.Rows[i][10] = sumDataResults[i].Value;
                }

                tabs.Add(sumTabA);

                DataTable sumTabB = new DataTable();

                sumTabB.TableName = "汇总2";

                var maxRowCount = sumDataResults.Max(m => m.Value);

                for (int i = 0; i < maxRowCount; i++)
                {
                    sumTabB.Rows.Add(sumTabB.NewRow());
                }

                for (int i = 0; i < sumDataResults.Count; i++)
                {
                    sumTabB.Columns.Add(new DataColumn(string.Format("号型({0})", sumDataResults[i].Key)));

                    sumTabB.Columns.Add(new DataColumn(string.Format("姓名({0})", sumDataResults[i].Key)));

                    sumTabB.Columns.Add(new DataColumn(string.Format("年级({0})", sumDataResults[i].Key)));

                    sumTabB.Columns.Add(new DataColumn());

                    var sumList = sumData.Where(w => w.Model == sumDataResults[i].Key).Select(s => new { s.Model, s.SheetName, s.Name }).ToList();

                    for (int j = 0; j < sumList.Count; j++)
                    {
                        sumTabB.Rows[j][i * 4] = sumList[j].Model;

                        sumTabB.Rows[j][i * 4 + 1] = sumList[j].Name;

                        sumTabB.Rows[j][i * 4 + 2] = sumList[j].SheetName;
                    }
                }

                tabs.Add(sumTabB);
            });

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
            path =Application.StartupPath.Substring(0, Application.StartupPath.LastIndexOf("bin")) + "Template\\规则模版.xlsx";
#else
            path = Global.InstallPath + "\\Template\\规则模版.xlsx";
#endif

            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

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
            path = Application.StartupPath.Substring(0, Application.StartupPath.LastIndexOf("bin")) + "Template\\订单模版.xlsx";
#else
            path = Global.InstallPath + "\\Template\\订单模版.xlsx";
#endif

            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                File.Copy(path, filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("获取订单模板异常！异常原因：{0}，请尝试在该路径下取出模板！{1}{2}", ex.Message, Environment.NewLine, path));

                return;
            }

            MessageBox.Show("导出模板成功！");
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private List<KeyValuePair<string, int>> Sort(List<KeyValuePair<string, int>> data)
        {
            var ruleCache = new Cache<List<SizeRuleItemModel>>();

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

        /// <summary>
        /// 显示 Loding
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="owner"></param>
        /// <param name="work"></param>
        /// <param name="workArg"></param>
        public static void ShowLoadingForm(string msg, Form owner, ParameterizedThreadStart work, object workArg = null)
        {
            LoadingForm loadingForm = new LoadingForm(msg);

            dynamic expObj = new ExpandoObject();

            expObj.Form = loadingForm;

            expObj.WorkArg = workArg;

            loadingForm.SetWorkAction(work, expObj);

            loadingForm.ShowDialog(owner);

            if (loadingForm.WorkException != null)
            {
                throw loadingForm.WorkException;
            }
        }

        /// <summary>
        /// 汇总结果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Summary_Click(object sender, EventArgs e)
        {
            openFilesDialog.FileName = string.Empty;

            DialogResult dialogResult = openFilesDialog.ShowDialog();

            if (dialogResult != DialogResult.OK && dialogResult != DialogResult.Yes)
            {
                return;
            }

            var filePathArr = openFilesDialog.FileNames;

            List<SummaryModel> summaryList = new List<SummaryModel>();

            int flag = -1;

            bool isSummaryAll = true;

            MessageBoxButtons messButton = MessageBoxButtons.YesNo;

            foreach (var item in filePathArr)
            {
                var dataResult = SummaryService.SummaryResultTable(item);

                if (!dataResult.IsSuccess)
                {
                    MessageBox.Show("汇总数据异常！异常原因：" + dataResult.Message);

                    return;
                }

                if (dataResult.Data.All(a => !string.IsNullOrWhiteSpace(a.Sex)))
                {
                    MessageBox.Show("汇总数据异常！异常原因：汇总表总汇总数据为空！");

                    return;
                }

                if (dataResult.Data.Any(a => !string.IsNullOrWhiteSpace(a.Sex)))
                {
                    if (flag == -1)
                    {
                        flag = 1;
                    }
                    else if (flag != 1)
                    {
                        DialogResult dr = MessageBox.Show("检测到表格格式不一致，继续操作则仅汇总总数据！是否继续？", "警告", messButton);

                        if (dr != DialogResult.Yes)
                        {
                            return;
                        }
                        else
                        {
                            isSummaryAll = false;
                        }
                    }
                }
                else
                {
                    if (flag == -1)
                    {
                        flag = 2;
                    }
                    else if (flag != 2)
                    {
                        DialogResult dr = MessageBox.Show("检测到表格格式不一致，继续操作则仅汇总总数据！", "警告", messButton);

                        if (dr != DialogResult.OK)
                        {
                            return;
                        }
                        else
                        {
                            isSummaryAll = false;
                        }
                    }
                }

                summaryList.AddRange(dataResult.Data);
            }

            List<DataTable> tabs = new List<DataTable>();

            DataTable tab = new DataTable();

            tab.TableName = "结果汇总";

            tab.Columns.Add(new DataColumn("汇总(男)"));

            tab.Columns.Add(new DataColumn("号型(男)"));

            tab.Columns.Add(new DataColumn("数量(男)"));

            tab.Columns.Add(new DataColumn());

            tab.Columns.Add(new DataColumn("汇总(女)"));

            tab.Columns.Add(new DataColumn("号型(女)"));

            tab.Columns.Add(new DataColumn("数量(女)"));

            tab.Columns.Add(new DataColumn());

            tab.Columns.Add(new DataColumn("总汇总"));

            tab.Columns.Add(new DataColumn("号型"));

            tab.Columns.Add(new DataColumn("数量"));

            var rowsCount = summaryList.Count(c => string.IsNullOrWhiteSpace(c.Sex));

            for (int i = 0; i < rowsCount; i++)
            {
                tab.Rows.Add(tab.NewRow());
            }

            if (isSummaryAll)
            {
                var manList = summaryList
                .Where(w => w.Sex == "男")
                .GroupBy(g => g.Model)
                .Select(s => new
                {
                    Model = s.Key,
                    Count = s.Sum(ss => ss.Count)
                }).ToList();

                for (int i = 0; i < manList.Count; i++)
                {
                    tab.Rows[i]["号型(男)"] = manList[i].Model;
                    tab.Rows[i]["数量(男)"] = manList[i].Count;
                }

                var womanList = summaryList
                    .Where(w => w.Sex == "女")
                    .GroupBy(g => g.Model)
                    .Select(s => new
                    {
                        Model = s.Key,
                        Count = s.Sum(ss => ss.Count)
                    }).ToList();

                for (int i = 0; i < womanList.Count; i++)
                {
                    tab.Rows[i]["号型(女)"] = womanList[i].Model;
                    tab.Rows[i]["数量(女)"] = womanList[i].Count;
                }
            }

            var sumList = summaryList
                .Where(w => string.IsNullOrWhiteSpace(w.Sex))
                .GroupBy(g => g.Model)
                .Select(s => new
                {
                    Model = s.Key,
                    Count = s.Sum(ss => ss.Count)
                }).ToList();

            for (int i = 0; i < sumList.Count; i++)
            {
                tab.Rows[i]["号型"] = sumList[i].Model;
                tab.Rows[i]["数量"] = sumList[i].Count;
            }

            tabs.Add(tab);

            saveResultFileDialog.FileName = "汇总结果";

            dialogResult = saveResultFileDialog.ShowDialog();

            if (dialogResult != DialogResult.OK && dialogResult != DialogResult.Yes)
            {
                return;
            }

            string filePath = saveResultFileDialog.FileName;

            ExcelHelper.TableToExcelForXLS(tabs, filePath);

            MessageBox.Show("匹配成功！");
        }
    }
}
