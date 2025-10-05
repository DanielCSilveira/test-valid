using System.ComponentModel.DataAnnotations;

namespace Application.DTO
{

    public class CustomerDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; } // Exemplo de renomeação de campo
    }


    public class CustomerCreateOrUpdateDto
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [MinLength(2, ErrorMessage = "O nome não pode ser vazio.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de email inválido.")]
        [StringLength(100, ErrorMessage = "O email deve ter no máximo 100 caracteres.")]
        public required string Email { get; set; }
    }

    public class CustomerUpdateActiveDto : CustomerCreateOrUpdateDto
    {
        public bool Active { get; set; } = true;
    }
}
