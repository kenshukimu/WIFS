using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace WIFS.Util
{
    public sealed class InitSetting
    {
        public static InitSetting Instance { get; } = new InitSetting();
        static ClientConfig cConf = new ClientConfig();

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public static ClientConfig CConf { get { return cConf; } set => cConf = value; }

        InitSetting() { }

        public void Init()
        {
            ConfigGetProfile();

            SetStatupProgram();

            new CommonUtil().CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Kico_Work");
        }

        public async static void ConfigGetProfile()
        {
            String IniFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Kico_Work\Config.ini";

            if (!File.Exists(IniFilePath))
            {
                String[] param = new string[5];
                ConfigWriteProfile(param,0);
            }

            StringBuilder ret = new StringBuilder();
            GetPrivateProfileString("CONFIG", "ServerIP", " ", ret, 100, IniFilePath);
            //cConf.serverIP = ret.ToString();
            cConf.serverIP = "54.180.140.98";

            GetPrivateProfileString("CONFIG", "userID", " ", ret, 20, IniFilePath);
            cConf.userID = ret.ToString();

            GetPrivateProfileString("CONFIG", "userPass", "0", ret, 20, IniFilePath);
            cConf.userPass = ret.ToString();

            GetPrivateProfileString("CONFIG", "autoLogin", "0", ret, 2, IniFilePath);
            cConf.autoLogin = ret.ToString();

            GetPrivateProfileString("CONFIG", "autoAlarm", "0", ret, 2, IniFilePath);
            cConf.autoAlarm = ret.ToString();

            GetPrivateProfileString("CONFIG", "workStartLastTime", "0", ret, 15, IniFilePath);
            cConf.workStartLastTime = ret.ToString();

            GetPrivateProfileString("CONFIG", "workEndLastTime", "0", ret, 15, IniFilePath);
            cConf.workEndLastTime = ret.ToString();           
        }


        public static void ConfigWriteProfile(String[] param, int fg)
        {
            //Read Config(권한문제로 현디렉토리를 이용할 수는 없음)
            //String IniFilePath = Environment.CurrentDirectory + @"\Config.ini";
            String IniFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Kico_Work\Config.ini";

            if (fg == 0)
            {
                WritePrivateProfileString("CONFIG", "ServerIP", "54.180.140.98", IniFilePath);
                WritePrivateProfileString("CONFIG", "userID", param[1], IniFilePath);
                WritePrivateProfileString("CONFIG", "userPass", param[2], IniFilePath);
                WritePrivateProfileString("CONFIG", "autoLogin", param[3], IniFilePath);
                WritePrivateProfileString("CONFIG", "autoAlarm", param[4], IniFilePath);

                cConf.serverIP = "54.180.140.98";
                cConf.userID = param[1];
                cConf.autoLogin = param[3];
                cConf.autoAlarm = param[4];
            }
            else if (fg == 1) //출근처리
            {
                WritePrivateProfileString("CONFIG", "workStartLastTime", param[0], IniFilePath);
                WritePrivateProfileString("CONFIG", "workEndLastTime", param[1], IniFilePath);
            }
            else if (fg == 2) //퇴근처리
            {
                WritePrivateProfileString("CONFIG", "workEndLastTime", param[0], IniFilePath);
            }
        }

        private void SetStatupProgram()
        {
            Assembly curAssembly = Assembly.GetExecutingAssembly();
            RegistryKey rgkRun = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (rgkRun == null)
            {
                rgkRun = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
            }

            if(rgkRun.GetValue(curAssembly.GetName().Name) == null)
            {
                rgkRun.SetValue(curAssembly.GetName().Name, Application.ExecutablePath.ToString());
            }
        }
    }
}
