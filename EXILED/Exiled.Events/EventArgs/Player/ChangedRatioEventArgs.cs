// -----------------------------------------------------------------------
// <copyright file="ChangedRatioEventArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using System;
    using System.Collections.Generic;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information after a player's Aspect Ratio changes.
    /// </summary>
    public class ChangedRatioEventArgs : IPlayerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangedRatioEventArgs"/> class.
        /// </summary>
        /// <param name="player">The player who is changed ratio.
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="oldratio">The time in seconds before the Sense ability can be used again.
        /// <inheritdoc cref="float" />
        /// </param>
        /// <param name="newratio">Specifies whether the Sense effect is allowed to finish.
        /// <inheritdoc cref="float" />
        /// </param>
        public ChangedRatioEventArgs(ReferenceHub player, float oldratio, float newratio)
        {
            Player = Player.Get(player);
            OldRatio = GetAspectRatioLabel(oldratio);
            NewRatio = GetAspectRatioLabel(newratio);
        }

        /// <summary>
        /// Gets the player who is changed ratio.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the players old ratio.
        /// </summary>
        public AspectRatioType OldRatio { get; }

        /// <summary>
        /// Gets the players new ratio.
        /// </summary>
        public AspectRatioType NewRatio { get; }

        private static AspectRatioType GetAspectRatioLabel(float ratio)
        {
            Dictionary<AspectRatioType, float> referencevalues = new()
            {
            { AspectRatioType.Ratio3_2, 3f / 2f },
            { AspectRatioType.Ratio4_3, 4f / 3f },
            { AspectRatioType.Ratio5_4, 5f / 4f },
            { AspectRatioType.Ratio16_9, 16f / 9f },
            { AspectRatioType.Ratio16_10, 16f / 10f },
            { AspectRatioType.Ratio21_9, 21f / 9f },
            { AspectRatioType.Ratio32_9, 32f / 9f },
            };

            float closestDiff = float.MaxValue;
            AspectRatioType closestRatio = AspectRatioType.Unknown;

            foreach (KeyValuePair<AspectRatioType, float> kvp in referencevalues)
            {
                float diff = Math.Abs(ratio - kvp.Value);
                if (diff < closestDiff)
                {
                    closestDiff = diff;
                    closestRatio = kvp.Key;
                }
            }

            return closestRatio;
        }
    }
}
