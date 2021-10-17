using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;
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
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace BreakHub
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string input = "";
        private const string new_timer = "00:00:00";
        private int time = 0;
        private bool flag = true; //true for study; false for break
        private DispatcherTimer Timer = new DispatcherTimer();
        private int study_time = 0, break_time = 50;
        private bool killThread = false;

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hwnd, StringBuilder ss, int count);


        

        public MainWindow()
        {
            InitializeComponent();
        }

        private string ActiveWindowTitle()
        {
            const int nChar = 256;
            StringBuilder build = new StringBuilder(nChar);
            IntPtr handle = IntPtr.Zero;
            handle = GetForegroundWindow();
            if (GetWindowText(handle, build, nChar) > 0)
            {
                return build.ToString();
            }
            else
            {
                return "";
            }

        }

        //helper method to see if any key was pressed
        private static bool anyKeyDown()
        {
            var values = Enum.GetValues(typeof(Key));

            foreach (var k in values)
                if (((Key)k) != Key.None && Keyboard.IsKeyDown((Key)k))
                    return true;

            return false;
        }

        
        //check if the user is working, if the mouse is used or any key is pressed at every half a second, then the timer will be starting or stopping
        private void isWorking()
        {
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            while (true)
            {
                if (killThread) break;
                int count = 0;
                var mouseLeftState = Mouse.LeftButton;
                var mouseRightState = Mouse.RightButton;
                var mouseCenterState = Mouse.MiddleButton;
                watch.Start();
                System.Windows.Point pos;
                Dispatcher.Invoke(() => {
                    pos = Mouse.GetPosition(this);
                });

                while (watch.ElapsedMilliseconds < 5000)
                {
                    string windowName = ActiveWindowTitle();
                    System.Windows.Point currPos;
                    Dispatcher.Invoke(() => {
                        currPos = Mouse.GetPosition(this);
                    });
                    if (mouseLeftState == MouseButtonState.Pressed || mouseCenterState == MouseButtonState.Pressed || mouseRightState == MouseButtonState.Pressed
                        || anyKeyDown() || windowName.Contains("Youtube") || windowName.Contains("Netflix") || Math.Abs(pos.X - currPos.X) + Math.Abs(pos.Y - currPos.Y) > 20)
                    {
                        count++;
                    }
                }
                watch.Stop();
                watch.Reset();
                if (count > 0)
                {
                    flag = true;
                } 
                else
                {
                    flag = false;
                }

                if (flag)
                {
                    Timer.Start();
                    System.Threading.Thread.Sleep(20000);
                    Timer.Stop();
                } 
                else
                {
                    Timer.Stop();
                    System.Threading.Thread.Sleep(10000);
                }
            }
        }

        


        private void beg_timer_click(object sender, RoutedEventArgs e)
        {
            Console.Write("hi");
            killThread = false;
            System.Threading.Thread trackFlagThread = new System.Threading.Thread(isWorking);
            trackFlagThread.SetApartmentState(System.Threading.ApartmentState.STA);
            trackFlagThread.IsBackground = true;
            trackFlagThread.Start();
            Console.Write("Hello");
            input = Timer_Input.Text;
            time = string_time(input);
            study_time = time;
            //popup(SystemIcons.Warning.ToBitmap(), "Timer in Secs", "\n\n" + string_time(input).ToString());
            //time_remain = input;
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Tick += Timer_tick;
            Timer.Start();
        }
        private void stp_timer_click(object sender, RoutedEventArgs e)
        {
            killThread = true;
            System.Threading.Thread.Sleep(100);
            popup(SystemIcons.Warning.ToBitmap(), "Timer Interrupted", "\n\nThe timer is stopped because user clicks stop timer button!");
            Timer_Input.Text = new_timer;
            Timer.Stop();
            flag = true;
        }
        private void popup(System.Drawing.Image image, string label, string message)
        {
            SystemSounds.Exclamation.Play();
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
            ihr = (time / 60) / 60;
            if (ihr < 10) hr = "0" + ihr.ToString();
            else if (ihr == 0) hr = "00";
            else hr = ihr.ToString();

            imin = (time / 60) % 60;
            if (imin < 10) min = "0" + imin.ToString();
            else if (imin == 0) min = "00";
            else min = imin.ToString();

            isec = time % 60;
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
        private void Timer_tick(object sender, EventArgs e)
        {
            if(time == 0)
            {
                if (flag)
                {
                    popup(SystemIcons.Exclamation.ToBitmap(), "Stop studying", "\n\nYou need take a break!");
                    reset_timer(break_time);
                }
                else 
                {
                    popup(SystemIcons.Exclamation.ToBitmap(), "Break finished", "\n\nYou can back to study");
                    reset_timer(study_time);
                }
                
            }
            else
            {
                time--;
                Timer_Input.Text = time_string(time);
            }
            
        }

        private void Startup_CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }


        private void reset_timer(int input_time) {
            Timer.Stop();
            Timer_Input.Text = new_timer;
            time = input_time;
            Timer = new DispatcherTimer();
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Tick += Timer_tick;
            Timer.Start();
            flag = !flag;
        }

        
    }
}
