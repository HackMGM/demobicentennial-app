using System;
using Newtonsoft.Json;
using System.Net.Http;
using System.Collections.Generic;
using Microsoft.CSharp.RuntimeBinder;

using MapsuiFormsSample.DataObjects;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MapsuiFormsSample
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MarkerInfoPage : ContentPage
    {
        private Marker _marker;

        public MarkerInfoPage(Marker marker)
        {
            _marker = marker;
            this.Title = marker.Title;
            InitializeComponent();

            Label testLabel = new Label
            {
                Text = "Description: " + marker.Description
                            + "\n\nLocation: X: " + marker.LocationSphericalMercator.X
                            + " Y: " + marker.LocationSphericalMercator.Y,
                HorizontalOptions = LayoutOptions.Center
            };
            Button button = new Button
            {
                Text = "Get Directions with Google Maps",
                BorderWidth = 1,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            button.Clicked += OnGoogleMapsButtonClicked;
            ContentGrid.Children.Add(testLabel);
            ContentGrid.Children.Add(button);


        }

        void OnGoogleMapsButtonClicked(object sender, EventArgs e)
        {
            // TODO: load google maps so directions can be gotten.

            // Open URL like
            // https://www.google.com/maps/?q=32.37685,-86.30078333
            Device.OpenUri(new Uri("https://www.google.com/maps/?q="
                                   + _marker.Latitude + "," + _marker.Longitude));
        }
    }
}