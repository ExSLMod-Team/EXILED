// -----------------------------------------------------------------------
// <copyright file="JailbirdChargingPatch.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Item
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection.Emit;

    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Item;
    using HarmonyLib;
    using InventorySystem.Items.Jailbird;
    using Mirror;
    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    using Item = Exiled.API.Features.Items.Item;
    using Player = Exiled.API.Features.Player;

        /// <summary>
    /// Patches <see cref="JailbirdItem.ServerProcessCmd(NetworkReader)" />.
    /// Adds the <see cref="Handlers.Item.ChargingJailbird" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Item), nameof(Handlers.Item.ChargingJailbird))]
    [HarmonyPatch(typeof(JailbirdItem), nameof(JailbirdItem.ServerProcessCmd))]
    internal static class JailbirdChargingPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(ChargingJailbirdEventArgs));

            Label continueLabel = generator.DefineLabel();

            const int offset = -2;
            int index = newInstructions.FindIndex(i => i.Calls(Method(typeof(Stopwatch), nameof(Stopwatch.Start)))) + offset;

            List<Label> labels = newInstructions[index].ExtractLabels();

            newInstructions[index].WithLabels(continueLabel);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // ev = new ChargingJailbirdEventArgs(this.Owner, this, true)
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(labels),
                new (OpCodes.Callvirt, PropertyGetter(typeof(JailbirdItem), nameof(JailbirdItem.Owner))),
                new (OpCodes.Ldarg_0),
                new (OpCodes.Ldc_I4_1),
                new (OpCodes.Newobj, GetDeclaredConstructors(typeof(ChargingJailbirdEventArgs))[0]),
                new (OpCodes.Dup),
                new (OpCodes.Dup),
                new (OpCodes.Stloc_S, ev),

                // Handlers.Item.OnChargingJailbird(ev)
                new (OpCodes.Call, Method(typeof(Handlers.Item), nameof(Handlers.Item.OnChargingJailbird))),

                // if (ev.IsAllowed) goto continueLabel
                new (OpCodes.Callvirt, PropertyGetter(typeof(ChargingJailbirdEventArgs), nameof(ChargingJailbirdEventArgs.IsAllowed))),
                new (OpCodes.Brtrue_S, continueLabel),

                // this.SendRpc(JailbirdMessageType.ChargeFailed, null)
                new (OpCodes.Ldarg_0),
                new (OpCodes.Ldc_I4_S, (sbyte)JailbirdMessageType.ChargeFailed),
                new (OpCodes.Ldnull),
                new (OpCodes.Call, Method(typeof(JailbirdItem), nameof(JailbirdItem.SendRpc))),

                // ev.Player.RemoveItem(ev.Item, false)
                new (OpCodes.Ldloc_S, ev),
                new (OpCodes.Callvirt, PropertyGetter(typeof(ChargingJailbirdEventArgs), nameof(ChargingJailbirdEventArgs.Player))),
                new (OpCodes.Ldloc_S, ev),
                new (OpCodes.Callvirt, PropertyGetter(typeof(ChargingJailbirdEventArgs), nameof(ChargingJailbirdEventArgs.Item))),
                new (OpCodes.Ldc_I4_0),
                new (OpCodes.Callvirt, Method(typeof(Player), nameof(Player.RemoveItem), new[] { typeof(Item), typeof(bool) })),
                new (OpCodes.Pop),

                // ev.Player.AddItem(ev.Item)
                new (OpCodes.Ldloc_S, ev),
                new (OpCodes.Callvirt, PropertyGetter(typeof(JailbirdChargeCompleteEventArgs), nameof(JailbirdChargeCompleteEventArgs.Player))),
                new (OpCodes.Ldloc_S, ev),
                new (OpCodes.Callvirt, PropertyGetter(typeof(JailbirdChargeCompleteEventArgs), nameof(JailbirdChargeCompleteEventArgs.Item))),
                new (OpCodes.Call, Method(typeof(Player), nameof(Player.AddItem), new[] { typeof(Item) })),

                // return
                new (OpCodes.Ret),

                // continueLabel:
            });

            foreach (CodeInstruction instruction in newInstructions)
                yield return instruction;

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}