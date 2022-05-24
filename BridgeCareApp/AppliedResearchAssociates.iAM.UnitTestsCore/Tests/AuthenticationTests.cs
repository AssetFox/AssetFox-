using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class AuthenticationTests
    {        
        private HttpClient _client;
        private Mock<HttpMessageHandler> handlerMock;

        public AuthenticationTests()
        {                    
            SetupMockClient();
        }

        private void SetupMockClient()
        {                  
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(@"Successful operation"),
            };

            handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(response);
            _client = new HttpClient(handlerMock.Object);
            _client.BaseAddress = new Uri("http://localhost/");
        }
                
        [Fact]
        public async Task ShouldGetUserTokens()
        {
            try
            {
                // Arrange
                var code = "ZDVhZjg1MGMtZTVjMS00ZWIyLWE0OWUtNmQ5NGI2YmIzY2UyLUh5a2RyZGZVdXFvRTNHSDUwZE1YWHRZcm5pdz0=";
                
                // Act
                var response = await _client.GetAsync($"/api/Authentication/UserTokens/{code}");

                //Assert                
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
               ItExpr.IsAny<CancellationToken>());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
