// -----------------------------------------------------------------------
// <copyright file="SpawningItem.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Doors;
    using API.Features.Pickups;
    using API.Features.Pools;

    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Map;
    using Handlers;
    using HarmonyLib;
    using Interactables.Interobjects.DoorUtils;
    using MapGeneration.Distributors;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="ItemDistributor.ServerRegisterPickup" />.
    /// Adds the <see cref="Map.SpawningItem" /> event.
    /// </summary>
    [EventPatch(typeof(Map), nameof(Map.SpawningItem))]
    [HarmonyPatch(typeof(ItemDistributor), nameof(ItemDistributor.ServerRegisterPickup))]
    internal static class SpawningItem
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder door = generator.DeclareLocal(typeof(DoorVariant));
            LocalBuilder initiallySpawn = generator.DeclareLocal(typeof(bool));
            LocalBuilder ev = generator.DeclareLocal(typeof(SpawningItemEventArgs));

            Label skip = generator.DefineLabel();
            Label skipdoorSpawn = generator.DefineLabel();
            Label doorSpawn = generator.DefineLabel();
            Label returnLabel = generator.DefineLabel();

            int lastIndex = newInstructions.FindLastIndex(instruction => instruction.IsLdarg(0));

            int offset = -1;
            int index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(ItemDistributor), nameof(ItemDistributor.SpawnPickup)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // initiallySpawn = true
                    new CodeInstruction(OpCodes.Ldc_I4_1).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Stloc_S, initiallySpawn.LocalIndex),

                    // door = null
                    new(OpCodes.Ldnull),
                    new(OpCodes.Stloc_S, door.LocalIndex),

                    // goto skip
                    new(OpCodes.Br_S, skip),

                    // initiallySpawn = false
                    new CodeInstruction(OpCodes.Ldc_I4_0).MoveLabelsFrom(newInstructions[lastIndex]),
                    new(OpCodes.Stloc_S, initiallySpawn.LocalIndex),

                    // door = doorNametagExtension.TargetDoor
                    new(OpCodes.Ldloc_2),
                    new(OpCodes.Ldfld, Field(typeof(DoorVariantExtension), nameof(DoorVariantExtension.TargetDoor))),
                    new(OpCodes.Stloc_S, door.LocalIndex),

                    // ipb
                    new CodeInstruction(OpCodes.Ldloc_1).WithLabels(skip),

                    // initiallySpawn
                    new(OpCodes.Ldloc_S, initiallySpawn.LocalIndex),

                    // door
                    new(OpCodes.Ldloc_S, door.LocalIndex),

                    // SpawningItemEventArgs ev = new(ItemPickupBase, initiallySpawn, door)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SpawningItemEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Map.OnSpawningItem(ev)
                    new(OpCodes.Call, Method(typeof(Map), nameof(Map.OnSpawningItem))),

                    // if (!ev.IsAllowed)
                    //     return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SpawningItemEventArgs), nameof(SpawningItemEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),

                    // if (!ev.ShouldInitiallySpawn && ev.Door != null)
                    //     goto doorSpawn
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SpawningItemEventArgs), nameof(SpawningItemEventArgs.ShouldInitiallySpawn))),
                    new(OpCodes.Brtrue_S, skipdoorSpawn),

                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SpawningItemEventArgs), nameof(SpawningItemEventArgs.TriggerDoor))),
                    new(OpCodes.Brtrue_S, doorSpawn),

                    new CodeInstruction(OpCodes.Nop).WithLabels(skipdoorSpawn),
                });

            lastIndex = newInstructions.FindLastIndex(instruction => instruction.IsLdarg(0));

            newInstructions[lastIndex].labels.Add(doorSpawn);

            // Replace
            // "base.RegisterUnspawnedObject(doorNametagExtension.TargetDoor, itemPickupBase.gameObject);"
            // with "base.RegisterUnspawnedObject(ev.Door.Base, itemPickupBase.gameObject);"
            offset = -1;
            index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Ldfld) + offset;

            newInstructions.RemoveRange(index, 2);

            newInstructions.InsertRange(index, new[]
            {
                // ev.Door.Base
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(SpawningItemEventArgs), nameof(SpawningItemEventArgs.TriggerDoor))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Door), nameof(Door.Base))),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}