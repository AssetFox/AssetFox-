using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Debugging
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

        public static List<EventMemoModel> Default => GetInstance("Default");
    }
}
