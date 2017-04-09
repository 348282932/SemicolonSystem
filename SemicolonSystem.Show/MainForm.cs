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

            if (dialogResult != DialogResult.OK && dialogResult != DialogResult.Yes)
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
            var ruleCache = new Cache<List<SizeRuleModel>>();

            var ruletDataResult = ruleCache.GetCache("SizeRule");

            if (!ruletDataResult.IsSuccess)
            {
                MessageBox.Show("请导入规则信息！");

                return;
            }

            if (ruletDataResult.Data.Count == 1)
            {
                var cache = new Cache<List<WeightModel>>();

                MatchingForm matchingForm = new MatchingForm(ruletDataResult.Data[0].Name);

                matchingForm.ShowDialog();
            }
            else
            {
                MatchingSettingForm matchingForm = new MatchingSettingForm();

                matchingForm.ShowDialog();
            }
        }
        
        /// <summary>
        /// 匹配
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Matching_Click(object sender, EventArgs e)
        {
            try
            {
                var tabs = new List<DataTable>();

                var isSuccess = true;

                ShowLoadingForm("正在匹配，请稍等片刻！", this, (obj) =>
                {
                    var dataResult = Matching();

                    if (!dataResult.IsSuccess)
                    {
                        MessageBox.Show("匹配失败！失败原因：" + dataResult.Message);

                        isSuccess = false;

                        return;
                    }

                    tabs = dataResult.Data;
                });

                if (!isSuccess)
                {
                    return;
                }

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
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("匹配失败！失败原因{0}，请仔细检查表格格式是否正确！", ex.Message));
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
            path = Application.StartupPath.Substring(0, Application.StartupPath.LastIndexOf("bin")) + "Template\\规则模版.xlsx";
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
            catch (Exception ex)
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
        private List<KeyValuePair<string, int>> Sort(List<KeyValuePair<string, int>> data, string sizeRule)
        {
            var ruleCache = new Cache<List<SizeRuleModel>>();

            var ruleDataResult = ruleCache.GetCache("SizeRule");

            if (!ruleDataResult.IsSuccess)
            {
                MessageBox.Show("请导入规则信息！");
            }

            var ruleData = ruleDataResult.Data.FirstOrDefault(w => w.Name == sizeRule);

            var models = ruleData.Items.GroupBy(g => g.Model).Select(s => s.Key).ToList();

            var query = from r in models.AsQueryable()
                        join s in data.AsQueryable() on r equals s.Key into l
                        from a in l.DefaultIfEmpty(new KeyValuePair<string, int>(r, 0))
                        select new KeyValuePair<string, int>(r, a.Value);

            return query.Where(w => w.Value != 0).ToList();
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

            var columnNames = summaryList.GroupBy(g => g.RuleName).Select(s => s.Key).ToList();

            var manColumnNames = summaryList.Where(w=>w.Sex == "男").GroupBy(g => g.RuleName).Select(s => s.Key).ToList();

            var womanColumnNames = summaryList.Where(w=>w.Sex == "女").GroupBy(g => g.RuleName).Select(s => s.Key).ToList();

            List<DataTable> tabs = new List<DataTable>();

            DataTable tab = new DataTable();

            tab.TableName = "汇总1";

            tab.Columns.Add("汇总(男)");

            foreach (var item in manColumnNames)
            {
                tab.Columns.Add(string.Format("号型(男)_{0}", item));

                tab.Columns.Add(string.Format("数量(男)_{0}", item));
            }

            tab.Columns.Add(new DataColumn());

            tab.Columns.Add(new DataColumn("汇总(女)"));

            foreach (var item in womanColumnNames)
            {
                tab.Columns.Add(string.Format("号型(女)_{0}", item));

                tab.Columns.Add(string.Format("数量(女)_{0}", item));
            }

            tab.Columns.Add(new DataColumn());

            tab.Columns.Add(new DataColumn("总汇总"));

            foreach (var item in columnNames)
            {
                tab.Columns.Add(string.Format("号型_{0}", item));

                tab.Columns.Add(string.Format("数量_{0}", item));
            }

            var rowsCount = summaryList.Count(c => string.IsNullOrWhiteSpace(c.Sex));

            for (int i = 0; i < rowsCount; i++)
            {
                tab.Rows.Add(tab.NewRow());
            }

            if (isSummaryAll)
            {
                foreach (var item in manColumnNames)
                {
                    var manList = summaryList
                        .Where(w =>w.RuleName == item && w.Sex == "男")
                        .GroupBy(g => g.Model)
                        .Select(s => new
                        {
                            Model = s.Key,
                            Count = s.Sum(ss => ss.Count)
                        }).ToList();

                    for (int i = 0; i < manList.Count; i++)
                    {
                        tab.Rows[i]["号型(男)_" + item] = manList[i].Model;
                        tab.Rows[i]["数量(男)_" + item] = manList[i].Count;
                    }
                }

                foreach (var item in womanColumnNames)
                {
                    var womanList = summaryList
                        .Where(w => w.RuleName == item && w.Sex == "女")
                        .GroupBy(g => g.Model)
                        .Select(s => new
                        {
                            Model = s.Key,
                            Count = s.Sum(ss => ss.Count)
                        }).ToList();

                    for (int i = 0; i < womanList.Count; i++)
                    {
                        tab.Rows[i]["号型(女)_" + item] = womanList[i].Model;
                        tab.Rows[i]["数量(女)_" + item] = womanList[i].Count;
                    }
                }
            }
            foreach (var item in columnNames)
            {
                var sumList = summaryList
                .Where(w => w.RuleName == item && string.IsNullOrWhiteSpace(w.Sex))
                .GroupBy(g => g.Model)
                .Select(s => new
                {
                    Model = s.Key,
                    Count = s.Sum(ss => ss.Count)
                }).ToList();

                for (int i = 0; i < sumList.Count; i++)
                {
                    tab.Rows[i]["号型_" + item] = sumList[i].Model;
                    tab.Rows[i]["数量_" + item] = sumList[i].Count;
                }
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

        /// <summary>
        /// 匹配
        /// </summary>
        /// <returns></returns>
        private DataResult<List<DataTable>> Matching()
        {
            List<DataTable> tabs = new List<DataTable>();

            var dataResult = OrderService.GetMatchingResult();

            if (!dataResult.IsSuccess)
            {
                return new DataResult<List<DataTable>>(dataResult.Message);
            }

            List<MatchingDataModel> listData = new List<MatchingDataModel>();

            var list = dataResult.Data;

            var orderCache = new Cache<List<OrderModel>>();

            var orderDataResult = orderCache.GetCache("Order");

            if (!orderDataResult.IsSuccess)
            {
                return new DataResult<List<DataTable>>("请导入订单信息");
            }

            foreach (var sheet in orderDataResult.Data)
            {
                var matchingData = new MatchingDataModel();

                matchingData.SheetName = sheet.SheetName;

                var rows = new List<MatchingDataSheetModel>();

                foreach (var row in sheet.OrderRows)
                {
                    List<MatchingDataSheetItemModel> items = new List<MatchingDataSheetItemModel>();

                    var sexList = list.Where(w => w.Sex == row.Sex).ToList();

                    if (sexList == null || sexList.Count == 0)
                    {
                        sexList = list.Where(w => string.IsNullOrWhiteSpace(w.Sex)).ToList();
                    }

                    foreach (var f in sexList)
                    {
                        var data = f.Items.FirstOrDefault(w => w.SheetName == sheet.SheetName).MatchingRows.FirstOrDefault(ff => ff.Name == row.Name);

                        if (data == null) continue;

                        items.Add(new MatchingDataSheetItemModel
                        {
                            Model = data.Model,
                            MatchingLevel = data.MatchingLevel,
                            SizeRuleName = f.SizeRuleName
                        });
                    }

                    rows.Add(new MatchingDataSheetModel
                    {
                        SheetName = sheet.SheetName,
                        Name = row.Name,
                        Sex = row.Sex,
                        Items = items,
                        Property1 = row.Property1,
                        Property2 = row.Property2,
                        Property3 = row.Property3
                    });
                }

                matchingData.Items = rows;

                listData.Add(matchingData);
            }

            var isHasSex = false;

            foreach (var item in listData)
            {
                item.Items = item.Items.OrderBy(o => o.Property1).ThenBy(o => o.Property2).ThenBy(o => o.Property3).ToList();

                isHasSex = item.Items.Any(a => !string.IsNullOrWhiteSpace(a.Sex));

                DataTable dt = new DataTable();

                dt.TableName = item.SheetName;

                var sumResultList = new List<KeyValuePair<string, List<KeyValuePair<string, int>>>>();

                foreach (var model in list)
                {
                    var sheetRows = model.Items.FirstOrDefault(f => f.SheetName == item.SheetName).MatchingRows;

                    sumResultList.Add(new KeyValuePair<string, List<KeyValuePair<string, int>>>(model.SizeRuleName, Sort(sheetRows.GroupBy(g => g.Model).Select(s => new KeyValuePair<string, int>(s.Key, s.Count())).ToList(), model.SizeRuleName)));
                }

                if (isHasSex)
                {
                    var manfirstRow = item.Items.FirstOrDefault(f => f.Sex == "男");

                    if (!string.IsNullOrWhiteSpace(manfirstRow.Property1)) dt.Columns.Add("属性一_男");

                    if (!string.IsNullOrWhiteSpace(manfirstRow.Property2)) dt.Columns.Add("属性二_男");

                    if (!string.IsNullOrWhiteSpace(manfirstRow.Property3)) dt.Columns.Add("属性三_男");

                    dt.Columns.Add("姓名(男)");

                    foreach (var model in manfirstRow.Items)
                    {
                        dt.Columns.Add(string.Format("结果(男)_{0}", model.SizeRuleName));

                        dt.Columns.Add(string.Format("匹配程度(男)_{0}", model.SizeRuleName));
                    }

                    dt.Columns.Add();

                    foreach (var model in manfirstRow.Items)
                    {
                        dt.Columns.Add(string.Format("号型(男)_{0}", model.SizeRuleName));

                        dt.Columns.Add(string.Format("数量(男)_{0}", model.SizeRuleName));
                    }

                    dt.Columns.Add();

                    var womanfirstRow = item.Items.FirstOrDefault(f => f.Sex == "女");

                    if (!string.IsNullOrWhiteSpace(manfirstRow.Property1)) dt.Columns.Add("属性一_女");

                    if (!string.IsNullOrWhiteSpace(manfirstRow.Property2)) dt.Columns.Add("属性二_女");

                    if (!string.IsNullOrWhiteSpace(manfirstRow.Property3)) dt.Columns.Add("属性三_女");

                    dt.Columns.Add("姓名(女)");

                    foreach (var model in womanfirstRow.Items)
                    {
                        dt.Columns.Add(string.Format("结果(女)_{0}", model.SizeRuleName));

                        dt.Columns.Add(string.Format("匹配程度(女)_{0}", model.SizeRuleName));
                    }

                    dt.Columns.Add();

                    foreach (var model in womanfirstRow.Items)
                    {
                        dt.Columns.Add(string.Format("号型(女)_{0}", model.SizeRuleName));

                        dt.Columns.Add(string.Format("数量(女)_{0}", model.SizeRuleName));
                    }

                    dt.Columns.Add();

                    dt.Columns.Add("汇总");

                    var firstRow = new MatchingDataSheetModel();

                    var items = new List<MatchingDataSheetItemModel>();

                    items.AddRange(manfirstRow.Items);

                    items.AddRange(womanfirstRow.Items);

                    firstRow.Items = items;

                    for (int i = 0; i < dataResult.Data.Count; i++)
                    {
                        dt.Columns.Add(string.Format("汇总_号型_{0}", dataResult.Data[i].SizeRuleName));

                        dt.Columns.Add(string.Format("汇总_数量_{0}", dataResult.Data[i].SizeRuleName));

                        //if (dataResult.Data.Count - 1 == i)
                        //{
                        //    break;
                        //}
                    }

                    var boyList = item.Items.Where(w => w.Sex.Contains("男")).ToList();

                    var girlList = item.Items.Where(w => w.Sex.Contains("女")).ToList();

                    var sumBoyResultList = new List<KeyValuePair<string, List<KeyValuePair<string, int>>>>();

                    var sumGirlResultList = new List<KeyValuePair<string, List<KeyValuePair<string, int>>>>();

                    foreach (var model in list)
                    {
                        var sheetRows = model.Items.FirstOrDefault(f => f.SheetName == item.SheetName).MatchingRows;

                        sumBoyResultList.Add(new KeyValuePair<string, List<KeyValuePair<string, int>>>(model.SizeRuleName, Sort(sheetRows.Where(w => w.Sex.Contains("男")).GroupBy(g => g.Model).Select(s => new KeyValuePair<string, int>(s.Key, s.Count())).ToList(), model.SizeRuleName)));

                        sumGirlResultList.Add(new KeyValuePair<string, List<KeyValuePair<string, int>>>(model.SizeRuleName, Sort(sheetRows.Where(w => w.Sex.Contains("女")).GroupBy(g => g.Model).Select(s => new KeyValuePair<string, int>(s.Key, s.Count())).ToList(), model.SizeRuleName)));
                    }

                    var index = Math.Max(boyList.Count, girlList.Count);

                    for (int i = 0; i < index; i++)
                    {
                        dt.Rows.Add(dt.NewRow());
                    }

                    for (int i = 0; i < boyList.Count; i++)
                    {
                        dt.Rows[i]["姓名(男)"] = boyList[i].Name;

                        if (!string.IsNullOrWhiteSpace(manfirstRow.Property1)) dt.Rows[i]["属性一_男"] = boyList[i].Property1;

                        if (!string.IsNullOrWhiteSpace(manfirstRow.Property2)) dt.Rows[i]["属性二_男"] = boyList[i].Property2;

                        if (!string.IsNullOrWhiteSpace(manfirstRow.Property3)) dt.Rows[i]["属性三_男"] = boyList[i].Property3;

                        var mancount = boyList[i].Items.Where(s => s.SizeRuleName.Contains("男")).ToList();

                        for (int j = 0; j < mancount.Count; j++)
                        {
                            dt.Rows[i][string.Format("结果(男)_{0}", mancount[j].SizeRuleName)] = mancount[j].Model;

                            dt.Rows[i][string.Format("匹配程度(男)_{0}", mancount[j].SizeRuleName)] = mancount[j].MatchingLevel.ToString();
                        }
                    }

                    foreach (var sumBoy in sumBoyResultList)
                    {
                        for (int i = 0; i < sumBoy.Value.Count; i++)
                        {
                            dt.Rows[i][string.Format("号型(男)_{0}", sumBoy.Key)] = sumBoy.Value[i].Key;

                            dt.Rows[i][string.Format("数量(男)_{0}", sumBoy.Key)] = sumBoy.Value[i].Value;
                        }
                    }

                    for (int i = 0; i < girlList.Count; i++)
                    {
                        dt.Rows[i]["姓名(女)"] = girlList[i].Name;

                        if (!string.IsNullOrWhiteSpace(womanfirstRow.Property1)) dt.Rows[i]["属性一_女"] = girlList[i].Property1;

                        if (!string.IsNullOrWhiteSpace(womanfirstRow.Property2)) dt.Rows[i]["属性二_女"] = girlList[i].Property2;

                        if (!string.IsNullOrWhiteSpace(womanfirstRow.Property3)) dt.Rows[i]["属性三_女"] = girlList[i].Property3;

                        var wonancount = girlList[i].Items.Where(s => s.SizeRuleName.Contains("女")).ToList();

                        for (int j = 0; j < wonancount.Count; j++)
                        {
                            dt.Rows[i][string.Format("结果(女)_{0}", wonancount[j].SizeRuleName)] = wonancount[j].Model;

                            dt.Rows[i][string.Format("匹配程度(女)_{0}", wonancount[j].SizeRuleName)] = wonancount[j].MatchingLevel.ToString();
                        }
                    }

                    foreach (var sumGirl in sumGirlResultList)
                    {
                        for (int i = 0; i < sumGirl.Value.Count; i++)
                        {
                            dt.Rows[i][string.Format("号型(女)_{0}", sumGirl.Key)] = sumGirl.Value[i].Key;

                            dt.Rows[i][string.Format("数量(女)_{0}", sumGirl.Key)] = sumGirl.Value[i].Value;
                        }
                    }

                    var sumRowCount = sumResultList.Max(m => m.Value.Count);

                    if (index < sumRowCount)
                    {
                        for (int i = 0; i < sumRowCount - index; i++)
                        {
                            dt.Rows.Add(dt.NewRow());
                        }
                    }

                    foreach (var sum in sumResultList)
                    {
                        for (int i = 0; i < sum.Value.Count; i++)
                        {
                            dt.Rows[i][string.Format("汇总_号型_{0}", sum.Key)] = sum.Value[i].Key;

                            dt.Rows[i][string.Format("汇总_数量_{0}", sum.Key)] = sum.Value[i].Value;
                        }
                    }
                }
                else
                {
                    var firstRow = item.Items.FirstOrDefault();

                    if (!string.IsNullOrWhiteSpace(firstRow.Property1)) dt.Columns.Add("属性一");

                    if (!string.IsNullOrWhiteSpace(firstRow.Property2)) dt.Columns.Add("属性二");

                    if (!string.IsNullOrWhiteSpace(firstRow.Property3)) dt.Columns.Add("属性三");

                    dt.Columns.Add("姓名");

                    foreach (var model in firstRow.Items)
                    {
                        dt.Columns.Add(string.Format("结果_{0}", model.SizeRuleName));

                        dt.Columns.Add(string.Format("匹配程度_{0}", model.SizeRuleName));
                    }

                    dt.Columns.Add();

                    foreach (var model in firstRow.Items)
                    {
                        dt.Columns.Add(string.Format("号型_{0}", model.SizeRuleName));

                        dt.Columns.Add(string.Format("数量_{0}", model.SizeRuleName));
                    }

                    for (int i = 0; i < item.Items.Count; i++)
                    {
                        dt.Rows.Add(dt.NewRow());

                        dt.Rows[i]["姓名"] = item.Items[i].Name;

                        if (!string.IsNullOrWhiteSpace(firstRow.Property1)) dt.Rows[i]["属性一"] = item.Items[i].Property1;

                        if (!string.IsNullOrWhiteSpace(firstRow.Property2)) dt.Rows[i]["属性二"] = item.Items[i].Property2;

                        if (!string.IsNullOrWhiteSpace(firstRow.Property3)) dt.Rows[i]["属性三"] = item.Items[i].Property3;

                        for (int j = 0; j < item.Items[i].Items.Count; j++)
                        {

                            dt.Rows[i][string.Format("结果_{0}", item.Items[i].Items[j].SizeRuleName)] = item.Items[i].Items[j].Model;

                            dt.Rows[i][string.Format("匹配程度_{0}", item.Items[i].Items[j].SizeRuleName)] = item.Items[i].Items[j].MatchingLevel.ToString();
                        }
                    }

                    foreach (var sum in sumResultList)
                    {
                        for (int i = 0; i < sum.Value.Count; i++)
                        {
                            dt.Rows[i][string.Format("号型_{0}", sum.Key)] = sum.Value[i].Key;

                            dt.Rows[i][string.Format("数量_{0}", sum.Key)] = sum.Value[i].Value;
                        }
                    }
                }

                tabs.Add(dt);
            }

            DataTable sumTabA = new DataTable();

            sumTabA.TableName = "汇总1";

            var sumDataList = new List<KeyValuePair<string, List<KeyValuePair<string, int>>>>();

            var sizeRuleList = new List<KeyValuePair<string, List<MatchingRowModel>>>();

            foreach (var model in dataResult.Data)
            {
                var tempList = new List<MatchingRowModel>();

                foreach (var item in model.Items)
                {
                    tempList.AddRange(item.MatchingRows);
                }

                sizeRuleList.Add(new KeyValuePair<string, List<MatchingRowModel>>(model.SizeRuleName, tempList));
            }

            foreach (var item in sizeRuleList)
            {
                sumDataList.Add(new KeyValuePair<string, List<KeyValuePair<string, int>>>(item.Key, Sort(item.Value.GroupBy(g => g.Model).Select(s => new KeyValuePair<string, int>(s.Key, s.Count())).ToList(), item.Key)));
            }

            var manSizeRule = dataResult.Data.Where(w => w.Sex == "男" || string.IsNullOrWhiteSpace(w.Sex)).ToList();

            if (manSizeRule != null && manSizeRule.Count >= 0)
            {
                sumTabA.Columns.Add("汇总_男");
            }

            foreach (var model in manSizeRule)
            {
                sumTabA.Columns.Add(string.Format("号型(男)_{0}", model.SizeRuleName));

                sumTabA.Columns.Add(string.Format("数量(男)_{0}", model.SizeRuleName));
            }

            var womanSizeRule = dataResult.Data.Where(w => w.Sex == "女" || string.IsNullOrWhiteSpace(w.Sex)).ToList();

            if (womanSizeRule != null && womanSizeRule.Count >= 0)
            {
                sumTabA.Columns.Add();

                sumTabA.Columns.Add("汇总_女");
            }

            foreach (var model in womanSizeRule)
            {
                sumTabA.Columns.Add(string.Format("号型(女)_{0}", model.SizeRuleName));

                sumTabA.Columns.Add(string.Format("数量(女)_{0}", model.SizeRuleName));
            }

            sumTabA.Columns.Add();

            sumTabA.Columns.Add(new DataColumn("汇总"));

            foreach (var model in dataResult.Data)
            {
                sumTabA.Columns.Add(string.Format("汇总_号型_{0}", model.SizeRuleName));

                sumTabA.Columns.Add(string.Format("汇总_数量_{0}", model.SizeRuleName));
            }

            var sumItems = new List<KeyValuePair<string, List<MatchingRowModel>>>();

            foreach (var item in list)
            {
                var data = new List<MatchingRowModel>();

                foreach (var sheet in item.Items)
                {
                    data.AddRange(sheet.MatchingRows);
                }

                sumItems.Add(new KeyValuePair<string, List<MatchingRowModel>>(item.SizeRuleName, data));
            }

            var sumBoyDataList = new List<KeyValuePair<string, List<KeyValuePair<string, int>>>>();

            var sumGirlDataList = new List<KeyValuePair<string, List<KeyValuePair<string, int>>>>();

            foreach (var model in sumItems)
            {
                sumBoyDataList.Add(new KeyValuePair<string, List<KeyValuePair<string, int>>>(model.Key, Sort(model.Value.Where(w => w.Sex.Contains("男")).GroupBy(g => g.Model).Select(s => new KeyValuePair<string, int>(s.Key, s.Count())).ToList(), model.Key)));

                sumGirlDataList.Add(new KeyValuePair<string, List<KeyValuePair<string, int>>>(model.Key, Sort(model.Value.Where(w => w.Sex.Contains("女")).GroupBy(g => g.Model).Select(s => new KeyValuePair<string, int>(s.Key, s.Count())).ToList(), model.Key)));
            }

            //var count = Math.Max(sumBoyDataList.Max(m => m.Value.Count), sumGirlDataList.Max(m => m.Value.Count));

            var count = sumDataList.Max(m => m.Value.Count);

            for (int i = 0; i < count; i++)
            {
                sumTabA.Rows.Add(sumTabA.NewRow());
            }

            foreach (var sumBoy in sumBoyDataList)
            {
                for (int i = 0; i < sumBoy.Value.Count; i++)
                {
                    sumTabA.Rows[i][string.Format("号型(男)_{0}", sumBoy.Key)] = sumBoy.Value[i].Key;

                    sumTabA.Rows[i][string.Format("数量(男)_{0}", sumBoy.Key)] = sumBoy.Value[i].Value;
                }
            }

            foreach (var sumGirl in sumGirlDataList)
            {
                for (int i = 0; i < sumGirl.Value.Count; i++)
                {
                    sumTabA.Rows[i][string.Format("号型(女)_{0}", sumGirl.Key)] = sumGirl.Value[i].Key;

                    sumTabA.Rows[i][string.Format("数量(女)_{0}", sumGirl.Key)] = sumGirl.Value[i].Value;
                }
            }

            foreach (var item in sumDataList)
            {
                for (int i = 0; i < item.Value.Count; i++)
                {
                    sumTabA.Rows[i][string.Format("汇总_号型_{0}", item.Key)] = item.Value[i].Key;

                    sumTabA.Rows[i][string.Format("汇总_数量_{0}", item.Key)] = item.Value[i].Value;
                }
            }

            tabs.Add(sumTabA);

            foreach (var item in sumDataList)
            {
                DataTable sumTabB = new DataTable();

                sumTabB.TableName = "汇总_" + item.Key;

                var maxRowCount = item.Value.Max(m => m.Value);

                for (int i = 0; i < maxRowCount; i++)
                {
                    sumTabB.Rows.Add(sumTabB.NewRow());
                }

                for (int i = 0; i < item.Value.Count; i++)
                {
                    sumTabB.Columns.Add(new DataColumn(string.Format("号型_{0}", item.Value[i].Key)));

                    sumTabB.Columns.Add(new DataColumn(string.Format("姓名_{0}", item.Value[i].Key)));

                    sumTabB.Columns.Add(new DataColumn(string.Format("表名_{0}", item.Value[i].Key)));

                    sumTabB.Columns.Add(new DataColumn());

                    var sumList = sizeRuleList.FirstOrDefault(f => f.Key == item.Key).Value.Where(w => w.Model == item.Value[i].Key).Select(s => new { s.Model, s.SheetName, s.Name }).ToList();

                    for (int j = 0; j < sumList.Count; j++)
                    {
                        sumTabB.Rows[j][i * 4] = sumList[j].Model;

                        sumTabB.Rows[j][i * 4 + 1] = sumList[j].Name;

                        sumTabB.Rows[j][i * 4 + 2] = sumList[j].SheetName;
                    }
                }

                tabs.Add(sumTabB);
            }

            return new DataResult<List<DataTable>>(tabs);
        }
    }
}
