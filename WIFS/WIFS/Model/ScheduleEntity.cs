using System;
using System.Collections.Generic;

namespace WIFS
{
    public class Schedule
    {
        public IList<ScheduleEntity> scheduleList { get; set; }
    }
    public class ScheduleEntity
    {
        public string _id { get; set; }
        public string id { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
        public string location { get; set; }
        public string properties { get; set; }
        public string date { get; set; }
        public string start { get; set; }
        public string end { get; set; }
    }
}