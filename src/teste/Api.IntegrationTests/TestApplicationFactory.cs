using System;
using System.Linq;
using System.Security.Claims;
using Application.Services;
using Infra;
using Infra.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Api.IntegrationTests
{
    // Handler para mockar a autenticação
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string AuthenticationScheme = "TestScheme";

        public TestAuthHandler(
            Microsoft.Extensions.Options.IOptionsMonitor<AuthenticationSchemeOptions> options,
            Microsoft.Extensions.Logging.ILoggerFactory logger,
            System.Text.Encodings.Web.UrlEncoder encoder,
            Microsoft.AspNetCore.Authentication.ISystemClock clock)
            : base(options, logger, encoder, clock) { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Validação Mínima: Garante que o cabeçalho 'Authorization' existe.
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return Task.FromResult(AuthenticateResult.Fail("Missing Authorization Header."));
            }

            // Injeção da Identidade (Mock de Autenticação)
            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, "911e0ccd-b673-4d3d-8c00-56efdf57db51"),
                new Claim(ClaimTypes.Name, "teste"),
                new Claim(ClaimTypes.Role, "admin")
            };

            var identity = new ClaimsIdentity(claims, AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, AuthenticationScheme);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }

    public class TestApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly string _databaseName = "SharedIntegrationTestDb";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                var authDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IAuthenticationService));
                if (authDescriptor != null) services.Remove(authDescriptor);

                services.AddAuthentication(TestAuthHandler.AuthenticationScheme)
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                        TestAuthHandler.AuthenticationScheme, options => { });


                var dbContextDescriptors = services.Where(d =>
                    d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                    d.ServiceType == typeof(AppDbContext)).ToList();

                foreach (var descriptor in dbContextDescriptors)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase(_databaseName);
                });

                var rabbitServiceDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(Application.Services.IRabbitService));
                if (rabbitServiceDescriptor != null)
                {
                    services.Remove(rabbitServiceDescriptor);
                }

                services.AddSingleton(new Mock<Application.Services.IRabbitService>().Object);
            });

            builder.ConfigureServices(services =>
            {
                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<AppDbContext>();

                    db.Database.EnsureCreated();

                    if (!db.Customers.Any())
                    {
                        var seedCustomerId = Guid.NewGuid();
                        var seedOrderId = Guid.NewGuid();

                        db.Customers.Add(new Infra.Model.Customer
                        {
                            Id = seedCustomerId,
                            Name = "Seeded Mock Customer",
                            Email = "seeded@mock.com",
                            Active = true,
                            CreatedAt = DateTime.UtcNow
                        });

                        db.Orders.Add(new Infra.Model.Order
                        {
                            Id = seedOrderId,
                            CustomerId = seedCustomerId,
                            Amount = 99.99m,
                            Status = "INITIAL",
                            CreatedAt = DateTime.UtcNow
                        });

                        db.SaveChanges();
                    }
                }
            });
        }
    }
}
