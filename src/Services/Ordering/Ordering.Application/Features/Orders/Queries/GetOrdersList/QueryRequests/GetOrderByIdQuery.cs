using MediatR;

namespace Ordering.Application.Features.Orders.Queries.GetOrdersList.QueryRequests
{
    public class GetOrderByIdQuery : IRequest<OrdersVm>
    {
        public int Id { get; set; }

        public GetOrderByIdQuery(int id)
        {
            Id = id;
        }
    }
}
