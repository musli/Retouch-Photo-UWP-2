﻿using Microsoft.Graphics.Canvas;
using Retouch_Photo2.Tools.Buttons;
using Retouch_Photo2.Tools.Pages;
using System.Numerics;
using Windows.UI.Xaml;

namespace Retouch_Photo2.Tools.Models
{
    /// <summary>
    /// <see cref="ITool"/>'s NoneTool.
    /// </summary>
    public class NoneTool : ITool
    {
        public ToolType Type => ToolType.None;
        public FrameworkElement Icon { get; } = null;
        public IToolButton Button { get; } = new NoneButton();
        public IToolPage Page { get; } = new NonePage();


        public void Starting(Vector2 point) { }
        public void Started(Vector2 startingPoint, Vector2 point) { }
        public void Delta(Vector2 startingPoint, Vector2 point) { }
        public void Complete(Vector2 startingPoint, Vector2 point, bool isSingleStarted) { }

        public void Draw(CanvasDrawingSession drawingSession) { }


        public void OnNavigatedTo() { }
        public void OnNavigatedFrom() { }
    }
}