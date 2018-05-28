using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Test
{
    [Serializable()]
    public class Journal
    {
        private char[,] baseJournal;

        //Конструктор журнала
        public Journal()
        {
            baseJournal = JournalGetBase();
        }

        //Геттер текущего журнала
        public char[,] JournalGet()
        {
            char[,] curField = JournalGetBase();
            return curField;
        }

        //Получение заготовки для журнала из файла
        private char[,] JournalGetBase()
        {
            string fileName = "journal.txt";
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



    }
}
