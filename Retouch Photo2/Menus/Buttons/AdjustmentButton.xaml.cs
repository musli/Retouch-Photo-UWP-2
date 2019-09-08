﻿using Retouch_Photo2.ViewModels;
using Retouch_Photo2.ViewModels.Tips;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Retouch_Photo2.Menus.Buttons
{
    public sealed partial class AdjustmentButton : UserControl, IMenuButton
    {
        //@ViewModel
        ViewModel ViewModel => App.ViewModel;
        TipViewModel TipViewModel => App.TipViewModel;

        //@Content
        public MenuState State
        {
            set
            {
                this.Button.Background = (value == MenuState.FlyoutShow) ? this.AccentColor : this.UnAccentColor;

                switch (value)
                {
                    case MenuState.FlyoutHide:
                        this.Button.Foreground = this.UnCheckColor;
                        break;
                    case MenuState.FlyoutShow:
                        this.Button.Foreground = this.ThreeStateColor;
                        break;
                    default:
                        this.Button.Foreground = this.CheckColor;
                        break;
                }
            }
        }
        public FrameworkElement Self => this;
        
        //@Construct
        public AdjustmentButton()
        {
            this.InitializeComponent();
        }
    }
}