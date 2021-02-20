using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WIFS.Util;

namespace WIFS.View.Sub_View
{
    /// <summary>
    /// pop_inputReason.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class pop_inputReason : MetroWindow
    {
        public pop_inputReason()
        {
            InitializeComponent();
        }

        private async void Btn_overTimeReasonReg_Click(object sender, RoutedEventArgs e)
        {
            ClientConfig cf = InitSetting.CConf;
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (txtOverTimeReason.Text.Trim() == "")
            {
                await window.ShowMessageAsync("알림창", "내용을 입력하여 주시기 바랍니다.");
            }
            else
            {
                cf.overtimeReason = txtOverTimeReason.Text.Trim();
                this.Close();
                MainWindow mw = (MainWindow)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);

                mw.Opacity = 1;
            }
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            this.Close();
            MainWindow mw = (MainWindow)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);

            mw.Opacity = 1;
        }
    }
}
