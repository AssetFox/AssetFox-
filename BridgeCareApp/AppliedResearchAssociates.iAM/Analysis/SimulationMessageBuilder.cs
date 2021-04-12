using System;

namespace AppliedResearchAssociates.iAM.Analysis
{
    public sealed class SimulationMessageBuilder
    {
        public SimulationMessageBuilder(string messageDetail) => MessageDetail = messageDetail;

        public Guid? ItemId { get; set; }

        public string ItemName { get; set; }

        public string MessageDetail { get; }

        public Guid? SectionId { get; set; }

        public string SectionName { get; set; }

        public override string ToString()
        {
            var message = $"{MessageDetail} [Item:{getNameText(ItemName)}{getIdText(ItemId)}{getSectionPart()}]";
            return message;

            string getNameText(string name) => $" \"{name ?? "n/a"}\"";
            string getIdText(Guid? id) => id.HasValue ? $" ({id})" : "";
            string getSectionPart() => SectionName is object || SectionId.HasValue ? $", Section:{getNameText(SectionName)}{getIdText(SectionId)}" : "";
        }
    }
}
