using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Api.IntegrationTests.Helpers;
using Application.DTO;
using Application.Services;
using FluentAssertions;
using Infra.Model;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Api.IntegrationTests
{
    public class OrderIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;
        private readonly IConfiguration _config;
        public OrderIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _factory = factory;
            _config = _factory.Services.GetRequiredService<IConfiguration>();
        }

        [Fact]
        public async Task Post_CreateOrder_returnCreated()
        {
            var mockRabbit = new Mock<IRabbitService>();
            // use a mock Rabbit to validate the message publish
            // create a exclusive factory and client to this test
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
            var token = await JwtTokenHelper.GenerateValidToken(_config);

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            //for test porpoise, get first customer to use
            var responseGet = await client.GetAsync($"/api/customers/");
            var customers = await responseGet.Content.ReadFromJsonAsync<IEnumerable<CustomerDto>>();
            var customer = customers?.First();

            Assert.NotNull(customer);


            var newOrder = new OrderCreateDto() { CustomerId = customer.Id, Amount = 70 };
            var responsePost = await client.PostAsJsonAsync($"/api/orders", newOrder);



            responsePost.StatusCode.Should().Be(HttpStatusCode.Created);
            // Verifica se o PublishAsync foi chamado uma única vez
            mockRabbit.Verify(
                r => r.PublishAsync(It.IsAny<string>(), It.IsAny<object>()),
                Times.Once
            );

        }


        /// <summary>
        /// POC exclusive, publish on rabbitMq, direct to test 
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PublishMessageOnQueue_CreateOrder_returnCreated()
        {
            var token = await JwtTokenHelper.GenerateValidToken(_config);

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            //for test porpoise, get first customer to use
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
            var token = await JwtTokenHelper.GenerateValidToken(_config);

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            //for test porpoise, get first order to set a new status
            var responseGet = await _client.GetAsync($"/api/orders/");
            var orders = await responseGet.Content.ReadFromJsonAsync<IEnumerable<OrderDto>>();
            var order = orders?.First();

            var orderUpdate = new OrderStatusUpdateDto
            {
                NewStatus = order?.Status != "New Status A" ? "New Status A" : "Other Status"
            };

            var responsePut = await _client.PutAsJsonAsync($"/api/orders/{order?.Id}/status", orderUpdate);

            responsePut.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var responseGetUpdatedOrder = await _client.GetAsync($"/api/orders/{order?.Id}");
            var orderUpdated = await responseGetUpdatedOrder.Content.ReadFromJsonAsync<OrderDto>();

            Assert.Equal(orderUpdate.NewStatus, orderUpdated?.Status);

        }

    }
}
