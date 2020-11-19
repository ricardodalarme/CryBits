using System;
using System.Collections.Generic;
using System.Drawing;
using CryBits.Entities;
using CryBits.Server.Library;
using CryBits.Server.Logic;
using CryBits.Server.Network;
using static CryBits.Defaults;
using static CryBits.Utils;

namespace CryBits.Server.Entities
{
    internal class Player : Character
    {
        // Dados permantes
        public string Name { get; set; } = string.Empty;
        public Class Class { get; set; }
        public short TextureNum { get; set; }
        public bool Genre { get; set; }
        public short Level { get; set; }
        public int Experience { get; set; }
        public byte Points { get; set; }
        public short[] Attribute { get; set; } = new short[(byte)Attributes.Count];
        public ItemSlot[] Inventory { get; } = new ItemSlot[MaxInventory];
        public Item[] Equipment { get; } = new Item[(byte)Equipments.Count];
        public Hotbar[] Hotbar { get; } = new Hotbar[MaxHotbar];

        // Dados temporários
        public bool GettingMap;
        public List<Player> Party = new List<Player>();
        public string PartyRequest;
        public Player Trade;
        public string TradeRequest;
        public TradeSlot[] TradeOffer;
        public Shop Shop;
        public Account Account;
        private int _attackTimer;

        // Constutor
        public Player(Account account)
        {
            Account = account;
            for (byte i = 0; i < Inventory.Length; i++)
                Inventory[i] = new ItemSlot(null, 0);
        }

        // Cálcula o dano do jogador
        public short Damage
        {
            get
            {
                short value = Attribute[(byte)Attributes.Strength];
                if (Equipment[(byte)Equipments.Weapon] != null) value += Equipment[(byte)Equipments.Weapon].WeaponDamage;
                return value;
            }
        }

        // Cálcula a defesa do jogador
        public short PlayerDefense => Attribute[(byte)Attributes.Resistance];

        public short MaxVital(byte vital)
        {
            short[] @base = Class.Vital;

            // Cálcula o máximo de vital que um jogador possui
            switch ((Vitals)vital)
            {
                case Vitals.HP: return (short)(@base[vital] + Attribute[(byte)Attributes.Vitality] * 1.50 * (Level * 0.75) + 1);
                case Vitals.MP: return (short)(@base[vital] + Attribute[(byte)Attributes.Intelligence] * 1.25 * (Level * 0.5) + 1);
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
            if (Environment.TickCount > Loop.TimerRegen + 5000)
                for (byte v = 0; v < (byte)Vitals.Count; v++)
                    if (Vital[v] < MaxVital(v))
                    {
                        // Renera a vida do jogador
                        Vital[v] += Regeneration(v);
                        if (Vital[v] > MaxVital(v)) Vital[v] = MaxVital(v);

                        // Env ia os dados aos jogadores
                        Send.PlayerVitals(this);
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
            Send.MapPlayers(this);
            Send.PlayerExperience(this);
            Send.PlayerInventory(this);
            Send.PlayerHotbar(this);

            // Transporta o jogador para a sua determinada Posição
            Warp(Map, X, Y, true);

            // Entra no jogo
            Send.JoinGame(this);
            Send.Message(this, WelcomeMessage, Color.Blue);
        }

        public void Leave()
        {
            // Salva os dados do jogador e atualiza os demais jogadores da desconexão
            Write.Character(Account);
            Send.PlayerLeave(this);

            // Sai dos grupos
            PartyLeave();
            TradeLeave();
        }

        public void Warp(TempMap to, byte x, byte y, bool needUpdate = false)
        {
            TempMap mapOld = Map;

            // Cancela a troca ou a loja
            if (Trade != null) TradeLeave();
            if (Shop != null) ShopLeave();

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
                Send.PlayerLeaveMap(this, mapOld);

                // Inviabiliza o jogador de algumas ações até que ele receba os dados necessários
                GettingMap = true;

                // Envia dados necessários do mapa
                Send.MapRevision(this, to.Data);
                Send.MapItems(this, to);
                Send.MapNPCs(this, to);
            }
            // Apenas atualiza a posição do jogador
            else
                Send.PlayerPosition(this);
        }

        public void Move(byte movement)
        {
            byte nextX = X, nextY = Y;
            byte oldX = X, oldY = Y;
            TempMap link = TempMap.Get(new Guid(Map.Data.Link[(byte)Direction].GetID()));
            bool secondMovement = false;

            // Previne erros
            if (movement < 1 || movement > 2) return;
            if (GettingMap) return;

            // Cancela a troca ou a loja
            if (Trade != null) TradeLeave();
            if (Shop != null) ShopLeave();

            // Próximo azulejo
            NextTile(Direction, ref nextX, ref nextY);

            // Ponto de ligação
            if (CryBits.Entities.Map.OutLimit(nextX, nextY))
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
                    Send.PlayerPosition(this);
                    return;
                }
            }
            // Bloqueio
            else if (!Map.TileBlocked(oldX, oldY, Direction))
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
                    if (tile.Data4 > 0) Direction = (Directions)tile.Data4 - 1;
                    Warp(TempMap.Get(new Guid(tile.Data1)), (byte)tile.Data2, (byte)tile.Data3);
                    secondMovement = true;
                    break;
            }

            // Envia os dados
            if (!secondMovement && (oldX != X || oldY != Y))
                Send.PlayerMove(this, movement);
            else
                Send.PlayerPosition(this);
        }

        public void Died()
        {
            // Recupera os vitais
            for (byte n = 0; n < (byte)Vitals.Count; n++) Vital[n] = MaxVital(n);
            Send.PlayerVitals(this);

            // Perde 10% da experiência
            Experience /= 10;
            Send.PlayerExperience(this);

            // Retorna para o ínicio
            Direction = (Directions)Class.SpawnDirection;
            Warp(TempMap.Get(Class.SpawnMap.ID), Class.SpawnX, Class.SpawnY);
        }

        public void Attack()
        {
            byte nextX = X, nextY = Y;
            Character victim;

            // Próximo azulejo
            NextTile(Direction, ref nextX, ref nextY);

            // Apenas se necessário
            if (Trade != null) return;
            if (Shop != null) return;
            if (Environment.TickCount < _attackTimer + AttackSpeed) return;
            if (Map.TileBlocked(X, Y, Direction, false)) goto @continue;

            // Ataca um jogador
            victim = Map.HasPlayer(nextX, nextY);
            if (victim != null)
            {
                AttackPlayer((Player)victim);
                return;
            }

            // Ataca um NPC
            victim = Map.HasNPC(nextX, nextY);
            if (victim != null)
            {
                AttackNPC((TempNPC)victim);
                return;
            }

        @continue:
            // Demonstra que aos outros jogadores o ataque
            Send.PlayerAttack(this, null);
            _attackTimer = Environment.TickCount;
        }

        private void AttackPlayer(Player victim)
        {
            // Verifica se a vítima pode ser atacada
            if (victim.GettingMap) return;
            if (Map.Data.Moral == (byte)Morals.Pacific)
            {
                Send.Message(this, "This is a peaceful area.", Color.White);
                return;
            }

            // Tempo de ataque 
            _attackTimer = Environment.TickCount;

            // Cálculo de dano
            short attackDamage = (short)(Damage - victim.PlayerDefense);

            // Dano não fatal
            if (attackDamage > 0)
            {
                // Demonstra o ataque aos outros jogadores
                Send.PlayerAttack(this, victim.Name, Targets.Player);

                if (attackDamage < victim.Vital[(byte)Vitals.HP])
                {
                    victim.Vital[(byte)Vitals.HP] -= attackDamage;
                    Send.PlayerVitals(victim);
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
                Send.PlayerAttack(this);
        }

        private void AttackNPC(TempNPC victim)
        {
            // Mensagem
            if (victim.Target != this && !string.IsNullOrEmpty(victim.Data.SayMsg)) Send.Message(this, victim.Data.Name + ": " + victim.Data.SayMsg, Color.White);

            // Não executa o combate com um NPC amigavel
            switch (victim.Data.Behaviour)
            {
                case NPCs.Friendly: return;
                case NPCs.ShopKeeper: ShopOpen(victim.Data.Shop); return;
            }

            // Define o alvo do NPC
            victim.Target = this;

            // Tempo de ataque 
            _attackTimer = Environment.TickCount;

            // Cálculo de dano
            short attackDamage = (short)(Damage - victim.Data.Attribute[(byte)Attributes.Resistance]);

            // Dano não fatal
            if (attackDamage > 0)
            {
                // Demonstra o ataque aos outros jogadores
                Send.PlayerAttack(this, victim.Index.ToString(), Targets.NPC);

                if (attackDamage < victim.Vital[(byte)Vitals.HP])
                {
                    victim.Vital[(byte)Vitals.HP] -= attackDamage;
                    Send.MapNPCVitals(victim);
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
                Send.PlayerAttack(this);
        }

        public void GiveExperience(int value)
        {
            // Dá a experiência ao jogador, caso ele estiver em um grupo divide a experiência entre os membros
            if (Party.Count > 0 && value > 0) PartySplitXP(value);
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
                Points += NumPoints;
                Experience = expRest;
            }

            // Envia os dados
            Send.PlayerExperience(this);
            if (numLevel > 0) Send.MapPlayers(this);
        }

        public bool GiveItem(Item item, short amount)
        {
            ItemSlot slotItem = FindInventory(item);
            ItemSlot slotEmpty = FindInventory(null);

            // Somente se necessário
            if (item == null) return false;
            if (slotEmpty == null) return false;
            if (amount == 0) amount = 1;

            // Empilhável
            if (slotItem != null && item.Stackable)
                slotItem.Amount += amount;
            // Não empilhável
            else
            {
                slotEmpty.Item = item;
                slotEmpty.Amount = item.Stackable ? amount : (byte)1;
            }

            // Envia os dados ao jogador
            Send.PlayerInventory(this);
            return true;
        }

        public void TakeItem(ItemSlot slot, short amount)
        {
            // Previne erros
            if (slot == null) return;
            if (amount <= 0) amount = 1;

            // Tira o item do jogaor
            if (amount == slot.Amount)
            {
                slot.Item = null;
                slot.Amount = 0;

                // Retira o item da hotbar caso estier
                var hotbarSlot = FindHotbar(Hotbars.Item, slot);
                if (hotbarSlot != null)
                {
                    hotbarSlot.Type = Hotbars.None;
                    hotbarSlot.Slot = 0;
                    Send.PlayerHotbar(this);
                }
            }
            // Apenas desconta a quantidade
            else
                slot.Amount -= amount;

            // Atualiza o inventário
            Send.PlayerInventory(this);
        }

        public void DropItem(short slot, short amount)
        {
            if (slot != -1)
                DropItem(Inventory[slot], amount);
        }

        public void DropItem(ItemSlot slot, short amount)
        {
            // Somente se necessário
            if (Map.Item.Count == MaxMapItems) return;
            if (slot.Item == null) return;
            if (slot.Item.Bind == BindOn.Pickup) return;
            if (Trade != null) return;

            // Verifica se não está dropando mais do que tem
            if (amount > slot.Amount) amount = slot.Amount;

            // Solta o item no chão
            Map.Item.Add(new MapItems(slot.Item, amount, X, Y));
            Send.MapItems(Map);

            // Retira o item do inventário do jogador 
            TakeItem(slot, amount);
        }

        public void UseItem(short slot)
        {
            if (slot != -1)
                UseItem(Inventory[slot]);
        }

        public void UseItem(ItemSlot slot)
        {
            Item item = slot.Item;

            // Somente se necessário
            if (item == null) return;
            if (Trade != null) return;

            // Requerimentos
            if (Level < item.ReqLevel)
            {
                Send.Message(this, "You do not have the level required to use this item.", Color.White);
                return;
            }
            if (item.ReqClass != null)
                if (Class != item.ReqClass)
                {
                    Send.Message(this, "You can not use this item.", Color.White);
                    return;
                }

            if (item.Type == Items.Equipment)
            {
                // Retira o item do inventário
                TakeItem(slot, 1);

                // Caso já estiver com algum equipamento, desequipa ele
                Item currentEquip = Equipment[item.EquipType];
                if (currentEquip != null) GiveItem(currentEquip, 1);

                // Equipa o item
                Equipment[item.EquipType] = item;
                for (byte i = 0; i < (byte)Attributes.Count; i++) Attribute[i] += item.EquipAttribute[i];

                // Envia os dados
                Send.PlayerInventory(this);
                Send.PlayerEquipments(this);
                Send.PlayerHotbar(this);
            }
            else if (item.Type == Items.Potion)
            {
                // Efeitos
                bool hadEffect = false;
                GiveExperience(item.PotionExperience);
                for (byte i = 0; i < (byte)Vitals.Count; i++)
                {
                    // Verifica se o item causou algum efeito 
                    if (Vital[i] < MaxVital(i) && item.PotionVital[i] != 0) hadEffect = true;

                    // Efeito
                    Vital[i] += item.PotionVital[i];

                    // Impede que passe dos limites
                    if (Vital[i] < 0) Vital[i] = 0;
                    if (Vital[i] > MaxVital(i)) Vital[i] = MaxVital(i);
                }

                // Foi fatal
                if (Vital[(byte)Vitals.HP] == 0) Died();

                // Remove o item caso tenha tido algum efeito
                if (item.PotionExperience > 0 || hadEffect) TakeItem(slot, 1);
            }
        }

        public Hotbar FindHotbar(Hotbars type, short slot)
        {
            // Encontra algo especifico na hotbar
            for (byte i = 0; i < MaxHotbar; i++)
                if (Hotbar[i].Type == type && Hotbar[i].Slot == slot)
                    return Hotbar[i];

            return null;
        }

        public Hotbar FindHotbar(Hotbars type, ItemSlot slot)
        {
            // Encontra algo especifico na hotbar
            for (byte i = 0; i < MaxHotbar; i++)
                if (Hotbar[i].Type == type && Inventory[Hotbar[i].Slot] == slot)
                    return Hotbar[i];

            return null;
        }

        public ItemSlot FindInventory(Item item)
        {
            // Encontra algo especifico na hotbar
            for (byte i = 0; i < MaxInventory; i++)
                if (Inventory[i].Item == item)
                    return Inventory[i];

            return null;
        }

        public byte TotalInventoryFree()
        {
            byte total = 0;

            // Retorna a quantidade de itens oferecidos na troca
            for (byte i = 0; i < MaxInventory; i++)
                if (Inventory[i].Item == null)
                    total++;

            return total;
        }

        public void PartyLeave()
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

        private void PartySplitXP(int value)
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
                givenExperience = (int)(value / 2 * diff[i]);
                experienceSum += givenExperience;
                Party[i].GiveExperience(givenExperience);
                Send.PlayerExperience(Party[i]);
            }

            // Dá ao jogador principal o restante da experiência
            Experience += value - experienceSum;
            CheckLevelUp();
            Send.PlayerExperience(this);
        }

        public void TradeLeave()
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

        public byte TotalTradeItems()
        {
            byte total = 0;

            // Retorna a quantidade de itens oferecidos na troca
            for (byte i = 0; i < MaxInventory; i++)
                if (TradeOffer[i].SlotNum != 0)
                    total++;

            return total;
        }

        public void ShopOpen(Shop shop)
        {
            // Abre a loja
            Shop = shop;
            Send.ShopOpen(this, shop);
        }

        public void ShopLeave()
        {
            // Fecha a loja
            Shop = null;
            Send.ShopOpen(this, null);
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

    internal class Hotbar
    {
        public Hotbars Type;
        public short Slot;

        public Hotbar(Hotbars type, short slot)
        {
            Type = type;
            Slot = slot;
        }
    }
}