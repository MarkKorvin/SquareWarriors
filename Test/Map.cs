using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;


namespace Test
{
    [Serializable()]
    public class Map
    {
        public string name;
        public int height;
        public int width;
        public char[,] mapField;
        public Cell[,] MapObjects;
        static private System.Timers.Timer aTimer = new System.Timers.Timer();
       

        [Serializable()]
        public struct Cell //Структура одной ячейки, из которых состоит карта
        {
            public BackGround BG;           //В ячейке есть фон
            public List<Stuff> PocketObj;   // Есть набор переносных предметов
            public Block MainObj ;          // Один большой объект
            public Roof TopObject;          // Маскирующий объект
            public Person Dude;             // И один человек
            public bool hited;              //true когда по области ударили

            public List<Acts> InThere()     //Содержимое ячейки
            {
                List<Acts> Acts = new List<Acts>();

                Acts.Add(new Acts(BG.Name, BG.show));
                if (Dude != null)
                {
                    Acts.Add(new Acts(Dude.Name, Dude.show));
                }
                if (TopObject != null)
                {
                    Acts.Add(new Acts(TopObject.Name, TopObject.show));
                }
                if (MainObj != null)
                {
                    Acts.Add(new Acts(MainObj.Name, MainObj.show));
                }
                if (PocketObj != null)
                    foreach (Stuff x in PocketObj)
                    {
                        Acts.Add(new Acts(x.Name, x.show));
                    }
                return Acts;
            }
            public int ElemNumber()         //Количество объектов в ячейке.
            {
                int length = 1;
                if (Dude != null)
                { length++; }
                if (TopObject != null)
                { length++; }
                if (MainObj != null)
                { length++; }
                if (PocketObj != null)
                    foreach (Stuff x in PocketObj)
                    { length++; }
                return length;
            }
            public bool IsBlock()           //Является ли ячейка заблокированной
            {
                if ((BG!=null && BG.isBlock) || (MainObj!=null && MainObj.isBlock) || (Dude!=null && Dude.isBlock))
                    return true;
                else
                    return false;
            }
            public bool IsBlockNotByBlock() //Если она заблокирована не объектом
            {
                if ((BG != null && BG.isBlock) || (Dude != null && Dude.isBlock))
                    return true;
                else
                    return false;
            }   

        }
        
        //=========================================================================================== Код самой карты

        //Конструкторы карты
        public Map(string fileName) // Создать карту с помощью файла
        {
            GetMapFromFile(fileName);

            aTimer.AutoReset = true;
            aTimer = new System.Timers.Timer(150);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.Enabled = true;
        }
        
        //Вспомогательные методы для генерации карты
        public void GetMapFromFile(string fileName) //Метод читает указанный файл и переводит его в массив объектов
        {

            string[] lines = File.ReadAllLines("Maps//"+fileName);

            height = 0;
            for (int k = 0; k < lines.Length; k++) //Определяем высоту карты. Карта должна заканчиваться кодовым словом <objects>
            {
                if (lines[k] == "<objects>")
                { height = k; break; }
            }

            if (height == 0)
            { height = lines.Length; }
            width = lines[0].Length;

            char[,] curField = new char[height, width]; //Создаем основной массив символов

            for (int k = 0; k< height; k++)
            {
                char[] tempArray = lines[k].ToCharArray();
                for (int j = 0; j < width; j++)
                {
                    curField[k, j] = lines[k][j];
                }
            }

            MapObjects = new Cell[height, width];

            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    MapObjects[i, j].BG = new BackGround(' ', "Ground",  j, i, false);
                    SymbolToObject(curField[i, j], j, i);           
                }

            mapField = curField;

            MapObjects[Program.Hero.y, Program.Hero.x].Dude = Program.Hero;

            Program.Helper.Say("Load was succesfully");
        }

        public void SymbolToObject(char Sym, int x, int y) //Перевод символа в объект. В таблице ниже перечислены все известные объекты (Кроме переносимых, у которых одинаковый символ)
        {
            MapObjects[y, x].PocketObj = new List<Stuff> { };
            switch (Sym)
            {
                //============================================================================================= Недвижимые объекты
                //Список типов - "SimpleMov", "SimStable"
                case '╔': MapObjects[y, x].MainObj = new Block(Sym, "Wall", "SimStable", x, y, true); break;    
                case '╦': MapObjects[y, x].MainObj = new Block(Sym, "Wall", "SimStable", x, y, true); break;
                case '╗': MapObjects[y, x].MainObj = new Block(Sym, "Wall", "SimStable", x, y, true); break;
                case '╠': MapObjects[y, x].MainObj = new Block(Sym, "Wall", "SimStable", x, y, true); break;
                case '╬': MapObjects[y, x].MainObj = new Block(Sym, "Wall", "SimStable", x, y, true); break;
                case '╣': MapObjects[y, x].MainObj = new Block(Sym, "Wall", "SimStable", x, y, true); break;
                case 'Ƹ': MapObjects[y, x].MainObj = new Block(Sym, "Wall", "SimStable", x, y, true); break;
                case '╚': MapObjects[y, x].MainObj = new Block(Sym, "Wall", "SimStable", x, y, true); break;
                case '╩': MapObjects[y, x].MainObj = new Block(Sym, "Wall", "SimStable", x, y, true); break;
                case '╝': MapObjects[y, x].MainObj = new Block(Sym, "Wall", "SimStable", x, y, true); break;
                case '═': MapObjects[y, x].MainObj = new Block(Sym, "Wall", "SimStable", x, y, true); break;
                case '║': MapObjects[y, x].MainObj = new Block(Sym, "Wall", "SimStable", x, y, true); break;
                case '│': MapObjects[y, x].MainObj = new Block(Sym, "Little Wall", "SimStable", x, y, true); break;
                case '┐': MapObjects[y, x].MainObj = new Block(Sym, "Little Wall", "SimStable", x, y, true); break;
                case '─': MapObjects[y, x].MainObj = new Block(Sym, "Little Wall", "SimStable", x, y, true); break;
                case '‡': MapObjects[y, x].MainObj = new Block(Sym, "Hedge", "SimStable", x, y, true); break;
                case 'W': MapObjects[y, x].MainObj = new Block(Sym, "Wood", "SimStable", x, y, true); break;
                case 'O': MapObjects[y, x].MainObj = new Block(Sym, "Window", "SimStable", x, y, true); break;
                case '▓': MapObjects[y, x].MainObj = new Block(Sym, "Table", "SimStable", x, y, true); break;
                case '▒': MapObjects[y, x].MainObj = new Block(Sym, "Sitting Place", "SimStable", x, y, false); break;
                case '█': MapObjects[y, x].MainObj = new Block(Sym, "Furniture", "SimStable", x, y, true); break;
                case '▄': MapObjects[y, x].MainObj = new Block(Sym, "Furniture", "SimStable", x, y, true); break;
                case '¤': MapObjects[y, x].MainObj = new Block(Sym, "TV", "SimStable", x, y, true); break;
                //============================================================================================= Движимые объекты
                case 'h': MapObjects[y, x].MainObj = new Block(Sym, "Chair", "SimpleMov", x, y, true); break;
                case '■': MapObjects[y, x].MainObj = new Block(Sym, "Box", "SimpleMov", x, y, true); break;
                //============================================================================================= Персонажи
                case 'X': Program.Hero = new Person(Sym, "Hero", x, y, BaseGroups.GrHero); break;
                case 'Y': MapObjects[y, x].Dude = new Person(Sym, "Friend", x, y, BaseGroups.GrFriends); MapObjects[y, x].Dude.Inv.RightHand = new Stuff("Stick", MapObjects[y, x].Dude, "Weapon", 2, 1); break;
                case 'Z': MapObjects[y, x].Dude = new Person(Sym, "Enemy", x, y, BaseGroups.GrEnemies); MapObjects[y, x].Dude.Inv.RightHand = new Stuff("Stick", MapObjects[y, x].Dude, "Weapon", 2, 1); break;

                //============================================================================================= Фоновые объекты
                case ' ': MapObjects[y, x].BG = new BackGround(Sym, "Ground",  x, y, false); break;             
                case '«': MapObjects[y, x].BG = new BackGround(Sym, "Grass",  x, y, false); break;
                case '░': MapObjects[y, x].BG = new BackGround(Sym, "Water",  x, y, true); break;
                case 'Ж': MapObjects[y, x].BG = new Home(Sym, "Closed Door", x, y, true, true,new List<Person>(){ new Person('Y', "Vano", x, y, BaseGroups.GrFriends), new Person('Y', "Denchik", x, y, BaseGroups.GrFriends) }); break;
                case 'Щ': MapObjects[y, x].BG = new Door(Sym, "Closed Door", x, y, true, true); break;
                case 'Ш': MapObjects[y, x].BG = new Door(Sym, "Closed Door", x, y, true, false); break;
                //case 'П': MapObjects[y, x].BG = new Door(Sym, "Opened Door", x, y, false); break;
                case '<': MapObjects[y, x].BG = new BackGround(Sym, "Automatic Door", x, y, false); break;
                case '>': MapObjects[y, x].BG = new BackGround(Sym, "Automatic Door", x, y, false); break;
                case 'u': MapObjects[y, x].BG = new BackGround(Sym, "Stair", x, y, false); break;
                //============================================================================================= Маскировочные, верхние объекты
                case 'z': MapObjects[y, x].TopObject = new Roof(Sym, "Leafage",  x, y); break;

                default: MapObjects[y, x].PocketObj.Add(new Stuff("Unindent.", x, y, "Simple", 0, 0)); break;

            }        
        }


        //Получение информации о карте
        public void GetTopView() // По массиву объектов получаем вид сверху
        {
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    if (MapObjects[i, j].TopObject != null)
                        mapField[i, j] = MapObjects[i, j].TopObject.symbol;
                    else if (MapObjects[i, j].Dude != null)
                        mapField[i, j] = MapObjects[i, j].Dude.symbol;
                    else if (MapObjects[i, j].MainObj != null)
                        mapField[i, j] = MapObjects[i, j].MainObj.symbol;
                    else if (MapObjects[i, j].PocketObj.Count > 0)
                        mapField[i, j] = MapObjects[i, j].PocketObj.Last().symbol;
                    else
                        mapField[i, j] = MapObjects[i, j].BG.symbol;
        }
        public List<Acts> GetObjInfo(int x, int y)
        {
            List<Acts> ObjAround = MapObjects[y, x].InThere();
            Program.isOnPause = false;
            return ObjAround;
        }

        //Эффекты ячейки
        public void GetHit(Person agressor, int damage, int x, int y)
        {
            if (MapObjects[y, x].Dude != null)
                MapObjects[y, x].Dude.TakeDamage(agressor, damage);
            if (MapObjects[y, x].MainObj != null)
                MapObjects[y, x].MainObj.TakeDamage(agressor, damage);

            MapObjects[y, x].hited = true;

            //Thread Hit = new Thread(() => { MapObjects[y, x].hited = true; Thread.Sleep(50); MapObjects[y, x].hited = false; });
            //if (MapObjects[y, x].hited == false) 
            //    Hit.Start();
        } //Ячейка под ударом

        private static void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            for (int i = 0; i < Program.CurrentMap.height; i++)
                for (int j = 0; j < Program.CurrentMap.width; j++)
                    if(Program.CurrentMap.MapObjects[i, j].hited == true)
                    Program.CurrentMap.MapObjects[i, j].hited = false;
        }
    }
}
