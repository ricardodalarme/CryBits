using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CryBits.Entities;
using CryBits.Entities.Shop;
using CryBits.Entities.Slots;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Server.Entities.TempMap;
using CryBits.Server.Library;
using CryBits.Server.Logic;
using CryBits.Server.Network;
using static CryBits.Globals;
using static CryBits.Utils;

namespace CryBits.Server.Entities;

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
    public short[] Attribute { get; set; } = new short[(byte)Enums.Attribute.Count];
    public ItemSlot[] Inventory { get; } = new ItemSlot[MaxInventory];
    public Item[] Equipment { get; } = new Item[(byte)Enums.Equipment.Count];
    public HotbarSlot[] Hotbar { get; } = new HotbarSlot[MaxHotbar];

    // Dados temporários
    public bool GettingMap;
    public List<Player> Party = new();
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
            var value = Attribute[(byte)Enums.Attribute.Strength];
            if (Equipment[(byte)Enums.Equipment.Weapon] != null) value += Equipment[(byte)Enums.Equipment.Weapon].WeaponDamage;
            return value;
        }
    }

    // Cálcula a defesa do jogador
    public short PlayerDefense => Attribute[(byte)Enums.Attribute.Resistance];

    public short MaxVital(byte vital)
    {
        var @base = Class.Vital;

        // Cálcula o máximo de vital que um jogador possui
        switch ((Vital)vital)
        {
            case Enums.Vital.Hp: return (short)(@base[vital] + (Attribute[(byte)Enums.Attribute.Vitality] * 1.50 * (Level * 0.75)) + 1);
            case Enums.Vital.Mp: return (short)(@base[vital] + (Attribute[(byte)Enums.Attribute.Intelligence] * 1.25 * (Level * 0.5)) + 1);
        }

        return 1;
    }

    public short Regeneration(byte vital)
    {
        // Cálcula o máximo de vital que um jogador possui
        switch ((Vital)vital)
        {
            case Enums.Vital.Hp: return (short)((MaxVital(vital) * 0.05) + (Attribute[(byte)Enums.Attribute.Vitality] * 0.3));
            case Enums.Vital.Mp: return (short)((MaxVital(vital) * 0.05) + (Attribute[(byte)Enums.Attribute.Intelligence] * 0.1));
        }

        return 1;
    }

    // Quantidade de experiência para passar para o próximo level
    public int ExpNeeded
    {
        get
        {
            short total = 0;
            for (byte i = 0; i < (byte)Enums.Attribute.Count; i++) total += Attribute[i];
            return (int)(((Level + 1) * 2.5) + ((total + Points) / 2));
        }
    }

    /////////////
    // Funções //
    /////////////
    public void Logic()
    {
        // Reneração 
        if (Environment.TickCount > Loop.TimerRegeneration + 5000)
            for (byte v = 0; v < (byte)Enums.Vital.Count; v++)
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
        Send.Npcs(Account);
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

    public void Warp(TempMap.TempMap map, byte x, byte y, bool needUpdate = false)
    {
        var oldMap = Map;

        // Cancela a troca ou a loja
        if (Trade != null) TradeLeave();
        if (Shop != null) ShopLeave();

        // Evita que o jogador seja transportado para fora do limite
        if (map == null) return;
        if (x >= CryBits.Entities.Map.Map.Width) x = CryBits.Entities.Map.Map.Width - 1;
        if (y >= CryBits.Entities.Map.Map.Height) y = CryBits.Entities.Map.Map.Height - 1;

        // Define a Posição do jogador
        Map = map;
        X = x;
        Y = y;

        // Altera o mapa
        if (oldMap != map || needUpdate)
        {
            // Sai do mapa antigo
            Send.PlayerLeaveMap(this, oldMap);

            // Inviabiliza o jogador de algumas ações até que ele receba os dados necessários
            GettingMap = true;

            // Envia dados necessários do mapa
            Send.MapRevision(this, map.Data);
            Send.MapItems(this, map);
            Send.MapNpcs(this, map);
        }
        // Apenas atualiza a posição do jogador
        else
            Send.PlayerPosition(this);
    }

    public void Move(byte movement)
    {
        byte nextX = X, nextY = Y;
        byte oldX = X, oldY = Y;
        var link = TempMap.TempMap.List.Get(Map.Data.Link[(byte)Direction].GetId());
        var secondMovement = false;

        // Previne erros
        if (movement < 1 || movement > 2) return;
        if (GettingMap) return;

        // Cancela a troca ou a loja
        if (Trade != null) TradeLeave();
        if (Shop != null) ShopLeave();

        // Próximo azulejo
        NextTile(Direction, ref nextX, ref nextY);

        // Ponto de ligação
        if (CryBits.Entities.Map.Map.OutLimit(nextX, nextY))
        {
            if (link != null)
                switch (Direction)
                {
                    case Direction.Up: Warp(link, oldX, CryBits.Entities.Map.Map.Height - 1); return;
                    case Direction.Down: Warp(link, oldX, 0); return;
                    case Direction.Right: Warp(link, 0, oldY); return;
                    case Direction.Left: Warp(link, CryBits.Entities.Map.Map.Width - 1, oldY); return;
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
        var tile = Map.Data.Attribute[nextX, nextY];

        switch ((TileAttribute)tile.Type)
        {
            // Teletransporte
            case TileAttribute.Warp:
                if (tile.Data4 > 0) Direction = (Direction)tile.Data4 - 1;
                Warp(TempMap.TempMap.List.Get(new Guid(tile.Data1)), (byte)tile.Data2, (byte)tile.Data3);
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
        for (byte n = 0; n < (byte)Enums.Vital.Count; n++) Vital[n] = MaxVital(n);
        Send.PlayerVitals(this);

        // Perde 10% da experiência
        Experience /= 10;
        Send.PlayerExperience(this);

        // Retorna para o ínicio
        Direction = (Direction)Class.SpawnDirection;
        Warp(TempMap.TempMap.List.Get(Class.SpawnMap.Id), Class.SpawnX, Class.SpawnY);
    }

    public void Attack()
    {
        byte nextX = X, nextY = Y;

        // Próximo azulejo
        NextTile(Direction, ref nextX, ref nextY);

        // Apenas se necessário
        if (Trade != null) return;
        if (Shop != null) return;
        if (Environment.TickCount < _attackTimer + AttackSpeed) return;
        if (Map.TileBlocked(X, Y, Direction, false)) goto @continue;

        // Ataca um jogador
        Character victim = Map.HasPlayer(nextX, nextY);
        if (victim != null)
        {
            AttackPlayer((Player)victim);
            return;
        }

        // Ataca um Npc
        victim = Map.HasNpc(nextX, nextY);
        if (victim != null)
        {
            AttackNpc((TempNpc)victim);
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
        if (Map.Data.Moral == (byte)Moral.Pacific)
        {
            Send.Message(this, "This is a peaceful area.", Color.White);
            return;
        }

        // Tempo de ataque 
        _attackTimer = Environment.TickCount;

        // Cálculo de dano
        var attackDamage = (short)(Damage - victim.PlayerDefense);

        // Dano não fatal
        if (attackDamage > 0)
        {
            // Demonstra o ataque aos outros jogadores
            Send.PlayerAttack(this, victim.Name, Target.Player);

            if (attackDamage < victim.Vital[(byte)Enums.Vital.Hp])
            {
                victim.Vital[(byte)Enums.Vital.Hp] -= attackDamage;
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

    private void AttackNpc(TempNpc victim)
    {
        // Mensagem
        if (victim.Target != this && !string.IsNullOrEmpty(victim.Data.SayMsg)) Send.Message(this, victim.Data.Name + ": " + victim.Data.SayMsg, Color.White);

        // Não executa o combate com um Npc amigavel
        switch (victim.Data.Behaviour)
        {
            case Behaviour.Friendly: return;
            case Behaviour.ShopKeeper: ShopOpen(victim.Data.Shop); return;
        }

        // Define o alvo do Npc
        victim.Target = this;

        // Tempo de ataque 
        _attackTimer = Environment.TickCount;

        // Cálculo de dano
        var attackDamage = (short)(Damage - victim.Data.Attribute[(byte)Enums.Attribute.Resistance]);

        // Dano não fatal
        if (attackDamage > 0)
        {
            // Demonstra o ataque aos outros jogadores
            Send.PlayerAttack(this, victim.Index.ToString(), Target.Npc);

            if (attackDamage < victim.Vital[(byte)Enums.Vital.Hp])
            {
                victim.Vital[(byte)Enums.Vital.Hp] -= attackDamage;
                Send.MapNpcVitals(victim);
            }
            // FATALITY
            else
            {
                // Experiência ganhada
                GiveExperience(victim.Data.Experience);

                // Reseta os dados do Npc 
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
        if (Party.Count > 0 && value > 0) PartySplitXp(value);
        else Experience += value;

        // Verifica se a experiência não ficou negtiva
        if (Experience < 0) Experience = 0;

        // Verifica se passou de level
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        byte numLevel = 0;

        while (Experience >= ExpNeeded)
        {
            numLevel++;
            var expRest = Experience - ExpNeeded;

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
        var slotItem = FindInventory(item);
        var slotEmpty = FindInventory(null);

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
            var hotbarSlot = FindHotbar(SlotType.Item, slot);
            if (hotbarSlot != null)
            {
                hotbarSlot.Type = SlotType.None;
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
        Map.Item.Add(new TempMapItems(slot.Item, amount, X, Y));
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
        var item = slot.Item;

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

        if (item.Type == ItemType.Equipment)
        {
            // Retira o item do inventário
            TakeItem(slot, 1);

            // Caso já estiver com algum equipamento, desequipa ele
            var currentEquip = Equipment[item.EquipType];
            if (currentEquip != null) GiveItem(currentEquip, 1);

            // Equipa o item
            Equipment[item.EquipType] = item;
            for (byte i = 0; i < (byte)Enums.Attribute.Count; i++) Attribute[i] += item.EquipAttribute[i];

            // Envia os dados
            Send.PlayerInventory(this);
            Send.PlayerEquipments(this);
            Send.PlayerHotbar(this);
        }
        else if (item.Type == ItemType.Potion)
        {
            // Efeitos
            var hadEffect = false;
            GiveExperience(item.PotionExperience);
            for (byte i = 0; i < (byte)Enums.Vital.Count; i++)
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
            if (Vital[(byte)Enums.Vital.Hp] == 0) Died();

            // Remove o item caso tenha tido algum efeito
            if (item.PotionExperience > 0 || hadEffect) TakeItem(slot, 1);
        }
    }

    public HotbarSlot FindHotbar(SlotType type, short slot) => Hotbar.First(x => x.Type == type && x.Slot == slot);

    public HotbarSlot FindHotbar(SlotType type, ItemSlot slot) => Hotbar.First(x => x.Type == type && Inventory[x.Slot] == slot);

    public ItemSlot FindInventory(Item item) => Inventory.First(x => x.Item == item);

    public byte TotalInventoryFree => (byte)Inventory.Count(x => x.Item != null);

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

    private void PartySplitXp(int value)
    {
        // Somatório do level de todos os jogadores do grupo
        int experienceSum = 0;
        var diff = new double[Party.Count];
        double diffSum = 0;

        // Cálcula a diferença dos leveis entre os jogadores
        for (byte i = 0; i < Party.Count; i++)
        {
            var difference = Math.Abs(Level - Party[i].Level);

            // Constante para a diminuir potêncialmente a experiência que diferenças altas ganhariam
            double k;
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
            var givenExperience = (int)(value / 2 * diff[i]);
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

    public byte TotalTradeItems => (byte)TradeOffer.Count(x => x.SlotNum != 0);

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

    public static Player Find(string name) => Account.List.Find(x => x.IsPlaying && x.Character.Name.Equals(name))?.Character;
}