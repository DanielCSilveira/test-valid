using Application.DTO;
using AutoMapper;
using Infra.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;
        private readonly IRabbitService _rabbit;
        public OrderService(IOrderRepository orderRepository,
            IMapper mapper,
            IRabbitService rabbit)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
            _rabbit = rabbit;
        }
        public async Task<OrderDto> CreateAndPublishAsync(OrderCreateDto orderDto)
        {
            var id = await _orderRepository.Insert(new Infra.Model.Order { Amount = orderDto.Amount, Status = "NEW", CustomerId = orderDto.CustomerId });


            var inserted = await GetByIdAsync(id);
            if (inserted == null) {
                throw new Exception("Ocorreu um erro durante a inserção do pedido");
            }

            await _rabbit.PublishAsync("new_orders", new { id = id });

            return inserted;
        }

        public async Task<IEnumerable<OrderDto>> GetAllAsync()
        {
            var result = await _orderRepository.GetAll();
            return _mapper.Map<IEnumerable<OrderDto>>(result);

        }

        public async Task<OrderDto?> GetByIdAsync(Guid id)
        {
            var result = await _orderRepository.Get(id);
            return _mapper.Map<OrderDto>(result); 
        }

        public async Task<bool> UpdateStatusAsync(Guid id, string newStatus)
        {
            var result =await _orderRepository.UpdateStatusAsync(id, newStatus);
            return true;
        }
    }
}
