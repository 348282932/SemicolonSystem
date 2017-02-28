using SemicolonSystem.Common;
using SemicolonSystem.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SemicolonSystem.Business
{
    public class OrderService
    {
        /// <summary>
        /// 获取权重配置
        /// </summary>
        /// <returns></returns>
        public static DataResult<List<WeightModel>> GetWeightConfig()
        {
            List<WeightModel> list = new List<WeightModel>();

            var cache = new Cache<List<KeyValuePair<String, List<SizeRuleModel>>>>();

            try
            {
                var cacheData = cache.GetCache("Order");

                if (cacheData.Count == 0)
                {
                    return new DataResult<List<WeightModel>>("找不到订单信息！");
                }

                list.AddRange(
                    cacheData.FirstOrDefault().Value.Select(ss => new WeightModel
                    {
                        Offset = 0,
                        Position = ss.Position,
                        PriorityLevel = 0
                    }).ToList()
                );
                
            }
            catch (Exception ex)
            {
                return new DataResult<List<WeightModel>>(ex.Message);
            }

            return new DataResult<List<WeightModel>>(list);
        }

        /// <summary>
        /// 导入订单 Excel
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static DataResult ImportOrderExcel(string fileName)
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

            var orderList = new List<KeyValuePair<String, List<SizeRuleModel>>>();

            try
            {
                for (int i = 0; i < tab.Rows.Count; i++)
                {
                    List<SizeRuleModel> ruleList = new List<SizeRuleModel>();

                    for (int j = 1; j < tab.Columns.Count; j++)
                    {
                        ruleList.Add(new SizeRuleModel
                        {
                            Position = tab.Columns[j].ColumnName,
                            Size = Convert.ToDecimal(tab.Rows[0][j].ToString().Trim()),
                            Model = String.Empty
                        });

                    }

                    orderList.Add(new KeyValuePair<String, List<SizeRuleModel>>(tab.Rows[i][0].ToString(), ruleList));
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

            var cache = new Cache<List<KeyValuePair<String, List<SizeRuleModel>>>>();

            cache.SetCache("Order", orderList);

            return dataResult;
        }
    }
}
