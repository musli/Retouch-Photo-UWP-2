﻿using FanKit.Transformers;
using Microsoft.Graphics.Canvas;
using Retouch_Photo2.Elements;
using Retouch_Photo2.Layers;
using Retouch_Photo2.Layers.Models;
using Retouch_Photo2.Tools.Buttons;
using Retouch_Photo2.Tools.Icons;
using Retouch_Photo2.Tools.Pages;
using Retouch_Photo2.ViewModels;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Retouch_Photo2.Tools.Models
{
    /// <summary>
    /// <see cref="ITool"/>'s CropTool.
    /// </summary>
    public partial class CropTool : ITool
    {
        //@ViewModel
        ViewModel ViewModel => App.ViewModel;
        SelectionViewModel SelectionViewModel => App.SelectionViewModel;
        MezzanineViewModel MezzanineViewModel => App.MezzanineViewModel;
        KeyboardViewModel KeyboardViewModel => App.KeyboardViewModel;
        TipViewModel TipViewModel => App.TipViewModel;

        ITransformerTool TransformerTool => this.TipViewModel.TransformerTool;
        CompositeMode CompositeMode => this.KeyboardViewModel.CompositeMode;
        bool IsRatio => this.KeyboardViewModel.IsRatio;
        bool IsCenter => this.KeyboardViewModel.IsCenter;
        bool IsStepFrequency => this.KeyboardViewModel.IsStepFrequency;


        public bool IsSelected
        {
            set
            {
                this.Button.IsSelected = value;
                this._cropPage.IsSelected = value;
            }
        }
        public ToolType Type => ToolType.Crop;
        public FrameworkElement Icon { get; } = new CropIcon();
        public IToolButton Button { get; } = new CropButton();
        public Page Page => this._cropPage;
        CropPage _cropPage { get; } = new CropPage();


        ILayer _layer;
        bool _startingIsCrop;
        Transformer _startingDestination;
        Transformer _startingCropDestination;
        TransformerMode _transformerMode;
        Transformer _startingActualDestination => this._startingIsCrop ? this._startingCropDestination : this._startingDestination;
        

        public void Starting(Vector2 point)
        {

        }
        public void Started(Vector2 startingPoint, Vector2 point)
        {
            //Selection
            foreach (ILayer layer in this.ViewModel.Layers)
            {
                if (layer.IsChecked)
                {
                    //Transformer
                    Transformer transformer = layer.TransformManager.ActualDestination;
                    Matrix3x2 matrix = this.ViewModel.CanvasTransformer.GetMatrix();
                    bool dsabledRadian = false;
                    TransformerMode transformerMode = Transformer.ContainsNodeMode
                    (
                         startingPoint,
                         transformer,
                         matrix,
                         dsabledRadian
                    );

                    if (transformerMode != TransformerMode.None)
                    {
                        this._layer = layer;
                        this._startingDestination = layer.TransformManager.Destination;
                        this._startingIsCrop = layer.TransformManager.IsCrop;
                        this._startingCropDestination = layer.TransformManager.CropDestination;
                        this._transformerMode = transformerMode;

                        break;
                    }
                }
            }

            this.ViewModel.Invalidate(InvalidateMode.Thumbnail);//Invalidate
        }
        public void Delta(Vector2 startingPoint, Vector2 point)
        {
            if (this._layer == null) return;
            if (this._layer.TransformManager.DisabledRadian) return;
            if (this._transformerMode == TransformerMode.None) return;

            bool isTranslation = (this._transformerMode == TransformerMode.Translation);
            Matrix3x2 inverseMatrix = this.ViewModel.CanvasTransformer.GetInverseMatrix();

            //Transformer
            Transformer startingDestination;
            if (isTranslation)
            {
                startingDestination = this._startingDestination;
            }
            else
            {
                startingDestination = this._startingActualDestination;
            }
            
            Transformer transformer = Transformer.Controller
            (
                this._transformerMode,
                startingPoint,
                point,
                startingDestination,
                inverseMatrix,
                this.IsRatio,
                this.IsCenter,
                this.IsStepFrequency
            );

            //Crop
            this._layer.TransformManager.IsCrop = true;
            if (isTranslation)
            {
                this._layer.TransformManager.Destination = transformer;
                if (this._startingIsCrop == false)
                {
                    this._layer.TransformManager.CropDestination = this._startingDestination;
                }
            }
            else
            {
                this._layer.TransformManager.CropDestination = transformer;
            }

            this.SelectionViewModel.IsCrop = true;//Selection
            this.ViewModel.Invalidate();//Invalidate
        }
        public void Complete(Vector2 startingPoint, Vector2 point, bool isSingleStarted)
        {
            this._layer = null;
            this._transformerMode = TransformerMode.None;

            this.ViewModel.Invalidate(InvalidateMode.HD);//Invalidate
        }

        public void Draw(CanvasDrawingSession drawingSession)
        {
            Matrix3x2 matrix = this.ViewModel.CanvasTransformer.GetMatrix();

            foreach (ILayer layer in this.ViewModel.Layers)
            {
                if (layer.IsChecked)
                {
                    if (layer.TransformManager.DisabledRadian==false)
                    {
                        Transformer transformer = layer.TransformManager.ActualDestination;

                        //LTRB
                        Vector2 leftTop = Vector2.Transform(transformer.LeftTop, matrix);
                        Vector2 rightTop = Vector2.Transform(transformer.RightTop, matrix);
                        Vector2 rightBottom = Vector2.Transform(transformer.RightBottom, matrix);
                        Vector2 leftBottom = Vector2.Transform(transformer.LeftBottom, matrix);

                        _drawCrop(drawingSession, leftTop, rightTop, rightBottom, leftBottom, Colors.BlueViolet);
                    }
                }
            }
        }



        private static void _drawCrop(CanvasDrawingSession drawingSession, Vector2 leftTop, Vector2 rightTop, Vector2 rightBottom, Vector2 leftBottom, Windows.UI.Color accentColor)
        {
            //Line            
            //TODO: 更新Fankit后，删掉
            if (false)
            {
                // CanvasDrawingSessionExtensions._drawBound(drawingSession, leftTop, rightTop, rightBottom, leftBottom, accentColor);
            }
            else
            {
                drawingSession.DrawLine(leftTop, rightTop, accentColor);
                drawingSession.DrawLine(rightTop, rightBottom, accentColor);
                drawingSession.DrawLine(rightBottom, leftBottom, accentColor);
                drawingSession.DrawLine(leftBottom, leftTop, accentColor);
            }


            //Center
            Vector2 centerLeft = (leftTop + leftBottom) / 2;
            Vector2 centerTop = (leftTop + rightTop) / 2;
            Vector2 centerRight = (rightTop + rightBottom) / 2;
            Vector2 centerBottom = (leftBottom + rightBottom) / 2;
                       
            //Vertical Horizontal
            Vector2 vertical = centerBottom - centerTop;
            Vector2 horizontal = centerRight - centerLeft;

            Vector2 verticalUnit = vertical / vertical.Length();
            Vector2 horizontalUnit = horizontal / horizontal.Length();

            const float length = 10;
            Vector2 verticalLength = verticalUnit * length;
            Vector2 horizontalLength = horizontalUnit * length;

            const float space = 2;
            Vector2 verticalSpace = verticalUnit * space;
            Vector2 horizontalSpace = horizontalUnit * space;

            //Scale2
            {
                const float strokeWidth = 2;
                Vector2 leftTopOutside = leftTop - verticalSpace - horizontalSpace;
                Vector2 rightTopOutside = rightTop - verticalSpace + horizontalSpace;
                Vector2 rightBottomOutside = rightBottom + verticalSpace + horizontalSpace;
                Vector2 leftBottomOutside = leftBottom + verticalSpace - horizontalSpace;

                drawingSession.DrawLine(leftTopOutside, leftTopOutside + horizontalLength, accentColor, strokeWidth);
                drawingSession.DrawLine(leftTopOutside, leftTopOutside + verticalLength, accentColor, strokeWidth);
           
                drawingSession.DrawLine(rightTopOutside, rightTopOutside - horizontalLength, accentColor, strokeWidth);
                drawingSession.DrawLine(rightTopOutside, rightTopOutside + verticalLength, accentColor, strokeWidth);
          
                drawingSession.DrawLine(rightBottomOutside, rightBottomOutside - horizontalLength, accentColor, strokeWidth);
                drawingSession.DrawLine(rightBottomOutside, rightBottomOutside - verticalLength, accentColor, strokeWidth);
           
                drawingSession.DrawLine(leftBottomOutside, leftBottomOutside + horizontalLength, accentColor, strokeWidth);
                drawingSession.DrawLine(leftBottomOutside, leftBottomOutside - verticalLength, accentColor, strokeWidth);
            }

            //Scale1
            if (FanKit.Math.OutNodeDistance(centerLeft, centerRight))
            {
                Vector2 centerTopOutside = centerTop - verticalSpace;
                Vector2 centerBottomOutside = centerBottom + verticalSpace;
                drawingSession.DrawLine(centerTopOutside + horizontalLength, centerTopOutside - horizontalLength, accentColor, 2);
                drawingSession.DrawLine(centerBottomOutside + horizontalLength, centerBottomOutside - horizontalLength, accentColor, 2);
            }
            if (FanKit.Math.OutNodeDistance(centerTop, centerBottom))
            {
                Vector2 centerLeftOutside = centerLeft - horizontalSpace;
                Vector2 centerRightOutside = centerRight + horizontalSpace;
                drawingSession.DrawLine(centerLeftOutside + verticalLength, centerLeftOutside - verticalLength, accentColor, 2);
                drawingSession.DrawLine(centerRightOutside + verticalLength, centerRightOutside - verticalLength, accentColor, 2);
            }
        }




        public void OnNavigatedTo() { }
        public void OnNavigatedFrom()
        {
            // The transformer may change after the layer is cropped.
            // So, reset the transformer.
            this.SelectionViewModel.SetMode(this.ViewModel.Layers);//Selection
        }
    }
}