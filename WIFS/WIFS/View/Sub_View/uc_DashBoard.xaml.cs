using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WIFS
{
    /// <summary>
    /// uc_DashBoard.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class uc_DashBoard : UserControl
    {
        ClientConfig cf = InitSetting.CConf;
        private readonly ToastViewModel _vm;

        public uc_DashBoard()
        {
            InitializeComponent();

            _vm = new ToastViewModel("1", 5, "TopRight", 10, 10);
            Unloaded += OnUnload;

            String[] weekinfo = new CommonUtil().getWeekInfo(DateTime.Now.ToString("yyyyMMdd"));
            lb_week.Content = weekinfo[0];
            String[] wi = weekinfo[1].Split('^');
            sDate.Content = wi[0];
            eDate.Content = wi[1];
        }

        void OnLoad(object sender, RoutedEventArgs e)
        {
            //DashBoard에 표시 할 내용 가져오기
            showDashBoard();
        }

        private void PreData_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //workChart.Value = 40;
            string _senderName = ((Label)sender).Name;

            int days = _senderName.Equals("nextData") ? 7 : -7;

            DateTime tagetDate = CDateTime.GetDateFromYYYYMMDD(sDate.Content.ToString()).AddDays(days);

            //if (tagetDate.Year == (DateTime.Now.Year-1))
            //{
            //    if(sDate.Content.ToString().Substring(4,4).Equals("0101"))
            //    {
            //        //MessageBox.Show("전년도로 이동은 불가합니다.");
            //        _vm.ShowError("전년도로 이동은 불가합니다.");
            //        tagetDate = CDateTime.GetDateFromYYYYMMDD(DateTime.Now.Year + "0101");
            //    }
            //    else
            //    {
            //        tagetDate = CDateTime.GetDateFromYYYYMMDD(DateTime.Now.Year + "0101");
            //    }               
            //}else if (tagetDate.Year == (DateTime.Now.Year + 1))
            //{
            //    if (eDate.Content.ToString().Substring(4, 4).Equals("1231"))
            //    {
            //        //MessageBox.Show("내년도로 이동은 불가합니다.");
            //        _vm.ShowError("내년도로 이동은 불가합니다.");
            //        tagetDate = CDateTime.GetDateFromYYYYMMDD(DateTime.Now.Year + "1231");
            //    }
            //    else
            //    {
            //        tagetDate = CDateTime.GetDateFromYYYYMMDD(DateTime.Now.Year + "1231");
            //    }
            //}
            try
            {
                String[] weekinfo = new CommonUtil().getWeekInfo(tagetDate.ToString("yyyyMMdd"));

                if(weekinfo[0] == null || weekinfo[1] == null)
                {
                    _vm.ShowError("연도데이터가 등록되지 않아 이동이 불가합니다.");
                    return;
                }

                lb_week.Content = weekinfo[0];
                String[] wi = weekinfo[1].Split('^');
                sDate.Content = wi[0];
                eDate.Content = wi[1];

                showDashBoard();
            }catch (Exception ex)
            {
                _vm.ShowError("연도데이터가 등록되지 않아 이동이 불가합니다.");
            }
        }

        private async void showDashBoard()
        {
            try
            {
                WorkEntity we = new WorkEntity()
                {
                    id = cf.userID
                };

                String result = await new CallWebApi().CallPostApiWorks("workInfoFind", we);
                Works workList = JsonConvert.DeserializeObject<Works>(result);

                IEnumerable<WorkEntity> totalOverTimeSum = workList.workList
                                                                   .Where(w=>w.status.Equals("1"))
                                                                   .GroupBy(g => g.id)
                                                        .Select(s => new WorkEntity
                                                        {
                                                            workOver = s.Sum(x => x.workOver)
                                                        });

                if (totalOverTimeSum != null)
                {
                    foreach (WorkEntity _sums in totalOverTimeSum)
                    {
                        lb_totalOverTime.Content = (_sums.workOver / 60).ToString() + "시간 " + (_sums.workOver % 60) + "분";
                    }
                }

                var _targerWorkList = workList.workList
                                        .Where(w => Int32.Parse(w.workDate) >= Int32.Parse(sDate.Content.ToString())
                                                   &&
                                                   Int32.Parse(w.workDate) <= Int32.Parse(eDate.Content.ToString())
                                                   &&
                                                   w.status.Equals("1")
                                         ).ToList<WorkEntity>();


                int _totalWorkTime = 0;
                int _totlaOverTime = 0;
                for (int i = 0; i < _targerWorkList.Count; i++)
                {
                    WorkEntity item = _targerWorkList[i];

                    _totalWorkTime += item.workHour;
                    _totlaOverTime += item.workOver;
                }

                _totlaOverTime = 720 - _totlaOverTime;
                leftOverTime.Content = (_totlaOverTime / 60).ToString() + "시간 " + (_totlaOverTime % 60) + "분";

                lb_workTime.Content = "누적근무시간 : " + (_totalWorkTime / 60).ToString() + "시간 " + (_totalWorkTime % 60) + "분";

                workChart.Value = Math.Round(((double)_totalWorkTime / (52 * 60)) * 100);

                ScheduleEntity se = new ScheduleEntity()
                {
                    id = cf.userID,
                    date = CDateTime.GetDateTimeFormat(DateTime.Now, "yyyymmdd")
                };

                var result3 = Task.Run(() => new CallWebApi().CallPostApiSchedules("scheduleFind", se));

                Schedule scheduleList = JsonConvert.DeserializeObject<Schedule>(await result3);
                cf.scheduleList = new List<AppointmentBusinessObject>();

                foreach (ScheduleEntity _item in scheduleList.scheduleList)
                {
                    AppointmentBusinessObject newObject = new AppointmentBusinessObject();
                    newObject.Subject = _item.subject;
                    newObject.Start = CDateTime.GetDateFrom_yyyyMMddHHmmss(_item.start);
                    newObject.End = CDateTime.GetDateFrom_yyyyMMddHHmmss(_item.end);
                    newObject.Body = _item.body;

                    cf.scheduleList.Add(newObject);
                }
            }
            catch(Exception ex)
            {
                //MessageBox.Show("관리자에게 문의 부탁드립니다.");
                _vm.ShowError("관리자에게 문의 부탁드립니다.");
            }
        }

        private void OnUnload(object sender, RoutedEventArgs e)
        {
            _vm.OnUnloaded();
        }
    }
}
