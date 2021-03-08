using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIFS
{
    public class CommonUtil
    {
        ClientConfig cf = InitSetting.CConf;
        public String[] getWeekInfo(String date)
        {
            String[] _viewText = new string[2];
            int _today = 0;
            if (date == null || date.Equals("") || date.Length !=8)
            {
                _today = Int32.Parse(DateTime.Now.ToString("yyyyMMdd"));
            }
            else
            {
                _today = Int32.Parse(date);
            }            

            var _todayWeek = cf.weekList.Where(w => Int32.Parse(w.sdate) <= _today && _today <= Int32.Parse(w.edate)).ToList<WeekEntity>();

            if (_todayWeek.Count > 0)
            {
                //1월 1째주(1월1일~1월2일)

                _viewText[0] = _today.ToString().Substring(0, 4)
                                   + "년 "
                                   + _todayWeek[0].weekNo
                                   + "째주 ("
                                   + CDateTime.ToYYYYMMDD(_todayWeek[0].sdate.Substring(4, 4), true)
                                   + " ~ " + CDateTime.ToYYYYMMDD(_todayWeek[0].edate.Substring(4, 4), true)
                                   + ")";

                _viewText[1] = _todayWeek[0].sdate + "^" + _todayWeek[0].edate;
            }
            return _viewText;
        }

        public String CreateDirectory(String l_sDirectoryName)
        {
            DirectoryInfo l_dDirInfo = new DirectoryInfo(l_sDirectoryName);
            if (l_dDirInfo.Exists == false)
            {
                Directory.CreateDirectory(l_sDirectoryName);
            }
            return l_sDirectoryName;
        }
    }
}
