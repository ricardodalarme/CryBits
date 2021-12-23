using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using CryBits.Client.Entities;
using CryBits.Client.Library;
using CryBits.Client.Logic;
using CryBits.Client.UI;
using CryBits.Entities;
using CryBits.Enums;
using Lidgren.Network;
using static CryBits.Globals;
using static CryBits.Utils;
using Attribute = CryBits.Enums.Attribute;
using Music = CryBits.Client.Media.Audio.Music;

namespace CryBits.Client.Network;

internal static class Receive
{
    public static void Handle(NetIncomingMessage data)
    {
        // Manuseia os dados recebidos
        switch ((ServerPacket)data.ReadByte())
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

    private static void Alert(NetBuffer data)
    {
        // Mostra a mensagem
        MessageBox.Show(data.ReadString());
    }

    private static void Connect()
    {
        // Reseta os valores
        Panels.SelectCharacter = 0;
        Class.List = new Dictionary<Guid, Class>();

        // Abre o painel de seleção de personagens
        Panels.MenuClose();
        Panels.List["SelectCharacter"].Visible = true;
    }

    private static void Join(NetBuffer data)
    {
        // Reseta alguns valores
        Player.List = new List<Player>();
        Item.List = new Dictionary<Guid, Item>();
        Shop.List = new Dictionary<Guid, Shop>();
        Npc.List = new Dictionary<Guid, Npc>();
        CryBits.Entities.Map.List = new Dictionary<Guid, Map>();
        TempMap.List = new Dictionary<Guid, TempMap>();

        // Definir os valores que são enviados do servidor
        Player.Me = new Me(data.ReadString());
        Player.List.Add(Player.Me);
    }

    private static void CreateCharacter()
    {
        // Reseta os valores
        TextBoxes.List["CreateCharacter_Name"].Text = string.Empty;
        CheckBoxes.List["GenderMale"].Checked = true;
        CheckBoxes.List["GenderFemale"].Checked = false;
        Panels.CreateCharacterClass = 0;
        Panels.CreateCharacterTex = 0;

        // Abre o painel de criação de personagem
        Panels.MenuClose();
        Panels.List["CreateCharacter"].Visible = true;
    }

    private static void Classes(NetBuffer data)
    {
        // Recebe os dados
        Class.List = (Dictionary<Guid, Class>)ByteArrayToObject(data);
    }

    private static void Characters(NetBuffer data)
    {
        // Redimensiona a lista
        Panels.Characters = new Panels.TempCharacter[data.ReadByte()];

        for (byte i = 0; i < Panels.Characters.Length; i++)
        {
            // Recebe os dados do personagem
            Panels.Characters[i] = new Panels.TempCharacter
            {
                Name = data.ReadString(),
                TextureNum = data.ReadInt16()
            };
        }
    }

    private static void JoinGame()
    {
        // Reseta os valores
        Chat.Order = new List<Chat.Structure>();
        Chat.LinesFirst = 0;
        Loop.ChatTimer = Environment.TickCount + Chat.SleepTimer;
        TextBoxes.List["Chat"].Text = string.Empty;
        CheckBoxes.List["Options_Sounds"].Checked = Options.Sounds;
        CheckBoxes.List["Options_Musics"].Checked = Options.Musics;
        CheckBoxes.List["Options_Chat"].Checked = Options.Chat;
        CheckBoxes.List["Options_FPS"].Checked = Options.Fps;
        CheckBoxes.List["Options_Latency"].Checked = Options.Latency;
        CheckBoxes.List["Options_Trade"].Checked = Options.Trade;
        CheckBoxes.List["Options_Party"].Checked = Options.Party;
        Loop.ChatTimer = Loop.ChatTimer = Environment.TickCount + 10000;
        Panels.InformationId = Guid.Empty;

        // Reseta a interface
        Panels.List["Menu_Character"].Visible = false;
        Panels.List["Menu_Inventory"].Visible = false;
        Panels.List["Menu_Options"].Visible = false;
        Panels.List["Chat"].Visible = false;
        Panels.List["Drop"].Visible = false;
        Panels.List["Party_Invitation"].Visible = false;
        Panels.List["Trade"].Visible = false;
        Buttons.List["Trade_Offer_Confirm"].Visible = true;
        Buttons.List["Trade_Offer_Accept"].Visible = Buttons.List["Trade_Offer_Decline"].Visible = false;
        Panels.List["Trade_Offer_Disable"].Visible = false;
        Panels.List["Shop"].Visible = false;
        Panels.List["Shop_Sell"].Visible = false;

        // Abre o jogo
        Music.Stop();
        Windows.Current = Window.Game;
    }

    private static void PlayerData(NetBuffer data)
    {
        var name = data.ReadString();
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
        player.TextureNum = data.ReadInt16();
        player.Level = data.ReadInt16();
        player.Map = TempMap.List[new Guid(data.ReadString())];
        player.X = data.ReadByte();
        player.Y = data.ReadByte();
        player.Direction = (Direction)data.ReadByte();
        for (byte n = 0; n < (byte)Vital.Count; n++)
        {
            player.Vital[n] = data.ReadInt16();
            player.MaxVital[n] = data.ReadInt16();
        }
        for (byte n = 0; n < (byte)Attribute.Count; n++) player.Attribute[n] = data.ReadInt16();
        for (byte n = 0; n < (byte)Equipment.Count; n++) player.Equipment[n] = Item.List.Get(new Guid(data.ReadString()));
        TempMap.Current = player.Map;
    }

    private static void PlayerPosition(NetBuffer data)
    {
        var player = Player.Get(data.ReadString());

        // Defini os dados do jogador
        player.X = data.ReadByte();
        player.Y = data.ReadByte();
        player.Direction = (Direction)data.ReadByte();

        // Para a movimentação
        player.X2 = 0;
        player.Y2 = 0;
        player.Movement = Movement.Stopped;
    }

    private static void PlayerVitals(NetBuffer data)
    {
        var player = Player.Get(data.ReadString());

        // Define os dados
        for (byte i = 0; i < (byte)Vital.Count; i++)
        {
            player.Vital[i] = data.ReadInt16();
            player.MaxVital[i] = data.ReadInt16();
        }
    }

    private static void PlayerEquipments(NetBuffer data)
    {
        var player = Player.Get(data.ReadString());

        // Altera os dados dos equipamentos do jogador
        for (byte i = 0; i < (byte)Equipment.Count; i++) player.Equipment[i] = Item.List.Get(new Guid(data.ReadString()));
    }

    private static void PlayerLeave(NetBuffer data)
    {
        // Limpa os dados do jogador
        Player.List.Remove(Player.Get(data.ReadString()));
    }

    private static void PlayerMove(NetBuffer data)
    {
        var player = Player.Get(data.ReadString());

        // Move o jogador
        player.X = data.ReadByte();
        player.Y = data.ReadByte();
        player.Direction = (Direction)data.ReadByte();
        player.Movement = (Movement)data.ReadByte();
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

    private static void PlayerDirection(NetBuffer data)
    {
        // Altera a posição do jogador
        Player.Get(data.ReadString()).Direction = (Direction)data.ReadByte();
    }

    private static void PlayerAttack(NetBuffer data)
    {
        var player = Player.Get(data.ReadString());
        var victim = data.ReadString();
        var victimType = data.ReadByte();

        // Inicia o ataque
        player.Attacking = true;
        player.AttackTimer = Environment.TickCount;

        // Sofrendo dano
        if (victim != string.Empty)
            if (victimType == (byte)Target.Player)
            {
                var victimData = Player.Get(victim);
                victimData.Hurt = Environment.TickCount;
                TempMap.Current.Blood.Add(new MapBlood((byte)MyRandom.Next(0, 3), victimData.X, victimData.Y, 255));
            }
            else if (victimType == (byte)Target.Npc)
            {
                TempMap.Current.Npc[byte.Parse(victim)].Hurt = Environment.TickCount;
                TempMap.Current.Blood.Add(new MapBlood((byte)MyRandom.Next(0, 3), TempMap.Current.Npc[byte.Parse(victim)].X, TempMap.Current.Npc[byte.Parse(victim)].Y, 255));
            }
    }

    private static void PlayerExperience(NetBuffer data)
    {
        // Define os dados
        Player.Me.Experience = data.ReadInt32();
        Player.Me.ExpNeeded = data.ReadInt32();
        Player.Me.Points = data.ReadByte();

        // Manipula a visibilidade dos botões
        Buttons.List["Attributes_Strength"].Visible = Player.Me.Points > 0;
        Buttons.List["Attributes_Resistance"].Visible = Player.Me.Points > 0;
        Buttons.List["Attributes_Intelligence"].Visible = Player.Me.Points > 0;
        Buttons.List["Attributes_Agility"].Visible = Player.Me.Points > 0;
        Buttons.List["Attributes_Vitality"].Visible = Player.Me.Points > 0;
    }

    private static void PlayerInventory(NetBuffer data)
    {
        // Define os dados
        for (byte i = 0; i < MaxInventory; i++)
            Player.Me.Inventory[i] = new ItemSlot(Item.List.Get(new Guid(data.ReadString())), data.ReadInt16());
    }

    private static void PlayerHotbar(NetBuffer data)
    {
        // Define os dados
        for (byte i = 0; i < MaxHotbar; i++)
        {
            Player.Me.Hotbar[i].Type = (SlotType)data.ReadByte();
            Player.Me.Hotbar[i].Slot = data.ReadByte();
        }
    }

    private static void MapRevision(NetBuffer data)
    {
        var needed = false;
        var id = new Guid(data.ReadString());
        var currentRevision = data.ReadInt16();

        // Limpa todos os outros jogadores
        for (byte i = 0; i < Player.List.Count; i++)
            if (Player.List[i] != Player.Me)
                Player.List.RemoveAt(i);

        // Verifica se é necessário baixar os dados do mapa
        if (File.Exists(Directories.MapsData.FullName + id + Directories.Format) || CryBits.Entities.Map.List.ContainsKey(id))
        {
            if (!CryBits.Entities.Map.List.ContainsKey(id)) Read.Map(id);

            if (CryBits.Entities.Map.List[id].Revision != currentRevision)
                needed = true;
        }
        else
            needed = true;

        // Solicita os dados do mapa
        Send.RequestMap(needed);

        // Reseta os sangues do mapa
        TempMap.Current.Blood = new List<MapBlood>();
    }

    private static void Map(NetBuffer data)
    {
        var map = (Map)ByteArrayToObject(data);
        var id = map.Id;

        // Obtém o dado
        if (CryBits.Entities.Map.List.ContainsKey(id)) CryBits.Entities.Map.List[id] = map;
        else
        {
            CryBits.Entities.Map.List.Add(id, map);
            TempMap.List.Add(id, new TempMap(map));
        }

        TempMap.Current = TempMap.List[id];

        // Salva o mapa
        Write.Map(map);

        // Redimensiona as partículas do clima
        TempMap.Current.UpdateWeatherType();
        TempMap.Current.Data.Update();
    }

    private static void JoinMap()
    {
        // Se tiver, reproduz a música de fundo do mapa
        if (TempMap.Current.Data.Music > 0)
            Music.Play((Enums.Music)TempMap.Current.Data.Music);
        else
            Music.Stop();
    }

    private static void Latency()
    {
        // Define a latência
        Socket.Latency = Environment.TickCount - Socket.LatencySend;
    }

    private static void Message(NetBuffer data)
    {
        // Adiciona a mensagem
        var text = data.ReadString();
        var color = Color.FromArgb(data.ReadInt32());
        Chat.AddText(text, new SFML.Graphics.Color(color.R, color.G, color.B));
    }

    private static void Items(NetBuffer data)
    {
        // Recebe os dados
        Item.List = (Dictionary<Guid, Item>)ByteArrayToObject(data);
    }

    private static void MapItems(NetBuffer data)
    {
        // Quantidade
        TempMap.Current.Item = new MapItems[data.ReadByte()];

        // Lê os dados de todos
        for (byte i = 0; i < TempMap.Current.Item.Length; i++)
            TempMap.Current.Item[i] = new MapItems
            {
                Item = Item.List.Get(new Guid(data.ReadString())),
                X = data.ReadByte(),
                Y = data.ReadByte()
            };
    }

    private static void Party(NetBuffer data)
    {
        // Lê os dados do grupo
        Player.Me.Party = new Player[data.ReadByte()];
        for (byte i = 0; i < Player.Me.Party.Length; i++) Player.Me.Party[i] = Player.Get(data.ReadString());
    }

    private static void PartyInvitation(NetBuffer data)
    {
        // Nega o pedido caso o jogador não quiser receber convites
        if (!Options.Party)
        {
            Send.PartyDecline();
            return;
        }

        // Abre a janela de convite para o grupo
        Panels.PartyInvitation = data.ReadString();
        Panels.List["Party_Invitation"].Visible = true;
    }

    private static void Trade(NetBuffer data)
    {
        var state = data.ReadBoolean();

        // Visibilidade do painel
        Panels.List["Trade"].Visible = data.ReadBoolean();

        if (state)
        {
            // Reseta os botões
            Buttons.List["Trade_Offer_Confirm"].Visible = true;
            Panels.List["Trade_Amount"].Visible = Buttons.List["Trade_Offer_Accept"].Visible = Buttons.List["Trade_Offer_Decline"].Visible = false;
            Panels.List["Trade_Offer_Disable"].Visible = false;

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

    private static void TradeInvitation(NetBuffer data)
    {
        // Nega o pedido caso o jogador não quiser receber convites
        if (!Options.Trade)
        {
            Send.TradeDecline();
            return;
        }

        // Abre a janela de convite para o grupo
        Panels.TradeInvitation = data.ReadString();
        Panels.List["Trade_Invitation"].Visible = true;
    }

    private static void TradeState(NetBuffer data)
    {
        switch ((TradeStatus)data.ReadByte())
        {
            case TradeStatus.Accepted:
            case TradeStatus.Declined:
                Buttons.List["Trade_Offer_Confirm"].Visible = true;
                Buttons.List["Trade_Offer_Accept"].Visible = Buttons.List["Trade_Offer_Decline"].Visible = false;
                Panels.List["Trade_Offer_Disable"].Visible = false;
                break;
            case TradeStatus.Confirmed:
                Buttons.List["Trade_Offer_Confirm"].Visible = false;
                Buttons.List["Trade_Offer_Accept"].Visible = Buttons.List["Trade_Offer_Decline"].Visible = true;
                Panels.List["Trade_Offer_Disable"].Visible = false;
                break;
        }
    }

    private static void TradeOffer(NetBuffer data)
    {
        // Recebe os dados da oferta
        if (data.ReadBoolean())
            for (byte i = 0; i < MaxInventory; i++)
            {
                Player.Me.TradeOffer[i].Item = Item.List.Get(new Guid(data.ReadString()));
                Player.Me.TradeOffer[i].Amount = data.ReadInt16();
            }
        else
            for (byte i = 0; i < MaxInventory; i++)
            {
                Player.Me.TradeTheirOffer[i].Item = Item.List.Get(new Guid(data.ReadString()));
                Player.Me.TradeTheirOffer[i].Amount = data.ReadInt16();
            }
    }

    private static void Shops(NetBuffer data)
    {
        // Recebe os dados
        Shop.List = (Dictionary<Guid, Shop>)ByteArrayToObject(data);
    }

    private static void ShopOpen(NetBuffer data)
    {
        // Abre a loja
        Panels.ShopOpen = Shop.List.Get(new Guid(data.ReadString()));
        Panels.List["Shop"].Visible = Panels.ShopOpen != null;
    }

    private static void Npcs(NetBuffer data)
    {
        // Recebe os dados
        Npc.List = (Dictionary<Guid, Npc>)ByteArrayToObject(data);
    }

    private static void MapNpcs(NetBuffer data)
    {
        // Lê os dados
        TempMap.Current.Npc = new TempNpc[data.ReadInt16()];
        for (byte i = 0; i < TempMap.Current.Npc.Length; i++)
        {
            TempMap.Current.Npc[i] = new TempNpc();
            TempMap.Current.Npc[i].X2 = 0;
            TempMap.Current.Npc[i].Y2 = 0;
            TempMap.Current.Npc[i].Data = Npc.List.Get(new Guid(data.ReadString()));
            TempMap.Current.Npc[i].X = data.ReadByte();
            TempMap.Current.Npc[i].Y = data.ReadByte();
            TempMap.Current.Npc[i].Direction = (Direction)data.ReadByte();

            // Vitais
            for (byte n = 0; n < (byte)Vital.Count; n++)
                TempMap.Current.Npc[i].Vital[n] = data.ReadInt16();
        }
    }

    private static void MapNpc(NetBuffer data)
    {
        // Lê os dados
        var i = data.ReadByte();
        TempMap.Current.Npc[i].X2 = 0;
        TempMap.Current.Npc[i].Y2 = 0;
        TempMap.Current.Npc[i].Data = Npc.List.Get(new Guid(data.ReadString()));
        TempMap.Current.Npc[i].X = data.ReadByte();
        TempMap.Current.Npc[i].Y = data.ReadByte();
        TempMap.Current.Npc[i].Direction = (Direction)data.ReadByte();
        TempMap.Current.Npc[i].Vital = new short[(byte)Vital.Count];
        for (byte n = 0; n < (byte)Vital.Count; n++) TempMap.Current.Npc[i].Vital[n] = data.ReadInt16();
    }

    private static void MapNpcMovement(NetBuffer data)
    {
        // Lê os dados
        var i = data.ReadByte();
        byte x = TempMap.Current.Npc[i].X, y = TempMap.Current.Npc[i].Y;
        TempMap.Current.Npc[i].X2 = 0;
        TempMap.Current.Npc[i].Y2 = 0;
        TempMap.Current.Npc[i].X = data.ReadByte();
        TempMap.Current.Npc[i].Y = data.ReadByte();
        TempMap.Current.Npc[i].Direction = (Direction)data.ReadByte();
        TempMap.Current.Npc[i].Movement = (Movement)data.ReadByte();

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

    private static void MapNpcAttack(NetBuffer data)
    {
        var index = data.ReadByte();
        var victim = data.ReadString();
        var victimType = data.ReadByte();

        // Inicia o ataque
        TempMap.Current.Npc[index].Attacking = true;
        TempMap.Current.Npc[index].AttackTimer = Environment.TickCount;

        // Sofrendo dano
        if (victim != string.Empty)
            if (victimType == (byte)Target.Player)
            {
                var victimData = Player.Get(victim);
                victimData.Hurt = Environment.TickCount;
                TempMap.Current.Blood.Add(new MapBlood((byte)MyRandom.Next(0, 3), victimData.X, victimData.Y, 255));
            }
            else if (victimType == (byte)Target.Npc)
            {
                TempMap.Current.Npc[byte.Parse(victim)].Hurt = Environment.TickCount;
                TempMap.Current.Blood.Add(new MapBlood((byte)MyRandom.Next(0, 3), TempMap.Current.Npc[byte.Parse(victim)].X, TempMap.Current.Npc[byte.Parse(victim)].Y, 255));
            }
    }

    private static void MapNpcDirection(NetBuffer data)
    {
        // Define a direção de determinado Npc
        var i = data.ReadByte();
        TempMap.Current.Npc[i].Direction = (Direction)data.ReadByte();
        TempMap.Current.Npc[i].X2 = 0;
        TempMap.Current.Npc[i].Y2 = 0;
    }

    private static void MapNpcVitals(NetBuffer data)
    {
        var index = data.ReadByte();

        // Define os vitais de determinado Npc
        for (byte n = 0; n < (byte)Vital.Count; n++)
            TempMap.Current.Npc[index].Vital[n] = data.ReadInt16();
    }

    private static void MapNpcDied(NetBuffer data)
    {
        var i = data.ReadByte();

        // Limpa os dados do Npc
        TempMap.Current.Npc[i].X2 = 0;
        TempMap.Current.Npc[i].Y2 = 0;
        TempMap.Current.Npc[i].Data = null;
        TempMap.Current.Npc[i].X = 0;
        TempMap.Current.Npc[i].Y = 0;
        TempMap.Current.Npc[i].Vital = new short[(byte)Vital.Count];
    }
}