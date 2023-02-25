﻿using FashionSense.Framework.Models.Generic;

namespace FashionSense.Framework.Models.Hat
{
    public class HatModel : AppearanceModel
    {
        public Position HeadPosition { get; set; } = new Position() { X = 0, Y = 0 };
        public Size HatSize { get; set; }
        public bool HideHair { get; set; }


        public override bool HideWhileSwimming { get; set; } = false;
        public override bool HideWhileWearingBathingSuit { get; set; } = false;
    }
}
