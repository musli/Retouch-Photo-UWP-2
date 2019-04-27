﻿using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Retouch_Photo2.Adjustments;
using Retouch_Photo2.Blends;
using Retouch_Photo2.Effects;
using Retouch_Photo2.Models.Layers;
using Retouch_Photo2.ViewModels;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Graphics.Effects;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using static Retouch_Photo2.Library.HomographyController;

namespace Retouch_Photo2.Models
{
    public abstract class Layer
    {

        //ViewModel
        DrawViewModel ViewModel => Retouch_Photo2.App.ViewModel;

        public string Name= "Layer";        
        public UIElement Icon;

        public double Opacity = 100;
        public bool IsVisual= true;
        public int BlendIndex;
        public Transformer Transformer, OldTransformer;
        public AdjustmentManager AdjustmentManager = new AdjustmentManager();
        public EffectManager EffectManager = new EffectManager();
        

        //Create
        public static Layer CreateFromXElement(ICanvasResourceCreator creator, XElement element)
        {
            int width = (int)element.Element("LayerWidth");
            int height = (int)element.Element("LayerHeight");

            string strings = element.Element("CanvasRenderTarget").Value;
            byte[] bytes = Convert.FromBase64String(strings);

            ImageLayer layer = ImageLayer.CreateFromBytes(creator, bytes, width, height);
            layer.Name = element.Element("LayerName").Value;
            layer.IsVisual = (bool)element.Element("LayerVisual");
            layer.Opacity = (double)element.Element("LayerOpacity");
            layer.BlendIndex =(int)element.Element("LayerBlendIndex");

            return layer;
        }



        //Transform
        public virtual void TransformStart() => this.OldTransformer= this.Transformer;
        public virtual void TransformDelta() { }
        public virtual void TransformComplete() { }

        //@Override
        /// <summary> 当前页面成为活动页面 </summary>
        public virtual void LayerOnNavigatedTo() { }
        /// <summary> 当前页面不再成为活动页面 </summary>
        public virtual void LayerOnNavigatedFrom() { }


        //@override
        /// <summary> 颜色改变 </summary>
        public virtual void ColorChanged(Color color, bool fillOrStroke = true) { }
        /// <summary> 在控件层上绘制蓝色边界线条（比如椭圆的边界线条） </summary>
        public virtual void Draw(CanvasDrawingSession ds, Matrix3x2 matrix) { }
        /// <summary> 得到图层渲染 </summary>
        protected abstract ICanvasImage GetRender(IGraphicsEffectSource image, Matrix3x2 canvasToVirtualMatrix);


        //@static
        /// <summary> LayerRender </summary>
        /// <param name="layer">当前图层</param>
        /// <param name="image">从当前图层上面 传下来的 图像</param>
        /// <returns>新的 向下传递的 图像</returns>
        public static ICanvasImage LayerRender(Layer layer, ICanvasImage image,Matrix3x2 matrix)
        {
            if (layer.IsVisual == false || layer.Opacity == 0) return image;

            ICanvasImage effect = EffectManager.Render
            (
                layer.EffectManager, 
                AdjustmentManager.Render
                (
                    layer.AdjustmentManager, layer.GetRender(image, matrix)
                )
            );

            return Blend.Render
            (
               type: (BlendType)layer.BlendIndex,
               foreground: image,
               background: (layer.Opacity == 100) ? effect : new OpacityEffect
               {
                   Source = effect,
                   Opacity = (float)(layer.Opacity / 100)
               }
            );
        }
        
    }
}
