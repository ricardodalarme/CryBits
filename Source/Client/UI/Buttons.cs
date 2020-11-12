using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CryBits.Client.Entities;
using CryBits.Client.Media;
using CryBits.Client.Network;
using SFML.Window;
using static CryBits.Client.Logic.Game;
using static CryBits.Client.Logic.Utils;
using Graphics = CryBits.Client.Media.Graphics;

namespace CryBits.Client.UI
{
    class Buttons : Tools.Structure
    {
        // Aramazenamento de dados da ferramenta
        public static Dictionary<string, Buttons> List = new Dictionary<string, Buttons>();

        // Estados dos botões
        public enum States
        {
            Normal,
            Click,
            Above,
        }

        // Dados
        public byte Texture_Num;
        public States State;

        public void MouseUp()
        {
            // Somente se necessário
            if (!IsAbove(new Rectangle(Position, Graphics.Size(Graphics.Tex_Button[Texture_Num])))) return;

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
            if (!IsAbove(new Rectangle(Position, Graphics.Size(Graphics.Tex_Button[Texture_Num])))) return;

            // Altera o estado do botão
            State = States.Click;
        }

        public void MouseMove()
        {
            // Se o mouse não estiver sobre a ferramenta, então não executar o evento
            if (!IsAbove(new Rectangle(Position, Graphics.Size(Graphics.Tex_Button[Texture_Num]))))
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

        private static void Execute(string name)
        {
            // Executa o evento do botão
            switch (name)
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
            bool visibility = Panels.Characters != null && Panels.SelectCharacter < Panels.Characters.Length;
            List["Character_Create"].Visible = !visibility;
            List["Character_Delete"].Visible = visibility;
            List["Character_Use"].Visible = visibility;
            return visibility;
        }

        private static void Connect()
        {
            // Termina a conexão
            Socket.Disconnect();

            // Abre o painel
            Panels.Menu_Close();
            Panels.List["Connect"].Visible = true;
        }

        private static void Register()
        {
            // Termina a conexão
            Socket.Disconnect();

            // Abre o painel
            Panels.Menu_Close();
            Panels.List["Register"].Visible = true;
        }

        private static void Options()
        {
            // Termina a conexão
            Socket.Disconnect();

            // Define as marcações corretas
            CheckBoxes.List["Sounds"].Checked = Option.Sounds;
            CheckBoxes.List["Musics"].Checked = Option.Musics;

            // Abre o painel
            Panels.Menu_Close();
            Panels.List["Options"].Visible = true;
        }

        private static void Menu_Return()
        {
            // Termina a conexão
            Socket.Disconnect();

            // Abre o painel
            Panels.Menu_Close();
            Panels.List["Connect"].Visible = true;
        }

        private static void Connect_Ok()
        {
            // Salva o nome do usuário
            Option.Username = TextBoxes.List["Connect_Username"].Text;
            Library.Write.Options();

            // Conecta-se ao jogo
            if (Socket.TryConnect()) Send.Connect();
        }

        private static void Register_Ok()
        {
            // Regras de segurança
            if (TextBoxes.List["Register_Password"].Text != TextBoxes.List["Register_Password2"].Text)
            {
                MessageBox.Show("The password don't match.");
                return;
            }

            // Registra o jogador, se estiver tudo certo
            if (Socket.TryConnect()) Send.Register();
        }

        private static void CreateCharacter()
        {
            // Abre a criação de personagem
            if (Socket.TryConnect()) Send.CreateCharacter();
        }

        private static void CreateCharacter_ChangeRight()
        {
            // Altera a classe selecionada pelo jogador
            if (Panels.CreateCharacter_Class == Class.List.Count - 1)
                Panels.CreateCharacter_Class = 0;
            else
                Panels.CreateCharacter_Class += 1;
        }

        private static void CreateCharacter_ChangeLeft()
        {
            // Altera a classe selecionada pelo jogador
            if (Panels.CreateCharacter_Class == 0)
                Panels.CreateCharacter_Class = (byte)Class.List.Count;
            else
                Panels.CreateCharacter_Class -= 1;
        }

        private static void CreateCharacter_Texture_ChangeRight()
        {
            // Lista de texturas
            short[] texList;
            if (CheckBoxes.List["GenderMale"].Checked)
                texList = Class.List.ElementAt(Panels.CreateCharacter_Class).Value.Tex_Male;
            else
                texList = Class.List.ElementAt(Panels.CreateCharacter_Class).Value.Tex_Female;

            // Altera a classe selecionada pelo jogador
            if (Panels.CreateCharacter_Tex == texList.Length - 1)
                Panels.CreateCharacter_Tex = 0;
            else
                Panels.CreateCharacter_Tex += 1;
        }

        private static void CreateCharacter_Texture_ChangeLeft()
        {
            // Lista de texturas
            short[] texList;
            if (CheckBoxes.List["GenderMale"].Checked)
                texList = Class.List.ElementAt(Panels.CreateCharacter_Class).Value.Tex_Male;
            else
                texList = Class.List.ElementAt(Panels.CreateCharacter_Class).Value.Tex_Female;

            // Altera a classe selecionada pelo jogador
            if (Panels.CreateCharacter_Tex == 0)
                Panels.CreateCharacter_Tex = (byte)(texList.Length - 1);
            else
                Panels.CreateCharacter_Tex -= 1;
        }

        private static void CreateCharacter_Return()
        {
            // Abre o painel de personagens
            Panels.Menu_Close();
            Panels.List["SelectCharacter"].Visible = true;
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
            if (Panels.SelectCharacter == Panels.Characters.Length)
                Panels.SelectCharacter = 0;
            else
                Panels.SelectCharacter += 1;
        }

        private static void Character_Change_Left()
        {
            // Altera o personagem selecionado pelo jogador
            if (Panels.SelectCharacter == 0)
                Panels.SelectCharacter = Panels.Characters.Length;
            else
                Panels.SelectCharacter -= 1;
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
            if (Chat.Order.Count - 1 - Chat.Lines_First - Chat.LinesVisible > 0)
                Chat.Lines_First += 1;
        }

        private static void Menu_Character()
        {
            // Altera a visibilidade do painel e fecha os outros
            Panels.List["Menu_Character"].Visible = !Panels.List["Menu_Character"].Visible;
            Panels.List["Menu_Inventory"].Visible = false;
            Panels.List["Menu_Options"].Visible = false;
        }

        private static void Attribute_Strenght()
        {
            Send.AddPoint(Attributes.Strength);
        }

        private static void Attribute_Resistance()
        {
            Send.AddPoint(Attributes.Resistance);
        }

        private static void Attribute_Intelligence()
        {
            Send.AddPoint(Attributes.Intelligence);
        }

        private static void Attribute_Agility()
        {
            Send.AddPoint(Attributes.Agility);
        }

        private static void Attribute_Vitality()
        {
            Send.AddPoint(Attributes.Vitality);
        }

        private static void Menu_Inventory()
        {
            // Altera a visibilidade do painel e fecha os outros
            Panels.List["Menu_Inventory"].Visible = !Panels.List["Menu_Inventory"].Visible;
            Panels.List["Menu_Character"].Visible = false;
            Panels.List["Menu_Options"].Visible = false;
        }

        private static void Menu_Options()
        {
            // Altera a visibilidade do painel e fecha os outros
            Panels.List["Menu_Options"].Visible = !Panels.List["Menu_Options"].Visible;
            Panels.List["Menu_Character"].Visible = false;
            Panels.List["Menu_Inventory"].Visible = false;
        }

        private static void Drop_Confirm()
        {
            // Quantidade
            short.TryParse(TextBoxes.List["Drop_Amount"].Text, out short amount);

            // Verifica se o valor digitado é válidp
            if (amount == 0)
            {
                MessageBox.Show("Enter a valid value!");
                return;
            }

            // Solta o item
            Send.DropItem(Panels.Drop_Slot, amount);
            Panels.List["Drop"].Visible = false;
        }

        private static void Drop_Cancel()
        {
            // Fecha o painel
            Panels.List["Drop"].Visible = false;
        }

        private static void Party_Yes()
        {
            // Aceita o grupo e fecha o painel
            Send.Party_Accept();
            Panels.List["Party_Invitation"].Visible = false;
        }

        private static void Party_No()
        {
            // Fecha o painel
            Send.Party_Decline();
            Panels.List["Party_Invitation"].Visible = false;
        }

        private static void Trade_Yes()
        {
            // Aceita o grupo e fecha o painel
            Send.Trade_Accept();
            Panels.List["Trade_Invitation"].Visible = false;
        }

        private static void Trade_No()
        {
            // Fecha o painel
            Send.Trade_Decline();
            Panels.List["Trade_Invitation"].Visible = false;
        }

        private static void Trade_Close()
        {
            // Fecha o painel
            Send.Trade_Leave();
            Panels.List["Trade"].Visible = false;
        }

        private static void Trade_Offer_Accept()
        {
            // Aceita a oferta
            List["Trade_Offer_Confirm"].Visible = true;
            List["Trade_Offer_Accept"].Visible = List["Trade_Offer_Decline"].Visible = false;
            Panels.List["Trade_Offer_Disable"].Visible = false;
            Send.Trade_Offer_State(TradeStatus.Accepted);

            // Limpa os dados da oferta
            Player.Me.Trade_Offer = new Inventory[MaxInventory + 1];
            Player.Me.Trade_Their_Offer = new Inventory[MaxInventory + 1];
        }

        private static void Trade_Offer_Decline()
        {
            // Recusa a oferta
            List["Trade_Offer_Confirm"].Visible = true;
            List["Trade_Offer_Accept"].Visible = List["Trade_Offer_Decline"].Visible = false;
            Panels.List["Trade_Offer_Disable"].Visible = false;
            Send.Trade_Offer_State(TradeStatus.Declined);
        }

        public static void Trade_Offer_Confirm()
        {
            // Confirma a oferta
            List["Trade_Offer_Confirm"].Visible = List["Trade_Offer_Accept"].Visible = List["Trade_Offer_Decline"].Visible = false;
            Panels.List["Trade_Offer_Disable"].Visible = true;
            Send.Trade_Offer_State(TradeStatus.Confirmed);
        }

        private static void Trade_Amount_Confirm()
        {
            // Quantidade
            short.TryParse(TextBoxes.List["Trade_Amount"].Text, out short amount);

            // Verifica se o valor digitado é válido
            if (amount <= 0)
            {
                MessageBox.Show("Enter a valid value!");
                return;
            }

            // Solta o item
            Send.Trade_Offer(Panels.Trade_Slot, Panels.Trade_Inventory_Slot, amount);
            Panels.List["Trade_Amount"].Visible = false;
        }

        private static void Trade_Amount_Cancel()
        {
            // Fecha o painel
            Panels.List["Trade_Amount"].Visible = false;
        }

        private static void Shop_Close()
        {
            // Fecha o painel
            Panels.List["Shop"].Visible = false;
            Send.Shop_Close();
        }

        private static void Shop_Sell_Confirm()
        {
            // Quantidade
            short.TryParse(TextBoxes.List["Shop_Sell_Amount"].Text, out short amount);

            // Verifica se o valor digitado é válido
            if (amount <= 0)
            {
                MessageBox.Show("Enter a valid value!");
                return;
            }

            // Vende o item
            Send.Shop_Sell(Panels.Shop_Inventory_Slot, amount);
            Panels.List["Shop_Sell"].Visible = false;
        }

        private static void Shop_Sell_Cancel()
        {
            // Fecha o painel
            Panels.List["Shop_Sell"].Visible = false;
        }
    }
}