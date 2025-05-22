// -----------------------------------------------------------------------
// <copyright file="Observers.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp173
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using Attributes;
    using Exiled.API.Features.Pools;
    using Exiled.Events.EventArgs.Scp173;
    using HarmonyLib;
    using PlayerRoles.PlayableScps.Scp173;
    using PlayerRoles.Subroutines;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp173ObserversTracker.UpdateObserver(ReferenceHub)" />.
    /// Adds the <see cref="Handlers.Scp173.AddingObserver" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp173), nameof(Handlers.Scp173.AddingObserver))]
    [EventPatch(typeof(Handlers.Scp173), nameof(Handlers.Scp173.RemoveObserver))]
    [HarmonyPatch(typeof(Scp173ObserversTracker), nameof(Scp173ObserversTracker.UpdateObserver))]
    internal static class Observers
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            // AddingObserver patch
            Label returnLabel = generator.DefineLabel();
            LocalBuilder ev = generator.DeclareLocal(typeof(AddingObserverEventArgs));

            int index = newInstructions.FindIndex(x => x.Calls(Method(typeof(HashSet<ReferenceHub>), nameof(HashSet<ReferenceHub>.Add)))) + 2;
            newInstructions[index].labels.Add(returnLabel);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // Player.Get(Owner);
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(StandardSubroutine<Scp173Role>), nameof(StandardSubroutine<Scp173Role>.Owner))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // Player.Get(ply);
                new(OpCodes.Ldarg_1),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // true
                new(OpCodes.Ldc_I4_1),

                // AddingObserverEventArgs ev = new(Player.Get(Owner), Player.Get(ply), true);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(AddingObserverEventArgs))[0]),
                new(OpCodes.Stloc, ev),

                // Scp173.OnAddingObserver(ev);
                new(OpCodes.Ldloc, ev),
                new(OpCodes.Call, Method(typeof(Handlers.Scp173), nameof(Handlers.Scp173.OnAddingObserver))),

                // if (!ev.IsAllowed)
                new(OpCodes.Ldloc, ev),
                new(OpCodes.Callvirt,
                    PropertyGetter(typeof(AddingObserverEventArgs), nameof(AddingObserverEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, returnLabel),

                // Observers.Remove(ply);
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(Scp173ObserversTracker), nameof(Scp173ObserversTracker.Observers))),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Callvirt, Method(typeof(HashSet<ReferenceHub>), nameof(HashSet<ReferenceHub>.Remove))),
                new(OpCodes.Pop),

                // return 0;
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Ret),
            });

            // RemoveObserver patch
            int index2 = newInstructions.FindLastIndex(x => x.Calls(Method(typeof(HashSet<ReferenceHub>), nameof(HashSet<ReferenceHub>.Remove)))) + 2;

            LocalBuilder ev2 = generator.DeclareLocal(typeof(RemoveObserverEventArgs));

            newInstructions.InsertRange(index2, new CodeInstruction[]
            {
                // Player.Get(Owner);
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt,
                    PropertyGetter(typeof(StandardSubroutine<Scp173Role>), nameof(StandardSubroutine<Scp173Role>.Owner))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // Player.Get(ply);
                new(OpCodes.Ldarg_1),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // new RemoveObserverEventArgs(Player.Get(Owner), Player.Get(ply));
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(RemoveObserverEventArgs))[0]),
                new(OpCodes.Stloc, ev2),

                // Scp173.OnRemovingObserver(new RemoveObserverEventArgs(Player.Get(Owner), Player.Get(ply)));
                new(OpCodes.Ldloc, ev2),
                new(OpCodes.Call, Method(typeof(Handlers.Scp173), nameof(Handlers.Scp173.OnRemoveObserver))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
            {
                yield return newInstructions[z];
            }

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}