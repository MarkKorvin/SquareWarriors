using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace Test
{
    [Serializable()]
    static class GlobalProperties
    {
        //=======================================  Параметры мира  =============== 

        static public int dayMinute = 48;   //Сколько минут идут одни сутки                            
        static public List<Acts> MenuMethods = new List<Acts>();
        static Action del;

        // Меню, вызываемое по нажатию ESC
        static public void ShowMenu()
        {
            MenuMethods.Clear();
            MenuMethods.Add(new Acts("Main Menu", del = () => { Program.Window = Program.Windows.menu; MainMenu.ShowMenu(); }));
            MenuMethods.Add(new Acts("Save", del = Save));
            MenuMethods.Add(new Acts("Load", del = Load));
            MenuMethods.Add(new Acts("Game Properties", del = GamePr));
            MenuMethods.Add(new Acts("Video Properties", del = VideoPr));
            MenuMethods.Add(new Acts("Audio Properties", del = AudioPr, true));
            Program.mainDialog.SetDialog("MENU", MenuMethods);
        }
        static public void Save()
        {   
            FileStream fstream = File.Open(AppDomain.CurrentDomain.BaseDirectory + "//Saves//test", FileMode.Create);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(fstream, Program.CurrentMap.mapField);
            fstream.Close();
            Program.Helper.Say("Save is succesfull");
        }

        static public void Load()
        {
            MenuMethods.Clear();

            List<string> MapFiles = new List<string>();
            DirectoryInfo dinfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory+"//Maps");
            FileInfo[] files = dinfo.GetFiles("*.txt");
            foreach (FileInfo filenames in files)
            {
                MapFiles.Add(filenames.ToString());
                MenuMethods.Add(new Acts(filenames.ToString(), del = () => { Program.DownloadMap(filenames.ToString()); ShowMenu(); }, true));
            }
            

            /*MenuMethods.Add(new Acts("mapTest.txt", del = () => { Program.DownloadMap("mapTest.txt"); ShowMenu(); }, true));
            MenuMethods.Add(new Acts("map1.txt", del = () => { Program.DownloadMap("map1.txt"); ShowMenu(); }, true));
            MenuMethods.Add(new Acts("testBattle.txt", del = () => { Program.DownloadMap("testBattle.txt"); ShowMenu(); }, true));
            Program.mainDialog.SetDialog("Choose the map", MenuMethods);
          */
        }
        static public void GamePr()
        {
            MenuMethods.Clear();
            MenuMethods.Add(new Acts("Day long = " + dayMinute.ToString() + " minutes", del = () => { dayMinute = dayMinute < 144 ? dayMinute += 24 : 48; GamePr(); }));
            Program.mainDialog.SetDialog("Game Properties", MenuMethods);
        }
        static public void VideoPr()
        {
            MenuMethods.Clear();
            MenuMethods.Add(new Acts("Screen width (max - " + (Console.LargestWindowWidth - 2).ToString()+ ") = "+ WindowWidth.ToString(), del = () => { WindowWidth = WindowWidth < Console.LargestWindowWidth - 2 ? WindowWidth += 1 : 30; SetProperties(); VideoPr(); }));
            MenuMethods.Add(new Acts("Screen height (max - " + (Console.LargestWindowHeight - InfoHeight - LogHeight).ToString() + ") = " + WindowHeight.ToString(), del = () => { WindowHeight = WindowHeight < Console.LargestWindowHeight - InfoHeight - LogHeight ? WindowHeight += 1 : 17; SetProperties(); VideoPr(); }));
            MenuMethods.Add(new Acts("Logbar height = " + (LogHeight-5).ToString(), del = () => { LogHeight = LogHeight < 20 ? LogHeight += 1 : 8; SetProperties(); VideoPr(); }));
            Program.mainDialog.SetDialog("Changes will take effect after reboot", MenuMethods);
        }
        static public void AudioPr()
        { }

        // Меню, вызываемое по нажатию F2
        static public void ShowHelp()
        {
            MenuMethods.Clear();
            MenuMethods.Add(new Acts("Press ESC to change game properties", del = () => { ShowHelp(); }));
            MenuMethods.Add(new Acts("If you have problems, try to check your JOURNAL", del = () => { ShowHelp(); }));
            MenuMethods.Add(new Acts("To open inventory press I", del = () => { ShowHelp(); }));
            MenuMethods.Add(new Acts("To move press W-A-S-D", del = () => { ShowHelp(); }));
            MenuMethods.Add(new Acts("If you are want look around press W/A/S/D + Shift", del = () => { ShowHelp(); }));

            Program.mainDialog.SetDialog("HELP", MenuMethods);
        }


        //=======================================  Параметры консоли  ===============

        static private int WindowWidth;
        static private int InfoHeight;
        static private int WindowHeight;
        static private int LogHeight;
        //Конструктор 
        static GlobalProperties()
        {
            //WindowWidth = 80;
            //WindowWidth = Console.LargestWindowWidth-2; //На весь экран
            InfoHeight = 8;//лучше не менять
            //LogHeight = 13;
            GetProperties();
            //WindowHeight = Console.LargestWindowHeight - InfoHeight - LogHeight;
        }

        static private void GetProperties()
        {
            string fileName = "Properties.txt";
            string[] lines = File.ReadAllLines(fileName);

            foreach (string line in lines)
            {
                try
                {
                    string[] dividedLine = line.Split('=');
                    switch (dividedLine[0])
                    {
                        case "WindowWidth": WindowWidth = Int32.Parse(dividedLine[1]); break;
                        //case "InfoHeight": InfoHeight = Int32.Parse(dividedLine[1]); break;
                        case "WindowHeight": WindowHeight = Int32.Parse(dividedLine[1]); break;
                        case "LogHeight": LogHeight = Int32.Parse(dividedLine[1]); break;
                        default: break;
                    }
                }
                catch (Exception ex)
                { }
            }

        }
        static private void SetProperties()
        {
            string fileName = "Properties.txt";
            string[] dataProperties = new string[3];
            
            dataProperties[0] = "WindowWidth=" + WindowWidth.ToString();
            //dataProperties[1] = "InfoHeight=" + InfoHeight.ToString();
            dataProperties[1] = "WindowHeight=" + WindowHeight.ToString();
            dataProperties[2] = "LogHeight=" + LogHeight.ToString();
            
            File.WriteAllLines(fileName, dataProperties);
        }

        //Геттеры
        static public int GetWidth()
        {
            return WindowWidth;
        }
        static public int GetInfoHeight()
        {
            return InfoHeight;
        }
        static public int GetHeight()
        {
            return WindowHeight;
        }
        static public int GetLogHeight()
        {
            return LogHeight;
        }
        
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("User32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        //Задаем параметры консоли
        static public void SetConsoleParams()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.SetWindowSize(WindowWidth , InfoHeight + WindowHeight + LogHeight);
            Console.Title = "WinAPI";
            var hWnd = FindWindow(null, Console.Title);
            var wndRect = new RECT();
            GetWindowRect(hWnd, out wndRect);
            var cWidth = wndRect.Right - wndRect.Left;
            var cHeight = wndRect.Bottom - wndRect.Top;
            var SWP_NOSIZE = 0x1;
            var HWND_TOPMOST = -1;
            var Width = 1920;
            var Height = 1080;
            SetWindowPos(hWnd, HWND_TOPMOST, Width / 2 - cWidth / 2, Height / 2 - cHeight / 2, 0, 0, SWP_NOSIZE);
            Console.CursorVisible = false;
        }
        //Получить масштабируемую разделительную полосу
        static public char[] GetEdge()
        {
            char[] edge = new char[WindowWidth];
            for (int i = 0; i < WindowWidth; i++)
            {
                edge[i] = '=';
            }
            return edge;
        }


    }
}
