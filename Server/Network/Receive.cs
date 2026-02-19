using CryBits.Enums;
using CryBits.Server.Entities;
using CryBits.Server.Network.Handlers;
using LiteNetLib;

namespace CryBits.Server.Network;

internal static class Receive
{
    public static void Handle(Account account, NetPacketReader data)
    {
        var player = account.Character;

        // Handle incoming packets by delegating to focused handlers
        switch ((ClientPacket)data.GetByte())
        {
            case ClientPacket.Connect: AuthHandler.Connect(account, data); break;
            case ClientPacket.Latency: AuthHandler.Latency(account); break;
            case ClientPacket.Register: AuthHandler.Register(account, data); break;

            case ClientPacket.CreateCharacter: AccountHandler.CreateCharacter(account, data); break;
            case ClientPacket.CharacterUse: AccountHandler.CharacterUse(account, data); break;
            case ClientPacket.CharacterCreate: AccountHandler.CharacterCreate(account); break;
            case ClientPacket.CharacterDelete: AccountHandler.CharacterDelete(account, data); break;

            case ClientPacket.PlayerDirection: PlayerHandler.PlayerDirection(player, data); break;
            case ClientPacket.PlayerMove: PlayerHandler.PlayerMove(player, data); break;
            case ClientPacket.PlayerAttack: PlayerHandler.PlayerAttack(player); break;
            case ClientPacket.AddPoint: PlayerHandler.AddPoint(player, data); break;
            case ClientPacket.CollectItem: PlayerHandler.CollectItem(player); break;
            case ClientPacket.DropItem: PlayerHandler.DropItem(player, data); break;
            case ClientPacket.InventoryChange: PlayerHandler.InventoryChange(player, data); break;
            case ClientPacket.InventoryUse: PlayerHandler.InventoryUse(player, data); break;
            case ClientPacket.EquipmentRemove: PlayerHandler.EquipmentRemove(player, data); break;
            case ClientPacket.HotbarAdd: PlayerHandler.HotbarAdd(player, data); break;
            case ClientPacket.HotbarChange: PlayerHandler.HotbarChange(player, data); break;
            case ClientPacket.HotbarUse: PlayerHandler.HotbarUse(player, data); break;

            case ClientPacket.Message: ChatHandler.Message(player, data); break;

            case ClientPacket.PartyInvite: PartyHandler.PartyInvite(player, data); break;
            case ClientPacket.PartyAccept: PartyHandler.PartyAccept(player); break;
            case ClientPacket.PartyDecline: PartyHandler.PartyDecline(player); break;
            case ClientPacket.PartyLeave: PartyHandler.PartyLeave(player); break;

            case ClientPacket.TradeInvite: TradeHandler.TradeInvite(player, data); break;
            case ClientPacket.TradeAccept: TradeHandler.TradeAccept(player); break;
            case ClientPacket.TradeDecline: TradeHandler.TradeDecline(player); break;
            case ClientPacket.TradeLeave: TradeHandler.TradeLeave(player); break;
            case ClientPacket.TradeOffer: TradeHandler.TradeOffer(player, data); break;
            case ClientPacket.TradeOfferState: TradeHandler.TradeOfferState(player, data); break;

            case ClientPacket.ShopBuy: ShopHandler.ShopBuy(player, data); break;
            case ClientPacket.ShopSell: ShopHandler.ShopSell(player, data); break;
            case ClientPacket.ShopClose: ShopHandler.ShopClose(player); break;

            case ClientPacket.WriteSettings: EditorHandler.WriteSettings(account, data); break;
            case ClientPacket.WriteClasses: EditorHandler.WriteClasses(account, data); break;
            case ClientPacket.WriteMaps: EditorHandler.WriteMaps(account, data); break;
            case ClientPacket.WriteNpcs: EditorHandler.WriteNpcs(account, data); break;
            case ClientPacket.WriteItems: EditorHandler.WriteItems(account, data); break;
            case ClientPacket.WriteShops: EditorHandler.WriteShops(account, data); break;

            case ClientPacket.RequestSetting: EditorHandler.RequestSetting(account); break;
            case ClientPacket.RequestClasses: EditorHandler.RequestClasses(account); break;
            case ClientPacket.RequestMap: EditorHandler.RequestMap(account, data); break;
            case ClientPacket.RequestMaps: EditorHandler.RequestMaps(account); break;
            case ClientPacket.RequestNpcs: EditorHandler.RequestNpcs(account); break;
            case ClientPacket.RequestItems: EditorHandler.RequestItems(account); break;
            case ClientPacket.RequestShops: EditorHandler.RequestShops(account); break;
        }
    }
}
