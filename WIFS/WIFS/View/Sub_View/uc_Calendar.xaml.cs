using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using C1.C1Schedule;
using C1.WPF.Schedule;
using Newtonsoft.Json;

namespace WIFS
{
    /// <summary>
    /// uc_Calendar.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class uc_Calendar : UserControl
    {
        AppointmentBOList _list = null;
        ClientConfig cf = InitSetting.CConf;
        
        public uc_Calendar()
        {
            InitializeComponent();            
            _list = new AppointmentBOList();

            Language = System.Windows.Markup.XmlLanguage.GetLanguage(System.Globalization.CultureInfo.CurrentCulture.Name);
            Unloaded += OnUnload;

            scheduler1.LayoutUpdated += new EventHandler(scheduler1_LayoutUpdated);
            scheduler1.VisualIntervalScale = TimeSpan.FromMinutes(30);
            scheduler1.CalendarHelper.StartDayTime = TimeSpan.Parse("09:00:00");
            scheduler1.Settings.FirstVisibleTime = TimeSpan.Parse("09:00:00");

            scheduler1.CalendarHelper.EndDayTime = TimeSpan.Parse("18:00:00");

            scheduler1.BeginUpdate();
            scheduler1.Theme = C1SchedulerResources.Office2010Blue;
            scheduler1_LayoutUpdated(null, null);

            scheduler1.EndUpdate();
        }

        void OnLoad(object sender, RoutedEventArgs e)
        {
            Import();
        }


        private async void Import()
        {
            _list.Clear();
            ScheduleEntity se = new ScheduleEntity()
            {
                id = cf.userID
            };

            var result = Task.Run(() => new CallWebApi().CallPostApiSchedules("scheduleFind", se));

            Schedule scheduleList = JsonConvert.DeserializeObject<Schedule>(await result);

            var _targerList = scheduleList.scheduleList;

            foreach(ScheduleEntity _item in _targerList)
            {
                AppointmentBusinessObject _object = new AppointmentBusinessObject();
                _object.Subject = _item.subject;
                _object.Start = CDateTime.GetDateFrom_yyyyMMddHHmmss(_item.start);
                _object.End = CDateTime.GetDateFrom_yyyyMMddHHmmss(_item.end);
                _object.Body = _item.body;
                _object.BOProperty1 = _item._id;

                _list.Add(_object);
            }
        }

        /// <summary>
		/// Gets a collection of custom business objects, used as a data source for C1Scheduler's AppointmentStorage.
		/// </summary>
		public AppointmentBOList Appointments
        {
            get
            {
                return _list;
            }
        }
      
        private void Views_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (scheduler1 == null)
            {
                return;
            }
            switch (views.SelectedIndex)
            {
                case 0:
                    SetStyle(scheduler1.OneDayStyle);
                    break;
                case 1:
                    SetStyle(scheduler1.WorkingWeekStyle);
                    break;
                case 2:
                    SetStyle(scheduler1.WeekStyle);
                    break;
                case 3:
                    SetStyle(scheduler1.MonthStyle);
                    break;
                case 4:
                    SetStyle(scheduler1.TimeLineStyle);
                    break;
            }
        }

        void scheduler1_LayoutUpdated(object sender, EventArgs e)
        {
            if (scheduler1.Style == scheduler1.MonthStyle)
            {
                views.SelectedIndex = 3;
            }
            else
            {
                if (scheduler1.Style == scheduler1.OneDayStyle)
                {
                    views.SelectedIndex = 0;
                }
                else if (scheduler1.Style == scheduler1.WorkingWeekStyle)
                {
                    views.SelectedIndex = 1;
                }
                else if (scheduler1.Style == scheduler1.WeekStyle)
                {
                    views.SelectedIndex = 2;
                }
                else
                {
                    views.SelectedIndex = 4;
                }
            }
        }

        private void SetStyle(Style style)
        {
            if (!IsLoaded || scheduler1.Style == style)
            {
                return;
            }
            scheduler1.BeginUpdate();
            try
            {
                scheduler1.ChangeStyle(style);
            }
            finally
            {
                scheduler1_LayoutUpdated(null, null);
                // Always call EndUpdate to apply all changes.
                scheduler1.EndUpdate();
            }
        }

        void ContactStorage_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            if (e.ListChangedType == System.ComponentModel.ListChangedType.ItemAdded)
            {
                Contact c = scheduler1.DataStorage.ContactStorage.Contacts[e.NewIndex];
                // you have to fill at least MenuCaption as 
                // it's mapped to the Employees.LastName field which doesn't allow empty strings.
                c.MenuCaption = "test contact";
            }
        }

        private void OnUnload(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
