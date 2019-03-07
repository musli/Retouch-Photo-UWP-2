﻿using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Retouch_Photo.Blends.Controls;
using Windows.UI.Xaml;

namespace Retouch_Photo.Blends.Models
{
    public class ColorBurnBlend : Blend
    {
        public ColorBurnBlend()
        {
            base.Type = BlendType.ColorBurn;
        }

        protected override FrameworkElement GetIcon() => new ColorBurnControl();
        protected override ICanvasImage GetRender(ICanvasImage background, ICanvasImage foreground)
        {
            return new BlendEffect
            {
                Background = background,
                Foreground = foreground,
                Mode = BlendEffectMode.ColorBurn
            };
        }
    }
}