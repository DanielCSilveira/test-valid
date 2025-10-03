


using ValidApi.DTO;
using ValidApi.Model;

namespace ValidApi.Service
{
    // Contrato de Serviço para a lógica de negócio de Clientes.
    public interface ICustomerService
    {
        // CRUD Básico.
        Task<Guid> CreateAsync(CustomerCreateOrUpdateDto customerDto);
        Task<Customer?> GetByIdAsync(Guid id);
        Task<IEnumerable<Customer>> GetAllAsync();
        
        // PUT para atualização completa do registro (ou apenas Name/Email).
        Task<bool> UpdateAsync(Guid id, CustomerCreateOrUpdateDto customerDto);
        
        // Soft Delete: Atualiza o campo 'Active' para false.
        Task<bool> SoftDeleteAsync(Guid id);
    }
}
