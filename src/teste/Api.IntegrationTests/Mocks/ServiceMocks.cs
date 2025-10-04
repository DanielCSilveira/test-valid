using Application.DTO;
using Application.Service;
using Application.Services;
using Moq;

namespace Api.IntegrationTests.Mocks
{
    public static class ServiceMocks
    {
        public static Mock<IRabbitService> CreateRabbitServiceMock()
        {
            var mock = new Mock<IRabbitService>();

            mock.Setup(r => r.PublishAsync(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.CompletedTask);

            return mock;
        }

        public static Mock<ICustomerService> CreateCustomerServiceMock()
        {
            var mock = new Mock<ICustomerService>();

            var testCustomers = new List<CustomerDto>
            {
                new CustomerDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Mock Customer 1",
                    Email = "mock1@example.com",
                    IsActive = true,
                    CreationDate = DateTime.Now
                    
                },
                new CustomerDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Mock Customer 2",
                    Email = "mock2@example.com",
                    IsActive = true,
                    CreationDate = DateTime.Now


                }
            };

            mock.Setup(s => s.GetAllAsync())
                .ReturnsAsync(testCustomers);

            mock.Setup(s => s.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => testCustomers.FirstOrDefault(c => c.Id == id));

            mock.Setup(s => s.CreateAsync(It.IsAny<CustomerCreateOrUpdateDto>()))
                .ReturnsAsync(Guid.NewGuid());

            mock.Setup(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<CustomerCreateOrUpdateDto>()))
                .ReturnsAsync(true);

            mock.Setup(s => s.SoftDeleteAsync(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            return mock;
        }

        public static Mock<IOrderService> CreateOrderServiceMock()
        {
            var mock = new Mock<IOrderService>();

            var testOrders = new List<OrderDto>
            {
                new OrderDto
                {
                    Id = Guid.NewGuid(),
                    CustomerId = Guid.NewGuid(),
                    Amount = 150.00m,
                    Status = "Processing"
                },
                new OrderDto
                {
                    Id = Guid.NewGuid(),
                    CustomerId = Guid.NewGuid(),
                    Amount = 300.00m,
                    Status = "Shipped"
                }
            };

            mock.Setup(s => s.GetAllAsync())
                .ReturnsAsync(testOrders);

            mock.Setup(s => s.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => testOrders.FirstOrDefault(o => o.Id == id));

            mock.Setup(s => s.CreateAndPublishAsync(It.IsAny<OrderCreateDto>()))
                .ReturnsAsync((OrderCreateDto dto) => new OrderDto
                {
                    Id = Guid.NewGuid(),
                    CustomerId = dto.CustomerId,
                    Amount = dto.Amount,
                    Status = "Created"
                });

            mock.Setup(s => s.UpdateStatusAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            return mock;
        }
    }
}
