using SemicolonSystem.Common;
using SemicolonSystem.Model;
using SemicolonSystem.Model.Enum;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

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
            var ruleCache = new Cache<List<SizeRuleModel>>();

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

            List<MatchingResultModel> list = new List<MatchingResultModel>();

            List<Task> taskArr = new List<Task>();

            foreach (var rule in ruleDataResult.Data)
            {
                try
                {
                    taskArr.Add(Task.Factory.StartNew(() =>
                    {
                        var resultList = new List<MatchingSizeRuleModel>();

                        var weightCache = new Cache<List<WeightModel>>();

                        var weightDataResult = weightCache.GetCache("WeightCofig_" + rule.Name);

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
                                    OffsetLeft = 0,
                                    OffsetRight = 0,
                                    Position = ss.Position,
                                    PriorityLevel = 999
                                }).ToList()
                            );
                        }

                        var taskArray = new List<Task>();

                        foreach (var item in orderDataResult.Data)
                        {
                            taskArray.Add(Task.Factory.StartNew(() =>
                            {
                                var matchingResult = new MatchingSizeRuleModel();

                                matchingResult.SheetName = item.SheetName;

                                List<OrderRow> orderRows = new List<OrderRow>();

                                if (!string.IsNullOrWhiteSpace(rule.Sex))
                                {
                                    orderRows = item.OrderRows.Where(w => w.Sex == rule.Sex).ToList();
                                }
                                else
                                {
                                    orderRows = item.OrderRows;
                                }

                                matchingResult.MatchingRows = Matching(rule.Items, orderRows, weights, item.SheetName);

                                resultList.Add(matchingResult);
                            }));
                        }

                        Task.WaitAll(taskArray.ToArray());

                        list.Add(new MatchingResultModel
                        {
                            SizeRuleName = rule.Name,
                            Sex = rule.Sex,
                            Items = resultList
                        });
                    }));
                }
                catch (Exception ex)
                {
                    return new DataResult<List<MatchingResultModel>>(ex.Message);
                }
            }

            Task.WaitAll(taskArr.ToArray());

            return new DataResult<List<MatchingResultModel>>(list);
        }

        /// <summary>
        /// 获取权重配置
        /// </summary>
        /// <returns></returns>
        public static DataResult<List<WeightModel>> GetWeightConfig(string sizeRuleName)
        {
            List<WeightModel> list = new List<WeightModel>();

            var cache = new Cache<List<OrderModel>>();

            var weightCache = new Cache<List<WeightModel>>();

            var ruleCache = new Cache<List<SizeRuleModel>>();

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

                var ruleData = ruletDataResult.Data.FirstOrDefault(f => f.Name == sizeRuleName);

                var orderPositions = orderDataResult.Data.FirstOrDefault().OrderRows.FirstOrDefault().SizeRules.Where(w => ruleData.Items.Any(a => a.Position == w.Position)).ToList();

                var weightDataResult = weightCache.GetCache("WeightCofig_" + sizeRuleName);

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
                        OffsetLeft = 0,
                        OffsetRight = 0,
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
        public static DataResult ImportWeightCofig(List<WeightModel> list, string sizeRuleName)
        {
            var cache = new Cache<List<WeightModel>>();

            return cache.SetCache("WeightCofig_" + sizeRuleName, list);
        }

        /// <summary>
        /// 导入订单 Excel
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static DataResult ImportOrderExcel(string fileName, int marginHader, int marginBottom)
        {
            var ruleCache = new Cache<List<SizeRuleModel>>();

            var ruleDataResult = ruleCache.GetCache("SizeRule");

            if (!ruleDataResult.IsSuccess)
            {
                return new DataResult<List<MatchingSizeRuleModel>>("请导入尺寸规则");
            }

            var ruleItems = new List<SizeRuleItemModel>();

            foreach (var item in ruleDataResult.Data)
            {
                ruleItems.AddRange(item.Items);
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
                    List<string> propertyList = new List<string> { "", "", "" };

                    for (int i = 0; i < 3; i++)
                    {
                        if (tab.Columns[i].ColumnName != "姓名")
                        {
                            propertyList[i] = tab.Columns[i].ColumnName;
                        }
                        else
                        {
                            break;
                        }
                    }

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
                            else if (!ruleItems.Any(a => a.Position == tab.Columns[j].ColumnName.Trim()))
                            {
                                continue;
                            }
                            else
                            {
                                var sizeStr = tab.Rows[i][j].ToString().Trim();

                                var size = 0m;

                                if (!string.IsNullOrWhiteSpace(sizeStr))
                                {
                                    size = Convert.ToDecimal(sizeStr);
                                }

                                ruleList.Add(new SizeRuleItemModel
                                {
                                    Position = tab.Columns[j].ColumnName.Trim(),
                                    Size = size,
                                    Model = string.Empty
                                });
                            }
                        }

                        var orderRow = new OrderRow();

                        orderRow.Name = tab.Rows[i]["姓名"].ToString();
                        orderRow.SizeRules = ruleList;
                        orderRow.Sex = sex;
                        orderRow.Property1 = string.IsNullOrEmpty(propertyList[0]) ? string.Empty : tab.Rows[i][propertyList[0]].ToString();
                        orderRow.Property2 = string.IsNullOrEmpty(propertyList[1]) ? string.Empty : tab.Rows[i][propertyList[1]].ToString();
                        orderRow.Property3 = string.IsNullOrEmpty(propertyList[2]) ? string.Empty : tab.Rows[i][propertyList[2]].ToString();

                        orderRows.Add(orderRow);
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
                            MinSize = s.Size - w.OffsetLeft,
                            MaxSize = s.Size + w.OffsetRight
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

                matchingResult.Property1 = order.Property1;

                matchingResult.Property2 = order.Property2;

                matchingResult.Property3 = order.Property3;

                matchingResult.SheetName = sheetName;

                var list = weightSizeRules.Where(f => order.SizeRules.Where(w=>w.Size != 0).All(a => f.Value.Where(w => a.Position == w.Position).All(aa => a.Size >= aa.MinSize && a.Size <= aa.MaxSize)));

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

                list = weightSizeRules.Where(f => order.SizeRules.Where(w => w.Size != 0).Any(a => f.Value.Where(w => a.Position == w.Position).All(aa => a.Size >= aa.MinSize && a.Size <= aa.MaxSize)));

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

                if (size == 0) continue;

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

            if (arrTow == null)
            {
                throw new Exception(string.Format("发现客户:{0},无有效量体数据！", order.Name));
            }

            return arrTow[0];
        }
    }
}
