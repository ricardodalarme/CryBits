using Avalonia.Controls;
using Avalonia.Interactivity;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Maps;
using CryBits.Definitions.Npcs;

namespace CryBits.Editors.Forms.Maps.Panes;

internal partial class NpcPane : UserControl
{
    public NpcPane()
    {
        InitializeComponent();
        numNPC_Zone.Maximum = Definitions.Globals.MaxZones;
        butNPC_Add.Click += OnAddNpc;
        butNPC_Remove.Click += OnRemoveNpc;
        butNPC_Clear.Click += OnClearNpcs;
    }

    public Map? SelectedMap { get; set; }
    public DefinitionCatalog? Catalog { get; set; }
    public ComboBox CmbNPC => cmbNPC;
    public NumericUpDown NumNPC_Zone => numNPC_Zone;

    public void PopulateCombo()
    {
        cmbNPC.Items.Clear();
        if (Catalog == null) return;
        foreach (var npc in Catalog.Npcs.Values) cmbNPC.Items.Add(npc);
        if (cmbNPC.Items.Count > 0) cmbNPC.SelectedIndex = 0;
        numNPC_Zone.Value = 0;
    }

    public void RefreshList()
    {
        if (SelectedMap == null) return;
        lstNPC.ItemsSource = null;
        lstNPC.ItemsSource = SelectedMap.Npc;
    }

    public void AddNpcAt(int x, int y) => AddNpc(true, x, y);

    private void AddNpc(bool fixedSpawn = false, int x = 0, int y = 0)
    {
        if (SelectedMap == null || cmbNPC.SelectedItem is not Npc npc) return;
        SelectedMap.Npc.Add(new MapNpc
        {
            NpcId = npc.Id,
            Zone = (byte)(numNPC_Zone.Value ?? 0),
            Spawn = fixedSpawn,
            X = x,
            Y = y
        });
        RefreshList();
    }

    private void OnAddNpc(object? sender, RoutedEventArgs e) => AddNpc();

    private void OnRemoveNpc(object? sender, RoutedEventArgs e)
    {
        if (SelectedMap == null || lstNPC.SelectedIndex < 0) return;
        SelectedMap.Npc.RemoveAt(lstNPC.SelectedIndex);
        RefreshList();
    }

    private void OnClearNpcs(object? sender, RoutedEventArgs e)
    {
        SelectedMap?.Npc.Clear();
        RefreshList();
    }
}
