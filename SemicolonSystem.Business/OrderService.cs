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
            var ruleCache = new Cache<List<SizeRuleModel>>();

            var ruleDataResult = ruleCache.GetCache("SizeRule");

            if (!ruleDataResult.IsSuccess)
            {
                return new DataResult<List<MatchingResultModel>>("请导入尺寸规则");
            }

            var orderCache = new Cache<List<KeyValuePair<String, List<SizeRuleModel>>>>();

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
                    orderDataResult.Data.FirstOrDefault().Value.Select(ss => new WeightModel
                    {
                        Offset = 0,
                        Position = ss.Position,
                        PriorityLevel = 999
                    }).ToList()
                );
            }

            var matchingResultList = Matching(ruleDataResult.Data, orderDataResult.Data, weights);

            return new DataResult<List<MatchingResultModel>>(matchingResultList);
        }

        /// <summary>
        /// 匹配方法
        /// </summary>
        /// <param name="sizeRules"></param>
        /// <param name="orders"></param>
        /// <param name="weights"></param>
        /// <returns></returns>
        public static List<MatchingResultModel> Matching(List<SizeRuleModel> sizeRules, List<KeyValuePair<String, List<SizeRuleModel>>> orders, List<WeightModel> weights)
        {
            List<MatchingResultModel> matchingResultList = new List<MatchingResultModel>();

            var query = from w in weights.AsQueryable()
                        join s in sizeRules.AsQueryable() on w.Position equals s.Position
                        orderby w.PriorityLevel ascending
                        select new 
                        {
                            Model = s.Model,
                            Position = w.Position, 
                            PriorityLevel = w.PriorityLevel, 
                            MaxSize = s.Size + w.Offset, 
                            MinSize = s.Size - w.Offset  
                        };

            var weightSizeRules = query.GroupBy(g=>g.Model).Select(s=>new KeyValuePair<string, List<MatchingModel>>
            (
                s.Key,
                s.Select(se=> new MatchingModel
                {
                    Position = se.Position,
                    PriorityLevel = se.PriorityLevel,
                    MaxSize = se.MaxSize,
                    MinSize = se.MinSize
                }).ToList()
            ));

            foreach (var order in orders)
            {
                var matchingResult = new MatchingResultModel();

                matchingResult.Name = order.Key;

                var list = weightSizeRules.Where(f => order.Value.All(a => f.Value.Where(w => a.Position == w.Position).All(aa => a.Size >= aa.MinSize && a.Size <= aa.MaxSize)));

                var result = list.FirstOrDefault();

                if(!default(KeyValuePair<string, List<MatchingModel>>).Equals(result))
                {
                    matchingResult.MatchingLevel = MatchingLevel.PerfectMatch;

                    matchingResult.Model = result.Key;

                    matchingResultList.Add(matchingResult);

                    continue;
                }

                result = weightSizeRules.FirstOrDefault(f => order.Value.Any(a => f.Value.Where(w => a.Position == w.Position).All(aa => a.Size >= aa.MinSize && a.Size <= aa.MaxSize)));

                if (!default(KeyValuePair<string, List<MatchingModel>>).Equals(result))
                {
                    matchingResult.MatchingLevel = MatchingLevel.BarelyMatch;

                    matchingResult.Model = result.Key;

                    matchingResultList.Add(matchingResult);
                }
                else
                {
                    var priorityLevelMaxPosition = weights.FirstOrDefault(f => f.PriorityLevel > 0 && f.PriorityLevel == weights.Min(m => m.PriorityLevel));

                    var size = order.Value.FirstOrDefault(w => w.Position == priorityLevelMaxPosition.Position).Size;

                    result = weightSizeRules.FirstOrDefault(f => f.Value.Any(a => Math.Abs((a.MaxSize + a.MinSize) - size) == f.Value.Where(w => w.Position == priorityLevelMaxPosition.Position).Min(m => Math.Abs((m.MaxSize + m.MinSize) - size))));

                    matchingResult.MatchingLevel = MatchingLevel.ForceMatching;

                    matchingResult.Model = result.Key;

                    matchingResultList.Add(matchingResult);
                }
            }

            return matchingResultList;
        }



        /// <summary>
        /// 获取权重配置
        /// </summary>
        /// <returns></returns>
        public static DataResult<List<WeightModel>> GetWeightConfig()
        {
            List<WeightModel> list = new List<WeightModel>();

            var cache = new Cache<List<KeyValuePair<String, List<SizeRuleModel>>>>();

            var weightCache = new Cache<List<WeightModel>>();

            try
            {
                var orderDataResult = cache.GetCache("Order");

                if (!orderDataResult.IsSuccess)
                {
                    return new DataResult<List<WeightModel>>("找不到订单信息！");
                }

                var weightDataResult = weightCache.GetCache("WeightCofig");

                List<WeightModel> weights = new List<WeightModel>();

                if (weightDataResult.IsSuccess)
                {
                    weights = weightDataResult.Data;

                    var orderPositions = orderDataResult.Data.FirstOrDefault().Value;

                    if (orderPositions.Count == weights.Count && orderPositions.All(a => weights.Any(aa => aa.Position == a.Position)))
                    {
                        return new DataResult<List<WeightModel>>(weights);
                    }
                }

                list.AddRange(
                    orderDataResult.Data.FirstOrDefault().Value.Select(ss => new WeightModel
                    {
                        Offset = 0,
                        Position = ss.Position,
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

            cache.SetCache("WeightCofig", list);

            return new DataResult();
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
                            Size = Convert.ToDecimal(tab.Rows[i][j].ToString().Trim()),
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
