using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using System.Net.Mime;
using Application.Service;
using Application.DTO;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;


        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        /// <summary>
        /// Cria um novo cliente.
        /// </summary>
        /// <param name="customerDto">Dados do novo cliente.</param>
        /// <returns>O ID do cliente criado.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] CustomerCreateOrUpdateDto customerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newId = await _customerService.CreateAsync(customerDto);

            return CreatedAtAction(nameof(GetById), new { id = newId }, new { id = newId });
        }

        /// <summary>
        /// Obtém a lista de todos os clientes ativos.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAll()
        {
            var customers = await _customerService.GetAllAsync();
            return Ok(customers);
        }

        /// <summary>
        /// Obtém um cliente por ID.
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CustomerDto>> GetById(Guid id)
        {
            var customer = await _customerService.GetByIdAsync(id);

            if (customer == null || !customer.IsActive)
            {
                return NotFound(new { message = $"Cliente com ID {id} não encontrado ou inativo." });
            }

            return Ok(customer);
        }

        /// <summary>
        /// Atualiza os dados de um cliente existente.
        /// </summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put(Guid id, [FromBody] CustomerCreateOrUpdateDto customerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _customerService.UpdateAsync(id, customerDto);

            if (!success)
            {
                return NotFound(new { message = $"Cliente com ID {id} não encontrado para atualização." });
            }

            return NoContent();
        }

        /// <summary>
        /// Realiza o Soft Delete de um cliente, marcando-o como inativo.
        /// </summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _customerService.SoftDeleteAsync(id);

            if (!success)
            {
                return NotFound(new { message = $"Cliente com ID {id} não encontrado para exclusão (soft delete)." });
            }

            return NoContent();
        }
    }
}
