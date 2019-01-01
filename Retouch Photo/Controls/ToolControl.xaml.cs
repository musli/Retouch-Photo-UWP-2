﻿using Retouch_Photo.Models;
using Retouch_Photo.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Retouch_Photo.Controls
{
    public sealed partial class ToolControl : UserControl
    {
        //ViewModel
        DrawViewModel ViewModel => App.ViewModel;


        //delegate
        public delegate void IndexChangedHandler(int index);
        public static event IndexChangedHandler IndexChanged = null;
        public static void SetIndex(int index) => ToolControl.IndexChanged?.Invoke(index);
        
        public ToolControl()
        {
            this.InitializeComponent();
            ToolControl.IndexChanged += (int index) => this.ListBox.SelectedIndex = index;
        }

        private void ListBox_Loaded(object sender, RoutedEventArgs e) => this.ListBox.ItemsSource = Tool.ToolList;

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = this.ListBox.SelectedIndex;

            App.ViewModel.Text = index.ToString();
            if (index < 0) return;
            if (index >= Tool.ToolList.Count) return;

            this.ViewModel.Tool = Tool.ToolList[index];
            this.ViewModel.Invalidate();
        }
    }
}
