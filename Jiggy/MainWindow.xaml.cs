using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Jiggy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int adjustment = 1;
        CancellationTokenSource wtoken;
        Task task; 
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
        const int VK_UP = 0x26; //up key
        const int VK_DOWN = 0x28;  //down key
        const int VK_LEFT = 0x25;
        const int VK_RIGHT = 0x27;
        const uint KEYEVENTF_KEYUP = 0x0002;
        const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        void PressUpKey()
        {
            //Press the key
            keybd_event((byte)VK_UP, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
        }
        void PressDownKey()
        {
            //Press the key
            keybd_event((byte)VK_DOWN, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
        }
        Point GetMousePos() => Mouse.GetPosition(this);
        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int x, int y);


        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };
        public static Point GetMousePosition()
        {
            var w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);

            return new Point(w32Mouse.X, w32Mouse.Y);
        }

        public MainWindow()
        {
            InitializeComponent();

            wtoken = new CancellationTokenSource();
        }
        private void MoveMouse()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                //Point pointToWindow = Mouse.GetPosition(this);
                //Point pointToScreen = PointToScreen(pointToWindow);
                var mouseLoc = GetMousePosition();
                SetCursorPos((int)mouseLoc.X + adjustment, (int)mouseLoc.Y + adjustment);
                adjustment = adjustment * -1;

                if (adjustment > 0)
                    PressUpKey();
                else
                    PressDownKey();
            }));
        }

        private void btnStartStop_Click(object sender, RoutedEventArgs e)
        {
            if (btnStartStop.Content.ToString().Equals("Start"))
            {
                wtoken = new CancellationTokenSource();
                StartMovingMouse();
                Application.Current.Dispatcher.Invoke(() => btnStartStop.Content = "Stop");
            }
            else
            {
                wtoken.Cancel();
                Application.Current.Dispatcher.Invoke(() => btnStartStop.Content = "Start");
            }
        }
        private void StartMovingMouse()
        {
            task = Task.Run(async () =>  // <- marked async
            {
                while (true)
                {
                    MoveMouse();
                    await Task.Delay(1000, wtoken.Token); // <- await with cancellation
                }
            }, wtoken.Token);
        }
    }
}
