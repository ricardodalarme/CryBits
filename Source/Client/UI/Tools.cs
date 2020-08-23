using System.Collections.Generic;
using System.Drawing;

namespace Interface
{
    class Tools
    {
        // Ordem da renderização das ferramentas
        public static List<Order_Structure>[] All_Order = new List<Order_Structure>[(byte)Windows.Types.Count];
        public static List<Order_Structure> Order => All_Order[(byte)Windows.Current];

        public class Order_Structure
        {
            public Structure Data;
            public Order_Structure Parent;
            public List<Order_Structure> Nodes;
            public bool Viewable => Viewable(this);
        }

        public class Structure
        {
            public string Name;
            public bool Visible;
            public Point Position;
            public Windows.Types Window;
        }

        // Tipos de ferramentas
        public enum Types
        {
            Button,
            Panel,
            CheckBox,
            TextBox
        }

        public static bool Viewable(Order_Structure Order)
        {
            // Verifica se a ferramenta está visível
            if (Order == null) return true;
            if (Order.Data.Window != Windows.Current) return false;
            if (!Order.Data.Visible) return false;
            return Viewable(Order.Parent);
        }

        public static Order_Structure Get(Structure Tool)
        {
            // Percorre toda a árvore de ordem para encontrar a ferramenta
            Stack<List<Order_Structure>> Stack = new Stack<List<Order_Structure>>();
            for (byte i = 0; i < All_Order.Length; i++) Stack.Push(All_Order[i]);
            while (Stack.Count != 0)
            {
                List<Order_Structure> Top = Stack.Pop();

                for (byte i = 0; i < Top.Count; i++)
                {
                    if (Top[i].Data == Tool) return Top[i];
                    Stack.Push(Top[i].Nodes);
                }
            }
            return null;
        }
    }
}