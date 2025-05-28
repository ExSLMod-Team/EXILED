// -----------------------------------------------------------------------
// <copyright file="ChaosKeycard.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Snake
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Attributes;
    using Exiled.API.Features.Items;
    using HarmonyLib;
    using InventorySystem.Items.Keycards;
    using InventorySystem.Items.Keycards.Snake;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="ChaosKeycardItem.ServerProcessCustomCmd" />.
    /// </summary>
    [EventPatch(typeof(Handlers.Snake), nameof(Handlers.Snake.GainedScore))]
    [EventPatch(typeof(Handlers.Snake), nameof(Handlers.Snake.GameOver))]
    [EventPatch(typeof(Handlers.Snake), nameof(Handlers.Snake.NewGame))]
    [HarmonyPatch(typeof(ChaosKeycardItem), nameof(ChaosKeycardItem.ServerProcessCustomCmd))]
    internal static class ChaosKeycard
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatcher matcher = new CodeMatcher(instructions)
                .MatchEndForward(new CodeMatch(OpCodes.Ldc_I4_1))
                .Advance(-1)
                .Insert(
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldloc_1),
                    new(OpCodes.Call, Method(typeof(ChaosKeycard), nameof(OnCardUse))));

            return matcher.InstructionEnumeration();
        }

        private static void OnCardUse(ChaosKeycardItem itemBase, SnakeNetworkMessage message)
        {
            Keycard item = Item.Get<Keycard>(itemBase);
            API.Features.Player player = item.Owner;

            if (!ChaosKeycardItem.SnakeSessions.TryGetValue(item.Serial, out SnakeEngine engine))
                return;

            bool isNewGame = message.HasFlag(SnakeNetworkMessage.SyncFlags.GameReset);

            if(message.HasFlag(SnakeNetworkMessage.SyncFlags.HasNewFood) && !isNewGame)
                Handlers.Snake.OnGainedScore(new (engine, player, item));
            else if (message.HasFlag(SnakeNetworkMessage.SyncFlags.GameOver))
                Handlers.Snake.OnGameOver(new(engine, player, item));
            else if (isNewGame)
                Handlers.Snake.OnNewGame(new(engine, player, item));
        }
    }
}