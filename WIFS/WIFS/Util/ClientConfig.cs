using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIFS
{
    public class ClientConfig
    {  
        public string serverIP;
        public string userID;
        public string userPass;
        public string autoLogin;
        public string autoAlarm;

        public string userName;
        public string empID;

        public string workStatus;

        public string workStartLastTime;
        public string workEndLastTime;

        public string overtimeReason;

        public string dept;

        public IList<WeekEntity> weekList { get; set; }
        public IList<AppointmentBusinessObject> scheduleList { get; set; }
    }
}
