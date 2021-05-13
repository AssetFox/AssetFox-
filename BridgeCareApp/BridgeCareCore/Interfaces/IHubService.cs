namespace BridgeCareCore.Interfaces
{
    public interface IHubService
    {
        void SendRealTimeMessage(string userId, string method, object arg);

        void SendRealTimeMessage(string userId, string method, object arg1, object arg2);
    }
}
