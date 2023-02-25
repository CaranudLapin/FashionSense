﻿using FashionSense.Framework.Models.Generic;
using Microsoft.Xna.Framework;

namespace FashionSense.Framework.Models.Sleeves
{
    public class SleevesModel : AppearanceModel
    {
        public Position BodyPosition { get; set; } = new Position() { X = 0, Y = 0 };
        public Size SleevesSize { get; set; }
        public bool DrawBeforeShirt { get; set; }
        public bool DrawBeforeHair { get; set; }
        public bool UseShirtColors { get; set; }
        public SkinToneModel ShirtToneMask { get; set; }

        internal bool IsShirtToneMaskColor(Color color)
        {
            if (!HasShirtToneMask() || ShirtToneMask is null)
            {
                return false;
            }

            if (ShirtToneMask.LightTone is not null && color == ShirtToneMask.Lightest)
            {
                return true;
            }
            else if (ShirtToneMask.MediumTone is not null && color == ShirtToneMask.Medium)
            {
                return true;
            }
            else if (ShirtToneMask.DarkTone is not null && color == ShirtToneMask.Darkest)
            {
                return true;
            }

            return false;
        }

        internal bool HasShirtToneMask()
        {
            if (ShirtToneMask is null)
            {
                return false;

            }
            return ShirtToneMask.LightTone is not null || ShirtToneMask.MediumTone is not null || ShirtToneMask.DarkTone is not null;
        }
    }
}
