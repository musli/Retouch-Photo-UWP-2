﻿using FanKit.Transformers;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Retouch_Photo2.Layers.Icons;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Xml.Linq;

namespace Retouch_Photo2.Layers.Models
{
    /// <summary>
    /// <see cref="IGeometryLayer"/>'s GeometryCurveLayer .
    /// </summary>
    public class GeometryCurveLayer : IGeometryLayer, ILayer
    {
        //@Content       
        public string Type => "GeometryCurveLayer";

        public NodeCollection NodeCollection { get; private set; }
        
        //@Construct
        /// <summary>
        /// Construct a curve layer.
        /// </summary>
        /// <param name="nodes"> The source nodes. </param>
        public GeometryCurveLayer(IEnumerable<Node> nodes)
        {
            base.Control = new LayerControl(this)
            {
                Icon = new GeometryCurveIcon(),
                Text = "Curve",
            };

            this.NodeCollection = new NodeCollection(nodes); 
        }
        /// <summary>
        /// Construct a curve layer from a line.
        /// </summary>
        /// <param name="left"> The first source vector. </param>
        /// <param name="right"> The second source vector. </param>
        public GeometryCurveLayer(Vector2 left, Vector2 right)
        {
            base.Control = new LayerControl(this)
            {
                Icon = new GeometryCurveIcon(),
                Text = "Curve",
            };

            this.NodeCollection = new NodeCollection(left, right);
        }


        //@Override
        public override void CacheTransform()
        {
            base.CacheTransform();

            this.NodeCollection.CacheTransform();
        }
        public override void TransformMultiplies(Matrix3x2 matrix)
        {
            base.TransformMultiplies(matrix);

            this.NodeCollection.TransformMultiplies(matrix);
        }
        public override void TransformAdd(Vector2 vector)
        {
            base.TransformAdd(vector);

            this.NodeCollection.TransformAdd(vector);
        }


        public override Transformer GetActualDestinationWithRefactoringTransformer
        {
            get
            {
                if (this.IsRefactoringTransformer)
                {
                    Transformer transformer = LayerCollection.RefactoringTransformer(this.NodeCollection);
                    this.TransformManager.Source = transformer;
                    this.TransformManager.Destination = transformer;

                    this.IsRefactoringTransformer = false;
                }

                return base.GetActualDestinationWithRefactoringTransformer;
            }
        }

        public override CanvasGeometry CreateGeometry(ICanvasResourceCreator resourceCreator, Matrix3x2 canvasToVirtualMatrix)
        {
            return this.NodeCollection.CreateGeometry(resourceCreator).Transform(canvasToVirtualMatrix);
        }

        public ILayer Clone(ICanvasResourceCreator resourceCreator)
        {
            GeometryCurveLayer curveLayer = new GeometryCurveLayer( this.NodeCollection)
            {
                NodeCollection = new NodeCollection(from node in this.NodeCollection select node)
            };

            LayerBase.CopyWith(resourceCreator, curveLayer, this);
            return curveLayer;
        }

        public XElement Save()
        {
            XElement element = new XElement("GeometryCurveLayer");
            
            element.Add(new XElement("NodeCollection", this.NodeCollection));

            LayerBase.SaveWidth(element, this);
            return element;
        }

    }
}