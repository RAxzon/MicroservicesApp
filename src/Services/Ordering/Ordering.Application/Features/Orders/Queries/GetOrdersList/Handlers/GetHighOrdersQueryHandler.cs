using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Features.Orders.Queries.GetOrdersList.QueryRequests;

namespace Ordering.Application.Features.Orders.Queries.GetOrdersList.Handlers
{
    public class GetHighOrdersQueryHandler : IRequestHandler<GetHighOrdersQuery, IEnumerable<OrdersVm>>
    {

        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;

        public GetHighOrdersQueryHandler(IMapper mapper, IOrderRepository orderRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }


        public async Task<IEnumerable<OrdersVm>> Handle(GetHighOrdersQuery request, CancellationToken cancellationToken)
        {
            var orders = await _orderRepository.GetHighOrders(request.Price);
            return _mapper.Map<List<OrdersVm>>(orders);
        }
    }
}
