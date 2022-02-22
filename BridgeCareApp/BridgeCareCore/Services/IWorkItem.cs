using System;

namespace BridgeCareCore.Services
{
    public interface IWorkItem
    {
        void DoWork(IServiceProvider serviceProvider);
    }
}
