using System;
using MediatR;

namespace Ordering.Application.Features.Orders.Queries.GetOrdersList.QueryRequests
{
    public class GetOrderQuery : IRequest<OrdersVm>
    {

        public int Id { get; set; }

        public GetOrderQuery(int id)
        {
            Id = id;
        }
    }
}
