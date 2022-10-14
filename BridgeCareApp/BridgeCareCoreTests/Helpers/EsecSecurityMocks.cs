using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BridgeCareCore.Models;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;

namespace BridgeCareCoreTests
{
    public static class EsecSecurityMocks
    {
        public const string AdminUsername = "pdsystbamsusr01";
        public const string AdminRole = "PD-BAMS-Administrator";
        public const string AdminEmail = "pdstseseca5@pa.gov";

        public const string DbeUsername = "b-bamsadmin";
        public const string DbeRole = "PD-BAMS-DBEngineer";
        public const string DbeEmail = "jmalmberg@ara.com";

        public static Mock<IEsecSecurity> AdminMock
        {
            get
            {
                var mock = new Mock<IEsecSecurity>();
                mock.Setup(_ => _.GetUserInformation(It.IsAny<HttpRequest>()))
                .Returns(new UserInfo
                {
                    Name = AdminUsername,
                    HasAdminAccess = true,
                    HasSimulationAccess = true,
                    Email = AdminEmail,
                });
                return mock;
            }
        }

        public static IEsecSecurity Admin
        {
            get
            {
                var mock = AdminMock;
                return mock.Object;
            }
        }

        public static Mock<IEsecSecurity> DbeMock
        {
            get
            {
                var mock = new Mock<IEsecSecurity>();
                mock.Setup(_ => _.GetUserInformation(It.IsAny<HttpRequest>()))
                    .Returns(new UserInfo
                    {
                        Name = DbeUsername,
                        HasAdminAccess = false,
                        HasSimulationAccess = false,
                        Email = DbeEmail,
                    });
                return mock;
            }
        }

        public static IEsecSecurity Dbe
        {
            get
            {
                return DbeMock.Object;
            }
        }
    }
}
