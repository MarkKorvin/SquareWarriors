using System;
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
            baseSymTable.Add('Y', new Person('Y', "Friend", 0, 0, BaseGroups.GrFriends));
            baseSymTable.Add('Z', new Person('Z', "Enemy", 0, 0, BaseGroups.GrEnemies));

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



        public void AddBlock(char sym, char realSym, string Name, string[] data)
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
        }

        public void AddPerson(char sym, char realSym, string Name, string[] data)
        {
            Person person = new Person(realSym, Name, 0, 0, BaseGroups.GrFriends);

            foreach (string tag in data)
                switch (tag)
                {
                    case ("Friend"): person.group = BaseGroups.GrFriends; break;
                    case ("Enemy"):person.group = BaseGroups.GrEnemies; break;

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
                            person.LevelUp();
                            lvl++;
                        }
                        break;

                    default: break;
                }

            missionSymTable.Add(sym, person);
        }

        public void AddBG(char sym, char realSym, string Name, string[] data)
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
        }







        public void AddToTable(string type, char sym, char realSym, string Name, string[] data)
        {
            switch (type)
            {
                case "Person": { AddPerson(sym, realSym, Name, data); break; }
                case "Block": { AddBlock(sym, realSym, Name, data); break; }
                case "BG": { AddBG(sym, realSym, Name, data); break; }
            }
        }
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
