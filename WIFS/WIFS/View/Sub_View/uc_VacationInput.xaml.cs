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

namespace WIFS
{
    /// <summary>
    /// uc_DashBoard.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class uc_VacationInput : UserControl
    {
        public struct workInfoList
        {
            public string workDate;
            public string workTimeS;
            public string workTimeE;
            public string workOverTIme;
        }

        ClientConfig cf = InitSetting.CConf;
        private readonly ToastViewModel _vm;

        public uc_VacationInput()
        {
            InitializeComponent();

            _vm = new ToastViewModel("1", 5, "TopRight", 10, 10);
            Unloaded += OnUnload;
        }

        void OnLoad(object sender, RoutedEventArgs e)
        {
            DataTable dataTable = new DataTable();

            // DataTable에 Column 추가
            dataTable.Columns.Add("NAME", typeof(string));
            dataTable.Columns.Add("VALUE", typeof(string));

            // DataTable에 데이터 추가
            dataTable.Rows.Add(new string[] { "직접입력", "0" });
            dataTable.Rows.Add(new string[] { "반차", "1" });
            dataTable.Rows.Add(new string[] { "일차", "2" });

            // Combo Box에 DataView 바인딩
            cbVacationKb.ItemsSource = dataTable.DefaultView;
            // Combo Box에 표시될 Column 바인딩
            cbVacationKb.DisplayMemberPath = "NAME";
            // Combo Box에 값(Value) Column 바인딩
            cbVacationKb.SelectedValuePath = "VALUE";

            cbVacationKb.SelectedIndex = 0;
            setLeftTime();
            GetList();
        }
         
        private void Btn_workTimeReg_Click(object sender, RoutedEventArgs e)
        {
            String _workDate = "";

            String _sDateTime = "";
            String _eDateTime = "";

            if (workDate.Text.Equals(""))
            {
                //MessageBox.Show("일자를 입력하여 주시기 바랍니다.");
                _vm.ShowError("일자를 입력하여 주시기 바랍니다.");
                workDate.Focus();
                return;
            }
            else
            {
                _workDate = workDate.Text.Replace("-", "");
            }

            if (vacationReason.Text.Equals(""))
            {
                //MessageBox.Show("휴가사유를 입력하여 주시기 바랍니다.");
                _vm.ShowError("휴가사유를 입력하여 주시기 바랍니다.");
                vacationReason.Focus();
                return;
            }

            //직접입력
            if (cbVacationKb.SelectedIndex == 0)
            {
                //시간입력 체크
                System.DateTime? sdt = startTime.SelectedDateTime;
                System.DateTime? edt = endTime.SelectedDateTime;

                if (sdt == null || edt == null)
                {
                    //MessageBox.Show("시간이 입력되지 않았습니다.");
                    _vm.ShowError("시간이 입력되지 않았습니다.");
                    return;
                }

                _sDateTime = sdt?.ToString("yyyyMMdd") + sdt?.TimeOfDay.ToString().Replace(":", "");
                _eDateTime = edt?.ToString("yyyyMMdd") + edt?.TimeOfDay.ToString().Replace(":", "");

            } else if (cbVacationKb.SelectedIndex == 1)
            {
                _sDateTime = _workDate + "130000";
                _eDateTime = _workDate + "180000";
            }
            else
            {
                _sDateTime = _workDate + "090000";
                _eDateTime = _workDate + "170000";
            }

            DateTime workStart = CDateTime.GetDateFrom_yyyyMMddHHmmss(_sDateTime);
            DateTime workEnd = CDateTime.GetDateFrom_yyyyMMddHHmmss(_eDateTime);

            TimeSpan TS = new TimeSpan(workEnd.Ticks - workStart.Ticks);

            //총근무시간
            int spanMinute = (int)TS.TotalMinutes;

            if(Int32.Parse(_leftTimeCheck.Content.ToString()) < spanMinute)
            {
                //MessageBox.Show("야근 총 시간보다 더 많은 시간을 입력할 수 없습니다.");
                _vm.ShowError("야근 총 시간보다 더 많은 시간을 입력할 수 없습니다.");
                return;
            }

            //데이터베이스 등록처리
            regWorkData(_workDate, _sDateTime, _eDateTime);
        }

        private async void regWorkData(String workDate, String _sDateTime, String _eDateTime)
        {
            try
            {
                MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

                ClientConfig cf = InitSetting.CConf;

                //18:00이후 인경우만 처리(저녁)
                int _dinnerTime = 0;

                //12:00이후 인경우만 처리 (점심)
                int _lunchTime = 0;

                WorkEntity we = new WorkEntity()
                {
                    id = cf.userID,
                    workDate = workDate
                };

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
                we.workHour = 0;

                //야근시간
                int overTime = spanMinute * -1;
                we.workOver = overTime;

                we.overTimeReason = vacationReason.Text;
                we.status = "0";

                //등록처리
                String result = await new CallWebApi().CallPostApiWorks("workInfoAdd", we);
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
                            await window.ShowMessageAsync("알림창", "등록었습니다. 승인 후 시간이 반영되어 집니다.");
                        }
                        break;
                    }
                }
                GetList();
                cf.overtimeReason = "";
            }
            catch { }
        }

        private async void setLeftTime()
        {
            WorkEntity we = new WorkEntity()
            {
                id = cf.userID
            };

            String result = await new CallWebApi().CallPostApiWorks("workInfoFind", we);
            Works workList = JsonConvert.DeserializeObject<Works>(result);
            int _leftTime = 0;
            //같은 날자에 등록되어 있는 자료가 있음.
            if (workList.workList.Count > 0)
            {
                var totalWorkHour = workList.workList.Where(w => w.workOver != 0)
                                                     .GroupBy(g => g.id)
                                                     .Select(s=>new
                                                     {
                                                         totalHour = s.Sum(x => x.workOver)
                                                     });


                if (totalWorkHour.Count() > 0)
                {
                    foreach(var item in totalWorkHour)
                    {
                        _leftTime = item.totalHour;
                    }                    
                }
            }

            leftTime.Content = _leftTime + "분 사용가능";
            _leftTimeCheck.Content = _leftTime;
        }

        private async void GetList()
        {
            try
            {   
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
                dt.Columns.Add("workOverTIme");
                dt.Columns.Add("status");

                var _targerWorkList = workList.workList
                                        .Where(w => w.workHour == 0
                                         ).ToList<WorkEntity>();

                for (int i = 0; i < _targerWorkList.Count; i++)
                {
                    WorkEntity item = _targerWorkList[i];

                    //근무시작시간
                    DateTime workStart = CDateTime.GetDateFrom_yyyyMMddHHmmss(item.workTimeS);
                    //근무종료시간
                    DateTime workEnd = CDateTime.GetDateFrom_yyyyMMddHHmmss(item.workTimeE);
                    //야근시간
                    int overTime = item.workOver * -1;
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
                        overTime.ToString() + "분",
                        status
                    });
                };

                WorkDataGrid.ItemsSource = dt.DefaultView;
            }
            catch { }
        }

        private void OnUnload(object sender, RoutedEventArgs e)
        {
            _vm.OnUnloaded();
        }
    }
}
