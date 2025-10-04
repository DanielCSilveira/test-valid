using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Api.IntegrationTests.Helpers;
using Application.DTO;
using FluentAssertions;
using Infra.Model;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
                NewStatus = order?.Status != "New Status A" ? "New Status A":"Other Status"
            };

            var responsePut = await _client.PutAsJsonAsync($"/api/orders/{order?.Id}/status", orderUpdate);

            responsePut.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var responseGetUpdatedOrder = await _client.GetAsync($"/api/orders/{order?.Id}");
            var orderUpdated = await responseGetUpdatedOrder.Content.ReadFromJsonAsync<OrderDto>();

            Assert.Equal(orderUpdate.NewStatus, orderUpdated?.Status);

        }

    }
}
