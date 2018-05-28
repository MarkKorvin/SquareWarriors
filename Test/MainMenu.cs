using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Test
{
    [Serializable()]
    class MainMenu
    {
        private char[,] baseMenu;
        List<Acts> Acts = new List<Acts>();
        Action del;

        //Конструктор журнала
        public MainMenu()
        {
            baseMenu = MenuGet();
        }

        //Геттер текущего журнала
     
        //Получение заготовки для журнала из файла
        public char[,] MenuGet()
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


        public void ShowMenu() //Предметы в инвентаре
        {
            Acts.Clear();
            Acts.Add(new Acts("Start new game", del = newGame));
            Acts.Add(new Acts("Load game", del = loadGame, true));
            Program.mainDialog.SetDialog("", Acts);
        }

        public void newGame()
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
        public void loadGame()
        {
            Acts.Clear();
        }
    }
}
