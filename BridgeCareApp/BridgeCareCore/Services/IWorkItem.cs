using System;

namespace BridgeCareCore.Services
{
    public interface IWorkItem
    {
        string WorkId { get; }

        void DoWork(IServiceProvider serviceProvider);
    }
}
