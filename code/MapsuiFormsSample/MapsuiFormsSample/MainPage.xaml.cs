using System;
using System.Linq;
using Xamarin.Forms;
using System.Collections.Generic;
using MapsuiFormsSample.DataObjects;
using MapsuiFormsSample.Services;
using System.Diagnostics;
using System.Threading.Tasks;
using Plugin.Permissions.Abstractions;
using Plugin.Permissions;
using MapsuiFormsSample.DataObjects.UI;
using MapsuiFormsSample.Helpers;
#if __MOBILE__
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
#endif

namespace MapsuiFormsSample
{
    public partial class MainPage : ILocationServiceChangeWatcher
    {
        private IMarkerService _markerService;
        private ILocationService _locationService;

        private Double? _currentLat = null;
        private Double? _currentLong = null;
        ListView _listView;
        List<Marker> _markersList;

        public MainPage()
        {
            this.Title = "Closest Markers";

            _markerService = new MarkerService();
            _locationService = new LocationService(this);

            // Required line when using XAML file.
            InitializeComponent();
            //ShowTestButton();
            ShowMarkersList();

            _locationService.InitLocationChangeListener();
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

        private List<MarkerListViewItem> GetMarkersWithDistances(List<Marker> markersList)
        {
            List<MarkerListViewItem> list = new List<MarkerListViewItem>();
            foreach (Marker marker in markersList)
            {
                var item = new MarkerListViewItem()
                {
                    Marker = marker
                };


                if (_currentLat != null && _currentLong != null)
                {
                    /*
                    // Adapted from https://forums.xamarin.com/discussion/comment/34987/#Comment_34987
                    double d = Math.Acos(
                        (Math.Sin((double)_currentLat) * Math.Sin(marker.Latitude)) +
                        (Math.Cos((double)_currentLat) * Math.Cos(marker.Latitude))
                        * Math.Cos(marker.Longitude - (double)_currentLong));

                    // in meters
                    //item.DistanceAway = (6378137 * d).ToString();
                    // in miles
                    item.DistanceAway = (3959 * d).ToString();
                    */

                    Double distInMeters = DistanceHelper.DistanceInMetres((double)_currentLat, (double)_currentLong, marker.Latitude, marker.Longitude);
                    Decimal distInMiles = (Decimal)(distInMeters * 0.000621371);
                    Decimal distRounded = Decimal.Round(distInMiles, 1);
                    item.DistanceAway = distRounded.ToString() + " mi";
                    item.DistanceAwayDecimal = distRounded;
                }
                else
                {
                    item.DistanceAway = "Unknown";
                    item.DistanceAwayDecimal = 0;


                }


                list.Add(item);
            }
            return list;
        }

        async void ShowMarkersList()
        {
            _markersList = await _markerService.GetAllMarkers();

            List<MarkerListViewItem> markersWithDistances = GetMarkersWithDistances(_markersList);

            // Create the ListView.
            _listView = new ListView
            {
                // Source of data items.
                ItemsSource = markersWithDistances,

                // Define template for displaying each item.
                // (Argument of DataTemplate constructor is called for 
                //      each item; it must return a Cell derivative.)
                ItemTemplate = new DataTemplate(() =>
                {
                    // Create views with bindings for displaying each property.
                    Label nameLabel = new Label();
                    nameLabel.SetBinding(Label.TextProperty, "Marker.Title");

                    Label nodeIdLabel = new Label();
                    nodeIdLabel.SetBinding(Label.TextProperty, "DistanceAway");
                    /*nodeIdLabel.SetBinding(Label.TextProperty,
                        new Binding("DistanceAway", BindingMode.OneWay,
                            null, null, "Distance:  {0}"));
*/

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

            _listView.ItemTapped += async (sender, args) =>
            {
                MarkerListViewItem marker = args.Item as MarkerListViewItem;
                if (marker == null)
                {
                    return;
                }
                // TODO: Show marker detail screen
                //ShowMarkerLocation(marker.Title, "/?q=mobileapi/node/" + marker.NodeId);

                await Navigation.PushAsync(new MarkerInfoPage(marker.Marker));
                _listView.SelectedItem = null;
            };

            ContentGrid.Children.Add(_listView);

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
            var output = "PositionChanged() called. Full: Lat: "
                + position.Latitude + " Long: " + position.Longitude;
            output += "\n" + $"Time: {position.Timestamp}";
            output += "\n" + $"Heading: {position.Heading}";
            output += "\n" + $"Speed: {position.Speed}";
            output += "\n" + $"Accuracy: {position.Accuracy}";
            output += "\n" + $"Altitude: {position.Altitude}";
            output += "\n" + $"Altitude Accuracy: {position.AltitudeAccuracy}";
            Debug.WriteLine(output);

            _currentLat = position.Latitude;
            _currentLong = position.Longitude;

            // Refresh marker list and recalculate distance away.
            List<MarkerListViewItem> markersWithDistances = GetMarkersWithDistances(_markersList);
            markersWithDistances.Sort((m1, m2) => m1.DistanceAwayDecimal.CompareTo(m2.DistanceAwayDecimal));

            _listView.ItemsSource = markersWithDistances;
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
