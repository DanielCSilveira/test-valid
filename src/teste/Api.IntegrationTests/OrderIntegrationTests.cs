using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Api.IntegrationTests.Helpers;
using Application.DTO;
using Application.Services;
using FluentAssertions;
using Infra.Model;
using Infra.Repository;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Api.IntegrationTests
{
    public class OrderIntegrationTests : IClassFixture<TestApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly TestApplicationFactory _factory;
        private readonly IConfiguration _config;
        public OrderIntegrationTests(TestApplicationFactory factory)
        {
            _client = factory.CreateClient();
            _factory = factory;
            _config = _factory.Services.GetRequiredService<IConfiguration>();
        }

        [Fact]
        public async Task Post_CreateOrder_returnCreated()
        {
            var mockRabbit = new Mock<IRabbitService>();
            var factory = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.Single(
                        d => d.ServiceType == typeof(IRabbitService));
                    services.Remove(descriptor);
                    services.AddSingleton(mockRabbit.Object);
                });
            });

            var client = factory.CreateClient();
            var token = JwtTokenHelper.GenerateTestToken();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var responseGet = await client.GetAsync($"/api/customers/");
            var customers = await responseGet.Content.ReadFromJsonAsync<IEnumerable<CustomerDto>>();
            var customer = customers?.First();

            Assert.NotNull(customer);


            var newOrder = new OrderCreateDto() { CustomerId = customer.Id, Amount = 70 };
            var responsePost = await client.PostAsJsonAsync($"/api/orders", newOrder);



            responsePost.StatusCode.Should().Be(HttpStatusCode.Created);
            mockRabbit.Verify(
                r => r.PublishAsync(It.IsAny<string>(), It.IsAny<object>()),
                Times.Once
            );

        }


      
        [Fact]
        public async Task PublishMessageOnQueue_CreateOrder_returnCreated()
        {
            var token = JwtTokenHelper.GenerateTestToken();

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            
            var responseGet = await _client.GetAsync($"/api/customers/");
            var customers = await responseGet.Content.ReadFromJsonAsync<IEnumerable<CustomerDto>>();
            var customer = customers?.First();

            Assert.NotNull(customer);


            var newOrder = new OrderCreateDto() { CustomerId = customer.Id, Amount = 70 };
            var responsePost = await _client.PostAsJsonAsync($"/api/orders", newOrder);

            responsePost.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task Put_UpdateStatus_ReturnNoContent()
        {
            var token = JwtTokenHelper.GenerateTestToken();
            var testOrderId = Guid.Parse("a5c92c8b-68d7-40e9-b2f7-5c1d6e8b2b7a");
            var newStatus = "NEW_STATUS_A";
            var orderUpdate = new OrderStatusUpdateDto { NewStatus = newStatus };

            var mockOrderRepo = new Mock<IOrderRepository>();

            mockOrderRepo.Setup(r =>
                r.UpdateStatusAsync(testOrderId, newStatus))
                .ReturnsAsync(1);

            var factory = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.Single(d => d.ServiceType == typeof(IOrderRepository));
                    services.Remove(descriptor);
                    services.AddSingleton(mockOrderRepo.Object);
                });
            });

            var client = factory.CreateClient();


            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthHandler.AuthenticationScheme, "TestToken");


            var responsePut = await client.PutAsJsonAsync($"/api/orders/{testOrderId}/status", orderUpdate);


            responsePut.StatusCode.Should().Be(HttpStatusCode.NoContent);


            mockOrderRepo.Verify(r =>
                r.UpdateStatusAsync(testOrderId, newStatus),
                Times.Once);

        }

    }
}
