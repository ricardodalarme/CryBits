using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CryBits.Entities;
using CryBits.Entities.Shop;
using CryBits.Entities.Slots;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Server.Library.Repositories;
using CryBits.Server.Network.Senders;
using CryBits.Server.Systems;
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
    public long AttackTimer;

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
            if (Equipment[(byte)Enums.Equipment.Weapon] != null)
                value += Equipment[(byte)Enums.Equipment.Weapon].WeaponDamage;
            return value;
        }
    }

    // Cálcula a defesa do jogador
    public short PlayerDefense => Attribute[(byte)Enums.Attribute.Resistance];

    public short MaxVital(byte vital)
    {
        var @base = Class.Vital;

        // Cálcula o máximo de vital que um jogador possui
        return (Vital)vital switch
        {
            Enums.Vital.Hp => (short)(@base[vital] + Attribute[(byte)Enums.Attribute.Vitality] * 1.50 * (Level * 0.75) +
                                      1),
            Enums.Vital.Mp => (short)(@base[vital] +
                                      Attribute[(byte)Enums.Attribute.Intelligence] * 1.25 * (Level * 0.5) + 1),
            _ => 1
        };
    }

    public short Regeneration(byte vital)
    {
        // Cálcula o máximo de vital que um jogador possui
        return (Vital)vital switch
        {
            Enums.Vital.Hp => (short)(MaxVital(vital) * 0.05 + Attribute[(byte)Enums.Attribute.Vitality] * 0.3),
            Enums.Vital.Mp => (short)(MaxVital(vital) * 0.05 + Attribute[(byte)Enums.Attribute.Intelligence] * 0.1),
            _ => 1
        };
    }

    // Quantidade de experiência para passar para o próximo level
    public int ExpNeeded
    {
        get
        {
            short total = 0;
            for (byte i = 0; i < (byte)Enums.Attribute.Count; i++) total += Attribute[i];
            return (int)((Level + 1) * 2.5 + (total + Points) / 2);
        }
    }

    public void Join()
    {
        // Limpa os dados dos outros personagens
        Account.Characters = null;

        // Envia todos os dados necessários
        PlayerSender.Join(this);
        ItemSender.Items(Account);
        NpcSender.Npcs(Account);
        ShopSender.Shops(Account);
        MapSender.Map(Account, Map.Data);
        MapSender.MapPlayers(this);
        PlayerSender.PlayerExperience(this);
        PlayerSender.PlayerInventory(this);
        PlayerSender.PlayerHotbar(this);

        // Warp to starting position
        MovementSystem.Warp(this, Map, X, Y, true);

        // Entra no jogo
        PlayerSender.JoinGame(this);
        ChatSender.Message(this, WelcomeMessage, Color.Blue);
    }

    public void Leave()
    {
        // Salva os dados do jogador e atualiza os demais jogadores da desconexão
        CharacterRepository.Write(Account);
        PlayerSender.PlayerLeave(this);

        // Leave party and any active trade
        PartySystem.Leave(this);
        TradeSystem.Leave(this);
    }

    public void Died()
    {
        // Recupera os vitais
        for (byte n = 0; n < (byte)Enums.Vital.Count; n++) Vital[n] = MaxVital(n);
        PlayerSender.PlayerVitals(this);

        // Perde 10% da experiência
        Experience /= 10;
        PlayerSender.PlayerExperience(this);

        // Return to spawn
        Direction = (Direction)Class.SpawnDirection;
        MovementSystem.Warp(this, TempMap.TempMap.List.Get(Class.SpawnMap.Id), Class.SpawnX, Class.SpawnY);
    }

    public void Attack()
    {
        byte nextX = X, nextY = Y;

        // Próximo azulejo
        NextTile(Direction, ref nextX, ref nextY);

        // Apenas se necessário
        if (Trade != null) return;
        if (Shop != null) return;
        if (Environment.TickCount64 < AttackTimer + AttackSpeed) return;
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
        // Demonstrate the attack to other players
        PlayerSender.PlayerAttack(this, null);
        AttackTimer = Environment.TickCount64;
    }

    private void AttackPlayer(Player victim)
    {
        // Verifica se a vítima pode ser atacada
        if (victim.GettingMap) return;
        if (Map.Data.Moral == (byte)Moral.Pacific)
        {
            ChatSender.Message(this, "This is a peaceful area.", Color.White);
            return;
        }

        AttackTimer = Environment.TickCount64;

        // Damage calculation
        var attackDamage = (short)(Damage - victim.PlayerDefense);

        // Dano não fatal
        if (attackDamage > 0)
        {
            // Demonstra o ataque aos outros jogadores
            PlayerSender.PlayerAttack(this, victim.Name, Target.Player);

            if (attackDamage < victim.Vital[(byte)Enums.Vital.Hp])
            {
                victim.Vital[(byte)Enums.Vital.Hp] -= attackDamage;
                PlayerSender.PlayerVitals(victim);
            }
            // FATALITY
            else
            {
                // Award 10% of the victim's XP to the attacker
                LevelingSystem.GiveExperience(this, victim.Experience / 10);

                // Mata a vítima
                victim.Died();
            }
        }
        else
            // Demonstra o ataque aos outros jogadores
            PlayerSender.PlayerAttack(this);
    }

    private void AttackNpc(TempNpc victim)
    {
        // Mensagem
        if (victim.Target != this && !string.IsNullOrEmpty(victim.Data.SayMsg))
            ChatSender.Message(this, victim.Data.Name + ": " + victim.Data.SayMsg, Color.White);

        // Não executa o combate com um Npc amigavel
        switch (victim.Data.Behaviour)
        {
            case Behaviour.Friendly: return;
            case Behaviour.ShopKeeper:
                ShopSystem.Open(this, victim.Data.Shop);
                return;
        }

        // Define o alvo do Npc
        victim.Target = this;

        AttackTimer = Environment.TickCount64;

        // Damage calculation
        var attackDamage = (short)(Damage - victim.Data.Attribute[(byte)Enums.Attribute.Resistance]);

        // Dano não fatal
        if (attackDamage > 0)
        {
            // Demonstra o ataque aos outros jogadores
            PlayerSender.PlayerAttack(this, victim.Index.ToString(), Target.Npc);

            if (attackDamage < victim.Vital[(byte)Enums.Vital.Hp])
            {
                victim.Vital[(byte)Enums.Vital.Hp] -= attackDamage;
                NpcSender.MapNpcVitals(victim);
            }
            // FATALITY
            else
            {
                // Award NPC drop experience
                LevelingSystem.GiveExperience(this, victim.Data.Experience);

                // Reseta os dados do Npc 
                victim.Died();
            }
        }
        else
            // Demonstra o ataque aos outros jogadores
            PlayerSender.PlayerAttack(this);
    }

    public HotbarSlot FindHotbar(SlotType type, short slot) => Hotbar.First(x => x.Type == type && x.Slot == slot);

    public HotbarSlot FindHotbar(SlotType type, ItemSlot slot) =>
        Hotbar.First(x => x.Type == type && Inventory[x.Slot] == slot);

    public ItemSlot FindInventory(Item item) => Inventory.First(x => x.Item == item);

    public byte TotalInventoryFree => (byte)Inventory.Count(x => x.Item != null);

    public byte TotalTradeItems => (byte)TradeOffer.Count(x => x.SlotNum != 0);

    public static Player Find(string name) =>
        Account.List.Find(x => x.IsPlaying && x.Character.Name.Equals(name))?.Character;
}