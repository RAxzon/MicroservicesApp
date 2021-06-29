using System.Collections.Generic;
using MediatR;

namespace Ordering.Application.Features.Orders.Queries.GetOrdersList.QueryRequests
{
    public class GetAllOrdersQuery : IRequest<IEnumerable<OrdersVm>>
    {
        public GetAllOrdersQuery()
        {
        }
    }
}
