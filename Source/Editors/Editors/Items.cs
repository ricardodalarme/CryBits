using System;
using System.Windows.Forms;

public partial class Editor_Items : Form
{
    // Usado para acessar os dados da janela
    public static Editor_Items Objects = new Editor_Items();

    // Index do item selecionado
    public byte Selected;

    public Editor_Items()
    {
        InitializeComponent();
    }

    public static void Request()
    {
        // Lê os dados
        Send.Request_Classes();
        Send.Request_Items(true);
    }

    public static void Open()
    {
        // Lista de classes
        Objects.cmbReq_Class.Items.Clear();
        Objects.cmbReq_Class.Items.Add("None");
        for (byte i = 1; i <= Lists.Class.GetUpperBound(0); i++) Objects.cmbReq_Class.Items.Add(Lists.Class[i].Name);

        // Lista de raridades
        Objects.cmbRarity.Items.Clear();
        for (byte i = 0; i < (byte)Globals.Rarity.Count; i++) Objects.cmbRarity.Items.Add((Globals.Rarity)i);

        // Lista de preensão
        Objects.cmbBind.Items.Clear();
        for (byte i = 0; i < (byte)Globals.BindOn.Count; i++) Objects.cmbBind.Items.Add((Globals.Rarity)i);

        // Lista os itens
        Update_List();

        // Abre a janela
        Selection.Objects.Visible = false;
        Objects.Visible = true;
    }

    private static void Update_List()
    {
        // Limpa a lista
        Objects.List.Items.Clear();

        // Adiciona os itens à lista
        for (byte i = 1; i <= Lists.Item.GetUpperBound(0); i++)
            Objects.List.Items.Add(Globals.Numbering(i, Lists.Item.GetUpperBound(0)) + ":" + Lists.Item[i].Name);

        // Seleciona o primeiro item
        Objects.List.SelectedIndex = 0;
    }

    private void Update_Data()
    {
        Selected = (byte)(List.SelectedIndex + 1);

        // Previne erros
        if (Selected == 0) return;

        // Define os limites
        numTexture.Maximum = Graphics.Tex_Item.GetUpperBound(0);

        // Lista os dados
        txtName.Text = Lists.Item[Selected].Name;
        txtDescription.Text = Lists.Item[Selected].Description;
        numTexture.Value = Lists.Item[Selected].Texture;
        cmbType.SelectedIndex = Lists.Item[Selected].Type;
        numPrice.Value = Lists.Item[Selected].Price;
        chkStackable.Checked = Lists.Item[Selected].Stackable;
        cmbBind.SelectedIndex = Lists.Item[Selected].Bind;
        cmbRarity.SelectedIndex = Lists.Item[Selected].Rarity;
        numReq_Level.Value = Lists.Item[Selected].Req_Level;
        cmbReq_Class.SelectedIndex = Lists.Item[Selected].Req_Class;
        numPotion_Experience.Value = Lists.Item[Selected].Potion_Experience;
        numPotion_HP.Value = Lists.Item[Selected].Potion_Vital[(byte)Globals.Vitals.HP];
        numPotion_MP.Value = Lists.Item[Selected].Potion_Vital[(byte)Globals.Vitals.MP];
        cmbEquipment_Type.SelectedIndex = Lists.Item[Selected].Equip_Type;
        numEquip_Strength.Value = Lists.Item[Selected].Equip_Attribute[(byte)Globals.Attributes.Strength];
        numEquip_Resistance.Value = Lists.Item[Selected].Equip_Attribute[(byte)Globals.Attributes.Resistance];
        numEquip_Intelligence.Value = Lists.Item[Selected].Equip_Attribute[(byte)Globals.Attributes.Intelligence];
        numEquip_Agility.Value = Lists.Item[Selected].Equip_Attribute[(byte)Globals.Attributes.Agility];
        numEquip_Vitality.Value = Lists.Item[Selected].Equip_Attribute[(byte)Globals.Attributes.Vitality];
        numWeapon_Damage.Value = Lists.Item[Selected].Weapon_Damage;
    }

    public static void Change_Quantity()
    {
        int Quantity = (int)Editor_Quantity.Objects.numQuantity.Value;
        int Old = Lists.Item.GetUpperBound(0);

        // Altera a quantidade de itens
        Array.Resize(ref Lists.Item, Quantity + 1);

        // Limpa os novos itens
        if (Quantity > Old)
            for (byte i = (byte)(Old + 1); i <= Quantity; i++)
                Clear.Item(i);

        Update_List();
    }

    private void List_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Atualiza a lista
        Update_Data();
    }

    private void butSave_Click(object sender, EventArgs e)
    {
        // Salva os dados
        Send.Write_Items();

        // Volta à janela de seleção
        Visible = false;
        Selection.Objects.Visible = true;
    }

    private void butClear_Click(object sender, EventArgs e)
    {
        // Limpa os dados
        Clear.Item(Selected);

        // Atualiza os valores
        List.Items[Selected - 1] = Globals.Numbering(Selected, List.Items.Count) + ":";
        Update_Data();
    }

    private void butCancel_Click(object sender, EventArgs e)
    {
        // Volta ao menu
        Visible = false;
        Selection.Objects.Visible = true;
    }

    private void butQuantity_Click(object sender, EventArgs e)
    {
        // Abre a janela de alteração
        Editor_Quantity.Open(Lists.Item.GetUpperBound(0));
    }

    private void txtName_Validated(object sender, EventArgs e)
    {
        // Atualiza a lista
        if (Selected > 0)
        {
            Lists.Item[Selected].Name = txtName.Text;
            List.Items[Selected - 1] = Globals.Numbering(Selected, List.Items.Count) + ":" + txtName.Text;
        }
    }

    private void numTexture_ValueChanged(object sender, EventArgs e)
    {
        Lists.Item[Selected].Texture = (short)numTexture.Value;
    }

    private void butTexture_Click(object sender, EventArgs e)
    {
        // Abre a pré visualização
        Lists.Item[Selected].Texture = Preview.Select(Graphics.Tex_Item, Lists.Item[Selected].Texture);
        numTexture.Value = Lists.Item[Selected].Texture;
    }

    private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
    {
        Lists.Item[Selected].Type = (byte)cmbType.SelectedIndex;

        // Visibilidade dos paineis
        if (cmbType.SelectedIndex == (byte)Globals.Items.Equipment)
            grpEquipment.Visible = true;
        else
            grpEquipment.Visible = false;

        if (cmbType.SelectedIndex == (byte)Globals.Items.Potion)
            grpPotion.Visible = true;
        else
            grpPotion.Visible = false;
    }

    private void numReq_Level_ValueChanged(object sender, EventArgs e)
    {
        Lists.Item[Selected].Req_Level = (short)numReq_Level.Value;
    }

    private void cmbReq_Class_SelectedIndexChanged(object sender, EventArgs e)
    {
        Lists.Item[Selected].Req_Class = (byte)cmbReq_Class.SelectedIndex;
    }

    private void numEquip_Vida_ValueChanged(object sender, EventArgs e)
    {
        Lists.Item[Selected].Potion_Vital[(byte)Globals.Vitals.HP] = (short)numPotion_HP.Value;
    }

    private void numEquip_Mana_ValueChanged(object sender, EventArgs e)
    {
        Lists.Item[Selected].Potion_Vital[(byte)Globals.Vitals.MP] = (short)numPotion_MP.Value;
    }

    private void numEquip_Experience_ValueChanged(object sender, EventArgs e)
    {
        Lists.Item[Selected].Potion_Experience = (short)numPotion_Experience.Value;
    }

    private void numEquip_Strength_ValueChanged(object sender, EventArgs e)
    {
        Lists.Item[Selected].Equip_Attribute[(byte)Globals.Attributes.Strength] = (short)numEquip_Strength.Value;
    }

    private void numEquip_Resistance_ValueChanged(object sender, EventArgs e)
    {
        Lists.Item[Selected].Equip_Attribute[(byte)Globals.Attributes.Resistance] = (short)numEquip_Resistance.Value;
    }

    private void numEquip_Intelligence_ValueChanged(object sender, EventArgs e)
    {
        Lists.Item[Selected].Equip_Attribute[(byte)Globals.Attributes.Intelligence] = (short)numEquip_Intelligence.Value;
    }

    private void numEquip_Agility_ValueChanged(object sender, EventArgs e)
    {
        Lists.Item[Selected].Equip_Attribute[(byte)Globals.Attributes.Agility] = (short)numEquip_Agility.Value;
    }

    private void numEquip_Vitality_ValueChanged(object sender, EventArgs e)
    {
        Lists.Item[Selected].Equip_Attribute[(byte)Globals.Attributes.Vitality] = (short)numEquip_Vitality.Value;
    }
    private void numPrice_ValueChanged(object sender, EventArgs e)
    {
        Lists.Item[Selected].Price = (short)numPrice.Value;
    }

    private void chkStackable_CheckedChanged(object sender, EventArgs e)
    {
        Lists.Item[Selected].Stackable = chkStackable.Checked;
    }

    private void numWeapon_Damage_ValueChanged(object sender, EventArgs e)
    {
        Lists.Item[Selected].Weapon_Damage = (short)numWeapon_Damage.Value;
    }

    private void cmbEquipment_Type_SelectedIndexChanged(object sender, EventArgs e)
    {
        Lists.Item[Selected].Equip_Type = (byte)cmbEquipment_Type.SelectedIndex;

        // Visibilidade dos paineis
        if (cmbEquipment_Type.SelectedIndex == (byte)Globals.Equipments.Weapon)
            grpWeapon.Visible = true;
        else
            grpWeapon.Visible = false;
    }

    private void txtDescription_Validated(object sender, EventArgs e)
    {
        Lists.Item[Selected].Description = txtDescription.Text;
    }

    private void cmbRarity_SelectedIndexChanged(object sender, EventArgs e)
    {
        Lists.Item[Selected].Rarity = (byte)cmbRarity.SelectedIndex;
    }

    private void cmbBind_SelectedIndexChanged(object sender, EventArgs e)
    {
        Lists.Item[Selected].Bind = (byte)cmbBind.SelectedIndex;
    }
}