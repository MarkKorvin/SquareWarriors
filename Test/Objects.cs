using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    [Serializable()]
    public abstract class Objects
    {
        public int x;                 //Текущие координаты персонажа
        public int y;
        public char symbol;
        public bool isBlock = false;
        public string Name;           // Название предмета/имя персонажа
        public List<Acts> Methods;
        public List<Acts> Info;
        public Action show;

        //Конструктор
        public Objects(string Name, int x, int y)
        {
            this.Name = Name;
            this.x = x;
            this.y = y;
            show += ShowMethods;
            Methods = new List<Acts>();
        }

        //Общие методы
        public virtual void ShowMethods()
        {
            string text = "It's a " + Name;
            Program.mainDialog.SetDialog(text, Methods);
        }
    }

        //Интерфейсы
        interface IUsable
        {
           void Use();
        }
        interface IHealth
        {
          void TakeDamage(Person Agressor, int HitPower);
          void Heal(int HitPower);
          void Dead();
        }
        interface IContainer
        {
          void Contain(Stuff obj);
          void Take(Stuff obj);   
        }


    //===============================================Свойства земли
    [Serializable()]
    public class BackGround : Objects  
    {
        //Конструктор фоновых объектов
        public BackGround(char symbol, string Name, int x, int y,  bool isBlock) : base(Name, x, y)
        {
            Action del;
            this.isBlock = isBlock;
            this.symbol = symbol;
        }

    }

    //===============================================Свойства карманных предметов
    [Serializable()]
    public class Stuff : Objects
    {
        Action del;
        public string Type = "Simple";
        public int Quality = 0;
        public int Cost = 0;
        public Person Owner;

        //Конструкторы карманных предметов
        public Stuff(string Name, int x, int y, string Type, int Quality, int Cost) : base(Name, x, y)
        {
            symbol = '?';
            isBlock = false;
            this.Quality = Quality;
            this.Cost = Cost;
            this.Type = Type;
            Methods.Add(new Acts("Leave", del = () => { Program.mainDialog.SetDialog("You see:", Program.CurrentMap.GetObjInfo(Program.Hero.x, Program.Hero.y)); }));
            Methods.Add(new Acts("Take", del = () => { Take(Program.Hero); Program.mainDialog.SetDialog("You see:", Program.CurrentMap.GetObjInfo(Program.Hero.x, Program.Hero.y)); }));
            Methods.Add(new Acts("Info", del = () => { Program.Helper.Say("This is - "+Name+ ", Type - " + Type+ ", Quality - " + Quality+", Price - "+Cost); Program.mainDialog.SetDialog("You see:", Program.CurrentMap.GetObjInfo(Program.Hero.x, Program.Hero.y)); }));
        }
        public Stuff(string Name, Person Owner, string Type, int Quality, int Cost) : base(Name, 0, 0) // Координаты объекта, который у кого-то в рюкзаке - 0, 0
        {
            symbol = '?';
            isBlock = false;
            this.Quality = Quality;
            this.Cost = Cost;
            this.Owner = Owner;
            this.Type = Type;
            Methods.Add(new Acts("Leave", del = () => { Program.Hero.Inv.ShowObj(); }));
            Methods.Add(new Acts("Equip", del = () => { Equip(); Program.Hero.Inv.ShowObj(); }));
            Methods.Add(new Acts("Discard", del = () => { Drop(); Program.Hero.Inv.ShowObj();}));
        }
        
        //Методы работы с инвентарем
        public void Take(Person Whom)
        {
            Owner = Whom;
            if (Owner.Inv.FirstNotNull() >= 0)
            {
                Owner.Inv.Pocket[Owner.Inv.FirstNotNull()] = this;
                Program.CurrentMap.MapObjects[y, x].PocketObj.Remove(this);
             //   try
                {
                    Methods.Clear(); //RemoveAt(Methods.FindIndex(x => x.ActInfo == "Take"));
                    Methods.Add(new Acts("Leave", del = () => { Program.Hero.Inv.ShowObj(); }));
                    if(Type =="Weapon" || Type == "OffWeapon" || Type == "Dress" || Type == "Accessory") Methods.Add(new Acts("Equip", del = () => { Equip(); Program.Hero.Inv.ShowObj(); }));
                    Methods.Add(new Acts("Discard", del = () => { Drop(); Program.Hero.Inv.ShowObj(); }));
                }
             //   catch { }
            }
        }   //Надо все эти методы переделать в методы персонажа
        public void Drop()
        {
            x = Owner.x; y = Owner.y; //Owner = null;
            Program.CurrentMap.MapObjects[y, x].PocketObj.Add(this);

            if (this == Owner.Inv.RightHand)
                Owner.Inv.RightHand = null;
            else if (this == Owner.Inv.LeftHand)
                Owner.Inv.LeftHand = null;
            else
            Owner.Inv.Pocket[Array.IndexOf(Owner.Inv.Pocket,this)] = null;

            Methods.Clear();//RemoveAt(Methods.FindIndex(x => x.ActInfo == "Drop"));
            Methods.Add(new Acts("Leave", del = () => { Program.mainDialog.SetDialog("You see:", Program.CurrentMap.GetObjInfo(Program.Hero.x, Program.Hero.y)); }));
            Methods.Add(new Acts("Take", del = () => { Take(Program.Hero); Program.mainDialog.SetDialog("You see:", Program.CurrentMap.GetObjInfo(Program.Hero.x, Program.Hero.y)); }));
            Methods.Add(new Acts("Info", del = () => { Program.Helper.Say("This is - " + Name + ", Type - " + Type + ", Quality - " + Quality + ", Price - " + Cost); Program.mainDialog.SetDialog("You see:", Program.CurrentMap.GetObjInfo(Program.Hero.x, Program.Hero.y)); }));
            //  Program.SecondWindow = Program.Hero.Inv.InventoryGet();
        }   //Этот переделан
        public void Equip()
        {
            Stuff buf;
            Inventory inv = Program.Hero.Inv;
            switch (Type)
              {
                case "Weapon": buf = inv.RightHand; inv.RightHand = this; inv.Pocket[Array.IndexOf(inv.Pocket, this)] = null; buf.Take(Owner); break;
                case "OffWeapon": buf = inv.LeftHand; inv.LeftHand = this; inv.Pocket[Array.IndexOf(inv.Pocket, this)] = null; buf.Take(Owner); break;
                case "Dress": buf = inv.Dress; inv.Dress = this;  inv.Pocket[Array.IndexOf(inv.Pocket, this)] = null; buf.Take(Owner); break;
                case "Accessory": buf = inv.Accessory; inv.Accessory = this; inv.Pocket[Array.IndexOf(inv.Pocket, this)] = null; buf.Take(Owner); break;
                default: break;
            }
        }
        
    }
    //==============================================Свойства верхних предметов (крыши, листва, туман...)

    public class Roof : Objects   
    {
        public Roof(char symbol, string Name, int x, int y) : base(Name, x, y)
        {
            isBlock = false;
            this.symbol = symbol;
        }
    }

    //==============================================Свойства основных объектов
    [Serializable()]
    public class Block : Objects, IHealth
    {
        public List<Acts> MoveMethods = new List<Acts>();

        public string Type; 
        public int MaxHealth =  40;
        public int CurHealth = 0;
        Action del;

        //Конструктор объекта
        public Block(char symbol, string Name, string Type, int x, int y, bool isBlock) : base(Name, x, y)
        {
            this.isBlock = isBlock;
            this.symbol = symbol;
            this.Type = Type;
            CurHealth = MaxHealth;
            MoveMethods.Add(new Acts("Left", del = MoveLeft, true));
            MoveMethods.Add(new Acts("Right", del = MoveRight, true));
            MoveMethods.Add(new Acts("Up", del = MoveTop, true));
            MoveMethods.Add(new Acts("Down", del = MoveDown, true));
           

            if (Type == "SimpleMov")
            {
                Methods.Add(new Acts("Leave", del = () => { Program.mainDialog.SetDialog("You see:", Program.CurrentMap.GetObjInfo(Program.Hero.x, Program.Hero.y)); }));
                Methods.Add(new Acts("Move", del = Move)); 
            }
            if (Name == "TV" || Name == "Working TV")
            {
                Methods.Add(new Acts("On/Off", del = () => { this.Name = this.Name == "TV" ? this.Name = "Working TV" : "TV"; }, true));
            }
        }

        public override void ShowMethods()
        {
            string text = "This is - " + Name+". Health = "+CurHealth+"/"+MaxHealth;
            Program.mainDialog.SetDialog(text, Methods);
        }

        //Методы IHealth
        public void Heal(int healPower)
        {
            CurHealth = CurHealth + healPower > MaxHealth ? MaxHealth : CurHealth + healPower;
        }
        public void TakeDamage(Person agressor, int damage)
        {
            CurHealth -= damage;
            if (CurHealth <= 0) Dead();
        }
        public void Dead()
        {
            Program.CurrentMap.MapObjects[y, x].MainObj = null;
        }

        //Перемещение
        public void Move()
        {
            Program.mainDialog.SetDialog("Direction?", MoveMethods);
        }
        public void MoveLeft()
        {
            if (x > 0 && !Program.CurrentMap.MapObjects[y, x - 1].IsBlock())
            { Program.CurrentMap.MapObjects[y, x].MainObj = null; x--; Program.CurrentMap.MapObjects[y, x].MainObj = this; }
            else if (Program.Hero.x == x - 1 && Program.Hero.y == y && !Program.CurrentMap.MapObjects[y, x - 2].IsBlock())
            {
                Program.Hero.MoveLeft();
                Program.CurrentMap.MapObjects[y, x].MainObj = null; x--; Program.CurrentMap.MapObjects[y, x].MainObj = this;
            }
        }
        public void MoveRight()
        {
            if (x < Program.CurrentMap.MapObjects.GetLength(1) - 1 && !Program.CurrentMap.MapObjects[y, x + 1].IsBlock())
            {   Program.CurrentMap.MapObjects[y, x].MainObj = null; x++; Program.CurrentMap.MapObjects[y, x].MainObj = this;     }
            else if (Program.Hero.x == x + 1 && Program.Hero.y == y && !Program.CurrentMap.MapObjects[y, x + 2].IsBlock())
            {
                Program.Hero.MoveRight();
                Program.CurrentMap.MapObjects[y, x].MainObj = null; x++; Program.CurrentMap.MapObjects[y, x].MainObj = this;
            }
        }
        public void MoveTop()
        {
            if (y > 0 && !Program.CurrentMap.MapObjects[y - 1, x].IsBlock())
            {  Program.CurrentMap.MapObjects[y, x].MainObj = null; y--;  Program.CurrentMap.MapObjects[y, x].MainObj = this;     }
            else if (Program.Hero.x == x && Program.Hero.y == y-1 && !Program.CurrentMap.MapObjects[y-2, x].IsBlock())
            {
                Program.Hero.MoveTop();
                Program.CurrentMap.MapObjects[y, x].MainObj = null; y--; Program.CurrentMap.MapObjects[y, x].MainObj = this;
            }
        }
        public void MoveDown()
        {
            if (y < Program.CurrentMap.MapObjects.GetLength(0) - 1 && !Program.CurrentMap.MapObjects[y + 1, x].IsBlock())
            {   Program.CurrentMap.MapObjects[y, x].MainObj = null; y++; Program.CurrentMap.MapObjects[y, x].MainObj = this;     }
            else if (Program.Hero.x == x && Program.Hero.y == y + 1 && !Program.CurrentMap.MapObjects[y + 2, x].IsBlock())
            {
                Program.Hero.MoveDown();
                Program.CurrentMap.MapObjects[y, x].MainObj = null; y++; Program.CurrentMap.MapObjects[y, x].MainObj = this;
            }
        }


    }

    //=============================================Свойства персонажей
    [Serializable()]
    public class Person : Objects, IHealth
    {
        Action del;
        public AI humanAI;
        public Politics group; //Содержит цвет 
        public int level = 1;
        public int exp;
        //public ConsoleColor Color = ConsoleColor.Gray;
        
        public int MaxHealth = 100; //Изменяемые параметры
        public int CurHealth;
        public int Regeneration = 1; 
        public int Moral = 3;
        public int ViewLength = 10;

        public Inventory Inv; //Производные параметры
        public Journal journal;
        public Perk Perks;

        //Конструктор человека
        public Person(char symbol, string Name, int x, int y, Politics group) : base(Name, x, y)
        {
            this.group = group;
            this.symbol = symbol;
            CurHealth = MaxHealth;
            isBlock = true;
            Inv = new Inventory();
            journal = new Journal();
            Perks = new Perk();
            Info = new List<Acts>();
            Action del;

            if (Name != "Hero")
            {
                humanAI = new AI(this, Perks.CurAbils.Inteligence);
                humanAI.StartThinking();
            }
            Methods.Add(new Acts("Info", del = Explore));

        }

        //Боевые методы
        public int GetDamage() //Узнать урон
        { int damage = Inv.RightHand != null ? Inv.RightHand.Quality + Perks.CurAbils.Strength : Perks.CurAbils.Strength;  return damage; }
        public int GetArmor()  //Узнать броню
        { int armor = Inv.Dress != null ? Inv.Dress.Quality : 0; return armor; }
        public void Heal(int healPower)
        {
            CurHealth = CurHealth + healPower > MaxHealth ? MaxHealth : CurHealth + healPower; 
        }

        //Получение урона
        public void TakeDamage(Person agressor, int damage)
        {
            int armor = GetArmor();
            if (damage - armor > 0) // Если враг пробивает броню, то наносим урон
            {
                CurHealth -= (damage - armor);
                if (CurHealth <= 0) 
                {
                    Dead();
                    agressor.TakeExp(10 * level);
                }
            }
            if(humanAI != null)
            if (humanAI.targets.Exists(x => x.Agressor == agressor))  // Добавляем аггро против атакующего
            {
                humanAI.targets.Find(x => x.Agressor == agressor).TakedDamage += damage;
            }
            else
            {
                 humanAI.targets.Add(new Aggro(agressor, damage, 1));
            }

        }
        public void Hit(int direction)
        {
            if (this != null)
            {
                int damage = GetDamage(), hitedX = x, hitedY = y;
                switch (direction)
                {
                    case 0: hitedY--; break;
                    case 1: hitedX++; break;
                    case 2: hitedY++; break;
                    case 3: hitedX--; break;
                }
                Program.CurrentMap.GetHit(this, damage, hitedX, hitedY);
            }
         }

        public void Dead()
        {
            //Program.CurrentMap.MapObjects[y, x].PocketObj.Add(Inv.RightHand);
            try
            {
                Program.CurrentMap.MapObjects[y, x].Dude.Inv.RightHand.Drop();
            }
            catch (Exception e)
            { }
            Program.CurrentMap.MapObjects[y, x].Dude = null;

            Say("I'm dying!", 10);
        }
        
        //Нейтральные методы
        public override void ShowMethods()
        {
            string text = "This is - " + Name + ". Health = " + CurHealth+"/"+MaxHealth;
            Program.mainDialog.SetDialog(text, Methods);
        }
        public void Say(string Text, int Volume)
        {
            double length = Math.Sqrt((Program.Hero.x-x)^2 + (Program.Hero.y - y) ^ 2);
            if (Volume - length >= 0)
            {
                //if (Volume - length >= 10) { color = 0; }         //Для визуализации громкости
                //else if (Volume - length >= 5) { color = 1; }
                //else { color = 2; }
                // Program.logData.Push(color + ";" + Name + ": " + Text);

                Say(Text);
            }
        }
        public void Say(string Text)
        {
            int screenLength = GlobalProperties.GetWidth()-Name.Length -2;
            int screenLength1 = GlobalProperties.GetWidth();
            int lastLine = (Text.Length- screenLength) % screenLength1;
            int minus;

            if (Text.Length > screenLength)
            {
                minus = lastLine > 0 ? lastLine : screenLength1;
                Program.prevLogData = null;
                Program.logData.Push( Text.Substring(Text.Length - minus));
                Program.logColors.Push(group.Color);
                Say(Text.Substring(0, Text.Length - minus));
            }
            else
            {
                Program.prevLogData = null;
                Program.logData.Push(Name + ": " + Text);
                Program.logColors.Push(group.Color);
            }
        }

        //Получение опыта
        public int NeededExp()
        { return level * 100; }
        public void TakeExp(int i)
        {
            exp += i;
            while (exp >= NeededExp())
            {
                exp -= NeededExp();
                LevelUp();
            }
        }
        public void LevelUp()
        {
            level++;
            Perks.CurAbils.freePoints ++;
            Perks.CurAbils.perks[3, 3].AbilityLevel ++;
            Program.Helper.Say(Name+" achieved level "+level+"!");
        }
        
        //Для глобальных сообщений
        public void Explore()
        {
            GetInfo();
            Program.mainDialog.SetDialog(Name, Info);
        }
        public void GetInfo()
        {
            Info.Clear();
            Info.Add(new Acts("Health = " + CurHealth+"/"+MaxHealth, del = () => { }, true));
            Info.Add(new Acts("Current damage = " + GetDamage() + ", Current defence = " + GetArmor(), del = () => { }, true));
            Info.Add(new Acts("Morality = " + Moral, del = () => { }, true));
            Info.Add(new Acts("Agility = " + Perks.CurAbils.Agility + ", Strength = " + Perks.CurAbils.Strength + ", Inteligence = " + Perks.CurAbils.Inteligence + ", Charisma = " + Perks.CurAbils.Charisma, del = () => { }, true));
            Info.Add(new Acts("Experience = " + exp + "/" + NeededExp(), del = () => { }, true));
        }

        //Движение
        public void MoveLeft()
        {
            if (x > 0 && !Program.CurrentMap.MapObjects[y, x - 1].IsBlock())
            { Program.CurrentMap.MapObjects[y, x].Dude = null; x--;  Program.CurrentMap.MapObjects[y, x].Dude = this;     }
            else if (x > 0 && Program.CurrentMap.MapObjects[y, x - 1].MainObj != null && Program.CurrentMap.MapObjects[y, x - 1].MainObj.Type == "SimpleMov" && !Program.CurrentMap.MapObjects[y, x - 1].IsBlockNotByBlock()&&!Program.CurrentMap.MapObjects[y, x - 2].IsBlock())
            {   Program.CurrentMap.MapObjects[y, x].Dude = null; x--; Program.CurrentMap.MapObjects[y, x].Dude = this; Program.CurrentMap.MapObjects[y, x].MainObj.MoveLeft( );   }
        }
        public void MoveRight()
        {
            if (x < Program.CurrentMap.MapObjects.GetLength(1) - 1 && !Program.CurrentMap.MapObjects[y, x + 1].IsBlock())
            { Program.CurrentMap.MapObjects[y, x].Dude = null;  x++;  Program.CurrentMap.MapObjects[y, x].Dude = this;    }
            else if ((x < Program.CurrentMap.MapObjects.GetLength(1) - 1) && (Program.CurrentMap.MapObjects[y, x + 1].MainObj!=null && Program.CurrentMap.MapObjects[y, x + 1].MainObj.Type == "SimpleMov") && !Program.CurrentMap.MapObjects[y, x + 1].IsBlockNotByBlock() && !Program.CurrentMap.MapObjects[y, x + 2].IsBlock())
            {  Program.CurrentMap.MapObjects[y, x].Dude = null; x++; Program.CurrentMap.MapObjects[y, x].Dude = this; Program.CurrentMap.MapObjects[y, x].MainObj.MoveRight( );  }
        }
        public void MoveTop()
        {
            if (y > 0 && !Program.CurrentMap.MapObjects[y - 1, x].IsBlock())
            {  Program.CurrentMap.MapObjects[y, x].Dude = null;  y--;   Program.CurrentMap.MapObjects[y, x].Dude = this; }
            else if (y > 0 && Program.CurrentMap.MapObjects[y-1, x].MainObj != null && Program.CurrentMap.MapObjects[y - 1, x].MainObj.Type == "SimpleMov" && !Program.CurrentMap.MapObjects[y - 1, x].IsBlockNotByBlock() && !Program.CurrentMap.MapObjects[y-2, x].IsBlock())
            {   Program.CurrentMap.MapObjects[y, x].Dude = null; y--; Program.CurrentMap.MapObjects[y, x].Dude = this; Program.CurrentMap.MapObjects[y, x].MainObj.MoveTop( );   }
        }
        public void MoveDown()
        {
            if (y < Program.CurrentMap.MapObjects.GetLength(0) - 1 && !Program.CurrentMap.MapObjects[y + 1, x].IsBlock())
            {  Program.CurrentMap.MapObjects[y, x].Dude = null; y++;  Program.CurrentMap.MapObjects[y, x].Dude = this;  }
            else if (y < Program.CurrentMap.MapObjects.GetLength(0) - 1 && ( Program.CurrentMap.MapObjects[y+1, x ].MainObj != null && Program.CurrentMap.MapObjects[y+1, x ].MainObj.Type == "SimpleMov") && !Program.CurrentMap.MapObjects[y+1, x].IsBlockNotByBlock() && !Program.CurrentMap.MapObjects[y+2, x].IsBlock())
            {   Program.CurrentMap.MapObjects[y, x].Dude = null; y++; Program.CurrentMap.MapObjects[y, x].Dude = this; Program.CurrentMap.MapObjects[y, x].MainObj.MoveDown( );   }
        }
    }
}
