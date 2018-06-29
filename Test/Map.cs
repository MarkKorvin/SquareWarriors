using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Collections;

namespace Test
{
    [Serializable()]
    public class Map
    {
        public string name;
        public int height;
        public int width;
        public char[,] mapField;
        public MapCell[,] MapObjects;
        static private System.Timers.Timer aTimer = new System.Timers.Timer();
        public MapEditor editor;

 
        //Конструкторы карты
        public Map(string fileName) // Создать карту с помощью файла
        {
            editor = new MapEditor();
            GetMapFromFile(fileName);

            aTimer.AutoReset = true;
            aTimer = new System.Timers.Timer(150);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.Enabled = true;
        }

        //Вспомогательные методы для генерации карты
        public void GetMapFromFile(string fileName) //Метод читает указанный файл и переводит его в массив объектов
        {
            string[] lines = File.ReadAllLines("Maps//" + fileName);

            width = lines[0].Length;
            height = 0;
            for (int k = 0; k < lines.Length; k++) //Определяем высоту карты. Карта должна заканчиваться кодовым словом <objects>
            {
                if (lines[k] == "<objects>")
                { height = k; break; }
                height++;
            }

            if (height < lines.Length)//Заполняем всспомогательную таблицу объектов (добавляем объекты из редактора)
            {
                for (int i = height + 1; i < lines.Length; i++)
                {
                    if(lines[i].Count()>1)
                    editor.LineToMas(lines[i]);
                }
            }
            editor.UniteTabs();

            char[,] curField = new char[height, width]; //Создаем основной массив символов

            MapObjects = new MapCell[height, width];
            for (int k = 0; k < height; k++) //Переводим символы по всей высоте
            {
                char[] tempArray = lines[k].ToCharArray();
                for (int j = 0; j < width; j++)
                {
                    curField[k, j] = lines[k][j];
                    MapObjects[k, j] = new MapCell();
                    MapObjects[k, j].BG = new BackGround(' ', "Ground", j, k, false);
                    SymbolToObject(curField[k, j], j, k);
                }
            }
            mapField = curField;
            Program.Helper.Say("Load was succesfully");
        }



        public void SymbolToObject(char Sym, int x, int y) //Перевод символа в объект. В таблице ниже перечислены все известные объекты (Кроме переносимых, у которых одинаковый символ)
        {
            MapObjects[y, x].PocketObj = new List<Stuff> { };

            if (editor.baseSymTable.ContainsKey(Sym) && editor.baseSymTable[Sym] != null)
            {
                Objects obj = editor.baseSymTable[Sym];
                obj.x = x;
                obj.y = y;

                if (obj is Hero)
                {
                    Program.Hero = (Hero)obj;
                    MapObjects[y, x].Dude = (Hero)obj;
                }
                if (obj is Block)
                    MapObjects[y, x].MainObj = AddObject<Block>(obj);
                else if (obj is Person && !(obj is Hero))
                {
                    MapObjects[y, x].Dude = AddObject<Person>(obj);
                    MapObjects[y, x].Dude.humanAI.StartThinking();
                }

                else if (obj is BackGround)
                    MapObjects[y, x].BG = AddObject<BackGround>(obj);
                else if (obj is Door)
                    MapObjects[y, x].BG = AddObject<Door>(obj);
                else if (obj is Door)
                    MapObjects[y, x].BG = AddObject<Home>(obj);
                else if (obj is Roof)
                    MapObjects[y, x].TopObject = AddObject<Roof>(obj);
                else if (obj is Stuff)
                    MapObjects[y, x].PocketObj.Add(AddObject<Stuff>(obj)); 
            }
            else
            { MapObjects[y, x].PocketObj.Add(new Stuff("Unindent.", x, y, "Simple", 0, 0)); }

        }

        public T AddObject<T>(Objects obj) where T: Objects
        {
            return (T)(obj).DeepCopy();
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
            MapObjects[y, x].Dude?.TakeDamage(agressor, damage);
            MapObjects[y, x].MainObj?.TakeDamage(agressor, damage);

            MapObjects[y, x].hited = true;

        } //Ячейка под ударом

        private static void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            for (int i = 0; i < Program.CurrentMap.height; i++)
                for (int j = 0; j < Program.CurrentMap.width; j++)
                    if (Program.CurrentMap.MapObjects[i, j].hited == true)
                        Program.CurrentMap.MapObjects[i, j].hited = false;
        }
    }










    [Serializable()]
    public class MapCell
    {
        public BackGround BG;           //В ячейке есть фон
        public List<Stuff> PocketObj;   // Есть набор переносных предметов
        public Block MainObj;          // Один большой объект
        public Roof TopObject;          // Маскирующий объект
        public Person Dude;             // И один человек

        public bool hited;              //true когда по области ударили

        public IEnumerator GetEnumerator()
        {
            List<Object> objects = new List<Object>();
            objects.Add(BG);
            objects.Add(PocketObj);
            objects.Add(MainObj);
            objects.Add(TopObject);
            objects.Add(Dude);
            return objects.GetEnumerator();
        }

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

       
        public bool IsBlock()           //Является ли ячейка заблокированной
        {
            if ((BG != null && BG.isBlock) || (MainObj != null && MainObj.isBlock) || (Dude != null && Dude.isBlock))
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
}
