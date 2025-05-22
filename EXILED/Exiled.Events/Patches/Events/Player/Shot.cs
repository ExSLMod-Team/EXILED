// -----------------------------------------------------------------------
// <copyright file="Shot.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Attributes;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;
    using InventorySystem.Items.Firearms.Modules;
    using InventorySystem.Items.Firearms.Modules.Misc;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="HitscanHitregModuleBase.ServerApplyDamage" />.
    /// Adds the <see cref="Handlers.Player.Shot" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.Shot))]
    [HarmonyPatch(typeof(HitscanHitregModuleBase), nameof(HitscanHitregModuleBase.ServerApplyDamage))]
    internal static class Shot
    {
        // public virtual void ServerApplyDamage(HitscanResult result)
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            CodeMatcher matcher = new(instructions);
            matcher.Start();

            LocalBuilder eventArgs = il.DeclareLocal(typeof(ShotEventArgs));

            /* Insert event call
            ShotEventArgs args = new ShotEventArgs(result, this);
            Handlers.Player.OnShot(args);*/
            matcher.Insert(new List<CodeInstruction>
            {
                // ShotEventArgs args = new ShotEventArgs(result, this);
                new(OpCodes.Ldarg_1), // result
                new(OpCodes.Ldarg_0), // this
                new(OpCodes.Newobj, Constructor(typeof(ShotEventArgs), new[] { typeof(HitscanResult), typeof(HitscanHitregModuleBase) })),
                new(OpCodes.Stloc, eventArgs),

                // Handlers.Player.OnShot(args);
                new(OpCodes.Ldloc, eventArgs),
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnShot))),
            }).Advance(6);

            /* Insert the if's:

            if (args.CanDamageDestructibles)
            {
                foreach (DestructibleHitPair destructible in result.Destructibles)
                    __instance.ServerApplyDestructibleDamage(destructible, result);
            }

            if (args.CanDamageObstacles)
            {
                foreach (HitRayPair obstacle in result.Obstacles)
                    __instance.ServerApplyObstacleDamage(obstacle, result);
            }*/

            bool LoadsShotResult(CodeInstruction i) => i.IsLdarg(1);

            // Labels for skipping loops
            Label skipDestructiblesLabel = il.DefineLabel();
            Label skipObstaclesLabel = il.DefineLabel();

            matcher
                .SearchForward(LoadsShotResult)
                .Insert(new List<CodeInstruction>
                {
                    new(OpCodes.Ldloc, eventArgs),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ShotEventArgs), nameof(ShotEventArgs.CanDamageDestructibles))),
                    new(OpCodes.Brfalse, skipDestructiblesLabel),
                })
                .SearchForward(LoadsShotResult).Advance(1) // Skip result inside of foreach

                .SearchForward(LoadsShotResult)
                .Insert(new List<CodeInstruction>
                {
                    new(OpCodes.Ldloc, eventArgs),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ShotEventArgs), nameof(ShotEventArgs.CanDamageObstacles))),
                    new(OpCodes.Brfalse, skipObstaclesLabel),
                }).AddLabels(new[] { skipDestructiblesLabel })
                .SearchForward(LoadsShotResult).Advance(1) // Skip result inside of foreach

                .SearchForward(i => i.IsLdarg(0)) // Go before the last call
                .AddLabels(new[] { skipObstaclesLabel });

            return matcher.Instructions();
        }
    }
}
