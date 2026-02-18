using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;

namespace CryBits.Server.Systems;

/// <summary>
/// Request-driven system that owns all trade lifecycle logic.
/// </summary>
internal static class TradeSystem
{
    /// <summary>
    /// Cancels the active trade for <paramref name="player"/>, notifying both sides.
    /// No-ops if the player has no active trade.
    /// </summary>
    public static void Leave(Player player)
    {
        if (player.Trade == null) return;

        player.Trade.Trade = null;
        TradeSender.Trade(player.Trade, false);
        player.Trade = null;
        TradeSender.Trade(player, false);
    }
}
