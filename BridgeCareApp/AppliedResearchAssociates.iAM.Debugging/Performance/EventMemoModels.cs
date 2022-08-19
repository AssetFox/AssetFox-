using System;

namespace AppliedResearchAssociates.iAM.Debugging
{
    public static class EventMemoModels
    {
        public static EventMemoModel Now(string text)
            => new EventMemoModel
            {
                Text = text,
                UtcTime = DateTime.UtcNow
            };
    }
}
