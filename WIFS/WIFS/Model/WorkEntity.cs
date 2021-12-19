using System;
using System.Collections.Generic;

namespace WIFS
{
    public class Works
    {
        public IList<WorkEntity> workList { get; set; }
        public String accesstoken { get; set; }
    }
    public class WorkEntity
    {
        public string id { get; set; }
        public string workDate { get; set; }
        public string workTimeS { get; set; }
        public string workTimeE { get; set; }
        public int workHour { get; set; }
        public int workOver { get; set; }
        public int dinnerTime { get; set; }
        public int lunchTime { get; set; }
        public string status { get; set; }
        public string result { get; set; }
        public string overTimeReason { get; set; }


        public string token { get; set; }
    }
}