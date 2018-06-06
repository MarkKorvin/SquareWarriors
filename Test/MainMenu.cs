using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Test
{
    [Serializable()]
    static class MainMenu
    {
        static char[,] baseMenu;
        static List<Acts> Acts = new List<Acts>();
        static Action del;

        //Конструктор журнала
        static MainMenu()
        {
            baseMenu = MenuGet();
        }

        //Геттер текущего журнала
     
        //Получение заготовки для журнала из файла
        static public char[,] MenuGet()
        {
            string fileName = "MainPicture.txt";
            string[] lines = File.ReadAllLines(fileName);

            int height = lines.Length;
            int width = lines[0].Length;

            char[,] curField = new char[height, width]; //Создаем основной массив символов

            for (int k = 0; k < height; k++)
            {
                char[] tempArray = lines[k].ToCharArray();
                for (int j = 0; j < width; j++)
                {
                    curField[k, j] = lines[k][j];
                }
            }
            return curField;
        }


        static public void ShowMenu()
        {
            Acts.Clear();
            Acts.Add(new Acts("Start new game", del = newGame));
            //Acts.Add(new Acts("Load game", del = loadGame));
            Acts.Add(new Acts("Load game", del = ShowMenu));
            Program.mainDialog.SetDialog("", Acts);
        }

        static private void newGame()
        {
            Acts.Clear();

            List<string> MapFiles = new List<string>();
            DirectoryInfo dinfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "//Maps");
            FileInfo[] files = dinfo.GetFiles("*.txt");
            foreach (FileInfo filenames in files)
            {
                MapFiles.Add(filenames.ToString());
                Acts.Add(new Acts(filenames.ToString(), del = () => { Program.DownloadMap(filenames.ToString()); ShowMenu(); }, true));
            }

        }

        static private void loadGame()
        {
            Acts.Clear();
        }
    }
}
