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
    // This needs a request and a response, and must be named in the same way as the query.
    // This class will be triggered when a request goes through the GetOrdersListQuery
    public class GetOrdersListByUsernameQueryHandler : IRequestHandler<GetOrdersListByUsernameQuery, List<OrdersVm>>
    {

        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public GetOrdersListByUsernameQueryHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<OrdersVm>> Handle(GetOrdersListByUsernameQuery request, 
            CancellationToken cancellationToken)
        {
            var orderList = await _orderRepository.GetOrderByUserName(request.UserName);
            return _mapper.Map<List<OrdersVm>>(orderList);
        }
    }
}
