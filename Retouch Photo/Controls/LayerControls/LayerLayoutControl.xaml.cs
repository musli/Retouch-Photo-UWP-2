﻿using Retouch_Photo.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Retouch_Photo.Controls.LayerControls
{
    public sealed partial class LayerLayoutControl : UserControl
    {

        public UIElement Frist { get => this.FristBoder.Child; set => this.FristBoder.Child = value; }
        public UIElement Second { get => this.SecondBoder.Child; set => this.SecondBoder.Child = value; }
        public UIElement Third { get => this.ThirdBoder.Child; set => this.ThirdBoder.Child = value; }


        #region DependencyProperty

        public Layer Layer
        {
            get { return (Layer)GetValue(LayerProperty); }
            set { SetValue(LayerProperty, value); }
        }
        public static readonly DependencyProperty LayerProperty = DependencyProperty.Register(nameof(Layer), typeof(Layer), typeof(LayerLayoutControl), new PropertyMetadata(null));

        #endregion

        //Delegate
        public delegate void FlyoutShowHandler(UserControl control,Layer layer);
        public event FlyoutShowHandler FlyoutShow = null;


        public LayerLayoutControl()
        {
            this.InitializeComponent();
        }


        //Flyout
        private Grid element;
        private void Grid_Holding(object sender, HoldingRoutedEventArgs e) => this.FlyoutShow?.Invoke(this, this.Layer);
        private void Grid_RightTapped(object sender, RightTappedRoutedEventArgs e) => this.FlyoutShow?.Invoke(this, this.Layer);
        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (sender is Grid element)
            {
                if (this.element == element)
                {
                    this.FlyoutShow?.Invoke(this,this.Layer);
                }
            }
            this.element = (Grid)sender;
        }


    }
}
