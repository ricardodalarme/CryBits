using SFML.Window;
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
            // Somente se necessário
            if (!Tools.IsAbove(new Rectangle(Position, Graphics.TSize(Graphics.Tex_Button[Texture_Num])))) return;

            // Altera o estado do botão
            Audio.Sound.Play(Audio.Sounds.Click);
            State = States.Above;

            // Executa o evento
            Execute(Name);
        }

        public void MouseDown(MouseButtonEventArgs e)
        {
            // Somente se necessário
            if (e.Button == Mouse.Button.Right) return;
            if (!Tools.IsAbove(new Rectangle(Position, Graphics.TSize(Graphics.Tex_Button[Texture_Num])))) return;

            // Altera o estado do botão
            State = States.Click;
        }

        public void MouseMove()
        {
            // Se o mouse não estiver sobre a ferramenta, então não executar o evento
            if (!Tools.IsAbove(new Rectangle(Position, Graphics.TSize(Graphics.Tex_Button[Texture_Num]))))
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

    // Retorna o botão procurado
    public static Structure Get(string Name) => List.Find(x => x.Name.Equals(Name));

    private static void Execute(string Name)
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
            case "Drop_Confirm": Drop_Confirm(); break;
            case "Drop_Cancel": Drop_Cancel(); break;
            case "Party_Yes": Party_Yes(); break;
            case "Party_No": Party_No(); break;
            case "Trade_Yes": Trade_Yes(); break;
            case "Trade_No": Trade_No(); break;
            case "Trade_Close": Trade_Close(); break;
            case "Trade_Offer_Accept": Trade_Offer_Accept(); break;
            case "Trade_Offer_Decline": Trade_Offer_Decline(); break;
            case "Trade_Offer_Confirm": Trade_Offer_Confirm(); break;
            case "Trade_Amount_Confirm": Trade_Amount_Confirm(); break;
            case "Trade_Amount_Cancel": Trade_Amount_Cancel(); break;
            case "Shop_Close": Shop_Close(); break;
            case "Shop_Sell_Confirm": Shop_Sell_Confirm(); break;
            case "Shop_Sell_Cancel": Shop_Sell_Cancel(); break;
        }
    }

    public static bool Characters_Change_Buttons()
    {
        // Altera os botões visíveis
        bool Visibility = Lists.Characters != null && Lists.Characters[Game.SelectCharacter].Class > 0;
        Get("Character_Create").Visible = !Visibility;
        Get("Character_Delete").Visible = Visibility;
        Get("Character_Use").Visible = Visibility;
        return Visibility;
    }

    private static void Connect()
    {
        // Termina a conexão
        Socket.Disconnect();

        // Abre o painel
        Panels.Menu_Close();
        Panels.Get("Connect").Visible = true;
    }

    private static void Register()
    {
        // Termina a conexão
        Socket.Disconnect();

        // Abre o painel
        Panels.Menu_Close();
        Panels.Get("Register").Visible = true;
    }

    private static void Options()
    {
        // Termina a conexão
        Socket.Disconnect();

        // Define as marcações corretas
        CheckBoxes.Get("Sounds").Checked = Lists.Options.Sounds;
        CheckBoxes.Get("Musics").Checked = Lists.Options.Musics;

        // Abre o painel
        Panels.Menu_Close();
        Panels.Get("Options").Visible = true;
    }

    private static void Menu_Return()
    {
        // Termina a conexão
        Socket.Disconnect();

        // Abre o painel
        Panels.Menu_Close();
        Panels.Get("Connect").Visible = true;
    }

    private static void Connect_Ok()
    {
        // Salva o nome do usuário
        Lists.Options.Username = TextBoxes.Get("Connect_Username").Text;
        Write.Options();

        // Conecta-se ao jogo
        Game.SetSituation(Game.Situations.Connect);
    }

    private static void Register_Ok()
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

    private static void CreateCharacter()
    {
        // Abre a criação de personagem
        Game.SetSituation(Game.Situations.CreateCharacter);
    }

    private static void CreateCharacter_ChangeRight()
    {
        // Altera a classe selecionada pelo jogador
        if (Game.CreateCharacter_Class == Lists.Class.GetUpperBound(0))
            Game.CreateCharacter_Class = 1;
        else
            Game.CreateCharacter_Class += 1;
    }

    private static void CreateCharacter_ChangeLeft()
    {
        // Altera a classe selecionada pelo jogador
        if (Game.CreateCharacter_Class == 1)
            Game.CreateCharacter_Class = (byte)Lists.Class.GetUpperBound(0);
        else
            Game.CreateCharacter_Class -= 1;
    }

    private static void CreateCharacter_Texture_ChangeRight()
    {
        // Lista de texturas
        short[] Tex_List;
        if (CheckBoxes.Get("GenderMale").Checked == true)
            Tex_List = Lists.Class[Game.CreateCharacter_Class].Tex_Male;
        else
            Tex_List = Lists.Class[Game.CreateCharacter_Class].Tex_Female;

        // Altera a classe selecionada pelo jogador
        if (Game.CreateCharacter_Tex == Tex_List.Length - 1)
            Game.CreateCharacter_Tex = 0;
        else
            Game.CreateCharacter_Tex += 1;
    }

    private static void CreateCharacter_Texture_ChangeLeft()
    {
        // Lista de texturas
        short[] Tex_List;
        if (CheckBoxes.Get("GenderMale").Checked == true)
            Tex_List = Lists.Class[Game.CreateCharacter_Class].Tex_Male;
        else
            Tex_List = Lists.Class[Game.CreateCharacter_Class].Tex_Female;

        // Altera a classe selecionada pelo jogador
        if (Game.CreateCharacter_Tex == 0)
            Game.CreateCharacter_Tex = (byte)(Tex_List.Length - 1);
        else
            Game.CreateCharacter_Tex -= 1;
    }

    private static void CreateCharacter_Return()
    {
        // Abre o painel de personagens
        Panels.Menu_Close();
        Panels.Get("SelectCharacter").Visible = true;
    }

    private static void Character_Use()
    {
        // Usa o personagem selecionado
        Send.Character_Use();
    }

    private static void Character_Delete()
    {
        // Deleta o personagem selecionado
        Send.Character_Delete();
    }

    private static void Character_Create()
    {
        // Abre a criação de personagem
        Send.Character_Create();
    }

    private static void Character_Change_Right()
    {
        // Altera o personagem selecionado pelo jogador
        if (Game.SelectCharacter == Lists.Server_Data.Max_Characters)
            Game.SelectCharacter = 1;
        else
            Game.SelectCharacter += 1;
    }

    private static void Character_Change_Left()
    {
        // Altera o personagem selecionado pelo jogador
        if (Game.SelectCharacter == 1)
            Game.SelectCharacter = Lists.Server_Data.Max_Characters;
        else
            Game.SelectCharacter -= 1;
    }

    private static void Chat_Up()
    {
        // Sobe as linhas do chat
        if (Chat.Lines_First > 0)
            Chat.Lines_First -= 1;
    }

    private static void Chat_Down()
    {
        // Sobe as linhas do chat
        if (Chat.Order.Count - 1 - Chat.Lines_First - Chat.Lines_Visible > 0)
            Chat.Lines_First += 1;
    }

    private static void Menu_Character()
    {
        // Altera a visibilidade do painel e fecha os outros
        Panels.Get("Menu_Character").Visible = !Panels.Get("Menu_Character").Visible;
        Panels.Get("Menu_Inventory").Visible = false;
        Panels.Get("Menu_Options").Visible = false;
    }

    private static void Attribute_Strenght()
    {
        Send.AddPoint(Game.Attributes.Strength);
    }

    private static void Attribute_Resistance()
    {
        Send.AddPoint(Game.Attributes.Resistance);
    }

    private static void Attribute_Intelligence()
    {
        Send.AddPoint(Game.Attributes.Intelligence);
    }

    private static void Attribute_Agility()
    {
        Send.AddPoint(Game.Attributes.Agility);
    }

    private static void Attribute_Vitality()
    {
        Send.AddPoint(Game.Attributes.Vitality);
    }

    private static void Menu_Inventory()
    {
        // Altera a visibilidade do painel e fecha os outros
        Panels.Get("Menu_Inventory").Visible = !Panels.Get("Menu_Inventory").Visible;
        Panels.Get("Menu_Character").Visible = false;
        Panels.Get("Menu_Options").Visible = false;
    }

    private static void Menu_Options()
    {
        // Altera a visibilidade do painel e fecha os outros
        Panels.Get("Menu_Options").Visible = !Panels.Get("Menu_Options").Visible;
        Panels.Get("Menu_Character").Visible = false;
        Panels.Get("Menu_Inventory").Visible = false;
    }

    private static void Drop_Confirm()
    {
        // Quantidade
        short.TryParse(TextBoxes.Get("Drop_Amount").Text, out short Amount);

        // Verifica se o valor digitado é válidp
        if (Amount == 0)
        {
            MessageBox.Show("Enter a valid value!");
            return;
        }

        // Solta o item
        Send.DropItem(Game.Drop_Slot, Amount);
        Panels.Get("Drop").Visible = false;
    }

    private static void Drop_Cancel()
    {
        // Fecha o painel
        Panels.Get("Drop").Visible = false;
    }

    private static void Party_Yes()
    {
        // Aceita o grupo e fecha o painel
        Send.Party_Accept();
        Panels.Get("Party_Invitation").Visible = false;
    }

    private static void Party_No()
    {
        // Fecha o painel
        Send.Party_Decline();
        Panels.Get("Party_Invitation").Visible = false;
    }

    private static void Trade_Yes()
    {
        // Aceita o grupo e fecha o painel
        Send.Trade_Accept();
        Panels.Get("Trade_Invitation").Visible = false;
    }

    private static void Trade_No()
    {
        // Fecha o painel
        Send.Trade_Decline();
        Panels.Get("Trade_Invitation").Visible = false;
    }

    private static void Trade_Close()
    {
        // Fecha o painel
        Player.Me.Trade = 0;
        Send.Trade_Leave();
        Panels.Get("Trade").Visible = false;
    }

    private static void Trade_Offer_Accept()
    {
        // Aceita a oferta
        Get("Trade_Offer_Confirm").Visible = true;
        Get("Trade_Offer_Accept").Visible = Get("Trade_Offer_Decline").Visible = false;
        Panels.Get("Trade_Offer_Disable").Visible = false;
        Send.Trade_Offer_State(Game.Trade_Status.Accepted);

        // Limpa os dados da oferta
        Player.Trade_Offer = new Lists.Structures.Inventory[Game.Max_Inventory + 1];
        Player.Trade_Their_Offer = new Lists.Structures.Inventory[Game.Max_Inventory + 1];
    }

    private static void Trade_Offer_Decline()
    {
        // Recusa a oferta
        Get("Trade_Offer_Confirm").Visible = true;
        Get("Trade_Offer_Accept").Visible = Get("Trade_Offer_Decline").Visible = false;
        Panels.Get("Trade_Offer_Disable").Visible = false;
        Send.Trade_Offer_State(Game.Trade_Status.Declined);
    }

    public static void Trade_Offer_Confirm()
    {
        // Confirma a oferta
        Get("Trade_Offer_Confirm").Visible = Get("Trade_Offer_Accept").Visible = Get("Trade_Offer_Decline").Visible = false;
        Panels.Get("Trade_Offer_Disable").Visible = true;
        Send.Trade_Offer_State(Game.Trade_Status.Confirmed);
    }

    private static void Trade_Amount_Confirm()
    {
        // Quantidade
        short.TryParse(TextBoxes.Get("Trade_Amount").Text, out short Amount);

        // Verifica se o valor digitado é válido
        if (Amount <= 0 )
        {
            MessageBox.Show("Enter a valid value!");
            return;
        }

        // Solta o item
        Send.Trade_Offer(Game.Trade_Slot, Game.Trade_Inventory_Slot, Amount);
        Panels.Get("Trade_Amount").Visible = false;
    }

    private static void Trade_Amount_Cancel()
    {
        // Fecha o painel
        Panels.Get("Trade_Amount").Visible = false;
    }

    private static void Shop_Close()
    {
        // Fecha o painel
        Panels.Get("Shop").Visible = false;
        Send.Shop_Close();
    }

    private static void Shop_Sell_Confirm()
    {
        // Quantidade
        short.TryParse(TextBoxes.Get("Shop_Sell_Amount").Text, out short Amount);

        // Verifica se o valor digitado é válido
        if (Amount <= 0)
        {
            MessageBox.Show("Enter a valid value!");
            return;
        }

        // Vende o item
        Send.Shop_Sell(Game.Shop_Inventory_Slot, Amount);
        Panels.Get("Shop_Sell").Visible = false;
    }

    private static void Shop_Sell_Cancel()
    {
        // Fecha o painel
        Panels.Get("Shop_Sell").Visible = false;
    }
}