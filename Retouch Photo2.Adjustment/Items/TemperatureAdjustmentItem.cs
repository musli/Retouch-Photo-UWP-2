﻿using Retouch_Photo2.Adjustments.Models;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;

namespace Retouch_Photo2.Adjustments.Items
{
    /// <summary>
    /// Item of <see cref="TemperatureAdjustment">.
    /// </summary>
    public class TemperatureAdjustmentItem: AdjustmentItem
    {
        public float Temperature;
        public float Tint;

        //@Construct
        public TemperatureAdjustmentItem()
        {
            base.Name = TemperatureAdjustment.Name;
        }

        //@Override      
        public override Adjustment GetNewAdjustment()
        {
            TemperatureAdjustment adjustment = new TemperatureAdjustment();

            adjustment.TemperatureAdjustmentItem.Temperature = this.Temperature;
            adjustment.TemperatureAdjustmentItem.Tint = this.Tint;

            return adjustment;
        }
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
