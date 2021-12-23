using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CryBits.Client.Entities;
using CryBits.Client.Library;
using CryBits.Client.Media.Graphics;
using CryBits.Client.Network;
using CryBits.Entities;
using CryBits.Enums;
using SFML.Window;
using static CryBits.Client.Logic.Utils;
using static CryBits.Globals;
using Sound = CryBits.Client.Media.Audio.Sound;

namespace CryBits.Client.UI;

internal class Buttons : Tools.Structure
{
    // Aramazenamento de dados da ferramenta
    public static Dictionary<string, Buttons> List = new();

    // Estados dos botões
    public enum States
    {
        Normal,
        Click,
        Above
    }

    // Dados
    public byte TextureNum;
    public States State;

    public void MouseUp()
    {
        // Somente se necessário
        if (!IsAbove(new Rectangle(Position, Textures.Buttons[TextureNum].ToSize()))) return;

        // Altera o estado do botão
        Sound.Play(Enums.Sound.Click);
        State = States.Above;

        // Executa o evento
        Execute(Name);
    }

    public void MouseDown(MouseButtonEventArgs e)
    {
        // Somente se necessário
        if (e.Button == Mouse.Button.Right) return;
        if (!IsAbove(new Rectangle(Position, Textures.Buttons[TextureNum].ToSize()))) return;

        // Altera o estado do botão
        State = States.Click;
    }

    public void MouseMove()
    {
        // Se o mouse não estiver sobre a ferramenta, então não executar o evento
        if (!IsAbove(new Rectangle(Position, Textures.Buttons[TextureNum].ToSize())))
        {
            State = States.Normal;
            return;
        }

        // Se o botão já estiver no estado normal, isso não é necessário
        if (State != States.Normal) return;

        // Altera o estado do botão
        State = States.Above;
        Sound.Play(Enums.Sound.Above);
    }

    private static void Execute(string name)
    {
        // Executa o evento do botão
        switch (name)
        {
            case "Connect": Connect(); break;
            case "Register": Register(); break;
            case "Options": Options(); break;
            case "Options_Back": MenuReturn(); break;
            case "Connect_Confirm": ConnectOk(); break;
            case "Register_Confirm": RegisterOk(); break;
            case "CreateCharacter": CreateCharacter(); break;
            case "CreateCharacter_ChangeRight": CreateCharacterChangeRight(); break;
            case "CreateCharacter_ChangeLeft": CreateCharacterChangeLeft(); break;
            case "CreateCharacter_Texture_ChangeLeft": CreateCharacterTextureChangeLeft(); break;
            case "CreateCharacter_Texture_ChangeRight": CreateCharacterTextureChangeRight(); break;
            case "CreateCharacter_Back": CreateCharacter_Return(); break;
            case "Character_Use": CharacterUse(); break;
            case "Character_Create": CharacterCreate(); break;
            case "Character_Delete": CharacterDelete(); break;
            case "Character_ChangeRight": CharacterChangeRight(); break;
            case "Character_ChangeLeft": CharacterChangeLeft(); break;
            case "Chat_Up": ChatUp(); break;
            case "Chat_Down": ChatDown(); break;
            case "Menu_Character": MenuCharacter(); break;
            case "Attributes_Strength": AttributeStrength(); break;
            case "Attributes_Resistance": AttributeResistance(); break;
            case "Attributes_Intelligence": AttributeIntelligence(); break;
            case "Attributes_Agility": AttributeAgility(); break;
            case "Attributes_Vitality": AttributeVitality(); break;
            case "Menu_Inventory": MenuInventory(); break;
            case "Menu_Options": MenuOptions(); break;
            case "Drop_Confirm": DropConfirm(); break;
            case "Drop_Cancel": DropCancel(); break;
            case "Party_Yes": PartyYes(); break;
            case "Party_No": PartyNo(); break;
            case "Trade_Yes": TradeYes(); break;
            case "Trade_No": TradeNo(); break;
            case "Trade_Close": TradeClose(); break;
            case "Trade_Offer_Accept": TradeOfferAccept(); break;
            case "Trade_Offer_Decline": TradeOfferDecline(); break;
            case "Trade_Offer_Confirm": TradeOfferConfirm(); break;
            case "Trade_Amount_Confirm": TradeAmountConfirm(); break;
            case "Trade_Amount_Cancel": TradeAmountCancel(); break;
            case "Shop_Close": ShopClose(); break;
            case "Shop_Sell_Confirm": ShopSellConfirm(); break;
            case "Shop_Sell_Cancel": ShopSellCancel(); break;
        }
    }

    public static bool Characters_Change_Buttons()
    {
        // Altera os botões visíveis
        var visibility = Panels.Characters != null && Panels.SelectCharacter < Panels.Characters.Length;
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
        Panels.MenuClose();
        Panels.List["Connect"].Visible = true;
    }

    private static void Register()
    {
        // Termina a conexão
        Socket.Disconnect();

        // Abre o painel
        Panels.MenuClose();
        Panels.List["Register"].Visible = true;
    }

    private static void Options()
    {
        // Termina a conexão
        Socket.Disconnect();

        // Define as marcações corretas
        CheckBoxes.List["Sounds"].Checked = Logic.Options.Sounds;
        CheckBoxes.List["Musics"].Checked = Logic.Options.Musics;

        // Abre o painel
        Panels.MenuClose();
        Panels.List["Options"].Visible = true;
    }

    private static void MenuReturn()
    {
        // Termina a conexão
        Socket.Disconnect();

        // Abre o painel
        Panels.MenuClose();
        Panels.List["Connect"].Visible = true;
    }

    private static void ConnectOk()
    {
        // Salva o nome do usuário
        Logic.Options.Username = TextBoxes.List["Connect_Username"].Text;
        Write.Options();

        // Conecta-se ao jogo
        if (Socket.TryConnect()) Send.Connect();
    }

    private static void RegisterOk()
    {
        // Regras de segurança
        if (TextBoxes.List["Register_Password"].Text != TextBoxes.List["Register_Password2"].Text)
        {
            MessageBox.Show(@"The password don't match.");
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

    private static void CreateCharacterChangeRight()
    {
        // Altera a classe selecionada pelo jogador
        if (Panels.CreateCharacterClass == Class.List.Count - 1)
            Panels.CreateCharacterClass = 0;
        else
            Panels.CreateCharacterClass++;
    }

    private static void CreateCharacterChangeLeft()
    {
        // Altera a classe selecionada pelo jogador
        if (Panels.CreateCharacterClass == 0)
            Panels.CreateCharacterClass = (byte)Class.List.Count;
        else
            Panels.CreateCharacterClass--;
    }

    private static void CreateCharacterTextureChangeRight()
    {
        // Lista de texturas
        var @class = Class.List.ElementAt(Panels.CreateCharacterClass).Value;
        var texList = CheckBoxes.List["GenderMale"].Checked ? @class.TextureMale : @class.TextureFemale;

        // Altera a classe selecionada pelo jogador
        if (Panels.CreateCharacterTex == texList.Count - 1)
            Panels.CreateCharacterTex = 0;
        else
            Panels.CreateCharacterTex++;
    }

    private static void CreateCharacterTextureChangeLeft()
    {
        // Lista de texturas
        var @class = Class.List.ElementAt(Panels.CreateCharacterClass).Value;
        var texList = CheckBoxes.List["GenderMale"].Checked ? @class.TextureMale : @class.TextureFemale;

        // Altera a classe selecionada pelo jogador
        if (Panels.CreateCharacterTex == 0)
            Panels.CreateCharacterTex = (byte)(texList.Count - 1);
        else
            Panels.CreateCharacterTex--;
    }

    private static void CreateCharacter_Return()
    {
        // Abre o painel de personagens
        Panels.MenuClose();
        Panels.List["SelectCharacter"].Visible = true;
    }

    private static void CharacterUse()
    {
        // Usa o personagem selecionado
        Send.CharacterUse();
    }

    private static void CharacterDelete()
    {
        // Deleta o personagem selecionado
        Send.CharacterDelete();
    }

    private static void CharacterCreate()
    {
        // Abre a criação de personagem
        Send.CharacterCreate();
    }

    private static void CharacterChangeRight()
    {
        // Altera o personagem selecionado pelo jogador
        if (Panels.SelectCharacter == Panels.Characters.Length)
            Panels.SelectCharacter = 0;
        else
            Panels.SelectCharacter++;
    }

    private static void CharacterChangeLeft()
    {
        // Altera o personagem selecionado pelo jogador
        if (Panels.SelectCharacter == 0)
            Panels.SelectCharacter = Panels.Characters.Length;
        else
            Panels.SelectCharacter--;
    }

    private static void ChatUp()
    {
        // Sobe as linhas do chat
        if (Chat.LinesFirst > 0)
            Chat.LinesFirst--;
    }

    private static void ChatDown()
    {
        // Sobe as linhas do chat
        if (Chat.Order.Count - 1 - Chat.LinesFirst - Chat.LinesVisible > 0)
            Chat.LinesFirst++;
    }

    private static void MenuCharacter()
    {
        // Altera a visibilidade do painel e fecha os outros
        Panels.List["Menu_Character"].Visible = !Panels.List["Menu_Character"].Visible;
        Panels.List["Menu_Inventory"].Visible = false;
        Panels.List["Menu_Options"].Visible = false;
    }

    private static void AttributeStrength()
    {
        Send.AddPoint(Attribute.Strength);
    }

    private static void AttributeResistance()
    {
        Send.AddPoint(Attribute.Resistance);
    }

    private static void AttributeIntelligence()
    {
        Send.AddPoint(Attribute.Intelligence);
    }

    private static void AttributeAgility()
    {
        Send.AddPoint(Attribute.Agility);
    }

    private static void AttributeVitality()
    {
        Send.AddPoint(Attribute.Vitality);
    }

    private static void MenuInventory()
    {
        // Altera a visibilidade do painel e fecha os outros
        Panels.List["Menu_Inventory"].Visible = !Panels.List["Menu_Inventory"].Visible;
        Panels.List["Menu_Character"].Visible = false;
        Panels.List["Menu_Options"].Visible = false;
    }

    private static void MenuOptions()
    {
        // Altera a visibilidade do painel e fecha os outros
        Panels.List["Menu_Options"].Visible = !Panels.List["Menu_Options"].Visible;
        Panels.List["Menu_Character"].Visible = false;
        Panels.List["Menu_Inventory"].Visible = false;
    }

    private static void DropConfirm()
    {
        // Quantidade
        short.TryParse(TextBoxes.List["Drop_Amount"].Text, out var amount);

        // Verifica se o valor digitado é válidp
        if (amount == 0)
        {
            MessageBox.Show(@"Enter a valid value!");
            return;
        }

        // Solta o item
        Send.DropItem(Panels.DropSlot, amount);
        Panels.List["Drop"].Visible = false;
    }

    private static void DropCancel()
    {
        // Fecha o painel
        Panels.List["Drop"].Visible = false;
    }

    private static void PartyYes()
    {
        // Aceita o grupo e fecha o painel
        Send.PartyAccept();
        Panels.List["Party_Invitation"].Visible = false;
    }

    private static void PartyNo()
    {
        // Fecha o painel
        Send.PartyDecline();
        Panels.List["Party_Invitation"].Visible = false;
    }

    private static void TradeYes()
    {
        // Aceita o grupo e fecha o painel
        Send.TradeAccept();
        Panels.List["Trade_Invitation"].Visible = false;
    }

    private static void TradeNo()
    {
        // Fecha o painel
        Send.TradeDecline();
        Panels.List["Trade_Invitation"].Visible = false;
    }

    private static void TradeClose()
    {
        // Fecha o painel
        Send.TradeLeave();
        Panels.List["Trade"].Visible = false;
    }

    private static void TradeOfferAccept()
    {
        // Aceita a oferta
        List["Trade_Offer_Confirm"].Visible = true;
        List["Trade_Offer_Accept"].Visible = List["Trade_Offer_Decline"].Visible = false;
        Panels.List["Trade_Offer_Disable"].Visible = false;
        Send.TradeOfferState(TradeStatus.Accepted);

        // Limpa os dados da oferta
        Player.Me.TradeOffer = new ItemSlot[MaxInventory];
        Player.Me.TradeTheirOffer = new ItemSlot[MaxInventory];
    }

    private static void TradeOfferDecline()
    {
        // Recusa a oferta
        List["Trade_Offer_Confirm"].Visible = true;
        List["Trade_Offer_Accept"].Visible = List["Trade_Offer_Decline"].Visible = false;
        Panels.List["Trade_Offer_Disable"].Visible = false;
        Send.TradeOfferState(TradeStatus.Declined);
    }

    private static void TradeOfferConfirm()
    {
        // Confirma a oferta
        List["Trade_Offer_Confirm"].Visible = List["Trade_Offer_Accept"].Visible = List["Trade_Offer_Decline"].Visible = false;
        Panels.List["Trade_Offer_Disable"].Visible = true;
        Send.TradeOfferState(TradeStatus.Confirmed);
    }

    private static void TradeAmountConfirm()
    {
        // Quantidade
        short.TryParse(TextBoxes.List["Trade_Amount"].Text, out var amount);

        // Verifica se o valor digitado é válido
        if (amount <= 0)
        {
            MessageBox.Show(@"Enter a valid value!");
            return;
        }

        // Solta o item
        Send.TradeOffer(Panels.TradeSlot, Panels.TradeInventorySlot, amount);
        Panels.List["Trade_Amount"].Visible = false;
    }

    private static void TradeAmountCancel()
    {
        // Fecha o painel
        Panels.List["Trade_Amount"].Visible = false;
    }

    private static void ShopClose()
    {
        // Fecha o painel
        Panels.List["Shop"].Visible = false;
        Send.ShopClose();
    }

    private static void ShopSellConfirm()
    {
        // Quantidade
        short.TryParse(TextBoxes.List["Shop_Sell_Amount"].Text, out var amount);

        // Verifica se o valor digitado é válido
        if (amount <= 0)
        {
            MessageBox.Show(@"Enter a valid value!");
            return;
        }

        // Vende o item
        Send.ShopSell(Panels.ShopInventorySlot, amount);
        Panels.List["Shop_Sell"].Visible = false;
    }

    private static void ShopSellCancel()
    {
        // Fecha o painel
        Panels.List["Shop_Sell"].Visible = false;
    }
}