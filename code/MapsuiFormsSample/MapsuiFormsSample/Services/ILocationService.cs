using System;
using System.Threading.Tasks;

namespace MapsuiFormsSample.Services
{
    public interface ILocationService
    {
        Task StartListening(ILocationServiceChangeWatcher mapPage);
    }
}
