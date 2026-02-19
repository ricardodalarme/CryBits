using CryBits.Client.Framework.Interfacily.Components;

namespace CryBits.Client.Framework.Constants;

public static class CheckBoxes
{
    public static readonly Dictionary<string, CheckBox> List = [];
    public static CheckBox Sounds => List["Sounds"];
    public static CheckBox Musics => List["Musics"];
    public static CheckBox ConnectSaveUsername => List["Connect_Save_Username"];
    public static CheckBox GenderMale => List["GenderMale"];
    public static CheckBox GenderFemale => List["GenderFemale"];
    public static CheckBox OptionsSounds => List["Options_Sounds"];
    public static CheckBox OptionsMusics => List["Options_Musics"];
    public static CheckBox OptionsChat => List["Options_Chat"];
    public static CheckBox OptionsFps => List["Options_FPS"];
    public static CheckBox OptionsLatency => List["Options_Latency"];
    public static CheckBox OptionsParty => List["Options_Party"];
    public static CheckBox OptionsTrade => List["Options_Trade"];
}