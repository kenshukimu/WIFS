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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls.Dialogs;

namespace WIFS
{
    /// <summary>
    /// uc_DashBoard.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class uc_Setting : UserControl
    {
        public uc_Setting()
        {
            InitializeComponent();

            ClientConfig cf = InitSetting.CConf;

            txt_ServerIP.Text = cf.serverIP;
            txt_UserID.Text = cf.userID;
            txt_UserPass.Password = cf.userPass;
            chk_AutoLogin.IsChecked = cf.autoLogin.Equals("1") ? true:false;
            chk_AutoAlarm.IsChecked = cf.autoAlarm.Equals("1") ? true : false;
        }

        private async void Btn_save_Click(object sender, RoutedEventArgs e)
        {
            ClientConfig cf = InitSetting.CConf;

            String[] param = new string[6];

            param[0] = txt_ServerIP.Text.Trim();
            param[1] = txt_UserID.Text;
            param[2] = txt_UserPass.Password;
            param[3] = (bool)chk_AutoLogin.IsChecked ? "1" : "0";
            param[4] = (bool)chk_AutoAlarm.IsChecked ? "1" : "0";
            
            InitSetting.ConfigWriteProfile(param,0);

            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;
            if (window != null)
            {
                await window.ShowMessageAsync("환경설정 저장", "저장되었습니다.");
            }

        }
    }
}
