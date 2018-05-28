using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    [Serializable()]
    public class Politics
    {
        public  int number;
        public string Name;
        public ConsoleColor Color;
        Relations.Style Style;

        public Politics(string Name, ConsoleColor Color, Relations.Style Style)
        {
            this.Name = Name;
            this.Color = Color;
            this.Style = Style;
        }
    }

    [Serializable()]
    public class Relations
    {
        public enum Style { aggresive, protective, neutral } //Как фракция  играет
        public enum Relate { neutral, haters, lovers} //Как фракция относится к другим
        public static int N = 0;// Количество групп 
        public static Relate[,] relations = new Relate[0, 0];

        
        public static void AddGroup(Politics newGroup)
        {
          N++;
          Relate[,] NewRelations = new Relate[N, N];

            for (int i = 0; i < N - 1; i++)
                for (int j = 0; j < N - 1; j++)
                { NewRelations[i, j] = relations[i, j];}

          relations = NewRelations;
        }

        public static void SetRelations(int first, int second, Relate relation)
        {

            relations[first, second] = relation;
            relations[second, first] = relations[first, second];

            /*   if (relation == Relate.haters)     //Возникает бесконечная рекурсия. Переделать по возможности
                   for (int i = 0; i < N; i++)
                   {
                       if (relations[first, i] == Relate.lovers)
                       {
                           SetRelations(i, second, Relate.haters);
                           SetRelations(second, i, Relate.haters);
                       }
                       if (relations[second, i] == Relate.lovers)
                       {
                           SetRelations(i, first, Relate.haters);
                           SetRelations(first, i, Relate.haters);
                       }
                   }
            */

            if (relation == Relate.lovers)
                   for (int i = 0; i < N; i++)
                   {
                       if (relations[first, i] == Relate.haters)
                       {
                           SetRelations(i, second, Relate.haters);
                           SetRelations(second, i, Relate.haters);
                       }
                       if (relations[second, i] == Relate.haters)
                       {
                           SetRelations(i, first, Relate.haters);
                           SetRelations(first, i, Relate.haters);
                       }
                   }
            
            // if (relation == Relate.lovers)

        }


    }
}




