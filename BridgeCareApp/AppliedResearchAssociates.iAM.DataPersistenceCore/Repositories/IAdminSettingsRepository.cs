using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Common.Logging;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using Attribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;
using System.Drawing;

public interface IAdminSettingsRepository
{
    IList<string> GetKeyFields();

    void SetKeyFields(string keyFields);

    void SetPrimaryNetwork(string name);

    string GetPrimaryNetwork();

    IList<string> GetSimulationReportNames();

    void SetInventoryReports(string inventoryReports);

    string GetAttributeName(Guid attributeId);

    void SetSimulationReports(string simulationReports);

    string GetImplementationName();

    void SetImplementationName(string name);

    string GetAgencyLogo();

    void SetAgencyLogo(Image agencyLogo);

    string GetImplementationLogo();

    void SetImplementationLogo(Image productLogo);
}
