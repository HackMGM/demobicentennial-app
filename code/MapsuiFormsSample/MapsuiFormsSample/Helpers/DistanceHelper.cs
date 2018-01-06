using System;
namespace MapsuiFormsSample.Helpers
{
    public class DistanceHelper
    {

        /* By Stuart Lodge http://slodge.blogspot.com/2012/04/calculating-distance-between-latlng.html
         * Note that the license on this snippet is CC attribution - because its derived from movable type -http://www.movable-type.co.uk/scripts/latlong.html:
I offer these formula & scripts for free use and adaptation as my contribution to the open-source info-sphere from which I have received so much. You are welcome to re-use these scripts [under a simple attribution license, without any warranty express or implied] provided solely that you retain my copyright notice and a reference to this page.
*/
        /// 

        /// Calculates the distance between two points of latitude and longitude.
        /// Great Link - http://www.movable-type.co.uk/scripts/latlong.html
        /// 

        /// 
        // First coordinate.
        /// 
        //First coordinate.
        /// 
        //Second coordinate.
        /// 
        //Second coordinate.
        /// the distance in metres
        public static Double DistanceInMetres(double lat1, double lon1, double lat2, double lon2)
        {

            if (lat1 == lat2 && lon1 == lon2)
                return 0.0;

            var theta = lon1 - lon2;

            var distance = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) +
                           Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) *
                           Math.Cos(deg2rad(theta));

            distance = Math.Acos(distance);
            if (double.IsNaN(distance))
                return 0.0;

            distance = rad2deg(distance);
            distance = distance * 60.0 * 1.1515 * 1609.344;

            return (distance);
        }

        private static double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        private static double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }
    }

}
