using System;
using System.Threading.Tasks;

namespace SampleApp.Core.ViewModels
{
    public interface ILoadable
    {
        Task Load();
    }
}
