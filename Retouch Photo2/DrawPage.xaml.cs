﻿using FanKit.Transformers;
using Retouch_Photo2.Elements;
using Retouch_Photo2.Layers;
using Retouch_Photo2.Layers.Models;
using Retouch_Photo2.Menus;
using Retouch_Photo2.Tools;
using Retouch_Photo2.ViewModels;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using Windows.Foundation.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI;
using System.Linq;
using Windows.Foundation;
using Retouch_Photo2.Tools.Elements;
using System;
using System.Linq;
using System.Xml.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.System;
using Windows.Storage;
using Windows.Storage.Streams;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Windows.ApplicationModel.DataTransfer;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.UI.Controls;

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
        SettingViewModel SettingViewModel => App.SettingViewModel;
        SelectionViewModel SelectionViewModel => App.SelectionViewModel;


        //@Converter
        public Visibility BoolToVisibilityConverter(bool isChecked) => isChecked ? Visibility.Visible : Visibility.Collapsed;


        //@Construct
        public DrawPage()
        {
            this.InitializeComponent();
            MoreTransformButton.Flyout = this.MoreTransformFlyout;
            MoreCreateButton.Flyout = this.MoreCreateFlyout;

            //Appbar
            this.DrawLayout.BackButton.Tapped += (s, e) => this.Frame.GoBack();
            this.SaveButton.Tapped += (s, e) =>
            {
                //1、创建一个XDocument对象  
                XDocument xDoc = new XDocument
                {
                    Declaration = new XDeclaration("1.0", "utf-8", "no"),//设置xml的文档定义  
                };

                //2、创建一个根部
                XElement root = new XElement("Root");
                xDoc.Add(root);

                //3、宽高
                root.Add(new XElement("Width", this.ViewModel.CanvasTransformer.Width));
                root.Add(new XElement("Height", this.ViewModel.CanvasTransformer.Height));

                //4、图层
                XElement layers = new XElement("Layers");
                root.Add(layers);
                {
                    foreach (ILayer layer in this.ViewModel.Layers.RootLayers)
                    {
                        XElement element = layer.Save();
                        layers.Add(element);
                    }
                }
                
                //5、保存
                string path = ApplicationData.Current.LocalFolder.Path + "/" + "Unsdasd" + ".photo2"; //将XML文件加载进来
                xDoc.Save(path);
            };

            this.ThemeButton.Loaded += (s, e) =>
            {
                ElementTheme theme = this.SettingViewModel.ElementTheme;
                this.ThemeControl.Theme = theme;
            };
            this.ThemeButton.Tapped += (s, e) =>
            {
                //Trigger switching theme.
                ElementTheme theme = this.ThemeControl.Theme;
                theme = (theme == ElementTheme.Dark) ? ElementTheme.Light : ElementTheme.Dark;

                this.RequestedTheme = theme;
                this.ThemeControl.Theme = theme;
                ApplicationViewTitleBarBackgroundExtension.SetTheme(theme);
            };


            //FullScreen
            this.UnFullScreenButton.Tapped += (s, e) => this.DrawLayout.IsFullScreen = !DrawLayout.IsFullScreen;
            this.FullScreenButton.Tapped += (s, e) => this.DrawLayout.IsFullScreen = !this.DrawLayout.IsFullScreen;
            this.DrawLayout.FullScreenChanged += (isFullScreen, vector) =>
            {
                if (isFullScreen)
                    this.ViewModel.CanvasTransformer.Position += vector;
                else
                    this.ViewModel.CanvasTransformer.Position -= vector;

                this.ViewModel.CanvasTransformer.ReloadMatrix();
            };

            
            //Layers
            this.LayersControl.WidthButton.Tapped += (s, e) => this.DrawLayout.PadChangeLayersWidth();


            //Tool
            foreach (ITool tool in this.TipViewModel.Tools)
            {
                this.ConstructTool(tool);
            }
            this.TooLeft.Add(this.MoreToolButton);
            this.ToolFirst();


            //Menu
            foreach (IMenu menu in this.TipViewModel.Menus)
            {
                this.ConstructMenu(menu);
            }
            this.OverlayCanvas.Tapped += (s, e) => this.MenusHideAndCrop(isCrop: false);
            this.OverlayCanvas.SizeChanged += (s, e) => this.MenusHideAndCrop(isCrop: true);
        }


        #region Tool


        MoreToolButton MoreToolButton = new MoreToolButton();
        UIElementCollection TooLeft => this.DrawLayout.LeftPaneChildren;

        ToolButtonType _tempToolButtonType;
 
        private void ConstructTool(ITool tool)
        {
            ToolButtonType type;
            UIElement button = null;

            if (tool == null)
            {
                type = this._tempToolButtonType;
                button = new Rectangle
                {
                    Fill = new SolidColorBrush(Colors.Gray),
                    Height = 1,
                    Opacity = 0.2,
                    Margin = new Thickness(4, 0, 4, 0),
                };
            }
            else
            {
                type = tool.Button.Type;
                button = tool.Button.Self;

                this._tempToolButtonType = tool.Button.Type;
                tool.Button.Self.Tapped += (s, e) =>
                {
                    //Group
                    this.ToolGroupType(tool.Type);

                    //Changed
                    this.TipViewModel.Tool = tool;
                    this.DrawLayout.LeftIcon = tool.Icon;

                    this.ViewModel.Invalidate();//Invalidate
                };
            }

            switch (type)
            {
                case ToolButtonType.None:
                    this.TooLeft.Add(button);
                    break;
                case ToolButtonType.Second:
                    this.MoreToolButton.Add(button);
                    break;
            }
        }


        private void ToolGroupType(ToolType currentType)
        {
            foreach (ITool tool in this.TipViewModel.Tools)
            {
                if (tool != null)
                {
                    bool isSelected = (tool.Type == currentType);

                    tool.Button.IsSelected = isSelected;
                    tool.Page.IsSelected = isSelected;
                }
            }
        }

        private void ToolFirst()
        {
            ITool tool = this.TipViewModel.Tools.FirstOrDefault();
            if (tool != null)
            {
                //Group
                tool.Button.IsSelected = true;
                tool.Page.IsSelected = true;

                //Changed
                this.TipViewModel.Tool = tool;
                this.DrawLayout.LeftIcon = tool.Icon;
                                
                this.FootPageControl.Content = tool.Page.Self;//FootPage
            }
        }


        #endregion


        #region Menu

        UIElementCollection MennuHead => this.DrawLayout.HeadRightChildren;

        public void ConstructMenu(IMenu menu)
        {
            if (menu == null) return;

            this.OverlayCanvas.Children.Add(menu.Layout.Self);

            menu.Move += () =>
            {
                //Move to top
                int index = this.OverlayCanvas.Children.IndexOf(menu.Layout.Self);
                int count = this.OverlayCanvas.Children.Count;
                this.OverlayCanvas.Children.Move((uint)index, (uint)count - 1);
            };
            menu.Opened += () => this.MenusDisable(menu);//Menus is disable
            menu.Closed += () => this.MenusEnable();//Menus is enable

            //MenuButton
            switch (menu.Button.Type)
            {
                case MenuButtonType.None:
                    this.MennuHead.Add(menu.Button.Self);
                    break;
                case MenuButtonType.LayersControlIndicator:
                    this.LayersControl.IndicatorBorder.Child = menu.Button.Self;
                    break;
            }
        }

        private void MenusHideAndCrop( bool isCrop)
        {
            foreach (IMenu menu in this.TipViewModel.Menus)
            {
                switch (menu.State)
                {
                    case MenuState.FlyoutShow:
                        menu.State = MenuState.FlyoutHide;
                        break;
                    case MenuState.OverlayExpanded:
                    case MenuState.OverlayNotExpanded:
                        if (isCrop)
                        {
                            Point postion = MenuHelper.GetOverlayPostion(menu.Layout.Self);
                            Point postion2 = MenuHelper.GetBoundPostion(postion, menu.Layout.Self);
                            MenuHelper.SetOverlayPostion(menu.Layout.Self, postion2);
                        }
                        break;
                }
            }
            this.OverlayCanvas.Background = null;
        }

        private void MenusDisable(IMenu currentMenu)
        {
            foreach (IMenu menu in this.TipViewModel.Menus)
            {
                if (menu.Type != currentMenu.Type)
                {
                    menu.IsHitTestVisible = false;
                }
            }
            this.OverlayCanvas.Background = new SolidColorBrush(Colors.Transparent);
        }
        private void MenusEnable()
        {
            foreach (IMenu menu in this.TipViewModel.Menus)
            {
                menu.IsHitTestVisible = true;
            }
            this.OverlayCanvas.Background = null;
        }


        #endregion

        //The current page becomes the active page
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //Theme
            ElementTheme theme = this.SettingViewModel.ElementTheme;
            this.RequestedTheme = theme;
            this.ThemeControl.Theme = theme;
            ApplicationViewTitleBarBackgroundExtension.SetTheme(theme);

            //Layout
            this.DrawLayout.VisualStateDeviceType = this.SettingViewModel.LayoutDeviceType;
            this.DrawLayout.VisualStatePhoneMaxWidth = this.SettingViewModel.LayoutPhoneMaxWidth;
            this.DrawLayout.VisualStatePadMaxWidth = this.SettingViewModel.LayoutPadMaxWidth;


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