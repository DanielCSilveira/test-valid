using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Api.IntegrationTests.Helpers;
using Api.IntegrationTests.Mocks;
using Application.DTO;
using FluentAssertions;
using Infra.Model;
using Infra;
using Infra.Repository;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Api.IntegrationTests
{
    public class CustomersIntegrationTests : IClassFixture<TestApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly TestApplicationFactory _factory;
        private readonly IConfiguration _config;

        private readonly Mock<ICustomerRepository> _mockCustomeRepository;
        public CustomersIntegrationTests(TestApplicationFactory factory)
        {
            _mockCustomeRepository = RepositoryMocks.CreateCustomerRepositoryMock();
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
        public async Task Get_WithValidAuth_ReturnsAuthorized()
        {

            var token = JwtTokenHelper.GenerateTestToken();

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

            var token = JwtTokenHelper.GenerateTestToken();

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);


            var response = await _client.PostAsJsonAsync("/api/customers", newCustomer);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task Post_DuplicateUser_ReturnsBadRequest()
        {
            var duplicateEmail = $"duplicate@test.com";
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<AppDbContext>();

                db.Customers.Add(new Customer
                {
                    Id = Guid.NewGuid(),
                    Name = "Customer Duplicado Original",
                    Email = duplicateEmail,
                    Active = true,
                    CreatedAt = DateTime.UtcNow
                });
                await db.SaveChangesAsync();
            }
            var newCustomer = new CustomerCreateOrUpdateDto
            {
                Name = $"DuplicateUser",
                Email = duplicateEmail
            };

            var token = JwtTokenHelper.GenerateTestToken();

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);


            var response = await _client.PostAsJsonAsync("/api/customers", newCustomer);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

    }
}
