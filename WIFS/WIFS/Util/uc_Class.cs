using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace WIFS.Util
{   
    public class uc_Class
    {
        static WeakReference wUserContrl;
        public static UserControl UserControl
        {
            get { return (wUserContrl != null) ? wUserContrl.Target as UserControl : null; }
            set { wUserContrl = new WeakReference(value); }
        }

        public static void Uc_Link(System.Windows.Controls.Grid grd, System.Windows.Controls.UserControl uc)
        {
            //CsharpWebComponent Process 삭제
            //Process[] pros = Process.GetProcessesByName("CefSharp.BrowserSubprocess");

            //Process[] pros = Process.GetProcesses();

            //foreach (Process p in pros)
            //{
            //    p.Kill();
            //}
            if (grd.Children.Count > 0)
            {
                grd.Children.Remove(uc);
                grd.Children.Clear();
                grd.DataContext = null;
                GC.Collect();

                grd.Children.Add(uc);
            }
            else
            {
                grd.Children.Add(uc);
            }
        }
        public class SettingPopupParameter
        {
            public string _ID { get; set; }

            public SettingPopupParameter(string id)
            {
                _ID = id;
            }
        }
    }
}
