using Plugin.Geolocator.Abstractions;
using System;

namespace MapsuiFormsSample.Services
{
    // TODO: Remove if can't make LocationService take this instead of MainPage.
    public interface ILocationServiceChangeWatcher 
    {
        void PositionChanged(object sender, PositionEventArgs e);
    }
}
