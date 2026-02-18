using System.Linq;
using CryBits.Client.Utils;
using CryBits.Client.Entities;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Library;
using CryBits.Client.Framework.Library.Repositories;
using CryBits.Client.Network;
using CryBits.Client.Network.Senders;
using CryBits.Entities;
using CryBits.Entities.Slots;
using CryBits.Enums;
using static CryBits.Globals;

namespace CryBits.Client.UI.Events;

internal static class ButtonsEvents
{
    public static void Bind()
    {
        Buttons.Connect.OnMouseUp += Connect;
        Buttons.Register.OnMouseUp += Register;
        Buttons.Options.OnMouseUp += Options;
        Buttons.OptionsBack.OnMouseUp += MenuReturn;
        Buttons.ConnectConfirm.OnMouseUp += ConnectOk;
        Buttons.RegisterConfirm.OnMouseUp += RegisterOk;
        Buttons.CreateCharacter.OnMouseUp += CreateCharacter;
        Buttons.CreateCharacterChangeRight.OnMouseUp += CreateCharacterChangeRight;
        Buttons.CreateCharacterChangeLeft.OnMouseUp += CreateCharacterChangeLeft;
        Buttons.CreateCharacterTextureChangeLeft.OnMouseUp += CreateCharacterTextureChangeLeft;
        Buttons.CreateCharacterTextureChangeRight.OnMouseUp += CreateCharacterTextureChangeRight;
        Buttons.CreateCharacterBack.OnMouseUp += CreateCharacter_Return;
        Buttons.CharacterUse.OnMouseUp += CharacterUse;
        Buttons.CharacterCreate.OnMouseUp += CharacterCreate;
        Buttons.CharacterDelete.OnMouseUp += CharacterDelete;
        Buttons.CharacterChangeRight.OnMouseUp += CharacterChangeRight;
        Buttons.CharacterChangeLeft.OnMouseUp += CharacterChangeLeft;
        Buttons.ChatUp.OnMouseUp += ChatUp;
        Buttons.ChatDown.OnMouseUp += ChatDown;
        Buttons.MenuCharacter.OnMouseUp += MenuCharacter;
        Buttons.AttributesStrength.OnMouseUp += AttributeStrength;
        Buttons.AttributesResistance.OnMouseUp += AttributeResistance;
        Buttons.AttributesIntelligence.OnMouseUp += AttributeIntelligence;
        Buttons.AttributesAgility.OnMouseUp += AttributeAgility;
        Buttons.AttributesVitality.OnMouseUp += AttributeVitality;
        Buttons.MenuInventory.OnMouseUp += MenuInventory;
        Buttons.MenuOptions.OnMouseUp += MenuOptions;
        Buttons.DropConfirm.OnMouseUp += DropConfirm;
        Buttons.DropCancel.OnMouseUp += DropCancel;
        Buttons.PartyYes.OnMouseUp += PartyYes;
        Buttons.PartyNo.OnMouseUp += PartyNo;
        Buttons.TradeYes.OnMouseUp += TradeYes;
        Buttons.TradeNo.OnMouseUp += TradeNo;
        Buttons.TradeClose.OnMouseUp += TradeClose;
        Buttons.TradeOfferAccept.OnMouseUp += TradeOfferAccept;
        Buttons.TradeOfferDecline.OnMouseUp += TradeOfferDecline;
        Buttons.TradeOfferConfirm.OnMouseUp += TradeOfferConfirm;
        Buttons.TradeAmountConfirm.OnMouseUp += TradeAmountConfirm;
        Buttons.TradeAmountCancel.OnMouseUp += TradeAmountCancel;
        Buttons.ShopClose.OnMouseUp += ShopClose;
        Buttons.ShopSellConfirm.OnMouseUp += ShopSellConfirm;
        Buttons.ShopSellCancel.OnMouseUp += ShopSellCancel;
    }

    public static bool Characters_Change_Buttons()
    {
        // Altera os botões visíveis
        var visibility = PanelsEvents.Characters != null &&
                         PanelsEvents.SelectCharacter < PanelsEvents.Characters.Length;
        Buttons.CharacterCreate.Visible = !visibility;
        Buttons.CharacterDelete.Visible = visibility;
        Buttons.CharacterUse.Visible = visibility;
        return visibility;
    }

    private static void Connect()
    {
        // Termina a conexão
        Socket.Disconnect();

        // Abre o painel
        PanelsEvents.MenuClose();
        Panels.Connect.Visible = true;
    }

    private static void Register()
    {
        // Termina a conexão
        Socket.Disconnect();

        // Abre o painel
        PanelsEvents.MenuClose();
        Panels.Register.Visible = true;
    }

    private static void Options()
    {
        // Termina a conexão
        Socket.Disconnect();

        // Define as marcações corretas
        CheckBoxes.Sounds.Checked = Framework.Options.Sounds;
        CheckBoxes.Musics.Checked = Framework.Options.Musics;

        // Abre o painel
        PanelsEvents.MenuClose();
        Panels.Options.Visible = true;
    }

    private static void MenuReturn()
    {
        // Termina a conexão
        Socket.Disconnect();

        // Abre o painel
        PanelsEvents.MenuClose();
        Panels.Connect.Visible = true;
    }

    private static void ConnectOk()
    {
        // Salva o nome do usuário
        Framework.Options.Username = TextBoxes.ConnectUsername.Text;
        OptionsRepository.Write();

        // Conecta-se ao jogo
        if (Socket.TryConnect()) AuthSender.Connect();
    }

    private static void RegisterOk()
    {
        // Regras de segurança
        if (TextBoxes.RegisterPassword.Text != TextBoxes.RegisterPassword2.Text)
        {
            Alert.Show("The password don't match.");
            return;
        }

        // Registra o jogador, se estiver tudo certo
        if (Socket.TryConnect()) AuthSender.Register();
    }

    private static void CreateCharacter()
    {
        // Abre a criação de personagem
        if (Socket.TryConnect()) AccountSender.CreateCharacter();
    }

    private static void CreateCharacterChangeRight()
    {
        // Altera a classe selecionada pelo jogador
        if (PanelsEvents.CreateCharacterClass == Class.List.Count - 1)
            PanelsEvents.CreateCharacterClass = 0;
        else
            PanelsEvents.CreateCharacterClass++;
    }

    private static void CreateCharacterChangeLeft()
    {
        // Altera a classe selecionada pelo jogador
        if (PanelsEvents.CreateCharacterClass == 0)
            PanelsEvents.CreateCharacterClass = (byte)Class.List.Count;
        else
            PanelsEvents.CreateCharacterClass--;
    }

    private static void CreateCharacterTextureChangeRight()
    {
        // Lista de texturas
        var @class = Class.List.ElementAt(PanelsEvents.CreateCharacterClass).Value;
        var texList = CheckBoxes.GenderMale.Checked ? @class.TextureMale : @class.TextureFemale;

        // Altera a classe selecionada pelo jogador
        if (PanelsEvents.CreateCharacterTex == texList.Count - 1)
            PanelsEvents.CreateCharacterTex = 0;
        else
            PanelsEvents.CreateCharacterTex++;
    }

    private static void CreateCharacterTextureChangeLeft()
    {
        // Lista de texturas
        var @class = Class.List.ElementAt(PanelsEvents.CreateCharacterClass).Value;
        var texList = CheckBoxes.GenderMale.Checked ? @class.TextureMale : @class.TextureFemale;

        // Altera a classe selecionada pelo jogador
        if (PanelsEvents.CreateCharacterTex == 0)
            PanelsEvents.CreateCharacterTex = (byte)(texList.Count - 1);
        else
            PanelsEvents.CreateCharacterTex--;
    }

    private static void CreateCharacter_Return()
    {
        // Abre o painel de personagens
        PanelsEvents.MenuClose();
        Panels.SelectCharacter.Visible = true;
    }

    private static void CharacterUse()
    {
        // Usa o personagem selecionado
        AccountSender.CharacterUse();
    }

    private static void CharacterDelete()
    {
        // Deleta o personagem selecionado
        AccountSender.CharacterDelete();
    }

    private static void CharacterCreate()
    {
        // Abre a criação de personagem
        AccountSender.CharacterCreate();
    }

    private static void CharacterChangeRight()
    {
        // Altera o personagem selecionado pelo jogador
        if (PanelsEvents.SelectCharacter == PanelsEvents.Characters.Length)
            PanelsEvents.SelectCharacter = 0;
        else
            PanelsEvents.SelectCharacter++;
    }

    private static void CharacterChangeLeft()
    {
        // Altera o personagem selecionado pelo jogador
        if (PanelsEvents.SelectCharacter == 0)
            PanelsEvents.SelectCharacter = PanelsEvents.Characters.Length;
        else
            PanelsEvents.SelectCharacter--;
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
        Panels.MenuCharacter.Visible = !Panels.MenuCharacter.Visible;
        Panels.MenuInventory.Visible = false;
        Panels.MenuOptions.Visible = false;
    }

    private static void AttributeStrength()
    {
        PlayerSender.AddPoint(Attribute.Strength);
    }

    private static void AttributeResistance()
    {
        PlayerSender.AddPoint(Attribute.Resistance);
    }

    private static void AttributeIntelligence()
    {
        PlayerSender.AddPoint(Attribute.Intelligence);
    }

    private static void AttributeAgility()
    {
        PlayerSender.AddPoint(Attribute.Agility);
    }

    private static void AttributeVitality()
    {
        PlayerSender.AddPoint(Attribute.Vitality);
    }

    private static void MenuInventory()
    {
        // Altera a visibilidade do painel e fecha os outros
        Panels.MenuInventory.Visible = !Panels.MenuInventory.Visible;
        Panels.MenuCharacter.Visible = false;
        Panels.MenuOptions.Visible = false;
    }

    private static void MenuOptions()
    {
        // Altera a visibilidade do painel e fecha os outros
        Panels.MenuOptions.Visible = !Panels.MenuOptions.Visible;
        Panels.MenuCharacter.Visible = false;
        Panels.MenuInventory.Visible = false;
    }

    private static void DropConfirm()
    {
        // Verifica se o valor digitado é válidp
        if (!short.TryParse(TextBoxes.DropAmount.Text, out var amount) || amount <= 0)
        {
            Alert.Show("Enter a valid value!");
            return;
        }

        // Solta o item
        PlayerSender.DropItem(PanelsEvents.DropSlot, amount);
        Panels.Drop.Visible = false;
    }

    private static void DropCancel()
    {
        // Fecha o painel
        Panels.Drop.Visible = false;
    }

    private static void PartyYes()
    {
        // Aceita o grupo e fecha o painel
        PartySender.PartyAccept();
        Panels.PartyInvitation.Visible = false;
    }

    private static void PartyNo()
    {
        // Fecha o painel
        PartySender.PartyDecline();
        Panels.PartyInvitation.Visible = false;
    }

    private static void TradeYes()
    {
        // Aceita o grupo e fecha o painel
        TradeSender.TradeAccept();
        Panels.TradeInvitation.Visible = false;
    }

    private static void TradeNo()
    {
        // Fecha o painel
        TradeSender.TradeDecline();
        Panels.TradeInvitation.Visible = false;
    }

    private static void TradeClose()
    {
        // Fecha o painel
        TradeSender.TradeLeave();
        Panels.Trade.Visible = false;
    }

    private static void TradeOfferAccept()
    {
        // Aceita a oferta
        Buttons.TradeOfferConfirm.Visible = true;
        Buttons.TradeOfferAccept.Visible = Buttons.TradeOfferDecline.Visible = false;
        Panels.TradeOfferDisable.Visible = false;
        TradeSender.TradeOfferState(TradeStatus.Accepted);

        // Limpa os dados da oferta
        Player.Me.TradeOffer = new ItemSlot[MaxInventory];
        Player.Me.TradeTheirOffer = new ItemSlot[MaxInventory];
    }

    private static void TradeOfferDecline()
    {
        // Recusa a oferta
        Buttons.TradeOfferConfirm.Visible = true;
        Buttons.TradeOfferAccept.Visible = Buttons.TradeOfferDecline.Visible = false;
        Panels.TradeOfferDisable.Visible = false;
        TradeSender.TradeOfferState(TradeStatus.Declined);
    }

    private static void TradeOfferConfirm()
    {
        // Confirma a oferta
        Buttons.TradeOfferConfirm.Visible =
            Buttons.TradeOfferAccept.Visible = Buttons.TradeOfferDecline.Visible = false;
        Panels.TradeOfferDisable.Visible = true;
        TradeSender.TradeOfferState(TradeStatus.Confirmed);
    }

    private static void TradeAmountConfirm()
    {
        // Verifica se o valor digitado é válido
        if (!short.TryParse(TextBoxes.TradeAmount.Text, out var amount) || amount <= 0)
        {
            Alert.Show("Enter a valid value!");
            return;
        }

        // Solta o item
        TradeSender.TradeOffer(PanelsEvents.TradeSlot, PanelsEvents.TradeInventorySlot,
            amount);
        Panels.TradeAmount.Visible = false;
    }

    private static void TradeAmountCancel()
    {
        // Fecha o painel
        Panels.TradeAmount.Visible = false;
    }

    private static void ShopClose()
    {
        // Fecha o painel
        Panels.Shop.Visible = false;
        ShopSender.ShopClose();
    }

    private static void ShopSellConfirm()
    {
        // Verifica se o valor digitado é válido
        if (!short.TryParse(TextBoxes.ShopSellAmount.Text, out var amount) || amount <= 0)
        {
            Alert.Show("Enter a valid value!");
            return;
        }

        // Vende o item
        ShopSender.ShopSell(PanelsEvents.ShopInventorySlot, amount);
        Panels.ShopSell.Visible = false;
    }

    private static void ShopSellCancel()
    {
        // Fecha o painel
        Panels.ShopSell.Visible = false;
    }
}
