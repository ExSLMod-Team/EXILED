// -----------------------------------------------------------------------
// <copyright file="GainedScoreEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Snake
{
    using Exiled.API.Features.Items;
    using Interfaces;
    using InventorySystem.Items.Keycards.Snake;

    /// <summary>
    /// Contains all information after a player gains score in the Snake minigame.
    /// </summary>
    public class GainedScoreEventArgs : ISnakeEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GainedScoreEventArgs" /> class.
        /// </summary>
        /// <param name="engine">The <see cref="SnakeEngine"/> instance.</param>
        /// <param name="player">The <see cref="API.Features.Player"/> who triggered this event.</param>
        /// <param name="keycard">The <see cref="Keycard"/> triggering this event.</param>
        public GainedScoreEventArgs(SnakeEngine engine, API.Features.Player player, Keycard keycard)
        {
            Engine = engine;
            Player = player;
            Score = engine.Score;
            Keycard = keycard;
        }

        /// <inheritdoc />
        public SnakeEngine Engine { get;  }

        /// <summary>
        /// Gets the person playing the game.
        /// </summary>
        public API.Features.Player Player { get; }

        /// <inheritdoc />
        public Keycard Keycard { get; }

        /// <summary>
        /// Gets the newly obtained score.
        /// </summary>
        public int Score { get; }
    }
}