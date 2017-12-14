using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Mapsui.Geometries;
using Mapsui.Projection;
using MapsuiFormsSample.DataObjects;
using MapsuiFormsSample.DataObjects.Dto;
using MapsuiFormsSample.Helpers;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;

namespace MapsuiFormsSample.Services
{
    public class MarkerService : IMarkerService
    {
        HttpClient _client = null;
        IHtmlHelper _htmlHelper;

        public MarkerService()
        {
            _client = new HttpClient();
            // TODO: Get from config file instead of hard-coding
            _client.BaseAddress = new Uri($"http://13.82.106.207/");

            _htmlHelper = new HtmlHelper();
        }



        public async Task<List<Marker>> GetAllMarkers()
        {

            List<MarkerDto> markerDtos = null;
            var json = await _client.GetStringAsync($"/?q=mobileapi/markersjson.json");
            try
            {
                markerDtos = JsonConvert.DeserializeObject<List<MarkerDto>>(json);
            }
            catch (JsonReaderException e)
            {
                Debug.WriteLine("Error parsing markersjson: " + e.ToString());

            }
            List<Marker> markersList = new List<Marker>();
            int tempNodeId = 1;
            foreach (MarkerDto markerDto in markerDtos)
            {

                string label = _htmlHelper.ExtractText(markerDto.title);

                try
                {
                    double lat = Double.Parse(markerDto.Coordinates[0]);
                    double longitude = Double.Parse(markerDto.Coordinates[1]);
                    // Format (Long, Lat)
                    // Zoom to marker location
                    var currentMarker = new Mapsui.Geometries.Point(longitude, lat);
                    // OSM uses spherical mercator coordinates. So transform the lon lat coordinates to spherical mercator
                    Point sphericalMercatorCoordinate = SphericalMercator.FromLonLat(currentMarker.X, currentMarker.Y);
                    string description = "Marker is located in city of " + _htmlHelper.ExtractText(markerDto.City)
                                                                                      + " and county: " + _htmlHelper.ExtractText(markerDto.County);
                    markersList.Add(new Marker(label, tempNodeId.ToString(), sphericalMercatorCoordinate, description));
                }
                catch (InvalidCastException e)
                {
                    Debug.WriteLine("GPS coordinates are in invalid format for this marker: " + label + " exception: " + e.ToString());
                }
                catch (Exception e)
                {
                    // TODO: Show user
                    Debug.WriteLine("Exception: " + e.ToString());
                }

            }

            return markersList;
        }
    }
}
