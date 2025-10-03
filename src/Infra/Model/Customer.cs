using System.ComponentModel.DataAnnotations;

namespace Infra.Model
{
    public class Customer
    {
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }
        
        [Required]
        [StringLength(100)]
        public required string Email { get; set; }
        
        public bool Active { get; set; } = true;
        
        public DateTime CreatedAt { get; set; }
    }
}
