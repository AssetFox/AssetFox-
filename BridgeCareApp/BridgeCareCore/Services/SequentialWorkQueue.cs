using System.Collections.Concurrent;

namespace BridgeCareCore.Services
{
    public class SequentialWorkQueue : BlockingCollection<IWorkItem>
    {
    }
}
