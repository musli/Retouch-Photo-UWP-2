﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Retouch_Photo2.Elements
{
    /// <summary>
    /// <see cref = "Page" />'s <see cref = "LoadingControl" />.
    /// </summary>
    public sealed partial class LoadingControl : UserControl
    {

        #region DependencyProperty

        /// <summary>
        /// Gets or sets whether the <see cref = "LoadingControl" /> Visibility.
        /// </summary>
        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(nameof(IsActive), typeof(bool), typeof(ThemeControl), new PropertyMetadata(false, (sender, e) =>
        {
            LoadingControl con = (LoadingControl)sender;

            if (e.NewValue is bool value)
            {
                con.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                con.ProgressRing.IsActive = value;
            }
        }));

        #endregion

        /// <summary> <see cref = "Windows.UI.Xaml.Controls.TextBlock" /> s Text. </summary>
        public string Text { set => this.TextBlock.Text = value; get => this.TextBlock.Text; }


        public LoadingControl()
        {
            this.InitializeComponent();
        }
    }
}
