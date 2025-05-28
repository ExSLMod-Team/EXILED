// -----------------------------------------------------------------------
// <copyright file="Snake.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
#pragma warning disable SA1623 // Property summary documentation should match accessors

    using Exiled.Events.EventArgs.Snake;
    using Features;

    /// <summary>
    /// Handles Snake related events.
    /// </summary>
    public class Snake
    {
        /// <summary>
        /// Invoked when gaining score in Snake.
        /// </summary>
        public static Event<GainedScoreEventArgs> GainedScore { get; set; } = new();

        /// <summary>
        /// Invoked when a player loses in Snake.
        /// </summary>
        public static Event<GameOverEventArgs> GameOver { get; set; } = new();

        /// <summary>
        /// Invoked when a player starts a new game.
        /// </summary>
        public static Event<NewGameEventArgs> NewGame { get; set; } = new();

        /// <summary>
        /// Called when a player gains score in Snake.
        /// </summary>
        /// <param name="ev">The <see cref="GainedScoreEventArgs"/> instance.</param>
        public static void OnGainedScore(GainedScoreEventArgs ev)
        {
            GainedScore.InvokeSafely(ev);
        }

        /// <summary>
        /// Called when a player loses in Snake.
        /// </summary>
        /// <param name="ev">The <see cref="GameOverEventArgs"/> instance.</param>
        public static void OnGameOver(GameOverEventArgs ev)
        {
            GameOver.InvokeSafely(ev);
        }

        /// <summary>
        /// Called when a player starts a new game in Snake.
        /// </summary>
        /// <param name="ev">The <see cref="NewGameEventArgs"/> instance.</param>
        public static void OnNewGame(NewGameEventArgs ev)
        {
            NewGame.InvokeSafely(ev);
        }
    }
}