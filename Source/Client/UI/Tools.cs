using System.Collections.Generic;
using System.Drawing;

namespace CryBits.Client.UI
{
    class Tools
    {
        // Ordem da renderização das ferramentas
        public static List<OrderStructure>[] All_Order = new List<OrderStructure>[(byte)WindowsTypes.Count];
        public static List<OrderStructure> Order => All_Order[(byte)Windows.Current];

        public class OrderStructure
        {
            public Structure Data;
            public OrderStructure Parent;
            public List<OrderStructure> Nodes;
            public bool Viewable => Viewable(this);
        }

        public class Structure
        {
            public string Name;
            public bool Visible;
            public Point Position;
            public WindowsTypes Window;
        }

        // Tipos de ferramentas
        public enum Types
        {
            Button,
            Panel,
            CheckBox,
            TextBox
        }

        public static bool Viewable(OrderStructure order)
        {
            // Verifica se a ferramenta está visível
            if (order == null) return true;
            if (order.Data.Window != Windows.Current) return false;
            if (!order.Data.Visible) return false;
            return Viewable(order.Parent);
        }

        public static OrderStructure Get(Structure tool)
        {
            // Percorre toda a árvore de ordem para encontrar a ferramenta
            Stack<List<OrderStructure>> stack = new Stack<List<OrderStructure>>();
            for (byte i = 0; i < All_Order.Length; i++) stack.Push(All_Order[i]);
            while (stack.Count != 0)
            {
                List<OrderStructure> top = stack.Pop();

                for (byte i = 0; i < top.Count; i++)
                {
                    if (top[i].Data == tool) return top[i];
                    stack.Push(top[i].Nodes);
                }
            }
            return null;
        }
    }
}