using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Test
{
    [Serializable()]
    public class Inventory
    {
        public Stuff Accessory;
        public Stuff Dress;
        public Stuff LeftHand;
        public Stuff RightHand;
        public Stuff[] Pocket = new Stuff[8];
        private char[,] baseInventory;

        public Inventory()
        {
            baseInventory = InventoryGetBase();
        }

        public int FirstNotNull() //Первая свободная ячейка в инвентаре
        {
            for (int i = 0; i < Pocket.Length; i++)
            {
                if (Pocket[i] == null)
                    return i;
            }
            return -1;         
        }

        private char[,] InventoryGetBase() //Получение заготовки инвентаря
        {
            string fileName = "inventory.txt";
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
        public char[,] InventoryGet() //Визуализация инвентаря
        {
            char[,] curField = InventoryGetBase();

            PrintObj(curField, Accessory, 3, 16);
            PrintObj(curField, RightHand, 10, 3);
            PrintObj(curField, LeftHand, 10, 28);
            PrintObj(curField, Dress, 15, 16);

            for (int j = 0; j < 4; j++)
                for (int i = 0; i < 2; i++)
                {
                    PrintObj(curField, Pocket[2*j + i], 5 + 3*j, 50 + 17*i);
                }

            return curField;
        }

        private void PrintObj(char[,] curField, Stuff obj, int str, int col) //Вывод объекта на экран
        {
            if (obj!= null)
            {
                int shift = obj.Name.Length <= 13 ? (13 - obj.Name.Length) / 2 : 0;
                for (int i = col + shift; i < col + shift + obj.Name.Length; i++)
                    curField[str, i] = obj.Name[i - col - shift];
            }

        }

        public void ShowObj() //Предметы в инвентаре
        {
            List<Acts> Acts = new List<Acts>();
            for (int i = 0; i < 8; i++)
                if (Pocket[i] != null)
                {
                    Acts.Add(new Acts(Pocket[i].Name, Pocket[i].ShowMethods));
                }
                else
                {
                    Acts.Add(new Test.Acts("-----", VoidMethods));
                }
            Program.mainDialog.SetDialog("Ваши предметы", Acts);
        }
        public void ShowObjOnPerson() //Предметы на персонаже
        {
             List<Acts> Acts = new List<Acts>();
            if (RightHand != null)
                Acts.Add(new Acts("Right hand - " + RightHand.Name, PutIntoPocketRH));
            if (LeftHand!=null)
                Acts.Add(new Acts("Left hand - " + LeftHand.Name, PutIntoPocketLH));
            if (Dress != null)
                Acts.Add(new Acts("Chest - " + Dress.Name, PutIntoPocketD));
            if (Accessory != null)
                Acts.Add(new Acts("Accessory - " + Accessory.Name, PutIntoPocketA));
                Acts.Add(new Acts("Nothing", ShowObj));

            if (Acts.Count == 0)
                {
                    Acts.Add(new Test.Acts("You have no nothing", ShowObj));
                }
            
                Program.mainDialog.SetDialog("What do you want to pull off?", Acts);
        }

        public void VoidMethods()
        {
            List<Acts> Methods = new List<Acts>();
            Action del;

            string text = "What are you gonna do?";
            //Methods.Add(new Acts("Взять", del = Back, true));
            Methods.Add(new Acts("Hide item in the bag", del = ShowObjOnPerson));
            Program.mainDialog.SetDialog(text, Methods);
        }

        //Снять предметы
        public void PutIntoPocketLH()
        {   LeftHand.Take(LeftHand.Owner);   LeftHand = null; ShowObj(); }
        public void PutIntoPocketRH()
        {   RightHand.Take(RightHand.Owner);  RightHand = null; ShowObj(); }
        public void PutIntoPocketD()
        {   Dress.Take(Dress.Owner);      Dress = null; ShowObj();    }
        public void PutIntoPocketA()
        {   Accessory.Take(Accessory.Owner);   Accessory = null; ShowObj();   }


        

    }
}
