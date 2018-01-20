using System;
namespace MapsuiFormsSample.DataObjects.UI
{
    public class MarkerListViewItem
    {
        public MarkerListViewItem()
        {
        }

        public Marker Marker { get; set; }
        public decimal DistanceAwayDecimal { get; set; }
        public string DistanceAway { get; set; }
    }
}
