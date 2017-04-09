using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SemicolonSystem.Model.Enum;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace SemicolonSystem.Common
{
    public class ExcelHelper
    {
        #region Excel2003
        /// <summary>
        /// 将Excel文件中的数据读出到DataTable中(xls)
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static List<DataTable> ExcelToTablesForXLS(string file, int marginHader, int marginBottom, bool isSummary = false)
        {
            List<DataTable> tabs = new List<DataTable>();

            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                HSSFWorkbook hssfworkbook = new HSSFWorkbook(fs);

                var index = 0;

                while (true)
                {
                    if (index + 1 > hssfworkbook.Count) break;

                    DataTable dt = new DataTable();

                    ISheet sheet = hssfworkbook.GetSheetAt(index);

                    dt.TableName = sheet.SheetName.Trim();

                    if (isSummary)
                    {
                        if (dt.TableName != "汇总1")
                        {
                            index++;

                            continue;
                        }
                    }

                    // 表头

                    IRow header = sheet.GetRow(sheet.FirstRowNum + marginHader);

                    if (header == null)
                    {
                        throw new Exception(string.Format("表：{0}，首行为空！请检查！", dt.TableName));
                    }

                    List<int> columns = new List<int>();

                    for (int i = 0; i < header.LastCellNum; i++)
                    {
                        object obj = GetValueTypeForXLS(header.GetCell(i) as HSSFCell);

                        if (obj == null || obj.ToString() == string.Empty)
                        {
                            //if (i == 0)
                            //{
                            //    throw new Exception("导入出错！请注意导入的工作表是否有空页或者格式有误！");
                            //}

                            dt.Columns.Add(new DataColumn("Columns" + i.ToString()));
                        }
                        else
                        {
                            // TODO：此处列名重复会抛出异常！

                            dt.Columns.Add(new DataColumn(obj.ToString()));
                        }

                        columns.Add(i);
                    }

                    // 数据

                    for (int i = sheet.FirstRowNum + marginHader + 1; i <= sheet.LastRowNum; i++)
                    {
                        DataRow dr = dt.NewRow();

                        bool hasValue = false;

                        foreach (int j in columns)
                        {
                            dr[j] = GetValueTypeForXLS(sheet.GetRow(i).GetCell(j) as HSSFCell);

                            if (dr[j] != null && dr[j].ToString() != string.Empty)
                            {
                                hasValue = true;
                            }
                        }
                        if (hasValue)
                        {
                            dt.Rows.Add(dr);
                        }
                    }

                    for (int i = 1; i < marginBottom + 1; i++)
                    {
                        dt.Rows.RemoveAt(dt.Rows.Count - 1);
                    }

                    tabs.Add(dt);

                    index++;
                }
            }

            return tabs;
        }

        /// <summary>
        /// 将DataTable数据导出到Excel文件中(xls)
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="file"></param>
        public static void TableToExcelForXLS(List<DataTable> tabs, string file)
        {
            XSSFWorkbook hssfworkbook = new XSSFWorkbook();

            foreach (var dt in tabs)
            {
                ISheet sheet = hssfworkbook.CreateSheet(dt.TableName);

                //表头

                IRow row = sheet.CreateRow(0);

                var num = 0;

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (!dt.Columns[i].ColumnName.Contains("匹配程度"))
                    {
                        ICell cell = row.CreateCell(i - num);

                        if (dt.Columns[i].ColumnName.Contains("Column") || dt.Columns[i].ColumnName.Contains("属性"))
                        {
                            cell.SetCellValue(string.Empty);
                        }
                        else
                        {
                            var str = dt.Columns[i].ColumnName;

                            if (dt.Columns[i].ColumnName.Contains("汇总_"))
                            {
                                str = dt.Columns[i].ColumnName.Substring(dt.Columns[i].ColumnName.IndexOf("_") + 1);
                            }

                            cell.SetCellValue(str);
                        }
                    }
                    else
                    {
                        num++;
                    }
                }

                int rowCount = dt.Rows.Count;

                int count = dt.Rows.Count;

                // 数据

                for (int i = 0; i < count; i++)
                {
                    IRow row1 = sheet.CreateRow(i + 1);

                    var index = 0;

                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        ICell cell = null;

                        if (!dt.Columns[j].ColumnName.Contains("匹配程度"))
                        {
                            cell = row1.CreateCell(j - index);
                        }

                        string cellValue = dt.Rows[i][j].ToString();

                        if (dt.Columns[j].ColumnName.Contains("匹配程度"))
                        {
                            var styleCell = row1.Cells[j - 1 - index];

                            ICellStyle colorStyle = hssfworkbook.CreateCellStyle();

                            colorStyle.FillPattern = FillPattern.SolidForeground;

                            if (cellValue == MatchingLevel.PerfectMatch.ToString())
                                colorStyle.FillForegroundColor = 3;

                            if (cellValue == MatchingLevel.BarelyMatch.ToString())
                                colorStyle.FillForegroundColor = 5;

                            if (cellValue == MatchingLevel.ForceMatching.ToString())
                                colorStyle.FillForegroundColor = 2;

                            if (!string.IsNullOrWhiteSpace(cellValue))
                            {
                                styleCell.CellStyle = colorStyle;
                            }

                            cellValue = string.Empty;

                            index++;

                            continue;
                        }


                        if (dt.Columns[j].ColumnName.Contains("数量"))
                        {
                            ICellStyle style = hssfworkbook.CreateCellStyle();

                            style.Alignment = HorizontalAlignment.Center;

                            IFont f = hssfworkbook.CreateFont();

                            f.Boldweight = (short)FontBoldWeight.Bold;

                            style.SetFont(f);

                            cell.CellStyle = style;

                            if (dt.Columns[j].ColumnName.Contains("数量"))
                            {
                                if (dt.Rows[i][j] is DBNull)
                                {
                                    cell.SetCellValue(cellValue);
                                }
                                else
                                {
                                    cell.SetCellValue(Convert.ToInt32(dt.Rows[i][j]));
                                }
                            }
                            else
                            {
                                cell.SetCellValue(cellValue);
                            }
                        }
                        else
                        {
                            cell.SetCellValue(cellValue);
                        }
                    }

                    if (i < dt.Rows.Count) dt.Rows.Add(dt.NewRow());
                }
            }

            

            //转为字节数组
            MemoryStream stream = new MemoryStream();
            hssfworkbook.Write(stream);
            var buf = stream.ToArray();

            //保存为Excel文件
            using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write))
            {
                fs.Write(buf, 0, buf.Length);
                fs.Flush();
            }
        }

        /// <summary>
        /// 获取单元格类型(xls)
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private static object GetValueTypeForXLS(HSSFCell cell)
        {
            if (cell == null)
                return null;
            switch (cell.CellType)
            {
                case CellType.Blank: //BLANK:
                    return null;
                case CellType.Boolean: //BOOLEAN:
                    return cell.BooleanCellValue;
                case CellType.Numeric: //NUMERIC:
                    return cell.NumericCellValue;
                case CellType.String: //STRING:
                    return cell.StringCellValue;
                case CellType.Error: //ERROR:
                    return cell.ErrorCellValue;
                case CellType.Formula: //FORMULA:
                default:
                    return "=" + cell.CellFormula;
            }
        }
        #endregion

        #region Excel2007
        /// <summary>
        /// 将Excel文件中的数据读出到DataTable中(xlsx)
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static List<DataTable> ExcelToTablesForXLSX(string file, int marginHader, int marginBottom, bool isSummary = false)
        {
            List<DataTable> tabs = new List<DataTable>();

            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                XSSFWorkbook xssfworkbook = new XSSFWorkbook(fs);

                var index = 0;

                while (true)
                {
                    if (index + 1 > xssfworkbook.Count) break;

                    DataTable dt = new DataTable();

                    ISheet sheet = xssfworkbook.GetSheetAt(index);

                    dt.TableName = sheet.SheetName.Trim();

                    if (isSummary)
                    {
                        if (dt.TableName != "汇总1")
                        {
                            index++;

                            continue;
                        }
                    }

                    // 表头

                    IRow header = sheet.GetRow(sheet.FirstRowNum + marginHader);

                    if (header == null)
                    {
                        throw new Exception(string.Format("表：{0}，首行为空！请检查！", dt.TableName));
                    }

                    List<int> columns = new List<int>();

                    for (int i = 0; i < header.LastCellNum; i++)
                    {
                        object obj = GetValueTypeForXLSX(header.GetCell(i) as XSSFCell);

                        if (obj == null || obj.ToString() == string.Empty)
                        {
                            //if (i == 0)
                            //{
                            //    throw new Exception("导入出错！请注意导入的工作表是否有空页或者格式有误！");
                            //}

                            dt.Columns.Add(new DataColumn("Columns" + i.ToString()));
                        }
                        else
                        {
                            dt.Columns.Add(new DataColumn(obj.ToString()));
                        }
                         
                        columns.Add(i);
                    }
                    //数据
                    for (int i = sheet.FirstRowNum + marginHader + 1; i <= sheet.LastRowNum - marginBottom; i++)
                    {
                        DataRow dr = dt.NewRow();

                        bool hasValue = false;

                        foreach (int j in columns)
                        {
                            dr[j] = GetValueTypeForXLSX(sheet.GetRow(i).GetCell(j) as XSSFCell);

                            if (dr[j] != null && dr[j].ToString() != string.Empty)
                            {
                                hasValue = true;
                            }
                        }
                        if (hasValue)
                        {
                            dt.Rows.Add(dr);
                        }
                    }

                    for (int i = 1; i < marginBottom + 1; i++)
                    {
                        dt.Rows.RemoveAt(dt.Rows.Count - 1);
                    }

                    tabs.Add(dt);

                    index++;
                }
            }
            return tabs;
        }

        /// <summary>
        /// 将DataTable数据导出到Excel文件中(xlsx)
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="file"></param>
        public static void TableToExcelForXLSX(DataTable dt, string file)
        {
            XSSFWorkbook xssfworkbook = new XSSFWorkbook();
            ISheet sheet = xssfworkbook.CreateSheet("Test");

            //表头
            IRow row = sheet.CreateRow(0);
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                ICell cell = row.CreateCell(i);
                cell.SetCellValue(dt.Columns[i].ColumnName);
            }

            //数据
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                IRow row1 = sheet.CreateRow(i + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    ICell cell = row1.CreateCell(j);
                    cell.SetCellValue(dt.Rows[i][j].ToString());
                }
            }

            //转为字节数组
            MemoryStream stream = new MemoryStream();
            xssfworkbook.Write(stream);
            var buf = stream.ToArray();

            //保存为Excel文件
            using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write))
            {
                fs.Write(buf, 0, buf.Length);
                fs.Flush();
            }
        }

        /// <summary>
        /// 获取单元格类型(xlsx)
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private static object GetValueTypeForXLSX(XSSFCell cell)
        {
            if (cell == null)
                return null;
            switch (cell.CellType)
            {
                case CellType.Blank: //BLANK:
                    return null;
                case CellType.Boolean: //BOOLEAN:
                    return cell.BooleanCellValue;
                case CellType.Numeric: //NUMERIC:
                    return cell.NumericCellValue;
                case CellType.String: //STRING:
                    return cell.StringCellValue;
                case CellType.Error: //ERROR:
                    return cell.ErrorCellValue;
                case CellType.Formula: //FORMULA:
                default:
                    return "=" + cell.CellFormula;
            }
        }
        #endregion

        /// <summary>
        /// 获取 Excel 表格
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static List<DataTable> GetDataTable(string filepath, int marginHader = 0, int marginBottom = 0, bool isSummary = false)
        {
            var dt = new List<DataTable>();

            if (filepath.Last() == 's')
            {
                dt = ExcelToTablesForXLS(filepath, marginHader, marginBottom, isSummary);
            }
            else
            {
                dt = ExcelToTablesForXLSX(filepath, marginHader, marginBottom, isSummary);
            }
            return dt;
        }
    }
}