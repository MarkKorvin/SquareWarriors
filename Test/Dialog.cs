using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    [Serializable()]
    public class Dialog
    {

        public List<Acts> Actions = null;
        public List<Acts> prevActions = null;
        public Acts TabAction = null;
        public string DialogText;
        public int Next = 0;
        private Action del;

        List<Acts> nullList = new List<Acts>();
        
        public Dialog()
        {
            Acts qwe = new Acts("", del = () => { });
            nullList.Insert(0, qwe);
            TabAction = new Acts("-----Show more-----", del = () => { Next = Next == GlobalProperties.GetInfoHeight() - 4 ? 0 : GlobalProperties.GetInfoHeight() - 4; });
        }

        //Конструкторы диалогов
        public void SetDialog(string text)
        {
            Actions = nullList;
            DialogText = text;

            Next = 0;
        }
        public void SetDialog(string title, List<Acts> Actions)
        {
            DialogText = title;
            this.Actions = Actions;  
        }
    }

    [Serializable()]
    public class Acts
    {
        public string ActInfo;
        public string ActEnd = "Complete";
        public Action Act;
        public bool isEnd = false;

        //Конструкторы действий
        public Acts(string text, Action action) //Простое действие
        {
            ActInfo = text;
            Act = action;
        }
        public Acts(string text, Action action, bool isEnd) //Конечное действие в цепочке
        {
            ActInfo = text;
            Act = action;
            this.isEnd = isEnd;
        }
        public Acts(string text, Action action, string textFin) //Действие с выводом посттекста 
        {
            ActInfo = text;
            Act = action;
            isEnd = true;
            ActEnd = textFin;
        }
    }
}
