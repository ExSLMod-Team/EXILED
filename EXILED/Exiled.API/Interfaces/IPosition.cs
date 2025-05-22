// -----------------------------------------------------------------------
// <copyright file="IPosition.cs" company="ExSlMod Team">
// Copyright (c) ExSlMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Interfaces
{
    using UnityEngine;

    /// <summary>
    /// Represents an object with a <see cref="Vector3"/> position.
    /// </summary>
    public interface IPosition
    {
        /// <summary>
        /// Gets the position of this object.
        /// </summary>
        public Vector3 Position { get; }
    }
}
