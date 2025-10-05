using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class OrderCreateDto
    {
        [Required(ErrorMessage = "O ID do cliente é obrigatório.")]
        public Guid CustomerId { get; set; }

        [Required(ErrorMessage = "O valor do pedido é obrigatório.")]
        [Range(0.01, 999999999.99, ErrorMessage = "O valor deve ser positivo e válido.")]
        public decimal Amount { get; set; }
    }

    /// <summary>
    /// DTO para a atualização de status do pedido (PUT), que utilizará uma procedure.
    /// </summary>
    public class OrderStatusUpdateDto
    {
        [Required(ErrorMessage = "O novo status é obrigatório.")]
        [StringLength(50, MinimumLength = 3)]
        public string NewStatus { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO de resposta para consulta de Pedidos (GET).
    /// </summary>
    public class OrderDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
