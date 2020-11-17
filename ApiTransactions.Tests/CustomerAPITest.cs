﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Assert = Xunit.Assert;

namespace ApiTransactions.Tests
{
    [TestClass]
    public class CustomerAPITest
    {
        private readonly HttpClient _client;

        public CustomerAPITest()
        {
            var server = new TestServer(new WebHostBuilder()
                .UseEnvironment("Development"));
            _client = server.CreateClient();
        }

        [Theory]
        [InlineData("GET")]
        public async Task CustomerGetAllTestAsync(string method)
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod(method), "/api/Customer/");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("GET", 1)]
        public async Task CustomerGetTestAsync(string method, int? id = null)
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod(method), $"/api/Customer/{id}");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

}