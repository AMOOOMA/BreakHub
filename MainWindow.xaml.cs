using System;
using System.Collections.Generic;
using System.Drawing;
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
using System.Windows.Threading;
using Tulpep.NotificationWindow;

namespace BreakHub
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string input = "";
        private string time_remain = "";
        private const string new_timer = "00:00:00";
        private int time = 0;
        private DispatcherTimer Timer;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void beg_timer_click(object sender, RoutedEventArgs e)
        {
            input = Timer_Input.Text;
            
            time = string_time(input);
            popup(SystemIcons.Warning.ToBitmap(), "Timer in Secs", "\n\n" + string_time(input).ToString());
            //time_remain = input;
            Timer = new DispatcherTimer();
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Tick += timer_tick;
            Timer.Start();
        }
        private void stp_timer_click(object sender, RoutedEventArgs e)
        {
            popup(SystemIcons.Warning.ToBitmap(), "Timer Interrupted", "\n\nThe timer is stopped because user clicks stop timer button!");
            time_remain = new_timer;
            Timer_Input.Text = new_timer;
        }
        private void popup(System.Drawing.Image image, string label, string message)
        {
            PopupNotifier popup = new PopupNotifier();
            popup.Image = image;            //SystemIcons.Information.ToBitmap();
            popup.TitleText = label;        //"Counting down for study";
            popup.ContentText = message;    //"Time remaining: " + input;
            popup.Popup();
        }

        private string time_string(int time) // time is in sec form => return string in 00:00:00 form
        {
            string sec = "", min = "", hr = "";
            int isec = 0, imin = 0, ihr = 0;
            ihr = time / 60;
            if (ihr < 10) hr = "0" + ihr.ToString();
            else if (ihr == 0) hr = "00";
            else hr = ihr.ToString();

            imin = (time % 60) / 60;
            if (imin < 10) min = "0" + imin.ToString();
            else if (imin == 0) min = "00";
            else min = imin.ToString();

            isec = (time % 60) % 60;
            if (isec < 10) sec = "0" + isec.ToString();
            else if (isec == 0) sec = "00";
            else sec = isec.ToString();

            return String.Format("{0}:{1}:{2}", hr, min, sec);
        }

        private int string_time(string str) // str is in 00:00:00 form => return time in sec form
        {
            string[] times = str.Split(':');
            return 60*(60*int.Parse(times[0]) + int.Parse(times[1])) + int.Parse(times[2]);
        }

        private void timer_tick(object sender, EventArgs e)
        {
            if(time == 0)
            {
                popup(SystemIcons.Exclamation.ToBitmap(), "Timer Finished", "\n\nThe timer is stopped because countdown is finished!");
                return;
            }
            else
            {
                time--;
                Timer_Input.Text = time_string(time);
            }
            
        }
    }
}
