// -----------------------------------------------------------------------
// <copyright file="Text.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Toys
{
    using AdminToys;
    using Enums;
    using Interfaces;
    using UnityEngine;

    /// <summary>
    /// A wrapper class for <see cref="TextToy"/>.
    /// </summary>
    public class Text : AdminToy, IWrapper<TextToy>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Text"/> class.
        /// </summary>
        /// <param name="textToy">The <see cref="TextToy"/> of the toy.</param>
        internal Text(TextToy textToy)
            : base(textToy, AdminToyType.TextToy) => Base = textToy;

        /// <summary>
        /// Gets the prefab.
        /// </summary>
        public static TextToy Prefab => PrefabHelper.GetPrefab<TextToy>(PrefabType.TextToy);

        /// <summary>
        /// Gets the base <see cref="TextToy"/>.
        /// </summary>
        public TextToy Base { get; }

        /// <summary>
        /// Gets or sets the Text shown.
        /// </summary>
        public string TextFormat
        {
            get => Base.Network_textFormat;
            set => Base.Network_textFormat = value;
        }

        /// <summary>
        /// Gets or sets the size of the Display Size of the Text.
        /// </summary>
        public Vector2 DisplaySize
        {
            get => Base.Network_displaySize;
            set => Base.Network_displaySize = value;
        }

        /// <summary>
        /// Creates a new <see cref="Text"/>.
        /// </summary>
        /// <param name="newText"> The text to shown <see cref="TextFormat"/>.</param>
        /// <param name="displaySize"> The size of the <see cref="DisplaySize"/>.</param>
        /// <param name="posititon"> The position of the <see cref="Text"/>.</param>
        /// <param name="rotation"> The rotation of the <see cref="Text"/>.</param>
        /// <param name="scale"> The scale of the <see cref="Text"/>.</param>
        /// <param name="spawn"> Whether the <see cref="Text"/> should be initially spawned.</param>
        /// <returns> The new <see cref="Text"/>.</returns>
        public static Text Create(string newText, Vector2? displaySize, Vector3? posititon, Vector3? rotation, Vector3? scale, bool spawn)
        {
            Text text = new Text(Object.Instantiate(Prefab));
            text.Position = posititon ?? Vector3.zero;
            text.Rotation = Quaternion.Euler(rotation ?? Vector3.zero);
            text.Scale = scale ?? Vector3.one;

            text.TextFormat = newText ?? string.Empty;
            text.DisplaySize = displaySize ?? TextToy.DefaultDisplaySize;

            if (spawn)
                text.Spawn();
            return text;
        }
    }
}
