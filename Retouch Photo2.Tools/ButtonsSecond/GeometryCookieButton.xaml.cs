﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Retouch_Photo2.Tools.Buttons
{
    /// <summary> 
    /// Button of <see cref = "GeometryCookieTool"/>.
    /// </summary>
    public sealed partial class GeometryCookieButton : UserControl, IToolButton
    {
        //@Content
        public bool IsSelected { set => this.Button.IsSelected = value; }
        public FrameworkElement Self => this;
        public ToolButtonType Type => ToolButtonType.Second;

        //@Construct
        public GeometryCookieButton()
        {
            this.InitializeComponent();
        }
    }
}