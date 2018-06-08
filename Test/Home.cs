using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{

    class Home : Door
    {

        List<Person> dwellers = new List<Person>();
        List<Person> dwellersInHome;
        static Action del;

        public Home(char symbol, string Name, int x, int y, bool isBlock, bool isKeyNeeded, List<Person> dwellers) : base(symbol, Name, x, y, isBlock, isKeyNeeded)
        {
            this.dwellers = dwellers;
            dwellersInHome = dwellers;

            this.isBlock = isBlock;
            this.symbol = symbol;
            this.isKeyNeeded = isKeyNeeded;

            Methods.Add(new Acts("Knock-knock", del = Call));

        }

       
        public void AddDweller(Person dweller)
        {
            dwellers.Add(dweller);
        }

        private void GoOut(Person dweller)
        {
            dwellersInHome.Remove(dweller);
            Open();
            Program.CurrentMap.MapObjects[y, x].Dude = dweller;
        }

        private void GoBack(Person dweller)
        {
            dwellersInHome.Add(dweller);
        }

        private void Call()
        {
            List<Acts> servants = new List<Acts>();
            foreach (Person dweller in dwellersInHome)
            { servants.Add(new Acts(dweller.Name + ", Group - " + dweller.group.Name + ", Moral = " + dweller.Moral, del = () => { GoOut(dweller); }, true)); }
            Program.mainDialog.SetDialog("In this house you can find:",servants);
        }

    }


        class Door : BackGround
        {
            public bool isKeyNeeded = false;

            public Door(char symbol, string Name, int x, int y, bool isBlock, bool isKeyNeeded) : base(symbol, Name, x, y, isBlock)
            {
                Action del;
                this.isBlock = isBlock;
                this.symbol = symbol;
                this.isKeyNeeded = isKeyNeeded;
                { Methods.Add(new Acts("Open/Close", del = Open, true)); }
            }

            public void Open()
            {
                if (Program.CurrentMap.MapObjects[y, x].MainObj == null && Program.CurrentMap.MapObjects[y, x].Dude == null)
                {
                    if (!isKeyNeeded || !isBlock)
                    {
                        isBlock = !isBlock;
                        symbol = !isBlock ? 'П' : 'Ш';
                        Name = Name == "Opened Door" ? "Closed Door" : "Opened Door";
                    }
                    else if (isBlock)
                    {
                        Program.Helper.Say("Door is closed");
                    }
                }
            }

        }
    }




