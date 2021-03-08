using MahApps.Metro.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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

namespace WIFS.View.Sub_View
{
    /// <summary>
    /// Login.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Login_old : MetroWindow
    {
        Boolean loginCheck = false;

        public Login_old()
        {
            InitializeComponent();

            errorMessage.Content = "출퇴근 관리 시스템 로그인";
            errorMessage.Foreground = new SolidColorBrush(Colors.Black);
        }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            ClientConfig cf = InitSetting.CConf;

            UserEntity ue = new UserEntity()
            {
                id = Txt_UserId.Text
            };

            String result = await new CallWebApi().CallPostApiUsers("userFind", ue);
            Users userList = JsonConvert.DeserializeObject<Users>(result);

            if(userList.userList == null || userList.userList.Count != 1)
            {
                errorMessage.Content = "로그인ID를 다시 입력하여 주시기 바랍니다.";
                errorMessage.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                //비밀번호가 틀릴경우 로그인창을 띄운다.
                if (!Txt_UserPass.Password.Equals(userList.userList[0].password))
                {
                    errorMessage.Content = "패스워드를 다시 입력하여 주시기 바랍니다.";
                    errorMessage.Foreground = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    errorMessage.Content = "정상로그인";                   
                    loginCheck = true;
                    cf.userName = userList.userList[0].name;
                    this.Close();
                }
            }
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {   
            if(!loginCheck) {
                System.Windows.Application.Current.Shutdown();
            }
        }
    }
}
