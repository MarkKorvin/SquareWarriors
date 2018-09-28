using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


namespace Test
{
    [Serializable()]
    public class AI
    {
        static public System.Timers.Timer aTimer = new System.Timers.Timer();      //таймер

        Person Man;                                             //Человек, которому принадлежит данный АИ
        int SmartLevel;                                         //Уровень интеллекта
        int Reaction = 200;                                     //Задержка между действиями(мсек)
        enum Patterns {SeekAndDestroy, Rest, Explore, Saving}  //Перечисление возможных режимов мышления
        Patterns CurPattern;                                    //Текущий режим
        Queue<int> Way = new Queue<int>();                      //Путь, если нужно идти
        public List<Aggro> targets = new List<Aggro>();         //Цели, если нужно сражаться

           
        public AI(Person Man, int SmartLevel)                   //Конструктор интеллекта
        {
            this.Man = Man;
            this.SmartLevel = SmartLevel;
        }

        public void StartThinking()                             //Запуск работы АИ
        {
                aTimer.AutoReset = true;
                aTimer = new System.Timers.Timer(Reaction);
                aTimer.Elapsed += OnTimedEvent;
                aTimer.Enabled = true;
        }

        public void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e) //По таймеру человек думает и делает дела
        {
            if (Program.isOnPause == false && Program.Window == 0 && Man.CurHealth > 0 )
            {
                //Man.Heal(Man.Regeneration); // Можно конечно создать дополнительный таймер, но пусть пока будет так
                
               // Thread Warden = new Thread(() => { WardenStyle(); });
               // Warden.Start();//Промеряем местность на наличие врагов
                WardenStyle();
                if (targets.Count > 0)
                CurPattern = Patterns.SeekAndDestroy;

                if (CurPattern == Patterns.SeekAndDestroy)
                { Fight(); }
                if (CurPattern == Patterns.Rest)
                {}
                if (CurPattern == Patterns.Explore)
                {}

                Moving();

            }
        }

        //Методы движения к цели
        public void Moving()    //Непосредственное движение
        {
            try
            {
                if (Way.Count() > 0)//Если есть путь
                {
                    int direction = Way.Dequeue();
                    switch (direction)
                    {
                        case 0: Man.MoveTop(); break;
                        case 1: Man.MoveRight(); break;
                        case 2: Man.MoveDown(); break;
                        case 3: Man.MoveLeft(); break;
                    }
                }
            }
            catch (Exception e) { }
        }
        public void MoveTo(int x, int y)    //Поиск пути в отдельном потоке (Какая-то хрень. Кто-то его очень часто вызывает)
        {
                Thread WaySearching = new Thread(() => { A(x, y); });
                WaySearching.Start();
        }



        public Cell FindWay(int x, int y)
        {
            Stack<Cell> WayMap = new Stack<Cell>();

            Cell Start = new Cell(Man.x, Man.y);
            Cell Goal = new Cell(x, y);
            Cell current;
            Cell key = null;

            Queue<Cell> Frontier = new Queue<Cell>();
            Dictionary<Cell, Cell> CameFrom = new Dictionary<Cell, Cell>();
            CameFrom.Add(Start, null);

            Frontier.Enqueue(Start);

            //Проходим по фронту движения, смотрим куда можно двинуться (Потом нужно уменьшить до области видимости) 
            while (Frontier.Count() > 0)
            {
                try
                {
                    current = Frontier.Dequeue();
                    if (current.x == Goal.x && current.y == Goal.y) //Если нашли путь - идем
                    {
                        key = current;
                        break;
                    }

                    else
                    {
                        foreach (Cell next in current.neighbors(Goal)) //Смотрим все соседние точки
                        {
                            if (UnicCell(CameFrom, next))    //Если еще не рассматривали точку
                            {
                                Frontier.Enqueue(next);      //Добавляем ее в список
                                CameFrom.Add(next, current);
                            }
                        }
                    }
                }
                catch (Exception e) { };
            }

            return key;
        }



        public void A(int x,int y)   //Алгоритм поиска пути к цели
        {
            Stack<Cell> WayMap = new Stack<Cell>();

            Cell Start = new Cell(Man.x, Man.y);
            Cell Goal = new Cell(x, y);
            Cell current;
            Cell key = null;

            Queue<Cell> Frontier = new Queue<Cell>();
            Dictionary<Cell, Cell> CameFrom = new Dictionary<Cell, Cell>();
            CameFrom.Add(Start, null);

            Frontier.Enqueue(Start);

            //Проходим по фронту движения, смотрим куда можно двинуться (Потом нужно уменьшить до области видимости) 
            while (Frontier.Count() > 0)
            {
                try
                {
                    current = Frontier.Dequeue();
                    if (current.x == Goal.x && current.y == Goal.y) //Если нашли путь - идем
                    {
                        key = current;
                        break;
                    }

                    else
                    {
                        foreach (Cell next in current.neighbors(Goal)) //Смотрим все соседние точки
                        {
                            if (UnicCell(CameFrom, next))    //Если еще не рассматривали точку
                            {
                                Frontier.Enqueue(next);      //Добавляем ее в список
                                CameFrom.Add(next, current);
                            }
                        }
                    }
                }
                catch (Exception e) { };
            }

            this.Way.Clear();

            if (key != null)//Если смогли найти путь
            {

                while (key != Start) //Записываем путь в стек
                {
                    WayMap.Push(CameFrom[key]);
                    key = CameFrom[key];
                }

                Queue<int> Way = new Queue<int>(); //Переписываем координаты в последовательность шагов
                Cell pos = new Cell(Man.x, Man.y);
                while (WayMap.Count > 0)
                {
                    Cell newpos = WayMap.Pop();
                    int xlong = pos.x - newpos.x;
                    int ylong = pos.y - newpos.y;
                    if (xlong < 0) Way.Enqueue(1);
                    else if (xlong > 0) Way.Enqueue(3);
                    else if (ylong < 0) Way.Enqueue(2);
                    else if (ylong > 0) Way.Enqueue(0);
                    pos = newpos;
                }

                this.Way = Way; //Записываем найденный путь
            }

        } 
        public bool UnicCell(Dictionary<Cell, Cell> CameFrom, Cell Cur) //Проверка на уникальность ячейки в списке
        {
            foreach (Cell key in CameFrom.Keys)
            {
                if (key.x == Cur.x && key.y == Cur.y)
                    return false;
            }
            return true;
        }

        //Методы ведения боя
        public void WardenStyle()
        {
            Map map = Program.CurrentMap;
            int leftx = Man.x - Man.ViewLength;
            int topy = Man.y - Man.ViewLength;

            //targets = new List<Aggro>();
            try
            {
                if (targets.Count == 0)
                {

                    for (int i = Math.Max(topy,0); i < Math.Min( topy + 2 * Man.ViewLength + 1, map.mapField.GetLength(0)); i++)
                    {
                        for (int j = Math.Max(leftx,0); j < Math.Min(leftx + 2 * Man.ViewLength + 1, map.mapField.GetLength(1)); j++)
                        {
                            if (map.MapObjects[i, j].Dude != null && map.MapObjects[i, j].Dude.CurHealth > 0)
                                if (Relations.relations[map.MapObjects[i, j].Dude.group.number, Man.group.number] == Relations.Relate.haters)
                                {
                                    if(FindWay(map.MapObjects[i, j].Dude.x, map.MapObjects[i, j].Dude.y)!=null)
                                    targets.Add(new Aggro(map.MapObjects[i, j].Dude, 0, 1));
                                }
                        }
                    }
                }
                else
                {
                    foreach (Aggro agg in targets)
                    {
                        if (agg.Agressor.CurHealth < 0 || Math.Max(Math.Abs(Man.x - agg.Agressor.x), Math.Abs(Man.y - agg.Agressor.y)) > Man.ViewLength)
                            targets.Remove(agg);
                    }
                }
            }
            catch (Exception exep) { }
        }
        public void Fight()                         //Нанесение урона врагу в радиусе атаки
        {
            if (targets.Count > 0)//Если есть цели
            {
                Aggro Target = dangerest(targets);
                MoveTo(Target.Agressor.x, Target.Agressor.y);

                if (Math.Abs(Target.Agressor.x - Man.x) + Math.Abs(Target.Agressor.y - Man.y) == 1)
                {
                    if (Target.Agressor.CurHealth > 0 && Man.CurHealth > 0)    
                        Program.CurrentMap.GetHit(Man, Man.GetDamage(), Target.Agressor.x, Target.Agressor.y); //переделать в Man.Hit
                    else
                        targets.Remove(Target);
                }
                
            }
        }

        public Aggro dangerest(List<Aggro> targets) //Поиск самого опасного врага в списке
        {
            Aggro Target = targets.First();
            foreach (Aggro target in targets)
            {
                if (target.TakedDamage > Target.TakedDamage)
                    Target = target;
            }
            return Target;
        }
       
    }

    [Serializable()]
    public class Aggro  //собирательный Класс элементов, влияющих на аггро 
    {
        public Person Agressor;
        public int TakedDamage;
        int dangerous;

        public  Aggro(Person Agressor, int TakedDamage, int dangerous)
        {
            this.Agressor = Agressor;
            this.TakedDamage = TakedDamage;
            this.dangerous = dangerous;
        }
    }
    [Serializable()]
    public class Cell   //Класс узла графа для поиска маршрута
    {
      public int x;
      public int y;

        public Cell(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public List<Cell> neighbors(Cell Goal)
        {
            try
            {
                List<Cell> neighbor = new List<Cell>();
                if (!Program.CurrentMap.MapObjects[y, x + 1].IsBlock() || (y == Goal.y && x + 1 == Goal.x))
                    neighbor.Add(new Cell(x + 1, y));
                if (!Program.CurrentMap.MapObjects[y + 1, x].IsBlock() || (y + 1 == Goal.y && x == Goal.x))
                    neighbor.Add(new Cell(x, y + 1));
                if (!Program.CurrentMap.MapObjects[y, x - 1].IsBlock() || (y == Goal.y && x - 1 == Goal.x))
                    neighbor.Add(new Cell(x - 1, y));
                if (!Program.CurrentMap.MapObjects[y - 1, x].IsBlock() || (y - 1 == Goal.y && x == Goal.x))
                    neighbor.Add(new Cell(x, y - 1));
                return (neighbor);
            }
            catch(Exception e) { return null; }
        }
    }

}
