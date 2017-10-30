using System.Diagnostics;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Utilities;
using Mapsui.Projection;
using MapsuiFormsSample.TestData;
using MapsuiFormsSample.DataObjects;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.CSharp.RuntimeBinder;
using System;

namespace MapsuiFormsSample
{
    public partial class MapPage
    {
        private List<Marker> _markersList = new List<Marker>();

        public MapPage()
        {
            this.Title = "Map";
            InitializeComponent();

            var mapControl = new MapsUIView();
            mapControl.NativeMap.Layers.Add(OpenStreetMap.CreateTileLayer());

            mapControl.NativeMap.Layers.Add(CreateLayer());

            // Set the center of the viewport to the coordinate. The UI will refresh automatically
            // mapControl.NativeMap.NavigateTo(sphericalMercatorCoordinate);
            // Additionally you might want to set the resolution, this could depend on your specific purpose
            // mapControl.NativeMap.NavigateTo(mapControl.NativeMap.Resolutions[18]);

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

        public ILayer CreateLayer()
        {
            var memoryProvider = new MemoryProvider();

            string markersJson = TestMarkerData.JsonTestData;

            dynamic markers = JsonConvert.DeserializeObject(markersJson);
            foreach (dynamic marker in markers)
            {
                if ("historic_marker".Equals(marker.type.ToString()))
                {
                    string label = marker.title.ToString();
                    _markersList.Add(new Marker(label, marker.nid.ToString()));

                    try
                    {
                        double lat = marker.field_coordinates.und[0].safe_value;
                        double longitude = marker.field_coordinates.und[1].safe_value;
                        // Get the lon lat coordinates from somewhere (Mapsui can not help you there)
                        // Format (Long, Lat)
                        // Zoom to marker location
                        var currentMarker = new Mapsui.Geometries.Point(longitude, lat);
                        // OSM uses spherical mercator coordinates. So transform the lon lat coordinates to spherical mercator
                        var sphericalMercatorCoordinate = SphericalMercator.FromLonLat(currentMarker.X, currentMarker.Y);


                        // Here is where we could loop through and add all the markers to the map as "Features"
                        var featureWithDefaultStyle = new Feature { Geometry = sphericalMercatorCoordinate };
                        featureWithDefaultStyle.Styles.Add(new LabelStyle { Text = label });
                        memoryProvider.Features.Add(featureWithDefaultStyle);
                    }
                    catch (RuntimeBinderException)
                    {
                        Debug.WriteLine("No valid GPS coordinates found for this marker:" + label);
                    }
                    catch (JsonReaderException e)
                    {
                        // Console.WriteLine("Exception: " + e.Message);
                        //Console.WriteLine("Stack trace: " + e.StackTrace);
                        Debug.WriteLine("Data for " + label + " marker is invalid: " + e.Message);
                    }
                    catch (InvalidCastException)
                    {
                        Debug.WriteLine("GPS coordinates are in invalid format for this marker: " + label);
                    }

                }
            }

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
