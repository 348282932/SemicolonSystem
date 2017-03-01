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
                dataResult.IsSuccess = false;

                dataResult.Message = ex.Message;

                return dataResult;
            }

            List<SizeRuleModel> sizeList = new List<SizeRuleModel>();

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
                dataResult.IsSuccess = false;

                dataResult.Message = "导入的尺寸大小必须是数字或小数";

                return dataResult;
            }
            catch (Exception)
            {
                dataResult.IsSuccess = false;

                dataResult.Message = "导入 Excel 格式不正确！请按照模版导入！";

                return dataResult;
            }

            Cache<List<SizeRuleModel>> cache = new Cache<List<SizeRuleModel>>();

            cache.SetCache("SizeRule", sizeList);

            return dataResult;
        }
    }
}
