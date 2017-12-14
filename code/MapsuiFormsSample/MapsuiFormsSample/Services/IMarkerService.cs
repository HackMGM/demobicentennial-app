using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MapsuiFormsSample.DataObjects;

namespace MapsuiFormsSample.Services
{
    public interface IMarkerService
    {
        Task<List<Marker>> GetAllMarkers();
    }
}
