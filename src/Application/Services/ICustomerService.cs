


using Application.DTO;

namespace Application.Service
{
    public interface ICustomerService
    {
        Task<Guid> CreateAsync(CustomerCreateOrUpdateDto customerDto);
        Task<CustomerDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<CustomerDto>> GetAllAsync();
        
        Task<bool> UpdateAsync(Guid id, CustomerCreateOrUpdateDto customerDto);
        
        Task<bool> SoftDeleteAsync(Guid id);
    }
}
