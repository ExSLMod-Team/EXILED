// -----------------------------------------------------------------------
// <copyright file="SettingGroup.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core.UserSettings
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Pools;

    /// <summary>
    /// Represents a group of SettingBase.
    /// </summary>
    public class SettingGroup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingGroup"/> class.
        /// </summary>
        /// <param name="settings"><inheritdoc cref="Settings"/></param>
        /// <param name="priority"><inheritdoc cref="Priority"/></param>
        /// <param name="viewers"><inheritdoc cref="Viewers"/></param>
        /// <param name="subgroups"><inheritdoc cref="SubGroups"/></param>
        public SettingGroup(IEnumerable<SettingBase> settings, int priority = 0, Predicate<Player> viewers = null, IEnumerable<SettingGroup> subgroups = null)
        {
            Settings = settings;
            Priority = priority;
            Viewers = viewers;
            SubGroups = subgroups;
        }

        /// <summary>
        /// Gets or sets the settings within this group.
        /// </summary>
        public IEnumerable<SettingBase> Settings { get; set; }

        /// <summary>
        /// Gets or sets the priority of this group when sent to players.
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets the predicate which determines who can see this group of settings.
        /// </summary>
        public Predicate<Player> Viewers { get; set; }

        /// <summary>
        /// Gets or sets the subgroups within this group.
        /// </summary>
        public IEnumerable<SettingGroup> SubGroups { get; set; }

        /// <summary>
        /// Handles constructing a group containing a singular setting. Useful for standalone settings.
        /// </summary>
        /// <param name="setting">A <see cref="SettingBase"/> instance.</param>
        /// <returns>A SettingGroup containing the singular setting.</returns>
        public static implicit operator SettingGroup(SettingBase setting) => new(new[] { setting });

        // These two methods are required to avoid implicit IEnumerable<SettingBase> conversions. They are used to make the registration of settings easier. The true use of these will likely only be utilized once the obsolete code is removed.

        /// <summary>
        /// Handles implicitly converting a List of SettingBase into a SettingGroup.
        /// </summary>
        /// <param name="settings">A <see cref="List{T}"/> of <see cref="SettingBase"/>.</param>
        /// <returns>A SettingGroup containing all the SettingBases.</returns>
        public static implicit operator SettingGroup(List<SettingBase> settings) => new(settings);

        /// <summary>
        /// Handles implicitly converting an Array of SettingBase into a SettingGroup.
        /// </summary>
        /// <param name="settings">An <see cref="Array"/> of <see cref="SettingBase"/>.</param>
        /// <returns>A SettingGroup containing all the SettingBases.</returns>
        public static implicit operator SettingGroup(SettingBase[] settings) => new(settings);

        /// <summary>
        /// Recursively gets all instances of <see cref="SettingBase"/> contained within this <see cref="SettingGroup"/>'s Settings and SubGroups.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="SettingBase"/>.</returns>
        public IEnumerable<SettingBase> GetAllSettings()
        {
            try
            {
                List<SettingBase> settings = new();
                settings.AddRange(Settings);
                if (SubGroups == null)
                    return settings;
                List<SettingGroup> recursiveCheck = ListPool<SettingGroup>.Pool.Get(1);
                recursiveCheck.Add(this);
                foreach (SettingGroup group in SubGroups)
                {
                    if (recursiveCheck.Contains(group))
                        throw new InvalidOperationException("SettingGroups cannot reference themselves within subgroups.");
                    settings.AddRange(group.GetAllSettings());
                }

                ListPool<SettingGroup>.Pool.Return(recursiveCheck);
                return settings;
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(GetAllSettings)}: {ex}");
                return Array.Empty<SettingBase>();
            }
        }

        /// <summary>
        /// Recursively gets all instances of <see cref="SettingBase"/> contained within this <see cref="SettingGroup"/>'s Settings and SubGroups that can be viewed by a specified player.
        /// </summary>
        /// <param name="viewer"><inheritdoc cref="Player"/></param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="SettingBase"/>.</returns>
        public IEnumerable<SettingBase> GetViewableSettingsOrdered(Player viewer)
        {
            try
            {
                if (viewer == null)
                    return Array.Empty<SettingBase>();
                List<SettingBase> settings = new();
                if (Viewers == null || Viewers(viewer))
                    settings.AddRange(Settings);
                if (SubGroups == null)
                    return settings;
                List<SettingGroup> recursiveCheck = ListPool<SettingGroup>.Pool.Get(1);
                recursiveCheck.Add(this);
                foreach (SettingGroup group in SubGroups.Where(group => group.Viewers == null || group.Viewers(viewer)).OrderByDescending(group => group.Priority))
                {
                    if (recursiveCheck.Contains(group))
                        throw new InvalidOperationException("SettingGroups cannot reference themselves within subgroups.");
                    settings.AddRange(group.GetAllSettings());
                }

                ListPool<SettingGroup>.Pool.Return(recursiveCheck);
                return settings;
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(GetViewableSettingsOrdered)}: {ex}");
                return Array.Empty<SettingBase>();
            }
        }

        /// <summary>
        /// Returns a string representation of this <see cref="SettingGroup"/>.
        /// </summary>
        /// <returns>A string in human-readable format.</returns>
        public override string ToString() => $"{Priority} ({Viewers}) [{string.Join(", ", Settings.Select(s => s.ToString()))}]";
    }
}