﻿using FanKit.Transformers;
using Microsoft.Graphics.Canvas;
using Retouch_Photo2.Adjustments;
using Retouch_Photo2.Blends;
using Retouch_Photo2.Effects;
using System.Collections.ObjectModel;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml;

namespace Retouch_Photo2.Layers
{
    /// <summary>
    /// Represents a layer that can have render properties.
    /// </summary>
    public interface ILayer : ICacheTransform
    { 
        /// <summary> Gets or Sets ILayer's IsChecked. </summary>
        bool IsChecked { get; set; }
        /// <summary> Gets or Sets ILayer's visibility. </summary>
        Visibility Visibility { get; set; }


        /// <summary> Gets ILayer's type name. </summary>
        string Type { get; }
        /// <summary> Gets ILayer's icon. </summary>
        UIElement Icon { get; }
        /// <summary> Gets or Sets ILayer's fill-color. </summary>
        Color? FillColor { get; set; }
        /// <summary> Gets or Sets ILayer's stroke-color. </summary>
        Color? StrokeColor { get; set; }


        /// <summary> Gets or Sets ILayer's name. </summary>
        string Name { get; set; }
        /// <summary> Gets or Sets ILayer's opacity. </summary>
        float Opacity { get; set; }
        /// <summary> Gets or Sets ILayer's blend type. </summary>
        BlendType BlendType { get; set; }


        /// <summary>
        /// Gets transformer-matrix's resulting matrix.
        /// </summary>
        /// <returns> The product matrix. </returns>
        Matrix3x2 GetMatrix();
        /// <summary> The source transformer. </summary>
        Transformer Source { get; set; }
        /// <summary> The destination transformer. </summary>
        Transformer Destination { get; set; }
        /// <summary> The destination transformer 's old cache. </summary>
        Transformer OldDestination { get; set; }
        /// <summary> Is disable rotate radian? Defult **false**. </summary>
        bool DisabledRadian { get; set; }

        /// <summary> The ILayer's children layers. </summary>
        ObservableCollection<ILayer> Children { get; }
        /// <summary> The ILayer's effect manager. </summary>
        EffectManager EffectManager { get; }
        /// <summary> The ILayer's adjustment manager. </summary>
        AdjustmentManager AdjustmentManager { get; }


        /// <summary>
        /// Gets a specific rended-layer.
        /// </summary>
        /// <param name="resourceCreator"> The resource-creator. </param>
        /// <param name="previousImage"> Previous rendered images. </param>
        /// <param name="canvasToVirtualMatrix"> The canvas-to-virtual matrix. </param>
        /// <returns> The rendered layer. </returns>
        ICanvasImage GetRender(ICanvasResourceCreator resourceCreator, ICanvasImage previousImage, Matrix3x2 canvasToVirtualMatrix);
        /// <summary>
        /// Draw lines on bound.
        /// </summary>
        /// <param name="resourceCreator"> The resource-creator. </param>
        /// <param name="drawingSession"> The drawing-session. </param>
        /// <param name="matrix"> The matrix. </param>
        /// <param name="accentColor"> The accent color. </param>
        void DrawBound(ICanvasResourceCreator resourceCreator, CanvasDrawingSession drawingSession, Matrix3x2 matrix, Windows.UI.Color accentColor);

        /// <summary>
        /// Get ILayer own copy.
        /// </summary>
        /// <param name="resourceCreator"> The resource-creator. </param>
        /// <returns> The cloned ILayer. </returns>
        ILayer Clone(ICanvasResourceCreator resourceCreator);
    }
}