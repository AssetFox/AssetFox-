using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Common.PerformanceMeasurement
{
    public static class EventMemoModelLists
    {
        private static Dictionary<object, List<EventMemoModel>> Instances { get; set; }
            = new Dictionary<object, List<EventMemoModel>>();
        public static List<EventMemoModel> StartingNow()
            => new List<EventMemoModel>
            {
                EventMemoModels.Now("")
            };

        public static List<EventMemoModel> GetInstance(object key)
        {
            if (!Instances.ContainsKey(key))
            {
                Instances[key] = StartingNow();
            }
            return Instances[key];
        }

        /// <summary>Gets a "fresh" instance, deleting whatever was there previously.</summary> 
        public static List<EventMemoModel> GetFreshInstance(string key)
        {
            if (Instances.ContainsKey(key))
            {
                Instances.Remove(key);
            }
            return GetInstance(key);
        }

        public static List<EventMemoModel> Default => GetInstance("Default");
    }
}
