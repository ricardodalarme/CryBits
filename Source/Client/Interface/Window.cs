using System;
using System.Collections.Generic;
using SFML.Window;

class Window
{
    // Detecção de duplo clique
    private static int DoubleClick_Timer;

    public static void OnClosed(object sender, EventArgs e)
    {
        // Fecha o jogo
        if (Tools.CurrentWindow == Tools.Windows.Game)
            Game.Leave();
        else
            Program.Working = false;
    }

    public static void OnMouseButtonPressed(object sender, MouseButtonEventArgs e)
    {
        // Clique duplo
        if (Environment.TickCount < DoubleClick_Timer + 255)
        {
            if (Tools.CurrentWindow == Tools.Windows.Game)
            {
                // Usar item
                byte Slot = Tools.Inventory_Mouse();
                if (Slot > 0)
                    if (Player.Inventory[Slot].Item_Num > 0)
                        Send.Inventory_Use(Slot);

                // Usar o que estiver na hotbar
                Slot = Tools.Hotbar_Mouse();
                if (Slot > 0)
                    if (Player.Hotbar[Slot].Slot > 0)
                        Send.Hotbar_Use(Slot);
            }
        }
        // Clique único
        else
        {
            // Percorre toda a árvore de ordem para executar o comando
            Stack<List<Tools.Order_Structure>> Stack = new Stack<List<Tools.Order_Structure>>();
            Stack.Push(Tools.Order);
            while (Stack.Count != 0)
            {
                List<Tools.Order_Structure> Top = Stack.Pop();

                for (byte i = 0; i < Top.Count; i++)
                    if (Top[i].Data.Visible)
                    {
                        // Executa o comando
                        if (Top[i].Data is Buttons.Structure) ((Buttons.Structure)Top[i].Data).MouseDown(e);
                        Stack.Push(Top[i].Nodes);
                    }
            }

            // Eventos em jogo
            if (Tools.CurrentWindow == Tools.Windows.Game)
            {
                Tools.Inventory_MouseDown(e);
                Tools.Equipment_MouseDown(e);
                Tools.Hotbar_MouseDown(e);
            }
        }
    }

    public static void OnMouseButtonReleased(object sender, MouseButtonEventArgs e)
    {
        // Contagem do clique duplo
        DoubleClick_Timer = Environment.TickCount;

        // Percorre toda a árvore de ordem para executar o comando
        Stack<List<Tools.Order_Structure>> Stack = new Stack<List<Tools.Order_Structure>>();
        Stack.Push(Tools.Order);
        while (Stack.Count != 0)
        {
            List<Tools.Order_Structure> Top = Stack.Pop();

            for (byte i = 0; i < Top.Count; i++)
                if (Top[i].Data.Visible)
                {
                    // Executa o comando
                    if (Top[i].Data is Buttons.Structure) ((Buttons.Structure)Top[i].Data).MouseUp();
                    else if (Top[i].Data is CheckBoxes.Structure) ((CheckBoxes.Structure)Top[i].Data).MouseUp();
                    else if (Top[i].Data is TextBoxes.Structure) ((TextBoxes.Structure)Top[i].Data).MouseUp(Top[i]);
                    Stack.Push(Top[i].Nodes);
                }
        }

        // Eventos em jogo
        if (Tools.CurrentWindow == Tools.Windows.Game)
        {
            // Muda o slot do item
            if (Player.Inventory_Change > 0)
                if (Tools.Inventory_Mouse() > 0)
                    Send.Inventory_Change(Player.Inventory_Change, Tools.Inventory_Mouse());

            // Muda o slot da hotbar
            if (Tools.Hotbar_Mouse() > 0)
            {
                if (Player.Hotbar_Change > 0) Send.Hotbar_Change(Player.Hotbar_Change, Tools.Hotbar_Mouse());
                if (Player.Inventory_Change > 0) Send.Hotbar_Add(Tools.Hotbar_Mouse(), (byte)Game.Hotbar.Item, Player.Inventory_Change);
            }

            // Reseta a movimentação
            Player.Inventory_Change = 0;
            Player.Hotbar_Change = 0;
        }
    }

    public static void OnMouseMoved(object sender, MouseMoveEventArgs e)
    {
        // Define a posição do mouse à váriavel
        Tools.Mouse.X = e.X;
        Tools.Mouse.Y = e.Y;

        // Percorre toda a árvore de ordem para executar o comando
        Stack<List<Tools.Order_Structure>> Stack = new Stack<List<Tools.Order_Structure>>();
        Stack.Push(Tools.Order);
        while (Stack.Count != 0)
        {
            List<Tools.Order_Structure> Top = Stack.Pop();

            for (byte i = 0; i < Top.Count; i++)
                if (Top[i].Data.Visible)
                {
                    // Executa o comando
                    if (Top[i].Data is Buttons.Structure) ((Buttons.Structure)Top[i].Data).MouseMove();
                    Stack.Push(Top[i].Nodes);
                }
        }
    }

    public static void OnKeyPressed(object sender, KeyEventArgs e)
    {
        // Define se um botão está sendo pressionado
        switch (e.Code)
        {
            case Keyboard.Key.Tab: TextBoxes.ChangeFocus(); return;
        }
    }

    public static void OnKeyReleased(object sender, KeyEventArgs e)
    {
        // Define se um botão está sendo pressionado
        if (Tools.CurrentWindow == Tools.Windows.Game)
            switch (e.Code)
            {
                case Keyboard.Key.Return: TextBoxes.Chat_Type(); break;
                case Keyboard.Key.Space: Player.CollectItem(); break;
                case Keyboard.Key.Num1: Send.Hotbar_Use(1); break;
                case Keyboard.Key.Num2: Send.Hotbar_Use(2); break;
                case Keyboard.Key.Num3: Send.Hotbar_Use(3); break;
                case Keyboard.Key.Num4: Send.Hotbar_Use(4); break;
                case Keyboard.Key.Num5: Send.Hotbar_Use(5); break;
                case Keyboard.Key.Num6: Send.Hotbar_Use(6); break;
                case Keyboard.Key.Num7: Send.Hotbar_Use(7); break;
                case Keyboard.Key.Num8: Send.Hotbar_Use(8); break;
                case Keyboard.Key.Num9: Send.Hotbar_Use(9); break;
                case Keyboard.Key.Num0: Send.Hotbar_Use(0); break;
            }
    }

    public static void OnTextEntered(object sender, TextEventArgs e)
    {
        // Executa os eventos
        if (TextBoxes.Focused != null) ((TextBoxes.Structure)TextBoxes.Focused.Data).TextEntered(e);
    }
}