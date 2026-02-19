using CryBits.Client.Framework.Interfacily.Components;

namespace CryBits.Client.Framework.Constants;

public static class TextBoxes
{
    public static readonly Dictionary<string, TextBox> List = [];
    public static TextBox Chat => List["Chat"];
    public static TextBox ConnectUsername => List["Connect_Username"];
    public static TextBox ConnectPassword => List["Connect_Password"];
    public static TextBox RegisterUsername => List["Register_Username"];
    public static TextBox RegisterPassword => List["Register_Password"];
    public static TextBox RegisterPassword2 => List["Register_Password2"];
    public static TextBox CreateCharacterName => List["CreateCharacter_Name"];
    public static TextBox ShopSellAmount => List["Shop_Sell_Amount"];
    public static TextBox TradeAmount => List["Trade_Amount"];
    public static TextBox DropAmount => List["Drop_Amount"];
}
