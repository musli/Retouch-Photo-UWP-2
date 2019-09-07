﻿using FanKit.Transformers;
using Microsoft.Graphics.Canvas;
using Retouch_Photo2.Brushs;
using Retouch_Photo2.Layers.Models;
using System.Numerics;

namespace Retouch_Photo2.Tools.Models
{
    /// <summary>
    /// <see cref="ITool"/>'s PenTool.
    /// </summary>
    public partial class PenTool : ITool
    {
        Vector2 _previewLeft;
        Vector2 _previewRight;

        /// <summary> Only the left point. </summary>
        bool _hasPreviewTempLeftPoint;

        public void PreviewStart(Vector2 canvasPoint)
        {
            if (this._hasPreviewTempLeftPoint == false) this._previewLeft = canvasPoint;
            this._previewRight = canvasPoint;
        }
        public void PreviewDelta(Vector2 canvasPoint)
        {
            this._previewRight = canvasPoint;
        }
        public void PreviewComplete(Vector2 canvasStartingPoint, Vector2 canvasPoint, bool isSingleStarted)
        {
            if (this._hasPreviewTempLeftPoint)
            {
                this._hasPreviewTempLeftPoint = false;
                this.CreateLayer(this._previewLeft, canvasPoint);
            }
            else if (isSingleStarted)
            {
                this._hasPreviewTempLeftPoint = false;
                this.CreateLayer(canvasStartingPoint, canvasPoint);
            }
            else
            {
                this._hasPreviewTempLeftPoint = true;
            }
        }

        /// <summary>
        /// Draw a line before creating a curve layer.
        /// </summary>
        /// <param name="drawingSession"> The drawing-session. </param>
        public void PreviewDraw(CanvasDrawingSession drawingSession)
        {
            Matrix3x2 matrix = this.ViewModel.CanvasTransformer.GetMatrix();
            Vector2 lineLeft = Vector2.Transform(this._previewLeft, matrix);

            if (this._hasPreviewTempLeftPoint)
            {
                drawingSession.DrawNode3(lineLeft);
            }
            else
            {
                Vector2 lineRight = Vector2.Transform(this._previewRight, matrix);

                drawingSession.DrawLineDodgerBlue(lineLeft, lineRight);
                drawingSession.DrawNode3(lineLeft);
                drawingSession.DrawNode3(lineRight);
            }
        }

        private void CreateLayer(Vector2 canvasStartingPoint, Vector2 canvasPoint)
        {
            //Transformer
            Transformer transformer = new Transformer(canvasPoint, canvasStartingPoint);

            //Layer
            CurveLayer curveLayer = new CurveLayer(canvasStartingPoint, canvasPoint)
            {
                IsChecked = true,
                StrokeWidth = 1,
                StrokeBrush = new Brush
                {
                    Type = BrushType.Color,
                    Color = this.SelectionViewModel.FillColor,
                },

                Source = transformer,
                Destination = transformer,
            };

            //Selection
            this.SelectionViewModel.SetValue((layer) =>
            {
                layer.IsChecked = false;
            });

            //Insert
            int index = this.MezzanineViewModel.GetfFrstIndex(this.ViewModel.Layers);
            this.ViewModel.Layers.Insert(index, curveLayer);

            this.SelectionViewModel.SetModeSingle(curveLayer);//Selection
            this.ViewModel.Invalidate();//Invalidate
        }

    }
}