namespace BridgeCareCore.Interfaces
{
    public interface IHubService
    {
        void SendRealTimeMessage(string username, string method, object arg);

        void SendRealTimeMessage(string username, string method, object arg1, object arg2);
    }
}
