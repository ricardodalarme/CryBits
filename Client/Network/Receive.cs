using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using CryBits.Client.Entities;
using CryBits.Client.Library;
using CryBits.Client.Logic;
using CryBits.Client.Media.Audio;
using CryBits.Client.UI;
using CryBits.Entities;
using CryBits.Packets;
using Lidgren.Network;
using static CryBits.Defaults;
using static CryBits.Utils;

namespace CryBits.Client.Network
{
    internal static class Receive
    {
        public static void Handle(NetIncomingMessage data)
        {
            // Manuseia os dados recebidos
            switch ((ServerPackets)data.ReadByte())
            {
                case ServerPackets.Alert: Alert(data); break;
                case ServerPackets.Connect: Connect(); break;
                case ServerPackets.Join: Join(data); break;
                case ServerPackets.CreateCharacter: CreateCharacter(); break;
                case ServerPackets.JoinGame: JoinGame(); break;
                case ServerPackets.Classes: Classes(data); break;
                case ServerPackets.Characters: Characters(data); break;
                case ServerPackets.PlayerData: PlayerData(data); break;
                case ServerPackets.PlayerPosition: PlayerPosition(data); break;
                case ServerPackets.PlayerVitals: PlayerVitals(data); break;
                case ServerPackets.PlayerMove: PlayerMove(data); break;
                case ServerPackets.PlayerLeave: PlayerLeave(data); break;
                case ServerPackets.PlayerDirection: PlayerDirection(data); break;
                case ServerPackets.PlayerAttack: PlayerAttack(data); break;
                case ServerPackets.PlayerExperience: PlayerExperience(data); break;
                case ServerPackets.PlayerInventory: PlayerInventory(data); break;
                case ServerPackets.PlayerEquipments: PlayerEquipments(data); break;
                case ServerPackets.PlayerHotbar: PlayerHotbar(data); break;
                case ServerPackets.MapRevision: MapRevision(data); break;
                case ServerPackets.Map: Map(data); break;
                case ServerPackets.JoinMap: JoinMap(); break;
                case ServerPackets.Latency: Latency(); break;
                case ServerPackets.Message: Message(data); break;
                case ServerPackets.NPCs: NPCs(data); break;
                case ServerPackets.MapNPCs: MapNPCs(data); break;
                case ServerPackets.MapNPC: MapNPC(data); break;
                case ServerPackets.MapNPCMovement: MapNPCMovement(data); break;
                case ServerPackets.MapNPCDirection: MapNPCDirection(data); break;
                case ServerPackets.MapNPCVitals: MapNPCVitals(data); break;
                case ServerPackets.MapNPCAttack: MapNPCAttack(data); break;
                case ServerPackets.MapNPCDied: MapNPCDied(data); break;
                case ServerPackets.Items: Items(data); break;
                case ServerPackets.MapItems: MapItems(data); break;
                case ServerPackets.Party: Party(data); break;
                case ServerPackets.PartyInvitation: PartyInvitation(data); break;
                case ServerPackets.Trade: Trade(data); break;
                case ServerPackets.TradeInvitation: TradeInvitation(data); break;
                case ServerPackets.TradeState: TradeState(data); break;
                case ServerPackets.TradeOffer: TradeOffer(data); break;
                case ServerPackets.Shops: Shops(data); break;
                case ServerPackets.ShopOpen: ShopOpen(data); break;
            }
        }

        private static void Alert(NetIncomingMessage data)
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

        private static void Join(NetIncomingMessage data)
        {
            // Reseta alguns valores
            Player.List = new List<Player>();
            Item.List = new Dictionary<Guid, Item>();
            Shop.List = new Dictionary<Guid, Shop>();
            NPC.List = new Dictionary<Guid, NPC>();
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

        private static void Classes(NetIncomingMessage data)
        {
            // Recebe os dados
            Class.List = (Dictionary<Guid, Class>)ByteArrayToObject(data);
        }

        private static void Characters(NetIncomingMessage data)
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
            CheckBoxes.List["Options_FPS"].Checked = Options.FPS;
            CheckBoxes.List["Options_Latency"].Checked = Options.Latency;
            CheckBoxes.List["Options_Trade"].Checked = Options.Trade;
            CheckBoxes.List["Options_Party"].Checked = Options.Party;
            Loop.ChatTimer = Loop.ChatTimer = Environment.TickCount + 10000;
            Panels.InformationID = Guid.Empty;

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
            Windows.Current = WindowsTypes.Game;
        }

        private static void PlayerData(NetIncomingMessage data)
        {
            string name = data.ReadString();
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
            player.Direction = (Directions)data.ReadByte();
            for (byte n = 0; n < (byte)Vitals.Count; n++)
            {
                player.Vital[n] = data.ReadInt16();
                player.MaxVital[n] = data.ReadInt16();
            }
            for (byte n = 0; n < (byte)Attributes.Count; n++) player.Attribute[n] = data.ReadInt16();
            for (byte n = 0; n < (byte)Equipments.Count; n++) player.Equipment[n] = Item.List.Get(new Guid(data.ReadString()));
            TempMap.Current = player.Map;
        }

        private static void PlayerPosition(NetIncomingMessage data)
        {
            Player player = Player.Get(data.ReadString());

            // Defini os dados do jogador
            player.X = data.ReadByte();
            player.Y = data.ReadByte();
            player.Direction = (Directions)data.ReadByte();

            // Para a movimentação
            player.X2 = 0;
            player.Y2 = 0;
            player.Movement = Movements.Stopped;
        }

        private static void PlayerVitals(NetIncomingMessage data)
        {
            Player player = Player.Get(data.ReadString());

            // Define os dados
            for (byte i = 0; i < (byte)Vitals.Count; i++)
            {
                player.Vital[i] = data.ReadInt16();
                player.MaxVital[i] = data.ReadInt16();
            }
        }

        private static void PlayerEquipments(NetIncomingMessage data)
        {
            Player player = Player.Get(data.ReadString());

            // Altera os dados dos equipamentos do jogador
            for (byte i = 0; i < (byte)Equipments.Count; i++) player.Equipment[i] = Item.List.Get(new Guid(data.ReadString()));
        }

        private static void PlayerLeave(NetIncomingMessage data)
        {
            // Limpa os dados do jogador
            Player.List.Remove(Player.Get(data.ReadString()));
        }

        private static void PlayerMove(NetIncomingMessage data)
        {
            Player player = Player.Get(data.ReadString());

            // Move o jogador
            player.X = data.ReadByte();
            player.Y = data.ReadByte();
            player.Direction = (Directions)data.ReadByte();
            player.Movement = (Movements)data.ReadByte();
            player.X2 = 0;
            player.Y2 = 0;

            // Posição exata do jogador
            switch (player.Direction)
            {
                case Directions.Up: player.Y2 = Grid; break;
                case Directions.Down: player.Y2 = Grid * -1; break;
                case Directions.Right: player.X2 = Grid * -1; break;
                case Directions.Left: player.X2 = Grid; break;
            }
        }

        private static void PlayerDirection(NetIncomingMessage data)
        {
            // Altera a posição do jogador
            Player.Get(data.ReadString()).Direction = (Directions)data.ReadByte();
        }

        private static void PlayerAttack(NetIncomingMessage data)
        {
            Player player = Player.Get(data.ReadString());
            string victim = data.ReadString();
            byte victimType = data.ReadByte();

            // Inicia o ataque
            player.Attacking = true;
            player.AttackTimer = Environment.TickCount;

            // Sofrendo dano
            if (victim != string.Empty)
                if (victimType == (byte)Target.Player)
                {
                    Player victimData = Player.Get(victim);
                    victimData.Hurt = Environment.TickCount;
                    TempMap.Current.Blood.Add(new MapBlood((byte)MyRandom.Next(0, 3), victimData.X, victimData.Y, 255));
                }
                else if (victimType == (byte)Target.NPC)
                {
                    TempMap.Current.NPC[byte.Parse(victim)].Hurt = Environment.TickCount;
                    TempMap.Current.Blood.Add(new MapBlood((byte)MyRandom.Next(0, 3), TempMap.Current.NPC[byte.Parse(victim)].X, TempMap.Current.NPC[byte.Parse(victim)].Y, 255));
                }
        }

        private static void PlayerExperience(NetIncomingMessage data)
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

        private static void PlayerInventory(NetIncomingMessage data)
        {
            // Define os dados
            for (byte i = 0; i < MaxInventory; i++)
                Player.Me.Inventory[i] = new ItemSlot(Item.List.Get(new Guid(data.ReadString())), data.ReadInt16());
        }

        private static void PlayerHotbar(NetIncomingMessage data)
        {
            // Define os dados
            for (byte i = 0; i < MaxHotbar; i++)
            {
                Player.Me.Hotbar[i].Type = data.ReadByte();
                Player.Me.Hotbar[i].Slot = data.ReadByte();
            }
        }

        private static void MapRevision(NetIncomingMessage data)
        {
            bool needed = false;
            Guid id = new Guid(data.ReadString());
            short currentRevision = data.ReadInt16();

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

        private static void Map(NetIncomingMessage data)
        {
            Map map = (Map)ByteArrayToObject(data);
            Guid id = map.ID;

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
                Music.Play((Musics)TempMap.Current.Data.Music);
            else
                Music.Stop();
        }

        private static void Latency()
        {
            // Define a latência
            Socket.Latency = Environment.TickCount - Socket.LatencySend;
        }

        private static void Message(NetIncomingMessage data)
        {
            // Adiciona a mensagem
            string text = data.ReadString();
            Color color = Color.FromArgb(data.ReadInt32());
            Chat.AddText(text, new SFML.Graphics.Color(color.R, color.G, color.B));
        }

        private static void Items(NetIncomingMessage data)
        {
            // Recebe os dados
            Item.List = (Dictionary<Guid, Item>)ByteArrayToObject(data);
        }

        private static void MapItems(NetIncomingMessage data)
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

        private static void Party(NetIncomingMessage data)
        {
            // Lê os dados do grupo
            Player.Me.Party = new Player[data.ReadByte()];
            for (byte i = 0; i < Player.Me.Party.Length; i++) Player.Me.Party[i] = Player.Get(data.ReadString());
        }

        private static void PartyInvitation(NetIncomingMessage data)
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

        private static void Trade(NetIncomingMessage data)
        {
            bool state = data.ReadBoolean();

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

        private static void TradeInvitation(NetIncomingMessage data)
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

        private static void TradeState(NetIncomingMessage data)
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

        private static void TradeOffer(NetIncomingMessage data)
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

        private static void Shops(NetIncomingMessage data)
        {
            // Recebe os dados
            Shop.List = (Dictionary<Guid, Shop>)ByteArrayToObject(data);
        }

        private static void ShopOpen(NetIncomingMessage data)
        {
            // Abre a loja
            Panels.ShopOpen = Shop.List.Get(new Guid(data.ReadString()));
            Panels.List["Shop"].Visible = Panels.ShopOpen != null;
        }

        private static void NPCs(NetIncomingMessage data)
        {
            // Recebe os dados
            NPC.List = (Dictionary<Guid, NPC>)ByteArrayToObject(data);
        }

        private static void MapNPCs(NetIncomingMessage data)
        {
            // Lê os dados
            TempMap.Current.NPC = new TempNPC[data.ReadInt16()];
            for (byte i = 0; i < TempMap.Current.NPC.Length; i++)
            {
                TempMap.Current.NPC[i] = new TempNPC();
                TempMap.Current.NPC[i].X2 = 0;
                TempMap.Current.NPC[i].Y2 = 0;
                TempMap.Current.NPC[i].Data = NPC.List.Get(new Guid(data.ReadString()));
                TempMap.Current.NPC[i].X = data.ReadByte();
                TempMap.Current.NPC[i].Y = data.ReadByte();
                TempMap.Current.NPC[i].Direction = (Directions)data.ReadByte();

                // Vitais
                for (byte n = 0; n < (byte)Vitals.Count; n++)
                    TempMap.Current.NPC[i].Vital[n] = data.ReadInt16();
            }
        }

        private static void MapNPC(NetIncomingMessage data)
        {
            // Lê os dados
            byte i = data.ReadByte();
            TempMap.Current.NPC[i].X2 = 0;
            TempMap.Current.NPC[i].Y2 = 0;
            TempMap.Current.NPC[i].Data = NPC.List.Get(new Guid(data.ReadString()));
            TempMap.Current.NPC[i].X = data.ReadByte();
            TempMap.Current.NPC[i].Y = data.ReadByte();
            TempMap.Current.NPC[i].Direction = (Directions)data.ReadByte();
            TempMap.Current.NPC[i].Vital = new short[(byte)Vitals.Count];
            for (byte n = 0; n < (byte)Vitals.Count; n++) TempMap.Current.NPC[i].Vital[n] = data.ReadInt16();
        }

        private static void MapNPCMovement(NetIncomingMessage data)
        {
            // Lê os dados
            byte i = data.ReadByte();
            byte x = TempMap.Current.NPC[i].X, y = TempMap.Current.NPC[i].Y;
            TempMap.Current.NPC[i].X2 = 0;
            TempMap.Current.NPC[i].Y2 = 0;
            TempMap.Current.NPC[i].X = data.ReadByte();
            TempMap.Current.NPC[i].Y = data.ReadByte();
            TempMap.Current.NPC[i].Direction = (Directions)data.ReadByte();
            TempMap.Current.NPC[i].Movement = (Movements)data.ReadByte();

            // Posição exata do jogador
            if (x != TempMap.Current.NPC[i].X || y != TempMap.Current.NPC[i].Y)
                switch (TempMap.Current.NPC[i].Direction)
                {
                    case Directions.Up: TempMap.Current.NPC[i].Y2 = Grid; break;
                    case Directions.Down: TempMap.Current.NPC[i].Y2 = Grid * -1; break;
                    case Directions.Right: TempMap.Current.NPC[i].X2 = Grid * -1; break;
                    case Directions.Left: TempMap.Current.NPC[i].X2 = Grid; break;
                }
        }

        private static void MapNPCAttack(NetIncomingMessage data)
        {
            byte index = data.ReadByte();
            string victim = data.ReadString();
            byte victimType = data.ReadByte();

            // Inicia o ataque
            TempMap.Current.NPC[index].Attacking = true;
            TempMap.Current.NPC[index].AttackTimer = Environment.TickCount;

            // Sofrendo dano
            if (victim != string.Empty)
                if (victimType == (byte)Target.Player)
                {
                    Player victimData = Player.Get(victim);
                    victimData.Hurt = Environment.TickCount;
                    TempMap.Current.Blood.Add(new MapBlood((byte)MyRandom.Next(0, 3), victimData.X, victimData.Y, 255));
                }
                else if (victimType == (byte)Target.NPC)
                {
                    TempMap.Current.NPC[byte.Parse(victim)].Hurt = Environment.TickCount;
                    TempMap.Current.Blood.Add(new MapBlood((byte)MyRandom.Next(0, 3), TempMap.Current.NPC[byte.Parse(victim)].X, TempMap.Current.NPC[byte.Parse(victim)].Y, 255));
                }
        }

        private static void MapNPCDirection(NetIncomingMessage data)
        {
            // Define a direção de determinado NPC
            byte i = data.ReadByte();
            TempMap.Current.NPC[i].Direction = (Directions)data.ReadByte();
            TempMap.Current.NPC[i].X2 = 0;
            TempMap.Current.NPC[i].Y2 = 0;
        }

        private static void MapNPCVitals(NetIncomingMessage data)
        {
            byte index = data.ReadByte();

            // Define os vitais de determinado NPC
            for (byte n = 0; n < (byte)Vitals.Count; n++)
                TempMap.Current.NPC[index].Vital[n] = data.ReadInt16();
        }

        private static void MapNPCDied(NetIncomingMessage data)
        {
            byte i = data.ReadByte();

            // Limpa os dados do NPC
            TempMap.Current.NPC[i].X2 = 0;
            TempMap.Current.NPC[i].Y2 = 0;
            TempMap.Current.NPC[i].Data = null;
            TempMap.Current.NPC[i].X = 0;
            TempMap.Current.NPC[i].Y = 0;
            TempMap.Current.NPC[i].Vital = new short[(byte)Vitals.Count];
        }
    }
}