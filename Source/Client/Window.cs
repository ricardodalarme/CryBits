using System.Windows.Forms;

public partial class Window : Form
{
    // Usado para acessar os dados da janela
    public static Window Objects = new Window();

    public Window()
    {
        InitializeComponent();
    }

    private void Window_FormClosing(object sender, FormClosingEventArgs e)
    {
        // Fecha o jogo
        if (Tools.CurrentWindow == Tools.Windows.Game)
        {
            e.Cancel = true;
            Game.Leave();
        }
        else
            Program.Working = false;
    }

    private void Window_MouseDown(object sender, MouseEventArgs e)
    {
        // Executa o evento de acordo a sobreposição do ponteiro
        for (byte i = 0; i < Tools.Order.Length; i++)
            switch (Tools.Order[i].Type)
            {
                case Tools.Types.Button: Buttons.Events.MouseDown(e, Tools.Order[i].Index); break;
            }

        // Eventos em jogo
        if (Tools.CurrentWindow == Tools.Windows.Game)
        {
            Tools.Inventory_MouseDown(e);
            Tools.Equipment_MouseDown(e);
            Tools.Hotbar_MouseDown(e);
        }
    }

    private void Window_MouseMove(object sender, MouseEventArgs e)
    {
        // Define a posição do mouse à váriavel
        Tools.Mouse.X = e.X;
        Tools.Mouse.Y = e.Y;

        // Executa o evento de acordo a sobreposição do ponteiro
        for (byte i = 0; i < Tools.Order.Length; i++)
            switch (Tools.Order[i].Type)
            {
                case Tools.Types.Button: Buttons.Events.MouseMove(e, Tools.Order[i].Index); break;
            }
    }

    private void Window_MouseUp(object sender, MouseEventArgs e)
    {
        // Executa o evento de acordo a sobreposição do ponteiro
        for (byte i = 0; i < Tools.Order.Length; i++)
            switch (Tools.Order[i].Type)
            {
                case Tools.Types.Button: Buttons.Events.MouseUp(e, Tools.Order[i].Index); break;
                case Tools.Types.CheckBox: CheckBoxes.Events.MouseUp(e, Tools.Order[i].Index); break;
                case Tools.Types.TextBox: TextBoxes.Events.MouseUp(e, Tools.Order[i].Index); break;
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

    private void Window_KeyPress(object sender, KeyPressEventArgs e)
    {
        // Executa os eventos
        TextBoxes.Events.KeyPress(e);
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        // Define se um botão está sendo pressionado
        switch (e.KeyCode)
        {
            case Keys.Up: Game.Press_Up = true; break;
            case Keys.Down: Game.Press_Down = true; break;
            case Keys.Left: Game.Press_Left = true; break;
            case Keys.Right: Game.Press_Right = true; break;
            case Keys.ShiftKey: Game.Press_Shift = true; break;
            case Keys.ControlKey: Game.Press_Control = true; break;
            case Keys.Enter: TextBoxes.Chat_Type(); break;
        }

        // Em jogo
        if (Tools.CurrentWindow == Tools.Windows.Game)
            if (!Panels.Find("Chat").General.Visible)
            {
                switch (e.KeyCode)
                {
                    case Keys.Space: Player.CollectItem(); break;
                    case Keys.D1: Send.Hotbar_Use(1); break;
                    case Keys.D2: Send.Hotbar_Use(2); break;
                    case Keys.D3: Send.Hotbar_Use(3); break;
                    case Keys.D4: Send.Hotbar_Use(4); break;
                    case Keys.D5: Send.Hotbar_Use(5); break;
                    case Keys.D6: Send.Hotbar_Use(6); break;
                    case Keys.D7: Send.Hotbar_Use(7); break;
                    case Keys.D8: Send.Hotbar_Use(8); break;
                    case Keys.D9: Send.Hotbar_Use(9); break;
                    case Keys.D0: Send.Hotbar_Use(0); break;
                }
            }
    }

    private void Window_KeyUp(object sender, KeyEventArgs e)
    {
        // Define se um botão está sendo pressionado
        switch (e.KeyCode)
        {
            case Keys.Up: Game.Press_Up = false; break;
            case Keys.Down: Game.Press_Down = false; break;
            case Keys.Left: Game.Press_Left = false; break;
            case Keys.Right: Game.Press_Right = false; break;
            case Keys.ShiftKey: Game.Press_Shift = false; break;
            case Keys.ControlKey: Game.Press_Control = false; break;
        }
    }

    private void Window_Paint(object sender, PaintEventArgs e)
    {
        // Atualiza a Window
        Graphics.Present();
    }

    private void Window_DoubleClick(object sender, System.EventArgs e)
    {
        // Eventos em jogo
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
}
