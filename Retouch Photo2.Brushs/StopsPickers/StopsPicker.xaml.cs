﻿using HSVColorPickers;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Retouch_Photo2.Elements;
using System;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Retouch_Photo2.Brushs
{
    /// <summary>
    /// Stops picker.
    /// </summary>
    public sealed partial class StopsPicker : UserControl
    {
        //@Delegate
        /// <summary> Occurs when the stops value changed. </summary>
        public event EventHandler<CanvasGradientStop[]> StopsChanged;
        /// <summary> Occurs when the stops change starts. </summary>
        public event EventHandler<CanvasGradientStop[]> StopsChangeStarted;
        /// <summary> Occurs when stops change. </summary>
        public event EventHandler<CanvasGradientStop[]> StopsChangeDelta;
        /// <summary> Occurs when the stops change is complete. </summary>
        public event EventHandler<CanvasGradientStop[]> StopsChangeCompleted;


        //@Content
        /// <summary> StopsPicker's ColorPicker. </summary>
        public ColorPicker2 ColorPicker => this._ColorPicker;
        /// <summary> StopsPicker's ColorFlyout. </summary>
        public Flyout ColorFlyout => this._ColorFlyout;


        //Background
        CanvasRenderTarget GrayAndWhiteBackground;

        StopsSize Size = new StopsSize();
        StopsManager Manager = new StopsManager();


        private CanvasGradientStop[] array;
        /// <summary>
        /// Set a brush value for the control.
        /// </summary>
        /// <param name="value"> brush </param>
        public void SetArray(CanvasGradientStop[] value)
        {
            if (value != null)
            {
                this.Manager.InitializeDate(value);
                CanvasGradientStop stop = value.First();
                this.StopChanged(stop.Color, (int)(stop.Position * 100), false);
            }

            this.array = value;
        }


        //@Construct
        /// <summary>
        /// Initializes a StopsPicker. 
        /// </summary>
        public StopsPicker()
        {
            this.InitializeComponent();
            this.ConstructStrings();
            this.ConstructCanvas();
            this.ConstructCanvasOperator();

            this.ConstructStop();


            //Color    
            this._ColorPicker.ColorChanged += (s, color) =>
            {
                bool isSucces = this.SetColor(color);
                if (isSucces) this.StopsChanged?.Invoke(this, this.array);//Delegate
            };

            this._ColorPicker.ColorChangeStarted += (s, color) =>
            {
                bool isSucces = this.SetColor(color);
                if (isSucces) this.StopsChangeStarted?.Invoke(this, this.array);//Delegate
            };
            this._ColorPicker.ColorChangeDelta += (s, color) =>
            {
                bool isSucces = this.SetColor(color);
                if (isSucces) this.StopsChangeDelta?.Invoke(this, this.array);//Delegate
            };
            this._ColorPicker.ColorChangeCompleted += (s, color) =>
            {
                bool isSucces = this.SetColor(color);
                if (isSucces) this.StopsChangeCompleted?.Invoke(this, this.array);//Delegate
            };

            this.ColorEllipse.Tapped += (s, e) =>
            {
                if (this.array == null) return;

                if (this.Manager.IsLeft || this.Manager.IsRight || this.Manager.Index >= 0)
                {
                    this._ColorPicker.Color = this.ColorEllipse.Color;
                    this._ColorFlyout.ShowAt(this.ColorEllipse);//Flyout
                }
            };


            //Offset         
            this.OffsetPicker.ValueChanged += (s, value) =>
            {
                float offset = value / 100.0f;               
                bool isSucces = this.SetOffset(offset);
                if (isSucces) this.StopsChanged?.Invoke(this, this.array);//Delegate
            };
            //Alpha
            this.AlphaPicker.ValueChanged += (s, value) =>
            {
                Color color = this.ColorEllipse.Color;
                color.A = (byte)value;
                bool isSucces = this.SetColor(color);
                if (isSucces) this.StopsChanged?.Invoke(this, this.array);//Delegate
            };
        }
    }
}