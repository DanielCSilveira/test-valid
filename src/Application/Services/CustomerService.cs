

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
        }
        public async Task<Guid> CreateAsync(CustomerCreateOrUpdateDto customerDto)
        {
            //check e-mail is used
            var customer = await _customerRepository.GetByMail(customerDto.Email);
            if (customer != null) {
                if (customer.Active)
                { throw new Exception("E-mail já em uso."); 
                }

                else
                {
                    throw new Exception("E-mail já em uso por um usuário inativado.");
                }
            }

            return await _customerRepository.Insert(new Customer() { Email = customerDto.Email, Name = customerDto.Name });
        }

        public async Task<IEnumerable<CustomerDto>> GetAllAsync()
        {
            var result = await _customerRepository.GetAll();
            return _mapper.Map<IEnumerable<CustomerDto>>(result);
        }

        public async Task<CustomerDto?> GetByIdAsync(Guid id)
        {
            var result = await _customerRepository.Get(id);
            return _mapper.Map<CustomerDto>(result);
        }

        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            var customer = await _customerRepository.Get(id);
            if (customer == null) {
                throw new Exception("Cliente não localizado");
            }
            customer.Active = false;
            return await _customerRepository.Update(customer) > 0;
        }

        public async Task<bool> UpdateAsync(Guid id, CustomerCreateOrUpdateDto customerDto)
        {
            var customer = await _customerRepository.Get(id);
            if (customer == null)
            {
                throw new Exception("Cliente não localizado");
            }

            customer.Name = customerDto.Name;
            customer.Email = customerDto.Email;

            return await _customerRepository.Update(customer) > 0;
        }
    }
}
