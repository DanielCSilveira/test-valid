using Infra.Model;
using Infra.Repository;
using Moq;

namespace Api.IntegrationTests.Mocks
{
    public static class RepositoryMocks
    {
        public static Mock<ICustomerRepository> CreateCustomerRepositoryMock()
        {
            var mock = new Mock<ICustomerRepository>();

            var customers = new List<Customer>
            {
                new Customer
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Customer 1",
                    Email = "test1@example.com",
                    Active = true,
                    CreatedAt= DateTime.Now
                },
                new Customer
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Customer 2",
                    Email = "test2@example.com",
                    Active = true,
                    CreatedAt= DateTime.Now
                }
            };

            mock.Setup(r => r.GetAll())
                .ReturnsAsync(customers);

            mock.Setup(r => r.Get(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => customers.FirstOrDefault(c => c.Id == id));

            mock.Setup(r => r.GetByMail(It.IsAny<string>()))
                .ReturnsAsync((string email) => customers.FirstOrDefault(c => c.Email == email));

            mock.Setup(r => r.Insert(It.IsAny<Customer>()))
                .ReturnsAsync((Customer customer) =>
                {
                    customer.Id = Guid.NewGuid();
                    customers.Add(customer);
                    return customer.Id;
                });

            mock.Setup(r => r.Update(It.IsAny<Customer>()))
                .ReturnsAsync(1);



            return mock;
        }

        public static Mock<IOrderRepository> CreateOrderRepositoryMock()
        {
            var mock = new Mock<IOrderRepository>();

            var orders = new List<Order>
            {
                new Order
                {
                    Id = Guid.NewGuid(),
                    CustomerId = Guid.NewGuid(),
                    Amount = 100.50m,
                    Status = "PENDING",
                    CreatedAt = DateTime.UtcNow
                },
                new Order
                {
                    Id = Guid.NewGuid(),
                    CustomerId = Guid.NewGuid(),
                    Amount = 250.75m,
                    Status = "COMPLETED",
                    CreatedAt = DateTime.UtcNow
                }
            };

            mock.Setup(r => r.GetAll())
                .ReturnsAsync(orders);

            mock.Setup(r => r.Get(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => orders.FirstOrDefault(o => o.Id == id));

            mock.Setup(r => r.Insert(It.IsAny<Order>()))
                .ReturnsAsync((Order order) =>
                {
                    order.Id = Guid.NewGuid();
                    orders.Add(order);
                    return order.Id;
                });

            mock.Setup(r => r.Update(It.IsAny<Order>()))
                .ReturnsAsync(1);

            mock.Setup(r => r.UpdateStatusAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync((Guid id, string status) =>
                {
                    var order = orders.FirstOrDefault(o => o.Id == id);
                    if (order != null)
                    {
                        order.Status = status;
                        return 1;
                    }
                    return 0;
                });



            return mock;
        }
    }
}
