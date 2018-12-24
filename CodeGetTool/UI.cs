using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Timers;
using System.IO;

namespace CodeGetTool
{
    public partial class UI : Form
    {
        private System.Timers.Timer intervalTimer = new System.Timers.Timer(1000);
        private int timeCount = 0;
        private int timeWait = 0;
        private int peroid;
        private string strPeroid = "";
        private string[] openInfo = new string[3];

        private int timeCountPK10 = 0;
        private int timeWaitPK10 = 0;
        private int peroidPK10;
        private string strPeroidPK10 = "";
        private string[] openInfoPK10 = new string[3];
        

        public UI()
        {
            InitializeComponent();
            this.Text = Application.ProductName + "_v" + Application.ProductVersion + "    " + Application.CompanyName;
            intervalTimer.Elapsed += new ElapsedEventHandler(timerElapseHandler);
            intervalTimer.Enabled = true;
            intervalTimer.Start();
        }

        private void timerElapseHandler(object sender, System.Timers.ElapsedEventArgs e)
        {
            timeCount += (int)(intervalTimer.Interval / 1000);
            timeCountPK10 += (int)(intervalTimer.Interval / 1000);

            if (timeCount >= timeWait)
            {
                timeCount = 0;

                if (openInfo[1] != null)
                {
                    peroid = Convert.ToInt32(openInfo[0].Substring(0, 6) + openInfo[0].Substring(7, 3)) + 1;
                    if (DateTime.Now.Hour == 0 && openInfo[0].Substring(7, 3) == "120")
                    {
                        peroid = Convert.ToInt32(DateTime.Now.ToString("yyMMdd") + "001" );
                    }
                    strPeroid = peroid.ToString().Substring(0, 6) + "-" + peroid.ToString().Substring(6, 3);
                }

                string[] temp = Data_get.getOpenCode("cqssc", strPeroid, 1);
                
                timeWait = getWaitTime(openInfo[2]);

                if ((openInfo != null && openInfo[0] != temp[0])
                    || openInfo == null)
                {
                    openInfo = temp;

                    updateUI(openInfo);
                }

                if (timeWait > 605)
                {
                    DateTime dt = DateTime.Parse("00:00:00").AddSeconds(timeWait);
                    updateUI(new string[] { "CQSSC Newday's Open Will Wait " + dt.Hour + " Hour " + dt.Minute + " Min " + dt.Second + " Sec" });
                }
            }


            if (timeCountPK10 >= timeWaitPK10)
            {
                timeCountPK10 = 0;

                if (openInfoPK10[1] != null)
                {
                    peroidPK10 = Convert.ToInt32(openInfoPK10[0]) + 1;
                    strPeroidPK10 = peroidPK10.ToString();
                }

                string[] temp = Data_get.getOpenCode("bjpk10", strPeroidPK10, 1);
                
                timeWaitPK10 = getWaitTimePK10(openInfoPK10[2]);

                if ((openInfoPK10 != null && openInfoPK10[0] != temp[0])
                    || openInfoPK10 == null)
                {
                    openInfoPK10 = temp;

                    updateUI(openInfoPK10);
                }

                if (timeWaitPK10 > 305)
                {
                    DateTime dt = DateTime.Parse("00:00:00").AddSeconds(timeWaitPK10);
                    updateUI(new string[] { "BJPK10 Newday's Open Will Wait " + dt.Hour + " Hour " + dt.Minute + " Min " + dt.Second + " Sec" });
                }
            }
          
        }

        private int getWaitTime(string strOpenTime)
        {
            int waitSec = 0;
            string strClock_02 = "01:55:00";
            string strClock_10 = "10:00:00";
            string strClock_22 = "22:00:00";


            if (DateTime.Now > DateTime.Parse(strClock_10) && DateTime.Now < DateTime.Parse(strClock_22))
            {
                if (strOpenTime != null)
                {
                    TimeSpan waitTs = DateTime.Parse(strOpenTime).TimeOfDay + DateTime.Parse("00:10:05").TimeOfDay - DateTime.Now.TimeOfDay;
                    waitSec = (int)waitTs.TotalSeconds;
                }
                else
                {
                    waitSec = 5;
                }
            }
            else if (DateTime.Now >= DateTime.Parse(strClock_22) || DateTime.Now <= DateTime.Parse(strClock_02))
            {
                if (strOpenTime != null)
                {
                    TimeSpan waitTs = DateTime.Parse(strOpenTime).TimeOfDay + DateTime.Parse("00:05:05").TimeOfDay - DateTime.Now.TimeOfDay;
                    waitSec = (int)waitTs.TotalSeconds;
                }
                else
                {
                    waitSec = 5;
                }
            }
            else
            {
                TimeSpan waitTs = (DateTime.Parse(strClock_10) > DateTime.Now) ? (DateTime.Parse(strClock_10) - DateTime.Now) : (DateTime.Parse(strClock_10).AddHours(24) - DateTime.Now);
                waitSec = (int)waitTs.TotalSeconds;
            }

            return waitSec;
        }

        private int getWaitTimePK10(string strOpenTime)
        {
            int waitSec = 0;
            string strClock_09 = "09:07:00";
            string strClock_24 = "23:57:00";

            if (DateTime.Now >= DateTime.Parse(strClock_09) && DateTime.Now <= DateTime.Parse(strClock_24))
            {
                if (strOpenTime != null)
                {
                    TimeSpan waitTs = DateTime.Parse(strOpenTime).TimeOfDay + DateTime.Parse("00:05:05").TimeOfDay - DateTime.Now.TimeOfDay;
                    waitSec = (int)waitTs.TotalSeconds;
                }
                else
                {
                    waitSec = 5;
                }
            }
            else
            {
                TimeSpan waitTs = (DateTime.Parse(strClock_09) > DateTime.Now) ? (DateTime.Parse(strClock_09) - DateTime.Now) : (DateTime.Parse(strClock_09).AddHours(24) - DateTime.Now);
                waitSec = (int)waitTs.TotalSeconds;
            }

            return waitSec;
        }

        private void btnGet_Click(object sender, EventArgs e)
        {
            if (txtUrl.Text.Trim() == "")
                return;

            string[] strs = txtUrl.Text.Trim().Split(',');
            string[] openInfo = Data_get.getOpenCode(strs[0], (strs.Length >1 ? strs[1] : ""), 1);

            updateUI(openInfo);
        }

        public delegate void updateUiHandler(string[] strs);

        private void updateUI(string[] openInfo)
        {
            if(InvokeRequired)
            {
                try
                {
                    Invoke(new updateUiHandler(updateUI), new object[] { openInfo });
                }
                catch(Exception e)
                {
                    using (StreamWriter sw = new StreamWriter("error.txt", true, Encoding.UTF8))
                    {
                        sw.Write(DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss.fff] ") + "Invoke updateUI Exception: " + e.Message);
                    }
                }

                return;
            }

            string strtemp = "";
            try
            {
                foreach (string str in openInfo)
                {
                    if (str == null) throw new Exception();

                    strtemp += str + "  ";
                }
                strtemp = DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss] ") + strtemp + "\r\n";
            }
            catch (Exception)
            {
                strtemp = openInfo[0] + "\t get failed..  now:" + DateTime.Now.ToString() + "\r\n";
                using (StreamWriter sw = new StreamWriter("error.txt", true, Encoding.UTF8))
                {
                    sw.Write(strtemp);
                }
                return;
            }

            if (openInfo[0].Length <= 6)
            {
                using(StreamWriter sw = new StreamWriter("pk10.txt", true, Encoding.UTF8))
                {
                    sw.Write(strtemp);
                }
                strtemp = "\t\t\t\t\t\t\t\t\t" + strtemp;
            }
            else
            {
                using (StreamWriter sw = new StreamWriter("ssc.txt", true, Encoding.UTF8))
                {
                    sw.Write(strtemp);
                }
            }

            txtResponse.Text = strtemp + txtResponse.Text;

            if(txtResponse.Text.Length >= 1024*1024)
            {
                txtResponse.Clear();
            }
        }
    }
}
