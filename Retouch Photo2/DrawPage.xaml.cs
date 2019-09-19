﻿using Retouch_Photo2.Elements.DrawPages;
using Retouch_Photo2.Menus;
using Retouch_Photo2.Tools;
using Retouch_Photo2.ViewModels;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Retouch_Photo2
{
    /// <summary> 
    /// Retouch_Photo2's the only <see cref = "DrawPage" />. 
    /// </summary>
    public sealed partial class DrawPage : Page
    {
        //@ViewModel
        ViewModel ViewModel => App.ViewModel;
        KeyboardViewModel KeyboardViewModel => App.KeyboardViewModel;
        TipViewModel TipViewModel => App.TipViewModel;

        //@Converter
        public Visibility BoolToVisibilityConverter(bool isChecked) => isChecked ? Visibility.Visible : Visibility.Collapsed;
        public UserControl TouchbarConverter(ITouchbar touchbar)
        {
            if (touchbar == null) return null;
            else return touchbar.Self;
        }

        //@Construct
        public DrawPage()
        {
            this.InitializeComponent();
            this.Loaded += (s, e) => this.ThemeControl.ApplicationTheme = App.Current.RequestedTheme;

            //Appbar
            this.DrawLayout.BackButton.Tapped += (s, e) => this.Frame.GoBack();
            this.SaveButton.Tapped += (s, e) => this.Frame.GoBack();

            this.ThemeButton.Tapped += (s, e) => this.ThemeControl.Toggle();
            
            //FullScreen
            this.UnFullScreenButton.Tapped += (s, e) => this.DrawLayout.IsFullScreen = !DrawLayout.IsFullScreen;
            this.FullScreenButton.Tapped += (s, e) => this.DrawLayout.IsFullScreen = !this.DrawLayout.IsFullScreen;


            //Tool
            foreach (ITool tool in this.TipViewModel.Tools)
            {
                this.ConstructTool(tool);
            }

            //Menu
            foreach (IMenu menu in this.TipViewModel.Menus)
            {
                this.ConstructMenu(menu);
            }
        }


        #region Tool


        List<ITool> Tools = new List<ITool>();

        UIElementCollection TooLeft => this.DrawLayout.LeftPaneChildren;
        object LeftIcon { set => this.DrawLayout.LeftIcon = value; }
        Page FootPage { set => this.DrawLayout.FootPage = value; }
                
 
        private void ConstructTool(ITool tool)
        {
            if (tool == null) return;

            this.Tools.Add(tool);

            IToolButton button = tool.Button;

            if (button!=null)
            {
                button.Self.Tapped += (s, e) =>
                {
                    this.ToolGroupType(tool.Type);

                    this.LeftIcon = tool.Icon;
                    this.FootPage = tool.Page;
                    this.TipViewModel.Tool = tool;
                };

                this.TooLeft.Add(button.Self);
            }
        }

        private void ToolGroupType(ToolType groupType)
        {
            foreach (ITool tool in this.Tools)
            {
                if (tool == null) break;

                tool.IsSelected = (tool.Type == groupType);
            }

            this.ViewModel.Invalidate();//Invalidate

            this.TipViewModel.TouchbarType = TouchbarType.None;//Touchbar
        }


        #endregion


        #region Menu


        UIElementCollection MenuOverlay => this.OverlayCanvas.Children;
        UIElementCollection MenuHead => this.DrawLayout.HeadRightChildren;
        UIElement MenuLayersControl { set => this.DrawLayout.RightPane = value; }


        private void ConstructMenu(IMenu menu)
        {
            if (menu == null) return;
            
            //MenuOverlay
            UIElement overlay = menu.Overlay;
            this.MenuOverlay.Add(overlay);

            menu.Move += (s, e) =>
            {
                int index = this.MenuOverlay.IndexOf(menu.Overlay);
                int count = this.MenuOverlay.Count;
                this.MenuOverlay.Move((uint)index, (uint)count - 1);
            };

            //MenuButton
            IMenuButton menuButton = menu.Button;
            switch (menuButton.Type)
            {
                case MenuButtonType.None: this.MenuHead.Add(menuButton.Self); break;
                case MenuButtonType.ToolButton: this.TooLeft.Add(menuButton.Self); break;
                case MenuButtonType.LayersControl: this.MenuLayersControl = menuButton.Self; break;
            }
        }


        #endregion


        //The current page becomes the active page
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            return;
            if (e.Parameter is Project project)
            {
                if (project == null)
                {
                    base.Frame.GoBack();
                    return;
                }

             //   this.Loaded += (sender, e2) =>
                //{

            this.LoadingControl.Visibility = Visibility.Visible;//Loading
            this.ViewModel.LoadFromProject(project);//Project
            this.LoadingControl.Visibility = Visibility.Collapsed;//Loading   

            this.ViewModel.Invalidate();
               // };
            }
        }
        //The current page no longer becomes an active page
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
        }

    }
}