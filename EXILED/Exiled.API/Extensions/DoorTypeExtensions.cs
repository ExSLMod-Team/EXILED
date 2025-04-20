// -----------------------------------------------------------------------
// <copyright file="DoorTypeExtensions.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using Exiled.API.Enums;

    /// <summary>
    /// A set of extensions for <see cref="DoorType"/>.
    /// </summary>
    public static class DoorTypeExtensions
    {
        /// <summary>
        /// Checks if a <see cref="DoorType">door type</see> is a gate.
        /// </summary>
        /// <param name="door">The door to be checked.</param>
        /// <returns>Returns whether the <see cref="DoorType"/> is a gate.</returns>
        public static bool IsGate(this DoorType door) => door.ToString().Contains("Gate");

        /// <summary>
        /// Checks if a <see cref="DoorType">door type</see> is a checkpoint.
        /// </summary>
        /// <param name="door">The door to be checked.</param>
        /// <returns>Returns whether the <see cref="DoorType"/> is a checkpoint.</returns>
        public static bool IsCheckpoint(this DoorType door) => door.ToString().Contains("Checkpoint");

        /// <summary>
        /// Checks if a <see cref="DoorType">door type</see> is an elevator.
        /// </summary>
        /// <param name="door">The door to be checked.</param>
        /// <returns>Returns whether the <see cref="DoorType"/> is an elevator.</returns>
        public static bool IsElevator(this DoorType door) => door.ToString().Contains("Elevator");

        /// <summary>
        /// Checks if a <see cref="DoorType">door type</see> is an HID door.
        /// </summary>
        /// <param name="door">The door to be checked.</param>
        /// <returns>Returns whether the <see cref="DoorType"/> is an HID door.</returns>
        public static bool IsHID(this DoorType door) => door.ToString().Contains("HID");

        /// <summary>
        /// Checks if a <see cref="DoorType">door type</see> is an SCP-related door.
        /// </summary>
        /// <param name="door">The door to be checked.</param>
        /// <returns>Returns whether the <see cref="DoorType"/> is an SCP-related door.</returns>
        public static bool IsScp(this DoorType door) => door.ToString().Contains("Scp");

        /// <summary>
        /// Checks if a <see cref="DoorType">door type</see> is an escape door.
        /// </summary>
        /// <param name="door">The door to be checked.</param>
        /// <returns>Returns whether the <see cref="DoorType"/> is an escape door.</returns>
        public static bool IsEscape(this DoorType door) => door.ToString().Contains("Escape");
    }
}