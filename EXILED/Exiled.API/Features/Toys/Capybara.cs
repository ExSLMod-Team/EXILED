// -----------------------------------------------------------------------
// <copyright file="Capybara.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Toys
{
    using AdminToys;
    using Enums;
    using Interfaces;
    using Mirror;
    using UnityEngine;

    /// <summary>
    /// A wrapper class for <see cref="CapybaraToy"/>.
    /// </summary>
    public class Capybara : AdminToy, IWrapper<CapybaraToy>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Capybara"/> class.
        /// </summary>
        /// <param name="capybaraToy">The <see cref="CapybaraToy"/> of the toy.</param>
        internal Capybara(CapybaraToy capybaraToy)
            : base(capybaraToy, AdminToyType.Capybara) => Base = capybaraToy;

        /// <summary>
        /// Gets the prefab.
        /// </summary>
        public static CapybaraToy Prefab => PrefabHelper.GetPrefab<CapybaraToy>(PrefabType.CapybaraToy);

        /// <summary>
        /// Gets the base <see cref="CapybaraToy"/>.
        /// </summary>
        public CapybaraToy Base { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the capybara can be collided with. Only server side.
        /// </summary>
        public bool Collidable
        {
            get => Base.CollisionsEnabled;
            set => Base.CollisionsEnabled = value;
        }

        /// <summary>
        /// Creates a new <see cref="Capybara"/>.
        /// </summary>
        /// <param name="posititon"> The position of the <see cref="Capybara"/>.</param>
        /// <param name="rotation"> The rotation of the <see cref="Capybara"/>.</param>
        /// <param name="scale"> The scale of the <see cref="Capybara"/>.</param>
        /// <param name="spawn"> Whether the <see cref="Capybara"/> should be initially spawned.</param>
        /// <returns> The new <see cref="Capybara"/>.</returns>
        public static Capybara Create(Vector3? posititon, Vector3? rotation, Vector3? scale, bool spawn)
        {
            Capybara capybara = new Capybara(Object.Instantiate(Prefab));
            capybara.Position = posititon ?? Vector3.zero;
            capybara.Rotation = Quaternion.Euler(rotation ?? Vector3.zero);
            capybara.Scale = scale ?? Vector3.one;

            if (spawn)
                capybara.Spawn();

            return capybara;
        }
    }
}
