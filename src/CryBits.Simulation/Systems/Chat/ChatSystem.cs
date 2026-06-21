using CryBits.Definitions.Common;
using CryBits.Definitions.Utils;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Events;
using CryBits.Simulation.Intents;

namespace CryBits.Simulation.Systems.Chat;

public sealed class ChatSystem : ISimulationSystem
{
    public void Execute(World world, Tick tick)
    {
        foreach (var intent in tick.Intents.All)
        {
            if (intent is not ChatMessageIntent chat) continue;

            if (chat.Text.Any(c => c < 32 || c > 126))
                continue;

            var entity = world.Entities.Get(chat.SourceEntityId);
            if (entity == null) continue;

            var appearance = entity.Get<PlayerAppearance>();
            if (appearance == null) continue;

            var formatted = appearance.Name + ": " + chat.Text;

            switch (chat.Type)
            {
                case Message.Global:
                    foreach (var state in world.Entities.All)
                    {
                        if (!state.Has<PlayerTag>()) continue;
                        tick.Events.Emit(new ChatMessageEvent
                        {
                            RecipientId = state.Id,
                            Text = formatted,
                            ColorArgb = ChatColors.White
                        });
                    }
                    break;

                case Message.Map:
                    var sourcePos = entity.Get<Position>();
                    if (sourcePos == null) break;

                    foreach (var state in world.Entities.All)
                    {
                        if (!state.Has<PlayerTag>()) continue;
                        var pos = state.Get<Position>();
                        if (pos == null || pos.MapId != sourcePos.MapId) continue;
                        tick.Events.Emit(new ChatMessageEvent
                        {
                            RecipientId = state.Id,
                            Text = formatted,
                            ColorArgb = ChatColors.White
                        });
                    }
                    break;

                case Message.Private:
                    if (chat.Addressee == null) break;
                    var targetId = world.FindPlayer(chat.Addressee);
                    if (targetId == null) break;

                    tick.Events.Emit(new ChatMessageEvent
                    {
                        RecipientId = chat.SourceEntityId,
                        Text = "To " + chat.Addressee + ": " + chat.Text,
                        ColorArgb = ChatColors.White
                    });
                    tick.Events.Emit(new ChatMessageEvent
                    {
                        RecipientId = targetId.Value,
                        Text = "From " + appearance.Name + ": " + chat.Text,
                        ColorArgb = ChatColors.White
                    });
                    break;

                case Message.Local:
                    tick.Events.Emit(new ChatMessageEvent
                    {
                        RecipientId = chat.SourceEntityId,
                        Text = formatted,
                        ColorArgb = ChatColors.White
                    });
                    break;
            }
        }
    }
}
