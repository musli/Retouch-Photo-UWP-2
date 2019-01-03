﻿using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Retouch_Photo.Controls.AdjustmentControls;
using Retouch_Photo.Pages.AdjustmentPages;

namespace Retouch_Photo.Models.Adjustments
{
    public class ContrastAdjustment : Adjustment
    {
        public float Contrast;

        public ContrastAdjustment()
        {
            base.Type = AdjustmentType.Contrast;
            base.Icon = new AdjustmentContrastControl();
            base.HasPage = true;
            this.Reset();
        }

        public override void Reset()
        {
            this.Contrast = 0.0f;
        }
        public override ICanvasImage GetRender(ICanvasImage image)
        {
            return new ContrastEffect
            {
                Contrast = this.Contrast,
                Source = image
            };
        }
    }


    public class ContrastAdjustmentCandidate : AdjustmentCandidate
    {
        public AdjustmentContrastPage page = new AdjustmentContrastPage();

        public ContrastAdjustmentCandidate()
        {
            base.Type = AdjustmentType.Contrast;
            base.Icon = new AdjustmentContrastControl();
            base.Page = this.page;
        }

        public override Adjustment GetNewAdjustment() => new ContrastAdjustment();
        public override void SetPage(Adjustment adjustment)
        {
            if (adjustment is ContrastAdjustment ContrastAdjustment)
            {
                this.page.ContrastAdjustment = null;
                this.page.ContrastAdjustment = ContrastAdjustment;
            }
        }
    }
}
