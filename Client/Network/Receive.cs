using CryBits.Client.Network.Handlers;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Packets.Server;
using LiteNetLib;

namespace CryBits.Client.Network;

internal static class Receive
{
    /// <summary>Process incoming client packets and dispatch to handlers.</summary>
    public static void Handle(NetPacketReader data)
    {
        switch ((ServerPacket)data.GetByte())
        {
            case ServerPacket.Latency: AuthHandler.Latency(); data.ReadObject(); break;
            case ServerPacket.Alert: AuthHandler.Alert((AlertPacket)data.ReadObject()); break;
            case ServerPacket.Connect: AuthHandler.Connect(); data.ReadObject(); break;
            case ServerPacket.Join: AccountHandler.Join((JoinPacket)data.ReadObject()); break; // Join was mapped in ServerPackets
            case ServerPacket.CreateCharacter: AccountHandler.CreateCharacter(); data.ReadObject(); break;
            case ServerPacket.JoinGame: AccountHandler.JoinGame(); break;
            case ServerPacket.Classes: ClassHandler.Classes((ClassesPacket)data.ReadObject()); break;
            case ServerPacket.Characters: AccountHandler.Characters((CharactersPacket)data.ReadObject()); break;
            case ServerPacket.PlayerData: PlayerHandler.PlayerData((PlayerDataPacket)data.ReadObject()); break;
            case ServerPacket.PlayerPosition: PlayerHandler.PlayerPosition((PlayerPositionPacket)data.ReadObject()); break;
            case ServerPacket.PlayerVitals: PlayerHandler.PlayerVitals((PlayerVitalsPacket)data.ReadObject()); break;
            case ServerPacket.PlayerMove: PlayerHandler.PlayerMove((PlayerMovePacket)data.ReadObject()); break;
            case ServerPacket.PlayerLeave: PlayerHandler.PlayerLeave((PlayerLeavePacket)data.ReadObject()); break;
            case ServerPacket.PlayerDirection: PlayerHandler.PlayerDirection((PlayerDirectionPacket)data.ReadObject()); break;
            case ServerPacket.PlayerAttack: PlayerHandler.PlayerAttack((PlayerAttackPacket)data.ReadObject()); break;
            case ServerPacket.PlayerExperience: PlayerHandler.PlayerExperience((PlayerExperiencePacket)data.ReadObject()); break;
            case ServerPacket.PlayerInventory: PlayerHandler.PlayerInventory((PlayerInventoryPacket)data.ReadObject()); break;
            case ServerPacket.PlayerEquipments: PlayerHandler.PlayerEquipments((PlayerEquipmentsPacket)data.ReadObject()); break;
            case ServerPacket.PlayerHotbar: PlayerHandler.PlayerHotbar((PlayerHotbarPacket)data.ReadObject()); break;
            case ServerPacket.MapRevision: MapHandler.MapRevision((MapRevisionPacket)data.ReadObject()); break;
            case ServerPacket.Map: MapHandler.Map((MapPacket)data.ReadObject()); break;
            case ServerPacket.JoinMap: MapHandler.JoinMap(); break;
            case ServerPacket.Message: ChatHandler.Message((MessagePacket)data.ReadObject()); break;
            case ServerPacket.Npcs: NpcHandler.Npcs((NpcsPacket)data.ReadObject()); break;
            case ServerPacket.MapNpcs: NpcHandler.MapNpcs((MapNpcsPacket)data.ReadObject()); break;
            case ServerPacket.MapNpc: NpcHandler.MapNpc((MapNpcPacket)data.ReadObject()); break;
            case ServerPacket.MapNpcMovement: NpcHandler.MapNpcMovement((MapNpcMovementPacket)data.ReadObject()); break;
            case ServerPacket.MapNpcDirection: NpcHandler.MapNpcDirection((MapNpcDirectionPacket)data.ReadObject()); break;
            case ServerPacket.MapNpcVitals: NpcHandler.MapNpcVitals((MapNpcVitalsPacket)data.ReadObject()); break;
            case ServerPacket.MapNpcAttack: NpcHandler.MapNpcAttack((MapNpcAttackPacket)data.ReadObject()); break;
            case ServerPacket.MapNpcDied: NpcHandler.MapNpcDied((MapNpcDiedPacket)data.ReadObject()); break;
            case ServerPacket.Items: ItemHandler.Items((ItemsPacket)data.ReadObject()); break;
            case ServerPacket.MapItems: MapHandler.MapItems((MapItemsPacket)data.ReadObject()); break;
            case ServerPacket.Party: PartyHandler.Party((PartyPacket)data.ReadObject()); break;
            case ServerPacket.PartyInvitation: PartyHandler.PartyInvitation((PartyInvitationPacket)data.ReadObject()); break;
            case ServerPacket.Trade: TradeHandler.Trade((TradePacket)data.ReadObject()); break;
            case ServerPacket.TradeInvitation: TradeHandler.TradeInvitation((TradeInvitationPacket)data.ReadObject()); break;
            case ServerPacket.TradeState: TradeHandler.TradeState((TradeStatePacket)data.ReadObject()); break;
            case ServerPacket.TradeOffer: TradeHandler.TradeOffer((TradeOfferPacket)data.ReadObject()); break;
            case ServerPacket.Shops: ShopHandler.Shops((ShopsPacket)data.ReadObject()); break;
            case ServerPacket.ShopOpen: ShopHandler.ShopOpen((ShopOpenPacket)data.ReadObject()); break;
        }
    }
}
