using System;
using System.Collections.Generic;
using System.Drawing;
using CryBits.Client.Entities;
using CryBits.Client.Media.Audio;
using CryBits.Client.Network;
using SFML.Window;
using static CryBits.Client.Logic.Game;

namespace CryBits.Client.UI
{
    internal static class Windows
    {
        // Janela que está aberta
        public static WindowsTypes Current;

        // Detecção de duplo clique
        private static int _doubleClickTimer;

        // Posição do ponteiro do mouse
        public static Point Mouse;

        public static void OnClosed(object sender, EventArgs e)
        {
            // Fecha o jogo
            if (Current == WindowsTypes.Game)
                Socket.Disconnect();
            else
                Program.Working = false;
        }

        public static void OnMouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            // Clique duplo
            if (Environment.TickCount < _doubleClickTimer + 142)
            {
                if (Current == WindowsTypes.Game)
                {
                    // Usar item
                    short slot = Panels.InventorySlot;
                    if (slot > 0)
                        if (Player.Me.Inventory[slot].Item != null)
                            Send.Inventory_Use((byte)slot);

                    // Usar o que estiver na hotbar
                    slot = Panels.HotbarSlot;
                    if (slot > 0)
                        if (Player.Me.Hotbar[slot].Slot > 0)
                            Send.Hotbar_Use((byte)slot);

                    // Compra o item da loja
                    slot = Panels.ShopSlot;
                    if (slot >= 0)
                        if (Panels.ShopOpen != null)
                            Send.Shop_Buy((byte)slot);
                }
            }
            // Clique único
            else
            {
                // Percorre toda a árvore de ordem para executar o comando
                Stack<List<Tools.OrderStructure>> stack = new Stack<List<Tools.OrderStructure>>();
                stack.Push(Tools.Order);
                while (stack.Count != 0)
                {
                    List<Tools.OrderStructure> top = stack.Pop();

                    for (byte i = 0; i < top.Count; i++)
                        if (top[i].Data.Visible)
                        {
                            // Executa o comando
                            if (top[i].Data is Buttons) ((Buttons)top[i].Data).MouseDown(e);
                            stack.Push(top[i].Nodes);
                        }
                }

                // Eventos em jogo
                if (Current == WindowsTypes.Game)
                {
                    Panels.Inventory_MouseDown(e);
                    Panels.Equipment_MouseDown(e);
                    Panels.Hotbar_MouseDown(e);
                    Panels.Trade_MouseDown(e);
                }
            }
        }

        public static void OnMouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            // Contagem do clique duplo
            _doubleClickTimer = Environment.TickCount;

            // Percorre toda a árvore de ordem para executar o comando
            Stack<List<Tools.OrderStructure>> stack = new Stack<List<Tools.OrderStructure>>();
            stack.Push(Tools.Order);
            while (stack.Count != 0)
            {
                List<Tools.OrderStructure> top = stack.Pop();

                for (byte i = 0; i < top.Count; i++)
                    if (top[i].Data.Visible)
                    {
                        // Executa o comando
                        if (top[i].Data is Buttons) ((Buttons)top[i].Data).MouseUp();
                        else if (top[i].Data is CheckBoxes) ((CheckBoxes)top[i].Data).MouseUp();
                        else if (top[i].Data is TextBoxes) ((TextBoxes)top[i].Data).MouseUp(top[i]);
                        stack.Push(top[i].Nodes);
                    }
            }

            // Eventos em jogo
            if (Current == WindowsTypes.Game)
            {
                // Muda o slot do item
                if (Panels.InventorySlot > 0)
                {
                    if (Panels.InventoryChange > 0) Send.Inventory_Change(Panels.InventoryChange, Panels.InventorySlot);
                }
                // Muda o slot da hotbar
                else if (Panels.HotbarSlot >= 0)
                {
                    if (Panels.HotbarChange >= 0) Send.Hotbar_Change(Panels.HotbarChange, Panels.HotbarSlot);
                    if (Panels.InventoryChange > 0) Send.Hotbar_Add(Panels.HotbarSlot, (byte)Hotbars.Item, Panels.InventoryChange);
                }
                // Adiciona um item à troca
                else if (Panels.TradeSlot > 0)
                {
                    if (Panels.InventoryChange > 0)
                        if (Player.Me.Inventory[Panels.InventoryChange].Amount == 1)
                            Send.Trade_Offer(Panels.TradeSlot, Panels.InventoryChange);
                        else
                        {
                            Panels.TradeInventorySlot = Panels.InventoryChange;
                            TextBoxes.List["Trade_Amount"].Text = string.Empty;
                            Panels.List["Trade_Amount"].Visible = true;
                        }
                }

                // Reseta a movimentação
                Panels.InventoryChange = 0;
                Panels.HotbarChange = -1;
            }
        }

        public static void OnMouseMoved(object sender, MouseMoveEventArgs e)
        {
            // Define a posição do mouse à váriavel
            Mouse.X = e.X;
            Mouse.Y = e.Y;

            // Percorre toda a árvore de ordem para executar o comando
            Stack<List<Tools.OrderStructure>> stack = new Stack<List<Tools.OrderStructure>>();
            stack.Push(Tools.Order);
            while (stack.Count != 0)
            {
                List<Tools.OrderStructure> top = stack.Pop();

                for (byte i = 0; i < top.Count; i++)
                    if (top[i].Data.Visible)
                    {
                        // Executa o comando
                        if (top[i].Data is Buttons) ((Buttons)top[i].Data).MouseMove();
                        stack.Push(top[i].Nodes);
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
            if (Current == WindowsTypes.Game)
                switch (e.Code)
                {
                    case Keyboard.Key.Enter: Chat.Type(); break;
                    case Keyboard.Key.Space: Player.Me.CollectItem(); break;
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
            if (TextBoxes.Focused != null) ((TextBoxes)TextBoxes.Focused.Data).TextEntered(e);
        }

        public static void OpenMenu()
        {
            // Reproduz a música de fundo
            Sound.Stop_All();
            if (Option.Musics) Music.Play(Musics.Menu);

            // Nome do usuário salvo
            CheckBoxes.List["Connect_Save_Username"].Checked = Option.SaveUsername;
            if (Option.SaveUsername) TextBoxes.List["Connect_Username"].Text = Option.Username;

            // Traz o jogador de volta ao menu
            Panels.Menu_Close();
            Panels.List["Connect"].Visible = true;
            Current = WindowsTypes.Menu;
        }
    }
}