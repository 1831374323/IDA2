using System.IO;
using System.Data;
using System.Text;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
// using System.Runtime.Serialization.Formatters.Binary;

namespace SY_Tool
{
    public static class SY_CSV
    {
        #region example
        //string path;
        // void Start()
        // {
        // path = Application.dataPath + "/Data/Resources/Test.CSV";

        // DataTable result = new DataTable();

        // string[] tableHead;

        // result = Read(path, out tableHead);
        // result = Read(path, out tableHead);

        // Debug.Log(result.Rows[0][0]);
        // Debug.Log(result.Rows[0][1]);
        // Debug.Log(result.Rows[1][0]);

        // Write("changed!2", path, 0, 1);
        // Write("changed!2", path, 1, 0);

        // result = Read(path, out tableHead);

        // Debug.Log(result.Rows[0][1]);
        // Debug.Log(result.Rows[1][0]);
        // }
        #endregion

        /// <summary>
        /// 有表头,using system.data
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static DataTable Read(string path, out string[] _tableHead)
        {

            DataTable dt = new DataTable();

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                {
                    //记录每次读取的一行记录
                    string strLine = "";
                    //记录每行记录中的各字段内容
                    string[] aryLine = null;
                    string[] tableHead = null;
                    //标示列数
                    int columnCout = 0;
                    bool isFirst = true;

                    //逐行读取CSV中的数据
                    while ((strLine = sr.ReadLine()) != null)
                    {
                        if (isFirst)
                        {
                            tableHead = strLine.Split(',');
                            isFirst = false;
                            //列数=表头列数
                            columnCout = tableHead.Length;

                            for (int j = 0; j < columnCout; j++)
                            {
                                DataColumn dc = new DataColumn(tableHead[j]);
                                dt.Columns.Add(dc);
                            }
                        }
                        else
                        {
                            aryLine = strLine.Split(',');
                            DataRow dr = dt.NewRow();
                            for (int j = 0; j < columnCout; j++)
                            {
                                dr[j] = aryLine[j];
                            }
                            dt.Rows.Add(dr);
                        }
                    }
                    if (aryLine != null && aryLine.Length > 0)
                    {
                        dt.DefaultView.Sort = tableHead[0] + " " + "asc";
                    }

                    sr.Close();
                    fs.Close();

                    _tableHead = tableHead;

                    return dt;
                }
            }



            // Encoding utf = Encoding.GetEncoding("UTF-8");

            // string AllDatas=File.ReadAllText(path, utf);

            // string[] rowData = AllDatas.Split('\n');

            // for (int i = 0; i<rowData.Length; i++)
            // {

            //     string[] lineData = rowData[i].Split(',');

            //     for (int j = 0; j < lineData.Length;j++){
            //         dt[i][j] = lineData[j];
            //     }

            // }

        }

        /// <summary>
        /// 路径从resources开始
        /// </summary>
        /// <param name="path"></param>
        /// <param name="hasTableHead"></param>
        /// <returns></returns>
        public static DataTable ReadFromResources(string path, bool hasTableHead = true)
        {

            DataTable dt = new DataTable();
            TextAsset textAsset = Resources.Load<TextAsset>(path);      //加载csv

            //按行分隔
            string[] strRow = null;
            strRow = textAsset.text.Split('\n');

            string[] tableHead = null;
            bool isFirst = true;
            int columnCout = 0;         //列的数量

            //如果没有表头，则直接插入列
            if (!hasTableHead)
            {
                isFirst = false;
                columnCout = strRow[0].Split(',').Length;
                for (int j = 0; j < columnCout; j++)
                {
                    DataColumn dc = new DataColumn();
                    dt.Columns.Add(dc);
                }
            }
            //逐行处理
            foreach (var row in strRow)
            {

                if (isFirst)        //处理表头
                {
                    // Debug.Log(row);
                    tableHead = row.Split(',');
                    isFirst = false;
                    columnCout = tableHead.Length;      //列数=表头列数


                    for (int j = 0; j < columnCout; j++)        //逐列插入
                    {
                        DataColumn dc = new DataColumn(tableHead[j]);

                        dt.Columns.Add(dc);
                    }
                }
                else        //处理数据
                {
                    string[] str = null;
                    str = row.Split(',');
                    DataRow dr = dt.NewRow();


                    try
                    {
                        for (int j = 0; j < columnCout; j++)        //逐个插入数据
                        {

                            dr[j] = str[j];
                            //Debug.Log(dt.Columns[j].ColumnName + ":" + dr[j]+" 序号: "+j);
                        }
                    }
                    catch (System.IndexOutOfRangeException)         //最后会多一行空数据，直接弹出
                    {
                        break;
                    }

                    dt.Rows.Add(dr);
                }
            }

            //Debug.Log(dt.Columns[12].ColumnName);
            //Debug.Log(dt.Rows[3]["maxTarget\n"]);
            return dt;
        }

        /// <summary>
        /// 改写CSV数据 path = Application.dataPath + "/Test.CSV";using system.data
        /// </summary>
        /// <param name="targetValue"></param>
        /// <param name="path"></param>
        /// <param name="rowNum"></param>
        /// <param name="columnNum"></param>
        public static void Write(string targetValue, string path, int rowNum, int columnNum)
        {
            //如果文件不存在，创建文件
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
            }

            string[] tableHead;
            FileInfo fi = new FileInfo(path);
            DataTable dt = Read(path, out tableHead);

            //如果列数不足，补齐列
            for (int j = dt.Columns.Count; j <= columnNum; j++)
            {
                DataColumn dc = new DataColumn();
                dt.Columns.Add(dc);
            }

            //如果行数不足，补齐行
            for (int j = dt.Rows.Count; j <= rowNum; j++)
            {
                DataRow dr = dt.NewRow();
                dt.Rows.Add(dr);
            }

            //修改值
            dt.Rows[rowNum][columnNum] = targetValue;

            //插入新表
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
                {
                    string data = "";

                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        data += dt.Columns[j].ColumnName.ToString();
                        if (j < dt.Columns.Count - 1)
                        {
                            data += ',';
                        }
                    }
                    sw.WriteLine(data);

                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        data = "";
                        for (var k = 0; k < dt.Columns.Count; k++)
                        {
                            string str = dt.Rows[j][k].ToString();
                            data += str;
                            if (k < dt.Columns.Count - 1)
                            {
                                data += ',';
                            }
                        }
                        sw.WriteLine(data);
                    }
                    sw.Close();
                    fs.Close();
                }
            }
        }

    }
}
