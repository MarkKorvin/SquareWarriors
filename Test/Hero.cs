using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    [Serializable()]
    class Hero : Person
    {
        public Hero(char symbol, string Name, int x, int y, Politics group) : base(symbol, Name, x, y, group)
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

            Methods.Add(new Acts("Info", del = Explore));

        }
    }
}
