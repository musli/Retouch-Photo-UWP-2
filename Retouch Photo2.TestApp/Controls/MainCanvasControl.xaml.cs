﻿using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Retouch_Photo2.Layers;
using Retouch_Photo2.Library;
using Retouch_Photo2.TestApp.ViewModels;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Retouch_Photo2.TestApp.Controls
{
    /// <summary> 
    /// Retouch_Photo2's the only <see cref = "MainCanvasControl" />. 
    /// </summary>
    public sealed partial class MainCanvasControl : UserControl
    {
        //ViewModel
        ViewModel ViewModel => Retouch_Photo2.TestApp.App.ViewModel;

        //Single
        bool isSingleStarted;
        Vector2 singleStartingPoint;
        //Right
        Vector2 rightStartPoint;
        Vector2 rightStartPosition;
        //Double
        Vector2 doubleStartCenter;
        Vector2 doubleStartPosition;
        float doubleStartScale;
        float doubleStartSpace;


        #region DependencyProperty


        /// <summary> Gets or sets <see cref = "MainCanvasControl" />'s shadow color. </summary>
        public Color ShadowColor
        {
            get { return (Color)GetValue(ShadowColorProperty); }
            set { SetValue(ShadowColorProperty, value); }
        }
        /// <summary> Identifies the <see cref = "MainCanvasControl.ShadowColor" /> dependency property. </summary>
        public static readonly DependencyProperty ShadowColorProperty = DependencyProperty.Register(nameof(ShadowColor), typeof(Color), typeof(MainCanvasControl), new PropertyMetadata(Colors.Black, (sender, e) =>
        {
            MainCanvasControl con = (MainCanvasControl)sender;

            if (e.NewValue is Color value)
            {
                con.CanvasControl.Invalidate();
            }
        }));


        #endregion

        public MainCanvasControl()
        {
            this.InitializeComponent();
            this.SizeChanged += (s, e) => 
            {
                if (e.NewSize == e.PreviousSize) return;
                this.ViewModel.MatrixTransformer.Size = e.NewSize;
            };
               

            //Draw
            this.ViewModel.InvalidateAction = (mode) =>
            {
                switch (mode)
                {
                    case InvalidateMode.Thumbnail:
                        this.CanvasControl.DpiScale = 0.5f;
                        break;
                    case InvalidateMode.HD:
                        this.CanvasControl.DpiScale = 1.0f;
                        break;
                }

                this.CanvasControl.Invalidate();
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                //Render : Blank Image
                ICanvasImage previousImage = new ColorSourceEffect { Color = Colors.White };
                Matrix3x2 canvasToVirtualMatrix = this.ViewModel.MatrixTransformer.GetMatrix(MatrixTransformerMode.CanvasToVirtual);

                //Mezzanine :
                //   If the mezzanine is turned on, 
                //   Insert a MezzanineLayer between the Layers
                if (this.ViewModel.IsMezzanine)
                {
                    for (int i = this.ViewModel.Layers.Count - 1; i >= 0; i--)
                    {
                        if (this.ViewModel.MezzanineIndex == i)
                        {
                            previousImage = Layer.Render(this.ViewModel.CanvasDevice, this.ViewModel.MezzanineLayer, previousImage, canvasToVirtualMatrix);
                        }

                        previousImage = Layer.Render(this.ViewModel.CanvasDevice, this.ViewModel.Layers[i], previousImage, canvasToVirtualMatrix);
                    }
                }
                else
                {
                    for (int i = this.ViewModel.Layers.Count - 1; i >= 0; i--)
                    {
                        previousImage = Layer.Render(this.ViewModel.CanvasDevice, this.ViewModel.Layers[i], previousImage, canvasToVirtualMatrix);
                    }
                }


                //Crop : Get the border from MatrixTransformer
                float width = this.ViewModel.MatrixTransformer.Width * this.ViewModel.MatrixTransformer.Scale;
                float height = this.ViewModel.MatrixTransformer.Height * this.ViewModel.MatrixTransformer.Scale;
                ICanvasImage cropRect = new CropEffect
                {
                    Source = previousImage,
                    SourceRectangle = new Rect(-width / 2, -height / 2, width, height),
                };


                //Final : Draw to Canvas
                ICanvasImage finalCanvas = new Transform2DEffect
                {
                    Source = cropRect,
                    TransformMatrix = this.ViewModel.MatrixTransformer.GetMatrix(MatrixTransformerMode.VirtualToControl)
                };
                ICanvasImage shadow = new ShadowEffect
                {
                    Source = finalCanvas,
                    ShadowColor = this.ShadowColor,
                    BlurAmount = 4.0f
                };
                args.DrawingSession.DrawImage(shadow, 5.0f, 5.0f);
                args.DrawingSession.DrawImage(finalCanvas);
            };


            //Single
            this.CanvasOperator.Single_Start += (point) =>
            {
                this.isSingleStarted = false;
                this.singleStartingPoint = point;
                this.ViewModel.Tool.Starting(point);//Starting
            };
            this.CanvasOperator.Single_Delta += (point) =>
            {
                //Delta
                if (this.isSingleStarted)
                {
                    this.ViewModel.Tool.Delta(this.singleStartingPoint, point);
                    return;
                }

                //Started
                if ((this.singleStartingPoint - point).LengthSquared() > 400.0f)
                {
                    this.isSingleStarted = true;
                    this.ViewModel.Tool.Started(this.singleStartingPoint, point);
                }
            };
            this.CanvasOperator.Single_Complete += (point) => this.ViewModel.Tool.Complete(this.singleStartingPoint, point, this.isSingleStarted);


            //Right
            this.CanvasOperator.Right_Start += (point) =>
            {
                this.rightStartPoint = point;
                this.rightStartPosition = this.ViewModel.MatrixTransformer.Position;

                this.ViewModel.Invalidate(InvalidateMode.Thumbnail);
            };
            this.CanvasOperator.Right_Delta += (point) =>
            {
                this.ViewModel.MatrixTransformer.Position = this.rightStartPosition - this.rightStartPoint + point;

                this.ViewModel.Invalidate();
            };
            this.CanvasOperator.Right_Complete += (point) => this.ViewModel.Invalidate(InvalidateMode.HD);


            //Double
            this.CanvasOperator.Double_Start += (center, space) =>
            {
                this.doubleStartCenter = (center - this.ViewModel.MatrixTransformer.Position) / this.ViewModel.MatrixTransformer.Scale + new Vector2(this.ViewModel.MatrixTransformer.ControlWidth / 2, this.ViewModel.MatrixTransformer.ControlHeight / 2);
                this.doubleStartPosition = this.ViewModel.MatrixTransformer.Position;

                this.doubleStartSpace = space;
                this.doubleStartScale = this.ViewModel.MatrixTransformer.Scale;

                this.ViewModel.Invalidate(InvalidateMode.Thumbnail);
            };
            this.CanvasOperator.Double_Delta += (center, space) =>
            {
                this.ViewModel.MatrixTransformer.Scale = this.doubleStartScale / this.doubleStartSpace * space;

                this.ViewModel.MatrixTransformer.Position = center - (this.doubleStartCenter - new Vector2(this.ViewModel.MatrixTransformer.ControlWidth / 2, this.ViewModel.MatrixTransformer.ControlHeight / 2)) * this.ViewModel.MatrixTransformer.Scale;

                this.ViewModel.Invalidate();
            };
            this.CanvasOperator.Double_Complete += (center, space) => this.ViewModel.Invalidate(InvalidateMode.HD);


            //Wheel
            this.CanvasOperator.Wheel_Changed += (point, space) =>
            {
                if (space > 0)
                {
                    if (this.ViewModel.MatrixTransformer.Scale < 10f)
                    {
                        this.ViewModel.MatrixTransformer.Scale *= 1.1f;
                        this.ViewModel.MatrixTransformer.Position = point + (this.ViewModel.MatrixTransformer.Position - point) * 1.1f;
                    }
                }
                else
                {
                    if (this.ViewModel.MatrixTransformer.Scale > 0.1f)
                    {
                        this.ViewModel.MatrixTransformer.Scale /= 1.1f;
                        this.ViewModel.MatrixTransformer.Position = point + (this.ViewModel.MatrixTransformer.Position - point) / 1.1f;
                    }
                }

                this.ViewModel.Invalidate();
            };
        }
    }
}