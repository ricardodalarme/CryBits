using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using CryBits.Client.Utils;
using CryBits.Client.Entities;
using CryBits.Client.Entities.TempMap;
using CryBits.Client.Framework;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Library;
using CryBits.Client.Logic;
using CryBits.Client.UI;
using CryBits.Client.UI.Events;
using CryBits.Entities;
using CryBits.Entities.Map;
using CryBits.Entities.Npc;
using CryBits.Entities.Shop;
using CryBits.Entities.Slots;
using CryBits.Enums;
using CryBits.Extensions;
using LiteNetLib;
using LiteNetLib.Utils;
using static CryBits.Globals;
using static CryBits.Utils;
using Attribute = CryBits.Enums.Attribute;
using Screen = CryBits.Client.Framework.Interfacily.Components.Screen;

namespace CryBits.Client.Network;

internal static class Receive
{
    public static void Handle(NetPacketReader data)
    {
        // Manuseia os dados recebidos
        switch ((ServerPacket)data.GetByte())
        {
            case ServerPacket.Alert: Alert(data); break;
            case ServerPacket.Connect: Connect(); break;
            case ServerPacket.Join: Join(data); break;
            case ServerPacket.CreateCharacter: CreateCharacter(); break;
            case ServerPacket.JoinGame: JoinGame(); break;
            case ServerPacket.Classes: Classes(data); break;
            case ServerPacket.Characters: Characters(data); break;
            case ServerPacket.PlayerData: PlayerData(data); break;
            case ServerPacket.PlayerPosition: PlayerPosition(data); break;
            case ServerPacket.PlayerVitals: PlayerVitals(data); break;
            case ServerPacket.PlayerMove: PlayerMove(data); break;
            case ServerPacket.PlayerLeave: PlayerLeave(data); break;
            case ServerPacket.PlayerDirection: PlayerDirection(data); break;
            case ServerPacket.PlayerAttack: PlayerAttack(data); break;
            case ServerPacket.PlayerExperience: PlayerExperience(data); break;
            case ServerPacket.PlayerInventory: PlayerInventory(data); break;
            case ServerPacket.PlayerEquipments: PlayerEquipments(data); break;
            case ServerPacket.PlayerHotbar: PlayerHotbar(data); break;
            case ServerPacket.MapRevision: MapRevision(data); break;
            case ServerPacket.Map: Map(data); break;
            case ServerPacket.JoinMap: JoinMap(); break;
            case ServerPacket.Latency: Latency(); break;
            case ServerPacket.Message: Message(data); break;
            case ServerPacket.Npcs: Npcs(data); break;
            case ServerPacket.MapNpcs: MapNpcs(data); break;
            case ServerPacket.MapNpc: MapNpc(data); break;
            case ServerPacket.MapNpcMovement: MapNpcMovement(data); break;
            case ServerPacket.MapNpcDirection: MapNpcDirection(data); break;
            case ServerPacket.MapNpcVitals: MapNpcVitals(data); break;
            case ServerPacket.MapNpcAttack: MapNpcAttack(data); break;
            case ServerPacket.MapNpcDied: MapNpcDied(data); break;
            case ServerPacket.Items: Items(data); break;
            case ServerPacket.MapItems: MapItems(data); break;
            case ServerPacket.Party: Party(data); break;
            case ServerPacket.PartyInvitation: PartyInvitation(data); break;
            case ServerPacket.Trade: Trade(data); break;
            case ServerPacket.TradeInvitation: TradeInvitation(data); break;
            case ServerPacket.TradeState: TradeState(data); break;
            case ServerPacket.TradeOffer: TradeOffer(data); break;
            case ServerPacket.Shops: Shops(data); break;
            case ServerPacket.ShopOpen: ShopOpen(data); break;
        }
    }

    private static void Alert(NetDataReader data)
    {
        // Mostra a mensagem
        Utils.Alert.Show(data.GetString());
    }

    private static void Connect()
    {
        // Reseta os valores
        PanelsEvents.SelectCharacter = 0;
        Class.List = new Dictionary<Guid, Class>();

        // Abre o painel de seleção de personagens
        PanelsEvents.MenuClose();
        Panels.SelectCharacter.Visible = true;
    }

    private static void Join(NetDataReader data)
    {
        // Reseta alguns valores
        Player.List = new List<Player>();
        Item.List = new Dictionary<Guid, Item>();
        Shop.List = new Dictionary<Guid, Shop>();
        Npc.List = new Dictionary<Guid, Npc>();
        CryBits.Entities.Map.Map.List = new Dictionary<Guid, Map>();
        TempMap.List = new Dictionary<Guid, TempMap>();

        // Definir os valores que são enviados do servidor
        Player.Me = new Me(data.GetString());
        Player.List.Add(Player.Me);
    }

    private static void CreateCharacter()
    {
        // Reseta os valores
        TextBoxes.CreateCharacterName.Text = string.Empty;
        CheckBoxes.GenderMale.Checked = true;
        CheckBoxes.GenderFemale.Checked = false;
        PanelsEvents.CreateCharacterClass = 0;
        PanelsEvents.CreateCharacterTex = 0;

        // Abre o painel de criação de personagem
        PanelsEvents.MenuClose();
        Panels.CreateCharacter.Visible = true;
    }

    private static void Classes(NetDataReader data)
    {
        // Recebe os dados
        Class.List = (Dictionary<Guid, Class>)data.ReadObject();
    }

    private static void Characters(NetDataReader data)
    {
        // Redimensiona a lista
        PanelsEvents.Characters = new PanelsEvents.TempCharacter[data.GetByte()];

        for (byte i = 0; i < PanelsEvents.Characters.Length; i++)
        {
            // Recebe os dados do personagem
            PanelsEvents.Characters[i] = new PanelsEvents.TempCharacter
            {
                Name = data.GetString(),
                TextureNum = data.GetShort()
            };
        }
    }

    private static void JoinGame()
    {
        // Reseta os valores
        Chat.Order = new List<Chat.Structure>();
        Chat.LinesFirst = 0;
        Loop.ChatTimer = Environment.TickCount + Chat.SleepTimer;
        TextBoxes.Chat.Text = string.Empty;
        CheckBoxes.OptionsSounds.Checked = Options.Sounds;
        CheckBoxes.OptionsMusics.Checked = Options.Musics;
        CheckBoxes.OptionsChat.Checked = Options.Chat;
        CheckBoxes.OptionsFps.Checked = Options.Fps;
        CheckBoxes.OptionsLatency.Checked = Options.Latency;
        CheckBoxes.OptionsTrade.Checked = Options.Trade;
        CheckBoxes.OptionsParty.Checked = Options.Party;
        Loop.ChatTimer = Loop.ChatTimer = Environment.TickCount + 10000;
        PanelsEvents.InformationId = Guid.Empty;

        // Reseta a interface
        Panels.MenuCharacter.Visible = false;
        Panels.MenuInventory.Visible = false;
        Panels.MenuOptions.Visible = false;
        Panels.Chat.Visible = false;
        Panels.Drop.Visible = false;
        Panels.PartyInvitation.Visible = false;
        Panels.Trade.Visible = false;
        Buttons.TradeOfferConfirm.Visible = true;
        Buttons.TradeOfferAccept.Visible = Buttons.TradeOfferDecline.Visible = false;
        Panels.TradeOfferDisable.Visible = false;
        Panels.Shop.Visible = false;
        Panels.ShopSell.Visible = false;

        // Abre o jogo
        Music.Stop();
        Screen.Current = Screens.Game;
    }

    private static void PlayerData(NetDataReader data)
    {
        var name = data.GetString();
        Player player;

        // Adiciona o jogador à lista
        if (name != Player.Me.Name)
        {
            player = new Player(name);
            Player.List.Add(player);
        }
        else
            player = Player.Me;

        // Defini os dados do jogador
        player.TextureNum = data.GetShort();
        player.Level = data.GetShort();
        player.Map = TempMap.List[data.GetGuid()];
        player.X = data.GetByte();
        player.Y = data.GetByte();
        player.Direction = (Direction)data.GetByte();
        for (byte n = 0; n < (byte)Vital.Count; n++)
        {
            player.Vital[n] = data.GetShort();
            player.MaxVital[n] = data.GetShort();
        }
        for (byte n = 0; n < (byte)Attribute.Count; n++) player.Attribute[n] = data.GetShort();
        for (byte n = 0; n < (byte)Equipment.Count; n++) player.Equipment[n] = Item.List.Get(data.GetGuid());
        TempMap.Current = player.Map;
    }

    private static void PlayerPosition(NetDataReader data)
    {
        var player = Player.Get(data.GetString());

        // Defini os dados do jogador
        player.X = data.GetByte();
        player.Y = data.GetByte();
        player.Direction = (Direction)data.GetByte();

        // Para a movimentação
        player.X2 = 0;
        player.Y2 = 0;
        player.Movement = Movement.Stopped;
    }

    private static void PlayerVitals(NetDataReader data)
    {
        var player = Player.Get(data.GetString());

        // Define os dados
        for (byte i = 0; i < (byte)Vital.Count; i++)
        {
            player.Vital[i] = data.GetShort();
            player.MaxVital[i] = data.GetShort();
        }
    }

    private static void PlayerEquipments(NetDataReader data)
    {
        var player = Player.Get(data.GetString());

        // Altera os dados dos equipamentos do jogador
        for (byte i = 0; i < (byte)Equipment.Count; i++) player.Equipment[i] = Item.List.Get(data.GetGuid());
    }

    private static void PlayerLeave(NetDataReader data)
    {
        // Limpa os dados do jogador
        Player.List.Remove(Player.Get(data.GetString()));
    }

    private static void PlayerMove(NetDataReader data)
    {
        var player = Player.Get(data.GetString());

        // Move o jogador
        player.X = data.GetByte();
        player.Y = data.GetByte();
        player.Direction = (Direction)data.GetByte();
        player.Movement = (Movement)data.GetByte();
        player.X2 = 0;
        player.Y2 = 0;

        // Posição exata do jogador
        switch (player.Direction)
        {
            case Direction.Up: player.Y2 = Grid; break;
            case Direction.Down: player.Y2 = Grid * -1; break;
            case Direction.Right: player.X2 = Grid * -1; break;
            case Direction.Left: player.X2 = Grid; break;
        }
    }

    private static void PlayerDirection(NetDataReader data)
    {
        // Altera a posição do jogador
        Player.Get(data.GetString()).Direction = (Direction)data.GetByte();
    }

    private static void PlayerAttack(NetDataReader data)
    {
        var player = Player.Get(data.GetString());
        var victim = data.GetString();
        var victimType = data.GetByte();

        // Inicia o ataque
        player.Attacking = true;
        player.AttackTimer = Environment.TickCount;

        // Sofrendo dano
        if (victim != string.Empty)
            if (victimType == (byte)Target.Player)
            {
                var victimData = Player.Get(victim);
                victimData.Hurt = Environment.TickCount;
                TempMap.Current.Blood.Add(new TempMapBlood((byte)MyRandom.Next(0, 3), victimData.X, victimData.Y, 255));
            }
            else if (victimType == (byte)Target.Npc)
            {
                TempMap.Current.Npc[byte.Parse(victim)].Hurt = Environment.TickCount;
                TempMap.Current.Blood.Add(new TempMapBlood((byte)MyRandom.Next(0, 3), TempMap.Current.Npc[byte.Parse(victim)].X, TempMap.Current.Npc[byte.Parse(victim)].Y, 255));
            }
    }

    private static void PlayerExperience(NetDataReader data)
    {
        // Define os dados
        Player.Me.Experience = data.GetInt();
        Player.Me.ExpNeeded = data.GetInt();
        Player.Me.Points = data.GetByte();

        // Manipula a visibilidade dos botões
        Buttons.AttributesStrength.Visible = Player.Me.Points > 0;
        Buttons.AttributesResistance.Visible = Player.Me.Points > 0;
        Buttons.AttributesIntelligence.Visible = Player.Me.Points > 0;
        Buttons.AttributesAgility.Visible = Player.Me.Points > 0;
        Buttons.AttributesVitality.Visible = Player.Me.Points > 0;
    }

    private static void PlayerInventory(NetDataReader data)
    {
        // Define os dados
        for (byte i = 0; i < MaxInventory; i++)
            Player.Me.Inventory[i] = new ItemSlot(Item.List.Get(data.GetGuid()), data.GetShort());
    }

    private static void PlayerHotbar(NetDataReader data)
    {
        // Define os dados
        for (byte i = 0; i < MaxHotbar; i++)
        {
            Player.Me.Hotbar[i] = new HotbarSlot((SlotType)data.GetByte(), data.GetByte());
        }
    }

    private static void MapRevision(NetDataReader data)
    {
        var needed = false;
        var id = data.GetGuid();
        var currentRevision = data.GetShort();

        // Limpa todos os outros jogadores
        for (byte i = 0; i < Player.List.Count; i++)
            if (Player.List[i] != Player.Me)
                Player.List.RemoveAt(i);

        // Verifica se é necessário baixar os dados do mapa
        if (File.Exists(Directories.MapsData.FullName + id + Directories.Format) || CryBits.Entities.Map.Map.List.ContainsKey(id))
        {
            if (!CryBits.Entities.Map.Map.List.ContainsKey(id))
            {
                Read.Map(id);
                TempMap.Current.Weather.Update();
                TempMap.Current.Data.Update();
            }

            if (CryBits.Entities.Map.Map.List[id].Revision != currentRevision)
                needed = true;
        }
        else
            needed = true;

        // Solicita os dados do mapa
        Send.RequestMap(needed);

        // Reseta os sangues do mapa
        TempMap.Current.Blood = new List<TempMapBlood>();
    }

    private static void Map(NetDataReader data)
    {
        var map = (Map)data.ReadObject();
        var id = map.Id;

        // Obtém o dado
        if (CryBits.Entities.Map.Map.List.ContainsKey(id)) CryBits.Entities.Map.Map.List[id] = map;
        else
        {
            CryBits.Entities.Map.Map.List.Add(id, map);
            TempMap.List.Add(id, new TempMap(map));
        }

        TempMap.Current = TempMap.List[id];

        // Salva o mapa
        Write.Map(map);

        // Redimensiona as partículas do clima
        TempMap.Current.Weather.UpdateType();
        TempMap.Current.Data.Update();
    }

    private static void JoinMap()
    {
        // Se tiver, reproduz a música de fundo do mapa
        if (string.IsNullOrEmpty(TempMap.Current.Data.Music))
            Music.Stop();
        else
            Music.Play(TempMap.Current.Data.Music);
    }

    private static void Latency()
    {
        // Define a latência
        Socket.Latency = Environment.TickCount - Socket.LatencySend;
    }

    private static void Message(NetDataReader data)
    {
        // Adiciona a mensagem
        var text = data.GetString();
        var color = Color.FromArgb(data.GetInt());
        Chat.AddText(text, new SFML.Graphics.Color(color.R, color.G, color.B));
    }

    private static void Items(NetDataReader data)
    {
        // Recebe os dados
        Item.List = (Dictionary<Guid, Item>)data.ReadObject();
    }

    private static void MapItems(NetDataReader data)
    {
        // Quantidade
        TempMap.Current.Item = new TempMapItems[data.GetByte()];

        // Lê os dados de todos
        for (byte i = 0; i < TempMap.Current.Item.Length; i++)
            TempMap.Current.Item[i] = new TempMapItems
            {
                Item = Item.List.Get(data.GetGuid()),
                X = data.GetByte(),
                Y = data.GetByte()
            };
    }

    private static void Party(NetDataReader data)
    {
        // Lê os dados do grupo
        Player.Me.Party = new Player[data.GetByte()];
        for (byte i = 0; i < Player.Me.Party.Length; i++) Player.Me.Party[i] = Player.Get(data.GetString());
    }

    private static void PartyInvitation(NetDataReader data)
    {
        // Nega o pedido caso o jogador não quiser receber convites
        if (!Options.Party)
        {
            Send.PartyDecline();
            return;
        }

        // Abre a janela de convite para o grupo
        PanelsEvents.PartyInvitation = data.GetString();
        Panels.PartyInvitation.Visible = true;
    }

    private static void Trade(NetDataReader data)
    {
        var state = data.GetBool();

        // Visibilidade do painel
        Panels.Trade.Visible = data.GetBool();

        if (state)
        {
            // Reseta os botões
            Buttons.TradeOfferConfirm.Visible = true;
            Panels.TradeAmount.Visible = Buttons.TradeOfferAccept.Visible = Buttons.TradeOfferDecline.Visible = false;
            Panels.TradeOfferDisable.Visible = false;

            // Limpa os dados
            Player.Me.TradeOffer = new ItemSlot[MaxInventory];
            Player.Me.TradeTheirOffer = new ItemSlot[MaxInventory];
        }
        else
        {
            // Limpa os dados
            Player.Me.TradeOffer = null;
            Player.Me.TradeTheirOffer = null;
        }
    }

    private static void TradeInvitation(NetDataReader data)
    {
        // Nega o pedido caso o jogador não quiser receber convites
        if (!Options.Trade)
        {
            Send.TradeDecline();
            return;
        }

        // Abre a janela de convite para o grupo
        PanelsEvents.TradeInvitation = data.GetString();
        Panels.TradeInvitation.Visible = true;
    }

    private static void TradeState(NetDataReader data)
    {
        switch ((TradeStatus)data.GetByte())
        {
            case TradeStatus.Accepted:
            case TradeStatus.Declined:
                Buttons.TradeOfferConfirm.Visible = true;
                Buttons.TradeOfferAccept.Visible = Buttons.TradeOfferDecline.Visible = false;
                Panels.TradeOfferDisable.Visible = false;
                break;
            case TradeStatus.Confirmed:
                Buttons.TradeOfferConfirm.Visible = false;
                Buttons.TradeOfferAccept.Visible = Buttons.TradeOfferDecline.Visible = true;
                Panels.TradeOfferDisable.Visible = false;
                break;
        }
    }

    private static void TradeOffer(NetDataReader data)
    {
        // Recebe os dados da oferta
        if (data.GetBool())
            for (byte i = 0; i < MaxInventory; i++)
            {
                Player.Me.TradeOffer[i].Item = Item.List.Get(data.GetGuid());
                Player.Me.TradeOffer[i].Amount = data.GetShort();
            }
        else
            for (byte i = 0; i < MaxInventory; i++)
            {
                Player.Me.TradeTheirOffer[i].Item = Item.List.Get(data.GetGuid());
                Player.Me.TradeTheirOffer[i].Amount = data.GetShort();
            }
    }

    private static void Shops(NetDataReader data)
    {
        // Recebe os dados
        Shop.List = (Dictionary<Guid, Shop>)data.ReadObject();
    }

    private static void ShopOpen(NetDataReader data)
    {
        // Abre a loja
        PanelsEvents.ShopOpen = Shop.List.Get(data.GetGuid());
        Panels.Shop.Visible = PanelsEvents.ShopOpen != null;
    }

    private static void Npcs(NetDataReader data)
    {
        // Recebe os dados
        Npc.List = (Dictionary<Guid, Npc>)data.ReadObject();
    }

    private static void MapNpcs(NetDataReader data)
    {
        // Lê os dados
        TempMap.Current.Npc = new TempNpc[data.GetShort()];
        for (byte i = 0; i < TempMap.Current.Npc.Length; i++)
        {
            TempMap.Current.Npc[i] = new TempNpc();
            TempMap.Current.Npc[i].X2 = 0;
            TempMap.Current.Npc[i].Y2 = 0;
            TempMap.Current.Npc[i].Data = Npc.List.Get(data.GetGuid());
            TempMap.Current.Npc[i].X = data.GetByte();
            TempMap.Current.Npc[i].Y = data.GetByte();
            TempMap.Current.Npc[i].Direction = (Direction)data.GetByte();

            // Vitais
            for (byte n = 0; n < (byte)Vital.Count; n++)
                TempMap.Current.Npc[i].Vital[n] = data.GetShort();
        }
    }

    private static void MapNpc(NetDataReader data)
    {
        // Lê os dados
        var i = data.GetByte();
        TempMap.Current.Npc[i].X2 = 0;
        TempMap.Current.Npc[i].Y2 = 0;
        TempMap.Current.Npc[i].Data = Npc.List.Get(data.GetGuid());
        TempMap.Current.Npc[i].X = data.GetByte();
        TempMap.Current.Npc[i].Y = data.GetByte();
        TempMap.Current.Npc[i].Direction = (Direction)data.GetByte();
        TempMap.Current.Npc[i].Vital = new short[(byte)Vital.Count];
        for (byte n = 0; n < (byte)Vital.Count; n++) TempMap.Current.Npc[i].Vital[n] = data.GetShort();
    }

    private static void MapNpcMovement(NetDataReader data)
    {
        // Lê os dados
        var i = data.GetByte();
        byte x = TempMap.Current.Npc[i].X, y = TempMap.Current.Npc[i].Y;
        TempMap.Current.Npc[i].X2 = 0;
        TempMap.Current.Npc[i].Y2 = 0;
        TempMap.Current.Npc[i].X = data.GetByte();
        TempMap.Current.Npc[i].Y = data.GetByte();
        TempMap.Current.Npc[i].Direction = (Direction)data.GetByte();
        TempMap.Current.Npc[i].Movement = (Movement)data.GetByte();

        // Posição exata do jogador
        if (x != TempMap.Current.Npc[i].X || y != TempMap.Current.Npc[i].Y)
            switch (TempMap.Current.Npc[i].Direction)
            {
                case Direction.Up: TempMap.Current.Npc[i].Y2 = Grid; break;
                case Direction.Down: TempMap.Current.Npc[i].Y2 = Grid * -1; break;
                case Direction.Right: TempMap.Current.Npc[i].X2 = Grid * -1; break;
                case Direction.Left: TempMap.Current.Npc[i].X2 = Grid; break;
            }
    }

    private static void MapNpcAttack(NetDataReader data)
    {
        var index = data.GetByte();
        var victim = data.GetString();
        var victimType = data.GetByte();

        // Inicia o ataque
        TempMap.Current.Npc[index].Attacking = true;
        TempMap.Current.Npc[index].AttackTimer = Environment.TickCount;

        // Sofrendo dano
        if (victim != string.Empty)
            if (victimType == (byte)Target.Player)
            {
                var victimData = Player.Get(victim);
                victimData.Hurt = Environment.TickCount;
                TempMap.Current.Blood.Add(new TempMapBlood((byte)MyRandom.Next(0, 3), victimData.X, victimData.Y, 255));
            }
            else if (victimType == (byte)Target.Npc)
            {
                TempMap.Current.Npc[byte.Parse(victim)].Hurt = Environment.TickCount;
                TempMap.Current.Blood.Add(new TempMapBlood((byte)MyRandom.Next(0, 3), TempMap.Current.Npc[byte.Parse(victim)].X, TempMap.Current.Npc[byte.Parse(victim)].Y, 255));
            }
    }

    private static void MapNpcDirection(NetDataReader data)
    {
        // Define a direção de determinado Npc
        var i = data.GetByte();
        TempMap.Current.Npc[i].Direction = (Direction)data.GetByte();
        TempMap.Current.Npc[i].X2 = 0;
        TempMap.Current.Npc[i].Y2 = 0;
    }

    private static void MapNpcVitals(NetDataReader data)
    {
        var index = data.GetByte();

        // Define os vitais de determinado Npc
        for (byte n = 0; n < (byte)Vital.Count; n++)
            TempMap.Current.Npc[index].Vital[n] = data.GetShort();
    }

    private static void MapNpcDied(NetDataReader data)
    {
        var i = data.GetByte();

        // Limpa os dados do Npc
        TempMap.Current.Npc[i].X2 = 0;
        TempMap.Current.Npc[i].Y2 = 0;
        TempMap.Current.Npc[i].Data = null;
        TempMap.Current.Npc[i].X = 0;
        TempMap.Current.Npc[i].Y = 0;
        TempMap.Current.Npc[i].Vital = new short[(byte)Vital.Count];
    }
}
