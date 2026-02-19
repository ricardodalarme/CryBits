using CryBits.Client.Framework.Interfacily.Components;

namespace CryBits.Client.Framework.Constants;

public static class Panels
{
    public static readonly Dictionary<string, Panel> List = [];
    public static Panel Connect => List["Connect"];
    public static Panel CreateCharacter => List["CreateCharacter"];
    public static Panel Options => List["Options"];
    public static Panel Register => List["Register"];
    public static Panel SelectCharacter => List["SelectCharacter"];
    public static Panel MenuInventory => List["Menu_Inventory"];
    public static Panel MenuOptions => List["Menu_Options"];
    public static Panel MenuCharacter => List["Menu_Character"];
    public static Panel Chat => List["Chat"];
    public static Panel Drop => List["Drop"];
    public static Panel PartyInvitation => List["Party_Invitation"];
    public static Panel Trade => List["Trade"];
    public static Panel TradeInvitation => List["Trade_Invitation"];
    public static Panel TradeAmount => List["Trade_Amount"];
    public static Panel TradeOfferDisable => List["Trade_Offer_Disable"];
    public static Panel Shop => List["Shop"];
    public static Panel ShopSell => List["Shop_Sell"];
    public static Panel Hotbar => List["Hotbar"];
    public static Panel Information => List["Information"];
}