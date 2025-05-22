// -----------------------------------------------------------------------
// <copyright file="ShotEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using System;

    using API.Features;
    using Exiled.API.Features.Items;
    using Interfaces;
    using InventorySystem.Items.Firearms.Modules;
    using InventorySystem.Items.Firearms.Modules.Misc;
    using UnityEngine;

    /// <summary>
    /// Contains all information after a player has fired a weapon.
    /// </summary>
    public class ShotEventArgs : IFirearmEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShotEventArgs"/> class.
        /// </summary>
        /// <param name="hitscanResult">Hitscan result that contains all hit information.</param>
        /// <param name="hitregModule">Hitreg module that calculated the shot.</param>
        public ShotEventArgs(HitscanResult hitscanResult, HitscanHitregModuleBase hitregModule)
        {
            HitregModule = hitregModule;
            ShotResult = hitscanResult;
            Firearm = Item.Get<Firearm>(HitregModule.Firearm);
            Player = Firearm.Owner;

            if (ShotResult.Destructibles.Count != 0)
            {
                DestructibleHitPair firstPair = ShotResult.Destructibles[0];
                RaycastHit = firstPair.Hit;
                Destructible = firstPair.Destructible;
                Hitbox = Destructible as HitboxIdentity;
                Target = Player.Get(Hitbox?.TargetHub);
            }
            else
            {
                if (ShotResult.Obstacles.Count > 0)
                    RaycastHit = ShotResult.Obstacles[0].Hit;
                else
                    RaycastHit = ApproximateHit();
            }

            foreach (DestructibleDamageRecord damageRecord in ShotResult.DamagedDestructibles)
            {
                Damage += damageRecord.AppliedDamage;
            }
        }

        /// <summary>
        /// Gets the HitscanResult object which represents the result of weapon shot raycasts and calculations.
        /// Each weapon controls how it is generated to determine the specific behavior. You can change that behaviour by modifying that object.
        /// </summary>
        /// <seealso cref="ResetShotResult"/>
        public HitscanResult ShotResult { get; }

        /// <summary>
        /// Gets the player who fired the shot.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the firearm used to fire the shot.
        /// </summary>
        public Firearm Firearm { get; }

        /// <inheritdoc/>
        public Item Item => Firearm;

        /// <summary>
        /// Gets the firearm hitreg module responsible for the shot.
        /// </summary>
        public HitscanHitregModuleBase HitregModule { get; }

        /// <summary>
        /// Gets the hit info of the first hit.
        /// </summary>
        public RaycastHit RaycastHit { get; }

        /// <summary>
        /// Gets the bullet travel distance of the first bullet.
        /// </summary>
        public float Distance => RaycastHit.distance;

        /// <summary>
        /// Gets the position of the first hit.
        /// </summary>
        public Vector3 Position => RaycastHit.point;

        /// <summary>
        /// Gets the direction of the first bullet.
        /// </summary>
        public Vector3 Direction => Position - Player.CameraTransform.position;

        /// <summary>
        /// Gets or sets a value indicating whether the shot can deal damage to <see cref="IDestructible"/> objects such as players or glass windows.
        /// Damage can be controlled on per instance basis using <see cref="ShotResult"/>.
        /// </summary>
        public bool CanDamageDestructibles { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the shot can deal damage to obstacles such as walls - spawning impact effects, or doors - breaking them in case of <see cref="ItemType.ParticleDisruptor"/>.
        /// Damage can be controlled on per instance basis using <see cref="ShotResult"/>.
        /// </summary>
        public bool CanDamageObstacles { get; set; } = true;

        /// <summary>
        /// Gets the sum of the damage that is going to be dealt by the shot if <see cref="ShotResult"/> remains unchanged.
        /// </summary>
        public float Damage { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the shot can produce impact effects (e.g. bullet holes).
        /// </summary>
        [Obsolete("Use CanDamageObstacles instead.")]
        public bool CanSpawnImpactEffects { get => CanDamageObstacles; set => CanDamageObstacles = value; }

        /// <summary>
        /// Gets or sets a value indicating whether the shot can deal damage.
        /// </summary>
        [Obsolete("Use CanDamageDestructibles instead.")]
        public bool CanHurt { get => CanDamageDestructibles; set => CanDamageDestructibles = value; }

        /// <summary>
        /// Gets the <see cref="IDestructible" /> component of the first hit collider. Can be null.
        /// </summary>
        public IDestructible Destructible { get; }

        /// <summary>
        /// Gets the <see cref="HitboxIdentity" /> component of the first target player that was hit. Can be null.
        /// </summary>
        public HitboxIdentity Hitbox { get; }

        /// <summary>
        /// Gets the first target player. Can be null.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        /// Reset the shot result generated by the firearm, allowing you to generate your own result.
        /// </summary>
        public void ResetShotResult()
        {
            ShotResult.Clear();
        }

        private RaycastHit ApproximateHit()
        {
            Ray ray = new(Player.CameraTransform.position, Player.CameraTransform.forward);
            float maxDistance = HitregModule.DamageFalloffDistance + HitregModule.FullDamageDistance;
            return new RaycastHit
            {
                distance = maxDistance,
                point = ray.GetPoint(maxDistance),
                normal = -ray.direction,
            };
        }
    }
}