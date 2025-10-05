using Api.Controllers;
using Application.DTO;
using Application.Service;
using FluentAssertions;
using Infra.Repository;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Api.Tests
{
    public class CustomersControllerTests
    {
        private readonly Mock<ICustomerService> _mockCustomerService;
        private readonly Mock<ICustomerRepository> _mockCustomerRepository;
        private readonly CustomersController _controller;

        public CustomersControllerTests()
        {
            _mockCustomerService = new Mock<ICustomerService>();
            _mockCustomerRepository = new Mock<ICustomerRepository>();
            _controller = new CustomersController(_mockCustomerService.Object);
        }

        [Fact]
        public async Task Post_WithValidCustomer_ReturnsCreatedAtAction()
        {

            var customerDto = new CustomerCreateOrUpdateDto { Name = "Nome Customer", Email = "customer@mail.com" };
            var expectedId = Guid.NewGuid();
            _mockCustomerService.Setup(s => s.CreateAsync(customerDto)).ReturnsAsync(expectedId);

            var result = await _controller.Post(customerDto);

            var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.ActionName.Should().Be(nameof(CustomersController.GetById));
        }

        [Fact]
        public async Task Post_WithInvalidModelState_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("Name", "Required");
            var customerDto = new CustomerCreateOrUpdateDto { Name = "Nome Customer", Email = "customer@mail.com" };

            var result = await _controller.Post(customerDto);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetAll_ReturnsOkWithCustomersList()
        {
            var customers = new List<CustomerDto>
            {
                new CustomerDto { Id = Guid.NewGuid(), Name = "Nome Customer", Email = "customer@mail.com", IsActive = true, CreationDate = DateTime.Now }
            };
            _mockCustomerService.Setup(s => s.GetAllAsync()).ReturnsAsync(customers);

            var result = await _controller.GetAll();

            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(customers);
        }

        [Fact]
        public async Task GetById_WithExistingId_ReturnsOkWithCustomer()
        {
            var customerId = Guid.NewGuid();
            var customer = new CustomerDto { Id = customerId, Name = "Nome Customer", Email = "customer@testm.com", IsActive = true, CreationDate = DateTime.Now };
            _mockCustomerService.Setup(s => s.GetByIdAsync(customerId)).ReturnsAsync(customer);

            var result = await _controller.GetById(customerId);

            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(customer);
        }

        [Fact]
        public async Task GetById_WithNonExistingId_ReturnsNotFound()
        {
            var customerId = Guid.NewGuid();
            _mockCustomerService.Setup(s => s.GetByIdAsync(customerId)).ReturnsAsync((CustomerDto?)null);

            var result = await _controller.GetById(customerId);

            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetById_WithInactiveCustomer_ReturnsNotFound()
        {
            var customerId = Guid.NewGuid();
            var customer = new CustomerDto { Id = customerId, Name = "Nome Customer", Email = "customer@testm.com", IsActive = false, CreationDate = DateTime.Now };
            _mockCustomerService.Setup(s => s.GetByIdAsync(customerId)).ReturnsAsync(customer);

            var result = await _controller.GetById(customerId);

            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Put_WithValidData_ReturnsNoContent()
        {
            var customerId = Guid.NewGuid();
            var customerDto = new CustomerCreateOrUpdateDto { Name = "Nome Customer Updated", Email = "customer@testm.com" };
            _mockCustomerService.Setup(s => s.UpdateAsync(customerId, customerDto)).ReturnsAsync(true);

            var result = await _controller.Put(customerId, customerDto);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Put_WithNonExistingId_ReturnsNotFound()
        {
            var customerId = Guid.NewGuid();
            var customerDto = new CustomerCreateOrUpdateDto { Name = "Nome Customer", Email = "customer@testm.com" };
            _mockCustomerService.Setup(s => s.UpdateAsync(customerId, customerDto)).ReturnsAsync(false);

            var result = await _controller.Put(customerId, customerDto);

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Delete_WithExistingId_ReturnsNoContent()
        {
            var customerId = Guid.NewGuid();
            _mockCustomerService.Setup(s => s.SoftDeleteAsync(customerId)).ReturnsAsync(true);

            var result = await _controller.Delete(customerId);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_WithNonExistingId_ReturnsNotFound()
        {
            var customerId = Guid.NewGuid();
            _mockCustomerService.Setup(s => s.SoftDeleteAsync(customerId)).ReturnsAsync(false);

            var result = await _controller.Delete(customerId);

            result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}
