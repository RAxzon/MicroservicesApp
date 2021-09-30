using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Features.Orders.Commands.CheckoutOrder.CommandRequests;
using Ordering.Application.Features.Orders.Commands.DeleteOrder.CommandRequest;
using Ordering.Application.Features.Orders.Commands.UpdateOrder.CommandRequests;
using Ordering.Application.Features.Orders.Queries.GetOrdersList;
using Ordering.Application.Features.Orders.Queries.GetOrdersList.QueryRequests;

namespace Ordering.API.Controllers
{
    [ApiController]
    [Route(("api/v1/[controller]"))]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IMediator mediator, ILogger<OrderController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet(Name = "GetOrders")]
        [ProducesResponseType(typeof(IEnumerable<OrdersVm>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<OrdersVm>>> GetAllOrders()
        {
            try
            {
                var query = new GetAllOrdersQuery();
                var orders = await _mediator.Send(query);

                if (orders == null)
                {
                    return NotFound();
                }

                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting orders", ex);
                throw new Exception($"Error getting orders", ex);
            }
        }

        [HttpGet("{id}/id", Name = "GetOrderById")]
        [ProducesResponseType(typeof(IEnumerable<OrdersVm>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<OrdersVm>>> GetOrderById(int id)
        {
            try
            {
                var query = new GetOrderByIdQuery(id);
                var orders = await _mediator.Send(query);

                if (orders == null)
                {
                    return NotFound();
                }

                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting orders", ex);
                throw new Exception($"Error getting orders", ex);
            }
        }

        [HttpGet("{userName}/userName", Name = "GetOrderByUserName")]
        [ProducesResponseType(typeof(IEnumerable<OrdersVm>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<OrdersVm>>> GetOrdersByUserName(string userName)
        {
            try
            {
                var query = new GetOrdersListByUsernameQuery(userName);
                var orders = await _mediator.Send(query);

                if (!orders.Any())
                {
                    return NotFound();
                }

                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting orders", ex);
                throw new Exception($"Error getting orders", ex);
            }
        }

        [HttpGet("{price}/price", Name = "GetHighOrders")]
        [ProducesResponseType(typeof(IEnumerable<OrdersVm>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<OrdersVm>>> GetHighOrders(int price)
        {
            try
            {
                var query = new GetHighOrdersQuery(price);
                var orders = await _mediator.Send(query);

                if (!orders.Any())
                {
                    return NotFound();
                }

                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting orders", ex);
                throw new Exception($"Error getting orders", ex);
            }
        }

        // For Testing
        [HttpPost(Name = "CheckoutOrder")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<int>> CheckoutOrder([FromBody] CheckoutOrderCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut(Name = "UpdateOrder")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<ActionResult> UpdateOrder([FromBody] UpdateOrderCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "DeleteOrder")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> DeleteOrder(int id)
        {
            var command = new DeleteOrderCommand() {Id = id};
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
