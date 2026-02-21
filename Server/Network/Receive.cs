using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Packets.Client;
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
            case ClientPacket.Connect: AuthHandler.Connect(account, (ConnectPacket)data.ReadObject()); break;
            case ClientPacket.Latency: AuthHandler.Latency(account, (LatencyPacket)data.ReadObject()); break;
            case ClientPacket.Register: AuthHandler.Register(account, (RegisterPacket)data.ReadObject()); break;

            case ClientPacket.CreateCharacter:
                AccountHandler.CreateCharacter(account, (CreateCharacterPacket)data.ReadObject()); break;
            case ClientPacket.CharacterUse:
                AccountHandler.CharacterUse(account, (CharacterUsePacket)data.ReadObject()); break;
            case ClientPacket.CharacterCreate:
                AccountHandler.CharacterCreate(account, (CharacterCreatePacket)data.ReadObject()); break;
            case ClientPacket.CharacterDelete:
                AccountHandler.CharacterDelete(account, (CharacterDeletePacket)data.ReadObject()); break;

            case ClientPacket.PlayerDirection:
                PlayerHandler.PlayerDirection(player, (PlayerDirectionPacket)data.ReadObject()); break;
            case ClientPacket.PlayerMove: PlayerHandler.PlayerMove(player, (PlayerMovePacket)data.ReadObject()); break;
            case ClientPacket.PlayerAttack:
                PlayerHandler.PlayerAttack(player);
                data.ReadObject();
                break;
            case ClientPacket.AddPoint: PlayerHandler.AddPoint(player, (AddPointPacket)data.ReadObject()); break;
            case ClientPacket.CollectItem:
                PlayerHandler.CollectItem(player);
                data.ReadObject();
                break;
            case ClientPacket.DropItem: PlayerHandler.DropItem(player, (DropItemPacket)data.ReadObject()); break;
            case ClientPacket.InventoryChange:
                PlayerHandler.InventoryChange(player, (InventoryChangePacket)data.ReadObject()); break;
            case ClientPacket.InventoryUse:
                PlayerHandler.InventoryUse(player, (InventoryUsePacket)data.ReadObject()); break;
            case ClientPacket.EquipmentRemove:
                PlayerHandler.EquipmentRemove(player, (EquipmentRemovePacket)data.ReadObject()); break;
            case ClientPacket.HotbarAdd: PlayerHandler.HotbarAdd(player, (HotbarAddPacket)data.ReadObject()); break;
            case ClientPacket.HotbarChange:
                PlayerHandler.HotbarChange(player, (HotbarChangePacket)data.ReadObject()); break;
            case ClientPacket.HotbarUse: PlayerHandler.HotbarUse(player, (HotbarUsePacket)data.ReadObject()); break;

            case ClientPacket.Message: ChatHandler.Message(player, (MessagePacket)data.ReadObject()); break;

            case ClientPacket.PartyInvite:
                PartyHandler.PartyInvite(player, (PartyInvitePacket)data.ReadObject()); break;
            case ClientPacket.PartyAccept:
                PartyHandler.PartyAccept(player);
                data.ReadObject();
                break;
            case ClientPacket.PartyDecline:
                PartyHandler.PartyDecline(player);
                data.ReadObject();
                break;
            case ClientPacket.PartyLeave:
                PartyHandler.PartyLeave(player);
                data.ReadObject();
                break;

            case ClientPacket.TradeInvite:
                TradeHandler.TradeInvite(player, (TradeInvitePacket)data.ReadObject()); break;
            case ClientPacket.TradeAccept:
                TradeHandler.TradeAccept(player);
                data.ReadObject();
                break;
            case ClientPacket.TradeDecline:
                TradeHandler.TradeDecline(player);
                data.ReadObject();
                break;
            case ClientPacket.TradeLeave:
                TradeHandler.TradeLeave(player);
                data.ReadObject();
                break;
            case ClientPacket.TradeOffer: TradeHandler.TradeOffer(player, (TradeOfferPacket)data.ReadObject()); break;
            case ClientPacket.TradeOfferState:
                TradeHandler.TradeOfferState(player, (TradeOfferStatePacket)data.ReadObject()); break;

            case ClientPacket.ShopBuy: ShopHandler.ShopBuy(player, (ShopBuyPacket)data.ReadObject()); break;
            case ClientPacket.ShopSell: ShopHandler.ShopSell(player, (ShopSellPacket)data.ReadObject()); break;
            case ClientPacket.ShopClose:
                ShopHandler.ShopClose(player);
                data.ReadObject();
                break;

            case ClientPacket.WriteSettings:
                EditorHandler.WriteSettings(account, (WriteSettingsPacket)data.ReadObject()); break;
            case ClientPacket.WriteClasses:
                EditorHandler.WriteClasses(account, (WriteClassesPacket)data.ReadObject()); break;
            case ClientPacket.WriteMaps: EditorHandler.WriteMaps(account, (WriteMapsPacket)data.ReadObject()); break;
            case ClientPacket.WriteNpcs: EditorHandler.WriteNpcs(account, (WriteNpcsPacket)data.ReadObject()); break;
            case ClientPacket.WriteItems: EditorHandler.WriteItems(account, (WriteItemsPacket)data.ReadObject()); break;
            case ClientPacket.WriteShops: EditorHandler.WriteShops(account, (WriteShopsPacket)data.ReadObject()); break;

            case ClientPacket.RequestSetting:
                EditorHandler.RequestSetting(account);
                data.ReadObject();
                break;
            case ClientPacket.RequestClasses:
                EditorHandler.RequestClasses(account);
                data.ReadObject();
                break;
            case ClientPacket.RequestMap: EditorHandler.RequestMap(account, (RequestMapPacket)data.ReadObject()); break;
            case ClientPacket.RequestMaps:
                EditorHandler.RequestMaps(account);
                data.ReadObject();
                break;
            case ClientPacket.RequestNpcs:
                EditorHandler.RequestNpcs(account);
                data.ReadObject();
                break;
            case ClientPacket.RequestItems:
                EditorHandler.RequestItems(account);
                data.ReadObject();
                break;
            case ClientPacket.RequestShops:
                EditorHandler.RequestShops(account);
                data.ReadObject();
                break;
        }
    }
}
