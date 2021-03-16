using System;
using System.IO;
using System.Net;
using System.Net.Http;
using BridgeCareCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class AuthenticationTests
    {
        private readonly TestServer _testServer;
        private readonly HttpClient _client;

        public AuthenticationTests()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("testConnections.json")
                .AddJsonFile("testEsec.json")
                .Build();
            var builder = new WebHostBuilder()
                .UseEnvironment("MsSqlDebug")
                .UseConfiguration(config)
                .UseStartup<Startup>();
            _testServer = new TestServer(builder);
            _client = _testServer.CreateClient();
        }

        //ZDVhZjg1MGMtZTVjMS00ZWIyLWE0OWUtNmQ5NGI2YmIzY2UyLUh5a2RyZGZVdXFvRTNHSDUwZE1YWHRZcm5pdz0=
        [Fact]
        public async void ShouldGetUserTokens()
        {
            try
            {
                // Arrange
                var code = "ZDVhZjg1MGMtZTVjMS00ZWIyLWE0OWUtNmQ5NGI2YmIzY2UyLUh5a2RyZGZVdXFvRTNHSDUwZE1YWHRZcm5pdz0=";

                // Act
                var response = await _client.GetAsync($"/api/Authentication/UserTokens/{code}");

                //Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
