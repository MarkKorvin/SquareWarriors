using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Test
{
    [Serializable()]
    public class Perk
    {
        int I; //номер выбранной абилки
        private char[,] basePerk;
        List<Acts> Acts = new List<Acts>();
        Action del;
        public Abilities CurAbils = new Abilities();



        public Perk()
            {
              basePerk = PerkGetBase();
            }//Визуализация

        public char[,] PerkGet()
            {
                char[,] curField = PerkGetBase();
                return curField;
            }
        private char[,] PerkGetBase()
            {

                string fileName = "perks.txt";
                string[] lines = File.ReadAllLines(fileName, Encoding.Default);

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

        public void ShowPerks() //Показать навыки
        {
            I = 10; 
            Acts.Clear();
            Acts.Add(new Acts("Improve skill ", del = chooseAbility));
            Acts.Add(new Acts("Look at the skills", del = ()=> { CurAbils.CurrentAbilities(); ShowPerks(); }));
            Program.mainDialog.SetDialog("This is a skillbar (free skillpoints = "+ CurAbils.freePoints+") ", Acts);
        }

        public void chooseAbility()
        {
            Acts.Clear();
            
            if (I == 10 )//Выбор направления
            {
                Acts.Add(new Acts("Persistance ", del = () => { I = 0; chooseAbility(); }));
                Acts.Add(new Acts("Strength ", del = () => { I = 1; chooseAbility(); }));
                Acts.Add(new Acts("Agility ", del = () => { I = 2; chooseAbility(); }));
                Acts.Add(new Acts("Charisma and Perception ", del = () => { I = 3; chooseAbility(); }));
                Acts.Add(new Acts("Intelligence ", del = () => { I = 4; chooseAbility(); }));
                Acts.Add(new Acts("Guts ", del = () => { I = 5; chooseAbility(); }));
                Acts.Add(new Acts("Imagination ", del = () => { I = 6; chooseAbility(); }));
            }
            else
            {
                Acts.Add(new Acts(CurAbils.AbilityInfo(I, 0), del = () => { CurAbils.ImproveAbils(I, 0); I = 10; ShowPerks(); }));
                Acts.Add(new Acts(CurAbils.AbilityInfo(I, 1), del = () => { CurAbils.ImproveAbils(I, 1); I = 10; ShowPerks(); }));
                Acts.Add(new Acts(CurAbils.AbilityInfo(I, 2), del = () => { CurAbils.ImproveAbils(I, 2); I = 10; ShowPerks(); }));
                if(I!=3)
                Acts.Add(new Acts(CurAbils.AbilityInfo(I, 3), del = () => { CurAbils.ImproveAbils(I, 3); I = 10; ShowPerks(); }));
                Acts.Add(new Acts(CurAbils.AbilityInfo(I, 4), del = () => { CurAbils.ImproveAbils(I, 4); I = 10; ShowPerks(); }));
                Acts.Add(new Acts(CurAbils.AbilityInfo(I, 5), del = () => { CurAbils.ImproveAbils(I, 5); I = 10; ShowPerks(); }));
                Acts.Add(new Acts(CurAbils.AbilityInfo(I, 6), del = () => { CurAbils.ImproveAbils(I, 6); I = 10; ShowPerks(); }));

            }
            
           Program.mainDialog.SetDialog("Choose the skill:", Acts);
        }

    }

    [Serializable()]
    public class Abilities
    {
        public Ability[,] perks;
        public int freePoints;

        public int Agility = 1;
        public int Strength = 1;
        public int Inteligence = 1;
        public int Charisma = 1;

        public Abilities()
        {
            perks = new Ability[7, 7];
            perks[0, 0] = new Ability("Leadership");
            perks[0, 1] = new Ability("Fidelity");
            perks[0, 2] = new Ability("Resistance");
            perks[0, 3] = new Ability("Persistance");
            perks[0, 4] = new Ability("Blocking");
            perks[0, 5] = new Ability("Melee");
            perks[0, 6] = new Ability("Fight");
            perks[1, 0] = new Ability("Help");
            perks[1, 1] = new Ability("Interrogation");
            perks[1, 2] = new Ability("Intimidation");
            perks[1, 3] = new Ability("Strength");
            perks[1, 4] = new Ability("Lifting");
            perks[1, 5] = new Ability("Capture");
            perks[1, 6] = new Ability("Range");
            perks[2, 0] = new Ability("Brave");
            perks[2, 1] = new Ability("Inspiration");
            perks[2, 2] = new Ability("Constitution");
            perks[2, 3] = new Ability("Stamina");
            perks[2, 4] = new Ability("Acrobatics");
            perks[2, 5] = new Ability("Reaction");
            perks[2, 6] = new Ability("Accuracy");
            perks[3, 0] = new Ability("Confidence");
            perks[3, 1] = new Ability("Conviction");
            perks[3, 2] = new Ability("Charism");
            perks[3, 3] = new Ability("LEVEL");
            perks[3, 4] = new Ability("Perception");
            perks[3, 5] = new Ability("Agility");
            perks[3, 6] = new Ability("Speed");
            perks[4, 0] = new Ability("Charm");
            perks[4, 1] = new Ability("Freedom");
            perks[4, 2] = new Ability("Lying");
            perks[4, 3] = new Ability("Intelligence");
            perks[4, 4] = new Ability("Stealth");
            perks[4, 5] = new Ability("Spying");
            perks[4, 6] = new Ability("Stealing");
            perks[5, 0] = new Ability("Extortion");
            perks[5, 1] = new Ability("Smuggling");
            perks[5, 2] = new Ability("Trade");
            perks[5, 3] = new Ability("Guts");
            perks[5, 4] = new Ability("Planning");
            perks[5, 5] = new Ability("Diversion");
            perks[5, 6] = new Ability("Escape");
            perks[6, 0] = new Ability("Recruitment");
            perks[6, 1] = new Ability("Bribe");
            perks[6, 2] = new Ability("Lease");
            perks[6, 3] = new Ability("Imagination");
            perks[6, 4] = new Ability("Inspiration");
            perks[6, 5] = new Ability("Traps");
            perks[6, 6] = new Ability("Forgery");

            freePoints = 3;
            perks[3, 3].AbilityLevel = 1;
        }

        public int[,] PossibAbils() //Список возможных для улучшения абилок
        {
            int[,] abils = new int[7, 7];

            for (int i = 0; i < 7; i++)
                for (int j = 0; j < 7; j++)
                {
                    if (perks[i, j].AbilityLevel > 0)
                        for (int k = 0; k < 7; k++)
                        {
                            abils[i, k] = 1;
                            abils[k, j] = 1;
                        }
                }
            return abils;
        }

        public void ImproveAbils(int i, int j)
        {
            if (freePoints > 0)
            {
                perks[i, j].AbilityLevel++;
                freePoints--;
                if (i == 3)
                    if (j < 3)
                        Charisma += 2;
                    else
                        Strength += 2;
                else
                if (j == 3)
                    if (i > 3)
                        Inteligence += 2;
                    else
                        Agility += 2;
                else
                if (i < 3) { Strength++; }
                if (i > 3) { Inteligence ++; }
                if (j < 3) { Charisma ++; }
                if (j > 3) { Agility++;}

            }
        }

        public string AbilityInfo(int i, int j)
        {
            return perks[i, j].AbilityName + " (" + perks[i, j].AbilityLevel + ") ";
        }

        public void CurrentAbilities()
        {
            string Answer="";
            foreach (Ability abil in perks)
                if (abil.AbilityLevel > 0 )
                {
                    Answer += abil.AbilityName + "(" + abil.AbilityLevel + "), ";
                }
            Answer = Answer.Remove(Answer.Length - 2);
            Program.Helper.Say(Answer);
        }
    }

    [Serializable()]
    public class Ability
    {
        public string AbilityName;
        public int AbilityLevel = 0;

        public Ability(string AbilityName)
        {
            this.AbilityName = AbilityName;
        }
    }
}
