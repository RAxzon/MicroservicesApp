using MediatR;

namespace Ordering.Application.Features.Orders.Commands.DeleteOrder.CommandRequest
{
    public class DeleteOrderCommand : IRequest
    {
        public int Id { get; set; }
    }
}
