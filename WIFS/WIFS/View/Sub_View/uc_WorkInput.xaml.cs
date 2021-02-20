using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WIFS.Util;

namespace WIFS.View.Sub_View
{   
    /// <summary>
    /// uc_DashBoard.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class uc_WorkInput : UserControl
    {
        public struct workInfoList
        {
            public string workDate;
            public string workTimeS;
            public string workTimeE;
            public string workHour;
            public string workOverTIme;
        }

        ClientConfig cf = InitSetting.CConf;

        public uc_WorkInput()
        {
            InitializeComponent();
            if (cf.workStatus != null && cf.workStatus.Equals("1"))
            {
                txt_status.Content = "근무";
                txt_status.Foreground = new SolidColorBrush(Colors.Blue);

                btn_work.IsChecked = true;
            }
            else
            {
                txt_status.Content = "퇴근";
                txt_status.Foreground = new SolidColorBrush(Colors.Red);

                btn_work.IsChecked = false;
            }
            String[] weekinfo = new CommonUtil().getWeekInfo(DateTime.Now.ToString("yyyyMMdd"));
            lb_week.Content = weekinfo[0];
            String[] wi = weekinfo[1].Split('^');
            sDate.Content = wi[0];
            eDate.Content = wi[1];

            //저장된 리스트 가져오기
            GetList();

        }

        private void PreData_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //workChart.Value = 40;
            string _senderName = ((Label)sender).Name;

            int days = _senderName.Equals("nextData") ? 7 : -7;

            DateTime tagetDate = CDateTime.GetDateFromYYYYMMDD(sDate.Content.ToString()).AddDays(days);

            if (tagetDate.Year == (DateTime.Now.Year - 1))
            {
                if (sDate.Content.ToString().Substring(4, 4).Equals("0101"))
                {
                    MessageBox.Show("전년도로 이동은 불가합니다.");
                    tagetDate = CDateTime.GetDateFromYYYYMMDD(DateTime.Now.Year + "0101");
                }
                else
                {
                    tagetDate = CDateTime.GetDateFromYYYYMMDD(DateTime.Now.Year + "0101");
                }
            }
            else if (tagetDate.Year == (DateTime.Now.Year + 1))
            {
                if (eDate.Content.ToString().Substring(4, 4).Equals("1231"))
                {
                    MessageBox.Show("내년도로 이동은 불가합니다.");
                    tagetDate = CDateTime.GetDateFromYYYYMMDD(DateTime.Now.Year + "1231");
                }
                else
                {
                    tagetDate = CDateTime.GetDateFromYYYYMMDD(DateTime.Now.Year + "1231");
                }
            }

            String[] weekinfo = new CommonUtil().getWeekInfo(tagetDate.ToString("yyyyMMdd"));
            lb_week.Content = weekinfo[0];
            String[] wi = weekinfo[1].Split('^');
            sDate.Content = wi[0];
            eDate.Content = wi[1];

            GetList();
        }

        private void Btn_work_Click(object sender, RoutedEventArgs e)
        {
            ClientConfig cf = InitSetting.CConf;
            System.Windows.Controls.Primitives.ToggleButton toggleSwitch
                    = sender as System.Windows.Controls.Primitives.ToggleButton;

            if (cf.workStatus != null && cf.workStatus.Equals("1"))
            {

                MessageBoxResult result = MessageBox.Show("퇴근처리 하시겠습니까?", "근무설정", 
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    MessageBoxResult result2 = MessageBox.Show("기본 18시 퇴근으로 처리하시겠습니까?(아니오일 경우 현재시간으로 설정되어 집니다."
                                                                , "근무설정",
                                                                MessageBoxButton.YesNo, MessageBoxImage.Question);
                    //파일처리
                    string[] param = new string[1];

                    if (result2 == MessageBoxResult.Yes)
                    {
                        param[0] = DateTime.Now.ToString("yyyyMMdd") + "180000";
                    }
                    else
                    {
                        param[0] = DateTime.Now.ToString("yyyyMMddHHmm") + "00";
                    }

                    txt_status.Content = "퇴근";
                    txt_status.Foreground = new SolidColorBrush(Colors.Red);
                    cf.workStatus = "0";
                    cf.workEndLastTime = param[0];
                    InitSetting.ConfigWriteProfile(param, 2);

                    //데이터베이스 등록처리
                    DateTime sdt = CDateTime.GetDateFrom_yyyyMMddHHmmss(cf.workStartLastTime);
                    DateTime edt = CDateTime.GetDateFrom_yyyyMMddHHmmss(cf.workEndLastTime);

                    String workDate = CDateTime.GetDateFrom_yyyyMMddHHmmss(cf.workStartLastTime).ToString("yyyyMMdd");

                    String _sDateTime = sdt.ToString("yyyyMMdd") + sdt.TimeOfDay.ToString().Replace(":", "");
                    String _eDateTime = edt.ToString("yyyyMMdd") + edt.TimeOfDay.ToString().Replace(":", "");

                    int sTime = Int32.Parse(sdt.TimeOfDay.ToString().Replace(":", "").Substring(0, 4) + "00");
                    int eTime = Int32.Parse(edt.TimeOfDay.ToString().Replace(":", "").Substring(0, 4) + "00");

                    //데이터베이스 등록처리
                    regWorkData(workDate, _sDateTime, _eDateTime, sTime, eTime);

                    cheDinner.IsChecked = false;
                }
                else
                {
                    toggleSwitch.IsChecked = true;
                }
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("출근처리 하시겠습니까?", "근무설정",
                   MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    MessageBoxResult result2 = MessageBox.Show("기본 9시 출근으로 처리하시겠습니까?(아니오일 경우 현재시간으로 설정되어 집니다."
                                                                , "근무설정",
                                                                MessageBoxButton.YesNo, MessageBoxImage.Question);


                    txt_status.Content = "근무";
                    txt_status.Foreground = new SolidColorBrush(Colors.Blue);
                    cf.workStatus = "1";

                    //기본파일 등록처리
                    string[] param = new string[2];
                    if (result2 == MessageBoxResult.Yes)
                    {
                        param[0] = DateTime.Now.ToString("yyyyMMdd") + "090000";
                    }
                    else
                    {
                        param[0] = DateTime.Now.ToString("yyyyMMddHHmm") + "00";
                    }

                    param[1] = string.Empty;
                    cf.workStartLastTime = param[0];
                    cf.workEndLastTime = "";

                    InitSetting.ConfigWriteProfile(param, 1);
                }
                else
                {
                    toggleSwitch.IsChecked = false;
                }
            }
        }

        private void Btn_workTimeReg_Click(object sender, RoutedEventArgs e)
        {            
            System.DateTime? sdt = sDateTime.SelectedDateTime;
            System.DateTime? edt = eDateTime.SelectedDateTime;

            String workDate = sdt?.ToString("yyyyMMdd");

            String _sDateTime = sdt?.ToString("yyyyMMdd") + sdt?.TimeOfDay.ToString().Replace(":", "");
            String _eDateTime = edt?.ToString("yyyyMMdd") + edt?.TimeOfDay.ToString().Replace(":", "");

            int sTime = Int32.Parse(sdt?.TimeOfDay.ToString().Replace(":", ""));
            int eTime = Int32.Parse(edt?.TimeOfDay.ToString().Replace(":", ""));

            if (sdt == null || edt == null)
            {
                MessageBox.Show("날짜가 입력되지 않았습니다.");
                return;
            }
            
            //등록시간 체크
            if (!(sdt?.ToString("yyyyMMdd")).Equals(edt?.ToString("yyyyMMdd")) 
                &&                 
                    eTime != 0                
                ) {
                MessageBox.Show("동일한 날짜만 넣을 수 있습니다.");
                return;
            }
         
            if (double.Parse(_sDateTime) >= double.Parse(_eDateTime))
            {
                MessageBox.Show("출근시간이 퇴근시간보다 클 수 없습니다.");
                return;
            }
            //데이터베이스 등록처리
            regWorkData(workDate, _sDateTime, _eDateTime, sTime, eTime);
        }

        private async void regWorkData(String workDate, String _sDateTime, String _eDateTime, int sTime, int eTime)
        {
            try
            {
                MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

                ClientConfig cf = InitSetting.CConf;

                //18:00이후 인경우만 처리(저녁)
                int _dinnerTime = 0;
                if (!(bool)cheDinner.IsChecked)
                {
                    if (sTime <= 180000 && eTime > 190000)
                    {
                        _dinnerTime = 60;
                    }
                }

                //12:00이후 인경우만 처리 (점심)
                int _lunchTime = 0;
                if (sTime <= 120000 && eTime > 130000)
                {
                    _lunchTime = 60;
                }

                WorkEntity we = new WorkEntity()
                {
                    id = cf.userID,
                    workDate = workDate
                };

                String result = await new CallWebApi().CallPostApiWorks("workInfoFind", we);
                Works workList = JsonConvert.DeserializeObject<Works>(result);

                //같은 날자에 등록되어 있는 자료가 있음.
                if (workList.workList.Count > 0)
                {
                    var totalWorkHour = workList.workList
                                       .Where(w => (
                                                    (
                                                        (
                                                            double.Parse(w.workTimeS) <= double.Parse(_sDateTime)
                                                            &&
                                                            double.Parse(w.workTimeE) > double.Parse(_sDateTime)
                                                        )
                                                    ||
                                                        (
                                                            double.Parse(w.workTimeS) < double.Parse(_eDateTime)
                                                            &&
                                                            double.Parse(w.workTimeE) >= double.Parse(_eDateTime)
                                                        )
                                                     )
                                                     && w.workHour > 0
                                                   )
                                        ).ToList<WorkEntity>();

                    if (totalWorkHour.Count > 0)
                    {
                        //MessageBox.Show("등록된 자료중에 시간이 중복되는 자료가 있습니다.");
                        await window.ShowMessageAsync("알림창", "등록된 자료중에 시간이 중복되는 자료가 있습니다.");
                        return;
                    }
                }

                //저장할 항목
                we.workTimeS = _sDateTime;
                we.workTimeE = _eDateTime;
                we.dinnerTime = _dinnerTime;
                we.lunchTime = _lunchTime;

                DateTime workStart = CDateTime.GetDateFrom_yyyyMMddHHmmss(_sDateTime);
                DateTime workEnd = CDateTime.GetDateFrom_yyyyMMddHHmmss(_eDateTime);

                TimeSpan TS = new TimeSpan(workEnd.Ticks - workStart.Ticks);

                //총근무시간
                int spanMinute = (int)TS.TotalMinutes - _dinnerTime - _lunchTime;
                we.workHour = spanMinute;

                //야근시간
                int overTime = spanMinute - 480;
                we.workOver = overTime <= 0 ? 0 : overTime;

                //오버타임인데 이유가 없을 경우
                if (overTime > 0 && overTimeReason.Content.ToString().Trim().Length == 0)
                {
                    MainWindow mw = (MainWindow)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);

                    pop_inputReason popup = new pop_inputReason();
                    popup.Owner = mw;
                    mw.Opacity = 0.8;
                    popup.ShowDialog();

                    if(cf.overtimeReason.Trim() == "")
                    {
                        await window.ShowMessageAsync("알림창", "야근이유가 등록되지 않았습니다.");
                        return;
                    }
                }

                we.overTimeReason = cf.overtimeReason;

                if (overTime >0)
                {
                    we.status = "0";
                }
                else
                {
                    we.status = "1";
                }

                //등록처리
                result = await new CallWebApi().CallPostApiWorks("workInfoAdd", we);
                var rtn = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);

                foreach (KeyValuePair<string, string> item in rtn)
                {
                    if (item.Key.Equals("result") && item.Value.Equals("NG"))
                    {

                        if (window != null)
                        {
                            await window.ShowMessageAsync("알림창", "등록에 실패하였습니다. 관리자에게 문의 부탁드립니다.");
                        }
                        break;
                    }
                    else
                    {
                        if (window != null)
                        {
                            if (overTime > 0)
                            {
                                await window.ShowMessageAsync("알림창", "등록었습니다. 야근이 있는 경우 승인 후 야근시간이 반영되어 집니다.");
                            }
                            else
                            {
                                await window.ShowMessageAsync("알림창", "등록되었습니다.");
                            }
                                
                        }
                        break;
                    }
                }
                GetList();
                cf.overtimeReason = "";
            }
            catch { }
        }
      
        private async void GetList()
        {
            try
            {
                overTimeReason.Content = "";
                WorkEntity we = new WorkEntity()
                {
                    id = cf.userID
                };

                String result = await new CallWebApi().CallPostApiWorks("workInfoFind", we);
                Works workList = JsonConvert.DeserializeObject<Works>(result);

                DataTable dt;
                dt = new DataTable();

                dt.Columns.Add("workDate");
                dt.Columns.Add("workTimeS");
                dt.Columns.Add("workTimeE");
                dt.Columns.Add("workHour");
                dt.Columns.Add("workOverTIme");
                dt.Columns.Add("status");

                var _targerWorkList = workList.workList
                                        .Where(w => Int32.Parse(w.workDate) >= Int32.Parse(sDate.Content.ToString())
                                                   &&
                                                   Int32.Parse(w.workDate) <= Int32.Parse(eDate.Content.ToString())
                                                   && w.workHour > 0
                                         ).ToList<WorkEntity>();

                for (int i = 0; i < _targerWorkList.Count; i++)
                {
                    WorkEntity item = _targerWorkList[i];

                    //근무시작시간
                    DateTime workStart = CDateTime.GetDateFrom_yyyyMMddHHmmss(item.workTimeS.Trim());
                    //근무종료시간
                    DateTime workEnd = CDateTime.GetDateFrom_yyyyMMddHHmmss(item.workTimeE.Trim());
                    //총근무시간
                    int spanMinute = item.workHour;
                    //야근시간
                    int overTime = item.workOver;
                    //상태값
                    String status = "";

                    switch (item.status)
                    {
                        case "1":
                            status = "처리완료";
                            break;
                        case "2":
                            status = "반려";
                            break;
                        default:
                            status = "미처리";
                            break;
                    }

                    dt.Rows.Add(new string[]
                    {
                        CDateTime.ToYYYYMMDD(item.workDate, true),
                        Convert.ToInt32(item.workTimeS.Substring(8, 2)) + "시 " + Convert.ToInt32(item.workTimeS.Substring(10, 2)) + "분",
                        Convert.ToInt32(item.workTimeE.Substring(8, 2)) + "시 " + Convert.ToInt32(item.workTimeE.Substring(10, 2)) + "분",

                        (spanMinute/60).ToString() + "시간 " + (spanMinute%60) + "분",

                        overTime < 0?"없음": (overTime/60).ToString() + "시간 " + (overTime%60) + "분",

                        status
                    });
                };

                WorkDataGrid.ItemsSource = dt.DefaultView;
            }
            catch {}
        }
    }
}

