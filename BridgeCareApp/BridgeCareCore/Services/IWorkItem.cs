using System;
using System.Threading;
using BridgeCareCore.Models;

namespace BridgeCareCore.Services
{
    public interface IWorkItem
    {
        UserInfo UserInfo { get; }

        string WorkId { get; }

        void DoWork(IServiceProvider serviceProvider, CancellationToken cancellationToken = default(CancellationToken));
    }
}
