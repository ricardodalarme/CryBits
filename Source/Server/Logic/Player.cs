using System;
using System.Drawing;

class Player
{
    public static Character_Structure Character(byte Index)
    {
        // Retorna com os valores do personagem atual
        return Lists.Player[Index].Character[Lists.TempPlayer[Index].Using];
    }

    public class Character_Structure
    {
        // Dados básicos
        public byte Index;
        public string Name = string.Empty;
        public byte Class;
        public bool Genre;
        public short Level;
        private short experience;
        public byte Points;
        public short[] Vital = new short[(byte)Game.Vitals.Amount];
        public short[] Attribute = new short[(byte)Game.Attributes.Amount];
        public short Map;
        public byte X;
        public byte Y;
        public Game.Directions Direction;
        public int Attack_Timer;
        public Lists.Structures.Inventories[] Inventory;
        public short[] Equipment;
        public Lists.Structures.Hotbar[] Hotbar;

        public short Experience
        {
            get
            {
                return experience;
            }
            set
            {
                experience = value;
                CheckLevelUp(Index);
            }
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

        // Cálcula o dano do jogador
        public short Player_Defense
        {
            get
            {
                return Attribute[(byte)Game.Attributes.Resistance];
            }
        }

        public short MaxVital(byte Vital)
        {
            short[] Base = Lists.Class[Class].Vital;

            // Cálcula o máximo de vital que um jogador possui
            switch ((Game.Vitals)Vital)
            {
                case Game.Vitals.HP:
                    return (short)(Base[Vital] + (Attribute[(byte)Game.Attributes.Vitality] * 1.50 * (Level * 0.75)) + 1);
                case Game.Vitals.MP:
                    return (short)(Base[Vital] + (Attribute[(byte)Game.Attributes.Intelligence] * 1.25 * (Level * 0.5)) + 1);
            }

            return 1;
        }

        public short Regeneration(byte Vital)
        {
            // Cálcula o máximo de vital que um jogador possui
            switch ((Game.Vitals)Vital)
            {
                case Game.Vitals.HP:
                    return (short)(MaxVital(Vital) * 0.05 + Attribute[(byte)Game.Attributes.Vitality] * 0.3);
                case Game.Vitals.MP:
                    return (short)(MaxVital(Vital) * 0.05 + Attribute[(byte)Game.Attributes.Intelligence] * 0.1);
            }

            return 0;
        }

        public short ExpNeeded
        {
            get
            {
                short Total = 0;
                // Amount de experiência para passar para o próximo level
                for (byte i = 0; i <= (byte)(Game.Attributes.Amount - 1); i++) Total += Attribute[i];
                return (short)((Level + 1) * 2.5 + (Total + Points) / 2);
            }
        }
    }

    public static void Join(byte Index)
    {
        // Previne que alguém que já está online de logar
        if (IsPlaying(Index))
            return;

        // Define que o jogador está dentro do jogo
        Lists.TempPlayer[Index].Playing = true;

        // Envia todos os dados necessários
        Send.Join(Index);
        Send.Map_Players(Index);
        Send.Player_Experience(Index);
        Send.Player_Inventory(Index);
        Send.Player_Hotbar(Index);
        Send.Items(Index);
        Send.NPCs(Index);
        Send.Map_Items(Index, Character(Index).Map);

        // Transporta o jogador para a sua determinada Posição
        Warp(Index, Character(Index).Map, Character(Index).X, Character(Index).Y);

        // Entra no jogo
        Send.JoinGame(Index);
        Send.Message(Index, Lists.Server_Data.Welcome, Color.Blue);
    }

    public static void Leave(byte Index)
    {
        // Salva os dados do jogador
        Write.Player(Index);
        Clear.Player(Index);

        // Envia a  todos a desconexão do jogador
        Send.Player_Leave(Index);
    }

    public static bool IsPlaying(byte Index)
    {
        // Verifica se o jogador está dentro do jogo
        if (Socket.IsConnected(Index))
            if (Lists.TempPlayer[Index].Playing)
                return true;

        return false;
    }

    public static byte Find(string Name)
    {
        // Encontra o usuário
        for (byte i = 1; i < Lists.Player.Length; i++)
            if (Character(i).Name == Name)
                return i;

        return 0;
    }

    public static byte FindCharacter(byte Index, string Name)
    {
        // Encontra o personagem
        for (byte i = 1; i <= Lists.Server_Data.Max_Characters; i++)
            if (Lists.Player[Index].Character[i].Name == Name)
                return i;

        return 0;
    }

    public static bool HasCharacter(byte Index)
    {
        // Verifica se o jogador tem algum personagem
        for (byte i = 1; i <= Lists.Server_Data.Max_Characters; i++)
            if (!string.IsNullOrEmpty(Lists.Player[Index].Character[i].Name))
                return true;

        return false;
    }

    public static bool MultipleAccounts(string User)
    {
        // Verifica se já há alguém conectado com essa conta
        for (byte i = 1; i <= Game.HigherIndex; i++)
            if (Socket.IsConnected(i))
                if (Lists.Player[i].User == User)
                    return true;

        return false;
    }

    public static void Move(byte Index, byte Movimento)
    {
        byte x = Character(Index).X, y = Character(Index).Y;
        short Map_Num = Character(Index).Map;
        short Next_X = x, Next_Y = y;
        short Link = Lists.Map[Map_Num].Link[(byte)Character(Index).Direction];
        bool SecondMovement = false;

        // Previne erros
        if (Movimento < 1 || Movimento > 2) return;
        if (Lists.TempPlayer[Index].GettingMap) return;

        // Próximo azulejo
        Map.NextTile(Character(Index).Direction, ref Next_X, ref Next_Y);

        // Ponto de ligação
        if (Map.OutLimit(Map_Num, Next_X, Next_Y))
        {
            if (Link > 0)
                switch (Character(Index).Direction)
                {
                    case Game.Directions.Up: Warp(Index, Link, x, Lists.Map[Map_Num].Height); break;
                    case Game.Directions.Down: Warp(Index, Link, x, 0); break;
                    case Game.Directions.Right: Warp(Index, Link, 0, y); break;
                    case Game.Directions.Left: Warp(Index, Link, Lists.Map[Map_Num].Width, y); break;
                }
            else
            {
                Send.Player_Position(Index);
                return;
            }
        }
        // Bloqueio
        else if (!Map.Tile_Blocked(Map_Num, x, y, Character(Index).Direction))
        {
            Character(Index).X = (byte)Next_X;
            Character(Index).Y = (byte)Next_Y;
        }

        // Atributos
        Lists.Structures.Map_Tile Azulejo = Lists.Map[Map_Num].Tile[Next_X, Next_Y];

        switch ((Map.Attributes)Azulejo.Attribute)
        {
            // Teletransporte
            case Map.Attributes.Warp:
                if (Azulejo.Data_4 > 0) Character(Index).Direction = (Game.Directions)Azulejo.Data_4 - 1;
                Warp(Index, Azulejo.Data_1, (byte)Azulejo.Data_2, (byte)Azulejo.Data_3);
                SecondMovement = true;
                break;
        }

        // Envia os dados
        if (!SecondMovement && (x != Character(Index).X || y != Character(Index).Y))
            Send.Player_Move(Index, Movimento);
        else
            Send.Player_Position(Index);
    }

    public static void Warp(byte Index, short Map, byte x, byte y)
    {
        short Map_Old = Character(Index).Map;

        // Evita que o jogador seja transportado para fora do limite
        if (Map < 0 || Map > Lists.Map.GetUpperBound(0)) return;
        if (x > Lists.Map[Map].Width) x = Lists.Map[Map].Width;
        if (y > Lists.Map[Map].Height) y = Lists.Map[Map].Height;
        if (x < 0) x = 0;
        if (y < 0) y = 0;

        // Define a Posição do jogador
        Character(Index).Map = Map;
        Character(Index).X = x;
        Character(Index).Y = y;

        // Envia os dados dos NPCs
        Send.Map_NPCs(Index, Map);

        // Envia os dados para os outros jogadores
        if (Map_Old != Map)
            Send.Player_LeaveMap(Index, Map_Old);

        Send.Player_Position(Index);

        // Atualiza os valores
        Lists.TempPlayer[Index].GettingMap = true;

        // Verifica se será necessário enviar os dados do mapa para o jogador
        Send.Map_Revision(Index, Map);
    }

    public static void Attack(byte Index)
    {
        short Next_X = Character(Index).X, Next_Y = Character(Index).Y;
        byte Victim_Index;

        // Próximo azulejo
        Map.NextTile(Character(Index).Direction, ref Next_X, ref Next_Y);

        // Apenas se necessário
        if (Environment.TickCount < Character(Index).Attack_Timer + 750) return;
        if (Map.Tile_Blocked(Character(Index).Map, Character(Index).X, Character(Index).Y, Character(Index).Direction, false)) goto @continue;

        // Ataca um jogador
        Victim_Index = Map.HasPlayer(Character(Index).Map, Next_X, Next_Y);
        if (Victim_Index > 0)
        {
            Attack_Player(Index, Victim_Index);
            return;
        }

        // Ataca um NPC
        Victim_Index = Map.HasNPC(Character(Index).Map, Next_X, Next_Y);
        if (Victim_Index > 0)
        {
            Attack_NPC(Index, Victim_Index);
            return;
        }

    @continue:
        // Demonstra que aos outros jogadores o ataque
        Send.Player_Attack(Index, 0, 0);
        Character(Index).Attack_Timer = Environment.TickCount;
    }

    public static void Attack_Player(byte Index, byte Victim)
    {
        short Damage;
        short x = Character(Index).X, y = Character(Index).Y;

        // Define o azujelo a frente do jogador
        Map.NextTile(Character(Index).Direction, ref x, ref y);

        // Verifica se a vítima pode ser atacada
        if (!IsPlaying(Victim)) return;
        if (Lists.TempPlayer[Victim].GettingMap) return;
        if (Character(Index).Map != Character(Victim).Map) return;
        if (Character(Victim).X != x || Character(Victim).Y != y) return;
        if (Lists.Map[Character(Index).Map].Moral == (byte)Map.Morals.Pacific)
        {
            Send.Message(Index, "This is a peaceful area..", Color.White);
            return;
        }

        // Demonstra o ataque aos outros jogadores
        Send.Player_Attack(Index, Victim, (byte)Game.Target.Player);

        // Tempo de ataque 
        Character(Index).Attack_Timer = Environment.TickCount;

        // Cálculo de dano
        Damage = (short)(Character(Index).Damage - Character(Victim).Player_Defense);

        // Dano não fatal
        if (Damage > 0)
            if (Damage <= Character(Victim).MaxVital((byte)Game.Vitals.HP))
            {
                Character(Victim).Vital[(byte)Game.Vitals.HP] -= Damage;
                Send.Player_Vitals(Victim);
            }
            // FATALITY
            else
            {
                // Dá 10% da experiência da vítima ao atacante
                Character(Index).Experience += (short)(Character(Victim).Experience / 10);

                // Mata a vítima
                Died(Victim);
            }
    }

    public static void Attack_NPC(byte Index, byte Victim)
    {
        short Damage;
        short x = Character(Index).X, y = Character(Index).Y;
        Lists.Structures.Map_NPCs NPC = Lists.Temp_Map[Character(Index).Map].NPC[Victim];

        // Define o azujelo a frente do jogador
        Map.NextTile(Character(Index).Direction, ref x, ref y);

        // Verifica se a vítima pode ser atacada
        if (NPC.X != x || NPC.Y != y) return;
        if (Lists.NPC[NPC.Index].Behaviour == (byte)global::NPC.Behaviour.Friendly) return;

        // Define o alvo do NPC
        Lists.Temp_Map[Character(Index).Map].NPC[Victim].Target_Index = Index;
        Lists.Temp_Map[Character(Index).Map].NPC[Victim].Target_Type = (byte)Game.Target.Player;

        // Demonstra o ataque aos outros jogadores
        Send.Player_Attack(Index, Victim, (byte)Game.Target.NPC);

        // Tempo de ataque 
        Character(Index).Attack_Timer = Environment.TickCount;

        // Cálculo de dano
        Damage = (short)(Character(Index).Damage - Lists.NPC[NPC.Index].Attribute[(byte)Game.Attributes.Resistance]);

        // Dano não fatal
        if (Damage > 0)
            if (Damage < Lists.Temp_Map[Character(Index).Map].NPC[Victim].Vital[(byte)Game.Vitals.HP])
            {
                Lists.Temp_Map[Character(Index).Map].NPC[Victim].Vital[(byte)Game.Vitals.HP] -= Damage;
                Send.Map_NPC_Vitals(Character(Index).Map, Victim);
            }
            // FATALITY
            else
            {
                // Experiência ganhada
                Character(Index).Experience += Lists.NPC[NPC.Index].Experience;

                // Reseta os dados do NPC 
                global::NPC.Died(Character(Index).Map, Victim);
            }
    }

    public static void Died(byte Index)
    {
        Lists.Structures.Class Data = Lists.Class[Character(Index).Class];

        // Recupera os vitais
        for (byte n = 0; n < (byte)Game.Vitals.Amount; n++)
            Character(Index).Vital[n] = Character(Index).MaxVital(n);

        // Perde 10% da experiência
        Character(Index).Experience /= 10;
        Send.Player_Experience(Index);

        // Retorna para o ínicio
        Character(Index).Direction = (Game.Directions)Data.Spawn_Direction;
        Warp(Index, Data.Spawn_Map, Data.Spawn_X, Data.Spawn_Y);
    }

    public static void Logic()
    {
        // Lógica dos jogadores
        for (byte i = 0; i <= Game.HigherIndex; i++)
        {
            // Não é necessário
            if (!IsPlaying(i)) continue;

            ///////////////
            // Reneração // 
            ///////////////
            if (Environment.TickCount > Loop.Timer_Player_Regen + 5000)
                for (byte v = 0; v < (byte)Game.Vitals.Amount; v++)
                    if (Character(i).Vital[v] < Character(i).MaxVital(v))
                    {
                        // Renera a vida do jogador
                        Character(i).Vital[v] += Character(i).Regeneration(v);
                        if (Character(i).Vital[v] > Character(i).MaxVital(v)) Character(i).Vital[v] = Character(i).MaxVital(v);

                        // Envia os dados aos jogadores
                        Send.Player_Vitals(i);
                    }
        }

        // Reseta as contagens
        if (Environment.TickCount > Loop.Timer_Player_Regen + 5000) Loop.Timer_Player_Regen = Environment.TickCount;
    }

    public static void CheckLevelUp(byte Index)
    {
        byte NumLevel = 0; short ExpRest;

        // Previne erros
        if (!IsPlaying(Index)) return;

        while (Character(Index).Experience >= Character(Index).ExpNeeded)
        {
            NumLevel += 1;
            ExpRest = (short)(Character(Index).Experience - Character(Index).ExpNeeded);

            // Define os dados
            Character(Index).Level += 1;
            Character(Index).Points += 3;
            Character(Index).Experience = ExpRest;
        }

        // Envia os dados
        Send.Player_Experience(Index);
        if (NumLevel > 0) Send.Map_Players(Index);
    }

    public static bool GiveItem(byte Index, short Item_Num, short Amount)
    {
        byte Slot_Item = FindInventory(Index, Item_Num);
        byte Slot_Empty = FindInventory(Index, 0);

        // Somente se necessário
        if (Item_Num == 0) return false;
        if (Slot_Empty == 0) return false;
        if (Amount == 0) Amount = 1;

        // Empilhável
        if (Slot_Item > 0 && Lists.Item[Item_Num].Stackable)
            Character(Index).Inventory[Slot_Item].Amount += Amount;
        // Não empilhável
        else
        {
            Character(Index).Inventory[Slot_Empty].Item_Num = Item_Num;
            Character(Index).Inventory[Slot_Empty].Amount = Amount;
        }

        // Envia os dados ao jogador
        Send.Player_Inventory(Index);
        return true;
    }

    public static void DropItem(byte Index, byte Slot)
    {
        short Map_Num = Character(Index).Map;
        Lists.Structures.Map_Items Map_Item = new Lists.Structures.Map_Items();

        // Somente se necessário
        if (Lists.Temp_Map[Map_Num].Item.Count == Game.Max_Map_Items) return;
        if (Character(Index).Inventory[Slot].Item_Num == 0) return;
        if (Lists.Item[Character(Index).Inventory[Slot].Item_Num].Bind) return;

        // Solta o item no chão
        Map_Item.Index = Character(Index).Inventory[Slot].Item_Num;
        Map_Item.Amount = Character(Index).Inventory[Slot].Amount;
        Map_Item.X = Character(Index).X;
        Map_Item.Y = Character(Index).Y;
        Lists.Temp_Map[Map_Num].Item.Add(Map_Item);
        Send.Map_Items(Map_Num);

        // Retira o item do inventário do jogador 
        Character(Index).Inventory[Slot].Item_Num = 0;
        Character(Index).Inventory[Slot].Amount = 0;
        Send.Player_Inventory(Index);
    }

    public static void UseItem(byte Index, byte Slot)
    {
        short Item_Num = Character(Index).Inventory[Slot].Item_Num;

        // Somente se necessário
        if (Item_Num == 0) return;

        // Requerimentos
        if (Character(Index).Level < Lists.Item[Item_Num].Req_Level)
        {
            Send.Message(Index, "You do not have the level required to use this item.", Color.White);
            return;
        }
        if (Lists.Item[Item_Num].Req_Class > 0)
            if (Character(Index).Class != Lists.Item[Item_Num].Req_Class)
            {
                Send.Message(Index, "You can not use this item.", Color.White);
                return;
            }

        if (Lists.Item[Item_Num].Type == (byte)Game.Items.Equipment)
        {
            // Retira o item da hotbar
            byte HotbarSlot = FindHotbar(Index, (byte)Game.Hotbar.Item, Slot);
            Character(Index).Hotbar[HotbarSlot].Type = 0;
            Character(Index).Hotbar[HotbarSlot].Slot = 0;

            // Retira o item do inventário
            Character(Index).Inventory[Slot].Item_Num = 0;
            Character(Index).Inventory[Slot].Amount = 0;

            // Caso já estiver com algum equipamento, desequipa ele
            if (Character(Index).Equipment[Lists.Item[Item_Num].Equip_Type] > 0) GiveItem(Index, Item_Num, 1);

            // Equipa o item
            Character(Index).Equipment[Lists.Item[Item_Num].Equip_Type] = Item_Num;
            for (byte i = 0; i < (byte)Game.Attributes.Amount; i++) Character(Index).Attribute[i] += Lists.Item[Item_Num].Equip_Attribute[i];

            // Envia os dados
            Send.Player_Inventory(Index);
            Send.Player_Equipments(Index);
            Send.Player_Hotbar(Index);
        }
        else if (Lists.Item[Item_Num].Type == (byte)Game.Items.Potion)
        {
            // Efeitos
            bool HadEffect = false;
            Character(Index).Experience += Lists.Item[Item_Num].Potion_Experience;
            if (Character(Index).Experience < 0) Character(Index).Experience = 0;
            for (byte i = 0; i < (byte)Game.Vitals.Amount; i++)
            {
                // Verifica se o item causou algum efeito 
                if (Character(Index).Vital[i] < Character(Index).MaxVital(i) && Lists.Item[Item_Num].Potion_Vital[i] != 0) HadEffect = true;

                // Efeito
                Character(Index).Vital[i] += Lists.Item[Item_Num].Potion_Vital[i];

                // Impede que passe dos limites
                if (Character(Index).Vital[i] < 0) Character(Index).Vital[i] = 0;
                if (Character(Index).Vital[i] > Character(Index).MaxVital(i)) Character(Index).Vital[i] = Character(Index).MaxVital(i);
            }

            // Foi fatal
            if (Character(Index).Vital[(byte)Game.Vitals.HP] == 0) Died(Index);

            // Remove o item caso tenha tido algum efeito
            if (Lists.Item[Item_Num].Potion_Experience > 0 || HadEffect)
            {
                Character(Index).Inventory[Slot].Item_Num = 0;
                Character(Index).Inventory[Slot].Amount = 0;
                Send.Player_Inventory(Index);
                Send.Player_Vitals(Index);
            }
        }
    }

    public static byte FindHotbar(byte Index, byte Type, byte Slot)
    {
        // Encontra algo especifico na hotbar
        for (byte i = 1; i <= Game.Max_Hotbar; i++)
            if (Character(Index).Hotbar[i].Type == Type && Character(Index).Hotbar[i].Slot == Slot)
                return i;

        return 0;
    }

    public static byte FindInventory(byte Index, short Item_Num)
    {
        // Encontra algo especifico na hotbar
        for (byte i = 1; i <= Game.Max_Inventory; i++)
            if (Character(Index).Inventory[i].Item_Num == Item_Num)
                return i;

        return 0;
    }
}