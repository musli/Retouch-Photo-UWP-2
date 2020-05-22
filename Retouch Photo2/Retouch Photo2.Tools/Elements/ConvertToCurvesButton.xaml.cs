﻿using FanKit.Transformers;
using Retouch_Photo2.Brushs;
using Retouch_Photo2.Elements;
using Retouch_Photo2.Layers;
using Retouch_Photo2.Layers.Models;
using Retouch_Photo2.ViewModels;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace Retouch_Photo2.Tools.Elements
{
    public sealed partial class ConvertToCurvesButton : UserControl
    {
        //@ViewModel
        ViewModel ViewModel => App.ViewModel;
        TipViewModel TipViewModel => App.TipViewModel;
               
        //@Construct
        public ConvertToCurvesButton()
        {
            this.InitializeComponent();
            this.Button.Click += (s, e) =>
            {
                if (this.ViewModel.SelectionMode == ListViewSelectionMode.None) return;

                //History
                this.ViewModel.HistoryPushLayeragesHistory("Convert to curves");

                this.ViewModel.SetValue((layerage)=>
                {
                    ILayer layer2 = layerage.Self;

                    //Turn to curve layer
                    Layer curveLayer = this.GetCurveLayer(layerage);
                    if (curveLayer == null) return;
                    Layerage curveLayerage = curveLayer.ToLayerage();

                    //set image brush
                    if (layer2.Type == LayerType.Image)
                    {
                        ImageLayer imageLayer = (ImageLayer)layer2;
                        curveLayer.Style.Fill = imageLayer.ToBrush();
                    }

                    this.Replace(curveLayerage, layerage);
                    curveLayer.IsSelected = true;
                });


                //Change tools group value.
                this.TipViewModel.Tool = this.TipViewModel.Tools.First(t => t != null && t.Type == ToolType.Node);
                this.TipViewModel.ToolGroupType(ToolType.Node);

                LayerCollection.ArrangeLayersControls(this.ViewModel.LayerCollection);
                this.ViewModel.SetMode(this.ViewModel.LayerCollection);//Selection
                this.ViewModel.Invalidate();//Invalidate
            };
        }


        //Get curve layer
        private Layer GetCurveLayer(Layerage layerage)
        {
            ILayer layer = layerage.Self;
            
            IEnumerable<IEnumerable<Node>> nodess = layer.ConvertToCurves();
            if (nodess == null) return null;
            
            if (nodess.Count() == 1)
            {
                CurveLayer curveLayer = new CurveLayer(nodess.Single());
                Layer.CopyWith(this.ViewModel.CanvasDevice, curveLayer, layer);
                return curveLayer;
            }

            CurveMultiLayer curveMultiLayer = new CurveMultiLayer(nodess);
            Layer.CopyWith(this.ViewModel.CanvasDevice, curveMultiLayer, layer);
            Layer.Instances.Add(curveMultiLayer);
            return curveMultiLayer;
        }
         
        
        //Replace curveLayer to layer
        private void Replace(Layerage curveLayer, Layerage layer)
        {
            IList<Layerage> parentsChildren = this.ViewModel.LayerCollection.GetParentsChildren(layer);
            int index = parentsChildren.IndexOf(layer);
            parentsChildren[index] = curveLayer;
        }


    }
}