using System.Linq;
using CryBits.Client.Entities;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Persistence.Repositories;
using CryBits.Client.Network;
using CryBits.Client.Network.Senders;
using CryBits.Client.Utils;
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

    /// <summary>
    /// Update visibility of character management buttons based on selection state.
    /// </summary>
    public static bool Characters_Change_Buttons()
    {
        var visibility = PanelsEvents.Characters != null &&
                         PanelsEvents.SelectCharacter < PanelsEvents.Characters.Length;
        Buttons.CharacterCreate.Visible = !visibility;
        Buttons.CharacterDelete.Visible = visibility;
        Buttons.CharacterUse.Visible = visibility;
        return visibility;
    }

    private static void Connect()
    {
        Socket.Disconnect();

        PanelsEvents.MenuClose();
        Panels.Connect.Visible = true;
    }

    private static void Register()
    {
        Socket.Disconnect();

        PanelsEvents.MenuClose();
        Panels.Register.Visible = true;
    }

    private static void Options()
    {
        Socket.Disconnect();

        CheckBoxes.Sounds.Checked = Framework.Options.Sounds;
        CheckBoxes.Musics.Checked = Framework.Options.Musics;

        PanelsEvents.MenuClose();
        Panels.Options.Visible = true;
    }

    private static void MenuReturn()
    {
        Socket.Disconnect();

        PanelsEvents.MenuClose();
        Panels.Connect.Visible = true;
    }

    private static void ConnectOk()
    {
        // Save username
        Framework.Options.Username = TextBoxes.ConnectUsername.Text;
        OptionsRepository.Write();

        // Connect to game
        if (Socket.TryConnect()) AuthSender.Connect();
    }

    private static void RegisterOk()
    {
        // Basic validation
        if (TextBoxes.RegisterPassword.Text != TextBoxes.RegisterPassword2.Text)
        {
            Alert.Show("The password don't match.");
            return;
        }

        if (Socket.TryConnect()) AuthSender.Register();
    }

    private static void CreateCharacter()
    {
        // Open character creation
        if (Socket.TryConnect()) AccountSender.CreateCharacter();
    }

    private static void CreateCharacterChangeRight()
    {
        // Cycle selected class to the right
        if (PanelsEvents.CreateCharacterClass == Class.List.Count - 1)
            PanelsEvents.CreateCharacterClass = 0;
        else
            PanelsEvents.CreateCharacterClass++;
    }

    private static void CreateCharacterChangeLeft()
    {
        // Cycle selected class to the left
        if (PanelsEvents.CreateCharacterClass == 0)
            PanelsEvents.CreateCharacterClass = (byte)Class.List.Count;
        else
            PanelsEvents.CreateCharacterClass--;
    }

    private static void CreateCharacterTextureChangeRight()
    {
        var @class = Class.List.ElementAt(PanelsEvents.CreateCharacterClass).Value;
        var texList = CheckBoxes.GenderMale.Checked ? @class.TextureMale : @class.TextureFemale;

        if (PanelsEvents.CreateCharacterTex == texList.Count - 1)
            PanelsEvents.CreateCharacterTex = 0;
        else
            PanelsEvents.CreateCharacterTex++;
    }

    private static void CreateCharacterTextureChangeLeft()
    {
        var @class = Class.List.ElementAt(PanelsEvents.CreateCharacterClass).Value;
        var texList = CheckBoxes.GenderMale.Checked ? @class.TextureMale : @class.TextureFemale;

        if (PanelsEvents.CreateCharacterTex == 0)
            PanelsEvents.CreateCharacterTex = (byte)(texList.Count - 1);
        else
            PanelsEvents.CreateCharacterTex--;
    }

    private static void CreateCharacter_Return()
    {
        // Open character panel
        PanelsEvents.MenuClose();
        Panels.SelectCharacter.Visible = true;
    }

    private static void CharacterUse()
    {
        AccountSender.CharacterUse();
    }

    private static void CharacterDelete()
    {
        AccountSender.CharacterDelete();
    }

    private static void CharacterCreate()
    {
        AccountSender.CharacterCreate();
    }

    private static void CharacterChangeRight()
    {
        if (PanelsEvents.SelectCharacter == PanelsEvents.Characters.Length)
            PanelsEvents.SelectCharacter = 0;
        else
            PanelsEvents.SelectCharacter++;
    }

    private static void CharacterChangeLeft()
    {
        if (PanelsEvents.SelectCharacter == 0)
            PanelsEvents.SelectCharacter = PanelsEvents.Characters.Length;
        else
            PanelsEvents.SelectCharacter--;
    }

    private static void ChatUp()
    {
        if (Chat.LinesFirst > 0)
            Chat.LinesFirst--;
    }

    private static void ChatDown()
    {
        if (Chat.Order.Count - 1 - Chat.LinesFirst - Chat.LinesVisible > 0)
            Chat.LinesFirst++;
    }

    private static void MenuCharacter()
    {
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
        Panels.MenuInventory.Visible = !Panels.MenuInventory.Visible;
        Panels.MenuCharacter.Visible = false;
        Panels.MenuOptions.Visible = false;
    }

    private static void MenuOptions()
    {
        Panels.MenuOptions.Visible = !Panels.MenuOptions.Visible;
        Panels.MenuCharacter.Visible = false;
        Panels.MenuInventory.Visible = false;
    }

    private static void DropConfirm()
    {
        // Validate entered amount
        if (!short.TryParse(TextBoxes.DropAmount.Text, out var amount) || amount <= 0)
        {
            Alert.Show("Enter a valid value!");
            return;
        }

        PlayerSender.DropItem(PanelsEvents.DropSlot, amount);
        Panels.Drop.Visible = false;
    }

    private static void DropCancel()
    {
        Panels.Drop.Visible = false;
    }

    private static void PartyYes()
    {
        PartySender.PartyAccept();
        Panels.PartyInvitation.Visible = false;
    }

    private static void PartyNo()
    {
        PartySender.PartyDecline();
        Panels.PartyInvitation.Visible = false;
    }

    private static void TradeYes()
    {
        TradeSender.TradeAccept();
        Panels.TradeInvitation.Visible = false;
    }

    private static void TradeNo()
    {
        TradeSender.TradeDecline();
        Panels.TradeInvitation.Visible = false;
    }

    private static void TradeClose()
    {
        TradeSender.TradeLeave();
        Panels.Trade.Visible = false;
    }

    private static void TradeOfferAccept()
    {
        Buttons.TradeOfferConfirm.Visible = true;
        Buttons.TradeOfferAccept.Visible = Buttons.TradeOfferDecline.Visible = false;
        Panels.TradeOfferDisable.Visible = false;
        TradeSender.TradeOfferState(TradeStatus.Accepted);

        Player.Me.TradeOffer = new ItemSlot[MaxInventory];
        Player.Me.TradeTheirOffer = new ItemSlot[MaxInventory];
    }

    private static void TradeOfferDecline()
    {
        Buttons.TradeOfferConfirm.Visible = true;
        Buttons.TradeOfferAccept.Visible = Buttons.TradeOfferDecline.Visible = false;
        Panels.TradeOfferDisable.Visible = false;
        TradeSender.TradeOfferState(TradeStatus.Declined);
    }

    private static void TradeOfferConfirm()
    {
        Buttons.TradeOfferConfirm.Visible =
            Buttons.TradeOfferAccept.Visible = Buttons.TradeOfferDecline.Visible = false;
        Panels.TradeOfferDisable.Visible = true;
        TradeSender.TradeOfferState(TradeStatus.Confirmed);
    }

    private static void TradeAmountConfirm()
    {
        // Validate entered amount
        if (!short.TryParse(TextBoxes.TradeAmount.Text, out var amount) || amount <= 0)
        {
            Alert.Show("Enter a valid value!");
            return;
        }

        TradeSender.TradeOffer(PanelsEvents.TradeSlot, PanelsEvents.TradeInventorySlot,
            amount);
        Panels.TradeAmount.Visible = false;
    }

    private static void TradeAmountCancel()
    {
        Panels.TradeAmount.Visible = false;
    }

    private static void ShopClose()
    {
        Panels.Shop.Visible = false;
        ShopSender.ShopClose();
    }

    private static void ShopSellConfirm()
    {
        // Validate entered amount
        if (!short.TryParse(TextBoxes.ShopSellAmount.Text, out var amount) || amount <= 0)
        {
            Alert.Show("Enter a valid value!");
            return;
        }

        ShopSender.ShopSell(PanelsEvents.ShopInventorySlot, amount);
        Panels.ShopSell.Visible = false;
    }

    private static void ShopSellCancel()
    {
        Panels.ShopSell.Visible = false;
    }
}
