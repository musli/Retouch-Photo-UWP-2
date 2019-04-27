﻿using Retouch_Photo2.Adjustments.Models;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;

namespace Retouch_Photo2.Adjustments.Items
{
    public class TemperatureAdjustmentItem: AdjustmentItem
    {
        public float Temperature;
        public float Tint;

        public TemperatureAdjustmentItem() => base.Name = TemperatureAdjustment.Name;

        //@override
        public override Adjustment GetAdjustment() => new TemperatureAdjustment()
        {
            TemperatureAdjustmentItem = this
        };
        public override void Reset()
        {
            this.Temperature = 0.0f;
            this.Tint = 0.0f;
        }
        public override ICanvasImage GetRender(ICanvasImage image)
        {
            return new TemperatureAndTintEffect
            {
                Temperature = this.Temperature,
                Tint = this.Tint,
                Source = image
            };
        }
    }
}