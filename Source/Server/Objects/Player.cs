using System;
using System.Collections.Generic;
using System.Drawing;
using Network;
using static Utils;

namespace Objects
{
    class Player : Character
    {
        // Dados permantes
        public string Name = string.Empty;
        public Class Class;
        public short Texture_Num;
        public bool Genre;
        public short Level;
        public int Experience;
        public byte Points;
        public short[] Attribute = new short[(byte)Attributes.Count];
        public Inventory[] Inventory = new Inventory[Max_Inventory + 1];
        public Item[] Equipment = new Item[(byte)Equipments.Count];
        public Hotbar[] Hotbar = new Hotbar[Max_Hotbar + 1];

        // Dados temporários
        public bool GettingMap;
        public int Attack_Timer;
        public List<Player> Party = new List<Player>();
        public string Party_Request;
        public Player Trade;
        public string Trade_Request;
        public Trade_Slot[] Trade_Offer;
        public Shop Shop;
        public Account Account;

        // Constutor
        public Player(Account Account)
        {
            this.Account = Account;
        }

        // Cálcula o dano do jogador
        public short Damage
        {
            get
            {
                short Value = Attribute[(byte)Attributes.Strength];
                if (Equipment[(byte)Equipments.Weapon] != null) Value += Equipment[(byte)Equipments.Weapon].Weapon_Damage;
                return Value;
            }
        }

        // Cálcula a defesa do jogador
        public short Player_Defense => Attribute[(byte)Attributes.Resistance];

        public short MaxVital(byte Vital)
        {
            short[] Base = Class.Vital;

            // Cálcula o máximo de vital que um jogador possui
            switch ((Vitals)Vital)
            {
                case Vitals.HP: return (short)(Base[Vital] + (Attribute[(byte)Attributes.Vitality] * 1.50 * (Level * 0.75)) + 1);
                case Vitals.MP: return (short)(Base[Vital] + (Attribute[(byte)Attributes.Intelligence] * 1.25 * (Level * 0.5)) + 1);
            }

            return 1;
        }

        public short Regeneration(byte Vital)
        {
            // Cálcula o máximo de vital que um jogador possui
            switch ((Vitals)Vital)
            {
                case Vitals.HP: return (short)(MaxVital(Vital) * 0.05 + Attribute[(byte)Attributes.Vitality] * 0.3);
                case Vitals.MP: return (short)(MaxVital(Vital) * 0.05 + Attribute[(byte)Attributes.Intelligence] * 0.1);
            }

            return 1;
        }

        // Quantidade de experiência para passar para o próximo level
        public int ExpNeeded
        {
            get
            {
                short Total = 0;
                for (byte i = 0; i < (byte)Attributes.Count; i++) Total += Attribute[i];
                return (int)((Level + 1) * 2.5 + (Total + Points) / 2);
            }
        }

        /////////////
        // Funções //
        /////////////
        public void Logic()
        {
            // Reneração 
            if (Environment.TickCount > Loop.Timer_Regen + 5000)
                for (byte v = 0; v < (byte)Vitals.Count; v++)
                    if (Vital[v] < MaxVital(v))
                    {
                        // Renera a vida do jogador
                        Vital[v] += Regeneration(v);
                        if (Vital[v] > MaxVital(v)) Vital[v] = MaxVital(v);

                        // Env ia os dados aos jogadores
                        Send.Player_Vitals(this);
                    }
        }

        public void Join()
        {
            // Limpa os dados dos outros personagens
            Account.Characters = null;

            // Envia todos os dados necessários
            Send.Join(this);
            Send.Map(Account, Map.Data);
            Send.Map_Players(this);
            Send.Player_Experience(this);
            Send.Player_Inventory(this);
            Send.Player_Hotbar(this);
            Send.Items(Account);
            Send.NPCs(Account);
            Send.Shops(Account);

            // Transporta o jogador para a sua determinada Posição
            Warp(Map, X, Y, true);

            // Entra no jogo
            Send.JoinGame(this);
            Send.Message(this, Welcome_Message, Color.Blue);
        }

        public void Leave()
        {
            // Salva os dados do jogador e atualiza os demais jogadores da desconexão
            Write.Character(Account);
            Send.Player_Leave(this);

            // Sai dos grupos
            Party_Leave();
            Trade_Leave();
        }

        public void Warp(TMap To, byte x, byte y, bool NeedUpdate = false)
        {
            TMap Map_Old = Map;

            // Cancela a troca ou a loja
            if (Trade != null) Trade_Leave();
            if (Shop != null) Shop_Leave();

            // Evita que o jogador seja transportado para fora do limite
            if (To == null) return;
            if (x >= Objects.Map.Width) x = Objects.Map.Width - 1;
            if (y >= Objects.Map.Height) y = Objects.Map.Height - 1;
            if (x < 0) x = 0;
            if (y < 0) y = 0;

            // Define a Posição do jogador
            Map = To;
            X = x;
            Y = y;

            // Altera o mapa
            if (Map_Old != To || NeedUpdate)
            {
                // Sai do mapa antigo
                Send.Player_LeaveMap(this, Map_Old);

                // Inviabiliza o jogador de algumas ações até que ele receba os dados necessários
                GettingMap = true;

                // Envia dados necessários do mapa
                Send.Map_Revision(this, To.Data);
                Send.Map_Items(this, To);
                Send.Map_NPCs(this, To);
            }
            // Apenas atualiza a posição do jogador
            else
                Send.Player_Position(this);
        }

        public void Move(byte Movement)
        {
            byte Next_X = X, Next_Y = Y;
            byte Old_X = X, Old_Y = Y;
            TMap Link = null;// (TMap)Lists.GetData(Lists.Temp_Map, Map.Data.Link[(byte)Direction].ID);
            bool SecondMovement = false;

            // Previne erros
            if (Movement < 1 || Movement > 2) return;
            if (GettingMap) return;

            // Cancela a troca ou a loja
            if (Trade != null) Trade_Leave();
            if (Shop != null) Shop_Leave();

            // Próximo azulejo
            NextTile(Direction, ref Next_X, ref Next_Y);

            // Ponto de ligação
            if (Map.Data.OutLimit(Next_X, Next_Y))
            {
                if (Link != null)
                    switch (Direction)
                    {
                        case Directions.Up: Warp(Link, Old_X, Objects.Map.Height - 1); return;
                        case Directions.Down: Warp(Link, Old_X, 0); return;
                        case Directions.Right: Warp(Link, 0, Old_Y); return;
                        case Directions.Left: Warp(Link, Objects.Map.Width - 1, Old_Y); return;
                    }
                else
                {
                    Send.Player_Position(this);
                    return;
                }
            }
            // Bloqueio
            else if (!Map.Tile_Blocked(Old_X, Old_Y, Direction))
            {
                X = Next_X;
                Y = Next_Y;
            }

            // Atributos
            Map_Attribute Tile = Map.Data.Attribute[Next_X, Next_Y];

            switch ((Tile_Attributes)Tile.Type)
            {
                // Teletransporte
                case Tile_Attributes.Warp:
                    if (Tile.Data_4 > 0) Direction = (Directions)Tile.Data_4 - 1;
                    Warp((TMap)Lists.GetData(Lists.Temp_Map, new Guid(Tile.Data_1)), (byte)Tile.Data_2, (byte)Tile.Data_3);
                    SecondMovement = true;
                    break;
            }

            // Envia os dados
            if (!SecondMovement && (Old_X != X || Old_Y != Y))
                Send.Player_Move(this, Movement);
            else
                Send.Player_Position(this);
        }

        public void Died()
        {
            // Recupera os vitais
            for (byte n = 0; n < (byte)Vitals.Count; n++) Vital[n] = MaxVital(n);
            Send.Player_Vitals(this);

            // Perde 10% da experiência
            Experience /= 10;
            Send.Player_Experience(this);

            // Retorna para o ínicio
            Direction = (Directions)Class.Spawn_Direction;
            Warp((TMap)Lists.GetData(Lists.Temp_Map, Class.Spawn_Map.ID), Class.Spawn_X, Class.Spawn_Y);
        }

        public void Attack()
        {
            byte Next_X = X, Next_Y = Y;
            object Victim;

            // Próximo azulejo
            NextTile(Direction, ref Next_X, ref Next_Y);

            // Apenas se necessário
            if (Trade != null) return;
            if (Shop != null) return;
            if (Environment.TickCount < Attack_Timer + 750) return;
            if (Map.Tile_Blocked(X, Y, Direction, false)) goto @continue;

            // Ataca um jogador
            Victim = Map.HasPlayer(Next_X, Next_Y);
            if (Victim != null)
            {
                Attack_Player((Player)Victim);
                return;
            }

            // Ataca um NPC
            Victim = Map.HasNPC(Next_X, Next_Y);
            if (Victim != null)
            {
                Attack_NPC((TNPC)Victim);
                return;
            }

        @continue:
            // Demonstra que aos outros jogadores o ataque
            Send.Player_Attack(this, null);
            Attack_Timer = Environment.TickCount;
        }

        private void Attack_Player(Player Victim)
        {
            // Verifica se a vítima pode ser atacada
            if (Victim.GettingMap) return;
            if (Map.Data.Moral == (byte)Map_Morals.Pacific)
            {
                Send.Message(this, "This is a peaceful area.", Color.White);
                return;
            }

            // Tempo de ataque 
            Attack_Timer = Environment.TickCount;

            // Cálculo de dano
            short Attack_Damage = (short)(Damage - Victim.Player_Defense);

            // Dano não fatal
            if (Attack_Damage > 0)
            {
                // Demonstra o ataque aos outros jogadores
                Send.Player_Attack(this, Victim.Name, Targets.Player);

                if (Attack_Damage < Victim.Vital[(byte)Vitals.HP])
                {
                    Victim.Vital[(byte)Vitals.HP] -= Attack_Damage;
                    Send.Player_Vitals(Victim);
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
                Send.Player_Attack(this);
        }

        private void Attack_NPC(TNPC Victim)
        {
            // Mensagem
            if (Victim.Target != this && !string.IsNullOrEmpty(Victim.Data.SayMsg)) Send.Message(this, Victim.Data.Name + ": " + Victim.Data.SayMsg, Color.White);

            // Não executa o combate com um NPC amigavel
            switch ((NPC_Behaviour)Victim.Data.Behaviour)
            {
                case NPC_Behaviour.Friendly: return;
                case NPC_Behaviour.ShopKeeper: Shop_Open(Victim.Data.Shop); return;
            }

            // Define o alvo do NPC
            Victim.Target = this;

            // Tempo de ataque 
            Attack_Timer = Environment.TickCount;

            // Cálculo de dano
            short Attack_Damage = (short)(Damage - Victim.Data.Attribute[(byte)Attributes.Resistance]);

            // Dano não fatal
            if (Attack_Damage > 0)
            {
                // Demonstra o ataque aos outros jogadores
                Send.Player_Attack(this, Victim.Index.ToString(), Targets.NPC);

                if (Attack_Damage < Victim.Vital[(byte)Vitals.HP])
                {
                    Victim.Vital[(byte)Vitals.HP] -= Attack_Damage;
                    Send.Map_NPC_Vitals(Victim);
                }
                // FATALITY
                else
                {
                    // Experiência ganhada
                    GiveExperience(Victim.Data.Experience);

                    // Reseta os dados do NPC 
                    Victim.Died();
                }
            }
            else
                // Demonstra o ataque aos outros jogadores
                Send.Player_Attack(this);
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

            while (Experience >= ExpNeeded)
            {
                NumLevel++;
                ExpRest = Experience - ExpNeeded;

                // Define os dados
                Level++;
                Points += Num_Points;
                Experience = ExpRest;
            }

            // Envia os dados
            Send.Player_Experience(this);
            if (NumLevel > 0) Send.Map_Players(this);
        }

        public bool GiveItem(Item Item, short Amount)
        {
            byte Slot_Item = FindInventory(Item);
            byte Slot_Empty = FindInventory(null);

            // Somente se necessário
            if (Item == null) return false;
            if (Slot_Empty == 0) return false;
            if (Amount == 0) Amount = 1;

            // Empilhável
            if (Slot_Item > 0 && Item.Stackable)
                Inventory[Slot_Item].Amount += Amount;
            // Não empilhável
            else
            {
                Inventory[Slot_Empty].Item = Item;
                Inventory[Slot_Empty].Amount = Item.Stackable ? Amount : (byte)1;
            }

            // Envia os dados ao jogador
            Send.Player_Inventory(this);
            return true;
        }

        public void TakeItem(byte Slot, short Amount)
        {
            // Previne erros
            if (Slot <= 0) return;
            if (Amount <= 0) Amount = 1;

            // Tira o item do jogaor
            if (Amount == Inventory[Slot].Amount)
            {
                Inventory[Slot] = new Inventory();

                // Retira o item da hotbar caso estier
                byte HotbarSlot = FindHotbar((byte)Hotbars.Item, Slot);
                if (HotbarSlot > 0)
                {
                    Hotbar[HotbarSlot] = new Hotbar();
                    Send.Player_Hotbar(this);
                }
            }
            // Apenas desconta a quantidade
            else
                Inventory[Slot].Amount -= Amount;

            // Atualiza o inventário
            Send.Player_Inventory(this);
        }

        public void DropItem(byte Slot, short Amount)
        {
            TMap_Items Map_Item = new TMap_Items();

            // Somente se necessário
            if (Map.Item.Count == Max_Map_Items) return;
            if (Inventory[Slot].Item == null) return;
            if (Inventory[Slot].Item.Bind == (byte)BindOn.Pickup) return;
            if (Trade != null) return;

            // Verifica se não está dropando mais do que tem
            if (Amount > Inventory[Slot].Amount) Amount = Inventory[Slot].Amount;

            // Solta o item no chão
            Map_Item.Item = Inventory[Slot].Item;
            Map_Item.Amount = Amount;
            Map_Item.X = X;
            Map_Item.Y = Y;
            Map.Item.Add(Map_Item);
            Send.Map_Items(Map);

            // Retira o item do inventário do jogador 
            TakeItem(Slot, Amount);
        }

        public void UseItem(byte Slot)
        {
            Item Item = Inventory[Slot].Item;

            // Somente se necessário
            if (Item == null) return;
            if (Trade != null) return;

            // Requerimentos
            if (Level < Item.Req_Level)
            {
                Send.Message(this, "You do not have the level required to use this item.", Color.White);
                return;
            }
            if (Item.Req_Class != null)
                if (Class != Item.Req_Class)
                {
                    Send.Message(this, "You can not use this item.", Color.White);
                    return;
                }

            if (Item.Type == (byte)Items.Equipment)
            {
                // Retira o item do inventário
                TakeItem(Slot, 1);

                // Caso já estiver com algum equipamento, desequipa ele
                Item Current_Equip = Equipment[Item.Equip_Type];
                if (Current_Equip != null) GiveItem(Current_Equip, 1);

                // Equipa o item
                Equipment[Item.Equip_Type] = Item;
                for (byte i = 0; i < (byte)Attributes.Count; i++) Attribute[i] += Item.Equip_Attribute[i];

                // Envia os dados
                Send.Player_Inventory(this);
                Send.Player_Equipments(this);
                Send.Player_Hotbar(this);
            }
            else if (Item.Type == (byte)Items.Potion)
            {
                // Efeitos
                bool HadEffect = false;
                GiveExperience(Item.Potion_Experience);
                for (byte i = 0; i < (byte)Vitals.Count; i++)
                {
                    // Verifica se o item causou algum efeito 
                    if (Vital[i] < MaxVital(i) && Item.Potion_Vital[i] != 0) HadEffect = true;

                    // Efeito
                    Vital[i] += Item.Potion_Vital[i];

                    // Impede que passe dos limites
                    if (Vital[i] < 0) Vital[i] = 0;
                    if (Vital[i] > MaxVital(i)) Vital[i] = MaxVital(i);
                }

                // Foi fatal
                if (Vital[(byte)Vitals.HP] == 0) Died();

                // Remove o item caso tenha tido algum efeito
                if (Item.Potion_Experience > 0 || HadEffect) TakeItem(Slot, 1);
            }
        }

        public byte FindHotbar(byte Type, byte Slot)
        {
            // Encontra algo especifico na hotbar
            for (byte i = 1; i <= Max_Hotbar; i++)
                if (Hotbar[i].Type == Type && Hotbar[i].Slot == Slot)
                    return i;

            return 0;
        }

        public byte FindInventory(Item Item)
        {
            // Encontra algo especifico na hotbar
            for (byte i = 1; i <= Max_Inventory; i++)
                if (Inventory[i].Item == Item)
                    return i;

            return 0;
        }

        public byte Total_Inventory_Free()
        {
            byte Total = 0;

            // Retorna a quantidade de itens oferecidos na troca
            for (byte i = 1; i <= Max_Inventory; i++)
                if (Inventory[i].Item == null)
                    Total++;

            return Total;
        }

        public void Party_Leave()
        {
            if (Party.Count > 0)
            {
                // Retira o jogador do grupo
                for (byte i = 0; i < Party.Count; i++)
                    Party[i].Party.Remove(this);

                // Envia o dados para todos os membros do grupo
                for (byte i = 0; i < Party.Count; i++) Send.Party(Party[i]);
                Party.Clear();
                Send.Party(this);
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
                Difference = Math.Abs(Level - Party[i].Level);

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
                Party[i].GiveExperience(Given_Experience);
                Send.Player_Experience(Party[i]);
            }

            // Dá ao jogador principal o restante da experiência
            Experience += Value - Experience_Sum;
            CheckLevelUp();
            Send.Player_Experience(this);
        }

        public void Trade_Leave()
        {
            // Cancela a troca
            if (Trade != null)
            {
                Trade.Trade = null;
                Send.Trade(Trade, false);
                Trade = null;
                Send.Trade(this, false);
            }
        }

        public byte Total_Trade_Items()
        {
            byte Total = 0;

            // Retorna a quantidade de itens oferecidos na troca
            for (byte i = 1; i <= Max_Inventory; i++)
                if (Trade_Offer[i].Slot_Num != 0)
                    Total++;

            return Total;
        }

        public void Shop_Open(Shop Shop)
        {
            // Abre a loja
            this.Shop = Shop;
            Send.Shop_Open(this, Shop);
        }

        public void Shop_Leave()
        {
            // Fecha a loja
            Shop = null;
            Send.Shop_Open(this, null);
        }

        ///////////////////////
        // Métodos Estáticos //
        ///////////////////////
        public static Player Find(string Name)
        {
            // Encontra o usuário
            foreach (var Account in Lists.Account)
                if (Account.IsPlaying)
                    if (Account.Character.Name.Equals(Name))
                        return Account.Character;

            return null;
        }
    }

    class Inventory
    {
        public Item Item;
        public short Amount;
    }

    class Trade_Slot
    {
        public short Slot_Num;
        public short Amount;
    }

    class Hotbar
    {
        public byte Type;
        public byte Slot;
    }
}