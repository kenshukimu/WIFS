using System;
using System.Collections.Generic;

namespace WIFS
{
    public class Weeks
    {
        public IList<WeekEntity> weekInfo { get; set; }
    }
    public class WeekEntity 
    {
        public string year { get; set; }
        public string weekNo { get; set; }
        public string sdate { get; set; }
        public string edate { get; set; }

        public string token { get; set; }
    }
}