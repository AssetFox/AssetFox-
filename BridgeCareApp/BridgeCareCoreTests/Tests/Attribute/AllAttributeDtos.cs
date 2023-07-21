using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Models;
using BridgeCareCore.Utils;
using BridgeCareCoreTests;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes
{
    public static class AllAttributeDtos
    {
        public static AllAttributeDTO BrKey(AllDataSource dataSource, Guid? id = null)
        {
            var resolvedId = id ?? Guid.NewGuid();
            var dto = new AllAttributeDTO
            {
                AggregationRuleType = AggregationRuleTypeNames.Predominant,
                Command = "",
                DataSource = dataSource,
                DefaultValue = "Unknown",
                IsAscending = false,
                IsCalculated = false,
                Id = resolvedId,
                Maximum = null,
                Minimum = null,
                Name = "BRKEY",
                Type = AttributeTypeNames.String,
            };
            return dto;
        }

        public static AllAttributeDTO ForAttribute(AttributeDTO attribute)
        {
            var dataSource = AllDataSourceDtoFakeFrontEndFactory.ToAll(attribute.DataSource);
            var allAttribute = new AllAttributeDTO
            {
                AggregationRuleType = attribute.AggregationRuleType,
                DefaultValue = attribute.DefaultValue,
                Command = attribute.Command,
                Id = attribute.Id,
                Maximum = attribute.Maximum,
                Minimum = attribute.Minimum,
                IsAscending = attribute.IsAscending,
                IsCalculated = attribute.IsCalculated,
                Type = attribute.Type,
                Name = attribute.Name,
                DataSource = dataSource,
            };
            return allAttribute;
        }

        public static List<AllAttributeDTO> ForAttributes(params AttributeDTO[] attributes)
        {
            var list = new List<AllAttributeDTO>();
            foreach (var attribute in attributes)
            {
                var all = ForAttribute(attribute);
                list.Add(all);
            }
            return list;
        }
    }
}
