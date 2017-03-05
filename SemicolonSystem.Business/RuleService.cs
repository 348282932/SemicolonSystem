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

            DataTable tab = new DataTable();

            try
            {
                tab = ExcelHelper.GetDataTable(fileName);
            }
            catch (Exception ex)
            {
                return new DataResult(ex.Message);
            }

            List<SizeRuleModel> sizeList = new List<SizeRuleModel>();

            if (tab.Rows.Count == 0 || tab.Columns.Count == 0)
            {
                return new DataResult("请按模版导入 Excel");
            }

            //if (tab.Columns[0].ToString().Trim() != "尺寸/型号")
            //{
            //    return new DataResult("请按模版导入 Excel");
            //}

            try
            {
                for (int i = 0; i < tab.Rows.Count; i++)
                {
                    for (int j = 1; j < tab.Columns.Count; j++)
                    {
                        sizeList.Add(new SizeRuleModel
                        {
                            Model = tab.Rows[i][0].ToString().Trim(),
                            Position = tab.Columns[j].ColumnName.Trim(),
                            Size = Convert.ToDecimal(tab.Rows[i][j].ToString().Trim())
                        });
                    }
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

            dataResult = cache.SetCache("SizeRule", sizeList);

            cache.Clear("Order");

            cache.Clear("WeightCofig");

            return dataResult;
        }
    }
}
