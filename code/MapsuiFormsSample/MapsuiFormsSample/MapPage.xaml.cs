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

            var layer = GenerateIconLayer();
            mapControl.NativeMap.Layers.Add(layer);
            mapControl.NativeMap.Layers.Add(CreateLayer(sphericalMercatorCoordinate));
            mapControl.NativeMap.InfoLayers.Add(layer);

       
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

        public static ILayer CreateLayer(Point point)
        {
            var memoryProvider = new MemoryProvider();

            var featureWithDefaultStyle = new Feature { Geometry = point };
            featureWithDefaultStyle.Styles.Add(new LabelStyle { Text = "Default Label" });
            memoryProvider.Features.Add(featureWithDefaultStyle);


            var featureWithRightAlignedStyle = new Feature { Geometry = new Point(0, -2000000) };
            featureWithRightAlignedStyle.Styles.Add(new LabelStyle
            {
                Text = "Right Aligned",
                BackColor = new Brush(Color.Gray),
                HorizontalAlignment = LabelStyle.HorizontalAlignmentEnum.Right
            });
            memoryProvider.Features.Add(featureWithRightAlignedStyle);


            var featureWithBottomAlignedStyle = new Feature { Geometry = new Point(0, -4000000) };
            featureWithBottomAlignedStyle.Styles.Add(new LabelStyle
            {
                Text = "Right Aligned",
                BackColor = new Brush(Color.Gray),
                VerticalAlignment = LabelStyle.VerticalAlignmentEnum.Bottom
            });
            memoryProvider.Features.Add(featureWithBottomAlignedStyle);


            var featureWithColors = new Feature { Geometry = new Point(0, -6000000) };
            featureWithColors.Styles.Add(CreateColoredLabelStyle());
            memoryProvider.Features.Add(featureWithColors);

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
