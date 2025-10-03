using Application.DTO;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.controller
{
    /// <summary>
    /// Controller responsável pela gestão de Pedidos (Orders).
    /// Requer autenticação JWT/OIDC.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        /// <summary>
        /// Construtor que recebe a injeção do serviço de Pedidos.
        /// </summary>
        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Cria um novo Pedido e publica a mensagem de criação na fila.
        /// </summary>
        /// <param name="orderDto">DTO contendo CustomerId e Amount.</param>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<OrderDto>> Create([FromBody] OrderCreateDto orderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newOrder = await _orderService.CreateAndPublishAsync(orderDto);

            return CreatedAtAction(nameof(GetById), new { id = newOrder.Id }, newOrder);
        }

        /// <summary>
        /// Obtém a lista completa de pedidos.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll()
        {
            var orders = await _orderService.GetAllAsync();
            return Ok(orders);
        }

        /// <summary>
        /// Obtém um pedido pelo ID.
        /// </summary>
        /// <param name="id">O ID do pedido.</param>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDto>> GetById(Guid id)
        {
            var order = await _orderService.GetByIdAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        /// <summary>
        /// Atualiza o Status do Pedido via Stored Procedure do PostgreSQL.
        /// </summary>
        /// <param name="id">O ID do pedido.</param>
        /// <param name="statusDto">O DTO contendo o novo status.</param>
        [HttpPut("{id:guid}/status")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] OrderStatusUpdateDto statusDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _orderService.UpdateStatusByProcedureAsync(id, statusDto.NewStatus);

            if (!success)
            {
                return NotFound($"Pedido com ID {id} não encontrado ou a operação falhou.");
            }

            return NoContent(); 
        }

  
    }
}
