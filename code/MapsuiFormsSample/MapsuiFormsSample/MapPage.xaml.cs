using System.Diagnostics;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Utilities;
using Mapsui.Projection;

namespace MapsuiFormsSample
{
    public partial class MapPage
    {
        public MapPage(string title, double longitude, double lat)
        {
            this.Title = title;
            InitializeComponent();

            var mapControl = new MapsUIView();
            mapControl.NativeMap.Layers.Add(OpenStreetMap.CreateTileLayer());

            // Get the lon lat coordinates from somewhere (Mapsui can not help you there)
            // Format (Long, Lat)
            // Zoom to marker location
            var currentMarker = new Mapsui.Geometries.Point(longitude, lat);
            // OSM uses spherical mercator coordinates. So transform the lon lat coordinates to spherical mercator
            var sphericalMercatorCoordinate = SphericalMercator.FromLonLat(currentMarker.X, currentMarker.Y);

            mapControl.NativeMap.Layers.Add(CreateLayer(title, sphericalMercatorCoordinate));
       
            // Set the center of the viewport to the coordinate. The UI will refresh automatically
            mapControl.NativeMap.NavigateTo(sphericalMercatorCoordinate);
            // Additionally you might want to set the resolution, this could depend on your specific purpose
            mapControl.NativeMap.NavigateTo(mapControl.NativeMap.Resolutions[18]);

            mapControl.NativeMap.Info += (sender, args) =>
            {
                var layername = args.Layer?.Name;
                var featureLabel = args.Feature?["Label"]?.ToString();
                var featureType = args.Feature?["Type"]?.ToString();

                Debug.WriteLine("Info Event was invoked.");
                Debug.WriteLine("Layername: " + layername);
                Debug.WriteLine("Feature Label: " + featureLabel);
                Debug.WriteLine("Feature Type: " + featureType);

                Debug.WriteLine("World Postion: {0:F4} , {1:F4}", args.WorldPosition?.X, args.WorldPosition?.Y);
                Debug.WriteLine("Screen Postion: {0:F4} , {1:F4}", args.ScreenPosition?.X, args.ScreenPosition?.Y);
            };

            ContentGrid.Children.Add(mapControl);

          

        }

        public static ILayer CreateLayer(string label, Point point)
        {
            var memoryProvider = new MemoryProvider();

            var featureWithDefaultStyle = new Feature { Geometry = point };
            featureWithDefaultStyle.Styles.Add(new LabelStyle { Text = label });
            memoryProvider.Features.Add(featureWithDefaultStyle);
            

            return new MemoryLayer { Name = "Points with labels", DataSource = memoryProvider };
        }

        private static IStyle CreateColoredLabelStyle()
        {
            return new LabelStyle
            {
                Text = "Colors",
                BackColor = new Brush(Color.Blue),
                ForeColor = Color.White,
                Halo = new Pen(Color.Red, 4)
            };
        }


       
    }
}
