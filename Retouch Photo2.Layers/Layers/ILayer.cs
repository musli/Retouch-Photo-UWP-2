﻿using FanKit.Transformers;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using Retouch_Photo2.Blends;
using Retouch_Photo2.Effects;
using Retouch_Photo2.Filters;
using System.Collections.Generic;
using System.Numerics;
using System.Xml.Linq;
using Windows.UI.Xaml;

namespace Retouch_Photo2.Layers
{
    /// <summary>
    /// Represents a layer that can have render properties.
    /// </summary>
    public interface ILayer : ICacheTransform
    {

        /// <summary> Gets or sets ILayer's control. </summary>
        LayerControl Control { get; }


        #region Instance


        /// <summary> Gets or sets <see cref="ILayer"/>'s id. </summary>
        string Id { get; set; }

        /// <summary>
        /// To <see cref="Layerage"/>.
        /// </summary>
        /// <returns> The producted layerage. </returns>
        Layerage ToLayerage();

        /// <summary>
        /// Returns a boolean indicating whether the given <see cref="Layerage"/> is equal to this <see cref="ILayer"/> instance.
        /// </summary>
        /// <param name="other"> The <see cref="Layerage"/> to compare this instance to. </param>
        /// <returns> True if the other <see cref="Layerage"/> is equal to this instance; False otherwise. </returns>
        bool Equals(Layerage other);
        /// <summary>
        /// Returns a boolean indicating whether the given <see cref="LayerBase"/> is equal to this <see cref="ILayer"/> instance.
        /// </summary>
        /// <param name="other"> The <see cref="LayerBase"/> to compare this instance to. </param>
        /// <returns> True if the other <see cref="LayerBase"/> is equal to this instance; False otherwise. </returns>
        bool Equals(LayerBase other);


        #endregion


        #region Property


        /// <summary> Gets ILayer's type. </summary>
        LayerType Type { get; }
        /// <summary> Gets or sets ILayer's name. </summary>
        string Name { get; set; } 
        /// <summary> Gets or sets ILayer's blend mode. </summary>
        BlendEffectMode? BlendMode { get; set; }
        
        /// <summary> Gets or sets ILayer's opacity. </summary>
        float Opacity { get; set; }
        /// <summary> The cache of <see cref="ILayer.Opacity"/>. </summary>
        float StartingOpacity { get; }
        /// <summary> Cache the <see cref="ILayer.Opacity"/>. </summary>
        void CacheOpacity();

        /// <summary> Gets or sets ILayer's visibility. </summary>
        Visibility Visibility { get; set; }
        /// <summary> Gets or sets ILayer's tag-type. </summary>
        TagType TagType { get; set; }

        /// <summary> Gets or sets ILayer's expand. </summary>
        bool IsExpand { get; set; }
        /// <summary> Gets or sets ILayer's selected. </summary>
        bool IsSelected { get; set; }


        /// <summary> Gets or sets ILayer is need to refactoring transformer. </summary>
        bool IsRefactoringTransformer { get; set; }
        /// <summary>
        /// Gets ILayer's actually transformer.
        /// </summary>
        /// <param name="layerage"> The layerage. </param>
        Transformer GetActualTransformer(Layerage layerage);


        /// <summary> Gets or sets ILayer's style. </summary>
        Retouch_Photo2.Styles.Style Style { get; set; }
        /// <summary> Gets or sets ILayer's transform. </summary>
        Transform Transform { get; set; }
        /// <summary> Gets or sets ILayer's effect. </summary>
        Effect Effect { get; set; }
        /// <summary> Gets or sets ILayer's filter. </summary>
        Filter Filter { get; set; }


        /// <summary>
        /// Get own copy.
        /// </summary>
        /// <param name="customDevice"> The custom-device. </param>
        /// <returns> The cloned ILayer. </returns>
        ILayer Clone(CanvasDevice customDevice);

        /// <summary>
        /// Saves the entire <see cref="ILayer"/> to a XElement.
        /// </summary>
        /// <param name="element"> The destination XElement. </param>
        void SaveWith(XElement element);
        /// <summary>
        /// Load the entire <see cref="ILayer"/> form a XElement.
        /// </summary>
        /// <param name="element"> The destination XElement. </param>
        void Load(XElement element);


        #endregion


        #region Render


        /// <summary> Gets or sets ILayer is need to refactoring render. </summary>
        bool IsRefactoringRender { get; set; }

        /// <summary> Gets or sets ILayer is need to refactoring icon render. </summary>
        bool IsRefactoringIconRender { get; set; }

        /// <summary>
        /// Gets a specific actual rended-layer (with icon render).
        /// </summary>
        /// <param name="resourceCreator"> The resource-creator. </param>
        /// <param name="children"> The children layerage. </param>
        /// <returns> The rendered layer. </returns>
        ICanvasImage GetActualRender(ICanvasResourceCreator resourceCreator, IList<Layerage> children);

        /// <summary>
        /// Gets a specific rended-layer.
        /// </summary>
        /// <param name="resourceCreator"> The resource-creator. </param>
        /// <param name="children"> The children layerage. </param>
        /// <returns> The rendered layer. </returns>
        ICanvasImage GetRender(ICanvasResourceCreator resourceCreator, IList<Layerage> children);


        /// <summary>
        /// Draw lines on bound.
        /// </summary>
        /// <param name="resourceCreator"> The resource-creator. </param>
        /// <param name="drawingSession"> The drawing-session. </param>
        /// <param name="matrix"> The matrix. </param>
        /// <param name="accentColor"> The accent color. </param>
        /// <param name="children"> The children layerage. </param>
        void DrawBound(ICanvasResourceCreator resourceCreator, CanvasDrawingSession drawingSession, Matrix3x2 matrix, IList<Layerage> children, Windows.UI.Color accentColor);


        /// <summary>
        /// Create a specific geometry.
        /// </summary>
        /// <param name="resourceCreator"> The resource-creator. </param>
        /// <returns> The product geometry. </returns>   
        CanvasGeometry CreateGeometry(ICanvasResourceCreator resourceCreator);
        /// <summary>
        /// Create a specific geometry.
        /// </summary>
        /// <param name="resourceCreator"> The resource-creator. </param>
        /// <param name="canvasToVirtualMatrix"> The canvas-to-virtual matrix. </param>
        /// <returns> The product geometry. </returns>   
        CanvasGeometry CreateGeometry(ICanvasResourceCreator resourceCreator, Matrix3x2 canvasToVirtualMatrix);
        

        /// <summary>
        /// Returns whether the area filled by the layer contains the specified point.
        /// </summary>
        bool FillContainsPoint(Layerage layerage, Vector2 point);


        #endregion


        #region Node


        /// <summary>
        ///  Convert to curves.
        /// </summary>
        /// <returns> The product curves. </returns>
        IEnumerable<IEnumerable<Node>> ConvertToCurves();
        /// <summary>
        /// Draw nodes.
        /// </summary>
        /// <param name="drawingSession"> The drawing-session. </param>
        /// <param name="matrix"> The matrix. </param>
        /// <param name="accentColor"> The accent color. </param>
        void DrawNode(CanvasDrawingSession drawingSession, Matrix3x2 matrix, Windows.UI.Color accentColor);
        

        /// <summary>
        /// Gets the all points by the NodeCollection contains the specified point. 
        /// </summary>
        /// <param name="point"> The input point. </param>
        /// <param name="matrix"> The matrix. </param>
        /// <returns> The NodeCollection mode. </returns>
        NodeCollectionMode ContainsNodeCollectionMode(Vector2 point, Matrix3x2 matrix);


        /// <summary>
        ///  Cache the NodeCollection's transformer.
        /// </summary>
        void NodeCacheTransform();
        /// <summary>
        ///  Transforms the node by the given vector.
        /// </summary>
        /// <param name="vector"> The add value use to summed. </param>
        void NodeTransformMultiplies(Matrix3x2 matrix);
        /// <summary>
        ///  Transforms the node by the given matrix.
        /// </summary>
        /// <param name="matrix"> The resulting matrix. </param>
        void NodeTransformAdd(Vector2 vector);


        /// <summary>
        /// Select only one node.
        /// </summary>
        /// <param name="point"> The point. </param>
        /// <param name="matrix"> The matrix. </param>
        bool NodeSelectionOnlyOne(Vector2 point, Matrix3x2 matrix);

        /// <summary>
        /// Check node which in the rect.
        /// </summary>
        /// <param name="boxRect"> The destination rectangle. </param>
        void NodeBoxChoose(TransformerRect boxRect);
        

        /// <summary>
        /// Move a node's point. 
        /// </summary>
        /// <param name="point"> The point. </param>
        void NodeMovePoint(Vector2 point);
        /// <summary>
        /// It controls the transformation of node contol point.
        /// </summary>
        /// <param name="mode"> The mode. </param>
        /// <param name="lengthMode"> The length mode. </param>
        /// <param name="angleMode"> The angle mode. </param>
        /// <param name="point"> The point. </param>
        /// <param name="isLeftControlPoint"> <see cref="Node.LeftControlPoint"/> or <see cref="Node.RightControlPoint"/>. </param>
        void NodeControllerControlPoint(SelfControlPointMode mode, EachControlPointLengthMode lengthMode, EachControlPointAngleMode angleMode, Vector2 point, bool isLeftControlPoint);


        /// <summary>
        /// Remove all checked nodes.
        /// </summary>
        NodeRemoveMode NodeRemoveCheckedNodes();
        /// <summary>
        /// Insert a new point between checked points
        /// </summary>
        void NodeInterpolationCheckedNodes();
        /// <summary>
        /// Sharpen all checked nodes.
        /// </summary>
        void NodeSharpCheckedNodes();
        /// <summary>
        /// Smoothly all checked nodes.
        /// </summary>
        void NodeSmoothCheckedNodes();


        #endregion

    }
}