﻿using FanKit.Transformers;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Retouch_Photo2.Layers;
using Retouch_Photo2.ViewModels;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Retouch_Photo2.Controls
{
    /// <summary> 
    /// Retouch_Photo2's the only <see cref = "MainCanvasControl" />. 
    /// </summary>
    public sealed partial class MainCanvasControl : UserControl
    {
        //@ViewModel
        ViewModel ViewModel => App.ViewModel;
        MezzanineViewModel MezzanineViewModel => App.MezzanineViewModel;
        TipViewModel TipViewModel => App.TipViewModel;


        bool isSingleStarted;
        Vector2 singleStartingPoint;


        #region DependencyProperty


        /// <summary> Gets or sets <see cref = "MainCanvasControl" />'s accent color. </summary>
        public Color AccentColor
        {
            get { return (Color)GetValue(AccentColorProperty); }
            set { SetValue(AccentColorProperty, value); }
        }
        /// <summary> Identifies the <see cref = "MainCanvasControl.AccentColor" /> dependency property. </summary>
        public static readonly DependencyProperty AccentColorProperty = DependencyProperty.Register(nameof(AccentColor), typeof(Color), typeof(MainCanvasControl), new PropertyMetadata(Colors.DodgerBlue, (sender, e) =>
        {
            MainCanvasControl con = (MainCanvasControl)sender;

            if (e.NewValue is Color value)
            {
                con.ViewModel.AccentColor = value;
                con.CanvasControl.Invalidate();
            }
        }));


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


        /// <summary> Sets or Gets the on state of the ruler on the <see cref = "MainCanvasControl" />. </summary>
        public bool RulerVisible
        {
            get { return (bool)GetValue(RulerVisibleProperty); }
            set { SetValue(RulerVisibleProperty, value); }
        }
        /// <summary> Identifies the <see cref = "MainCanvasControl.RulerVisible" /> dependency property. </summary>
        public static readonly DependencyProperty RulerVisibleProperty = DependencyProperty.Register(nameof(RulerVisible), typeof(bool), typeof(MainCanvasControl), new PropertyMetadata(false, (sender, e) =>
        {
            MainCanvasControl con = (MainCanvasControl)sender;

            if (e.NewValue is bool value)
            {
                con.CanvasControl.Invalidate();
            }
        }));


        #endregion


        //@Construct
        public MainCanvasControl()
        {
            this.InitializeComponent();
            this.SizeChanged += (s, e) =>
            {
                if (e.NewSize == e.PreviousSize) return;
                this.ViewModel.CanvasTransformer.Size = e.NewSize;
            };


            #region Draw


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
            this.CanvasControl.CreateResources += (sender, args) =>
            {
                this.ViewModel.CanvasTransformer.Size = new Size(sender.ActualWidth, sender.ActualHeight);
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                //Render & Mezzanine
                {
                    ICanvasImage previousImage = new ColorSourceEffect { Color = Colors.White };

                    Matrix3x2 canvasToVirtualMatrix = this.ViewModel.CanvasTransformer.GetMatrix(MatrixTransformerMode.CanvasToVirtual);

                    void aaa() =>
                      previousImage = Layer.Render(this.ViewModel.CanvasDevice, this.MezzanineViewModel.Layer, previousImage, canvasToVirtualMatrix);

                    void bbb(int i) =>
                        previousImage = Layer.Render(this.ViewModel.CanvasDevice, this.ViewModel.Layers[i], previousImage, canvasToVirtualMatrix);


                    //Mezzanine 
                    if (this.MezzanineViewModel.Layer != null)
                    {
                        if (this.ViewModel.Layers.Count == 0) aaa();
                        else
                        {
                            for (int i = this.ViewModel.Layers.Count - 1; i >= 0; i--)
                            {
                                bbb(i);

                                if (this.MezzanineViewModel.Index == i) aaa();//Mezzanine
                            }
                        }
                    }
                    else
                    {
                        for (int i = this.ViewModel.Layers.Count - 1; i >= 0; i--)
                        {
                            bbb(i);
                        }
                    }

                    //Draw
                    args.DrawingSession.DrawCrad(previousImage, this.ViewModel.CanvasTransformer, this.ShadowColor);
                }


                //Tool & Mezzanine
                {
                    if (this.MezzanineViewModel.Layer == null)
                    {
                        Matrix3x2 matrix = this.ViewModel.CanvasTransformer.GetMatrix();

                        foreach (ILayer layer in this.ViewModel.Layers)
                        {
                            if (layer.IsChecked)
                            {
                                layer.DrawBound(sender, args.DrawingSession, matrix, this.ViewModel.AccentColor);
                            }
                        }

                        //Tool
                        this.TipViewModel.Tool.Draw(args.DrawingSession);
                    }
                    else
                    {
                        //Mezzanine 
                        Matrix3x2 matrix = this.ViewModel.CanvasTransformer.GetMatrix();
                        this.MezzanineViewModel.Layer.DrawBound(sender, args.DrawingSession, matrix, this.AccentColor);
                    }
                }


                //IsRuler
                if (this.RulerVisible) args.DrawingSession.DrawRuler(this.ViewModel.CanvasTransformer);
            };


            #endregion


            #region CanvasOperator


            //Single
            this.CanvasOperator.Single_Start += (point) =>
            {
                this.isSingleStarted = false;
                this.singleStartingPoint = point;

                //Tool
                this.TipViewModel.Tool.Starting(point);//Starting

                this.ViewModel.CanvasHitTestVisible = false;//IsHitTestVisible
            };
            this.CanvasOperator.Single_Delta += (point) =>
            {
                //Delta
                if (this.isSingleStarted)
                {
                    //Tool
                    this.TipViewModel.Tool.Delta(this.singleStartingPoint, point);//Delta

                    return;
                }

                //Started
                if (FanKit.Math.OutNodeDistance(this.singleStartingPoint, point))
                {
                    this.isSingleStarted = true;

                    //Tool
                    this.TipViewModel.Tool.Started(this.singleStartingPoint, point);//Started
                }
            };
            this.CanvasOperator.Single_Complete += (point) =>
            {
                //Tool
                this.TipViewModel.Tool.Complete(this.singleStartingPoint, point, this.isSingleStarted);//Complete

                this.ViewModel.CanvasHitTestVisible = true;//IsHitTestVisible
            };


            //Right
            this.CanvasOperator.Right_Start += (point) =>
            {
                this.ViewModel.CanvasTransformer.CacheMove(point);
                this.ViewModel.CanvasHitTestVisible = false;//IsHitTestVisible
            };
            this.CanvasOperator.Right_Delta += (point) =>
            {
                this.ViewModel.CanvasTransformer.Move(point);
                this.ViewModel.Invalidate();//Invalidate
            };
            this.CanvasOperator.Right_Complete += (point) =>
            {
                this.ViewModel.CanvasTransformer.Move(point);
                this.ViewModel.Invalidate(InvalidateMode.HD);//Invalidate
                this.ViewModel.CanvasHitTestVisible = true;//IsHitTestVisible
            };


            //Double
            this.CanvasOperator.Double_Start += (center, space) =>
            {
                this.ViewModel.CanvasTransformer.CachePinch(center, space);

                this.ViewModel.NotifyCanvasTransformerScale();//Notify
                this.ViewModel.Invalidate(InvalidateMode.Thumbnail);

                this.ViewModel.CanvasHitTestVisible = false;//IsHitTestVisible
            };
            this.CanvasOperator.Double_Delta += (center, space) =>
            {
                this.ViewModel.CanvasTransformer.Pinch(center, space);

                this.ViewModel.NotifyCanvasTransformerScale();//Notify
                this.ViewModel.Invalidate();//Invalidate
            };
            this.CanvasOperator.Double_Complete += (center, space) =>
            {
                this.ViewModel.NotifyCanvasTransformerScale();//Notify
                this.ViewModel.Invalidate(InvalidateMode.HD);

                this.ViewModel.CanvasHitTestVisible = true;//IsHitTestVisible
            };

            //Wheel
            this.CanvasOperator.Wheel_Changed += (point, space) =>
            {
                if (space > 0)
                    this.ViewModel.CanvasTransformer.ZoomIn(point);
                else
                    this.ViewModel.CanvasTransformer.ZoomOut(point);

                this.ViewModel.NotifyCanvasTransformerScale();//Notify
                this.ViewModel.Invalidate();//Invalidate
            };


            #endregion

        }
    }
}