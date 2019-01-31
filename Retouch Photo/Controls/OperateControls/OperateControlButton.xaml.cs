﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Retouch_Photo.Controls.OperateControls
{
    public sealed partial class OperateControlButton : UserControl
    {
        //@Delegate
        public event TappedEventHandler ButtonTapped;

        public UIElement EnabledIcon { get => this.EnabledViewbox.Child; set => this.EnabledViewbox.Child = value; }
        public UIElement DisabledIcon { get => this.DisabledViewbox.Child; set => this.DisabledViewbox.Child = value; }
        public string Label
        {
            set => ToolTipService.SetToolTip(this, value);
        }

        public bool ButtonIsEnabled
        {
            set
            {
                this.Button.IsEnabled = value;

                this.EnabledViewbox.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                this.DisabledViewbox.Visibility = value ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public OperateControlButton()
        {
            this.InitializeComponent();
        }

        private void Button_Tapped(object sender, TappedRoutedEventArgs e) => this.ButtonTapped?.Invoke(sender, e);

    }
}