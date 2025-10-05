using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infra.Model
{
    [Table("customers")]
    public class Customer
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("name")]
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [Column("email")]
        [Required]
        [StringLength(100)]
        public required string Email { get; set; }

        [Column("active")]
        public bool Active { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
