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
using static CryBits.Client.Logic.Game;
using static CryBits.Utils;

namespace CryBits.Client.Network
{
    internal static class Receive
    {
        public static void Handle(NetIncomingMessage data)
        {
            // Manuseia os dados recebidos
            switch ((ServerClient)data.ReadByte())
            {
                case ServerClient.Alert: Alert(data); break;
                case ServerClient.Connect: Connect(); break;
                case ServerClient.Join: Join(data); break;
                case ServerClient.CreateCharacter: CreateCharacter(); break;
                case ServerClient.JoinGame: JoinGame(); break;
                case ServerClient.Classes: Classes(data); break;
                case ServerClient.Characters: Characters(data); break;
                case ServerClient.PlayerData: Player_Data(data); break;
                case ServerClient.PlayerPosition: Player_Position(data); break;
                case ServerClient.PlayerVitals: Player_Vitals(data); break;
                case ServerClient.PlayerMove: Player_Move(data); break;
                case ServerClient.PlayerLeave: Player_Leave(data); break;
                case ServerClient.PlayerDirection: Player_Direction(data); break;
                case ServerClient.PlayerAttack: Player_Attack(data); break;
                case ServerClient.PlayerExperience: Player_Experience(data); break;
                case ServerClient.PlayerInventory: Player_Inventory(data); break;
                case ServerClient.PlayerEquipments: Player_Equipments(data); break;
                case ServerClient.PlayerHotbar: Player_Hotbar(data); break;
                case ServerClient.MapRevision: Map_Revision(data); break;
                case ServerClient.Map: Map(data); break;
                case ServerClient.JoinMap: JoinMap(); break;
                case ServerClient.Latency: Latency(); break;
                case ServerClient.Message: Message(data); break;
                case ServerClient.NPCs: NPCs(data); break;
                case ServerClient.MapNPCs: Map_NPCs(data); break;
                case ServerClient.MapNPC: Map_NPC(data); break;
                case ServerClient.MapNPCMovement: Map_NPC_Movement(data); break;
                case ServerClient.MapNPCDirection: Map_NPC_Direction(data); break;
                case ServerClient.MapNPCVitals: Map_NPC_Vitals(data); break;
                case ServerClient.MapNPCAttack: Map_NPC_Attack(data); break;
                case ServerClient.MapNPCDied: Map_NPC_Died(data); break;
                case ServerClient.Items: Items(data); break;
                case ServerClient.MapItems: Map_Items(data); break;
                case ServerClient.Party: Party(data); break;
                case ServerClient.PartyInvitation: Party_Invitation(data); break;
                case ServerClient.Trade: Trade(data); break;
                case ServerClient.TradeInvitation: Trade_Invitation(data); break;
                case ServerClient.TradeState: Trade_State(data); break;
                case ServerClient.TradeOffer: Trade_Offer(data); break;
                case ServerClient.Shops: Shops(data); break;
                case ServerClient.ShopOpen: Shop_Open(data); break;
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
            Panels.Menu_Close();
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
            Panels.Menu_Close();
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
            CheckBoxes.List["Options_Sounds"].Checked = Option.Sounds;
            CheckBoxes.List["Options_Musics"].Checked = Option.Musics;
            CheckBoxes.List["Options_Chat"].Checked = Option.Chat;
            CheckBoxes.List["Options_FPS"].Checked = Option.FPS;
            CheckBoxes.List["Options_Latency"].Checked = Option.Latency;
            CheckBoxes.List["Options_Trade"].Checked = Option.Trade;
            CheckBoxes.List["Options_Party"].Checked = Option.Party;
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

        private static void Player_Data(NetIncomingMessage data)
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
            for (byte n = 0; n < (byte)Equipments.Count; n++) player.Equipment[n] = Item.Get(new Guid(data.ReadString()));
            Mapper.Current = player.Map;
        }

        private static void Player_Position(NetIncomingMessage data)
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

        private static void Player_Vitals(NetIncomingMessage data)
        {
            Player player = Player.Get(data.ReadString());

            // Define os dados
            for (byte i = 0; i < (byte)Vitals.Count; i++)
            {
                player.Vital[i] = data.ReadInt16();
                player.MaxVital[i] = data.ReadInt16();
            }
        }

        private static void Player_Equipments(NetIncomingMessage data)
        {
            Player player = Player.Get(data.ReadString());

            // Altera os dados dos equipamentos do jogador
            for (byte i = 0; i < (byte)Equipments.Count; i++) player.Equipment[i] = Item.Get(new Guid(data.ReadString()));
        }

        private static void Player_Leave(NetIncomingMessage data)
        {
            // Limpa os dados do jogador
            Player.List.Remove(Player.Get(data.ReadString()));
        }

        private static void Player_Move(NetIncomingMessage data)
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

        private static void Player_Direction(NetIncomingMessage data)
        {
            // Altera a posição do jogador
            Player.Get(data.ReadString()).Direction = (Directions)data.ReadByte();
        }

        private static void Player_Attack(NetIncomingMessage data)
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
                    Mapper.Current.Blood.Add(new MapBlood((byte)MyRandom.Next(0, 3), victimData.X, victimData.Y, 255));
                }
                else if (victimType == (byte)Target.NPC)
                {
                    Mapper.Current.NPC[byte.Parse(victim)].Hurt = Environment.TickCount;
                    Mapper.Current.Blood.Add(new MapBlood((byte)MyRandom.Next(0, 3), Mapper.Current.NPC[byte.Parse(victim)].X, Mapper.Current.NPC[byte.Parse(victim)].Y, 255));
                }
        }

        private static void Player_Experience(NetIncomingMessage data)
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

        private static void Player_Inventory(NetIncomingMessage data)
        {
            // Define os dados
            for (byte i = 0; i < MaxInventory; i++)
            {
                Player.Me.Inventory[i].Item = Item.Get(new Guid(data.ReadString()));
                Player.Me.Inventory[i].Amount = data.ReadInt16();
            }
        }

        private static void Player_Hotbar(NetIncomingMessage data)
        {
            // Define os dados
            for (byte i = 0; i < MaxHotbar; i++)
            {
                Player.Me.Hotbar[i].Type = data.ReadByte();
                Player.Me.Hotbar[i].Slot = data.ReadByte();
            }
        }

        private static void Map_Revision(NetIncomingMessage data)
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
            Mapper.Current.Blood = new List<MapBlood>();
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

            Mapper.Current = TempMap.List[id];

            // Salva o mapa
            Write.Map(map);

            // Redimensiona as partículas do clima
            Mapper.Weather_Update();
            Mapper.Current.Data.Update();
        }

        private static void JoinMap()
        {
            // Se tiver, reproduz a música de fundo do mapa
            if (Mapper.Current.Data.Music > 0)
                Music.Play((Musics)Mapper.Current.Data.Music);
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

        private static void Map_Items(NetIncomingMessage data)
        {
            // Quantidade
            Mapper.Current.Item = new MapItems[data.ReadByte()];

            // Lê os dados de todos
            for (byte i = 0; i < Mapper.Current.Item.Length; i++)
            {
                // Geral
                Mapper.Current.Item[i] = new MapItems();
                Mapper.Current.Item[i].Item = Item.Get(new Guid(data.ReadString()));
                Mapper.Current.Item[i].X = data.ReadByte();
                Mapper.Current.Item[i].Y = data.ReadByte();
            }
        }

        private static void Party(NetIncomingMessage data)
        {
            // Lê os dados do grupo
            Player.Me.Party = new Player[data.ReadByte()];
            for (byte i = 0; i < Player.Me.Party.Length; i++) Player.Me.Party[i] = Player.Get(data.ReadString());
        }

        private static void Party_Invitation(NetIncomingMessage data)
        {
            // Nega o pedido caso o jogador não quiser receber convites
            if (!Option.Party)
            {
                Send.Party_Decline();
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
                Player.Me.TradeOffer = new ItemSlot[MaxInventory ];
                Player.Me.TradeTheirOffer = new ItemSlot[MaxInventory];
            }
            else
            {
                // Limpa os dados
                Player.Me.TradeOffer = null;
                Player.Me.TradeTheirOffer = null;
            }
        }

        private static void Trade_Invitation(NetIncomingMessage data)
        {
            // Nega o pedido caso o jogador não quiser receber convites
            if (!Option.Trade)
            {
                Send.Trade_Decline();
                return;
            }

            // Abre a janela de convite para o grupo
            Panels.TradeInvitation = data.ReadString();
            Panels.List["Trade_Invitation"].Visible = true;
        }

        private static void Trade_State(NetIncomingMessage data)
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

        private static void Trade_Offer(NetIncomingMessage data)
        {
            // Recebe os dados da oferta
            if (data.ReadBoolean())
                for (byte i = 0; i < MaxInventory; i++)
                {
                    Player.Me.TradeOffer[i].Item = Item.Get(new Guid(data.ReadString()));
                    Player.Me.TradeOffer[i].Amount = data.ReadInt16();
                }
            else
                for (byte i = 0; i < MaxInventory; i++)
                {
                    Player.Me.TradeTheirOffer[i].Item = Item.Get(new Guid(data.ReadString()));
                    Player.Me.TradeTheirOffer[i].Amount = data.ReadInt16();
                }
        }

        private static void Shops(NetIncomingMessage data)
        {
            // Recebe os dados
            Shop.List = (Dictionary<Guid, Shop>)ByteArrayToObject(data);
        }

        private static void Shop_Open(NetIncomingMessage data)
        {
            // Abre a loja
            Panels.ShopOpen = Shop.Get(new Guid(data.ReadString()));
            Panels.List["Shop"].Visible = Panels.ShopOpen != null;
        }

        private static void NPCs(NetIncomingMessage data)
        {
            // Recebe os dados
            NPC.List = (Dictionary<Guid, NPC>)ByteArrayToObject(data);
        }

        private static void Map_NPCs(NetIncomingMessage data)
        {
            // Lê os dados
            Mapper.Current.NPC = new TempNPC[data.ReadInt16()];
            for (byte i = 0; i < Mapper.Current.NPC.Length; i++)
            {
                Mapper.Current.NPC[i] = new TempNPC();
                Mapper.Current.NPC[i].X2 = 0;
                Mapper.Current.NPC[i].Y2 = 0;
                Mapper.Current.NPC[i].Data = NPC.Get(new Guid(data.ReadString()));
                Mapper.Current.NPC[i].X = data.ReadByte();
                Mapper.Current.NPC[i].Y = data.ReadByte();
                Mapper.Current.NPC[i].Direction = (Directions)data.ReadByte();

                // Vitais
                for (byte n = 0; n < (byte)Vitals.Count; n++)
                    Mapper.Current.NPC[i].Vital[n] = data.ReadInt16();
            }
        }

        private static void Map_NPC(NetIncomingMessage data)
        {
            // Lê os dados
            byte i = data.ReadByte();
            Mapper.Current.NPC[i].X2 = 0;
            Mapper.Current.NPC[i].Y2 = 0;
            Mapper.Current.NPC[i].Data = NPC.Get(new Guid(data.ReadString()));
            Mapper.Current.NPC[i].X = data.ReadByte();
            Mapper.Current.NPC[i].Y = data.ReadByte();
            Mapper.Current.NPC[i].Direction = (Directions)data.ReadByte();
            Mapper.Current.NPC[i].Vital = new short[(byte)Vitals.Count];
            for (byte n = 0; n < (byte)Vitals.Count; n++) Mapper.Current.NPC[i].Vital[n] = data.ReadInt16();
        }

        private static void Map_NPC_Movement(NetIncomingMessage data)
        {
            // Lê os dados
            byte i = data.ReadByte();
            byte x = Mapper.Current.NPC[i].X, y = Mapper.Current.NPC[i].Y;
            Mapper.Current.NPC[i].X2 = 0;
            Mapper.Current.NPC[i].Y2 = 0;
            Mapper.Current.NPC[i].X = data.ReadByte();
            Mapper.Current.NPC[i].Y = data.ReadByte();
            Mapper.Current.NPC[i].Direction = (Directions)data.ReadByte();
            Mapper.Current.NPC[i].Movement = (Movements)data.ReadByte();

            // Posição exata do jogador
            if (x != Mapper.Current.NPC[i].X || y != Mapper.Current.NPC[i].Y)
                switch (Mapper.Current.NPC[i].Direction)
                {
                    case Directions.Up: Mapper.Current.NPC[i].Y2 = Grid; break;
                    case Directions.Down: Mapper.Current.NPC[i].Y2 = Grid * -1; break;
                    case Directions.Right: Mapper.Current.NPC[i].X2 = Grid * -1; break;
                    case Directions.Left: Mapper.Current.NPC[i].X2 = Grid; break;
                }
        }

        private static void Map_NPC_Attack(NetIncomingMessage data)
        {
            byte index = data.ReadByte();
            string victim = data.ReadString();
            byte victimType = data.ReadByte();

            // Inicia o ataque
            Mapper.Current.NPC[index].Attacking = true;
            Mapper.Current.NPC[index].AttackTimer = Environment.TickCount;

            // Sofrendo dano
            if (victim != string.Empty)
                if (victimType == (byte)Target.Player)
                {
                    Player victimData = Player.Get(victim);
                    victimData.Hurt = Environment.TickCount;
                    Mapper.Current.Blood.Add(new MapBlood((byte)MyRandom.Next(0, 3), victimData.X, victimData.Y, 255));
                }
                else if (victimType == (byte)Target.NPC)
                {
                    Mapper.Current.NPC[byte.Parse(victim)].Hurt = Environment.TickCount;
                    Mapper.Current.Blood.Add(new MapBlood((byte)MyRandom.Next(0, 3), Mapper.Current.NPC[byte.Parse(victim)].X, Mapper.Current.NPC[byte.Parse(victim)].Y, 255));
                }
        }

        private static void Map_NPC_Direction(NetIncomingMessage data)
        {
            // Define a direção de determinado NPC
            byte i = data.ReadByte();
            Mapper.Current.NPC[i].Direction = (Directions)data.ReadByte();
            Mapper.Current.NPC[i].X2 = 0;
            Mapper.Current.NPC[i].Y2 = 0;
        }

        private static void Map_NPC_Vitals(NetIncomingMessage data)
        {
            byte index = data.ReadByte();

            // Define os vitais de determinado NPC
            for (byte n = 0; n < (byte)Vitals.Count; n++)
                Mapper.Current.NPC[index].Vital[n] = data.ReadInt16();
        }

        private static void Map_NPC_Died(NetIncomingMessage data)
        {
            byte i = data.ReadByte();

            // Limpa os dados do NPC
            Mapper.Current.NPC[i].X2 = 0;
            Mapper.Current.NPC[i].Y2 = 0;
            Mapper.Current.NPC[i].Data = null;
            Mapper.Current.NPC[i].X = 0;
            Mapper.Current.NPC[i].Y = 0;
            Mapper.Current.NPC[i].Vital = new short[(byte)Vitals.Count];
        }
    }
}