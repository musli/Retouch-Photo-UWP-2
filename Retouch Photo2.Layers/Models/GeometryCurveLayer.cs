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
        //@Static     
        public const string ID = "GeometryCurveLayer";

        //@Content 
        public NodeCollection Nodes { get; private set; }

        //@Construct
        /// <summary>
        /// Construct a curve-layer.
        /// </summary>
        /// <param name="element"> The source XElement. </param>
        public GeometryCurveLayer(XElement element) : this() => this.Load(element);
        /// <summary>
        /// Construct a curve-layer.
        /// </summary>
        private GeometryCurveLayer()
        {
            base.Type = GeometryCurveLayer.ID;
            base.Control = new LayerControl(this)
            {
                Icon = new GeometryCurveIcon(),
                Text = "Curve",
            };
        }
        /// <summary>
        /// Construct a curve-layer.
        /// </summary>
        /// <param name="nodes"> The source nodes. </param>
        public GeometryCurveLayer(IEnumerable<Node> nodes) : this() => this.Nodes = new NodeCollection(nodes);
        /// <summary>
        /// Construct a curve-layer from a line.
        /// </summary>
        /// <param name="left"> The first source vector. </param>
        /// <param name="right"> The second source vector. </param>
        public GeometryCurveLayer(Vector2 left, Vector2 right) : this() => this.Nodes = new NodeCollection(left, right);


        //@Override
        public override void CacheTransform()
        {
            base.CacheTransform();
            this.Nodes.CacheTransform();
        }
        public override void TransformMultiplies(Matrix3x2 matrix)
        {
            base.TransformMultiplies(matrix);
            this.Nodes.TransformMultiplies(matrix);
        }
        public override void TransformAdd(Vector2 vector)
        {
            base.TransformAdd(vector);
            this.Nodes.TransformAdd(vector);
        }


        public override Transformer GetActualDestinationWithRefactoringTransformer
        {
            get
            {
                if (this.IsRefactoringTransformer)
                {
                    Transformer transformer = LayerCollection.RefactoringTransformer(this.Nodes);
                    this.TransformManager.Source = transformer;
                    this.TransformManager.Destination = transformer;

                    this.IsRefactoringTransformer = false;
                }

                return base.GetActualDestinationWithRefactoringTransformer;
            }
        }

        public override CanvasGeometry CreateGeometry(ICanvasResourceCreator resourceCreator, Matrix3x2 canvasToVirtualMatrix)
        {
            return this.Nodes.CreateGeometry(resourceCreator).Transform(canvasToVirtualMatrix);
        }


        public ILayer Clone(ICanvasResourceCreator resourceCreator)
        {
            GeometryCurveLayer curveLayer = new GeometryCurveLayer
            {
                Nodes = this.Nodes.Clone()
            };

            LayerBase.CopyWith(resourceCreator, curveLayer, this);
            return curveLayer;
        }

        public void SaveWith(XElement element)
        {
            element.Add(new XElement
            (
                "Nodes",
                from n
                in this.Nodes
                select FanKit.Transformers.XML.SaveNode("Node", n)
            ));
        }
        public void Load(XElement element)
        {
            XElement node = element.Element("Nodes");

            this.Nodes = new NodeCollection
            (
                from n
                in node.Elements()
                select FanKit.Transformers.XML.LoadNode(n)
            );
        }

    }
}