﻿using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Retouch_Photo.Controls.BlendsControls;
using Windows.UI.Xaml;

namespace Retouch_Photo.Models.Blends
{
    public class HardMixBlend : Blend
    {
        public HardMixBlend()
        {
            base.Type = BlendType.HardMix;
        }

        protected override FrameworkElement GetIcon() => new HardMixControl();
        protected override ICanvasImage GetRender(ICanvasImage background, ICanvasImage foreground)
        {
            return new BlendEffect
            {
                Background = background,
                Foreground = foreground,
                Mode = BlendEffectMode.HardMix
            };
        }
    }
}
