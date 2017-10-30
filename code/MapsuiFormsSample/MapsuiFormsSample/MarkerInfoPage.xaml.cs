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
        public MarkerInfoPage(Marker marker)
        {
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
                Text = "Google Maps (button not yet functional)",
                BorderWidth = 1,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            button.Clicked += OnGoogleMapsButtonClicked;
            ContentGrid.Children.Add(testLabel);
            ContentGrid.Children.Add(button);


        }

        async void OnGoogleMapsButtonClicked(object sender, EventArgs e)
        {
            // TODO: load google maps so directions can be gotten.
        }
    }
}