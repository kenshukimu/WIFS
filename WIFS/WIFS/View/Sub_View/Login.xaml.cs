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

namespace WIFS
{
    /// <summary>
    /// Login.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Login : MetroWindow
    {
        Boolean loginCheck = false;

        public Login()
        {
            InitializeComponent();

            ClientConfig cf = InitSetting.CConf;

            Txt_ServerIp.Text = "222.234.39.68";
            //Txt_ServerIp.Text = "localhost";
            Txt_UserId.Text = cf.userID;

        }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            ClientConfig cf = InitSetting.CConf;
            try
            {
                if (Txt_UserId.Text.Equals(""))
                {
                    errorMessage.Text = "아이디 입력오류";
                    errorMessage.Foreground = new SolidColorBrush(Colors.Red);
                    Txt_UserId.Focus();
                    return;
                } else if (Txt_UserPass.Password.Equals(""))
                {
                    errorMessage.Text = "패스워드 입력오류";
                    errorMessage.Foreground = new SolidColorBrush(Colors.Red);
                    Txt_UserPass.Focus();
                    return;
                }
                else if (Txt_ServerIp.Text.Equals(""))
                {
                    errorMessage.Text = "서버 입력오류";
                    errorMessage.Foreground = new SolidColorBrush(Colors.Red);
                    Txt_ServerIp.Focus();
                    return;
                }

                String[] param = new string[6];
                //param[0] = "ec2-54-86-81-58.compute-1.amazonaws.com";
                param[0] = "222.234.39.68";
                //param[0] = "localhost";

                param[1] = Txt_UserId.Text;
                param[2] = Txt_UserPass.Password;
                param[3] = "0";
                param[4] = "0";

                cf.serverIP = param[0];
                cf.userID = param[1];
                cf.userPass = param[2];
                cf.autoLogin = param[3];
                cf.autoAlarm = param[4];
                
                UserEntity ue = new UserEntity()
                {
                    id = Txt_UserId.Text
                };

                String result = await new CallWebApi().CallPostApiUsers("userFind", ue);
                Users userList = JsonConvert.DeserializeObject<Users>(result);

                //토큰정보 저장
                cf.accesstoken = userList.accesstoken;
                cf.refreshtoken = userList.refreshtoken;

                param[5] = cf.refreshtoken;

                InitSetting.ConfigWriteProfile(param, 0);

                if (userList.userList == null || userList.userList.Count != 1)
                {
                    errorMessage.Text = "아이디 입력오류";
                    errorMessage.Foreground = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    //비밀번호가 틀릴경우 로그인창을 띄운다.
                    if (!Txt_UserPass.Password.Equals(userList.userList[0].password))
                    {
                        errorMessage.Text = "패스워드 입력오류";
                        errorMessage.Foreground = new SolidColorBrush(Colors.Red);
                    }
                    else
                    {
                        errorMessage.Text = "정상로그인";
                        loginCheck = true;
                        cf.userName = userList.userList[0].name;
                        cf.dept = userList.userList[0].depart;

                        this.Close();
                    }
                }
            }
            catch(Exception ex)
            {
                errorMessage.Text = "데이터베이스 연결오류(" + ex + ")";
                errorMessage.Foreground = new SolidColorBrush(Colors.Red);
            }
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {   
            if(!loginCheck) {
                System.Windows.Application.Current.Shutdown();
            }
        }

        private void MetroWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnLogin_Click(sender, e);
            }
        }
    }
}
