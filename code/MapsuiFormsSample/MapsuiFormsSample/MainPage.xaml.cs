using System;
using Xamarin.Forms;
using System.Collections.Generic;
using MapsuiFormsSample.DataObjects;
using MapsuiFormsSample.Services;
using System.Diagnostics;
using System.Threading.Tasks;
using Plugin.Permissions.Abstractions;
using Plugin.Permissions;
#if __MOBILE__
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
#endif

namespace MapsuiFormsSample
{
    public partial class MainPage : ILocationServiceChangeWatcher
    {
        //HttpClient client = null;
        private IMarkerService _markerService;
        private ILocationService _locationService;

        public MainPage()
        {
            _markerService = new MarkerService();
            _locationService = new LocationService();

            // Required line when using XAML file.
            InitializeComponent();
            /*
            client = new HttpClient();
            client.BaseAddress = new Uri($"http://13.82.106.207/");
            */
            //ShowTestButton();
            ShowMarkersList();

            InitLocationChangeListener();
        }

        private async void InitLocationChangeListener()
        {
            Position userPosition = null;
#if __MOBILE__
            if (IsLocationAvailable())
            {
                userPosition = await GetCurrentLocation();
                _locationService.StartListening(this);
            }
            else
            {
                RequestLocationPermission();
            }
#endif
        }

#if __MOBILE__
        // Begin adapted from https://github.com/jamesmontemagno/permissionsplugin
        public async void RequestLocationPermission()
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                if (status != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                    {
                        await DisplayAlert("Need location", "Location needed to allow you to play this game", "OK");
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                    //Best practice to always check that the key exists
                    if (results.ContainsKey(Permission.Location))
                    {
                        status = results[Permission.Location];
                    }
                    _locationService.StartListening(this);
                }

                if (status == PermissionStatus.Granted)
                {
                    var results = await CrossGeolocator.Current.GetPositionAsync(TimeSpan.FromSeconds(20));
                    Debug.WriteLine("Lat: " + results.Latitude + " Long: " + results.Longitude);
                    _locationService.StartListening(this);
                }
                else if (status != PermissionStatus.Unknown)
                {
                    await DisplayAlert("Location Denied", "Can not continue, try again.", "OK");
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
        public async Task<Position> GetCurrentLocation()
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

        public bool IsLocationAvailable()
        {
#if __MOBILE__
            if (!CrossGeolocator.IsSupported)
                return false;

            return CrossGeolocator.Current.IsGeolocationAvailable;
#else
                return false;
#endif
        }


        void ShowTestButton()
        {
            Label testLabel = new Label
            {
                Text = "Test Label",
                HorizontalOptions = LayoutOptions.Center
            };
            Button button = new Button
            {
                Text = "Click Me!",
                BorderWidth = 1,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            button.Clicked += OnButtonClicked;
            ContentGrid.Children.Add(testLabel);
            ContentGrid.Children.Add(button);
        }

        async void ShowMarkersList()
        {
            List<Marker> markersList = await _markerService.GetAllMarkers();

            // Create the ListView.
            ListView listView = new ListView
            {
                // Source of data items.
                ItemsSource = markersList,

                // Define template for displaying each item.
                // (Argument of DataTemplate constructor is called for 
                //      each item; it must return a Cell derivative.)
                ItemTemplate = new DataTemplate(() =>
                {
                    // Create views with bindings for displaying each property.
                    Label nameLabel = new Label();
                    nameLabel.SetBinding(Label.TextProperty, "Title");

                    Label nodeIdLabel = new Label();
                    nodeIdLabel.SetBinding(Label.TextProperty,
                        new Binding("NodeId", BindingMode.OneWay,
                            null, null, "Node Id:  {0:d}"));


                    // Return an assembled ViewCell.
                    return new ViewCell
                    {
                        View = new StackLayout
                        {
                            Padding = new Thickness(0, 5),
                            Orientation = StackOrientation.Horizontal,
                            Children =
                                {
                                    new StackLayout
                                    {
                                        VerticalOptions = LayoutOptions.Center,
                                        Spacing = 0,
                                        Children =
                                        {
                                            nameLabel,
                                            nodeIdLabel
                                        }
                                        }
                                }
                        }
                    };
                })
            };

            listView.ItemTapped += (sender, args) =>
            {
                Marker marker = args.Item as Marker;
                if (marker == null)
                {
                    return;
                }
                // TODO: Show marker detail screen
                //ShowMarkerLocation(marker.Title, "/?q=mobileapi/node/" + marker.NodeId);
                listView.SelectedItem = null;
            };

            ContentGrid.Children.Add(listView);

        }

        async void OnButtonClicked(object sender, EventArgs e)
        {
            // TODO: load the specific item clicked.
            // ShowMarkerLocation("Test hardcoded marker", "/?q=mobileapi/node/2.json");
        }

        public async void PositionChanged(object sender, PositionEventArgs e)
        {
            Debug.WriteLine("PositionChanged called");
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
            /*if (_initialLoadCompleted)
            {
                // Redraw map

                _mapControl.NativeMap.Layers.Remove(_layer);
                _layer = await GenerateLayer(position);

                _mapControl.NativeMap.Layers.Add(_layer);
            }
            else
            {
                Debug.WriteLine("Initial load not completed so skipping map redraw");
            }
            */
        }

        /*
        async void ShowMarkerLocation(string title, string url)
        {
            var json = await client.GetStringAsync(url);
            try
            {
                dynamic stuff = JsonConvert.DeserializeObject(json);
                double lat = stuff.field_coordinates.und[0].safe_value;
                double longitude = stuff.field_coordinates.und[1].safe_value;
                // TODO: Open Google maps URL.
                //await Navigation.PushAsync(new MapPage(title, longitude, lat));
            }
            catch (RuntimeBinderException)
            {
                await DisplayAlert("Alert", "No valid GPS coordinates found for this marker", "OK");
            }
            catch (JsonReaderException e)
            {
                // Console.WriteLine("Exception: " + e.Message);
                //Console.WriteLine("Stack trace: " + e.StackTrace);
                await DisplayAlert("Alert", "Data for this marker is invalid: " + e.Message, "OK");


            }
            catch (InvalidCastException)
            {
                await DisplayAlert("Alert", "GPS coordinates are in invalid format for this marker", "OK");
            }
        }
        */


    }
}
