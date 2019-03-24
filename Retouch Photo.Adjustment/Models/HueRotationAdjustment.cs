﻿using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Retouch_Photo.Adjustments.Controls;
using Retouch_Photo.Adjustments.Items;
using Retouch_Photo.Adjustments.Pages;

namespace Retouch_Photo.Adjustments.Models
{
    public class HueRotationAdjustment : Adjustment
    {
        public static readonly string Name = "HueRotation";
        public HueRotationAdjustmentItem HueRotationAdjustmentitem=new HueRotationAdjustmentItem();

        public HueRotationAdjustment()
        {
            base.Type = AdjustmentType.HueRotation;
            base.Icon = new HueRotationControl();
            base.Item = this.HueRotationAdjustmentitem;
            base.Item.Reset();
            base.HasPage = true;
        }
    }
}

