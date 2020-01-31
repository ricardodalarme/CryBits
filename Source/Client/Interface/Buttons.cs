using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

class Buttons
{
    // Aramazenamento de dados da ferramenta
    public static List<Structure> List = new List<Structure>();

    // Estrutura das ferramentas
    public class Structure : Tools.Structure
    {
        // Dados
        public byte Texture_Num;
        public States State;

        public void MouseUp()
        {
            SFML.Graphics.Texture Texture = Graphics.Tex_Button[Texture_Num];

            // Somente se necessário
            if (!Tools.IsAbove(new Rectangle(Position, Graphics.TSize(Texture)))) return;

            // Altera o estado do botão
            Audio.Sound.Play(Audio.Sounds.Click);
            State = States.Above;

            // Executa o evento
            Execute(Name);
        }

        public void MouseDown(MouseEventArgs e)
        {
            SFML.Graphics.Texture Texture = Graphics.Tex_Button[Texture_Num];

            // Somente se necessário
            if (e.Button == MouseButtons.Right) return;
            if (!Tools.IsAbove(new Rectangle(Position, Graphics.TSize(Texture)))) return;

            // Altera o estado do botão
            State = States.Click;
        }

        public void MouseMove(MouseEventArgs e)
        {
            SFML.Graphics.Texture Texture = Graphics.Tex_Button[Texture_Num];

            // Somente se necessário
            if (e.Button == MouseButtons.Right) return;

            // Se o mouse não estiver sobre a ferramenta, então não executar o evento
            if (!Tools.IsAbove(new Rectangle(Position, Graphics.TSize(Texture))))
            {
                State = States.Normal;
                return;
            }

            // Se o botão já estiver no estado normal, isso não é necessário
            if (State != States.Normal) return;

            // Altera o estado do botão
            State = States.Above;
            Audio.Sound.Play(Audio.Sounds.Above);
        }
    }

    // Estados dos botões
    public enum States
    {
        Normal,
        Click,
        Above,
    }

    public static Structure Get(string Name)
    {
        // Lista os nomes das ferramentas
        for (byte i = 0; i < List.Count; i++)
            if (List[i].Name.Equals(Name))
                return List[i];

        return null;
    }

    public static void Execute(string Name)
    {
        // Executa o evento do botão
        switch (Name)
        {
            case "Connect": Connect(); break;
            case "Register": Register(); break;
            case "Options": Options(); break;
            case "Options_Back": Menu_Return(); break;
            case "Connect_Confirm": Connect_Ok(); break;
            case "Register_Confirm": Register_Ok(); break;
            case "CreateCharacter": CreateCharacter(); break;
            case "CreateCharacter_ChangeRight": CreateCharacter_ChangeRight(); break;
            case "CreateCharacter_ChangeLeft": CreateCharacter_ChangeLeft(); break;
            case "CreateCharacter_Texture_ChangeLeft": CreateCharacter_Texture_ChangeLeft(); break;
            case "CreateCharacter_Texture_ChangeRight": CreateCharacter_Texture_ChangeRight(); break;
            case "CreateCharacter_Back": CreateCharacter_Return(); break;
            case "Character_Use": Character_Use(); break;
            case "Character_Create": Character_Create(); break;
            case "Character_Delete": Character_Delete(); break;
            case "Character_ChangeRight": Character_Change_Right(); break;
            case "Character_ChangeLeft": Character_Change_Left(); break;
            case "Chat_Up": Chat_Up(); break;
            case "Chat_Down": Chat_Down(); break;
            case "Menu_Character": Menu_Character(); break;
            case "Attributes_Strength": Attribute_Strenght(); break;
            case "Attributes_Resistance": Attribute_Resistance(); break;
            case "Attributes_Intelligence": Attribute_Intelligence(); break;
            case "Attributes_Agility": Attribute_Agility(); break;
            case "Attributes_Vitality": Attribute_Vitality(); break;
            case "Menu_Inventory": Menu_Inventory(); break;
            case "Menu_Options": Menu_Options(); break;
        }
    }

    public static bool Characters_Change_Buttons()
    {
        bool Visibility = true;

        // Verifica apenas se o painel for visível
        if (Lists.Characters == null || Lists.Characters[Game.SelectCharacter].Class == 0) Visibility = false;

        // Altera os botões visíveis
        Get("Character_Create").Visible = !Visibility;
        Get("Character_Delete").Visible = Visibility;
        Get("Character_Use").Visible = Visibility;
        return Visibility;
    }

    public static void Connect()
    {
        // Termina a conexão
        Socket.Disconnect();

        // Abre o painel
        Panels.Menu_Close();
        Panels.Get("Connect").Visible = true;
    }

    public static void Register()
    {
        // Termina a conexão
        Socket.Disconnect();

        // Abre o painel
        Panels.Menu_Close();
        Panels.Get("Register").Visible = true;
    }

    public static void Options()
    {
        // Termina a conexão
        Socket.Disconnect();

        // Abre o painel
        Panels.Menu_Close();
        Panels.Get("Options").Visible = true;
    }

    public static void Menu_Return()
    {
        // Termina a conexão
        Socket.Disconnect();

        // Abre o painel
        Panels.Menu_Close();
        Panels.Get("Connect").Visible = true;
    }

    public static void Connect_Ok()
    {
        // Salva o nome do usuário
        Lists.Options.Username = TextBoxes.Get("Connect_Username").Text;
        Write.Options();

        // Conecta-se ao jogo
        Game.SetSituation(Game.Situations.Connect);
    }

    public static void Register_Ok()
    {
        // Regras de segurança
        if (TextBoxes.Get("Register_Password").Text != TextBoxes.Get("Register_Password2").Text)
        {
            MessageBox.Show("The password don't match.");
            return;
        }

        // Registra o jogador, se estiver tudo certo
        Game.SetSituation(Game.Situations.Registrar);
    }

    public static void CreateCharacter()
    {
        // Abre a criação de personagem
        Game.SetSituation(Game.Situations.CreateCharacter);
    }

    public static void CreateCharacter_ChangeRight()
    {
        // Altera a classe selecionada pelo jogador
        if (Game.CreateCharacter_Class == Lists.Class.GetUpperBound(0))
            Game.CreateCharacter_Class = 1;
        else
            Game.CreateCharacter_Class += 1;
    }

    public static void CreateCharacter_ChangeLeft()
    {
        // Altera a classe selecionada pelo jogador
        if (Game.CreateCharacter_Class == 1)
            Game.CreateCharacter_Class = (byte)Lists.Class.GetUpperBound(0);
        else
            Game.CreateCharacter_Class -= 1;
    }


    public static void CreateCharacter_Texture_ChangeRight()
    {
        // Lista de texturas
        short[] Tex_List;
        if (CheckBoxes.Get("GenderMale").State == true)
            Tex_List = Lists.Class[Game.CreateCharacter_Class].Tex_Male;
        else
            Tex_List = Lists.Class[Game.CreateCharacter_Class].Tex_Female;

        // Altera a classe selecionada pelo jogador
        if (Game.CreateCharacter_Tex == Tex_List.Length - 1)
            Game.CreateCharacter_Tex = 0;
        else
            Game.CreateCharacter_Tex += 1;
    }

    public static void CreateCharacter_Texture_ChangeLeft()
    {
        // Lista de texturas
        short[] Tex_List;
        if (CheckBoxes.Get("GenderMale").State == true)
            Tex_List = Lists.Class[Game.CreateCharacter_Class].Tex_Male;
        else
            Tex_List = Lists.Class[Game.CreateCharacter_Class].Tex_Female;

        // Altera a classe selecionada pelo jogador
        if (Game.CreateCharacter_Tex == 0)
            Game.CreateCharacter_Tex = (byte)(Tex_List.Length - 1);
        else
            Game.CreateCharacter_Tex -= 1;
    }

    public static void CreateCharacter_Return()
    {
        // Abre o painel de personagens
        Panels.Menu_Close();
        Panels.Get("SelectCharacter").Visible = true;
    }

    public static void Character_Use()
    {
        // Usa o personagem selecionado
        Send.Character_Use();
    }

    public static void Character_Delete()
    {
        // Deleta o personagem selecionado
        Send.Character_Delete();
    }

    public static void Character_Create()
    {
        // Abre a criação de personagem
        Send.Character_Create();
    }

    public static void Character_Change_Right()
    {
        // Altera o personagem selecionado pelo jogador
        if (Game.SelectCharacter == Lists.Server_Data.Max_Characters)
            Game.SelectCharacter = 1;
        else
            Game.SelectCharacter += 1;
    }

    public static void Character_Change_Left()
    {
        // Altera o personagem selecionado pelo jogador
        if (Game.SelectCharacter == 1)
            Game.SelectCharacter = Lists.Server_Data.Max_Characters;
        else
            Game.SelectCharacter -= 1;
    }

    public static void Chat_Up()
    {
        // Sobe as linhas do chat
        if (Tools.Chat_Line > 0)
            Tools.Chat_Line -= 1;
    }

    public static void Chat_Down()
    {
        // Sobe as linhas do chat
        if (Tools.Chat.Count - 1 - Tools.Chat_Line - Tools.Chat_Lines_Visible > 0)
            Tools.Chat_Line += 1;
    }

    public static void Menu_Character()
    {
        // Altera a visibilidade do painel e fecha os outros
        Panels.Get("Menu_Character").Visible = !Panels.Get("Menu_Character").Visible;
        Panels.Get("Menu_Inventory").Visible = false;
        Panels.Get("Menu_Options").Visible = false;
    }

    public static void Attribute_Strenght()
    {
        Send.AddPoint(Game.Attributes.Strength);
    }

    public static void Attribute_Resistance()
    {
        Send.AddPoint(Game.Attributes.Resistance);
    }

    public static void Attribute_Intelligence()
    {
        Send.AddPoint(Game.Attributes.Intelligence);
    }

    public static void Attribute_Agility()
    {
        Send.AddPoint(Game.Attributes.Agility);
    }

    public static void Attribute_Vitality()
    {
        Send.AddPoint(Game.Attributes.Vitality);
    }

    public static void Menu_Inventory()
    {
        // Altera a visibilidade do painel e fecha os outros
        Panels.Get("Menu_Inventory").Visible = !Panels.Get("Menu_Inventory").Visible;
        Panels.Get("Menu_Character").Visible = false;
        Panels.Get("Menu_Options").Visible = false;
    }

    public static void Menu_Options()
    {
        // Altera a visibilidade do painel e fecha os outros
        Panels.Get("Menu_Options").Visible = !Panels.Get("Menu_Options").Visible;
        Panels.Get("Menu_Character").Visible = false;
        Panels.Get("Menu_Inventory").Visible = false;
    }
}