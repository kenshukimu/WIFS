using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ToastNotifications.Core;

namespace WIFS
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        //HttpClient client = new HttpClient();
        uc_DashBoard _uc_dashboard = null;
        uc_WorkInput _uc_workInput = null;
        uc_Setting _uc_setting = null;
        uc_VacationInput _uc_vacationInput = null;
        uc_Calendar _uc_calendar = null;

        private readonly ToastViewModel _vm;
        private readonly ToastViewModel _vm2;
        
        public System.Windows.Forms.NotifyIcon notify;

        private int _moveFg = 0;

        public MainWindow()
        {
            InitializeComponent();

            _vm = new ToastViewModel("0", 15, "", 5, 5);
            _vm2 = new ToastViewModel("0", 15, "BottomCenter", 1, 
               System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height / 2);

            Unloaded += OnUnload;

            string strVersionText = Assembly.GetExecutingAssembly().FullName
            .Split(',')[1]
            .Trim()
            .Split('=')[1];

            version.Content = "버전정보 : " + strVersionText;

            InitSetting.Instance.Init();

            AutoLogin();            
        }

        private void OnUnload(object sender, RoutedEventArgs e)
        {
            _vm.OnUnloaded();
        }


        //타이머 처리
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            ClientConfig cf = InitSetting.CConf;

            if (cf.workStatus == null) cf.workStatus = "0";

            string format = "yyyy년 MM월 dd일";
            string _strDate = DateTime.Now.ToString(format);

            format = " HH시 mm분 ss초";
            string _strTime = DateTime.Now.ToString(format);

            labDate.Content = _strDate;
            labTime.Content = _strTime;
            var sb = new StringBuilder();

            //일정알림처리 (근무시일경우만)
            if (cf.autoAlarm.Equals("0") && cf.workStatus.Equals("1"))
            {
                if (cf.scheduleList != null)
                {
                    foreach (var _item in cf.scheduleList)
                    {
                        TimeSpan ts = _item.Start - DateTime.Now.AddMinutes(5);

                        if (ts.TotalSeconds < 0 && ts.TotalSeconds > -1)
                        {
                            sb.Append(CDateTime.GetDateTimeFormat(_item.Start, "yyyy-mm-dd hh:nn:ss"))
                              .Append(" ")
                              .Append(_item.Subject)
                              .Append("[")
                              .Append(_item.Body)
                              .Append("] ")
                              .Append(" 5분전입니다");


                            _vm2.ShowWarning(sb.ToString());
                        }
                    }
                }
            }

            //09시가 되었는데 근무가 처리 안되었을 경우 (월~금)        
            //60초동안 클릭을 안할 경우 출근안함으로 처리
            //cf.status => 1: 근무 , 0:퇴근
            if (!(DateTime.Now.DayOfWeek.Equals(DayOfWeek.Saturday) && DateTime.Now.DayOfWeek.Equals(DayOfWeek.Sunday)))
            {
                if (DateTime.Now.ToString("HHmmss").Equals("090000")
                       && cf.workStatus.Equals("0"))
                {   
                    var result = System.Windows.Forms.AutoClosingMessageBox.Show(
                                text: "9시입니다. 근무를 시작하시겠습니까?",
                                caption: "근무설정",
                                timeout: 60000,
                                buttons: System.Windows.Forms.MessageBoxButtons.YesNo,
                                defaultResult: System.Windows.Forms.DialogResult.No);
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        //기본파일 등록처리
                        string[] param = new string[2];
                        param[0] = DateTime.Now.ToString("yyyyMMdd") + "090000";
                        param[1] = string.Empty;
                        cf.workStartLastTime = param[0];
                        cf.workEndLastTime = "";

                        cf.workStatus = "1";

                        InitSetting.ConfigWriteProfile(param, 1);

                        _uc_dashboard = new uc_DashBoard();
                        uc_Class.Uc_Link(Contents_Border, _uc_dashboard);
                        btn_DashBoard.Focus();
                    }
                }
                //18시가 되었을 때 퇴근알림 처리
                else if (DateTime.Now.ToString("HHmmss").Equals("180000")
                           && cf.workStatus.Equals("1"))
                {
                    var result = System.Windows.Forms.AutoClosingMessageBox.Show(
                                text: "18시입니다. 퇴근 하시겠습니까?",
                                caption: "근무설정",
                                timeout: 60000,
                                buttons: System.Windows.Forms.MessageBoxButtons.YesNo,
                                defaultResult: System.Windows.Forms.DialogResult.No);
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        string[] param = new string[1];
                        param[0] = DateTime.Now.ToString("yyyyMMdd") + "180000";

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
                    }
                }
                else if (DateTime.Now.ToString("HHmmss").Equals("093000") || DateTime.Now.ToString("HHmmss").Equals("133000")
                       && cf.workStatus.Equals("1"))
                {
                    _vm.ShowInformation("오전 9시30분에서 11시30분 / 오후 13시30분 ~ 오후 17시30분은 업무 집중시간입니다.");
                }

                //if(cf.workStatus.Equals("1") && Int32.Parse(DateTime.Now.ToString("HHmmss")) > 183000)
                //{
                //    if(DateTime.Now.ToString("mmss").Equals("0000")) {
                //        _vm.ShowInformation("현재 연장근무중입니다. 업무 빨리 하시고 퇴근하세요.");
                //    }
                //}
            }
        }


        public static readonly DependencyProperty TestCollProperty = DependencyProperty.Register(
                    "TestColl", typeof(ObservableCollection<string>), typeof(MainWindow), new PropertyMetadata(new ObservableCollection<string>()));

        public ObservableCollection<string> TestColl
        {
            get { return (ObservableCollection<string>)GetValue(TestCollProperty); }
            set { SetValue(TestCollProperty, value); }
        }

        private void ButtonBase_OnClick(object sender, EventArgs e)
        {
            var w = new MetroWindow();
            w.GlowBrush = Brushes.Gray;
            w.BorderThickness = new Thickness(1);
            w.Title = "Modal";
            w.Width = 300;
            w.Height = 200;
            w.Owner = this;
            w.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            w.ShowDialog();
        }

        public static readonly DependencyProperty ToggleFullScreenProperty =
            DependencyProperty.Register("ToggleFullScreen",
                typeof(bool),
                typeof(MainWindow),
                new PropertyMetadata(default(bool), ToggleFullScreenPropertyChangedCallback));

        private static void ToggleFullScreenPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var metroWindow = (MetroWindow)dependencyObject;
            if (e.OldValue != e.NewValue)
            {
                var fullScreen = (bool)e.NewValue;
                if (fullScreen)
                {
                    metroWindow.UseNoneWindowStyle = true;
                    metroWindow.IgnoreTaskbarOnMaximize = true;
                    metroWindow.ShowMinButton = false;
                    metroWindow.ShowMaxRestoreButton = false;
                    metroWindow.ShowCloseButton = false;
                    metroWindow.WindowState = WindowState.Maximized;
                }
                else
                {
                    metroWindow.UseNoneWindowStyle = false;
                    metroWindow.ShowTitleBar = true; // <-- this must be set to true
                    metroWindow.IgnoreTaskbarOnMaximize = false;
                    metroWindow.ShowMinButton = true;
                    metroWindow.ShowMaxRestoreButton = true;
                    metroWindow.ShowCloseButton = true;
                    metroWindow.WindowState = WindowState.Normal;
                }
            }
        }

        public bool ToggleFullScreen
        {
            get { return (bool)GetValue(ToggleFullScreenProperty); }
            set { SetValue(ToggleFullScreenProperty, value); }
        }

        private void Btn_DataInput_Click(object sender, RoutedEventArgs e)
        {
            if(_uc_workInput == null)  _uc_workInput = new uc_WorkInput();
            uc_Class.Uc_Link(Contents_Border, _uc_workInput);
        }

        private void Btn_DashBoard_Click(object sender, RoutedEventArgs e)
        {
            if (_uc_dashboard == null) _uc_dashboard = new uc_DashBoard();
            uc_Class.Uc_Link(Contents_Border, _uc_dashboard);
        }

        private void Btn_vacationInput_Click(object sender, RoutedEventArgs e)
        {
            if (_uc_vacationInput == null) _uc_vacationInput = new uc_VacationInput();
            uc_Class.Uc_Link(Contents_Border, _uc_vacationInput);
        }

        private void Btn_Setting_Click(object sender, RoutedEventArgs e)
        {
            if (_uc_setting == null) _uc_setting = new uc_Setting();
            uc_Class.Uc_Link(Contents_Border, _uc_setting);
        }

        private void Btn_alarm_Click(object sender, RoutedEventArgs e)
        {
            if (_uc_calendar == null) _uc_calendar = new uc_Calendar();
            uc_Class.Uc_Link(Contents_Border, _uc_calendar);
        }

        private async void AutoLogin()
        {
            try
            {
                ClientConfig cf = InitSetting.CConf;

                //자동로그인 설정
                if (cf.autoLogin.Equals("1"))
                {
                    UserEntity ue = new UserEntity()
                    {
                        id = cf.userID
                    };

                    var result = Task.Run(() => new CallWebApi().CallPostApiUsers("userFind", ue));
                    Users userList = JsonConvert.DeserializeObject<Users>(await result);

                    //로그인 ID가 틀릴 경우 로그인창을 띄운다.
                    if (userList == null || userList.userList.Count != 1)
                    {
                        Login popup = new Login();
                        popup.ShowDialog();
                        lbl_userInfo.Content = cf.userName;
                    }
                    else
                    {
                        //비밀번호가 틀릴경우 로그인창을 띄운다.
                        if (!cf.userPass.Equals(userList.userList[0].password))
                        {
                            Login popup = new Login();
                            popup.ShowDialog();
                        }
                        else
                        {
                            //정상처리
                            cf.userName = userList.userList[0].name;
                            cf.dept = userList.userList[0].depart;

                            lbl_userInfo.Content = userList.userList[0].name;
                            this.Title = cf.dept;
                        }
                    }
                }
                else
                {
                    //자동로그인이 아닐경우 로그인창을 띄운다.
                    Login popup = new Login();
                    popup.ShowDialog();

                    lbl_userInfo.Content = cf.userName;
                }                

                //올해년도 기준
                WeekEntity we = new WeekEntity()
                {
                    //year = DateTime.Now.Year.ToString()
                };

                var result2 = Task.Run(() => new CallWebApi().CallPostApiWeeks("getWeekInfo", we));
                var rtn = JsonConvert.DeserializeObject<Weeks>(await result2);

                cf.weekList = rtn.weekInfo;

                //타이머처리
                System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
                dispatcherTimer.Tick += DispatcherTimer_Tick;
                dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                dispatcherTimer.Start();


                //마우스 이동이 없을 경우 처리(1분씩 체크)
                //System.Windows.Threading.DispatcherTimer CheckIdleTimer = new System.Windows.Threading.DispatcherTimer();
                //CheckIdleTimer.Tick += CheckIdleTimer_Tick;
                //CheckIdleTimer.Interval = new TimeSpan(0, 1, 0);               //CheckIdleTimer.Start();

                _uc_dashboard = new uc_DashBoard();
                uc_Class.Uc_Link(Contents_Border, _uc_dashboard);

                showNotify();

                //전에 처리하지 않은 데이터 확인
                checkData();
            }
            catch(Exception ex)
            {
                //MessageBox.Show("관리자에게 문의 부탁드립니다.");
            }
        }

        private void showNotify()
        {
            System.Windows.Forms.ContextMenu menu = new System.Windows.Forms.ContextMenu();

            // 아이콘 설정부분
            notify = new System.Windows.Forms.NotifyIcon();
            notify.Icon = Properties.Resources.working_time;   // Resources 아이콘 사용 시
            notify.Visible = true;
            notify.ContextMenu = menu;
            notify.Text = "출퇴근관리시스템";

            System.Windows.Forms.MenuItem item1 = new System.Windows.Forms.MenuItem();
            menu.MenuItems.Add(item1);
            item1.Index = 0;
            item1.Text = "열기";
            item1.Click += delegate (object click, EventArgs eClick)
            {
                this.Show();
            };

            System.Windows.Forms.MenuItem item2 = new System.Windows.Forms.MenuItem();
            menu.MenuItems.Add(item2);
            item2.Index = 1;
            item2.Text = "프로그램 종료";
            item2.Click += delegate (object click, EventArgs eClick)
            {
                System.Windows.Application.Current.Shutdown();
                notify.Dispose();
            };

            notify.DoubleClick += new System.EventHandler(this.notify_DoubleClick);
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void notify_DoubleClick(object Sender, EventArgs e)
        {
            this.Show();
        }

        private async void CheckIdleTimer_Tick(object sender, System.EventArgs e)
        {
            moveTime.Content = "Total time : " + Win32.GetTickCount().ToString() + "; " + "Last input time : " + Win32.GetLastInputTime().ToString();

            //20분 이석처리시 (1000 -> 1초, 60000 -> 1분)
            if (Win32.GetIdleTime() > 1200000)
            {
                _moveFg = 1;
                Win32.LockWorkStation();
            }
            else
            {
                if (_moveFg == 1)
                {
                    _moveFg = 0;
                    MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;
                    if (window != null)
                    {
                        await window.ShowMessageAsync("알림창", "다시 자리로 돌아오셨습니다.");
                    }
                }
            }
        }

        //데이터 확인
        private void checkData()
        {
            ClientConfig cf = InitSetting.CConf;

            if(!cf.workStartLastTime.Equals("") && cf.workEndLastTime.Equals("")) //퇴근처리가 안된 데이터가 존재
            {
                //오늘 날짜와 같을 경우
                if (cf.workStartLastTime.Substring(0, 8).Equals(DateTime.Now.ToString("yyyyMMdd")))
                {
                    MessageBoxResult result = MessageBox.Show("출근되어 있습니다. 계속 근무하고 계십니까?", "근무설정",
                                                 MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        cf.workStatus = "1";
                    }
                    else
                    {
                        DateTime sdt = CDateTime.GetDateFrom_yyyyMMddHHmmss(cf.workStartLastTime);
                        DateTime edt = DateTime.Now;

                        String workDate = CDateTime.GetDateFrom_yyyyMMddHHmmss(cf.workStartLastTime).ToString("yyyyMMdd");

                        String _sDateTime = sdt.ToString("yyyyMMdd") + sdt.TimeOfDay.ToString().Replace(":", "");
                        String _eDateTime = edt.ToString("yyyyMMdd") + edt.TimeOfDay.ToString().Replace(":", "");

                        int sTime = Int32.Parse(sdt.TimeOfDay.ToString().Replace(":", "").Substring(0,4) + "00");
                        int eTime = Int32.Parse(edt.TimeOfDay.ToString().Replace(":", "").Substring(0, 4) + "00");

                        regWorkData(workDate, _sDateTime, _eDateTime, sTime, eTime);                        

                        string[] param = new string[2];
                        param[0] = string.Empty;
                        param[1] = string.Empty;
                        cf.workStartLastTime = "";
                        cf.workEndLastTime = "";

                        cf.workStatus = "0";

                        InitSetting.ConfigWriteProfile(param, 1);

                        return;
                    }
                }
                else
                {
                    MessageBox.Show("전에 퇴근처리되지 않은 데이터가 유효기간(1일)이 지나 삭제되었습니다. " +
                        "데이터 확인 후 수동으로 등록 부탁드립니다.");

                    string[] param = new string[2];
                    param[0] = string.Empty;
                    param[1] = string.Empty;
                    cf.workStartLastTime = "";
                    cf.workEndLastTime = "";

                    cf.workStatus = "0";

                    InitSetting.ConfigWriteProfile(param, 1);

                    return;
                }
            }
        }

        private async void regWorkData(String workDate, String _sDateTime, String _eDateTime, int sTime, int eTime)
        {
            try
            {
                MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

                ClientConfig cf = InitSetting.CConf;

                //18:00이후 인경우만 처리(저녁)
                int _dinnerTime = 0;

                if (sTime <= 180000 && eTime > 190000)
                {
                    _dinnerTime = 60;
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
                                        ).ToList<WorkEntity>();

                    if (totalWorkHour.Count > 0)
                    {
                        MessageBox.Show("등록된 자료중에 시간이 중복되는 자료가 있습니다.");
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

                if (overTime > 0)
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
                            this.Show();
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
                                this.Show();
                                cf.workStatus = "0";
                                await window.ShowMessageAsync("알림창", "등록었습니다. 야근이 있는 경우 승인 후 야근시간이 반영되어 집니다.");
                            }
                            else
                            {                                
                                cf.workStatus = "0";
                                _uc_dashboard = new uc_DashBoard();
                                uc_Class.Uc_Link(Contents_Border, _uc_dashboard);
                            }
                        }
                        break;
                    }
                }
                cf.overtimeReason = "";
            }
            catch
            {
            }
        }
    }


    //마우스 이동이 일어나지 않을 경우 처리(이석처리)

    internal struct LASTINPUTINFO
    {
        public uint cbSize;
        public uint dwTime;
    }


    public class Win32
    {
        [DllImport("User32.dll")]
        public static extern bool LockWorkStation();

        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [DllImport("Kernel32.dll")]
        private static extern uint GetLastError();

        public static uint GetIdleTime()
        {
            LASTINPUTINFO lastInPut = new LASTINPUTINFO();
            lastInPut.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lastInPut);
            GetLastInputInfo(ref lastInPut);

            return ((uint)Environment.TickCount - lastInPut.dwTime);
        }

        public static long GetTickCount()
        {
            return Environment.TickCount;
        }

        public static long GetLastInputTime()
        {
            LASTINPUTINFO lastInPut = new LASTINPUTINFO();
            lastInPut.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lastInPut);
            if (!GetLastInputInfo(ref lastInPut))
            {
                throw new Exception(GetLastError().ToString());
            }

            return lastInPut.dwTime;
        }
    }
}
