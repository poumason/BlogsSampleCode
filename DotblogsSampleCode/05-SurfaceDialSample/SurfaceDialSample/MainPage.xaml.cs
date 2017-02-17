using Lumia.Imaging.Adjustments;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Storage.Streams;
using Windows.UI.Composition;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SurfaceDialSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        RadialController dialController;
        string currentDialItem { get; set; }

        ScaleEffect scaleEffect = new ScaleEffect();
        GaussianBlurEffect blurEffect = new GaussianBlurEffect();

        public MainPage()
        {
            this.InitializeComponent();
            CreateDialControllerMenu();
        }

        private void CreateDialControllerMenu()
        {
            dialController = RadialController.CreateForCurrentView();

            // Create an icon for the custom tool.
            RandomAccessStreamReference icon =
                RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/StoreLogo.png"));

            // Create opacity and blur menu items for the custom tool.
            RadialControllerMenuItem opacityItem = RadialControllerMenuItem.CreateFromIcon("Opacity", icon);
            opacityItem.Invoked += MenuItem_Invoked;

            RadialControllerMenuItem blurItem = RadialControllerMenuItem.CreateFromIcon("Blur", icon);
            blurItem.Invoked += MenuItem_Invoked;

            // Add the custom tool to the RadialController menu.
            dialController.Menu.Items.Add(opacityItem);
            dialController.Menu.Items.Add(blurItem);

            dialController.ButtonClicked += Controller_ButtonClicked;
            dialController.RotationChanged += Controller_RotationChanged;
        }

        private void MenuItem_Invoked(RadialControllerMenuItem sender, object args)
        {
            currentDialItem = sender.DisplayText;
        }

        private void Controller_ButtonClicked(RadialController sender, RadialControllerButtonClickedEventArgs args)
        {

        }

        private void Controller_RotationChanged(RadialController sender, RadialControllerRotationChangedEventArgs args)
        {
            if (currentDialItem == "Opacity")
            {
                SliderOpacity.Value += args.RotationDeltaInDegrees;
                ImageControl.Opacity = SliderOpacity.Value / 100;
            }
            else
            {
                SliderBlur.Value += args.RotationDeltaInDegrees;
                double blur = SliderBlur.Value / 100;

            }
        }

        private void DrawBlur()
        {
            LensBlurEffect effect = new LensBlurEffect();
        }
    }
}