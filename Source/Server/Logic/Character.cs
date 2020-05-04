﻿using System;
using System.Drawing;
using System.Collections.Generic;

class Character
{
    // Dados permantes
    public string Name;
    public byte Class_Num;
    public short Texture_Num;
    public bool Genre;
    public short Level;
    public int Experience;
    public byte Points;
    public short[] Vital = new short[(byte)Game.Vitals.Count];
    public short[] Attribute = new short[(byte)Game.Attributes.Count];
    public short Map_Num;
    public byte X;
    public byte Y;
    public Game.Directions Direction;
    public Lists.Structures.Inventories[] Inventory = new Lists.Structures.Inventories[Game.Max_Inventory + 1];
    public short[] Equipment = new short[(byte)Game.Equipments.Count];
    public Lists.Structures.Hotbar[] Hotbar = new Lists.Structures.Hotbar[Game.Max_Hotbar + 1];

    // Dados temporários
    public byte Index;
    public bool GettingMap;
    public int Attack_Timer;
    public List<byte> Party = new List<byte>();
    public string Party_Invitation;
    public byte Trade;
    public string Trade_Request;
    public Lists.Structures.Inventories[] Trade_Offer;
    public short Shop;

    // Constutor
    public Character(byte Index)
    {
        this.Index = Index;
    }

    // Cálcula o dano do jogador
    public short Damage
    {
        get
        {
            short Value = Attribute[(byte)Game.Attributes.Strength];
            if (Lists.Item[Equipment[(byte)Game.Equipments.Weapon]] != null) Value += Lists.Item[Equipment[(byte)Game.Equipments.Weapon]].Weapon_Damage;
            return Value;
        }
    }

    // Cálcula a defesa do jogador
    public short Player_Defense => Attribute[(byte)Game.Attributes.Resistance];

    public short MaxVital(byte Vital)
    {
        short[] Base = Lists.Class[Class_Num].Vital;

        // Cálcula o máximo de vital que um jogador possui
        switch ((Game.Vitals)Vital)
        {
            case Game.Vitals.HP: return (short)(Base[Vital] + (Attribute[(byte)Game.Attributes.Vitality] * 1.50 * (Level * 0.75)) + 1);
            case Game.Vitals.MP: return (short)(Base[Vital] + (Attribute[(byte)Game.Attributes.Intelligence] * 1.25 * (Level * 0.5)) + 1);
        }

        return 1;
    }

    public short Regeneration(byte Vital)
    {
        // Cálcula o máximo de vital que um jogador possui
        switch ((Game.Vitals)Vital)
        {
            case Game.Vitals.HP: return (short)(MaxVital(Vital) * 0.05 + Attribute[(byte)Game.Attributes.Vitality] * 0.3);
            case Game.Vitals.MP: return (short)(MaxVital(Vital) * 0.05 + Attribute[(byte)Game.Attributes.Intelligence] * 0.1);
        }

        return 0;
    }

    public int ExpNeeded
    {
        get
        {
            short Total = 0;
            // Quantidade de experiência para passar para o próximo level
            for (byte i = 0; i < (byte)Game.Attributes.Count; i++) Total += Attribute[i];
            return (int)((Level + 1) * 2.5 + (Total + Points) / 2);
        }
    }

    /////////////
    // Funções //
    /////////////
    public void Logic()
    {
        // Reneração 
        if (Environment.TickCount > Loop.Timer_Player_Regen + 5000)
            for (byte v = 0; v < (byte)Game.Vitals.Count; v++)
                if (Vital[v] < MaxVital(v))
                {
                    // Renera a vida do jogador
                    Vital[v] += Regeneration(v);
                    if (Vital[v] > MaxVital(v)) Vital[v] = MaxVital(v);

                    // Envia os dados aos jogadores
                    Send.Player_Vitals(Index);
                }
    }

    public void Join()
    {
        // Previne que alguém que já está online de logar
        if (Player.IsPlaying(Index)) return;

        // Define que o jogador está dentro do jogo
        Lists.Player[Index].Playing = true;

        // Envia todos os dados necessários
        Send.Join(Index);
        Send.Map_Players(Index);
        Send.Player_Experience(Index);
        Send.Player_Inventory(Index);
        Send.Player_Hotbar(Index);
        Send.Items(Index);
        Send.NPCs(Index);
        Send.Shops(Index);

        // Transporta o jogador para a sua determinada Posição
        Warp(Map_Num, X, Y, true);

        // Entra no jogo
        Send.JoinGame(Index);
        Send.Message(Index, Lists.Server_Data.Welcome, Color.Blue);
    }

    public void Leave()
    {
        if (!Lists.Player[Index].InEditor)
        {
            // Salva os dados do jogador e atualiza os demais jogadores da desconexão
            Write.Player(Index);
            Send.Player_Leave(Index);

            // Sai do grupo
            if (Lists.Player[Index].Playing)
            {
                Party_Leave();
                Trade_Leave();
            }
        }

        // Limpa os dados do jogador
        Clear.Player(Index);
    }

    public void Warp(short Map_Num, byte x, byte y, bool NeedUpdate = false)
    {
        short Map_Old = this.Map_Num;

        // Cancela a troca ou a loja
        if (Trade != 0) Trade_Leave();
        if (Shop != 0) Shop_Leave();

        // Evita que o jogador seja transportado para fora do limite
        if (Map_Num < 0 || Map_Num >= Lists.Map.Length) return;
        if (x > Lists.Map[Map_Num].Width) x = Lists.Map[Map_Num].Width;
        if (y > Lists.Map[Map_Num].Height) y = Lists.Map[Map_Num].Height;
        if (x < 0) x = 0;
        if (y < 0) y = 0;

        // Define a Posição do jogador
        this.Map_Num = Map_Num;
        X = x;
        Y = y;

        // Altera o mapa
        if (Map_Old != Map_Num || NeedUpdate)
        {
            // Sai do mapa antigo
            Send.Player_LeaveMap(Index, Map_Old);

            // Inviabiliza o jogador de algumas ações até que ele receba os dados necessários
            GettingMap = true;

            // Envia dados necessários do mapa
            Send.Map_Revision(Index, Map_Num);
            Send.Map_Items(Index, Map_Num);
            Send.Map_NPCs(Index, Map_Num);
        }
        // Apenas atualiza a posição do jogador
        else
            Send.Player_Position(Index);
    }

    public void Move(byte Movement)
    {
        short Next_X = X, Next_Y = Y;
        byte Old_X = X, Old_Y = Y;
        short Link = Lists.Map[Map_Num].Link[(byte)Direction];
        bool SecondMovement = false;

        // Previne erros
        if (Movement < 1 || Movement > 2) return;
        if (GettingMap) return;

        // Cancela a troca ou a loja
        if (Trade != 0) Trade_Leave();
        if (Shop != 0) Shop_Leave();

        // Próximo azulejo
        Map.NextTile(Direction, ref Next_X, ref Next_Y);

        // Ponto de ligação
        if (Map.OutLimit(Map_Num, Next_X, Next_Y))
        {
            if (Link > 0)
                switch (Direction)
                {
                    case Game.Directions.Up: Warp(Link, Old_X, Lists.Map[Link].Height); return;
                    case Game.Directions.Down: Warp(Link, Old_X, 0); return;
                    case Game.Directions.Right: Warp(Link, 0, Old_Y); return;
                    case Game.Directions.Left: Warp(Link, Lists.Map[Link].Width, Old_Y); return;
                }
            else
            {
                Send.Player_Position(Index);
                return;
            }
        }
        // Bloqueio
        else if (!Map.Tile_Blocked(Map_Num, Old_X, Old_Y, Direction))
        {
            X = (byte)Next_X;
            Y = (byte)Next_Y;
        }

        // Atributos
        Lists.Structures.Map_Tile Tile = Lists.Map[Map_Num].Tile[Next_X, Next_Y];

        switch ((Map.Attributes)Tile.Attribute)
        {
            // Teletransporte
            case Map.Attributes.Warp:
                if (Tile.Data_4 > 0) Direction = (Game.Directions)Tile.Data_4 - 1;
                Warp(Tile.Data_1, (byte)Tile.Data_2, (byte)Tile.Data_3);
                SecondMovement = true;
                break;
        }

        // Envia os dados
        if (!SecondMovement && (Old_X != X || Old_Y != Y))
            Send.Player_Move(Index, Movement);
        else
            Send.Player_Position(Index);
    }

    public void Died()
    {
        // Recupera os vitais
        for (byte n = 0; n < (byte)Game.Vitals.Count; n++) Vital[n] = MaxVital(n);
        Send.Player_Vitals(Index);

        // Perde 10% da experiência
        Experience /= 10;
        Send.Player_Experience(Index);

        // Retorna para o ínicio
        Lists.Structures.Class Data = Lists.Class[Class_Num];
        Direction = (Game.Directions)Data.Spawn_Direction;
        Warp(Data.Spawn_Map, Data.Spawn_X, Data.Spawn_Y);
    }

    public void Attack()
    {
        short Next_X = X, Next_Y = Y;
        byte Victim_Index;

        // Próximo azulejo
        Map.NextTile(Direction, ref Next_X, ref Next_Y);

        // Apenas se necessário
        if (Trade != 0) return;
        if (Shop != 0) return;
        if (Environment.TickCount < Attack_Timer + 750) return;
        if (Map.Tile_Blocked(Map_Num, X, Y, Direction, false)) goto @continue;

        // Ataca um jogador
        Victim_Index = Map.HasPlayer(Map_Num, Next_X, Next_Y);
        if (Victim_Index > 0)
        {
            Attack_Player(Player.Character(Victim_Index));
            return;
        }

        // Ataca um NPC
        Victim_Index = Map.HasNPC(Map_Num, Next_X, Next_Y);
        if (Victim_Index > 0)
        {
            Attack_NPC(Lists.Temp_Map[Map_Num].NPC[Victim_Index]);
            return;
        }

    @continue:
        // Demonstra que aos outros jogadores o ataque
        Send.Player_Attack(Index, 0, 0);
        Attack_Timer = Environment.TickCount;
    }

    private void Attack_Player(Character Victim)
    {
        short Attack_Damage;
        short Next_X = X, Next_Y = Y;

        // Define o azujelo a frente do jogador
        Map.NextTile(Direction, ref Next_X, ref Next_Y);

        // Verifica se a vítima pode ser atacada
        if (!Player.IsPlaying(Victim.Index)) return;
        if (Player.Character(Victim.Index).GettingMap) return;
        if (Map_Num != Victim.Map_Num) return;
        if (Victim.X != Next_X || Victim.Y != Next_Y) return;
        if (Lists.Map[Map_Num].Moral == (byte)Map.Morals.Pacific)
        {
            Send.Message(Index, "This is a peaceful area.", Color.White);
            return;
        }

        // Tempo de ataque 
        Attack_Timer = Environment.TickCount;

        // Cálculo de dano
        Attack_Damage = (short)(Damage - Victim.Player_Defense);

        // Dano não fatal
        if (Attack_Damage > 0)
        {
            // Demonstra o ataque aos outros jogadores
            Send.Player_Attack(Index, Victim.Index, (byte)Game.Target.Player);

            if (Attack_Damage < Victim.Vital[(byte)Game.Vitals.HP])
            {
                Victim.Vital[(byte)Game.Vitals.HP] -= Attack_Damage;
                Send.Player_Vitals(Victim.Index);
            }
            // FATALITY
            else
            {
                // Dá 10% da experiência da vítima ao atacante
                GiveExperience(Victim.Experience / 10);

                // Mata a vítima
                Victim.Died();
            }
        }
        else
            // Demonstra o ataque aos outros jogadores
            Send.Player_Attack(Index);
    }

    private void Attack_NPC(NPC.Structure Victim)
    {
        short Attack_Damage;
        short Next_X = X, Next_Y = Y;

        // Define o azujelo a frente do jogador
        Map.NextTile(Direction, ref Next_X, ref Next_Y);

        // Verifica se a vítima pode ser atacada
        if (Victim.X != Next_X || Victim.Y != Next_Y) return;

        // Mensagem
        if (Victim.Target_Index != Index && !string.IsNullOrEmpty(Lists.NPC[Victim.Data_Index].SayMsg)) Send.Message(Index, Lists.NPC[Victim.Data_Index].Name + ": " + Lists.NPC[Victim.Data_Index].SayMsg, Color.White);

        // Não executa o combate com um NPC amigavel
        switch ((NPC.Behaviour)Lists.NPC[Victim.Data_Index].Behaviour)
        {
            case NPC.Behaviour.Friendly: return;
            case NPC.Behaviour.ShopKeeper: Shop_Open(Lists.NPC[Victim.Data_Index].Shop); return;
        }

        // Define o alvo do NPC
        Victim.Target_Index = Index;
        Victim.Target_Type = (byte)Game.Target.Player;

        // Tempo de ataque 
        Attack_Timer = Environment.TickCount;

        // Cálculo de dano
        Attack_Damage = (short)(Damage - Lists.NPC[Victim.Data_Index].Attribute[(byte)Game.Attributes.Resistance]);

        // Dano não fatal
        if (Attack_Damage > 0)
        {
            // Demonstra o ataque aos outros jogadores
            Send.Player_Attack(Index, Victim.Index, (byte)Game.Target.NPC);

            if (Attack_Damage < Victim.Vital[(byte)Game.Vitals.HP])
            {
                Victim.Vital[(byte)Game.Vitals.HP] -= Attack_Damage;
                Send.Map_NPC_Vitals(Map_Num, Victim.Index);
            }
            // FATALITY
            else
            {
                // Experiência ganhada
                GiveExperience(Lists.NPC[Victim.Data_Index].Experience);

                // Reseta os dados do NPC 
                Victim.Died();
            }
        }
        else
            // Demonstra o ataque aos outros jogadores
            Send.Player_Attack(Index);
    }

    public void GiveExperience(int Value)
    {
        // Dá a experiência ao jogador, caso ele estiver em um grupo divide a experiência entre os membros
        if (Party.Count > 0 && Value > 0) Party_SplitXP(Value);
        else Experience += Value;

        // Verifica se a experiência não ficou negtiva
        if (Experience < 0) Experience = 0;

        // Verifica se passou de level
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        byte NumLevel = 0; int ExpRest;

        // Previne erros
        if (!Player.IsPlaying(Index)) return;

        while (Experience >= ExpNeeded)
        {
            NumLevel++;
            ExpRest = Experience - ExpNeeded;

            // Define os dados
            Level++;
            Points += Lists.Server_Data.Num_Points;
            Experience = ExpRest;
        }

        // Envia os dados
        Send.Player_Experience(Index);
        if (NumLevel > 0) Send.Map_Players(Index);
    }

    public bool GiveItem(short Item_Num, short Amount)
    {
        byte Slot_Item = FindInventory(Item_Num);
        byte Slot_Empty = FindInventory(0);

        // Somente se necessário
        if (Item_Num == 0) return false;
        if (Slot_Empty == 0) return false;
        if (Amount == 0) Amount = 1;

        // Empilhável
        if (Slot_Item > 0 && Lists.Item[Item_Num].Stackable)
            Inventory[Slot_Item].Amount += Amount;
        // Não empilhável
        else
        {
            Inventory[Slot_Empty].Item_Num = Item_Num;
            Inventory[Slot_Empty].Amount = Lists.Item[Item_Num].Stackable ? Amount : (byte)1;
        }

        // Envia os dados ao jogador
        Send.Player_Inventory(Index);
        return true;
    }

    public void TakeItem(byte Slot, short Amount)
    {
        // Previne erros
        if (Slot <= 0) return;
        if (Amount < 0) Amount = 1;

        // Tira o item do jogaor
        if (Amount == Inventory[Slot].Amount)
        {
            Inventory[Slot] = new Lists.Structures.Inventories();

            // Retira o item da hotbar caso estier
            byte HotbarSlot = FindHotbar((byte)Game.Hotbar.Item, Slot);
            if (HotbarSlot > 0)
            {
                Hotbar[HotbarSlot] = new Lists.Structures.Hotbar();
                Send.Player_Hotbar(Index);
            }
        }
        // Apenas desconta a quantidade
        else
            Inventory[Slot].Amount -= Amount;

        // Atualiza o inventário
        Send.Player_Inventory(Index);
    }

    public void DropItem(byte Slot, short Amount)
    {
        Lists.Structures.Map_Items Map_Item = new Lists.Structures.Map_Items();

        // Somente se necessário
        if (Lists.Temp_Map[Map_Num].Item.Count == Lists.Server_Data.Max_Map_Items) return;
        if (Inventory[Slot].Item_Num == 0) return;
        if (Lists.Item[Inventory[Slot].Item_Num].Bind == (byte)Game.BindOn.Pickup) return;
        if (Trade != 0) return;

        // Verifica se não está dropando mais do que tem
        if (Amount > Inventory[Slot].Amount) Amount = Inventory[Slot].Amount;

        // Solta o item no chão
        Map_Item.Index = Inventory[Slot].Item_Num;
        Map_Item.Amount = Amount;
        Map_Item.X = X;
        Map_Item.Y = Y;
        Lists.Temp_Map[Map_Num].Item.Add(Map_Item);
        Send.Map_Items(Map_Num);

        // Retira o item do inventário do jogador 
        TakeItem(Slot, Amount);
    }

    public void UseItem(byte Slot)
    {
        short Item_Num = Inventory[Slot].Item_Num;

        // Somente se necessário
        if (Item_Num == 0) return;
        if (Trade != 0) return;

        // Requerimentos
        if (Level < Lists.Item[Item_Num].Req_Level)
        {
            Send.Message(Index, "You do not have the level required to use this item.", Color.White);
            return;
        }
        if (Lists.Item[Item_Num].Req_Class > 0)
            if (Class_Num != Lists.Item[Item_Num].Req_Class)
            {
                Send.Message(Index, "You can not use this item.", Color.White);
                return;
            }

        if (Lists.Item[Item_Num].Type == (byte)Game.Items.Equipment)
        {
            // Retira o item do inventário
            TakeItem(Slot, 1);

            // Caso já estiver com algum equipamento, desequipa ele
            short Current_Equip = Equipment[Lists.Item[Item_Num].Equip_Type];
            if (Current_Equip > 0) GiveItem(Current_Equip, 1);

            // Equipa o item
            Equipment[Lists.Item[Item_Num].Equip_Type] = Item_Num;
            for (byte i = 0; i < (byte)Game.Attributes.Count; i++) Attribute[i] += Lists.Item[Item_Num].Equip_Attribute[i];

            // Envia os dados
            Send.Player_Inventory(Index);
            Send.Player_Equipments(Index);
            Send.Player_Hotbar(Index);
        }
        else if (Lists.Item[Item_Num].Type == (byte)Game.Items.Potion)
        {
            // Efeitos
            bool HadEffect = false;
            GiveExperience(Lists.Item[Item_Num].Potion_Experience);
            for (byte i = 0; i < (byte)Game.Vitals.Count; i++)
            {
                // Verifica se o item causou algum efeito 
                if (Vital[i] < MaxVital(i) && Lists.Item[Item_Num].Potion_Vital[i] != 0) HadEffect = true;

                // Efeito
                Vital[i] += Lists.Item[Item_Num].Potion_Vital[i];

                // Impede que passe dos limites
                if (Vital[i] < 0) Vital[i] = 0;
                if (Vital[i] > MaxVital(i)) Vital[i] = MaxVital(i);
            }

            // Foi fatal
            if (Vital[(byte)Game.Vitals.HP] == 0) Died();

            // Remove o item caso tenha tido algum efeito
            if (Lists.Item[Item_Num].Potion_Experience > 0 || HadEffect) TakeItem(Slot, 1);
        }
    }

    public byte FindHotbar(byte Type, byte Slot)
    {
        // Encontra algo especifico na hotbar
        for (byte i = 1; i <= Game.Max_Hotbar; i++)
            if (Hotbar[i].Type == Type && Hotbar[i].Slot == Slot)
                return i;

        return 0;
    }

    public byte FindInventory(short Item_Num)
    {
        // Encontra algo especifico na hotbar
        for (byte i = 1; i <= Game.Max_Inventory; i++)
            if (Inventory[i].Item_Num == Item_Num)
                return i;

        return 0;
    }

    public byte Total_Inventory_Free()
    {
        byte Total = 0;

        // Retorna a quantidade de itens oferecidos na troca
        for (byte i = 1; i <= Game.Max_Inventory; i++)
            if (Inventory[i].Item_Num == 0)
                Total++;

        return Total;
    }

    public void Party_Leave()
    {
        if (Party.Count > 0)
        {
            // Retira o jogador do grupo
            for (byte i = 0; i < Party.Count; i++)
                Player.Character(Party[i]).Party.Remove(Index);

            // Envia o dados para todos os membros do grupo
            for (byte i = 0; i < Party.Count; i++) Send.Party(Party[i]);
            Party.Clear();
            Send.Party(Index);
        }
    }

    private void Party_SplitXP(int Value)
    {
        // Somatório do level de todos os jogadores do grupo
        int Given_Experience, Experience_Sum = 0, Difference;
        double[] Diff = new double[Party.Count];
        double Diff_Sum = 0, k;

        // Cálcula a diferença dos leveis entre os jogadores
        for (byte i = 0; i < Party.Count; i++)
        {
            Difference = Math.Abs(Level - Player.Character(Party[i]).Level);

            // Constante para a diminuir potêncialmente a experiência que diferenças altas ganhariam
            if (Difference < 3) k = 1.15;
            else if (Difference < 6) k = 1.55;
            else if (Difference < 10) k = 1.85;
            else k = 2.3;

            // Transforma o valor em fração
            Diff[i] = 1 / Math.Pow(k, Math.Min(15, Difference));
            Diff_Sum += Diff[i];
        }

        // Divide a experiência pro grupo com base na diferença dos leveis 
        for (byte i = 0; i < Party.Count; i++)
        {
            // Caso a somatório for maior que um (100%) balanceia os valores
            if (Diff_Sum > 1) Diff[i] *= 1 / Diff_Sum;

            // Divide a experiência
            Given_Experience = (int)((Value / 2) * Diff[i]);
            Experience_Sum += Given_Experience;
            Player.Character(Party[i]).Experience += Given_Experience;
            Player.Character(Party[i]).CheckLevelUp();
            Send.Player_Experience(Party[i]);
        }

        // Dá ao jogador principal o restante da experiência
        Experience += Value - Experience_Sum;
        CheckLevelUp();
        Send.Player_Experience(Index);
    }

    public void Trade_Leave()
    {
        // Cancela a troca
        if (Trade > 0)
        {
            Player.Character(Trade).Trade = 0;
            Trade = 0;
            Send.Trade(Trade);
            Send.Trade(Index);
        }
    }

    public byte Total_Trade_Items()
    {
        byte Total = 0;

        // Retorna a quantidade de itens oferecidos na troca
        for (byte i = 1; i <= Game.Max_Inventory; i++)
            if (Trade_Offer[i].Item_Num > 0)
                Total++;

        return Total;
    }

    public void Shop_Open(short Shop_Num)
    {
        // Abre a loja
        Shop = Shop_Num;
        Send.Shop_Open(Index, Shop_Num);
    }

    public void Shop_Leave()
    {
        // Fecha a loja
        Shop = 0;
        Send.Shop_Open(Index, 0);
    }
}