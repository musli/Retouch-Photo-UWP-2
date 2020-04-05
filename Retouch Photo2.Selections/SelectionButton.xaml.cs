﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Retouch_Photo2.Selections
{
    /// <summary>
    /// Retouch_Photo2 Selections 's Button.
    /// </summary>
    public sealed partial class SelectionButton : UserControl
    {
        //@Content
        /// <summary> Enabled icon. </summary>
        public UIElement EnabledIcon { get => this.EnabledViewbox.Child; set => this.EnabledViewbox.Child = value; }
        /// <summary> Disabled icon. </summary>
        public UIElement DisabledIcon { get => this.DisabledViewbox.Child; set => this.DisabledViewbox.Child = value; }
        /// <summary> TextBlock' Text. </summary>
        public string Text { get => this.TextBlock.Text; set => this.TextBlock.Text = value; }


        //@VisualState
        bool _vsIsEnabled;
        ClickMode _vsClickMode;
        public VisualState VisualState
        {
            get
            {
                if (this._vsIsEnabled == false) return this.Disabled;

                switch (this._vsClickMode)
                {
                    case ClickMode.Release: return this.Normal;
                    case ClickMode.Hover: return this.PointerOver;
                    case ClickMode.Press: return this.Pressed;
                }
                return this.Normal;
            }
            set => VisualStateManager.GoToState(this, value.Name, false);
        }

        private ClickMode ClickMode
        {
            set
            {
                this._vsClickMode = value;
                this.VisualState = this.VisualState;//State
            }
        }


        //@Construct
        public SelectionButton()
        {
            this.InitializeComponent();
            this.Loaded += (s, e) =>
            {
                this._vsIsEnabled = base.IsEnabled;
                this.VisualState = this.VisualState;//State
            };
            this.IsEnabledChanged += (s, e) =>
            {
                this._vsIsEnabled = base.IsEnabled;
                this.VisualState = this.VisualState;//State
            };
            
            this.PointerEntered += (s, e) => this.ClickMode = ClickMode.Hover;
            this.PointerPressed += (s, e) => this.ClickMode = ClickMode.Press;
            this.PointerReleased += (s, e) => this.ClickMode = ClickMode.Release;
            this.PointerExited += (s, e) => this.ClickMode = ClickMode.Release;
        }
    }
}