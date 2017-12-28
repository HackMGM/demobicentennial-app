using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace MapsuiFormsSample.Services
{
    public class LocationService : ILocationService
    {
        // Begin adapted from https://jamesmontemagno.github.io/GeolocatorPlugin/LocationChanges.html
        public async Task StartListening()
        {
            Debug.WriteLine("LocationService.StartListening() called.");

            if (CrossGeolocator.Current.IsListening)
            {
                Debug.WriteLine("LocationService.IsListening is true. Not starting listening...");

                // TODO: If IsListening is true do we really want to return here?
                return;
            }
            else
            {
                Debug.WriteLine("LocationService.IsListening is false. Starting listening...");

            }

            await CrossGeolocator.Current.StartListeningAsync(TimeSpan.FromSeconds(5), 1, true);

            CrossGeolocator.Current.PositionChanged += PositionChanged;
            CrossGeolocator.Current.PositionError += PositionError;
        }

        private void PositionChanged(object sender, PositionEventArgs e)
        {

            //If updating the UI, ensure you invoke on main thread
            var position = e.Position;
            var output = "PositionChanged() called. Full: Lat: " + position.Latitude + " Long: " + position.Longitude;
            output += "\n" + $"Time: {position.Timestamp}";
            output += "\n" + $"Heading: {position.Heading}";
            output += "\n" + $"Speed: {position.Speed}";
            output += "\n" + $"Accuracy: {position.Accuracy}";
            output += "\n" + $"Altitude: {position.Altitude}";
            output += "\n" + $"Altitude Accuracy: {position.AltitudeAccuracy}";
            Debug.WriteLine(output);
        }

        private void PositionError(object sender, PositionErrorEventArgs e)
        {
            Debug.WriteLine("LocationService: PositionError() called.");
            Debug.WriteLine(e.Error);
            //Handle event here for errors
        }

        async Task StopListening()
        {
            if (!CrossGeolocator.Current.IsListening)
                return;

            await CrossGeolocator.Current.StopListeningAsync();

            CrossGeolocator.Current.PositionChanged -= PositionChanged;
            CrossGeolocator.Current.PositionError -= PositionError;
        }

        // End adapted from https://jamesmontemagno.github.io/GeolocatorPlugin/LocationChanges.html
    }
}
