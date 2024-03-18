using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Transform2JSON 的摘要说明
/// </summary>
namespace Engineering.Common
{
    public class Transform2JSON
    {
        public Transform2JSON()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        /// <summary>
        /// 将DataTable中的数据转换成json格式的数据
        /// </summary>
        /// <param name="dt">包含数据的DataTable</param>
        /// <returns>返回{total:xx,rows:[{},{}]}形式的数据</returns>
        public static string ToJSONString(DataTable dt, int count)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{");    //</br>
            strBuilder.Append("\"total\":" + count.ToString() + ",");
            strBuilder.Append("\"rows\":[");
            int num = dt.Rows.Count;
            if (num == 0)
            {
                return strBuilder.Append("]}").ToString();
            }

            string col = string.Empty;
            int colNum = dt.Columns.Count;

            for (int i = 0; i < colNum; i++)
            {
                col += "\"" + dt.Columns[i].ColumnName + "\":{" + i + "},";
            }
            col = col.Substring(0, col.LastIndexOf(","));

            object[] obj = new object[colNum];
            for (int m = 0; m < num; m++)
            {
                strBuilder.Append("{");
                for (int n = 0; n < colNum; n++)
                {
                    string tp = dt.Rows[m][n].GetType().ToString();
                    obj[n] = null;
                    if (tp == "System.Decimal")
                    {
                        obj[n] = dt.Rows[m][n].ToString();
                    }
                    else if (tp == "System.DateTime")
                    {
                        obj[n] = "\"" + Convert.ToDateTime(dt.Rows[m][n]).ToString("yyyy-MM-dd HH:mm:ss") + "\"";

                    }
                    else
                    {
                        obj[n] = "\"" + dt.Rows[m][n].ToString().Replace("'", "\\'").Replace("\"", "\\\"").Replace(":", "_58_").Replace("：", "_41914_") + "\"";
                    }
                }

                string str = string.Format(col, obj);
                strBuilder.Append(str);
                strBuilder.Append("},");
            }

            strBuilder.Append("]");
            strBuilder.Append("}");

            return strBuilder.ToString().Remove(strBuilder.ToString().LastIndexOf(","), 1);
        }

        /// <summary>
        /// 将DataTable中的数据转换成json格式的数据(OperationSchedule手术日程专用)
        /// </summary>
        /// <param name="dt">包含数据的DataTable</param>
        /// <returns>返回{total:xx,rows:[{},{}]}形式的数据</returns>
        public static string OSToJSONString(DataTable dt)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("({");    //</br>
            int num = dt.Rows.Count;
            strBuilder.Append("total:" + num.ToString());
            strBuilder.Append(",rows:[");

            if (num == 0)
            {
                return strBuilder.Append("]})").ToString();
            }

            string col = string.Empty;
            int colNum = dt.Columns.Count;

            for (int i = 0; i < colNum; i++)
            {
                col += dt.Columns[i].ColumnName + ":{" + i + "},";
            }
            col = col.Substring(0, col.LastIndexOf(","));

            for (int m = 0; m < num; m++)
            {
                object[] obj = new object[colNum];
                strBuilder.Append("{");
                for (int n = 0; n < colNum; n++)
                {
                    string tp = dt.Rows[m][n].GetType().ToString();
                    obj[n] = null;
                    if (tp == "System.Decimal")
                    {
                        obj[n] = dt.Rows[m][n].ToString();
                    }
                    if (tp == "System.DateTime")
                    {
                        obj[n] = "'" + Convert.ToDateTime(dt.Rows[m][n]).ToShortDateString() + "'";
                    }
                    obj[n] = "'" + dt.Rows[m][n].ToString() + "'";
                }
                string str = string.Format(col, obj);
                strBuilder.Append(str);
                strBuilder.Append("},");
            }

            strBuilder.Append("]");
            strBuilder.Append("})");

            return strBuilder.ToString().Remove(strBuilder.ToString().LastIndexOf(","), 1);
        }


        /// <summary>
        /// 将DataTable中的数据转换成json格式的数据(仅限于测试用)
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string TestToJSONString(DataTable dt)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{</br>");    //</br>
            int num = dt.Rows.Count;
            strBuilder.Append("total:" + num.ToString() + ",</br>");
            strBuilder.Append("rows:[</br>");

            if (num == 0)
            {
                return strBuilder.Append("]</br>}").ToString();
            }

            string col = string.Empty;
            int colNum = dt.Columns.Count;

            for (int i = 0; i < colNum; i++)
            {
                col += dt.Columns[i].ColumnName + ":{" + i + "},";
            }
            col = col.Substring(0, col.LastIndexOf(","));
            object[] obj = new object[colNum];
            for (int m = 0; m < num; m++)
            {

                strBuilder.Append("{</br>");
                for (int n = 0; n < colNum; n++)
                {
                    string tp = dt.Rows[m][n].GetType().ToString();
                    obj[n] = null;
                    if (tp == "System.Decimal")
                    {
                        obj[n] = dt.Rows[m][n].ToString() + "</br>";
                    }
                    if (tp == "System.DateTime")
                    {
                        obj[n] = "'" + Convert.ToDateTime(dt.Rows[m][n]).ToShortDateString() + "'</br>";
                    }
                    obj[n] = "'" + dt.Rows[m][n].ToString() + "'</br>";
                }

                string str = string.Format(col, obj);
                strBuilder.Append(str);
                strBuilder.Append("},</br>");
            }

            strBuilder.Append("</br>]");
            strBuilder.Append("</br>}");

            return strBuilder.ToString().Remove(strBuilder.ToString().LastIndexOf(","), 1);
        }

        #region ... 转换JSON字符串到List ...
        public static List<Hashtable> JSONConvert(string json)
        {
            json.Split('{')[1].Replace("}", "").Split(',')[0].Split(':')[1].Replace("\"", "");
            //根据'{'左大括号得到字符数组
            string[] jsonRow = json.Split('{');

            List<string[]> lst = new List<string[]>();
            foreach (string str in jsonRow)
            {
                if (!string.IsNullOrEmpty(str.Trim()))
                {
                    lst.Add(str.Replace("}", "").Split(','));
                }
            }

            List<Hashtable> htList = new List<Hashtable>();
            string[] temp = null;
            foreach (string[] str in lst)
            {
                Hashtable ht = new Hashtable();
                for (int i = 0; i < str.Length; i++)
                {
                    if (!string.IsNullOrEmpty(str[i].Trim()))
                    {
                        temp = str[i].Split(':');

                        ht.Add(temp[0].Replace("\"", "").Trim(), temp[1].Replace("\"", "").Replace("_58_", ":").Replace("_41914_", "：").Trim());
                    }
                }
                htList.Add(ht);
            }
            return htList;
        }


        #endregion


        #region ... 生成ID,日期+1000以内的随机数
        public static string CreateID()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(DateTime.Now.ToString().Replace(":", "").Replace("-", "").Replace(" ", ""));
            sb.Append(Guid.NewGuid().ToString().Substring(0, 4).ToUpper());
            return sb.ToString();
        }
        #endregion


        public static DataTable Convert2DataTable(List<Hashtable> list)
        {
            DataTable dt = new DataTable();
            if (list.Count == 0)
                return dt;

            foreach (string name in list[0].Keys)
                dt.Columns.Add(name);

            foreach (Hashtable item in list)
                dt.Rows.Add(new ArrayList(item.Values).ToArray());

            return dt;
        }
    }

}