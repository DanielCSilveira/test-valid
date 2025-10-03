

using Application.DTO;
using Application.Service;
using AutoMapper;
using Infra.Model;
using Infra.Repository;

namespace Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        public CustomerService(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _mapper = mapper;
        }
        public async Task<Guid> CreateAsync(CustomerCreateOrUpdateDto customerDto)
        {
            return await _customerRepository.Insert(new Customer() { Email = customerDto.Email, Name = customerDto.Name });
        }

        public async Task<IEnumerable<CustomerDto>> GetAllAsync()
        {
            var result = await _customerRepository.GetAll();
            return _mapper.Map<IEnumerable<CustomerDto>>(result);
        }

        public Task<CustomerDto?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SoftDeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(Guid id, CustomerCreateOrUpdateDto customerDto)
        {
            throw new NotImplementedException();
        }
    }
}
