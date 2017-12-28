using Plugin.Geolocator.Abstractions;
using System;

namespace MapsuiFormsSample.Services
{
    public interface ILocationServiceChangeWatcher
    {
        void PositionChanged(object sender, PositionEventArgs e);
    }
}
