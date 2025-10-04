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
        public OrderService(IOrderRepository orderRepository, IMapper mapper)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
        }
        public Task<OrderDto> CreateAndPublishAsync(OrderCreateDto orderDto)
        {
            throw new NotImplementedException();
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
