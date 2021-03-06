﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

//=============================================================================================
namespace Test
{
    public class  MapEditor
    {
        
        public Dictionary<char,Objects> baseSymTable = new Dictionary<char, Objects>();
        public Dictionary<char, Objects> missionSymTable = new Dictionary<char, Objects>();

        private Objects[] objects = new Objects[100];
        private int count = 0;

        public MapEditor()
        {
            baseSymTable.Add('╔', new Block('╔', "Wall", "SimStable", 0, 0, true));
            baseSymTable.Add('╦', new Block('╦', "Wall", "SimStable", 0, 0, true));
            baseSymTable.Add('╗', new Block('╗', "Wall", "SimStable", 0, 0, true));
            baseSymTable.Add('╠', new Block('╠', "Wall", "SimStable", 0, 0, true));
            baseSymTable.Add('╬', new Block('╬', "Wall", "SimStable", 0, 0, true));
            baseSymTable.Add('╣', new Block('╣', "Wall", "SimStable", 0, 0, true));
            baseSymTable.Add('Ƹ', new Block('Ƹ', "Wall", "SimStable", 0, 0, true));
            baseSymTable.Add('╚', new Block('╚', "Wall", "SimStable", 0, 0, true));
            baseSymTable.Add('╩', new Block('╩', "Wall", "SimStable", 0, 0, true));
            baseSymTable.Add('╝', new Block('╝', "Wall", "SimStable", 0, 0, true));
            baseSymTable.Add('═', new Block('═', "Wall", "SimStable", 0, 0, true));
            baseSymTable.Add('║', new Block('║', "Wall", "SimStable", 0, 0, true));
            baseSymTable.Add('│', new Block('│', "Little Wall", "SimStable", 0, 0, true));
            baseSymTable.Add('┐', new Block('┐', "Little Wall", "SimStable", 0, 0, true));
            baseSymTable.Add('─', new Block('─', "Little Wall", "SimStable", 0, 0, true));
            baseSymTable.Add('‡', new Block('‡', "Hedge", "SimStable", 0, 0, true));
            baseSymTable.Add('W', new Block('W', "Wood", "SimStable", 0, 0, true));
            baseSymTable.Add('O', new Block('O', "Window", "SimStable", 0, 0, true));
            baseSymTable.Add('▓', new Block('▓', "Table", "SimStable", 0, 0, true));
            baseSymTable.Add('▒', new Block('▒', "Sitting Place", "SimStable", 0, 0, false));
            baseSymTable.Add('█', new Block('█', "Furniture", "SimStable", 0, 0, true));
            baseSymTable.Add('▄', new Block('▄', "Furniture", "SimStable", 0, 0, true));
            baseSymTable.Add('¤', new Block('¤', "TV", "SimStable", 0, 0, true));
            //============================================================================================= Движимые объекты
            baseSymTable.Add('h', new Block('h', "Chair", "SimpleMov", 0, 0, true));
            baseSymTable.Add('■', new Block('■', "Box", "SimpleMov", 0, 0, true));
            //============================================================================================= Персонажи
            baseSymTable.Add('X', new Hero('X', "Hero", 0, 0, BaseGroups.GrHero));
            //baseSymTable.Add('Y', new Person('Y', "Friend", 0, 0, BaseGroups.GrFriends));
            //baseSymTable.Add('Z', new Person('Z', "Enemy", 0, 0, BaseGroups.GrEnemies));

            //============================================================================================= Фоновые объекты
            baseSymTable.Add(' ', new BackGround(' ', "Ground", 0, 0, false));
            baseSymTable.Add('«', new BackGround('«', "Grass", 0, 0, false));
            baseSymTable.Add('░', new BackGround('░', "Water", 0, 0, true));
            baseSymTable.Add('Ж', new Home('Ж', "Closed Door", 0, 0, true, true, new List<Person>() { new Person('Y', "Friend", 0, 0, BaseGroups.GrFriends), new Person('Y', "Friend", 0, 0, BaseGroups.GrFriends) }));
            baseSymTable.Add( 'Щ',   new Door('Щ', "Closed Door", 0, 0, true, true));
            baseSymTable.Add( 'Ш',   new Door('Ш', "Closed Door", 0, 0, true, false));
 
            baseSymTable.Add( '<',   new BackGround('<', "Automatic Door", 0, 0, false));
            baseSymTable.Add( '>',   new BackGround('>', "Automatic Door", 0, 0, false));
            baseSymTable.Add( 'u',   new BackGround('u', "Stair", 0, 0, false));
            //============================================================================================= Маскировочные, верхние объекты
            baseSymTable.Add( 'z',  new Roof('z', "Leafage", 0, 0));

        }


        public void LineToMas(string line)
        {
            List<string> str = line.Split(' ').ToList();
            List<string> str2 = str.GetRange(0,str.Count) ;

            foreach (string word in str)
                if (word == "^")
                    count++;

            str2.RemoveRange(0,count+4);
            AddToTable(str[count], str[count+1].ToCharArray()[0], str[count+2].ToCharArray()[0], str[count+3], str2);
        }


        public void AddToTable(string type, char sym, char realSym, string Name, List<string> data)
        {
            switch (type)
            {
                case "Person": { if (count == 0) Array.Clear(objects, 0, objects.Length); AddPerson(sym, realSym, Name, data); break; }
                case "Block": { if (count == 0) Array.Clear(objects, 0, objects.Length); AddBlock(sym, realSym, Name, data);  break; }
                case "BG": { if (count == 0) Array.Clear(objects, 0, objects.Length); AddBG(sym, realSym, Name, data);  break; }
                case "Stuff": { if (count == 0) Array.Clear(objects, 0, objects.Length); AddStuff(sym, realSym, Name, data); break; }

            }
        }

        //--------------------------------------------------------

        public void AddBlock(char sym, char realSym, string Name, List<string> data) //Добавление блока
        {
            Block block = new Block(realSym, Name, "SimStable", 0, 0, true);

            foreach (string tag in data)
                switch(tag)
                    {
                    case ("SimStable"): block.Type = tag; break;
                    case ("SimpleMov"): block.Type = tag; break;
                    case ("NotBlock"): block.isBlock = false; break;

                    case var someVal when new Regex(@"Health").IsMatch(tag): block.MaxHealth = GetNumeric(tag); break;

                    default: break;
                }
            
            missionSymTable.Add(sym, block);
            objects[count] = block;
            count = 0;
        }

        //--------------------------------------------------------

        public void AddPerson(char sym, char realSym, string Name, List<string> data)//Добавление персонажа
        {
            Person person = new Person(realSym, Name, 0, 0, BaseGroups.GrFriends);

                foreach (string tag in data)
                    switch (tag)
                    {
                        case ("Friend"): person.group = BaseGroups.GrFriends; break;
                        case ("Enemy"): person.group = BaseGroups.GrEnemies; break;

                        case var someVal when new Regex(@"Health").IsMatch(tag): person.MaxHealth = GetNumeric(tag); break;

                        case var someVal when new Regex(@"Charisma").IsMatch(tag): person.Perks.CurAbils.Charisma = GetNumeric(tag); break;
                        case var someVal when new Regex(@"Agility").IsMatch(tag): person.Perks.CurAbils.Agility = GetNumeric(tag); break;
                        case var someVal when new Regex(@"Inteligence").IsMatch(tag): person.Perks.CurAbils.Inteligence = GetNumeric(tag); break;
                        case var someVal when new Regex(@"Strength").IsMatch(tag): person.Perks.CurAbils.Strength = GetNumeric(tag); break;

                        case var someVal when new Regex(@"Level").IsMatch(tag):
                            int lvl = 1;
                            int newLvl = GetNumeric(tag);
                            while (lvl < newLvl)
                            {
                                person.LevelUp(false);
                                lvl++;
                            }
                            break;

                        default: break;
                    }

            if (count == 0)
                { missionSymTable.Add(sym, person); }

            else if (objects[count - 1] is BackGround)
            {
                ((Home)objects[count - 1]).AddDweller(person);
            }

            objects[count] = person;
            count = 0;
        }
        //--------------------------------------------------------

        public void AddStuff(char sym, char realSym, string Name, List<string> data)//Добавление переносного объекта
        {
            Stuff stuff = new Stuff(Name, 0, 0, "Simple", 0, 0);

            foreach (string tag in data)
                switch (tag)
                {
                    case var someVal when new Regex(@"Quality").IsMatch(tag): stuff.Quality = GetNumeric(tag); break;
                    case var someVal when new Regex(@"Cost").IsMatch(tag): stuff.Cost = GetNumeric(tag); break;
                    case "Weapon": stuff.Type = tag; break;
                    case "OffWeapon": stuff.Type = tag; break;
                    case "Dress": stuff.Type = tag; break;
                    case "Accessory": stuff.Type = tag; break;

                    default: break;
                }

            if (count == 0)
            { missionSymTable.Add(sym, stuff); }

            else if (objects[count - 1] is Person)
            {
                stuff.Take((Person)objects[count - 1]);
            }

            objects[count] = stuff;
            count = 0;
        }
        //--------------------------------------------------------

        public void AddBG(char sym, char realSym, string Name, List<string> data)//Добавление бэкграунда
        {
            BackGround bg = new BackGround(realSym, Name, 0, 0, false);

            foreach (string tag in data)
                switch (tag)
                {
                    case ("Door"): bg = new Door(realSym, Name, 0, 0, false, false); break;
                    case ("LockedDoor"): bg = new Door(realSym, Name, 0, 0, false, true); break;
                    case ("Home"): bg = new Home(realSym, Name, 0, 0, false, true, null); break;

                    case ("NotBlock"): bg.isBlock = false; break;
                    case ("IsBlock"): bg.isBlock = true; break;

                    
                    default: break;
                }

            missionSymTable.Add(sym, bg);
            objects[count] = bg;
            count = 0;
        }

        //--------------------------------------------------------






        public int GetNumeric(string word)
        {
            return Int32.Parse(word.Split('=').Last<string>());
        }


        public void UniteTabs()
        {
            foreach (KeyValuePair<char, Objects> pair in missionSymTable)
            {
                baseSymTable.Add(pair.Key, pair.Value);
            }
        }
       
    }
}
