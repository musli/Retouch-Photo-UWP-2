﻿using Retouch_Photo2.Adjustments;
using Retouch_Photo2.Blends;
using Retouch_Photo2.Brushs;
using Retouch_Photo2.Effects;
using Retouch_Photo2.Layers;
using Retouch_Photo2.Layers.Models;
using System.ComponentModel;
using Windows.UI.Xaml;

namespace Retouch_Photo2.ViewModels
{
    /// <summary> 
    /// Retouch_Photo2's the only <see cref = "SelectionViewModel" />. 
    /// </summary>
    public partial class SelectionViewModel : INotifyPropertyChanged
    {
               
        /// <summary> <see cref = "SelectionViewModel" />'s layer type. </summary>
        public string Type
        {
            get => this.type;
            set
            {
                this.type = value;
                this.OnPropertyChanged(nameof(this.Type));//Notify 
            }
        }
        private string type;


        /// <summary> <see cref = "SelectionViewModel" />'s opacity. </summary>
        public float Opacity { get; set; }
        /// <summary> Sets the <see cref = "SelectionViewModel.Opacity" />. </summary>
        public void SetOpacity(float value)
        {
            if (this.Opacity == value) return;
            this.Opacity = value;
            this.OnPropertyChanged(nameof(this.Opacity));//Notify 
        }


        /// <summary> <see cref = "SelectionViewModel" />'s blend type. </summary>
        public BlendType BlendType
        {
            get => this.blendType;
            set
            {
                this.blendType = value;
                this.OnPropertyChanged(nameof(this.BlendType));//Notify 
            }
        }
        private BlendType blendType;


        /// <summary> <see cref = "SelectionViewModel" />'s visibility. </summary>
        public Visibility Visibility
        {
            get => this.visibility;
            set
            {
                this.visibility = value;
                this.OnPropertyChanged(nameof(this.Visibility));//Notify 
            }
        }
        private Visibility visibility;

        /// <summary> <see cref = "SelectionViewModel" />'s tag type. </summary>
        public TagType TagType
        {
            get => this.tagType;
            set
            {
                if (this.tagType == value) return;
                this.tagType = value;
                this.OnPropertyChanged(nameof(this.TagType));//Notify 
            }
        }
        private TagType tagType;


        //////////////////////////


        /// <summary> <see cref = "SelectionViewModel" />'s IsCrop. </summary>
        public bool IsCrop
        {
            get => this.isCrop;
            set
            {
                this.isCrop = value;
                this.OnPropertyChanged(nameof(this.IsCrop));//Notify 
            }
        }
        private bool isCrop;


        /// <summary> <see cref = "SelectionViewModel" />'s EffectManager. </summary>
        public EffectManager EffectManager
        {
            get => this.effectManager;
            set
            {
                this.effectManager = value;
                this.OnPropertyChanged(nameof(this.EffectManager));//Notify 
            }
        }
        private EffectManager effectManager;


        /// <summary> <see cref = "SelectionViewModel" />'s AdjustmentManager. </summary>
        public AdjustmentManager AdjustmentManager
        {
            get => this.adjustmentManager;
            set
            {
                this.adjustmentManager = value;
                this.OnPropertyChanged(nameof(this.AdjustmentManager));//Notify 
            }
        }
        private AdjustmentManager adjustmentManager;


        //////////////////////////


        /// <summary> GroupLayer's Exist. </summary>     
        public bool IsGroupLayer { get; set; }
        /// <summary> Sets GroupLayer. </summary>     
        private void SetGroupLayer(ILayer layer)
        {
            this.IsGroupLayer = layer is GroupLayer;
            this.OnPropertyChanged(nameof(this.IsGroupLayer));//Notify 
        }


        /// <summary> AcrylicLayer's TintOpacity. </summary>     
        public float AcrylicTintOpacity
        {
            get => this.acrylicTintOpacity;
            set
            {
                if (this.acrylicTintOpacity == value) return;
                this.acrylicTintOpacity = value;
                this.OnPropertyChanged(nameof(this.AcrylicTintOpacity));//Notify 
            }
        }
        private float acrylicTintOpacity = 0.5f;
        /// <summary> AcrylicLayer's BlurAmount. </summary>     
        public float AcrylicBlurAmount
        {
            get => this.acrylicBlurAmount;
            set
            {
                if (this.acrylicBlurAmount == value) return;
                this.acrylicBlurAmount = value;
                this.OnPropertyChanged(nameof(this.AcrylicBlurAmount));//Notify 
            }
        }
        private float acrylicBlurAmount = 12.0f;
        /// <summary> Sets AcrylicLayer. </summary>     
        private void SetAcrylicLayer(ILayer layer)
        {
            if (layer is AcrylicLayer acrylicLayer)
            {
                this.AcrylicTintOpacity = acrylicLayer.TintOpacity;
                this.AcrylicBlurAmount = acrylicLayer.BlurAmount;
            }
        }


        /// <summary> ImageLayer's Exist. </summary>      
        public bool IsImageLayer
        {
            get => this.isImageLayer;
            set
            {
                this.isImageLayer = value;
                this.OnPropertyChanged(nameof(this.IsImageLayer));//Notify 
            }
        }
        private bool isImageLayer;
        /// <summary> <see cref = "SelectionViewModel" />'s ImageRe. </summary>
        public ImageRe ImageRe
        {
            get => this.imageRe;
            set
            {
                this.imageRe = value;
                this.OnPropertyChanged(nameof(this.ImageRe));//Notify 
            }
        }
        private ImageRe imageRe;
        /// <summary> Sets ImageLayer. </summary>     
        private void SetImageLayer(ILayer layer)
        {
            if (layer is ImageLayer imageLayer)
            {
                this.IsImageLayer = true;
                this.ImageRe = imageLayer.ImageRe;
            }
            else
            {
                this.IsImageLayer = false;
            }
        }
        

        /// <summary> Sets PenTool nodes mode. </summary>     
        public bool IsPenToolNodesMode
        {
            get => this.isPenToolNodesMode;
            set
            {
                this.isPenToolNodesMode = value;
                this.OnPropertyChanged(nameof(this.IsPenToolNodesMode));//Notify 
            }
        }
        private bool isPenToolNodesMode;
        /// <summary> <see cref = "SelectionViewModel" />'s CurveLayer. </summary>
        public GeometryCurveLayer CurveLayer { get; set; }
        /// <summary> Sets CurveLayer. </summary>     
        private void SetCurveLayer(ILayer layer)
        {
            if (layer is GeometryCurveLayer curveLayer)
            {
                this.IsPenToolNodesMode = (curveLayer.NodeCollection.Count != 2);
                this.CurveLayer = curveLayer;
            }
            else
            {
                this.IsPenToolNodesMode = false;
                this.CurveLayer = null;
            }
        }



    }
}