using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeGetTool
{
    class Data_get
    {
        /// <summary>
        /// 获取开奖号
        /// </summary>
        /// <param name="peroid">起始期号</param>
        /// <param name="count">期数</param>
        /// <returns>开奖号字节序列</returns>
        public static string[] getOpenCode(string strCode, string strPeroid, int count)
        {
            string[] openInfo = null;

            if(1 == count)
            {
                openInfo = getOpenCodeFrom_opencai_net(strCode, strPeroid);
            }
            else
            {
                openInfo = getOpenCodeFrom_bwlc_gov_cn(strCode, strPeroid, count);
            }

            return openInfo;
        }

        /// <summary>
        /// 获取开奖号 -- 从 开彩网API data.opencai.net 获取
        /// </summary>
        /// <param name="strCode"> 彩票代码 </param>
        /// <param name="strPeroid"> 期号 </param>
        /// <returns>
        ///     string[0] -- peroid
        ///     string[1] -- opencode
        ///     string[3] -- time
        /// </returns>
        /* 例如：北京PK10[code:bjpk10]、 重庆时时彩[code:cqssc]
         * 
         * http://data.opencai.net/ob/?ob=bjpk10
         * {"bjpk10":{"load":false,"_":[
            {"p":"670710","r":"!z,!%,!~,!&,!\",!a,!@,@!,!:,!#","t":"2018-03-13 11:37:34"},  03,02,07,09,06,05,01,10,08,04
            {"p":"670709","r":"!&,!@,!%,!a,!z,!:,!#,@!,!~,!\"","t":"2018-03-13 11:32:40"},
            {"p":"670708","r":"!%,@!,!z,!:,!\",!@,!a,!~,!#,!&","t":"2018-03-13 11:27:33"}],
            "ball":["r","b","g"],
            "__":"\"~:&!@%z#a"}} 
                   6789012345
         * http://data.opencai.net/ob/?ob=cqssc
         * {"cqssc":{"load":false,"_":[
            {"p":"180313-031","r":"%,~,&,%,~","t":"2018-03-13 11:11:00"},  0,6,4,0,6
            {"p":"180313-030","r":"!,#,@,@,:","t":"2018-03-13 11:01:00"},  7,8,2,2,5
            {"p":"180313-029","r":"&,#,#,:,%","t":"2018-03-13 10:50:54"}],
            "ball":["r","b","g"],
            "__":"~!#\"%;@a&:"}};
                  6789 012345
         * */
        private static string[] getOpenCodeFrom_opencai_net(string strCode, string strPeroid)
        {
            string[] strRet = new string[3];
            string strUrl = "http://data.opencai.net/ob/?ob" + (strCode == "" ? "" : "=" + strCode);
            string response = Http.DoGet(strUrl, "", null);

            strRet[0] = strPeroid;

            if (response != null && response.IndexOf(strCode) >= 0)
            {
                string strTmp = response.Substring(response.IndexOf(strCode));

                if(strPeroid == "")
                {
                    strPeroid = strTmp.Substring(strTmp.IndexOf("p\"") + 4, (strTmp.IndexOf("r\"") - strTmp.IndexOf("p\"") - 7)); 
                }

                if(strTmp.IndexOf(strPeroid) >= 0)
                {
                    strTmp = response.Substring(response.IndexOf(strPeroid) - 6).Replace("\\", "");

                    string strMap = strTmp.Substring(strTmp.IndexOf("__") + 5, (strTmp.IndexOf("}}") - strTmp.IndexOf("__") - 6));
                    char[] charMap = (strMap.Substring(4) + strMap.Substring(0,4)).ToCharArray();

                    string strEncode = strTmp.Substring(strTmp.IndexOf("r\"") + 4, (strTmp.IndexOf("t\"") - strTmp.IndexOf("r\"") - 7));
                    string[] encode = strEncode.Split(',');

                    string strDecode = "";
                    for (int i = 0; i < encode.Length; i++ )
                    {
                        char[] ch = encode[i].ToCharArray();
                        for (int j = 0; j < charMap.Length; j++ )
                        {
                            if (ch[0] == charMap[j])
                                ch[0] = Convert.ToChar(j + '0');

                            if (ch.Length > 1 && ch[1] == charMap[j])
                                ch[1] = Convert.ToChar(j + '0');
                        }
                        strDecode += ch[0] + ( ch.Length > 1 ? ch[1].ToString() : "") + (i < encode.Length - 1 ? "," : "");
                    }

                    string strTime = strTmp.Substring(strTmp.IndexOf("t\"") + 4, 19);

                    strRet[0] = strPeroid;
                    strRet[1] = strDecode;
                    strRet[2] = strTime;
                    //strRet[3] = strMap;
                    //strRet[4] = response;
                }
            }

            return strRet;
        }



        /// <summary>
        /// 获取开奖号 -- 从 北京福彩网 www.bwlc.gov.cn 获取
        /// </summary>
        /// <param name="strCode"> 彩票代码 </param>
        /// <param name="strPeroid"> 起始期号 </param>
        /// <param name="count">期数</param>
        /// <returns>
        ///     string[0] -- peroid
        ///     string[1] -- opencode
        ///     string[3] -- time
        /// </returns>
        private static string[] getOpenCodeFrom_bwlc_gov_cn(string strCode, string strPeroid, int count)
        {
            string[] opencode = null;

            return opencode;
        }
    }
}
