// -----------------------------------------------------------------------
// <copyright file="SwingingJailbirdEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Item
{
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before a player swings a <see cref="Jailbird"/>.
    /// </summary>
    public class SwingingJailbirdEventArgs : IItemEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SwingingJailbirdEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="swingItem">The item being swung.</param>
        /// <param name="cunHurt">Whether the item could cause harm.</param>
        public SwingingJailbirdEventArgs(ReferenceHub player, InventorySystem.Items.ItemBase swingItem, bool cunHurt = true)
        {
            Player = Player.Get(player);
            Jailbird = (Jailbird)Item.Get(swingItem);
            CanHurt = cunHurt;
        }

        /// <summary>
        /// Gets the <see cref="API.Features.Player"/> who's swinging an item.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the <see cref="API.Features.Items.Jailbird"/> that is being swung.
        /// </summary>
        public Jailbird Jailbird { get; }

        /// <summary>
        /// Gets the <see cref="API.Features.Items.Item"/> that is being swung.
        /// </summary>
        public Item Item => Jailbird;

        /// <summary>
        /// Gets or sets a value indicating whether the item could cause harm.
        /// </summary>
        public bool CanHurt { get; set; }
    }
}
