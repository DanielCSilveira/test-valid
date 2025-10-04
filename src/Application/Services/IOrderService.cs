using Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IOrderService
    {
        /// <summary>
        /// Cria um novo pedido e publica um evento de mensageria na fila.
        /// </summary>
        /// <param name="orderDto">Dados para a criação do pedido.</param>
        /// <returns>O DTO de resposta do pedido criado.</returns>
        Task<OrderDto> CreateAndPublishAsync(OrderCreateDto orderDto);

        /// <summary>
        /// Obtém a lista de todos os pedidos.
        /// </summary>
        /// <returns>Lista de pedidos.</returns>
        Task<IEnumerable<OrderDto>> GetAllAsync();

        /// <summary>
        /// Obtém um pedido pelo ID.
        /// </summary>
        /// <param name="id">ID do pedido.</param>
        /// <returns>O DTO de resposta do pedido ou null.</returns>
        Task<OrderDto?> GetByIdAsync(Guid id);

        /// <summary>
        /// Atualiza o status de um pedido através de uma Stored Procedure no PostgreSQL.
        /// </summary>
        /// <param name="id">ID do pedido.</param>
        /// <param name="newStatus">O novo status a ser aplicado.</param>
        /// <returns>True se o status foi atualizado com sucesso.</returns>
        Task<bool> UpdateStatusAsync(Guid id, string newStatus);

   
    }
}
