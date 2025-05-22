// -----------------------------------------------------------------------
// <copyright file="Shot.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System;

    using Exiled.API.Features;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;
    using InventorySystem.Items.Firearms.Modules;
    using InventorySystem.Items.Firearms.Modules.Misc;

    /// <summary>
    /// Patches <see cref="HitscanHitregModuleBase.ServerApplyDamage" />.
    /// Adds the <see cref="Handlers.Player.Shot" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.Shot))]
    [HarmonyPatch(typeof(HitscanHitregModuleBase), nameof(HitscanHitregModuleBase.ServerApplyDamage))]
    internal static class Shot
    {
        // public virtual void ServerApplyDamage(HitscanResult result)
#pragma warning disable SA1313
        private static bool Prefix(HitscanResult result, HitscanHitregModuleBase __instance)
#pragma warning restore SA1313
        {
            ShotEventArgs args = new(result, __instance);
            Handlers.Player.OnShot(args);

            try
            {
                if (args.CanDamageDestructibles)
                {
                    foreach (DestructibleHitPair destructible in result.Destructibles)
                        __instance.ServerApplyDestructibleDamage(destructible, result);
                }

                if (args.CanDamageObstacles)
                {
                    foreach (HitRayPair obstacle in result.Obstacles)
                        __instance.ServerApplyObstacleDamage(obstacle, result);
                }

                __instance.ServerSendAllIndicators(result);
            }
            catch (Exception e)
            {
                Log.Error($"Error in {nameof(Shot)} event - invalud modification of ev.{nameof(ShotEventArgs.ShotResult)} has caused an exception: {e}");
            }

            return false;
        }
    }
}
