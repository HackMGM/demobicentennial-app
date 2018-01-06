using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace MapsuiFormsSample.Services
{
    public class LocationService : ILocationService
    {
        MainPage _watcher;

        public LocationService(MainPage watcher) 
        {
            _watcher = watcher;
        }

        public async void InitLocationChangeListener()
        {
            Position userPosition = null;
#if __MOBILE__
            if (IsLocationAvailable())
            {
                userPosition = await GetCurrentLocation();
                await StartListening();
            }
            else
            {
                RequestLocationPermission();
            }
#endif
        }

#if __MOBILE__
        // Begin adapted from https://github.com/jamesmontemagno/permissionsplugin
        private async void RequestLocationPermission()
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                if (status != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                    {
                        await _watcher.DisplayAlert("Need location", "Location needed to allow you to play this game", "OK");
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                    //Best practice to always check that the key exists
                    if (results.ContainsKey(Permission.Location))
                    {
                        status = results[Permission.Location];
                    }
                    await StartListening();
                }

                if (status == PermissionStatus.Granted)
                {
                    var results = await CrossGeolocator.Current.GetPositionAsync(TimeSpan.FromSeconds(20));
                    Debug.WriteLine("Lat: " + results.Latitude + " Long: " + results.Longitude);
                    await StartListening();
                }
                else if (status != PermissionStatus.Unknown)
                {
                    await _watcher.DisplayAlert("Location Denied", "Can not continue, try again.", "OK");
                }
            }
            catch (Exception ex)
            {

                Debug.WriteLine("Error: " + ex);
            }
        }
        // End adapted from https://github.com/jamesmontemagno/permissionsplugin
#endif

#if __MOBILE__
        // Begin adapted from https://jamesmontemagno.github.io/GeolocatorPlugin/CurrentLocation.html
        private async Task<Position> GetCurrentLocation()
        {
            Position position = null;
            try
            {
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 100;

                position = await locator.GetLastKnownLocationAsync();

                if (position != null)
                {
                    Debug.WriteLine("TMP DEBUG: USING CACHED Position: Lat: " + position.Latitude + " Long: " + position.Longitude);
                    return position;
                }

                if (!locator.IsGeolocationAvailable || !locator.IsGeolocationEnabled)
                {
                    //not available or enabled
                    Debug.WriteLine("Location not available or enabled.");
                    return null;
                }

                position = await locator.GetPositionAsync(TimeSpan.FromSeconds(20), null, true);
                Debug.WriteLine("TMP DEBUG: Uncached Position: Lat: " + position.Latitude + " Long: " + position.Longitude);
                return position;
            }
            catch (Exception ex)
            {
                //Display error as we have timed out or can't get location.
                Debug.WriteLine("Error getting user's current location: " + ex);
                return null;
            }

        }
        // End adapted from https://jamesmontemagno.github.io/GeolocatorPlugin/CurrentLocation.html
#endif

        private bool IsLocationAvailable()
        {
#if __MOBILE__
            if (!CrossGeolocator.IsSupported)
                return false;

            return CrossGeolocator.Current.IsGeolocationAvailable;
#else
                return false;
#endif
        }

        // Begin adapted from https://jamesmontemagno.github.io/GeolocatorPlugin/LocationChanges.html
        private async Task StartListening()
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
            // Don't update locations more often than every 30 seconds
            int minTimeInSeconds = 30;
            int minDistince = 10; // TBD: Units. 10 was default from example.
            bool includeHeading = true; // Maybe set to false unless end up using.
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

        private async Task StopListening()
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
