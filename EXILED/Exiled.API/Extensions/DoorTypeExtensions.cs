// -----------------------------------------------------------------------
// <copyright file="DoorTypeExtensions.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Features.Doors;

    /// <summary>
    /// A set of extensions for <see cref="DoorType"/>.
    /// </summary>
    public static class DoorTypeExtensions
    {
        private static readonly List<DoorType> GateEnum = GetEnumByType("Gate");

        private static readonly List<DoorType> CheckpointEnum = GetEnumByType("Checkpoint");

        private static readonly List<DoorType> ElevatorEnum = GetEnumByType("Elevator");

        private static readonly List<DoorType> HIDEnum = GetEnumByType("HID");

        private static readonly List<DoorType> ScpEnum = GetEnumByType("Scp");

        private static readonly List<DoorType> EscapeEnum = GetEnumByType("Escape");

        private static readonly List<DoorType> UnknownEnum = GetEnumByType("Unknown");

        /// <summary>
        /// Checks if a <see cref="DoorType">door type</see> is a gate.
        /// </summary>
        /// <param name="door">The door to be checked.</param>
        /// <returns>Returns whether the <see cref="DoorType"/> is a gate.</returns>
        public static bool IsGate(this DoorType door) => GateEnum.Contains(door);

        /// <summary>
        /// Checks if a <see cref="DoorType">door type</see> is a checkpoint.
        /// </summary>
        /// <param name="door">The door to be checked.</param>
        /// <returns>Returns whether the <see cref="DoorType"/> is a checkpoint.</returns>
        public static bool IsCheckpoint(this DoorType door) => CheckpointEnum.Contains(door);

        /// <summary>
        /// Checks if a <see cref="DoorType">door type</see> is an elevator.
        /// </summary>
        /// <param name="door">The door to be checked.</param>
        /// <returns>Returns whether the <see cref="DoorType"/> is an elevator.</returns>
        public static bool IsElevator(this DoorType door) => ElevatorEnum.Contains(door);

        /// <summary>
        /// Checks if a <see cref="DoorType">door type</see> is an HID door.
        /// </summary>
        /// <param name="door">The door to be checked.</param>
        /// <returns>Returns whether the <see cref="DoorType"/> is an HID door.</returns>
        public static bool IsHID(this DoorType door) => HIDEnum.Contains(door);

        /// <summary>
        /// Checks if a <see cref="DoorType">door type</see> is an SCP-related door.
        /// </summary>
        /// <param name="door">The door to be checked.</param>
        /// <returns>Returns whether the <see cref="DoorType"/> is an SCP-related door.</returns>
        public static bool IsScp(this DoorType door) => ScpEnum.Contains(door);

        /// <summary>
        /// Checks if a <see cref="DoorType">door type</see> is an escape door.
        /// </summary>
        /// <param name="door">The door to be checked.</param>
        /// <returns>Returns whether the <see cref="DoorType"/> is an escape door.</returns>
        public static bool IsEscape(this DoorType door) => EscapeEnum.Contains(door);

        /// <summary>
        /// Checks if a <see cref="DoorType"/> is located in the Light Containment Zone (LCZ).
        /// </summary>
        /// <param name="door">The door to be checked.</param>
        /// <returns>Returns <c>true</c> if the <see cref="DoorType"/> is a door from LCZ; otherwise, <c>false</c>.</returns>
        public static bool IsLCZ(this DoorType door) => door is DoorType.Airlock or DoorType.Scp914Door or DoorType.Scp330 or DoorType.Scp173Armory
            or DoorType.Scp173Gate or DoorType.Scp173Connector or DoorType.Scp173Bottom or DoorType.Scp914Gate or DoorType.Scp330Chamber or DoorType.LczWc
            or DoorType.LczArmory or DoorType.ElevatorLczA or DoorType.ElevatorLczB or DoorType.CheckpointLczA or DoorType.CheckpointLczB or DoorType.GR18Gate
            or DoorType.GR18Inner or DoorType.LczCafe or DoorType.LightContainmentDoor or DoorType.PrisonDoor;

        /// <summary>
        /// Checks if a <see cref="DoorType"/> is located in the Heavy Containment Zone (HCZ).
        /// </summary>
        /// <param name="door">The door to be checked.</param>
        /// <returns>Returns <c>true</c> if the <see cref="DoorType"/> is a door from HCZ; otherwise, <c>false</c>.</returns>
        public static bool IsHCZ(this DoorType door) => door is DoorType.HczArmory or DoorType.Scp049Armory or DoorType.Scp049Gate or DoorType.Scp079Armory
            or DoorType.Scp079First or DoorType.Scp079Second or DoorType.Scp096 or DoorType.Scp106Primary or DoorType.Scp106Secondary or DoorType.Scp173NewGate
            or DoorType.Scp939Cryo or DoorType.ElevatorScp049 or DoorType.HeavyBulkDoor or DoorType.HeavyContainmentDoor or DoorType.HIDChamber or DoorType.HIDLab
            or DoorType.Hcz127Lab or DoorType.CheckpointEzHczA or DoorType.CheckpointEzHczB or DoorType.CheckpointGateA or DoorType.CheckpointGateB;

        /// <summary>
        /// Checks if a <see cref="DoorType"/> is located in the Entrance Zone (EZ).
        /// </summary>
        /// <param name="door">The door to be checked.</param>
        /// <returns>Returns <c>true</c> if the <see cref="DoorType"/> is a door from EZ; otherwise, <c>false</c>.</returns>
        public static bool IsEZ(this DoorType door) => door is DoorType.GateA or DoorType.GateB or DoorType.ElevatorGateA or DoorType.ElevatorGateB
            or DoorType.CheckpointEzHczA or DoorType.CheckpointEzHczB or DoorType.CheckpointGateA or DoorType.CheckpointGateB or DoorType.Intercom;

        /// <summary>
        /// Checks if a <see cref="DoorType"/> is located on the Surface.
        /// </summary>
        /// <param name="door">The door to be checked.</param>
        /// <returns>Returns <c>true</c> if the <see cref="DoorType"/> is a door from Surface; otherwise, <c>false</c>.</returns>
        public static bool IsSurface(this DoorType door) => door is DoorType.SurfaceGate or DoorType.NukeSurface
            or DoorType.EscapePrimary or DoorType.EscapeSecondary or DoorType.EscapeFinal;

        /// <summary>
        /// Checks if a <see cref="DoorType"/> is of an unknown type.
        /// </summary>
        /// <param name="door">The door to be checked.</param>
        /// <returns>Returns <c>true</c> if the <see cref="DoorType"/> is unknown; otherwise, <c>false</c>.</returns>
        public static bool IsUnknown(this DoorType door) => UnknownEnum.Contains(door);

        private static List<DoorType> GetEnumByType(string doorName) => EnumUtils<DoorType>.Values.Where(x => x.ToString().Contains(doorName)).ToList();
    }
}