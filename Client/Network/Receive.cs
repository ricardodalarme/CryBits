using CryBits.Client.Network.Handlers;
using CryBits.Enums;
using LiteNetLib;

namespace CryBits.Client.Network;

internal static class Receive
{
    public static void Handle(NetPacketReader data)
    {
        // Manuseia os dados recebidos
        switch ((ServerPacket)data.GetByte())
        {
            case ServerPacket.Latency: AuthHandler.Latency(); break;
            case ServerPacket.Alert: AuthHandler.Alert(data); break;
            case ServerPacket.Connect: AuthHandler.Connect(); break;
            case ServerPacket.Join: AccountHandler.Join(data); break;
            case ServerPacket.CreateCharacter: AccountHandler.CreateCharacter(); break;
            case ServerPacket.JoinGame: AccountHandler.JoinGame(); break;
            case ServerPacket.Classes: ClassHandler.Classes(data); break;
            case ServerPacket.Characters: AccountHandler.Characters(data); break;
            case ServerPacket.PlayerData: PlayerHandler.PlayerData(data); break;
            case ServerPacket.PlayerPosition: PlayerHandler.PlayerPosition(data); break;
            case ServerPacket.PlayerVitals: PlayerHandler.PlayerVitals(data); break;
            case ServerPacket.PlayerMove: PlayerHandler.PlayerMove(data); break;
            case ServerPacket.PlayerLeave: PlayerHandler.PlayerLeave(data); break;
            case ServerPacket.PlayerDirection: PlayerHandler.PlayerDirection(data); break;
            case ServerPacket.PlayerAttack: PlayerHandler.PlayerAttack(data); break;
            case ServerPacket.PlayerExperience: PlayerHandler.PlayerExperience(data); break;
            case ServerPacket.PlayerInventory: PlayerHandler.PlayerInventory(data); break;
            case ServerPacket.PlayerEquipments: PlayerHandler.PlayerEquipments(data); break;
            case ServerPacket.PlayerHotbar: PlayerHandler.PlayerHotbar(data); break;
            case ServerPacket.MapRevision: MapHandler.MapRevision(data); break;
            case ServerPacket.Map: MapHandler.Map(data); break;
            case ServerPacket.JoinMap: MapHandler.JoinMap(); break;
            case ServerPacket.Message: ChatHandler.Message(data); break;
            case ServerPacket.Npcs: NpcHandler.Npcs(data); break;
            case ServerPacket.MapNpcs: NpcHandler.MapNpcs(data); break;
            case ServerPacket.MapNpc: NpcHandler.MapNpc(data); break;
            case ServerPacket.MapNpcMovement: NpcHandler.MapNpcMovement(data); break;
            case ServerPacket.MapNpcDirection: NpcHandler.MapNpcDirection(data); break;
            case ServerPacket.MapNpcVitals: NpcHandler.MapNpcVitals(data); break;
            case ServerPacket.MapNpcAttack: NpcHandler.MapNpcAttack(data); break;
            case ServerPacket.MapNpcDied: NpcHandler.MapNpcDied(data); break;
            case ServerPacket.Items: ItemHandler.Items(data); break;
            case ServerPacket.MapItems: MapHandler.MapItems(data); break;
            case ServerPacket.Party: PartyHandler.Party(data); break;
            case ServerPacket.PartyInvitation: PartyHandler.PartyInvitation(data); break;
            case ServerPacket.Trade: TradeHandler.Trade(data); break;
            case ServerPacket.TradeInvitation: TradeHandler.TradeInvitation(data); break;
            case ServerPacket.TradeState: TradeHandler.TradeState(data); break;
            case ServerPacket.TradeOffer: TradeHandler.TradeOffer(data); break;
            case ServerPacket.Shops: ShopHandler.Shops(data); break;
            case ServerPacket.ShopOpen: ShopHandler.ShopOpen(data); break;
        }
    }
}
