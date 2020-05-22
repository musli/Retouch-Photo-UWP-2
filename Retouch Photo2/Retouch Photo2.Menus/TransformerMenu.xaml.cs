﻿using FanKit.Transformers;
using Retouch_Photo2.Elements;
using Retouch_Photo2.Layers;
using Retouch_Photo2.Tools;
using Retouch_Photo2.ViewModels;
using System;
using System.Numerics;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;

namespace Retouch_Photo2.Menus.Models
{
    /// <summary> 
    /// State of <see cref="TransformerMenu"/>. 
    /// </summary>
    public enum TransformerMenuState
    {
        /// <summary> Enabled. </summary>
        Enabled,

        /// <summary> Disabled radian. </summary>
        EnabledWithoutRadian,

        /// <summary> Disabled. </summary>
        Disabled
    }

    /// <summary>
    /// Retouch_Photo2's the only <see cref = "TransformerMenu" />. 
    /// </summary>
    public sealed partial class TransformerMenu : UserControl, IMenu
    {
        //@ViewModel
        ViewModel ViewModel => App.ViewModel;
        TipViewModel TipViewModel => App.TipViewModel;
        SettingViewModel SettingViewModel => App.SettingViewModel ;

        bool IsRatio => this.SettingViewModel.IsRatio;
        Transformer SelectionTransformer { get => this.ViewModel.Transformer; set => this.ViewModel.Transformer = value; }
               

        //@VisualState
        bool _vsDisabledTool;
        bool _vsDisabledRadian;
        Transformer _vsTransformer;
        ListViewSelectionMode _vsMode;
        public TransformerMenuState VisualState
        {
            get
            {
                if (this._vsDisabledTool) return TransformerMenuState.Disabled;

                switch (this._vsMode)
                {
                    case ListViewSelectionMode.None: return TransformerMenuState.Disabled;
                    case ListViewSelectionMode.Single:
                    case ListViewSelectionMode.Multiple:
                        {
                            if (this._vsDisabledRadian) return TransformerMenuState.EnabledWithoutRadian;
                            else return TransformerMenuState.Enabled;
                        }
                }

                return TransformerMenuState.Enabled;
            }
            set => this.SetTransformerMenuState(value);
        }
        
        bool _isLoaded;
        IndicatorMode IndicatorMode = IndicatorMode.LeftTop;
        

        #region DependencyProperty


        /// <summary> Gets or sets <see cref = "TransformerMenu" />'s tool. </summary>
        public ITool Tool
        {
            get { return (ITool)GetValue(ToolProperty); }
            set { SetValue(ToolProperty, value); }
        }
        /// <summary> Identifies the <see cref = "TransformerMenu.Tool" /> dependency property. </summary>
        public static readonly DependencyProperty ToolProperty = DependencyProperty.Register(nameof(Tool), typeof(ITool), typeof(TransformerMenu), new PropertyMetadata(null, (sender, e) =>
        {
            return;
            TransformerMenu con = (TransformerMenu)sender;

            if (e.NewValue is ITool value)
            {
                switch (value.Type)
                {
                    case ToolType.Cursor:
                    case ToolType.View:
                    case ToolType.GeometryRectangle:
                    case ToolType.GeometryEllipse:
                        {
                            con._vsDisabledTool = false;
                            con.VisualState = con.VisualState;//State
                            return;
                        }
                }
            }

            con._vsDisabledTool = true;
            con.VisualState = con.VisualState;//State
            return;
        }));

        
        /// <summary> Gets or sets <see cref = "TransformerMenu" />'s selection mode. </summary>
        public ListViewSelectionMode Mode
        {
            get { return (ListViewSelectionMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }
        /// <summary> Identifies the <see cref = "TransformerMenu.Mode" /> dependency property. </summary>
        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(nameof(Mode), typeof(ListViewSelectionMode), typeof(TransformerMenu), new PropertyMetadata(ListViewSelectionMode.None, (sender, e) =>
        {
            TransformerMenu con = (TransformerMenu)sender;

            if (e.NewValue is ListViewSelectionMode value)
            {
                con._vsMode = value;
                con.VisualState = con.VisualState;//State
            }
        }));


        /// <summary> Gets or sets <see cref = "TransformerMenu" />'s transformer. </summary>
        public Transformer Transformer
        {
            get { return (Transformer)GetValue(TransformerProperty); }
            set { SetValue(TransformerProperty, value); }
        }
        /// <summary> Identifies the <see cref = "TransformerMenu.Transformer" /> dependency property. </summary>
        public static readonly DependencyProperty TransformerProperty = DependencyProperty.Register(nameof(Transformer), typeof(Transformer), typeof(TransformerMenu), new PropertyMetadata(new Transformer(), (sender, e) =>
        {
            TransformerMenu con = (TransformerMenu)sender;

            if (e.NewValue is Transformer value)
            {
                con._vsTransformer = value;
                con.VisualState = con.VisualState;//State
            }
        }));



        public bool DisabledRadian
        {
            get { return (bool)GetValue(DisabledRadianProperty); }
            set { SetValue(DisabledRadianProperty, value); }
        }
        /// <summary> Identifies the <see cref = "TransformerMenu.DisabledRadian" /> dependency property. </summary>
        public static readonly DependencyProperty DisabledRadianProperty = DependencyProperty.Register(nameof(DisabledRadian), typeof(bool), typeof(TransformerMenu), new PropertyMetadata(false, (sender, e) =>
        {
            TransformerMenu con = (TransformerMenu)sender;

            if (e.NewValue is bool value)
            {
                con._vsDisabledRadian = value;
                con.VisualState = con.VisualState;//State
            }
        }));


        #endregion


        //@Construct
        public TransformerMenu()
        {
            this.InitializeComponent();
            this.DataContext = this.ViewModel;
            this.ConstructDataContext
            (
                 path: nameof(this.ViewModel.SelectionMode),
                 dp: TransformerMenu.ModeProperty
            );
            this.ConstructDataContext
            (
                 path: nameof(this.ViewModel.DisabledRadian),
                 dp: TransformerMenu.DisabledRadianProperty
            );
            this.ConstructDataContext
            (
                 path: nameof(this.ViewModel.Transformer),
                 dp: TransformerMenu.TransformerProperty
            );
            this.ConstructStrings();
            this.ConstructToolTip();
            this.ConstructMenu();

            this.Loaded += (s, e) => this._isLoaded = true;

            this.ConstructWidthHeight();
            this.ConstructRadianSkew();
            this.ConstructXY();


            this.ConstructIndicatorControl();


            this.ConstructPositionRemoteControl();
            this.PositionRemoteButton.Click += (s, e) =>
            {
                this._Expander.IsSecondPage = true;
                this._Expander.CurrentTitle = (string)this.PositionRemoteToolTip.Content;
            };
        }

    }

    /// <summary> 
    /// Retouch_Photo2's the only <see cref = "TransformerMenu" />. 
    /// </summary>
    public sealed partial class TransformerMenu : UserControl, IMenu
    {
        //DataContext
        public void ConstructDataContext(string path, DependencyProperty dp)
        {
            // Create the binding description.
            Binding binding = new Binding
            {
                Mode = BindingMode.OneWay,
                Path = new PropertyPath(path)
            };

            // Attach the binding to the target.
            this.SetBinding(dp, binding);
        }

        //Strings
        private void ConstructStrings()
        {
            ResourceLoader resource = ResourceLoader.GetForCurrentView();

            this._button.ToolTip.Content =
            this._Expander.Title =
            this._Expander.CurrentTitle = resource.GetString("/Menus/Transformer");

            this.WidthTextBlock.Text = resource.GetString("/Menus/Transformer_Height");
            this.HeightTextBlock.Text = resource.GetString("/Menus/Transformer_Width");
            this.RatioScalingToolTip.Content = resource.GetString("/Menus/Transformer_RatioScaling");

            this.RotateTextBlock.Text = resource.GetString("/Menus/Transformer_Rotate");
            this.SkewTextBlock.Text = resource.GetString("/Menus/Transformer_Skew");
            this.StepFrequencyToolTip.Content = resource.GetString("/Menus/Transformer_StepFrequency");

            this.XTextBlock.Text = resource.GetString("/Menus/Transformer_X");
            this.YTextBlock.Text = resource.GetString("/Menus/Transformer_Y");
            this.PositionRemoteToolTip.Content = resource.GetString("/Menus/Transformer_PositionRemote");
           
            this.IndicatorToolTip.Content = resource.GetString("/Menus/Transformer_Indicator");
        }

        //ToolTip
        private void ConstructToolTip()
        {
            this._button.ToolTip.Opened += (s, e) =>
            {
                if (this._Expander.IsSecondPage) return;

                if (this.Expander.State == ExpanderState.Overlay)
                {
                    this.RatioScalingToolTip.IsOpen = true;
                    this.StepFrequencyToolTip.IsOpen = true;
                    this.PositionRemoteToolTip.IsOpen = true;
                    this.IndicatorToolTip.IsOpen = true;
                }
            };
            this._button.ToolTip.Closed += (s, e) =>
            {
                this.RatioScalingToolTip.IsOpen = false;
                this.StepFrequencyToolTip.IsOpen = false;
                this.PositionRemoteToolTip.IsOpen = false;
                this.IndicatorToolTip.IsOpen = false;
            };
        }

        //Menu
        public MenuType Type => MenuType.Transformer;
        public IExpander Expander => this._Expander;
        MenuButton _button = new MenuButton
        {
            CenterContent = new FanKit.Transformers.Icon()
        };

        public void ConstructMenu()
        {
            this._Expander.Layout = this;
            this._Expander.Button = this._button;
            this._Expander.Initialize();
        }
    }

    /// <summary> 
    /// Retouch_Photo2's the only <see cref = "TransformerMenu" />. 
    /// </summary>
    public sealed partial class TransformerMenu : UserControl, IMenu
    {

        //TransformerMenu
        private void SetTransformerMenuState(TransformerMenuState value)
        {

            switch (value)
            {
                case TransformerMenuState.Enabled:
                    {
                        //Value
                        {
                            Vector2 horizontal = this._vsTransformer.Horizontal;
                            Vector2 vertical = this._vsTransformer.Vertical;

                            //Radians
                            float radians = this.GetRadians(horizontal);
                            this.RPicker.Value = (int)radians;

                            //Skew
                            float skew = this.GetSkew(vertical, radians);
                            this.SPicker.Value = (int)skew;

                            //Width Height
                            this.WPicker.Value = (int)horizontal.Length();
                            this.HPicker.Value = (int)vertical.Length();

                            //X Y
                            Vector2 vector = this.GetVectorWithIndicatorMode(this.Transformer, this.IndicatorMode);
                            this.XPicker.Value = (int)vector.X;
                            this.YPicker.Value = (int)vector.Y;

                            //Indicator
                            this.IndicatorControl.Radians = radians;
                        }
                        //IsEnabled
                        {
                            this.WPicker.IsEnabled = true;
                            this.HPicker.IsEnabled = true;

                            this.RPicker.IsEnabled = true;
                            this.SPicker.IsEnabled = true;

                            this.XPicker.IsEnabled = true;
                            this.YPicker.IsEnabled = true;

                            this.RatioToggleControl.IsEnabled = true;
                            this.StepFrequencyButton.IsEnabled = true;
                            this.PositionRemoteButton.IsEnabled = true;
                            this.IndicatorControl.Mode = this.IndicatorMode;
                        }
                    }
                    break;

                case TransformerMenuState.EnabledWithoutRadian:
                    {
                        //Value
                        {
                            Vector2 horizontal = this._vsTransformer.Horizontal;
                            Vector2 vertical = this._vsTransformer.Vertical;

                            //Radians
                            this.RPicker.Value = 0;

                            //Skew
                            this.SPicker.Value = 0;

                            //Width Height
                            this.WPicker.Value = (int)horizontal.Length();
                            this.HPicker.Value = (int)vertical.Length();

                            //X Y
                            Vector2 vector = this.GetVectorWithIndicatorMode(this.Transformer, this.IndicatorMode);
                            this.XPicker.Value = (int)vector.X;
                            this.YPicker.Value = (int)vector.Y;

                            //Indicator
                            this.IndicatorControl.Radians = 0;
                        }
                        //IsEnabled
                        {
                            this.WPicker.IsEnabled = true;
                            this.HPicker.IsEnabled = true;

                            this.RPicker.IsEnabled = false;
                            this.SPicker.IsEnabled = false;

                            this.XPicker.IsEnabled = true;
                            this.YPicker.IsEnabled = true;

                            this.RatioToggleControl.IsEnabled = true;
                            this.StepFrequencyButton.IsEnabled = true;
                            this.PositionRemoteButton.IsEnabled = true;
                            this.IndicatorControl.Mode = this.IndicatorMode;
                        }
                    }
                    break;

                case TransformerMenuState.Disabled:
                    {
                        //Value
                        {
                            //Radians
                            this.RPicker.Value = 0;

                            //Skew
                            this.SPicker.Value = 0;

                            //Width Height
                            this.WPicker.Value = 0;
                            this.HPicker.Value = 0;

                            //X Y
                            this.XPicker.Value = 0;
                            this.YPicker.Value = 0;

                            //Indicator
                            this.IndicatorControl.Radians = 0;
                        }
                        //IsEnabled
                        {
                            this.WPicker.IsEnabled = false;
                            this.HPicker.IsEnabled = false;

                            this.RPicker.IsEnabled = false;
                            this.SPicker.IsEnabled = false;

                            this.XPicker.IsEnabled = false;
                            this.YPicker.IsEnabled = false;

                            this.RatioToggleControl.IsEnabled = false;
                            this.StepFrequencyButton.IsEnabled = false;
                            this.PositionRemoteButton.IsEnabled = false;
                            this.IndicatorControl.Mode = IndicatorMode.None;
                        }
                    }
                    break;

            }

        }


        //RemoteControl
        private void ConstructPositionRemoteControl()
        {
            Vector2 remote(Vector2 value) =>
                (Math.Abs(value.X) > Math.Abs(value.Y)) ?
                new Vector2(value.X, 0) :
                new Vector2(0, value.Y);

            this.PositionRemoteControl.Moved += (s, value) => this.ViewModel.MethodTransformAdd(value);//Method
            this.PositionRemoteControl.ValueChangeStarted += (s, value) => this.ViewModel.MethodTransformAddStarted();//Method
            this.PositionRemoteControl.ValueChangeDelta += (s, value) => this.ViewModel.MethodTransformAddDelta(remote(value));//Method
            this.PositionRemoteControl.ValueChangeCompleted += (s, value) => this.ViewModel.MethodTransformAddComplete(remote(value));//Method
        }


        //IndicatorControl
        private void ConstructIndicatorControl()
        {

            this.IndicatorControl.ModeChanged += (s, mode) =>
            {
                {
                    IndicatorMode startingMode = this.IndicatorMode;

                    HorizontalAlignment newHorizontal = this.GetHorizontalAlignmentFormIndicatorMode(mode);
                    HorizontalAlignment oldHorizontal = this.GetHorizontalAlignmentFormIndicatorMode(startingMode);

                    VerticalAlignment newVertical = this.GetVerticalAlignmentFormIndicatorMode(mode);
                    VerticalAlignment oldVertical = this.GetVerticalAlignmentFormIndicatorMode(startingMode);

                    if (this._isLoaded)
                    {
                        if (newHorizontal != oldHorizontal) this.XEaseStoryboard.Begin();//Storyboard
                        if (newVertical != oldVertical) this.YEaseStoryboard.Begin();//Storyboard
                    }
                }

                this.IndicatorMode = mode;//IndicatorMode

                if (this.ViewModel.SelectionMode == ListViewSelectionMode.None) return;

                Transformer transformer = this.SelectionTransformer;
                Vector2 vector = this.GetVectorWithIndicatorMode(transformer, this.IndicatorMode);

                this.XPicker.Value = (int)vector.X;
                this.YPicker.Value = (int)vector.Y;
            };

        }


        //Width Height
        private void ConstructWidthHeight()
        {

            this.WPicker.Minimum = 1;
            this.WPicker.Maximum = int.MaxValue;
            this.WPicker.ValueChange += (sender, value) =>
            {
                Transformer transformer = this.SelectionTransformer;
                Vector2 horizontal = transformer.Horizontal;
                Vector2 vector = this.GetVectorWithIndicatorMode(transformer, this.IndicatorMode);

                float canvasStartingRadian = FanKit.Math.VectorToRadians(transformer.CenterTop - transformer.Center);
                float canvasStartingWidth = horizontal.Length();
                float scale = value / canvasStartingWidth;

                Matrix3x2 matrix =
                Matrix3x2.CreateRotation(-canvasStartingRadian, vector) *
                Matrix3x2.CreateScale(this.IsRatio ? scale : 1, scale, vector) *
                Matrix3x2.CreateRotation(canvasStartingRadian, vector);
                
                //Method
                this.ViewModel.MethodTransformMultiplies(matrix);
            };


            this.HPicker.Minimum = 1;
            this.HPicker.Maximum = int.MaxValue;
            this.HPicker.ValueChange += (s, value) =>
            {
                Transformer transformer = this.SelectionTransformer;
                Vector2 vertical = transformer.Vertical;
                Vector2 vector = this.GetVectorWithIndicatorMode(transformer, this.IndicatorMode);

                float canvasStartingRadian = FanKit.Math.VectorToRadians(transformer.CenterTop - transformer.Center);
                float canvasStartingWidth = vertical.Length();
                float scale = value / canvasStartingWidth;

                Matrix3x2 matrix =
                Matrix3x2.CreateRotation(-canvasStartingRadian, vector) *
                Matrix3x2.CreateScale(scale, this.IsRatio ? scale : 1, vector) *
                Matrix3x2.CreateRotation(canvasStartingRadian, vector);
                
                //Method
                this.ViewModel.MethodTransformMultiplies(matrix);
            };

        }


        //Radian Skew
        private void ConstructRadianSkew()
        {

            this.RPicker.Minimum = -180;
            this.RPicker.Maximum = 180;
            this.RPicker.ValueChange += (s, value) =>
            {
                Transformer transformer = this.SelectionTransformer;
                Vector2 vector = this.GetVectorWithIndicatorMode(transformer, this.IndicatorMode);

                float canvasRadian = value / 180.0f * FanKit.Math.Pi;
                float canvasStartingRadian = FanKit.Math.VectorToRadians(transformer.CenterTop - transformer.Center);

                float radian = canvasRadian - canvasStartingRadian - FanKit.Math.PiOver2;
                Matrix3x2 matrix = Matrix3x2.CreateRotation(radian, vector);
                
                //Method
                this.ViewModel.MethodTransformMultiplies(matrix);
            };

            this.SPicker.Minimum = -90;
            this.SPicker.Maximum = 90;
            this.SPicker.ValueChange += (s, value) =>
            {
                Transformer transformer = this.SelectionTransformer;
                float horizontalHalf = Vector2.Distance(transformer.Center, transformer.CenterRight);

                Vector2 footPoint = FanKit.Math.FootPoint(transformer.Center, transformer.LeftBottom, transformer.RightBottom);
                float verticalHalf = Vector2.Distance(transformer.Center, footPoint);

                Vector2 horizontal = transformer.Horizontal;
                float radians = this.GetRadians(horizontal) / 180.0f * FanKit.Math.Pi;
                float skew = -value / 180.0f * FanKit.Math.Pi;

                //Vector2
                Vector2 postion;
                Vector2 center;
                switch (this.IndicatorMode)
                {
                    case IndicatorMode.LeftTop:
                    case IndicatorMode.Top:
                    case IndicatorMode.RightTop:
                            postion = new Vector2(-horizontalHalf, 0);
                            center = transformer.CenterTop;
                        break;
                    case IndicatorMode.LeftBottom:
                    case IndicatorMode.Bottom:
                    case IndicatorMode.RightBottom:
                            postion = new Vector2(-horizontalHalf, -verticalHalf * 2);
                            center = transformer.CenterBottom;
                        break;
                    default:
                            postion = new Vector2(-horizontalHalf, -verticalHalf);
                            center = transformer.Center;
                        break;
                }

                //Matrix
                Matrix3x2 matrix =
                Matrix3x2.CreateSkew(skew, 0) *
                Matrix3x2.CreateRotation(radians) *
                Matrix3x2.CreateTranslation(center);
                Transformer zeroTransformer = new Transformer(horizontalHalf * 2, verticalHalf * 2, postion);
                
                //Method
                this.ViewModel.MethodTransformMultiplies(matrix);
            };

        }


        //X Y
        private void ConstructXY()
        {

            this.XPicker.Minimum = int.MinValue;
            this.XPicker.Maximum = int.MaxValue;
            this.XPicker.ValueChange += (s, value) =>
            {
                Transformer transformer = this.SelectionTransformer;
                Vector2 indicator = this.GetVectorWithIndicatorMode(transformer, this.IndicatorMode);
                Vector2 vector = new Vector2(value - indicator.X, 0);

                //Method
                this.ViewModel.MethodTransformAdd(vector);
            };

            this.YPicker.Minimum = int.MinValue;
            this.YPicker.Maximum = int.MaxValue;
            this.YPicker.ValueChange += (s, value) =>
            {
                Transformer transformer = this.SelectionTransformer;
                Vector2 indicator = this.GetVectorWithIndicatorMode(transformer, this.IndicatorMode);
                Vector2 vector = new Vector2(0, value - indicator.Y);

                //Method
                this.ViewModel.MethodTransformAdd(vector);
            };

        }

    }
       
    /// <summary> 
    /// Retouch_Photo2's the only <see cref = "TransformerMenu" />. 
    /// </summary>
    public sealed partial class TransformerMenu : UserControl, IMenu
    {

        /// <summary>
        /// Gets vector by left, right, top, bottom.
        /// </summary>
        /// <param name="value"> Transformer </param>
        /// <param name="mode"> IndicatorMode </param>
        /// <returns></returns>
        private Vector2 GetVectorWithIndicatorMode(Transformer value, IndicatorMode mode)
        {
            switch (this.IndicatorMode)
            {
                case IndicatorMode.LeftTop: return value.LeftTop;
                case IndicatorMode.RightTop: return value.RightTop;
                case IndicatorMode.RightBottom: return value.RightBottom;
                case IndicatorMode.LeftBottom: return value.LeftBottom;

                case IndicatorMode.Left: return value.CenterLeft;
                case IndicatorMode.Top: return value.CenterTop;
                case IndicatorMode.Right: return value.CenterRight;
                case IndicatorMode.Bottom: return value.CenterBottom;

                case IndicatorMode.Center: return value.Center;
            }
            return value.LeftTop;
        }


        private float GetRadians(Vector2 vector)
        {
            float radians = FanKit.Math.VectorToRadians(vector);
            if (float.IsNaN(radians)) return 0.0f;

            float value = radians * 180.0f / FanKit.Math.Pi;
            return value % 180.0f;
        }

        private float GetSkew(Vector2 vector, float radians)
        {
            float skew = FanKit.Math.VectorToRadians(vector);
            if (float.IsNaN(skew)) return 0;

            skew = skew * 180.0f / FanKit.Math.Pi;
            skew = skew - radians - 90.0f;

            return skew % 180.0f;
        }


        //@Debug
        //Indicator
        private HorizontalAlignment GetHorizontalAlignmentFormIndicatorMode(IndicatorMode mode)
        {
            switch (mode)
            {
                case IndicatorMode.None: return HorizontalAlignment.Center;
                case IndicatorMode.LeftTop: return HorizontalAlignment.Left;
                case IndicatorMode.RightTop: return HorizontalAlignment.Right;
                case IndicatorMode.RightBottom: return HorizontalAlignment.Right;
                case IndicatorMode.LeftBottom: return HorizontalAlignment.Left;
                case IndicatorMode.Left: return HorizontalAlignment.Left;
                case IndicatorMode.Top: return HorizontalAlignment.Center;
                case IndicatorMode.Right: return HorizontalAlignment.Right;
                case IndicatorMode.Bottom: return HorizontalAlignment.Center;
                case IndicatorMode.Center: return HorizontalAlignment.Center;
                default: return HorizontalAlignment.Center;
            }
        }
        private VerticalAlignment GetVerticalAlignmentFormIndicatorMode(IndicatorMode mode)
        {
            switch (mode)
            {
                case IndicatorMode.None: return VerticalAlignment.Center;
                case IndicatorMode.LeftTop: return VerticalAlignment.Top;
                case IndicatorMode.RightTop: return VerticalAlignment.Top;
                case IndicatorMode.RightBottom: return VerticalAlignment.Bottom;
                case IndicatorMode.LeftBottom: return VerticalAlignment.Bottom;
                case IndicatorMode.Left: return VerticalAlignment.Center;
                case IndicatorMode.Top: return VerticalAlignment.Top;
                case IndicatorMode.Right: return VerticalAlignment.Center;
                case IndicatorMode.Bottom: return VerticalAlignment.Bottom;
                case IndicatorMode.Center: return VerticalAlignment.Center;
                default: return VerticalAlignment.Center;
            }
        }

    }
    
}