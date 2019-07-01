﻿using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Retouch_Photo2.Adjustments.Models;

namespace Retouch_Photo2.Adjustments.Items
{
    /// <summary>
    /// Item of <see cref="HighlightsAndShadowsAdjustment">.
    /// </summary>
    public class HighlightsAndShadowsAdjustmentItem : AdjustmentItem
    {
        public float Shadows;
        public float Highlights;
        public float Clarity;
        public float MaskBlurAmount;
        public bool SourceIsLinearGamma;

        //@Construct
        public HighlightsAndShadowsAdjustmentItem()
        {
            base.Name = HighlightsAndShadowsAdjustment.Name;
        }

        //@Override
        public override Adjustment GetNewAdjustment()
        {
            HighlightsAndShadowsAdjustment adjustment = new HighlightsAndShadowsAdjustment();

            adjustment.HighlightsAndShadowsAdjustmentItem.Shadows = this.Shadows;
            adjustment.HighlightsAndShadowsAdjustmentItem.Highlights = this.Highlights;
            adjustment.HighlightsAndShadowsAdjustmentItem.Clarity = this.Clarity;
            adjustment.HighlightsAndShadowsAdjustmentItem.MaskBlurAmount = this.MaskBlurAmount;
            adjustment.HighlightsAndShadowsAdjustmentItem.SourceIsLinearGamma = this.SourceIsLinearGamma;

            return adjustment;
        }
        public override void Reset()
        {
            this.Shadows = 0.0f;
            this.Highlights = 0.0f;
            this.Clarity = 0.0f;
            this.MaskBlurAmount = 1.25f;
            this.SourceIsLinearGamma = false;
        }
        public override ICanvasImage GetRender(ICanvasImage image)
        {
            return new HighlightsAndShadowsEffect
            {
                Shadows = this.Shadows,
                Highlights = this.Highlights,
                Clarity = this.Clarity,
                MaskBlurAmount = this.MaskBlurAmount,
                SourceIsLinearGamma = this.SourceIsLinearGamma,
                Source = image
            };
        }
    }
}
