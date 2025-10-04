using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Api.IntegrationTests.Helpers;
using Application.DTO;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Api.IntegrationTests
{
    public class CustomersIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;
        private readonly IConfiguration _config;
        public CustomersIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _factory = factory;
            _config = _factory.Services.GetRequiredService<IConfiguration>();
        }

        [Fact]
        public async Task Post_WithoutAuth_ReturnsUnauthorized()
        {
            var newCustomer = new CustomerCreateOrUpdateDto
            {
                Name = "Integration Test Customer",
                Email = "integration@test.com"
            };

            var response = await _client.PostAsJsonAsync("/api/customers", newCustomer);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Post_WithInvalidAuth_ReturnsUnauthorized()
        {
            var newCustomer = new CustomerCreateOrUpdateDto
            {
                Name = "Integration Test Customer",
                Email = "integration@test.com"
            };

            var token = JwtTokenHelper.GenerateInvalidToken();

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);


            var response = await _client.PostAsJsonAsync("/api/customers", newCustomer);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_WithValidAuth_ReturnsAuthorized()
        {

            var token = await JwtTokenHelper.GenerateValidToken(_config);

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);


            var response = await _client.GetAsync("/api/customers");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        [Fact]
        public async Task Post_CreateCustomer_ReturnsCreated()
        {

            var newCustomer = new CustomerCreateOrUpdateDto
            {
                Name = $"Integration Test Customer{Guid.NewGuid()}",
                Email = $"{Guid.NewGuid()}@test.com"
            };

            var token = await JwtTokenHelper.GenerateValidToken(_config);

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);


            var response = await _client.PostAsJsonAsync("/api/customers", newCustomer);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task Post_DuplicateUser_ReturnsBadRequest()
        {

            var newCustomer = new CustomerCreateOrUpdateDto
            {
                Name = $"DuplicateUser",
                Email = $"duplicate@test.com"
            };

            var token = await JwtTokenHelper.GenerateValidToken(_config);

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);


            var response = await _client.PostAsJsonAsync("/api/customers", newCustomer);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

    }
}
