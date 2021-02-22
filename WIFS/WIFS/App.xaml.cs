using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WIFS
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        public bool IsProcessOpen(string name)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName.Contains(name))
                {
                    return true;
                }
            }

            return false;
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            // Get Reference to the current Process
            Process thisProc = Process.GetCurrentProcess();

            if (IsProcessOpen("WIFS") == false)
            {
                //System.Windows.MessageBox.Show("Application not open!");
                //System.Windows.Application.Current.Shutdown();
                var mainWindow = new MainWindow();
                mainWindow.Show();
            }
            else
            {
                // Check how many total processes have the same name as the current one
                if (Process.GetProcessesByName(thisProc.ProcessName).Length > 1)
                {
                    // If ther is more than one, than it is already running.
                    System.Windows.MessageBox.Show("프로그램이 실행 중에 있습니다.");
                    System.Windows.Application.Current.Shutdown();
                    return;
                }
                else
                {
                    var mainWindow = new MainWindow();
                    mainWindow.Show();
                }

                base.OnStartup(e);
            }
        }
    }
}