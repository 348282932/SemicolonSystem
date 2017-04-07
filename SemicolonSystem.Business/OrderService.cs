using SemicolonSystem.Common;
using SemicolonSystem.Model;
using SemicolonSystem.Model.Enum;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SemicolonSystem.Business
{
    public class OrderService
    {
        /// <summary>
        /// 获取匹配结果集
        /// </summary>
        /// <returns></returns>
        public static DataResult<List<MatchingResultModel>> GetMatchingResult()
        {
            var resultList = new List<MatchingResultModel>();

            var ruleCache = new Cache<List<SizeRuleItemModel>>();

            var ruleDataResult = ruleCache.GetCache("SizeRule");

            if (!ruleDataResult.IsSuccess)
            {
                return new DataResult<List<MatchingResultModel>>("请导入尺寸规则");
            }

            var orderCache = new Cache<List<OrderModel>>();

            var orderDataResult = orderCache.GetCache("Order");

            if (!orderDataResult.IsSuccess)
            {
                return new DataResult<List<MatchingResultModel>>("请导入订单信息");
            }

            var weightCache = new Cache<List<WeightModel>>();

            var weightDataResult = weightCache.GetCache("WeightCofig");

            List<WeightModel> weights = new List<WeightModel>();

            if (weightDataResult.IsSuccess)
            {
                weights = weightDataResult.Data;
            }
            else
            {
                weights.AddRange(
                    orderDataResult.Data.FirstOrDefault().OrderRows.FirstOrDefault().SizeRules.Select(ss => new WeightModel
                    {
                        Offset = 0,
                        Position = ss.Position,
                        PriorityLevel = 999
                    }).ToList()
                );
            }

            foreach (var item in orderDataResult.Data)
            {

                var matchingResult = new MatchingResultModel();

                matchingResult.SheetName = item.SheetName;

                matchingResult.MatchingRows = Matching(ruleDataResult.Data, item.OrderRows, weights, item.SheetName);

                resultList.Add(matchingResult);

            }

            return new DataResult<List<MatchingResultModel>>(resultList);
        }

        /// <summary>
        /// 获取权重配置
        /// </summary>
        /// <returns></returns>
        public static DataResult<List<WeightModel>> GetWeightConfig()
        {
            List<WeightModel> list = new List<WeightModel>();

            var cache = new Cache<List<OrderModel>>();

            var weightCache = new Cache<List<WeightModel>>();

            var ruleCache = new Cache<List<SizeRuleItemModel>>();

            try
            {
                var orderDataResult = cache.GetCache("Order");

                if (!orderDataResult.IsSuccess)
                {
                    return new DataResult<List<WeightModel>>("请导入订单信息！");
                }

                var ruletDataResult = ruleCache.GetCache("SizeRule");

                if (!ruletDataResult.IsSuccess)
                {
                    return new DataResult<List<WeightModel>>("请导入规则信息！");
                }

                var orderPositions = orderDataResult.Data.FirstOrDefault().OrderRows.FirstOrDefault().SizeRules.Where(w => ruletDataResult.Data.Any(a => a.Position == w.Position)).ToList();

                var weightDataResult = weightCache.GetCache("WeightCofig");

                List<WeightModel> weights = new List<WeightModel>();

                if (weightDataResult.IsSuccess)
                {
                    weights = weightDataResult.Data;

                    if (orderPositions.Count == weights.Count && orderPositions.All(a => weights.Any(aa => aa.Position == a.Position)))
                    {
                        return new DataResult<List<WeightModel>>(weights);
                    }
                }

                list.AddRange(
                    orderPositions.Select(s => new WeightModel
                    {
                        Offset = 0,
                        Position = s.Position,
                        PriorityLevel = 999
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
        /// 导入权重配置
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static DataResult ImportWeightCofig(List<WeightModel> list)
        {
            var cache = new Cache<List<WeightModel>>();

            return cache.SetCache("WeightCofig", list);
        }

        /// <summary>
        /// 导入订单 Excel
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static DataResult ImportOrderExcel(string fileName, int marginHader, int marginBottom)
        {
            var ruleCache = new Cache<List<SizeRuleItemModel>>();

            var ruleDataResult = ruleCache.GetCache("SizeRule");

            if (!ruleDataResult.IsSuccess)
            {
                return new DataResult<List<MatchingResultModel>>("请导入尺寸规则");
            }

            DataResult dataResult = new DataResult();

            List<DataTable> tabs = new List<DataTable>();

            try
            {
                tabs = ExcelHelper.GetDataTable(fileName, marginHader, marginBottom);
            }
            catch (DuplicateNameException ex)
            {
                dataResult.IsSuccess = false;

                dataResult.Message = "表头的列名重复，" + ex.Message;

                return dataResult;
            }
            catch (Exception ex)
            {
                dataResult.IsSuccess = false;

                dataResult.Message = ex.Message;

                return dataResult;
            }

            if (tabs == null || tabs.Count == 0)
            {
                return new DataResult("请按模版导入 Excel");
            }

            var orderList = new List<OrderModel>();

            foreach (var tab in tabs)
            {
                if (tab.Rows.Count == 0 || tab.Columns.Count == 0)
                {
                    return new DataResult("请按模版导入 Excel");
                }

                var orderModel = new OrderModel();

                orderModel.SheetName = tab.TableName;

                var orderRows = new List<OrderRow>();

                try
                {
                    for (int i = 0; i < tab.Rows.Count; i++)
                    {
                        List<SizeRuleItemModel> ruleList = new List<SizeRuleItemModel>();

                        var sex = string.Empty;

                        for (int j = 1; j < tab.Columns.Count; j++)
                        {
                            if (tab.Columns[j].ColumnName.Trim() == "性别")
                            {
                                sex = tab.Rows[i][j].ToString().Trim();

                                if (string.IsNullOrWhiteSpace(sex) || (!sex.Contains("男") && !sex.Contains("女")) || (sex.Contains("男") && sex.Contains("女")))
                                {
                                    dataResult.IsSuccess = false;

                                    dataResult.Message += string.Format("{2}表“{0}”性别存在格式错误！错误位置：第{1}行", tab.TableName, i + 2 + marginHader, Environment.NewLine);
                                }
                            }
                            else if (!ruleDataResult.Data.Any(a => a.Position == tab.Columns[j].ColumnName.Trim()))
                            {
                                continue;
                            }
                            else
                            {
                                ruleList.Add(new SizeRuleItemModel
                                {
                                    Position = tab.Columns[j].ColumnName.Trim(),
                                    Size = Convert.ToDecimal(tab.Rows[i][j].ToString().Trim()),
                                    Model = string.Empty
                                });
                            }
                        }

                        orderRows.Add(new OrderRow { Name = tab.Rows[i][0].ToString(), SizeRules = ruleList, Sex = sex });
                    }

                    orderModel.OrderRows = orderRows;

                    orderList.Add(orderModel);
                }
                catch (FormatException)
                {
                    dataResult.IsSuccess = false;

                    dataResult.Message = string.Format("表“{0}”导入的尺寸数据不符合规范", tab.TableName);

                    return dataResult;
                }
                catch (Exception)
                {
                    dataResult.IsSuccess = false;

                    dataResult.Message = "导入 Excel 格式不正确！请按照模版导入！";

                    return dataResult;
                }
            }

            //if (tab.Columns[0].ToString().Trim() != "姓名")
            //{
            //    return new DataResult("请按模版导入 Excel");
            //}

            if (!dataResult.IsSuccess)
            {
                return dataResult;
            }

            var cache = new Cache<List<OrderModel>>();

            return cache.SetCache("Order", orderList);
        }

        /// <summary>
        /// 匹配方法
        /// </summary>
        /// <param name="sizeRules"></param>
        /// <param name="orders"></param>
        /// <param name="weights"></param>
        /// <returns></returns> //List<KeyValuePair<string, List<SizeRuleModel>>>
        private static List<MatchingRowModel> Matching(List<SizeRuleItemModel> sizeRules, List<OrderRow> orders, List<WeightModel> weights, string sheetName)
        {
            List<MatchingRowModel> matchingResultList = new List<MatchingRowModel>();

            var query = from w in weights.AsQueryable()
                        join s in sizeRules.AsQueryable() on w.Position equals s.Position
                        where w.PriorityLevel > 0
                        orderby w.PriorityLevel ascending
                        select new
                        {
                            Model = s.Model,
                            Position = w.Position,
                            PriorityLevel = w.PriorityLevel,
                            MaxSize = s.Size + w.Offset,
                            MinSize = s.Size - w.Offset
                        };

            var weightSizeRules = query.GroupBy(g => g.Model).Select(s => new KeyValuePair<string, List<MatchingModel>>
              (
                  s.Key,
                  s.Select(se => new MatchingModel
                  {
                      Position = se.Position,
                      PriorityLevel = se.PriorityLevel,
                      MaxSize = se.MaxSize,
                      MinSize = se.MinSize
                  }).ToList()
              ));

            var weightSizePositions = query.GroupBy(g => new KeyValuePair<string, short>(g.Position, g.PriorityLevel)).Select(s => new KeyValuePair<KeyValuePair<string, short>, List<MatchingModel>>
                (
                    s.Key,
                    s.Select(se => new MatchingModel
                    {
                        Model = se.Model,
                        MaxSize = se.MaxSize,
                        MinSize = se.MinSize
                    }).ToList()
                )).OrderBy(o => o.Key.Value);

            foreach (var order in orders)
            {
                var matchingResult = new MatchingRowModel();

                matchingResult.Name = order.Name;

                matchingResult.Sex = order.Sex;

                matchingResult.SheetName = sheetName;

                var list = weightSizeRules.Where(f => order.SizeRules.All(a => f.Value.Where(w => a.Position == w.Position).All(aa => a.Size >= aa.MinSize && a.Size <= aa.MaxSize)));

                var count = list.Count();

                var result = list.FirstOrDefault();

                if (count > 0)
                {
                    matchingResult.MatchingLevel = MatchingLevel.PerfectMatch;

                    if (default(KeyValuePair<string, List<MatchingModel>>).Equals(result) && count == 1)
                    {
                        matchingResult.Model = result.Key;
                    }
                    else
                    {
                        matchingResult.Model = ModelMatching(order, weightSizePositions, true);
                    }

                    matchingResultList.Add(matchingResult);

                    continue;
                }

                list = weightSizeRules.Where(f => order.SizeRules.Any(a => f.Value.Where(w => a.Position == w.Position).All(aa => a.Size >= aa.MinSize && a.Size <= aa.MaxSize)));

                count = list.Count();

                result = list.FirstOrDefault();

                if (count > 0)
                {
                    matchingResult.MatchingLevel = MatchingLevel.BarelyMatch;

                    if (default(KeyValuePair<string, List<MatchingModel>>).Equals(result) && count == 1)
                    {
                        matchingResult.Model = result.Key;
                    }
                    else
                    {
                        matchingResult.Model = ModelMatching(order, weightSizePositions, true);
                    }

                    matchingResultList.Add(matchingResult);

                    continue;
                }
                else
                {
                    matchingResult.MatchingLevel = MatchingLevel.ForceMatching;

                    matchingResult.Model = ModelMatching(order, weightSizePositions, false);

                    matchingResultList.Add(matchingResult);
                }
            }

            return matchingResultList;
        }

        /// <summary>
        /// 按优先级进行偏移量匹配
        /// </summary>
        /// <param name="order"></param>
        /// <param name="weightSizePositions"></param>
        /// <returns></returns>
        private static string ModelMatching(OrderRow order,IOrderedQueryable<KeyValuePair<KeyValuePair<string, short>, List<MatchingModel>>> weightSizePositions, bool isMatched)
        {
            Dictionary<KeyValuePair<string, short>, Dictionary<string, decimal>> modelOffsetList = new Dictionary<KeyValuePair<string, short>, Dictionary<string, decimal>>();

            foreach (var item in weightSizePositions)
            {
                var size = order.SizeRules.FirstOrDefault(f => f.Position == item.Key.Key).Size;

                if (isMatched)
                {
                    modelOffsetList.Add(item.Key, item.Value.Select(s => new KeyValuePair<string, decimal>
                    (
                       s.Model,
                       Math.Abs((s.MaxSize + s.MinSize) - size * 2)
                    )).ToDictionary(k => k.Key, v => v.Value)
                    );
                }
                else
                {
                    modelOffsetList.Add(item.Key, item.Value.Select(s => new KeyValuePair<string, decimal>
                            (
                               s.Model,
                               Math.Min(Math.Abs(s.MaxSize - size), Math.Abs(s.MinSize - size))
                            )).ToDictionary(k => k.Key, v => v.Value)
                            );
                }
                
            }

            string[] arrTow = null;

            foreach (var item in modelOffsetList)
            {
                KeyValuePair<string, decimal>[] arr = null;

                if (arrTow == null)
                {
                    arr = item.Value.Where(w => w.Value == item.Value.Min(m => m.Value)).ToArray();

                    arrTow = arr.Select(s => s.Key).ToArray();
                }
                else
                {
                    var temp = item.Value.Where(w => arrTow.Any(a => a == w.Key));

                    arr = temp.Where(w => w.Value == temp.Min(m => m.Value)).ToArray();

                    arrTow = arr.Select(s => s.Key).ToArray();
                }

                if (arrTow.Length == 1)
                {
                    return arrTow[0];
                }
            }

            return arrTow[0];
        }
    }
}
