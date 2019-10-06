﻿using Microsoft.Graphics.Canvas;
using Retouch_Photo2.Layers.Icons;
using System.Numerics;
using Windows.UI.Xaml;

namespace Retouch_Photo2.Layers.Models
{
    /// <summary>
    /// <see cref="LayerBase"/>'s GroupLayer .
    /// </summary>
    public class GroupLayer : LayerBase
    {
        //@Construct
        public GroupLayer() 
        {
            base.Control.Icon = new GroupIcon();
            base.Control.Text = "Group";
        }

        //@Override
        public override string Type => "Group";
        
        public override void CacheTransform()
        {
            base.CacheTransform();

            foreach (ILayer child in this.Children)
            {
                child.TransformManager.CacheTransform();
            }
        }
        public override void TransformMultiplies(Matrix3x2 matrix)
        {
            base.TransformMultiplies(matrix);

            foreach (ILayer child in this.Children)
            {
                child.TransformManager.TransformMultiplies(matrix);
            }
        }
        public override void TransformAdd(Vector2 vector)
        {
            base.TransformAdd(vector);

            foreach (ILayer child in this.Children)
            {
                child.TransformManager.TransformAdd(vector);
            }
        }

        public override ICanvasImage GetRender(ICanvasResourceCreator resourceCreator, ICanvasImage previousImage, Matrix3x2 canvasToVirtualMatrix)
        { 
            CanvasCommandList command = new CanvasCommandList(resourceCreator);
            using (CanvasDrawingSession drawingSession = command.CreateDrawingSession())
            {
                foreach (ILayer child in this.Children)
                {
                    if (child.Visibility == Visibility.Collapsed) continue;
                    if (child.Opacity == 0) continue;

                    //GetRender
                    ICanvasImage currentImage = child.GetRender(resourceCreator, previousImage, canvasToVirtualMatrix);
                    drawingSession.DrawImage(currentImage);
                }
            }
            return command;
        }
        public override ILayer Clone(ICanvasResourceCreator resourceCreator)
        {
            GroupLayer groupLayer = new GroupLayer();

            LayerBase.CopyWith(resourceCreator, groupLayer, this);
            return groupLayer;
        }

    }
}