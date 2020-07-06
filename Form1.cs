using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GetWebDataDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnGet_Click(object sender, EventArgs e)
        {
            string url = @"" + txtUrl.Text.Trim().ToString();

            var list = GetTableData(url);

            StringBuilder sb = new StringBuilder();
            foreach(var i in list)
            {
                sb.AppendLine(i.ToString());
            }

            txtMsg.Text = sb.ToString();
        }

        private void btnNextPage_Click(object sender, EventArgs e)
        {

        }

        private void btnPrevPage_Click(object sender, EventArgs e)
        {

        }

        public List<string> GetTableData(string url)
        {
            List<string> list = new List<string>();
            
            WebRequest request = WebRequest.Create(url); //请求url
            WebResponse response = request.GetResponse(); //获取url数据
            StreamReader reader = null;

            string charset = "UTF-8";

            switch (charset)
            {
                case "UTF-8":
                    reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("UTF-8"));
                    break;
                case "Default":
                    reader = new StreamReader(response.GetResponseStream(), Encoding.Default);
                    break;
                default:
                    reader = new StreamReader(response.GetResponseStream(), Encoding.Default);
                    break;
            }
            string str = reader.ReadToEnd(); //将数据写入到textbox中
            reader.Close();
            reader.Dispose();
            response.Close();
            string strRegexR = @"(?<=<tr>)([\s\S]*?)(?=</tr>)"; //构造解析表格行数据的正则表达式
            string strRegexD = @"(?<=<td[^>]*>[\s]*?)([\S\s]*?)(?=</td>)"; //构造解析表格列数据的正则表达式
            Regex regexR = new Regex(strRegexR);
            MatchCollection mcR = regexR.Matches(str); //执行匹配
            bool first = true;
            foreach (Match mr in mcR)
            {
                Regex regexD = new Regex(strRegexD);
                MatchCollection mcD = regexD.Matches(mr.Groups[0].ToString()); //执行匹配
                string Mydata = "";
                //for (int i = 0; i < mcD.Count; i++)
                //{
                //    Mydata += mcD[i].Value + " ";
                //}
                if (mcD.Count == 11)
                {
                    Mydata = mcD[0].Value + "," + mcD[1].Value.Replace("  ", " ") + "," + mcD[10].Value;
                }
                list.Add(Mydata);
            }
            return list;
        }
    }
}
