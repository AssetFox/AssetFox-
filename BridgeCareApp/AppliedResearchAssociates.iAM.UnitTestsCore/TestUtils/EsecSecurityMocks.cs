using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BridgeCareCore.Models;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils
{
    public static class EsecSecurityMocks
    {
        public static Mock<IEsecSecurity> AdminMock
        {
            get
            {
                var mock = new Mock<IEsecSecurity>();
                mock.Setup(_ => _.GetUserInformation(It.IsAny<HttpRequest>()))
                .Returns(new UserInfo
                {
                    Name = "pdsystbamsusr01",
                    Role = "PD-BAMS-Administrator",
                    Email = "pdstseseca5@pa.gov"
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
                        Name = "b-bamsadmin",
                        Role = "PD-BAMS-DBEngineer",
                        Email = "jmalmberg@ara.com"
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
