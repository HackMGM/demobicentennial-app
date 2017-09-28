
namespace MapsuiFormsSample.DataObjects
{
    public class Marker
    {
        public Marker(string title, string nid)
        {
            this.Title = title;
            this.NodeId = nid;
        }

        public string Title { private set; get; }
        public string NodeId { private set; get; }

    }
}