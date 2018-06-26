using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


namespace Test
{
    [Serializable]
  static class Program
    {
        static public GUI mainGUI = new GUI();                                      //управление вводом/выводом
        static public Dialog mainDialog = new Dialog();                             //работа с диалогами
        //static public MainMenu menu = new MainMenu();
        static public Map CurrentMap;                                               //ссылка на текущую карту
        static public DateTime curTime;                                             //текущее время и дата
        static public Stack<string> logData = new Stack<string>();                  //логи
        static public Stack<ConsoleColor> logColors = new Stack<ConsoleColor>();    //
        static public Stack<string> prevLogData = new Stack<string>();              //
        static Random rand = new Random();
        static public Person Hero;                                                  //ГГ
        static public Person Enemy;
        static public Person Helper = new Person('?', "Helper", 0, 0, BaseGroups.GrHelper);

        public enum Windows {main, inventory, journal, menu, skills};
        static public Windows Window = Windows.main;                                               
        static public char[,] SecondWindow = null;                                  //инвентарь||журнал и пр..

        static private System.Timers.Timer aTimer = new System.Timers.Timer();      //таймер
        static public bool isOnPause = false;                                       //на паузе ли игра
        static public bool timeDraw = true;                                         //нужно ли обновить время

        //Изменение времени
        private static void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e) 
        {
            if (!isOnPause)
                curTime = curTime.AddMinutes(1);
            timeDraw = true;
        }

        //Сохранение/Загрузка карты
        public static void DownloadMap(string filename)
        {
            if (CurrentMap != null)
            {
                foreach (MapCell cell in CurrentMap.MapObjects)
                { if(cell.Dude!=null) cell.Dude.CurHealth = -100; }
            }
            CurrentMap = new Map(filename);
            Window = 0;
            //return currentMap;
        }

        static void Main(string[] args)
        {

            GlobalProperties.SetConsoleParams();                  // устанавливаем параметры консоли
            curTime = new DateTime(2007, 6, 14, 9, 0, 0);         // устанавливаем текущую дату
            BaseGroups.FillGroups();                              // Устанавливаем отношения между группами
            Window = Windows.menu;                                // Включаем главное меню
            MainMenu.ShowMenu();

            //---------------------------------------------------------------------------Обработчик действий
            ConsoleKeyInfo key;
            KeyEvent kevt = new KeyEvent();
            int hitDirection = 0;            //0 - верх, 1 - право, 2 - низ, 3 - лево
            kevt.KeyPress += (sender, e) =>
            {
                char ch = e.key.KeyChar;

                //------------------------------------------------------------------------Пауза
                if (e.key.Key == ConsoleKey.P)
                {
                    isOnPause = !isOnPause;
                    if (isOnPause) mainDialog.SetDialog("PAUSE");
                    else mainDialog.SetDialog("You see:", CurrentMap.GetObjInfo(Hero.x, Hero.y));
                }
                if (!isOnPause && Window == 0)
                {
                    if (e.key.Key == ConsoleKey.W && e.key.Modifiers != ConsoleModifiers.Shift )
                    {
                        Hero.MoveTop();  // e.Handled = true;
                        mainDialog.SetDialog("You see:", CurrentMap.GetObjInfo(Hero.x, Hero.y));
                        hitDirection = 0;
                    }
                    else if (e.key.Key == ConsoleKey.A && e.key.Modifiers != ConsoleModifiers.Shift)
                    {
                        Hero.MoveLeft();
                        mainDialog.SetDialog("You see:", CurrentMap.GetObjInfo(Hero.x, Hero.y));
                        hitDirection = 3;
                    }
                    else if (e.key.Key == ConsoleKey.S && e.key.Modifiers != ConsoleModifiers.Shift)
                    {
                        Hero.MoveDown();
                        mainDialog.SetDialog("You see:", CurrentMap.GetObjInfo(Hero.x, Hero.y));
                        hitDirection = 2;
                    }
                    else if (e.key.Key == ConsoleKey.D && e.key.Modifiers != ConsoleModifiers.Shift)
                    {
                        Hero.MoveRight();
                        mainDialog.SetDialog("You see:", CurrentMap.GetObjInfo(Hero.x, Hero.y));
                        hitDirection = 1;
                    }

                    //--------------------------------------------------------------------Обзор
                    else if (e.key.Key == ConsoleKey.W)
                    {
                        if (Hero.y > 0)
                        { mainDialog.SetDialog("Upside you see:", CurrentMap.GetObjInfo(Hero.x, Hero.y - 1)); }
                        else mainDialog.SetDialog("This is the Edge of the World");
                    }
                    else if (e.key.Key == ConsoleKey.A)
                    {
                        if (Hero.x > 0)
                        { mainDialog.SetDialog("Leftside you see:", CurrentMap.GetObjInfo(Hero.x - 1, Hero.y)); }
                        else mainDialog.SetDialog("This is the Edge of the World");
                    }
                    else if (e.key.Key == ConsoleKey.S)
                    {
                        if (Hero.y < CurrentMap.height)
                        { mainDialog.SetDialog("Downside you see:", CurrentMap.GetObjInfo(Hero.x, Hero.y + 1)); }
                        else mainDialog.SetDialog("This is the Edge of the World");
                    }
                    else if (e.key.Key == ConsoleKey.D)
                    {
                        if (Hero.x < CurrentMap.width)
                        { mainDialog.SetDialog("Rightside:", CurrentMap.GetObjInfo(Hero.x + 1, Hero.y)); }
                        else mainDialog.SetDialog("This is the Edge of the World");
                    }

                    //-------------------------------------------------------------------Команды и способности
                    else if (e.key.Key == ConsoleKey.R)                              
                    {
                        List < Person > flag = new List<Person>();
                        Hero.Say("Back!",25);//Громкость X :  обычный слух с трудом, но услышит звук с X клеток
                        foreach (MapCell cell in CurrentMap.MapObjects)
                             if (cell.Dude != null && cell.Dude != Hero)
                                if(Math.Max(Math.Abs(Hero.x - cell.Dude.x), Math.Abs(Hero.y - cell.Dude.y)) < 10)
                                  flag.Add(cell.Dude);
                        foreach(Person man in flag)
                             if (Hero.x - man.x > 0) man.MoveLeft();
                                  else man.MoveRight();
                    }
                    else if (e.key.Key == ConsoleKey.E)
                    {
                        Hero.Say("Follow me, Bros!", 25);//Громкость X :  обычный слух с трудом, но услышит звук с X клеток
                        foreach (MapCell cell in CurrentMap.MapObjects)
                        {
                            if (cell.Dude != null && cell.Dude != Hero && Math.Max(Math.Abs(Hero.x - cell.Dude.x), Math.Abs(Hero.y - cell.Dude.y)) < 25 && Relations.relations[cell.Dude.group.number, BaseGroups.GrHero.number] == Relations.Relate.lovers) //     cell.Dude.Color == ConsoleColor.Cyan)
                            {
                                if(rand.Next(100)<10) cell.Dude.Say("АААА!", 25);
                                cell.Dude.humanAI.MoveTo(Hero.x, Hero.y);
                            }
                        }
                    }
                    //-------------------------------------------------------------------Удар
                    else if (e.key.Key == ConsoleKey.Spacebar)
                    {
                        Hero.Hit(hitDirection);
                        int hitedX = Hero.x; int hitedY = Hero.y;
                        switch (hitDirection)
                        {
                            case 0: hitedY--; break;
                            case 1: hitedX++; break;
                            case 2: hitedY++; break;
                            case 3: hitedX--; break;
                        }
                        if (CurrentMap.MapObjects[hitedY, hitedX].Dude != null)
                        {
                            Enemy = CurrentMap.MapObjects[hitedY, hitedX].Dude;
                        }
                    }
                }

                //-----------------------------------------------------------------------Действие
                if (ch == '1' || ch == '2' || ch == '3' || ch == '4' || ch == '5' || ch == '6' || ch == '7' || ch == '8' || ch == '9' || e.key.Key == ConsoleKey.Tab)
                {
                    mainDialog.prevActions = null;
                    try
                    {
                        if (e.key.Key != ConsoleKey.Tab)
                        {
                            int ikey = (int)Char.GetNumericValue(ch);
                            if (mainDialog.Actions.Count > ikey - 1)
                            {
                                bool isEnd = mainDialog.Actions[ikey - 1].isEnd;
                                mainDialog.Actions[ikey - 1].Act.Invoke();
                                e.Handled = true;
                                if (isEnd)
                                    mainDialog.SetDialog("You see:", CurrentMap.GetObjInfo(Hero.x, Hero.y));
                                //mainDialog.SetDialog(mainDialog.Actions[ikey - 1].ActEnd);
                            }
                        }
                        else
                        {
                            mainDialog.TabAction.Act.Invoke();
                            e.Handled = true;
                        }
                    }
                    catch (Exception exep)
                    { }

                }
                //-----------------------------------------------------------------------Меню
                else if (e.key.Key ==  ConsoleKey.Escape)
                {
                    isOnPause = !isOnPause;
                    if (Window != Windows.menu)
                        try
                    {
                        if (Window != 0)
                        {
                            Window = 0;
                            mainDialog.SetDialog("You see:", CurrentMap.GetObjInfo(Hero.x, Hero.y));
                        }
                        else
                                GlobalProperties.ShowMenu();
                    }
                    catch (Exception exep)
                    { }
                }
                //-----------------------------------------------------------------------Помощь
                else if (e.key.Key == ConsoleKey.F2)
                {
                    try
                    {
                        GlobalProperties.ShowHelp();
                    }
                    catch (Exception exep)
                    { }
                }
                //-----------------------------------------------------------------------Инвентарь
                else if (e.key.Key == ConsoleKey.I)
                {
                    if (Window != Windows.menu)
                        try
                    {
                        Hero.Inv.ShowObj();
                        Window = Window == Windows.inventory ? Windows.main : Windows.inventory;

                        if (Window == 0)
                            mainDialog.SetDialog("You see:", CurrentMap.GetObjInfo(Hero.x, Hero.y));
                    }
                    catch (Exception exep)
                    {
                    }
                }
                //-----------------------------------------------------------------------Журнал
                else if (e.key.Key == ConsoleKey.J)
                {
                    if (Window != Windows.menu)
                        try
                    {
                        mainDialog.SetDialog("This is your personal diary");
                        Window = Window == Windows.journal ? Windows.main : Windows.journal;

                        if (Window == 0)
                            mainDialog.SetDialog("You see:", CurrentMap.GetObjInfo(Hero.x, Hero.y));
                    }
                    catch (Exception exep)
                    {
                    }
                }
                else if (e.key.Key == ConsoleKey.K)
                {
                    if (Window != Windows.menu)
                        try
                        {
                            Hero.Perks.ShowPerks();
                            //mainDialog.SetDialog("Это список ваших способностей и характеристик");
                            Window = Window == Windows.skills ? Windows.main : Windows.skills;

                            if (Window == 0)
                                mainDialog.SetDialog("You see:", CurrentMap.GetObjInfo(Hero.x, Hero.y));
                        }
                        catch (Exception exep)
                        {
                        }
                }

            };
            //----------------------------------------------------------------------------

            Thread drawThread = new Thread(DrawInTime);
            drawThread.Start();

            aTimer.AutoReset = true;
            aTimer = new System.Timers.Timer(1000 * GlobalProperties.dayMinute / 24);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.Enabled = true;

            //Контроль нажатия клавиш
            while (true) 
            {
                key = Console.ReadKey(true);
                kevt.OnKeyPress(key);
            }

            //Обновление экрана
            void DrawInTime()
            {
                while (true)
                {
                    if (Window != Windows.menu)
                    {
                        CurrentMap.GetTopView();//Рисуем карту
                        mainGUI.DrawMenu();//Рисуем время
                        mainGUI.DrawStat();//Рисуем параметры
                    }
                        
                    mainGUI.DrawDialog(mainDialog.DialogText);//Рисуем диалог
                    mainGUI.DrawLog();//Рисуем лог

                    if (Window == 0)
                        { mainGUI.DrawField(CurrentMap, Hero); }
                        else if (Window == Windows.inventory)
                        {
                            SecondWindow = Hero.Inv.InventoryGet();
                            mainGUI.DrawOtherField();
                        }
                        else if (Window == Windows.journal)
                        {
                            SecondWindow = Hero.journal.JournalGet();
                            mainGUI.DrawOtherField();
                        }
                        else if (Window == Windows.menu)
                        {
                            SecondWindow = MainMenu.MenuGet();
                            mainGUI.DrawOtherField();
                        }
                        else if (Window == Windows.skills)
                        {
                            SecondWindow = Hero.Perks.PerkGet();
                            mainGUI.DrawOtherField();
                        }

                }
            }

        }



    }
}
