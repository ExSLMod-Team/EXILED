// -----------------------------------------------------------------------
// <copyright file="GameOverEventArgs.cs" company="ExMod Team">
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
    /// Contains all information after a player loses at Snake.
    /// </summary>
    public class GameOverEventArgs : ISnakeEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameOverEventArgs" /> class.
        /// </summary>
        /// <param name="engine">The <see cref="SnakeEngine"/> instance.</param>
        /// <param name="player">The <see cref="API.Features.Player"/> who triggered this event.</param>
        /// <param name="keycard">The <see cref="Keycard"/> triggering this event.</param>
        public GameOverEventArgs(SnakeEngine engine, API.Features.Player player, Keycard keycard)
        {
            Engine = engine;
            Player = player;
            FinalScore = engine.Score;
            Length = engine.CurLength;
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
        /// Gets the final score.
        /// </summary>
        public int FinalScore { get; }

        /// <summary>
        /// Gets the final length of the snake.
        /// </summary>
        public int Length { get; }
    }
}