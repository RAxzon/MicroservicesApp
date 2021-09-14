using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Features.Orders.Queries.GetOrdersList.QueryRequests;

namespace Ordering.Application.Features.Orders.Queries.GetOrdersList.Handlers
{
    public class GetOrderQueryHandler : IRequestHandler<GetOrderByIdQuery, OrdersVm>
    {

        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public GetOrderQueryHandler(IMapper mapper, IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<OrdersVm> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.Id);
            return _mapper.Map<OrdersVm>(order);
        }
    }
}
