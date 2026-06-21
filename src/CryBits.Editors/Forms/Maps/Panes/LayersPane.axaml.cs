using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace CryBits.Editors.Maps;

internal partial class LayersPane : UserControl
{
    public LayersPane()
    {
        InitializeComponent();
    }

    public DataGrid LstLayers => lstLayers;
    public Border GrpLayersBorder => grpLayersBorder;
    public Border PnlLayerEdit => pnlLayerEdit;
    public TextBox TxtLayer_Name => txtLayer_Name;
    public ComboBox CmbLayers_Type => cmbLayers_Type;
    public TextBlock LblLayerEditTitle => lblLayerEditTitle;
    public Button ButLayer_Ok => butLayer_Ok;
    public Button ButLayer_Cancel => butLayer_Cancel;
    public Button ButLayers_Add => butLayers_Add;
    public Button ButLayers_Remove => butLayers_Remove;
    public Button ButLayers_Edit => butLayers_Edit;
    public Button ButLayers_Up => butLayers_Up;
    public Button ButLayers_Down => butLayers_Down;

    // Zones
    public Border GrpZones => grpZones;
    public TextBlock LblZone => lblZone;
    public ScrollBar ScrlZone => scrlZone;
    public Button ScrlZone_Clear => scrlZone_Clear;

    // Attributes
    public Border GrpAttributes => grpAttributes;
    public RadioButton OptA_Block => optA_Block;
    public RadioButton OptA_Warp => optA_Warp;
    public RadioButton OptA_Item => optA_Item;
    public RadioButton OptA_DirBlock => optA_DirBlock;
    public Border GrpA_Warp => grpA_Warp;
    public ComboBox CmbA_Warp_Map => cmbA_Warp_Map;
    public ComboBox CmbA_Warp_Direction => cmbA_Warp_Direction;
    public NumericUpDown NumA_Warp_X => numA_Warp_X;
    public NumericUpDown NumA_Warp_Y => numA_Warp_Y;
    public Border GrpA_Item => grpA_Item;
    public ComboBox CmbA_Item => cmbA_Item;
    public NumericUpDown NumA_Item_Amount => numA_Item_Amount;
    public Button ButAttributes_Clear => butAttributes_Clear;
    public Button ButAttributes_Import => butAttributes_Import;

    // NPCs
    public Border GrpNPCs => grpNPCs;
    public ComboBox CmbNPC => cmbNPC;
    public NumericUpDown NumNPC_Zone => numNPC_Zone;
    public Button ButNPC_Add => butNPC_Add;
    public ListBox LstNPC => lstNPC;
    public Button ButNPC_Remove => butNPC_Remove;
    public Button ButNPC_Clear => butNPC_Clear;
}
