﻿using Retouch_Photo.Controls.ToolControls;
using Retouch_Photo.Pages.ToolPages;
using Retouch_Photo.ViewModels.ToolViewModels;

namespace Retouch_Photo.Models.Tools
{
    public class PenTool : Tool
    {
        public PenTool()
        {
            base.Type = ToolType.Pen;
            base.Icon = new ToolPenControl();
            base.WorkIcon = new ToolPenControl();
            base.Page = new ToolPenPage();
            base.ViewModel = new ToolPenViewModel();
        }
    }
}