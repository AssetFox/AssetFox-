using System;

namespace AppliedResearchAssociates.iAM.Analysis.Engine;

internal sealed class SimulationMessageBuilder
{
    public SimulationMessageBuilder(string messageDetail) => MessageDetail = messageDetail;

    public Guid? ItemId { get; set; }

    public string ItemName { get; set; }

    public string MessageDetail { get; }

    public Guid? AssetId { get; set; }

    public string AssetName { get; set; }

    public override string ToString()
    {
        var message = $"{MessageDetail} [Item:{getNameText(ItemName)}{getIdText(ItemId)}{getAssetPart()}]";
        return message;

        string getNameText(string name) => $" \"{name ?? "n/a"}\"";
        string getIdText(Guid? id) => id.HasValue ? $" ({id})" : "";
        string getAssetPart() => AssetName is object || AssetId.HasValue ? $", Asset:{getNameText(AssetName)}{getIdText(AssetId)}" : "";
    }
}
