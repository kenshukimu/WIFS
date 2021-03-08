using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using C1.C1Schedule;
using C1.WPF;
using C1.WPF.DateTimeEditors;
using C1.WPF.Localization;
using C1.WPF.Schedule;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace WIFS
{
    /// <summary>
    /// EditAppointmentControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class EditAppointmentControl : UserControl
    {
        #region ** fields
        private ContentControl _parentWindow = null;
        private Appointment _appointment;

        private C1Scheduler _scheduler;
        private bool _isLoaded = false;

        private TimeSpan _defaultStart;
        private TimeSpan _defaultDuration;
        ClientConfig cf = InitSetting.CConf;

        #endregion

        #region ** initialization
        /// <summary>
        /// Creates the new instance of the <see cref="EditAppointmentControl"/> class.
        /// </summary>
        public EditAppointmentControl()
        {
            InitializeComponent();
        }

        private void EditAppointmentControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_isLoaded)
            {
                _appointment = DataContext as Appointment;

                _defaultStart = _appointment.AllDayEvent ? TimeSpan.FromHours(8) : _appointment.Start.TimeOfDay;
                _defaultDuration = _appointment.AllDayEvent ? TimeSpan.FromMinutes(30) : _appointment.Duration;
                if (!System.Windows.Interop.BrowserInteropHelper.IsBrowserHosted)
                {
                    _parentWindow = (ContentControl)VTreeHelper.GetParentOfType(this, typeof(Window));
                }
                else
                    _parentWindow = (ContentControl)VTreeHelper.GetParentOfType(this, typeof(C1Window));
                if (_parentWindow != null)
                {
                    Binding bnd = new Binding("Header");
                    bnd.Source = this;
                    if (_parentWindow is Window)
                    {
                        _parentWindow.SetBinding(Window.TitleProperty, bnd);
                    }
                    else
                    {
                        _parentWindow.SetBinding(C1Window.HeaderProperty, bnd);
                    }

                    if (_parentWindow is Window)
                    {
                        ((Window)_parentWindow).Closed += new EventHandler(_parentWindow_Closed);
                    }
                    else
                        ((C1Window)_parentWindow).Closed += new EventHandler(_parentWindow_Closed);
                }
                if (_appointment != null)
                {
                    ((System.ComponentModel.INotifyPropertyChanged)_appointment).PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(appointment_PropertyChanged);
                    if (_appointment.ParentCollection != null)
                    {
                        _scheduler = ((C1ScheduleStorage)_appointment.ParentCollection.ParentStorage.ScheduleStorage).Scheduler;

                        AppointmentBOList _os = (AppointmentBOList)_scheduler.DataStorage.AppointmentStorage.List;
                        var _list = _os.Where(w => w.Id.Equals(_appointment.Key[0])).ToList();

                        foreach(AppointmentBusinessObject _item in _list)
                        {
                            _hidid.Text = _item.BOProperty1.ToString();
                        }

                        if (_appointment.AllDayEvent)
                        {
                            _defaultStart = _scheduler.CalendarHelper.StartDayTime;
                            _defaultDuration = _scheduler.CalendarHelper.Info.TimeScale;
                        }
                    }
                    UpdateWindowHeader();
                    UpdateRecurrenceState();
                    UpdateCollections();
                    UpdateEndCalendar();
                    if (_appointment.AllDayEvent)
                    {
                        startCalendar.EditMode = endCalendar.EditMode = C1DateTimePickerEditMode.Date;
                    }
                    else
                    {
                        startCalendar.EditMode = endCalendar.EditMode = C1DateTimePickerEditMode.DateTime;
                    }
                    UpdateEditingControls();
                }
                if (_parentWindow != null && _appointment != null)
                {
                    _isLoaded = true;
                }
            }
            subject.Focus();
        }

        void _parentWindow_Closed(object sender, EventArgs e)
        {
            ((System.ComponentModel.INotifyPropertyChanged)_appointment).PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(appointment_PropertyChanged);
            if (_parentWindow is Window)
            {
                ((Window)_parentWindow).Closed -= new EventHandler(_parentWindow_Closed);
            }
            else
                ((C1Window)_parentWindow).Closed -= new EventHandler(_parentWindow_Closed);
        }

        #endregion

        #region ** object model
        /// <summary>
        /// Gets or sets an <see cref="Appointment"/> object representing current DataContext.
        /// </summary>
        public Appointment Appointment
        {
            get
            {
                return _appointment;
            }
            set
            {
                _appointment = value;
                if (_parentWindow != null)
                {
                    _parentWindow.Content =
                    _parentWindow.DataContext = value;
                }
                DataContext = value;
                if (_appointment != null)
                {
                    UpdateWindowHeader();
                    UpdateRecurrenceState();
                    UpdateCollections();
                }
            }
        }

        /// <summary>
        /// Gets a <see cref="String"/> value which can be used as an Appointment window header.
        /// </summary>
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            private set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Header"/> dependency property.
        /// </summary>
        private static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string),
            typeof(EditAppointmentControl), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets recurrence pattern description.
        /// </summary>
        public string PatternDescription
        {
            get { return (string)GetValue(PatternDescriptionProperty); }
            private set { SetValue(PatternDescriptionProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="PatternDescription"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PatternDescriptionProperty =
            DependencyProperty.Register("PatternDescription", typeof(string),
            typeof(EditAppointmentControl), new PropertyMetadata(string.Empty));

        #endregion

        #region ** private stuff
        private void UpdateWindowHeader()
        {
            string result;
            string subject = string.Empty;
            bool allDay = false;
            if (_appointment != null)
            {
                subject = _appointment.Subject;
                allDay = chkAllDay.IsChecked.Value;
            }
            if (String.IsNullOrEmpty(subject))
            {
                subject = C1Localizer.GetString("EditAppointment", "Untitled", "Untitled");
            }
            if (allDay)
            {
                result = C1Localizer.GetString("EditAppointment", "Event", "Event") + " - " + subject;
            }
            else
            {
                result = C1Localizer.GetString("EditAppointment", "Appointment", "Appointment") + " - " + subject;
            }

            Header = result;
        }

        private void UpdateRecurrenceState()
        {
            switch (_appointment.RecurrenceState)
            {
                case RecurrenceStateEnum.Master:
                    PatternDescription = Appointment.GetRecurrencePattern().Description;
                    startEndPanel.Visibility = Visibility.Collapsed;
                    recurrenceInfoPanel.Visibility = Visibility.Visible;
                    break;
                default:
                    PatternDescription = string.Empty;
                    startEndPanel.Visibility = Visibility.Visible;
                    recurrenceInfoPanel.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        void appointment_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateRecurrenceState();
            UpdateEndCalendar();
        }

        private void LayoutRoot_BindingValidationError(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
            {
                PART_DialogSaveButton.IsEnabled = false;
            }
            else
            {
                PART_DialogSaveButton.IsEnabled = true;
            }
        }

        private void SetAppointment()
        {
            subject.Focus();
            body.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        private void UpdateCollections()
        {
            if (Appointment != null)
            {
                Appointment.ReminderSet = false;
            }
        }

        private void UpdateEditingControls()
        {
            if (_scheduler != null)
            {

            }
        }

        private void PART_DialogSaveButton_Click(object sender, RoutedEventArgs e)
        {
            SetAppointment();

            var _app = _appointment;

            ScheduleEntity se = new ScheduleEntity()
            {
                id = cf.userID,
                subject = _app.Subject,
                date = CDateTime.GetDateTimeFormat(_app.Start, "yyyymmdd"),
                start = CDateTime.GetDateTimeFormat(_app.Start, "yyyymmddhhnnss"),
                end = CDateTime.GetDateTimeFormat(_app.End, "yyyymmddhhnnss"),
                body = _app.Body,
                _id = _hidid.Text
            };

            //데이터 재 취득
            DateControl(se);

            //데이터 재 취득
            DateControl2();

            if (_parentWindow is Window)
            {
                _parentWindow.Tag = "true";
                ((Window)_parentWindow).Close();
            }
            else
                ((C1Window)_parentWindow).DialogResult = MessageBoxResult.OK;
        }

        private async void DateControl(ScheduleEntity se)
        {
            if (_hidid.Text == null || _hidid.Text.Equals(""))
            {
                await new CallWebApi().CallPostApiSchedules("scheduleAdd", se);
            }
            else
            {
                await new CallWebApi().CallPostApiSchedules("scheduleUpdate", se);
            }
        }

        private async void DateControl2()
        {
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

        private void saveAsButton_Click(object sender, RoutedEventArgs e)
        {
            SetAppointment();
        }

        private void subject_TextChanged(object sender, TextChangedEventArgs e)
        {
            subject.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            UpdateWindowHeader();
        }

        private void chkAllDay_Checked(object sender, RoutedEventArgs e)
        {
            startCalendar.EditMode = endCalendar.EditMode = C1DateTimePickerEditMode.Date;
            UpdateWindowHeader();
        }

        private void chkAllDay_Unchecked(object sender, RoutedEventArgs e)
        {
            _appointment.Start = _appointment.Start.Add(_defaultStart);
            _appointment.Duration = _defaultDuration;
            startCalendar.EditMode = endCalendar.EditMode = C1DateTimePickerEditMode.DateTime;
            UpdateWindowHeader();
        }

        private void endCalendar_DateTimeChanged(object sender, NullablePropertyChangedEventArgs<DateTime> e)
        {
            if (_appointment != null)
            {
                DateTime end = endCalendar.DateTime.Value;
                if (_appointment.AllDayEvent)
                {
                    end = end.AddDays(1);
                }
                if (end < Appointment.Start)
                {
                    endCalendar.BorderBrush = endCalendar.Foreground = new SolidColorBrush(Colors.Red);
                    endCalendar.BorderThickness = new Thickness(2);
                    ToolTipService.SetToolTip(endCalendar, C1Localizer.GetString("Exceptions", "StartEndValidationFailed", "The End value should be greater than Start value."));
                    PART_DialogSaveButton.IsEnabled = false;
                }
                else
                {
                    _appointment.End = end;
                    if (!PART_DialogSaveButton.IsEnabled)
                    {
                        PART_DialogSaveButton.IsEnabled = true;
                        endCalendar.ClearValue(Control.ForegroundProperty);
                        endCalendar.ClearValue(Control.BorderBrushProperty);
                        endCalendar.ClearValue(Control.BorderThicknessProperty);
                        endCalendar.ClearValue(ToolTipService.ToolTipProperty);
                    }
                }
            }
        }

        private void UpdateEndCalendar()
        {
            DateTime end = _appointment.End;
            if (_appointment.AllDayEvent)
            {
                end = end.AddDays(-1);
            }
            endCalendar.DateTime = end;
            if (!PART_DialogSaveButton.IsEnabled)
            {
                PART_DialogSaveButton.IsEnabled = true;
                endCalendar.ClearValue(Control.BackgroundProperty);
                endCalendar.ClearValue(Control.ForegroundProperty);
                endCalendar.ClearValue(Control.BorderBrushProperty);
                endCalendar.ClearValue(Control.BorderThicknessProperty);
                endCalendar.ClearValue(ToolTipService.ToolTipProperty);
            }
        }
        #endregion

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_hidid.Text.Trim().Equals(""))
            {
                ScheduleEntity se = new ScheduleEntity()
                {
                    _id = _hidid.Text
                };

                await new CallWebApi().CallPostApiSchedules("scheduleDel", se);
            }
        }
            
    }
}