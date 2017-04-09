using SemicolonSystem.Common;
using SemicolonSystem.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SemicolonSystem.Business
{
    public class SummaryService
    {
        public static DataResult<List<SummaryModel>> SummaryResultTable(string fileName)
        {
            DataTable tab = new DataTable();

            List<SummaryModel> summaryList = new List<SummaryModel>();

            try
            {
                tab = ExcelHelper.GetDataTable(fileName, isSummary: true).FirstOrDefault(f => f.TableName == "汇总1");

                var man = new List<string>();

                var woman = new List<string>();

                var sum = new List<string>();

                for (int i = 0; i < tab.Columns.Count; i++)
                {
                    var str = tab.Columns[i].ToString().Trim();

                    if (str.Contains("号型(男)")) man.Add(str.Remove(0, 6));

                    if (str.Contains("号型(女)")) woman.Add(str.Remove(0, 6));

                    if (str.Contains("号型_")) sum.Add(str.Remove(0, 3));

                }

                for (int i = 0; i < tab.Rows.Count; i++)
                {
                    foreach (var item in man)
                    {
                        if (!(tab.Rows[i]["号型(男)_" + item] is DBNull))
                        {
                            summaryList.Add(new SummaryModel
                            {
                                Model = tab.Rows[i]["号型(男)_" + item].ToString().Trim(),
                                Count = Convert.ToInt32(tab.Rows[i]["数量(男)_" + item].ToString().Trim()),
                                Sex = "男",
                                RuleName = item
                            });
                        }
                    }

                    foreach (var item in woman)
                    {
                        if (!(tab.Rows[i]["号型(女)_" + item] is DBNull))
                        {
                            summaryList.Add(new SummaryModel
                            {
                                Model = tab.Rows[i]["号型(女)_" + item].ToString().Trim(),
                                Count = Convert.ToInt32(tab.Rows[i]["数量(女)_" + item].ToString().Trim()),
                                Sex = "女",
                                RuleName = item
                            });
                        }
                    }
                    foreach (var item in sum)
                    {
                        if (!(tab.Rows[i]["号型_" + item] is DBNull))
                        {
                            summaryList.Add(new SummaryModel
                            {
                                Model = tab.Rows[i]["号型_"+ item].ToString().Trim(),
                                Count = Convert.ToInt32(tab.Rows[i]["数量_"+ item].ToString().Trim()),
                                RuleName = item
                            });
                        }
                    }
                }

            }
            catch (FormatException)
            {
                return new DataResult<List<SummaryModel>>(string.Format("表“{0}”导入的数据不符合规范", fileName.Substring(fileName.LastIndexOf("\\") + 1)));
            }
            catch (Exception ex)
            {
                return new DataResult<List<SummaryModel>>(ex.Message);
            }

            return new DataResult<List<SummaryModel>>(summaryList);
        } 
    }
}
