﻿using Retouch_Photo2.Blends;
using Retouch_Photo2.Layers;
using Retouch_Photo2.Transformers;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Retouch_Photo2.TestApp.ViewModels
{

    /// <summary> 
    /// Retouch_Photo2's the only <see cref = "ViewModel" />. 
    /// </summary>
    public partial class ViewModel
    {

        /// <summary>
        /// Sets <see cref = "ViewModel.SelectionMode" /> to None.
        /// </summary>
        public void SetSelectionModeNone()
        {
            this.SelectionTransformer = new Transformer();
            this.SelectionLayer = null;
            this.SelectionLayers = null;

            this.SelectionMode = ListViewSelectionMode.None;

            //////////////////////////

            this.SetSelectionOpacity(1.0f);
            this.SetSelectionBlendType(BlendType.Normal);
            this.SelectionIsVisual = false;

            this.SelectionIsEffectManager =null;
        }

        /// <summary>
        /// Sets <see cref = "ViewModel.SelectionMode" /> to Single.
        /// </summary>
        /// <param name="layer"> The selection layer. </param>
        public void SetSelectionModeSingle(Layer layer)
        {
            this.SelectionTransformer = layer.TransformerMatrix.Destination;
            this.SelectionLayer = layer;
            this.SelectionLayers = null;

            this.SelectionMode = ListViewSelectionMode.Single;

            //////////////////////////

            this.SetSelectionOpacity(layer.Opacity);
            this.SetSelectionBlendType(layer.BlendType);
            this.SelectionIsVisual = layer.IsVisual;

            this.SelectionIsEffectManager = layer.EffectManager;

            if (layer.GetFillColor() is Color color)
            {
                this.FillColor = color;
            }
        }

        /// <summary>
        /// Sets <see cref = "ViewModel.SelectionMode" /> to Multiple.
        /// </summary>
        /// <param name="layers"> All selection layers. </param>
        public void SetSelectionModeMultiple(IEnumerable<Layer> layers)
        {
            float left = float.MaxValue;
            float top = float.MaxValue;
            float right = float.MinValue;
            float bottom = float.MinValue;

            {
                void aaa(Vector2 vector)
                {
                    if (left > vector.X) left = vector.X;
                    if (top > vector.Y) top = vector.Y;
                    if (right < vector.X) right = vector.X;
                    if (bottom < vector.Y) bottom = vector.Y;
                }

                //Foreach
                foreach (Layer item in layers)
                {
                    aaa(item.TransformerMatrix.Destination.LeftTop);
                    aaa(item.TransformerMatrix.Destination.RightTop);
                    aaa(item.TransformerMatrix.Destination.RightTop);
                    aaa(item.TransformerMatrix.Destination.LeftBottom);
                }
            }

            this.SelectionTransformer = new Transformer(left, top, right, bottom);
            this.SelectionLayer = null;
            this.SelectionLayers = layers;

            this.SelectionMode = ListViewSelectionMode.Multiple;//Transformer           
        }


        /// <summary> Sets <see cref = "ViewModel.SelectionMode" />. </summary>
        public void SetSelectionMode()
        {
            IEnumerable<Layer> checkedLayers = from item in this.Layers where item.IsChecked select item;
            int count = checkedLayers.Count();

            if (count == 0)
                this.SetSelectionModeNone();//None

            else if (count == 1)
                this.SetSelectionModeSingle(checkedLayers.Single());//Single

            else if (count >= 2)
                this.SetSelectionModeMultiple(checkedLayers);//Multiple
        }
        

    }
}