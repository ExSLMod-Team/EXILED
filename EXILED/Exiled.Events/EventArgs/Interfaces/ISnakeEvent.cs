// -----------------------------------------------------------------------
// <copyright file="ISnakeEvent.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Interfaces
{
    using Exiled.API.Features.Items;
    using InventorySystem.Items.Keycards.Snake;

    /// <summary>
    /// Event args used for all <see cref="Handlers.Snake" /> related events.
    /// </summary>
    public interface ISnakeEvent : IPlayerEvent
    {
        /// <summary>
        /// Gets the Snake game controller.
        /// </summary>
        public SnakeEngine Engine { get; }

        /// <summary>
        /// Gets the keycard that triggered this event.
        /// </summary>
        public Keycard Keycard { get; }
    }
}