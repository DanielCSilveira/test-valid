using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Model
{
    public class Order
    {
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }

        [Required]
        [Column(TypeName = "numeric(12, 2)")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "PENDING";

        public DateTime CreatedAt { get; set; }
    }
}
