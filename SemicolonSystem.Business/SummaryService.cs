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
                tab = ExcelHelper.GetDataTable(fileName).FirstOrDefault(f => f.TableName == "汇总1");

                for (int i = 0; i < tab.Rows.Count; i++)
                {
                    if (!(tab.Rows[i]["号型(男)"] is DBNull))
                    {
                        summaryList.Add(new SummaryModel
                        {
                            Model = tab.Rows[i]["号型(男)"].ToString().Trim(),
                            Count = Convert.ToInt32(tab.Rows[i]["数量(男)"].ToString().Trim()),
                            Sex = "男"
                        });
                    }

                    if (!(tab.Rows[i]["号型(女)"] is DBNull))
                    {
                        summaryList.Add(new SummaryModel
                        {
                            Model = tab.Rows[i]["号型(女)"].ToString().Trim(),
                            Count = Convert.ToInt32(tab.Rows[i]["数量(女)"].ToString().Trim()),
                            Sex = "女"
                        });
                    }

                    if (!(tab.Rows[i]["号型"] is DBNull))
                    {
                        summaryList.Add(new SummaryModel
                        {
                            Model = tab.Rows[i]["号型"].ToString().Trim(),
                            Count = Convert.ToInt32(tab.Rows[i]["数量"].ToString().Trim()),
                        });
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
