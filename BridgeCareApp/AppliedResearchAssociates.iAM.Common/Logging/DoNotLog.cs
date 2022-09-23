namespace AppliedResearchAssociates.iAM.Common
{
    public class DoNotLog : ILog
    {
        public void Debug(string message) { }
        public void Error(string message) { }
        public void Information(string message) { }
        public void Warning(string message) { }
    }
}
