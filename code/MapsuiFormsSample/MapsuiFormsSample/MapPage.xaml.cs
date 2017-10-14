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

            var layer = GenerateIconLayer();
            mapControl.NativeMap.Layers.Add(layer);
            mapControl.NativeMap.InfoLayers.Add(layer);

            // Get the lon lat coordinates from somewhere (Mapsui can not help you there)
            // Format (Long, Lat)
            // Zoom to marker location
            var currentMarker = new Mapsui.Geometries.Point(longitude, lat);
            // OSM uses spherical mercator coordinates. So transform the lon lat coordinates to spherical mercator
            var sphericalMercatorCoordinate = SphericalMercator.FromLonLat(currentMarker.X, currentMarker.Y);
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
        
        private ILayer GenerateIconLayer()
        {
            var layername = "My Local Layer";
            return new Layer(layername)
            {
                Name = layername,
                DataSource = new MemoryProvider(GetIconFeatures()),
                // Triangle near hamburg.
                Style = new SymbolStyle
                {
                    SymbolScale = 0.8,
                    Fill = new Brush(Mapsui.Styles.Color.Blue),
                    Outline = { Color = Mapsui.Styles.Color.Red, Width = 1 }
                }
            };
        }

        private Features GetIconFeatures()
        {
            var features = new Features();
            var feature = new Feature
            {

                Geometry = new Polygon(new LinearRing(new[]
                        {
                            new Mapsui.Geometries.Point(1066689.6851, 6892508.8652),
                            new Mapsui.Geometries.Point(1005540.0624, 6987290.7802),
                            new Mapsui.Geometries.Point(1107659.9322, 7056389.8538),
                            new Mapsui.Geometries.Point(1066689.6851, 6892508.8652)
                        })),
                ["Label"] = "My Feature Label",
                ["Type"] = "My Feature Type"
            };

            features.Add(feature);
            return features;
        }
    }
}
