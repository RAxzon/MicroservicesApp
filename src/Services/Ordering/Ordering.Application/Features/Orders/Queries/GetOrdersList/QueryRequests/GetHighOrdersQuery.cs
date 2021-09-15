using System.Collections;
using System.Collections.Generic;
using MediatR;

namespace Ordering.Application.Features.Orders.Queries.GetOrdersList.QueryRequests
{
    public class GetHighOrdersQuery : IRequest<IEnumerable<OrdersVm>>
    {
        public int Price { get; set; }

        public GetHighOrdersQuery(int price)
        {
            Price = price;
        }
    }
}
