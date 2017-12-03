using System;
using System.Collections.Generic;

namespace MapsuiFormsSample.DataObjects.Dto
{
    public class MarkerDto
    {
        public string City { set; get; }

        public string County { set; get; }

        public string title { set; get; }

        public List<string> Coordinates { set; get; }

    }
}
