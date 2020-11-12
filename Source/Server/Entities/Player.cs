using System;
using System.Collections.Generic;
using System.Drawing;
using CryBits.Entities;
using CryBits.Server.Logic;
using CryBits.Server.Network;
using static CryBits.Server.Logic.Utils;
using static CryBits.Utils;

namespace CryBits.Server.Entities
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
        public Inventory[] Inventory = new Inventory[MaxInventory + 1];
        public Item[] Equipment = new Item[(byte)Equipments.Count];
        public Hotbar[] Hotbar = new Hotbar[MaxHotbar];

        // Dados temporários
        public bool GettingMap;
        public int Attack_Timer;
        public List<Player> Party = new List<Player>();
        public string Party_Request;
        public Player Trade;
        public string Trade_Request;
        public TradeSlot[] Trade_Offer;
        public Shop Shop;
        public Account Account;

        // Constutor
        public Player(Account account)
        {
            Account = account;
        }

        // Cálcula o dano do jogador
        public short Damage
        {
            get
            {
                short value = Attribute[(byte)Attributes.Strength];
                if (Equipment[(byte)Equipments.Weapon] != null) value += Equipment[(byte)Equipments.Weapon].Weapon_Damage;
                return value;
            }
        }

        // Cálcula a defesa do jogador
        public short Player_Defense => Attribute[(byte)Attributes.Resistance];

        public short MaxVital(byte vital)
        {
            short[] @base = Class.Vital;

            // Cálcula o máximo de vital que um jogador possui
            switch ((Vitals)vital)
            {
                case Vitals.HP: return (short)(@base[vital] + (Attribute[(byte)Attributes.Vitality] * 1.50 * (Level * 0.75)) + 1);
                case Vitals.MP: return (short)(@base[vital] + (Attribute[(byte)Attributes.Intelligence] * 1.25 * (Level * 0.5)) + 1);
            }

            return 1;
        }

        public short Regeneration(byte vital)
        {
            // Cálcula o máximo de vital que um jogador possui
            switch ((Vitals)vital)
            {
                case Vitals.HP: return (short)(MaxVital(vital) * 0.05 + Attribute[(byte)Attributes.Vitality] * 0.3);
                case Vitals.MP: return (short)(MaxVital(vital) * 0.05 + Attribute[(byte)Attributes.Intelligence] * 0.1);
            }

            return 1;
        }

        // Quantidade de experiência para passar para o próximo level
        public int ExpNeeded
        {
            get
            {
                short total = 0;
                for (byte i = 0; i < (byte)Attributes.Count; i++) total += Attribute[i];
                return (int)((Level + 1) * 2.5 + (total + Points) / 2);
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
            Send.Items(Account);
            Send.NPCs(Account);
            Send.Shops(Account);
            Send.Map(Account, Map.Data);
            Send.Map_Players(this);
            Send.Player_Experience(this);
            Send.Player_Inventory(this);
            Send.Player_Hotbar(this);

            // Transporta o jogador para a sua determinada Posição
            Warp(Map, X, Y, true);

            // Entra no jogo
            Send.JoinGame(this);
            Send.Message(this, Welcome_Message, Color.Blue);
        }

        public void Leave()
        {
            // Salva os dados do jogador e atualiza os demais jogadores da desconexão
            Library.Write.Character(Account);
            Send.Player_Leave(this);

            // Sai dos grupos
            Party_Leave();
            Trade_Leave();
        }

        public void Warp(TempMap to, byte x, byte y, bool needUpdate = false)
        {
            TempMap mapOld = Map;

            // Cancela a troca ou a loja
            if (Trade != null) Trade_Leave();
            if (Shop != null) Shop_Leave();

            // Evita que o jogador seja transportado para fora do limite
            if (to == null) return;
            if (x >= CryBits.Entities.Map.Width) x = CryBits.Entities.Map.Width - 1;
            if (y >= CryBits.Entities.Map.Height) y = CryBits.Entities.Map.Height - 1;
            if (x < 0) x = 0;
            if (y < 0) y = 0;

            // Define a Posição do jogador
            Map = to;
            X = x;
            Y = y;

            // Altera o mapa
            if (mapOld != to || needUpdate)
            {
                // Sai do mapa antigo
                Send.Player_LeaveMap(this, mapOld);

                // Inviabiliza o jogador de algumas ações até que ele receba os dados necessários
                GettingMap = true;

                // Envia dados necessários do mapa
                Send.Map_Revision(this, to.Data);
                Send.Map_Items(this, to);
                Send.Map_NPCs(this, to);
            }
            // Apenas atualiza a posição do jogador
            else
                Send.Player_Position(this);
        }

        public void Move(byte movement)
        {
            byte nextX = X, nextY = Y;
            byte oldX = X, oldY = Y;
            TempMap link = null;// TMap.Get( Map.Data.Link[(byte)Direction].ID);
            bool secondMovement = false;

            // Previne erros
            if (movement < 1 || movement > 2) return;
            if (GettingMap) return;

            // Cancela a troca ou a loja
            if (Trade != null) Trade_Leave();
            if (Shop != null) Shop_Leave();

            // Próximo azulejo
            NextTile(Direction, ref nextX, ref nextY);

            // Ponto de ligação
            if (Map.Data.OutLimit(nextX, nextY))
            {
                if (link != null)
                    switch (Direction)
                    {
                        case Directions.Up: Warp(link, oldX, CryBits.Entities.Map.Height - 1); return;
                        case Directions.Down: Warp(link, oldX, 0); return;
                        case Directions.Right: Warp(link, 0, oldY); return;
                        case Directions.Left: Warp(link, CryBits.Entities.Map.Width - 1, oldY); return;
                    }
                else
                {
                    Send.Player_Position(this);
                    return;
                }
            }
            // Bloqueio
            else if (!Map.Tile_Blocked(oldX, oldY, Direction))
            {
                X = nextX;
                Y = nextY;
            }

            // Atributos
            MapAttribute tile = Map.Data.Attribute[nextX, nextY];

            switch ((TileAttributes)tile.Type)
            {
                // Teletransporte
                case TileAttributes.Warp:
                    if (tile.Data_4 > 0) Direction = (Directions)tile.Data_4 - 1;
                    Warp(TempMap.Get(new Guid(tile.Data_1)), (byte)tile.Data_2, (byte)tile.Data_3);
                    secondMovement = true;
                    break;
            }

            // Envia os dados
            if (!secondMovement && (oldX != X || oldY != Y))
                Send.Player_Move(this, movement);
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
            Warp(TempMap.Get(Class.Spawn_Map.ID), Class.Spawn_X, Class.Spawn_Y);
        }

        public void Attack()
        {
            byte nextX = X, nextY = Y;
            object victim;

            // Próximo azulejo
            NextTile(Direction, ref nextX, ref nextY);

            // Apenas se necessário
            if (Trade != null) return;
            if (Shop != null) return;
            if (Environment.TickCount < Attack_Timer + 750) return;
            if (Map.Tile_Blocked(X, Y, Direction, false)) goto @continue;

            // Ataca um jogador
            victim = Map.HasPlayer(nextX, nextY);
            if (victim != null)
            {
                Attack_Player((Player)victim);
                return;
            }

            // Ataca um NPC
            victim = Map.HasNPC(nextX, nextY);
            if (victim != null)
            {
                Attack_NPC((TempNPC)victim);
                return;
            }

        @continue:
            // Demonstra que aos outros jogadores o ataque
            Send.Player_Attack(this, null);
            Attack_Timer = Environment.TickCount;
        }

        private void Attack_Player(Player victim)
        {
            // Verifica se a vítima pode ser atacada
            if (victim.GettingMap) return;
            if (Map.Data.Moral == (byte)Morals.Pacific)
            {
                Send.Message(this, "This is a peaceful area.", Color.White);
                return;
            }

            // Tempo de ataque 
            Attack_Timer = Environment.TickCount;

            // Cálculo de dano
            short attackDamage = (short)(Damage - victim.Player_Defense);

            // Dano não fatal
            if (attackDamage > 0)
            {
                // Demonstra o ataque aos outros jogadores
                Send.Player_Attack(this, victim.Name, Targets.Player);

                if (attackDamage < victim.Vital[(byte)Vitals.HP])
                {
                    victim.Vital[(byte)Vitals.HP] -= attackDamage;
                    Send.Player_Vitals(victim);
                }
                // FATALITY
                else
                {
                    // Dá 10% da experiência da vítima ao atacante
                    GiveExperience(victim.Experience / 10);

                    // Mata a vítima
                    victim.Died();
                }
            }
            else
                // Demonstra o ataque aos outros jogadores
                Send.Player_Attack(this);
        }

        private void Attack_NPC(TempNPC victim)
        {
            // Mensagem
            if (victim.Target != this && !string.IsNullOrEmpty(victim.Data.SayMsg)) Send.Message(this, victim.Data.Name + ": " + victim.Data.SayMsg, Color.White);

            // Não executa o combate com um NPC amigavel
            switch (victim.Data.Behaviour)
            {
                case NPCBehaviour.Friendly: return;
                case NPCBehaviour.ShopKeeper: Shop_Open(victim.Data.Shop); return;
            }

            // Define o alvo do NPC
            victim.Target = this;

            // Tempo de ataque 
            Attack_Timer = Environment.TickCount;

            // Cálculo de dano
            short attackDamage = (short)(Damage - victim.Data.Attribute[(byte)Attributes.Resistance]);

            // Dano não fatal
            if (attackDamage > 0)
            {
                // Demonstra o ataque aos outros jogadores
                Send.Player_Attack(this, victim.Index.ToString(), Targets.NPC);

                if (attackDamage < victim.Vital[(byte)Vitals.HP])
                {
                    victim.Vital[(byte)Vitals.HP] -= attackDamage;
                    Send.Map_NPC_Vitals(victim);
                }
                // FATALITY
                else
                {
                    // Experiência ganhada
                    GiveExperience(victim.Data.Experience);

                    // Reseta os dados do NPC 
                    victim.Died();
                }
            }
            else
                // Demonstra o ataque aos outros jogadores
                Send.Player_Attack(this);
        }

        public void GiveExperience(int value)
        {
            // Dá a experiência ao jogador, caso ele estiver em um grupo divide a experiência entre os membros
            if (Party.Count > 0 && value > 0) Party_SplitXP(value);
            else Experience += value;

            // Verifica se a experiência não ficou negtiva
            if (Experience < 0) Experience = 0;

            // Verifica se passou de level
            CheckLevelUp();
        }

        private void CheckLevelUp()
        {
            byte numLevel = 0; int expRest;

            while (Experience >= ExpNeeded)
            {
                numLevel++;
                expRest = Experience - ExpNeeded;

                // Define os dados
                Level++;
                Points += Num_Points;
                Experience = expRest;
            }

            // Envia os dados
            Send.Player_Experience(this);
            if (numLevel > 0) Send.Map_Players(this);
        }

        public bool GiveItem(Item item, short amount)
        {
            byte slotItem = FindInventory(item);
            byte slotEmpty = FindInventory(null);

            // Somente se necessário
            if (item == null) return false;
            if (slotEmpty == 0) return false;
            if (amount == 0) amount = 1;

            // Empilhável
            if (slotItem > 0 && item.Stackable)
                Inventory[slotItem].Amount += amount;
            // Não empilhável
            else
            {
                Inventory[slotEmpty].Item = item;
                Inventory[slotEmpty].Amount = item.Stackable ? amount : (byte)1;
            }

            // Envia os dados ao jogador
            Send.Player_Inventory(this);
            return true;
        }

        public void TakeItem(byte slot, short amount)
        {
            // Previne erros
            if (slot <= 0) return;
            if (amount <= 0) amount = 1;

            // Tira o item do jogaor
            if (amount == Inventory[slot].Amount)
            {
                Inventory[slot] = new Inventory();

                // Retira o item da hotbar caso estier
                var hotbarSlot = FindHotbar(Hotbars.Item, slot);
                if (hotbarSlot != null)
                {
                    hotbarSlot = new Hotbar(Hotbars.None, 0);
                    Send.Player_Hotbar(this);
                }
            }
            // Apenas desconta a quantidade
            else
                Inventory[slot].Amount -= amount;

            // Atualiza o inventário
            Send.Player_Inventory(this);
        }

        public void DropItem(byte slot, short amount)
        {
            // Somente se necessário
            if (Map.Item.Count == Max_Map_Items) return;
            if (Inventory[slot].Item == null) return;
            if (Inventory[slot].Item.Bind == BindOn.Pickup) return;
            if (Trade != null) return;

            // Verifica se não está dropando mais do que tem
            if (amount > Inventory[slot].Amount) amount = Inventory[slot].Amount;

            // Solta o item no chão
            Map.Item.Add(new MapItems
            {
                Item = Inventory[slot].Item,
                Amount = amount,
                X = X,
                Y = Y
            });
            Send.Map_Items(Map);

            // Retira o item do inventário do jogador 
            TakeItem(slot, amount);
        }

        public void UseItem(byte slot)
        {
            Item item = Inventory[slot].Item;

            // Somente se necessário
            if (item == null) return;
            if (Trade != null) return;

            // Requerimentos
            if (Level < item.Req_Level)
            {
                Send.Message(this, "You do not have the level required to use this item.", Color.White);
                return;
            }
            if (item.Req_Class != null)
                if (Class != item.Req_Class)
                {
                    Send.Message(this, "You can not use this item.", Color.White);
                    return;
                }

            if (item.Type == Items.Equipment)
            {
                // Retira o item do inventário
                TakeItem(slot, 1);

                // Caso já estiver com algum equipamento, desequipa ele
                Item currentEquip = Equipment[item.Equip_Type];
                if (currentEquip != null) GiveItem(currentEquip, 1);

                // Equipa o item
                Equipment[item.Equip_Type] = item;
                for (byte i = 0; i < (byte)Attributes.Count; i++) Attribute[i] += item.Equip_Attribute[i];

                // Envia os dados
                Send.Player_Inventory(this);
                Send.Player_Equipments(this);
                Send.Player_Hotbar(this);
            }
            else if (item.Type == Items.Potion)
            {
                // Efeitos
                bool hadEffect = false;
                GiveExperience(item.Potion_Experience);
                for (byte i = 0; i < (byte)Vitals.Count; i++)
                {
                    // Verifica se o item causou algum efeito 
                    if (Vital[i] < MaxVital(i) && item.Potion_Vital[i] != 0) hadEffect = true;

                    // Efeito
                    Vital[i] += item.Potion_Vital[i];

                    // Impede que passe dos limites
                    if (Vital[i] < 0) Vital[i] = 0;
                    if (Vital[i] > MaxVital(i)) Vital[i] = MaxVital(i);
                }

                // Foi fatal
                if (Vital[(byte)Vitals.HP] == 0) Died();

                // Remove o item caso tenha tido algum efeito
                if (item.Potion_Experience > 0 || hadEffect) TakeItem(slot, 1);
            }
        }

        public Hotbar FindHotbar(Hotbars type, byte slot)
        {
            // Encontra algo especifico na hotbar
            for (byte i = 0; i < MaxHotbar; i++)
                if (Hotbar[i].Type == type && Hotbar[i].Slot == slot)
                    return Hotbar[i];

            return null;
        }

        public byte FindInventory(Item item)
        {
            // Encontra algo especifico na hotbar
            for (byte i = 1; i <= MaxInventory; i++)
                if (Inventory[i].Item == item)
                    return i;

            return 0;
        }

        public byte Total_Inventory_Free()
        {
            byte total = 0;

            // Retorna a quantidade de itens oferecidos na troca
            for (byte i = 1; i <= MaxInventory; i++)
                if (Inventory[i].Item == null)
                    total++;

            return total;
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

        private void Party_SplitXP(int value)
        {
            // Somatório do level de todos os jogadores do grupo
            int givenExperience, experienceSum = 0, difference;
            double[] diff = new double[Party.Count];
            double diffSum = 0, k;

            // Cálcula a diferença dos leveis entre os jogadores
            for (byte i = 0; i < Party.Count; i++)
            {
                difference = Math.Abs(Level - Party[i].Level);

                // Constante para a diminuir potêncialmente a experiência que diferenças altas ganhariam
                if (difference < 3) k = 1.15;
                else if (difference < 6) k = 1.55;
                else if (difference < 10) k = 1.85;
                else k = 2.3;

                // Transforma o valor em fração
                diff[i] = 1 / Math.Pow(k, Math.Min(15, difference));
                diffSum += diff[i];
            }

            // Divide a experiência pro grupo com base na diferença dos leveis 
            for (byte i = 0; i < Party.Count; i++)
            {
                // Caso a somatório for maior que um (100%) balanceia os valores
                if (diffSum > 1) diff[i] *= 1 / diffSum;

                // Divide a experiência
                givenExperience = (int)((value / 2) * diff[i]);
                experienceSum += givenExperience;
                Party[i].GiveExperience(givenExperience);
                Send.Player_Experience(Party[i]);
            }

            // Dá ao jogador principal o restante da experiência
            Experience += value - experienceSum;
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
            byte total = 0;

            // Retorna a quantidade de itens oferecidos na troca
            for (byte i = 1; i <= MaxInventory; i++)
                if (Trade_Offer[i].Slot_Num != 0)
                    total++;

            return total;
        }

        public void Shop_Open(Shop shop)
        {
            // Abre a loja
            Shop = shop;
            Send.Shop_Open(this, shop);
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
        public static Player Find(string name)
        {
            // Encontra o usuário
            foreach (var account in Account.List)
                if (account.IsPlaying)
                    if (account.Character.Name.Equals(name))
                        return account.Character;

            return null;
        }
    }

    struct Inventory
    {
        public Item Item;
        public short Amount;
    }

    class TradeSlot
    {
        public short Slot_Num;
        public short Amount;
    }

    class Hotbar
    {
        public Hotbars Type;
        public byte Slot;

        public Hotbar(Hotbars type, byte slot)
        {
            Type = type;
            Slot = slot;
        }
    }
}