using SemicolonSystem.Common;
using SemicolonSystem.Model;
using System;
using System.Collections.Generic;
using System.Data;

namespace SemicolonSystem.Business
{
    public class RuleService
    {
        /// <summary>
        /// 导入尺寸规则 Excel
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static DataResult ImportRuleExcel(string fileName)
        {
            DataResult dataResult = new DataResult();

            List<DataTable> tabs = new List<DataTable>();

            try
            {
                tabs = ExcelHelper.GetDataTable(fileName);
            }
            catch (Exception ex)
            {
                return new DataResult(ex.Message);
            }

            if (tabs == null || tabs.Count == 0)
            {
                return new DataResult("导入模板失败！请检查模板是否为空，格式是否有误！");
            }

            //if (tab.Columns[0].ToString().Trim() != "尺寸/型号")
            //{
            //    return new DataResult("请按模版导入 Excel");
            //}

            List<SizeRuleModel> sizeRuleList = new List<SizeRuleModel>();

            try
            {
                foreach (var tab in tabs)
                {
                    if (tab == null || tab.Rows.Count == 0 || tab.Columns.Count == 0)
                    {
                        return new DataResult("请按模版导入 Excel");
                    }

                    SizeRuleModel sizeRule = new SizeRuleModel();

                    sizeRule.Name = tab.TableName;

                    if (tab.TableName.Contains("男") && tab.TableName.Contains("女"))
                    {
                        return new DataResult("规则模版名称不能同时包含男和女");
                    }

                    if (tab.TableName.Contains("男"))
                    {
                        sizeRule.Sex = "男";
                    }
                    else if (tab.TableName.Contains("女"))
                    {
                        sizeRule.Sex = "女";
                    }
                    else
                    {
                        sizeRule.Sex = string.Empty;
                    }

                    List<SizeRuleItemModel> sizeRuleItemList = new List<SizeRuleItemModel>();

                    for (int i = 0; i < tab.Rows.Count; i++)
                    {
                        for (int j = 1; j < tab.Columns.Count; j++)
                        {
                            var sizeStr = tab.Rows[i][j].ToString().Trim();

                            if (string.IsNullOrWhiteSpace(sizeStr)) continue;

                            sizeRuleItemList.Add(new SizeRuleItemModel
                            {
                                Model = tab.Rows[i][0].ToString().Trim(),
                                Position = tab.Columns[j].ColumnName.Trim(),
                                Size = Convert.ToDecimal(sizeStr)
                            });
                        }
                    }

                    sizeRule.Items = sizeRuleItemList;

                    sizeRuleList.Add(sizeRule);
                }
            }
            catch (FormatException)
            {
                return new DataResult("导入的尺寸大小必须是数字或小数");
            }
            catch (Exception)
            {
                return new DataResult("导入 Excel 格式不正确！请按照模版导入！");
            }

            Cache<List<SizeRuleModel>> cache = new Cache<List<SizeRuleModel>>();

            dataResult = cache.SetCache("SizeRule", sizeRuleList);

            cache.Clear("Order");

            cache.Clear("WeightCofig", true);

            return dataResult;
        }
    }
}
