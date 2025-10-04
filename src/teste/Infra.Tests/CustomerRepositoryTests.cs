using FluentAssertions;
using Infra.Model;
using Infra.Repository;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Infra.Tests
{
    public class CustomerRepositoryTests
    {
        [Fact]
        public void Constructor_WithNullConnectionString_ThrowsException()
        {
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c.GetSection(It.IsAny<string>())).Returns(Mock.Of<IConfigurationSection>());

            Action act = () => new CustomerRepository(mockConfig.Object);

            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*PostgresConnection*");
        }

  

    }
}
