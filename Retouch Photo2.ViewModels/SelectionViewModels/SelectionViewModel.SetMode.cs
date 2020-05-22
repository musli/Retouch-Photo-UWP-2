﻿using FanKit.Transformers;
using Retouch_Photo2.Blends;
using Retouch_Photo2.Layers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Retouch_Photo2.ViewModels
{
    /// <summary> 
    /// Retouch_Photo2's the only <see cref = "ViewModel" />. 
    /// </summary>
    public partial class ViewModel : INotifyPropertyChanged
    {

        /// <summary>
        ///  Sets the mode and notify all properties.
        /// </summary>
        /// <param name="layerCollection"> The layer-collection. </param>
        public void SetMode(LayerCollection layerCollection)
        {
            IEnumerable<Layerage> checkedLayers = LayerCollection.GetAllSelectedLayers(layerCollection);
            int count = checkedLayers.Count();

            if (count == 0)
                this._setModeNone();//None
            else if (count == 1)
                this._setModeSingle(checkedLayers.Single());//Single
            else if (count >= 2)
                this._setModeMultiple(checkedLayers);//Multiple
        }
        

        //////////////////////////


        /// <summary>
        ///  Sets the mode to None.
        /// </summary>
        public void SetModeNone()
        {
            if (this.Layerage != null)
            {
                if (this.Layerage.Self is ILayer layer)
                {
                    layer.IsSelected = false;
                }
            }

            if (this.Layerages != null)
            {
                foreach (Layerage child in this.Layerages)
                {
                    child.Self.IsSelected = false;
                }
            }
            this._setModeNone();//None
        }
        private void _setModeNone()
        {
            this.SelectionMode = ListViewSelectionMode.None;
            this.SelectionUnNone = false;
            this.SelectionSingle = false;
            
            //this.Transformer = new Transformer();
            this.DisabledRadian = false;

            this.Layerage = new Layerage();
            this.Layerages = null;

            //////////////////////////

            this.LayerType = LayerType.None;
            this.LayerName = string.Empty;
            this.SetOpacity(1.0f);
            this.BlendMode = null;
            this.SetVisibility(Visibility.Collapsed);
            this.SetTagType(TagType.None);

            //////////////////////////

            this.IsCrop = false;
            this.Effect = null;
            this.Filter = null;
            this.SetStyle(null);

            //////////////////////////

            this.SetGroupLayer();
            this.SetImageLayer(null);
            this.SetCurveLayer(null);
            this.SetFontLayer(null);
            
            //////////////////////////

            this.SetIGeometryLayer(null);            
        }


        //////////////////////////


        /// <summary>
        ///  Sets the mode to Single.
        /// </summary>
        /// <param name="layerage"> The single layer. </param>
        public void SetModeSingle(Layerage layerage)
        {
            if (this.Layerages != null)
            {
                foreach (Layerage child in this.Layerages)
                {
                    child.Self.IsSelected = false;
                }
            }
            this._setModeSingle(layerage);//Single
        }
        private void _setModeSingle(Layerage layerage)
        {
            ILayer layer = layerage.Self;

            this.SelectionMode = ListViewSelectionMode.Single;
            this.SelectionUnNone = true;
            this.SelectionSingle = true;

            this.Transformer = layer.GetActualDestinationWithRefactoringTransformer;
            this.DisabledRadian = false;

            this.Layerage = layerage;
            this.Layerages = null;

            //////////////////////////

            this.LayerType = layer.Type;
            this.LayerName = layer.Name;
            this.SetOpacity(layer.Opacity);
            this.BlendMode = layer.BlendMode;
            this.SetVisibility(layer.Visibility);
            this.SetTagType(layer.TagType);

            //////////////////////////

            this.IsCrop = layer.Transform.IsCrop;
            this.Effect = layer.Effect;
            this.Filter = layer.Filter;
            this.SetStyle(layer.Style);

            //////////////////////////

            this.SetGroupLayer(layer);
            this.SetImageLayer(layer);
            this.SetCurveLayer(layer);
            this.SetFontLayer(layer);

            //////////////////////////

            this.SetIGeometryLayer(layer);
        }


        //////////////////////////


        /// <summary>
        ///  Sets the mode to Multiple.
        /// </summary>
        /// <param name="layer"> The multiple layer. </param>
        /// <param name="outermost"> The outermost layer. </param>
        public void SetModeMultiple(IList<Layerage> layerages)
        {
            if (this.Layerage != null)
            {
                ILayer layer = this.Layerage.Self;
                layer.IsSelected = false;
            }
            this._setModeMultiple(layerages);//Multiple
        }
        private void _setModeMultiple(IEnumerable<Layerage> layerages)
        {
            Layerage outermost = LayerCollection.FindOutermost_SelectedLayer(layerages);
            ILayer outermostLayer = outermost.Self;
            this.SelectionMode = ListViewSelectionMode.Multiple;//Transformer     
            this.SelectionUnNone = true;
            this.SelectionSingle = false;

            this.Layerage = new Layerage();
            this.Layerages = layerages;

            //TransformerBorder
            IEnumerable<Transformer> transformers = from l in layerages select l.Self.GetActualDestinationWithRefactoringTransformer;
            TransformerBorder border = new TransformerBorder(transformers);
            this.Transformer = border.ToTransformer();
            this.DisabledRadian = false;

            //////////////////////////

            this.LayerType = outermostLayer == null ? LayerType.None : outermostLayer.Type;
            this.SetOpacity(outermostLayer == null ? 1.0f : outermostLayer.Opacity);
            this.BlendMode = outermostLayer?.BlendMode;
            this.SetVisibility(outermostLayer == null ? Visibility.Visible : outermostLayer.Visibility);
            this.SetTagType(outermostLayer == null ? TagType.None : outermostLayer.TagType);

            //////////////////////////

            this.IsCrop = layerages.Any(layer => outermostLayer.Transform.IsCrop);
            this.Effect = outermostLayer?.Effect;
            this.Filter = null;
            this.SetStyle(outermostLayer?.Style);

            //////////////////////////

            this.SetGroupLayer(layerages);
            this.SetImageLayer(outermostLayer);
            this.SetCurveLayer(null);
            this.SetFontLayer(null);

            //////////////////////////

            this.SetIGeometryLayer(outermostLayer);
        }


        //////////////////////////


        /// <summary>
        ///  Sets the mode to Extended.
        /// </summary>
        public void SetModeExtended()
        {
            this.SetModeNone();
            this.SelectionMode = ListViewSelectionMode.Extended;
        }

    }
}