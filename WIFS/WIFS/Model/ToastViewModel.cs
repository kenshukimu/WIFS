using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ToastNotifications;
using ToastNotifications.Core;
using ToastNotifications.Lifetime;
using ToastNotifications.Lifetime.Clear;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace WIFS.Model
{
    public class ToastViewModel : INotifyPropertyChanged
    {
        private readonly Notifier _notifier;

        public ToastViewModel(String kb)
        {
            switch (kb)
            {
                case "0":
                    _notifier = new Notifier(cfg =>
                    {
                        cfg.PositionProvider = new PrimaryScreenPositionProvider(
                            corner: Corner.BottomRight,
                            offsetX: 5,
                            offsetY: 5);

                        cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                            notificationLifetime: TimeSpan.FromSeconds(6),
                            maximumNotificationCount: MaximumNotificationCount.FromCount(6));

                        cfg.Dispatcher = Application.Current.Dispatcher;

                        cfg.DisplayOptions.TopMost = false;
                        cfg.DisplayOptions.Width = 300;
                    });

                    _notifier.ClearMessages(new ClearAll());
                    break;
                case "1":
                    _notifier = new Notifier(cfg =>
                    {
                        cfg.PositionProvider = new WindowPositionProvider(
                            parentWindow: Application.Current.MainWindow,
                            corner: Corner.TopRight,
                            offsetX: 10,
                            offsetY: 10);


                        cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                            notificationLifetime: TimeSpan.FromSeconds(4),
                            maximumNotificationCount: MaximumNotificationCount.FromCount(4));

                        cfg.Dispatcher = Application.Current.Dispatcher;

                        cfg.DisplayOptions.TopMost = false;
                        cfg.DisplayOptions.Width = 300;
                    });

                    _notifier.ClearMessages(new ClearAll());
                    break;
                default:
                    break;
            }
        }

        public void OnUnloaded()
        {
            _notifier.Dispose();
        }

        public void ShowInformation(string message)
        {
            _notifier.ShowInformation(message);
        }

        public void ShowSuccess(string message)
        {
            _notifier.ShowSuccess(message);
        }

        internal void ClearMessages(string msg)
        {
            _notifier.ClearMessages(new ClearByMessage(msg));
        }

        public void ShowWarning(string message)
        {
            _notifier.ShowWarning(message);
        }

        public void ShowError(string message)
        {
            _notifier.ShowError(message);
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ClearAll()
        {
            _notifier.ClearMessages(new ClearAll());
        }
    }
}
