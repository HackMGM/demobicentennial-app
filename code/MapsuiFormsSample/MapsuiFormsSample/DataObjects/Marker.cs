using Mapsui.Geometries;

namespace MapsuiFormsSample.DataObjects
{
    public class Marker
    {
        public Marker(string title, string nid, 
                      Point locationSphericalMercator, 
                      string description)
        {
            this.Description = description;
            this.LocationSphericalMercator = locationSphericalMercator;
            this.NodeId = nid;
            this.Title = title;
        }

        public string Description { private set; get; }
        public Point LocationSphericalMercator { private set; get; }
        public string NodeId { private set; get; }
        public string Title { private set; get; }


    }
}