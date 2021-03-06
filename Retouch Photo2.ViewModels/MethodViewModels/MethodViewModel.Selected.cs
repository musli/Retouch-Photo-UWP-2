﻿using Retouch_Photo2.Historys;
using Retouch_Photo2.Layers;
using System.ComponentModel;

namespace Retouch_Photo2.ViewModels
{
    /// <summary> 
    /// Represents a ViewModel that contains some methods of the application
    /// </summary>
    public partial class ViewModel : INotifyPropertyChanged
    {

        public void MethodSelectedNone()
        {
            //History
            LayersPropertyHistory history = new LayersPropertyHistory("Set select mode");

            //Selection
            this.SetValue((layerage) =>
            {
                ILayer layer = layerage.Self;

                if (layer.IsSelected == true)
                {
                    //History
                    var previous = layer.IsSelected;
                    history.UndoAction += () =>
                    {
                        layer.IsSelected = previous;
                    };

                    layer.IsSelected = false;
                }
            });

            //History
            this.HistoryPush(history);

            this.SetModeNone();//Selection
            LayerageCollection.ArrangeLayersBackground(this.LayerageCollection);
            this.Invalidate();//Invalidate     
        }

        public void MethodSelectedNot(Layerage selectedLayerage)
        {
            ILayer selectedLayer = selectedLayerage.Self;

            //History 
            LayersPropertyHistory history = new LayersPropertyHistory("Set is selected");

            var previous = selectedLayer.IsSelected;
            history.UndoAction += () =>
            {
                selectedLayer.IsSelected = previous;
            };

            selectedLayer.IsSelected = !selectedLayer.IsSelected;

            //History 
            this.HistoryPush(history);

            this.SetMode(this.LayerageCollection);//Selection
            //LayerageCollection.ArrangeLayersBackgroundItemClick(selectedLayerage);
            LayerageCollection.ArrangeLayersBackground(this.LayerageCollection);
            this.Invalidate();//Invalidate
        }



        public void MethodSelectedNew(Layerage selectedLayerage)
        {
            ILayer selectedLayer = selectedLayerage.Self;

            //History
            LayersPropertyHistory history = new LayersPropertyHistory("Set select mode");

            if (selectedLayer.IsSelected == false)
            {
                var previous = selectedLayer.IsSelected;
                history.UndoAction += () =>
                {
                    selectedLayer.IsSelected = previous;
                };

                selectedLayer.IsSelected = true;
            }

            //Selection
            this.SetValue((layerage) =>
            {
                ILayer layer = layerage.Self;

                if (layer != selectedLayer)
                {
                    if (layer.IsSelected == true)
                    {
                        //History
                        var previous = layer.IsSelected;
                        history.UndoAction += () =>
                        {
                            layer.IsSelected = previous;
                        };

                        layer.IsSelected = false;
                    }
                }
            });

            //History
            this.HistoryPush(history);

            this.SetModeSingle(selectedLayerage);//Selection
            LayerageCollection.ArrangeLayersBackground(this.LayerageCollection);
            this.Invalidate();//Invalidate     
        }

        public void MethodSelectedAdd(Layerage selectedLayerage)
        {
            ILayer selectedLayer = selectedLayerage.Self;

            //History
            LayersPropertyHistory history = new LayersPropertyHistory("Set select mode");

            if (selectedLayer.IsSelected == false)
            {
                //History
                var previous = selectedLayer.IsSelected;
                history.UndoAction += () =>
                {
                    selectedLayer.IsSelected = previous;
                };

                selectedLayer.IsSelected = true;
            }

            //History
            this.HistoryPush(history);

            this.SetMode(this.LayerageCollection);//Selection
            LayerageCollection.ArrangeLayersBackground(this.LayerageCollection);
            this.Invalidate();//Invalidate
        }

        public void MethodSelectedSubtract(Layerage selectedLayerage)
        {
            ILayer selectedLayer = selectedLayerage.Self;

            //History
            LayersPropertyHistory history = new LayersPropertyHistory("Set select mode");

            if (selectedLayerage.Self.IsSelected == true)
            {
                //History
                var previous = selectedLayer.IsSelected;
                history.UndoAction += () =>
                {
                    selectedLayer.IsSelected = previous;
                };

                selectedLayer.IsSelected = false;
            }

            //History
            this.HistoryPush(history);

            this.SetMode(this.LayerageCollection);//Selection
            LayerageCollection.ArrangeLayersBackground(this.LayerageCollection);
            this.Invalidate();//Invalidate
        }

        /*
    public void MethodSelectedIntersect(Layerage selectedLayerage)
    {
        ILayer selectedLayer = selectedLayerage.Self;

        //History
        LayersPropertyHistory history = new LayersPropertyHistory("Set select mode");

        //Selection
        this.SetValue((layerage) =>
        {
            if (layerage != selectedLayerage)
            {
                ILayer layer = layerage.Self;

                if (layer.IsSelected == true)
                {
                    //History
                    var previous = selectedLayer.IsSelected;
                    history.UndoAction += () =>
                    {
                        selectedLayer.IsSelected = previous;
                    };

                    layer.IsSelected = false;
                }
            }
        });

        //History
        this.HistoryPush(history);

        this.SetModeSingle(selectedLayerage);//Selection
        LayerageCollection.ArrangeLayersBackground(this.LayerageCollection);
        this.Invalidate();//Invalidate
    }
        */


    }
}