﻿using FanKit.Transformers;
using Retouch_Photo2.Layers;
using Retouch_Photo2.Layers.Models;
using Retouch_Photo2.ViewModels;
using Retouch_Photo2.ViewModels.Selections;
using System.Numerics;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;

namespace Retouch_Photo2.Tools.Pages
{
    /// <summary> 
    /// Page of <see cref = "ImagePage"/>.
    /// </summary>
    public sealed partial class ImagePage : Page
    {
        //@ViewModel
        ViewModel ViewModel => App.ViewModel;
        SelectionViewModel SelectionViewModel => App.SelectionViewModel;


        //@Converter
        private string ImageReStoryboardConverter(ImageRe imageRe)
        {
            if (imageRe == null) return null;

            if (imageRe.IsStoryboardNotify == true)
            {
                this.EaseStoryboard.Begin();//Storyboard
                return null;
            }

            return imageRe.ToString();
        }
        private bool ImageReToIsEnabledConverter(ImageRe imageRe)
        {
            if (imageRe == null) return false;
            if (imageRe.IsStoryboardNotify == true) return false;

            return true;
        }


        //@Construct
        public ImagePage()
        {
            this.InitializeComponent();
            this.ClearButton.Tapped += (s, e) => this.SelectionViewModel.ImageRe = new ImageRe { IsStoryboardNotify = true };//ImageRe
            this.SelectButton.Tapped += async (s, e) =>
            {
                //File
                StorageFile file = await this.ViewModel.PickSingleFileAsync(PickerLocationId.PicturesLibrary);
                if (file == null) return;

                //imageRe
                ImageRe imageRe = await ImageRe.CreateFromStorageFile(this.ViewModel.CanvasDevice, file);
                if (imageRe == null) return;
                
                //Images
                this.ViewModel.DuplicateChecking(imageRe);

                this.SelectionViewModel.ImageRe = imageRe;//ImageRe
            };
            this.ReplaceButton.Tapped += async (s, e) =>
            {
                //File
                StorageFile file = await this.ViewModel.PickSingleFileAsync(PickerLocationId.PicturesLibrary);
                if (file == null) return;

                //imageRe
                ImageRe imageRe = await ImageRe.CreateFromStorageFile(this.ViewModel.CanvasDevice, file);
                if (imageRe == null) return;
                
                //Images
                this.ViewModel.DuplicateChecking(imageRe);

                //Transformer
                Transformer transformerSource = new Transformer(imageRe.Width, imageRe.Height, Vector2.Zero);

                //Selection
                this.SelectionViewModel.SetValue((layer) =>
                {
                    if (layer is ImageLayer imageLayer)
                    {
                        imageLayer.ImageRe = imageRe;
                        imageLayer.Source = transformerSource;
                    }
                }, true);

                this.ViewModel.Invalidate();//Invalidate
            };
            this.StackButton.Tapped +=   (s, e) => 
            {
                this.ListView.ItemsSource = null;
                this.ListView.ItemsSource = this.ViewModel.Images;
                this.Flyout.ShowAt(this.StackButton);//this.StackButton
            };
            this.ListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is ImageRe imageRe)
                {
                    this.SelectionViewModel.ImageRe = imageRe;//ImageRe
                }
            };
        }
    }
}