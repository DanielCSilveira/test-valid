using Application.Services;
using Infra;
using Infra.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Api.IntegrationTests.Mocks
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        public Mock<IRabbitService> MockRabbitService { get; }
        public Mock<ICustomerRepository> MockCustomerRepository { get; }
        public Mock<IOrderRepository> MockOrderRepository { get; }

        public CustomWebApplicationFactory()
        {
            MockRabbitService = new Mock<IRabbitService>();
            MockCustomerRepository = new Mock<ICustomerRepository>();
            MockOrderRepository = new Mock<IOrderRepository>();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (dbContextDescriptor != null)
                {
                    services.Remove(dbContextDescriptor);
                }

                var dbContextServiceDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(AppDbContext));
                if (dbContextServiceDescriptor != null)
                {
                    services.Remove(dbContextServiceDescriptor);
                }

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                var rabbitDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IRabbitService));
                if (rabbitDescriptor != null)
                {
                    services.Remove(rabbitDescriptor);
                }
                services.AddSingleton(MockRabbitService.Object);

                var sp = services.BuildServiceProvider();

                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<AppDbContext>();

                db.Database.EnsureCreated();
            });
        }
    }
}
