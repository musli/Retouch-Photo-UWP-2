﻿using Microsoft.Graphics.Canvas;
using Retouch_Photo.Models;
using Retouch_Photo.ViewModels;
using System.Numerics;
using static Retouch_Photo.Library.HomographyController;

namespace Retouch_Photo.Tools
{
    public class ICursorTool : ITool
    {
        //ViewModel
        DrawViewModel ViewModel => Retouch_Photo.App.ViewModel;

        public bool IsTransformer(TransformerMode mode)
        {
            if (mode == TransformerMode.None) return false;
            if (mode == TransformerMode.Translation) return false;
            return true;
        }

        public override bool Start(Vector2 point)
        {
            // Transformer
            Layer layer = this.ViewModel.CurrentLayer;
            if (layer == null)
            {
                this.ViewModel.TransformerMode = TransformerMode.None;
                return false;
            }
            else
            {
                TransformerMode mode = Transformer.ContainsNodeMode(point, layer.Transformer, this.ViewModel.MatrixTransformer.Matrix);
                if (this.IsTransformer(mode))
                {
                    this.ViewModel.TransformerMode = mode;
                    this.ViewModel.TransformerDictionary[mode].Start(point, layer, this.ViewModel.MatrixTransformer.Matrix, this.ViewModel.MatrixTransformer.InverseMatrix);
                    return true;
                }
            }
            return false;
        }
        public override bool Delta(Vector2 point)
        {
            if (this.ViewModel.TransformerMode == TransformerMode.None) return false;

            // Transformer
            Layer layer = this.ViewModel.CurrentLayer;
            if (layer == null)
            {
                return false;
            }
            else
            {
                TransformerMode mode = this.ViewModel.TransformerMode;
                this.ViewModel.TransformerDictionary[mode].Delta(point, layer, this.ViewModel.MatrixTransformer.Matrix, this.ViewModel.MatrixTransformer.InverseMatrix);

                this.ViewModel.Transformer = layer.Transformer;//Transformer
                this.ViewModel.Invalidate();
                return true;
            }
        }
        public override bool Complete(Vector2 point)
        {
            if (this.ViewModel.TransformerMode == TransformerMode.None) return false;

            // Transformer
            Layer layer = this.ViewModel.CurrentLayer;
            if (layer == null)
            {
                this.ViewModel.CurrentLayer = null;

                this.ViewModel.TransformerMode = TransformerMode.None;
                this.ViewModel.Invalidate();
                return false;
            }
            else
            {
                TransformerMode mode = this.ViewModel.TransformerMode;
                this.ViewModel.TransformerDictionary[mode].Complete(point, layer, this.ViewModel.MatrixTransformer.Matrix, this.ViewModel.MatrixTransformer.InverseMatrix);

                this.ViewModel.TransformerMode = TransformerMode.None;
                this.ViewModel.Invalidate();
                return true;
            }
        }

        public override bool Draw(CanvasDrawingSession ds)
        {
            //Transformer      
            Layer layer = this.ViewModel.CurrentLayer;
            if (layer == null)
            {
                return false;
            }
            else
            {
                Matrix3x2 matrix = this.ViewModel.MatrixTransformer.Matrix;

                layer.Draw(this.ViewModel.CanvasDevice, ds, matrix);
                Transformer.DrawBoundNodes(ds, layer.Transformer, matrix);
                return true;
            }
        }
    }
}
