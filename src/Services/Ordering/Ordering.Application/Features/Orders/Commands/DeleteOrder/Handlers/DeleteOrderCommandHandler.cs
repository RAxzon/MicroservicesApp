using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Exceptions;
using Ordering.Application.Features.Orders.Commands.DeleteOrder.CommandRequest;
using Ordering.Application.Models;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Orders.Commands.DeleteOrder.Handlers
{
    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand>
    {

        private readonly IOrderRepository _orderRepository;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly ILogger<DeleteOrderCommandHandler> _logger;

        public DeleteOrderCommandHandler(IOrderRepository orderRepository, IEmailService emailService,
            ILogger<DeleteOrderCommandHandler> logger, IMapper mapper)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper)); ;
        }

        public async Task<Unit> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var orderToDelete = await _orderRepository.GetByIdAsync(request.Id);

            if (orderToDelete == null)
            {
                //_logger.LogError($"Order with id: {request.Id} was not found.");
                throw new NotFoundException(nameof(Order), request);
            }

            try
            {
                _mapper.Map(request, orderToDelete, typeof(DeleteOrderCommand), typeof(Order));

                await _orderRepository.DeleteAsync(orderToDelete);
            }
            catch(Exception ex) 
            {
                throw new Exception($"Error deleting order with id: {orderToDelete.Id}", ex);
            }

            _logger.LogInformation($"Order with id: {orderToDelete.Id} was deleted.");

            //await SendEmail(orderToDelete);

            return Unit.Value;
        }

        private async Task SendEmail(Order order)
        {

            var email = new Email()
            {
                To = "Richiehstad@gmail.com",
                Body = $"Order with id {order.Id} was deleted.",
                Subject = "Order created",
                From = "Richie's Store"
            };

            try
            {
                await _emailService.SendEmail(email);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send email with order: {order.Id}", ex);
                throw;
            }
        }
    }
}
