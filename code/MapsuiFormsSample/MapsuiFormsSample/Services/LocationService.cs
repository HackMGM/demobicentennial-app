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
        ILocationServiceChangeWatcher _watcher;

        // Begin adapted from https://jamesmontemagno.github.io/GeolocatorPlugin/LocationChanges.html
        public async Task StartListening(ILocationServiceChangeWatcher watcher)
        {
            _watcher = watcher;
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
            int minTimeInSeconds = 5;
            int minDistince = 1; // TBD: Units. Probably change to 10 instead of 1.
            bool includeHeading = true; // TODO: perhaps draw heading on map so user knows which direction they are facing
            await CrossGeolocator.Current.StartListeningAsync(TimeSpan.FromSeconds(minTimeInSeconds), minDistince, includeHeading);

            CrossGeolocator.Current.PositionChanged += _watcher.PositionChanged;
            CrossGeolocator.Current.PositionError += PositionError;
        }

        /*
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
        */

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

            CrossGeolocator.Current.PositionChanged -= _watcher.PositionChanged;
            CrossGeolocator.Current.PositionError -= PositionError;
        }

        // End adapted from https://jamesmontemagno.github.io/GeolocatorPlugin/LocationChanges.html
    }
}
