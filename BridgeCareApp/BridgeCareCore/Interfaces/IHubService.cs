namespace BridgeCareCore.Interfaces
{
    public interface IHubService
    {
        void SendRealTimeMessage(string method, object arg);

        void SendRealTimeMessage(string method, object arg1, object arg2);
    }
}
