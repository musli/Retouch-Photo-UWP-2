﻿using Retouch_Photo2.Adjustments.Icons;
using Retouch_Photo2.Adjustments.Models;
using Retouch_Photo2.Historys;
using Retouch_Photo2.Layers;
using Retouch_Photo2.ViewModels;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;

namespace Retouch_Photo2.Adjustments.Pages
{
    /// <summary>
    /// Page of <see cref = "BrightnessAdjustment"/>.
    /// </summary>
    public sealed partial class BrightnessPage : IAdjustmentPage
    {

        //@ViewModel
        ViewModel ViewModel => App.ViewModel;
        ViewModel SelectionViewModel => App.SelectionViewModel;
        ViewModel MethodViewModel => App.MethodViewModel;


        //@Content
        private float WhiteLight
        {
            set
            {
                this.WhiteLightPicker.Value = (int)(value * 100.0f);
                this.WhiteLightSlider.Value = value;
            }
        }
        private float WhiteDark
        {
            set
            {
                this.WhiteDarkPicker.Value = (int)(value * 100.0f);
                this.WhiteDarkSlider.Value = value;
            }
        }
        private float BlackLight
        {
            set
            {
                this.BlackLightPicker.Value = (int)(value * 100.0f);
                this.BlackLightSlider.Value = value;
            }
        }
        private float BlackDark
        {
            set
            {
                this.BlackDarkPicker.Value = (int)(value * 100.0f);
                this.BlackDarkSlider.Value = value;
            }
        }


        //@Construct
        /// <summary>
        /// Initializes a BrightnessPage. 
        /// </summary>
        public BrightnessPage()
        {
            this.InitializeComponent();
            this.ConstructStrings();
            BrightnessAdjustment.GenericText = this.Text;
            BrightnessAdjustment.GenericPage = this;

            this.ConstructWhiteLight1();
            this.ConstructWhiteLight2();

            this.ConstructWhiteDark1();
            this.ConstructWhiteDark2();

            this.ConstructBlackLight1();
            this.ConstructBlackLight2();

            this.ConstructBlackDark1();
            this.ConstructBlackDark2();
        }
    }

    /// <summary>
    /// Page of <see cref = "BrightnessAdjustment"/>.
    /// </summary>
    public sealed partial class BrightnessPage : IAdjustmentPage
    {

        //Strings
        private void ConstructStrings()
        {
            ResourceLoader resource = ResourceLoader.GetForCurrentView();

            this.Text = resource.GetString("/Adjustments/Brightness");

            this.WhiteLightTextBlock.Text = resource.GetString("/Adjustments/Brightness_WhiteToLight");
            this.WhiteDarkTextBlock.Text = resource.GetString("/Adjustments/Brightness_WhiteToDark");

            this.BlackLightTextBlock.Text = resource.GetString("/Adjustments/Brightness_BlackToLight");
            this.BlackDarkTextBlock.Text = resource.GetString("/Adjustments/Brightness_BlackToDark");
        }


        //@Content
        /// <summary> Gets the type. </summary>
        public AdjustmentType Type => AdjustmentType.Brightness;
        /// <summary> Gets the icon. </summary>
        public FrameworkElement Icon { get; } = new BrightnessIcon();
        /// <summary> Gets the self. </summary>
        public FrameworkElement Self => this;
        /// <summary> Gets the text. </summary>
        public string Text { get; private set; }

        /// <summary> Return a new <see cref = "IAdjustment"/>. </summary>
        public IAdjustment GetNewAdjustment() => new BrightnessAdjustment();


        /// <summary> Gets the adjustment index. </summary>
        public int Index { get; set; }

        /// <summary>
        /// Reset the <see cref="IAdjustmentPage"/>'s data.
        /// </summary>
        public void Reset()
        {
            this.WhiteLight = 1.0f;
            this.WhiteDark = 1.0f;
            this.BlackLight = 0.0f;
            this.BlackDark = 0.0f;

            if (this.SelectionViewModel.SelectionLayerage is Layerage layerage)
            {
                ILayer layer = layerage.Self;

                if (layer.Filter.Adjustments[this.Index] is BrightnessAdjustment adjustment)
                {
                    //History
                    LayersPropertyHistory history = new LayersPropertyHistory("Set brightness adjustment");

                    var previous = layer.Filter.Adjustments.IndexOf(adjustment); 
                    var previous1 = adjustment.WhiteLight;
                    var previous2 = adjustment.WhiteDark;
                    var previous3 = adjustment.BlackLight;
                    var previous4 = adjustment.BlackDark;
                    history.UndoAction += () =>
                    {
                        if (previous < 0) return;
                        if (previous > layer.Filter.Adjustments.Count - 1) return;
                        if (layer.Filter.Adjustments[previous] is BrightnessAdjustment adjustment2)
                        {
                            //Refactoring
                            layer.IsRefactoringRender = true;
                            layer.IsRefactoringIconRender = true;
                            adjustment2.WhiteLight = previous1;
                            adjustment2.WhiteDark = previous2;
                            adjustment2.BlackLight = previous3;
                            adjustment2.BlackDark = previous4;
                        }
                    };

                    //Refactoring
                    layer.IsRefactoringRender = true;
                    layer.IsRefactoringIconRender = true;
                    layerage.RefactoringParentsRender();
                    layerage.RefactoringParentsIconRender();
                    adjustment.WhiteLight = 1.0f;
                    adjustment.WhiteDark = 1.0f;
                    adjustment.BlackLight = 0.0f;
                    adjustment.BlackDark = 0.0f;

                    //History
                    this.ViewModel.HistoryPush(history);

                    this.ViewModel.Invalidate();//Invalidate
                }
            }
        }
        /// <summary>
        /// <see cref="IAdjustmentPage"/>'s value follows the <see cref="IAdjustment"/>.
        /// </summary>
        public void Follow()
        {
            if (this.SelectionViewModel.SelectionLayerage is Layerage layerage)
            {
                ILayer layer = layerage.Self;

                if (layer.Filter.Adjustments[this.Index] is BrightnessAdjustment adjustment)
                {
                    this.WhiteLight = adjustment.WhiteLight;
                    this.WhiteDark = adjustment.WhiteDark;

                    this.BlackLight = adjustment.BlackLight;
                    this.BlackDark = adjustment.BlackDark;
                }
            }
        }

    }

    /// <summary>
    /// Page of <see cref = "BrightnessAdjustment"/>.
    /// </summary>
    public sealed partial class BrightnessPage : IAdjustmentPage
    {

        //WhiteLight
        private void ConstructWhiteLight1()
        {
            this.WhiteLightPicker.Unit = "%";
            this.WhiteLightPicker.Minimum = 50;
            this.WhiteLightPicker.Maximum = 100;
            this.WhiteLightPicker.ValueChanged += (s, value) =>
            {
                float whiteLight = (float)value / 100.0f;
                this.WhiteLight = whiteLight;
                
                if (whiteLight < 0.0f) whiteLight = 0.0f;
                if (whiteLight > 1.0f) whiteLight = 1.0f;

                this.MethodViewModel.TAdjustmentChanged<float, BrightnessAdjustment>
                (
                    index: this.Index,
                    set: (tAdjustment) => tAdjustment.WhiteLight = whiteLight,

                    historyTitle: "Set brightness adjustment white light",
                    getHistory: (tAdjustment) => tAdjustment.WhiteLight,
                    setHistory: (tAdjustment, previous) => tAdjustment.WhiteLight = previous
                );
            };
        }

        private void ConstructWhiteLight2()
        {
            this.WhiteLightSlider.Minimum = 0.5d;
            this.WhiteLightSlider.Maximum = 1.0d;
            this.WhiteLightSlider.ValueChangeStarted += (s, value) => this.MethodViewModel.TAdjustmentChangeStarted<BrightnessAdjustment>(index: this.Index, cache: (tAdjustment) => tAdjustment.CacheWhiteLight());
            this.WhiteLightSlider.ValueChangeDelta += (s, value) =>
            {
                float whiteLight = (float)value;
                this.WhiteLight = whiteLight;

                if (whiteLight < 0.0f) whiteLight = 0.0f;
                if (whiteLight > 1.0f) whiteLight = 1.0f;

                this.MethodViewModel.TAdjustmentChangeDelta<BrightnessAdjustment>(index: this.Index, set: (tAdjustment) => tAdjustment.WhiteLight = whiteLight);
            };
            this.WhiteLightSlider.ValueChangeCompleted += (s, value) =>
            {
                float whiteLight = (float)value;
                this.WhiteLight = whiteLight;

                if (whiteLight < 0.0f) whiteLight = 0.0f;
                if (whiteLight > 1.0f) whiteLight = 1.0f;

                this.MethodViewModel.TAdjustmentChangeCompleted<float, BrightnessAdjustment>
                (
                    index: this.Index,
                    set: (tAdjustment) => tAdjustment.WhiteLight = whiteLight,

                    historyTitle: "Set brightness adjustment white light",
                    getHistory: (tAdjustment) => tAdjustment.StartingWhiteLight,
                    setHistory: (tAdjustment, previous) => tAdjustment.WhiteLight = previous
                );
            };
        }


        //WhiteDark
        private void ConstructWhiteDark1()
        {
            this.WhiteDarkPicker.Unit = "%";
            this.WhiteDarkPicker.Minimum = 50;
            this.WhiteDarkPicker.Maximum = 100;
            this.WhiteDarkPicker.ValueChanged += (s, value) =>
            {
                float whiteDark = (float)value / 100.0f;
                this.WhiteDark = whiteDark;

                if (whiteDark < 0.0f) whiteDark = 0.0f;
                if (whiteDark > 1.0f) whiteDark = 1.0f;

                this.MethodViewModel.TAdjustmentChanged<float, BrightnessAdjustment>
                (
                    index: this.Index,
                    set: (tAdjustment) => tAdjustment.WhiteDark = whiteDark,

                    historyTitle: "Set brightness adjustment white dark",
                    getHistory: (tAdjustment) => tAdjustment.WhiteDark,
                    setHistory: (tAdjustment, previous) => tAdjustment.WhiteDark = previous
                );
            };
        }

        private void ConstructWhiteDark2()
        {
            this.WhiteDarkSlider.Minimum = 0.5d;
            this.WhiteDarkSlider.Maximum = 1.0d;
            this.WhiteDarkSlider.ValueChangeStarted += (s, value) => this.MethodViewModel.TAdjustmentChangeStarted<BrightnessAdjustment>(index: this.Index, cache: (tAdjustment) => tAdjustment.CacheWhiteDark());
            this.WhiteDarkSlider.ValueChangeDelta += (s, value) =>
            {
                float whiteDark = (float)value;
                this.WhiteDark = whiteDark;

                if (whiteDark < 0.0f) whiteDark = 0.0f;
                if (whiteDark > 1.0f) whiteDark = 1.0f;

                this.MethodViewModel.TAdjustmentChangeDelta<BrightnessAdjustment>(index: this.Index, set: (tAdjustment) => tAdjustment.WhiteDark = whiteDark);
            };
            this.WhiteDarkSlider.ValueChangeCompleted += (s, value) =>
            {
                float whiteDark = (float)value;
                this.WhiteDark = whiteDark;

                if (whiteDark < 0.0f) whiteDark = 0.0f;
                if (whiteDark > 1.0f) whiteDark = 1.0f;

                this.MethodViewModel.TAdjustmentChangeCompleted<float, BrightnessAdjustment>
                (
                    index: this.Index,
                    set: (tAdjustment) => tAdjustment.WhiteDark = whiteDark,

                    historyTitle: "Set brightness adjustment white dark",
                    getHistory: (tAdjustment) => tAdjustment.StartingWhiteDark,
                    setHistory: (tAdjustment, previous) => tAdjustment.WhiteDark = previous
                );
            };
        }


        //BlackLight
        private void ConstructBlackLight1()
        {
            this.BlackLightPicker.Unit = "%";
            this.BlackLightPicker.Minimum = 0;
            this.BlackLightPicker.Maximum = 50;
            this.BlackLightPicker.ValueChanged += (s, value) =>
            {
                float blackLight = (float)value / 100.0f;
                this.BlackLight = blackLight;

                if (blackLight < 0.0f) blackLight = 0.0f;
                if (blackLight > 1.0f) blackLight = 1.0f;

                this.MethodViewModel.TAdjustmentChanged<float, BrightnessAdjustment>
                (
                    index: this.Index,
                    set: (tAdjustment) => tAdjustment.BlackLight = blackLight,

                    historyTitle: "Set brightness adjustment black light",
                    getHistory: (tAdjustment) => tAdjustment.BlackLight,
                    setHistory: (tAdjustment, previous) => tAdjustment.BlackLight = previous
                );
            };
        }

        private void ConstructBlackLight2()
        {
            this.BlackLightSlider.Minimum = 0.0d;
            this.BlackLightSlider.Maximum = 0.5d;
            this.BlackLightSlider.ValueChangeStarted += (s, value) => this.MethodViewModel.TAdjustmentChangeStarted<BrightnessAdjustment>(index: this.Index, cache: (tAdjustment) => tAdjustment.CacheBlackLight());
            this.BlackLightSlider.ValueChangeDelta += (s, value) =>
            {
                float blackLight = (float)value;
                this.BlackLight = blackLight;

                if (blackLight < 0.0f) blackLight = 0.0f;
                if (blackLight > 1.0f) blackLight = 1.0f;

                this.MethodViewModel.TAdjustmentChangeDelta<BrightnessAdjustment>(index: this.Index, set: (tAdjustment) => tAdjustment.BlackLight = blackLight);
            };
            this.BlackLightSlider.ValueChangeCompleted += (s, value) =>
            {
                float blackLight = (float)value;
                this.BlackLight = blackLight;

                if (blackLight < 0.0f) blackLight = 0.0f;
                if (blackLight > 1.0f) blackLight = 1.0f;

                this.MethodViewModel.TAdjustmentChangeCompleted<float, BrightnessAdjustment>
                (
                    index: this.Index,
                    set: (tAdjustment) => tAdjustment.BlackLight = blackLight,

                    historyTitle: "Set brightness adjustment black light",
                    getHistory: (tAdjustment) => tAdjustment.StartingBlackLight,
                    setHistory: (tAdjustment, previous) => tAdjustment.BlackLight = previous
                );
            };
        }


        //BlackDark
        private void ConstructBlackDark1()
        {
            this.BlackDarkPicker.Unit = "%";
            this.BlackDarkPicker.Minimum = 0;
            this.BlackDarkPicker.Maximum = 50;
            this.BlackDarkPicker.ValueChanged += (s, value) =>
            {
                float blackDark = (float)value / 100.0f;
                this.BlackDark = blackDark;

                if (blackDark < 0.0f) blackDark = 0.0f;
                if (blackDark > 1.0f) blackDark = 1.0f;

                this.MethodViewModel.TAdjustmentChanged<float, BrightnessAdjustment>
                (
                    index: this.Index,
                    set: (tAdjustment) => tAdjustment.BlackDark = blackDark,

                    historyTitle: "Set brightness adjustment black dark",
                    getHistory: (tAdjustment) => tAdjustment.BlackDark,
                    setHistory: (tAdjustment, previous) => tAdjustment.BlackDark = previous
                );
            };
        }

        private void ConstructBlackDark2()
        {
            this.BlackDarkSlider.Minimum = 0.0d;
            this.BlackDarkSlider.Maximum = 0.5d;
            this.BlackDarkSlider.ValueChangeStarted += (s, value) => this.MethodViewModel.TAdjustmentChangeStarted<BrightnessAdjustment>(index: this.Index, cache: (tAdjustment) => tAdjustment.CacheBlackDark());
            this.BlackDarkSlider.ValueChangeDelta += (s, value) =>
            {
                float blackDark = (float)value;
                this.BlackDark = blackDark;

                if (blackDark < 0.0f) blackDark = 0.0f;
                if (blackDark > 1.0f) blackDark = 1.0f;

                this.MethodViewModel.TAdjustmentChangeDelta<BrightnessAdjustment>(index: this.Index, set: (tAdjustment) => tAdjustment.BlackDark = blackDark);
            };
            this.BlackDarkSlider.ValueChangeCompleted += (s, value) =>
            {
                float blackDark = (float)value;
                this.BlackDark = blackDark;

                if (blackDark < 0.0f) blackDark = 0.0f;
                if (blackDark > 1.0f) blackDark = 1.0f;

                this.MethodViewModel.TAdjustmentChangeCompleted<float, BrightnessAdjustment>
                (
                    index: this.Index,
                    set: (tAdjustment) => tAdjustment.BlackDark = blackDark,

                    historyTitle: "Set brightness adjustment black dark",
                    getHistory: (tAdjustment) => tAdjustment.StartingBlackDark,
                    setHistory: (tAdjustment, previous) => tAdjustment.BlackDark = previous
                );
            };
        }

    }
}