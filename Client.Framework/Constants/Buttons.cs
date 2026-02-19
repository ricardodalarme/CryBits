using CryBits.Client.Framework.Interfacily.Components;

namespace CryBits.Client.Framework.Constants;

public static class Buttons
{
    public static readonly Dictionary<string, Button> List = [];
    public static Button Connect => List["Connect"];
    public static Button Register => List["Register"];
    public static Button Options => List["Options"];
    public static Button OptionsBack => List["Options_Back"];
    public static Button ConnectConfirm => List["Connect_Confirm"];
    public static Button RegisterConfirm => List["Register_Confirm"];
    public static Button CreateCharacter => List["CreateCharacter"];
    public static Button CreateCharacterChangeRight => List["CreateCharacter_ChangeRight"];
    public static Button CreateCharacterChangeLeft => List["CreateCharacter_ChangeLeft"];
    public static Button CreateCharacterTextureChangeLeft => List["CreateCharacter_Texture_ChangeLeft"];
    public static Button CreateCharacterTextureChangeRight => List["CreateCharacter_Texture_ChangeRight"];
    public static Button CreateCharacterBack => List["CreateCharacter_Back"];
    public static Button CharacterUse => List["Character_Use"];
    public static Button CharacterCreate => List["Character_Create"];
    public static Button CharacterDelete => List["Character_Delete"];
    public static Button CharacterChangeRight => List["Character_ChangeRight"];
    public static Button CharacterChangeLeft => List["Character_ChangeLeft"];
    public static Button ChatUp => List["Chat_Up"];
    public static Button ChatDown => List["Chat_Down"];
    public static Button MenuCharacter => List["Menu_Character"];
    public static Button AttributesStrength => List["Attributes_Strength"];
    public static Button AttributesResistance => List["Attributes_Resistance"];
    public static Button AttributesIntelligence => List["Attributes_Intelligence"];
    public static Button AttributesAgility => List["Attributes_Agility"];
    public static Button AttributesVitality => List["Attributes_Vitality"];
    public static Button MenuInventory => List["Menu_Inventory"];
    public static Button MenuOptions => List["Menu_Options"];
    public static Button DropConfirm => List["Drop_Confirm"];
    public static Button DropCancel => List["Drop_Cancel"];
    public static Button PartyYes => List["Party_Yes"];
    public static Button PartyNo => List["Party_No"];
    public static Button TradeYes => List["Trade_Yes"];
    public static Button TradeNo => List["Trade_No"];
    public static Button TradeClose => List["Trade_Close"];
    public static Button TradeOfferAccept => List["Trade_Offer_Accept"];
    public static Button TradeOfferDecline => List["Trade_Offer_Decline"];
    public static Button TradeOfferConfirm => List["Trade_Offer_Confirm"];
    public static Button TradeAmountConfirm => List["Trade_Amount_Confirm"];
    public static Button TradeAmountCancel => List["Trade_Amount_Cancel"];
    public static Button ShopClose => List["Shop_Close"];
    public static Button ShopSellConfirm => List["Shop_Sell_Confirm"];
    public static Button ShopSellCancel => List["Shop_Sell_Cancel"];
}