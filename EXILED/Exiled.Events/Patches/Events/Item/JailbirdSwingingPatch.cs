// -----------------------------------------------------------------------
// <copyright file="JailbirdSwingingPatch.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Item
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Item;
    using HarmonyLib;
    using InventorySystem.Items.Jailbird;
    using Mirror;
    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="JailbirdItem.ServerProcessCmd(NetworkReader)" />.
    /// Adds the <see cref="Handlers.Item.Swinging" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Item), nameof(Handlers.Item.Swinging))]
    [HarmonyPatch(typeof(JailbirdItem), nameof(JailbirdItem.ServerProcessCmd))]
    internal static class JailbirdSwingingPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(SwingingJailbirdEventArgs));

            const int offset = -2;
            int index = newInstructions.FindIndex(i => i.StoresField(Field(typeof(JailbirdItem), nameof(JailbirdItem._attackTriggered)))) + offset;

            List<Label> labels = newInstructions[index].labels;

            // remove "this._attackTriggered = true"
            newInstructions.RemoveRange(index, 3);

            newInstructions[index].WithLabels(labels);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // ev = new SwingingEventArgs(this.Owner, this, true)
                new (OpCodes.Ldarg_0),
                new (OpCodes.Callvirt, PropertyGetter(typeof(JailbirdItem), nameof(JailbirdItem.Owner))),
                new (OpCodes.Ldarg_0),
                new (OpCodes.Ldc_I4_1),
                new (OpCodes.Newobj, GetDeclaredConstructors(typeof(SwingingJailbirdEventArgs))[0]),
                new (OpCodes.Dup),
                new (OpCodes.Stloc_S, ev),

                // Handlers.Item.OnSwinging(ev)
                new (OpCodes.Call, Method(typeof(Handlers.Item), nameof(Handlers.Item.OnSwinging))),

                // this._attackTriggered = ev.CanHurt
                new (OpCodes.Ldarg_0),
                new (OpCodes.Ldloc_S, ev),
                new (OpCodes.Callvirt, PropertyGetter(typeof(SwingingJailbirdEventArgs), nameof(SwingingJailbirdEventArgs.CanHurt))),
                new (OpCodes.Stfld, Field(typeof(JailbirdItem), nameof(JailbirdItem._attackTriggered))),
            });

            foreach (CodeInstruction instruction in newInstructions)
                yield return instruction;

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}