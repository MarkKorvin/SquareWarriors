using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Test
{
    [Serializable()]
    class GUI
    {
        public char[] edge = Program.Properties.GetEdge();
        int infoHeight = Program.Properties.GetInfoHeight();
        int Height = Program.Properties.GetHeight();
        int Width = Program.Properties.GetWidth();
        int LogHeight = Program.Properties.GetLogHeight();
        string prevStats="";

        //==========================================================================  Вывод на экран  ====================================================================================================
         
        public void DrawMenu() //Верхняя часть интерфейса
        {
            if (Program.timeDraw)
            {
                Console.SetCursorPosition(0, 0);
                Console.WriteLine("F2.HELP || I.INVENTORY || J.JOURNAL || K.SKILLS || " + Program.curTime.ToString().Remove(Program.curTime.ToString().Length - 3) + " " + Program.curTime.DayOfWeek.ToString()); // У времени убираем секунды. Хз как это проще сделать
                Console.WriteLine(edge);
                Console.SetCursorPosition(0, infoHeight);
                Program.timeDraw = false;
            }
        }
        public void DrawDialog(string Text) //Вывод диалога
        {
            if (Program.mainDialog.Actions != null && Program.mainDialog.Actions != Program.mainDialog.prevActions)
            {
                int Next = Program.mainDialog.Next;
                ClearInfoField();
                Console.SetCursorPosition(0, 2);
                Console.WriteLine(Text);

                if (Program.mainDialog.Actions.Count() < 5)
                { Next = 0; }

                if (Program.mainDialog.Actions.Count() > 0)
                {
                    for (int i = 1 + Next; i <= Math.Min(Program.mainDialog.Actions.Count(),Next+Program.Properties.GetInfoHeight()-4); i++)
                    {
                        if(Program.mainDialog.Actions[i - 1].ActInfo!="")
                        Console.WriteLine("" + i + ". " + Program.mainDialog.Actions[i - 1].ActInfo);
                    }
                    if (Program.mainDialog.Actions.Count() >= 5)
                        Console.WriteLine("Tab. " + Program.mainDialog.TabAction.ActInfo);

                    Program.mainDialog.prevActions = Program.mainDialog.Actions;
                }
            }
        }
        public void DrawField(Map CurrentMap, Person Hero) //Вывод основного поля (можно оптимизировать)
        {
            Console.SetCursorPosition(0, infoHeight);
            Console.WriteLine(edge);

            char[,] field = CurrentMap.mapField;
            int leftx = Hero.x - Width / 2;
            int topy = Hero.y - Height / 2;

            for (int i = topy; i < topy + Height; i++)
            {
                for (int j = leftx; j < leftx + Width + 1; j++)
                {
                    try
                    {
                        if (i >= 0 && j >= 0 && i < field.GetLength(0) && j < field.GetLength(1))
                        {
                            if (Program.CurrentMap.MapObjects[i, j].hited == true || Program.CurrentMap.MapObjects[i, j].Dude != null || field[i, j] == 'X')
                            {
                                if (Program.CurrentMap.MapObjects[i, j].hited != true)
                                {
                                    if (i == Hero.y && j == Hero.x || field[i, j] == 'X')
                                    {
                                        Console.ForegroundColor = Hero.group.Color;
                                        Console.Write(field[i, j]);
                                        Console.ForegroundColor = ConsoleColor.Gray;
                                    }
                                    else if (Program.CurrentMap.MapObjects[i, j].Dude != null)
                                    {
                                        Console.ForegroundColor = Program.CurrentMap.MapObjects[i, j].Dude.group.Color;
                                        Console.Write(field[i, j]);
                                        Console.ForegroundColor = ConsoleColor.Gray;
                                    }
                                }

                                else
                                {
                                    Console.BackgroundColor = ConsoleColor.Red;
                                    Console.Write(field[i, j]);
                                    Console.BackgroundColor = ConsoleColor.Black;
                                }
                            }
                            else Console.Write(field[i, j]);
                        }
                        else
                            Console.Write(' ');
                    }
                    catch (Exception ex)
                    { }
                }
                Console.WriteLine();
            }
            Console.WriteLine(edge);
        }
        public void DrawOtherField() //Вывод инвентаря, заставок, больших диалоговых окон и пр.
        {
            Console.SetCursorPosition(0, infoHeight);
            Console.WriteLine(edge);

            char[,] field = Program.SecondWindow;

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width + 1; j++)
                {
                    if (i >= 0 && j >= 0 && i < field.GetLength(0) && j < field.GetLength(1))
                        Console.Write(field[i, j]);
                    else
                        Console.Write(' ');
                }
                Console.WriteLine();
            }
            Console.WriteLine(edge);
        }
        public void DrawStat()
        {
            string stats;
            if (Program.Enemy!= null && Program.Enemy.CurHealth > 0)
            {
                stats = "Life " + Program.Hero.CurHealth + "/" + Program.Hero.MaxHealth + ", Damage " + Program.Hero.GetDamage() + ", Def " + Program.Hero.GetArmor();
                stats += "\t Enemy "+Program.Enemy.Name+", Life "+Program.Enemy.CurHealth + "/" + Program.Enemy.MaxHealth + ", Damage " + Program.Enemy.GetDamage() + ", Def " + Program.Hero.GetArmor();
            }
            else
                stats = "Life " + Program.Hero.CurHealth + "/" + Program.Hero.MaxHealth + ", Damage " + Program.Hero.GetDamage() + ", Def " + Program.Hero.GetArmor();

            if (stats!=prevStats)
            {
                ClearStat();
                Console.SetCursorPosition(0, infoHeight + Height + 2);
                Console.WriteLine(stats);
            }
        }
        public void DrawLog()//Вывод логов(звуков)
        {
            //ConsoleColor curColor = ConsoleColor.Gray;
            if (Program.logData != Program.prevLogData)
            {
                ClearLog();
                Console.SetCursorPosition(0, infoHeight + Height + 3);
                Console.WriteLine(edge);
                for (int i = 0; i < Math.Min(LogHeight - 5, Program.logData.Count()); i++)
                {
                    string[] NameAndText = Program.logData.ElementAt(i).Split(':');
                    if (NameAndText.Length > 1)
                    {
                        Console.ForegroundColor = Program.logColors.ElementAt(i);
                        Console.Write(NameAndText[0] + ":");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine(NameAndText[1]);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine(NameAndText[0]);
                    }
                }
                    //Console.WriteLine(Program.logData.ElementAt(i));
                    Program.prevLogData = Program.logData;
            }
            //=======================Визуализация громкости================(можно оптимизировать)
            //
            // if (Program.logData != null)
            // for (int i = 0; i < Math.Min(LogHeight-5, Program.logData.Count()); i++)
            //{
            // string[] ColAndElem =  Program.logData.ElementAt(i).Split(';');
            //switch (ColAndElem[0])
            // {
            //     case "0": curColor = ConsoleColor.White; break;
            //     case "1": curColor = ConsoleColor.Gray; break;
            //     case "2": curColor = ConsoleColor.DarkGray; break;
            // }
            //Console.ForegroundColor = curColor;
            // Console.WriteLine(ColAndElem[1]);
            //Console.ForegroundColor = ConsoleColor.Gray;
            //     }
        }

        //очистка 
        public void ClearStat()//Очистка логов
        {
            Console.SetCursorPosition(0, infoHeight + Height + 2);
            Console.MoveBufferArea(0, infoHeight + Height + 2, Console.BufferWidth, 1, Console.BufferWidth, infoHeight + Height + 2, ' ', Console.ForegroundColor, Console.BackgroundColor);
        }
        public void ClearLog()//Очистка логов
        {
            Console.SetCursorPosition(0, infoHeight + Height + 3);
            //for (int j = 0; j < infoHeight + Height + 2 + LogHeight; j++)
            for (int j = infoHeight + Height + 2; j < infoHeight + Height + 2 + LogHeight; j++)
                Console.MoveBufferArea(0, j, Console.BufferWidth, 1, Console.BufferWidth, j, ' ', Console.ForegroundColor, Console.BackgroundColor);
        }
        public void ClearField()//Очистка основного поля
        {
            Console.SetCursorPosition(0, infoHeight);
            for (int j = 0; j < infoHeight+Height; j++)
                    Console.Write("\r");
        }
        public void ClearInfoField()//Очистка диалогового окна
        {
            Console.SetCursorPosition(0, 2);
            for (int j = 2; j < infoHeight; j++)
            Console.MoveBufferArea(0, j, Console.BufferWidth, 1, Console.BufferWidth, j, ' ', Console.ForegroundColor, Console.BackgroundColor);
        }

    }

    //==========================================================================  Обработка нажатия клавиши  ===========================================================================================
    [Serializable()]
    class MyKeyEventArgs : HandledEventArgs
    {
        // нажатая кнопка
        public ConsoleKeyInfo key;

        public MyKeyEventArgs(ConsoleKeyInfo _key)
        {
            key = _key;
        }
    }
    [Serializable()]
    class KeyEvent
    {
        // событие нажатия
        public event EventHandler<MyKeyEventArgs> KeyPress;

        // метод запуска события
        public void OnKeyPress(ConsoleKeyInfo _key)
        {
            KeyPress(this, new MyKeyEventArgs(_key));
        }
    }
}
