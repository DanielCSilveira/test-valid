using Application.DTO;
using Application.Services;
using AutoMapper;
using FluentAssertions;
using Infra.Model;
using Infra.Repository;
using Moq;
using Xunit;

namespace Application.Tests
{
    public class CustomerServiceTests
    {
        private readonly Mock<ICustomerRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CustomerService _service;

        public CustomerServiceTests()
        {
            _mockRepository = new Mock<ICustomerRepository>();
            _mockMapper = new Mock<IMapper>();
            _service = new CustomerService(_mockRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task CreateAsync_WithValidData_ReturnsNewGuid()
        {
            var dto = new CustomerCreateOrUpdateDto {Name = "Nome Customer", Email = "customer@mail.com" };
            var expectedId = Guid.NewGuid();
            _mockRepository.Setup(r => r.Insert(It.IsAny<Customer>())).ReturnsAsync(expectedId);

            var result = await _service.CreateAsync(dto);

            result.Should().Be(expectedId);
            _mockRepository.Verify(r => r.Insert(It.Is<Customer>(c => c.Name == dto.Name && c.Email == dto.Email)), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_CallsRepositoryWithCorrectData()
        {
            var dto = new CustomerCreateOrUpdateDto {Name = "Nome Customer", Email = "customer@mail.com"  };
            var expectedId = Guid.NewGuid();
            _mockRepository.Setup(r => r.Insert(It.IsAny<Customer>())).ReturnsAsync(expectedId);

            await _service.CreateAsync(dto);

            _mockRepository.Verify(r => r.Insert(It.Is<Customer>(c => 
                c.Name == "Nome Customer" && c.Email == "customer@mail.com")), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmptyList_WhenNoCustomers()
        {
            _mockRepository.Setup(r => r.GetAll()).ReturnsAsync(new List<Customer>());
            _mockMapper.Setup(m => m.Map<IEnumerable<CustomerDto>>(It.IsAny<IEnumerable<Customer>>()))
                .Returns(new List<CustomerDto>());

            var result = await _service.GetAllAsync();

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllAsync_ReturnsMappedCustomers()
        {
            var customers = new List<Customer>
            {
                new Customer { Id = Guid.NewGuid(), Name = "Nome Customer", Email = "customer@mail.com", Active = true, CreatedAt = DateTime.Now }
            };
            var customerDtos = new List<CustomerDto>
            {
                new CustomerDto { Id = customers[0].Id, Name = "Nome Customer", Email = "customer@mail.com", IsActive = true, CreationDate = DateTime.Now }
            };

            _mockRepository.Setup(r => r.GetAll()).ReturnsAsync(customers);
            _mockMapper.Setup(m => m.Map<IEnumerable<CustomerDto>>(customers)).Returns(customerDtos);

            var result = await _service.GetAllAsync();

            result.Should().HaveCount(1);
            result.Should().BeEquivalentTo(customerDtos);
        }

        [Fact]
        public async Task GetAllAsync_CallsRepositoryGetAll()
        {
            var customers = new List<Customer>();
            _mockRepository.Setup(r => r.GetAll()).ReturnsAsync(customers);
            _mockMapper.Setup(m => m.Map<IEnumerable<CustomerDto>>(It.IsAny<IEnumerable<Customer>>()))
                .Returns(new List<CustomerDto>());

            await _service.GetAllAsync();

            _mockRepository.Verify(r => r.GetAll(), Times.Once);
        }

        [Fact]
        public void GetByIdAsync_ThrowsNotImplementedException()
        {
            var id = Guid.NewGuid();

            Func<Task> act = async () => await _service.GetByIdAsync(id);

            act.Should().ThrowAsync<NotImplementedException>();
        }

        [Fact]
        public void SoftDeleteAsync_ThrowsNotImplementedException()
        {
            var id = Guid.NewGuid();

            Func<Task> act = async () => await _service.SoftDeleteAsync(id);

            act.Should().ThrowAsync<NotImplementedException>();
        }

        [Fact]
        public async Task CreateAsync_WithSpecialCharacters_HandlesCorrectly()
        {
            var dto = new CustomerCreateOrUpdateDto { Name = "José María", Email = "jose@example.com" };
            var expectedId = Guid.NewGuid();
            _mockRepository.Setup(r => r.Insert(It.IsAny<Customer>())).ReturnsAsync(expectedId);

            var result = await _service.CreateAsync(dto);

            result.Should().Be(expectedId);
            _mockRepository.Verify(r => r.Insert(It.Is<Customer>(c => c.Name == "José María")), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_MapsCustomersCorrectly()
        {
            var customerId = Guid.NewGuid();
            var customers = new List<Customer>
            {
                new Customer { Id = customerId, Name = "Test User", Email = "test@example.com", Active = true, CreatedAt = DateTime.Now }
            };
            var expectedDtos = new List<CustomerDto>
            {
                new CustomerDto { Id = customerId, Name = "Test User", Email = "test@example.com", IsActive = true, CreationDate = DateTime.Now }
            };

            _mockRepository.Setup(r => r.GetAll()).ReturnsAsync(customers);
            _mockMapper.Setup(m => m.Map<IEnumerable<CustomerDto>>(customers)).Returns(expectedDtos);

            var result = await _service.GetAllAsync();

            result.Should().ContainSingle();
            var firstCustomer = result.First();
            firstCustomer.Id.Should().Be(customerId);
            firstCustomer.Name.Should().Be("Test User");
        }
    }
}
