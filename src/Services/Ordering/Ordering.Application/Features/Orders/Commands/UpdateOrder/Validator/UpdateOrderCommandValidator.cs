using FluentValidation;
using Ordering.Application.Features.Orders.Commands.UpdateOrder.CommandRequests;

namespace Ordering.Application.Features.Orders.Commands.UpdateOrder.Validator
{
    public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
    {
        public UpdateOrderCommandValidator()
        {
            RuleFor(p => p.UserName)
                .NotEmpty().WithMessage($"Username is required")
                .NotNull()
                .MaximumLength(50).WithMessage($"Username can not contain more than 50 characters.");

            RuleFor(p => p.EmailAddress)
                .NotEmpty().WithMessage($"Email address must be provided");

            RuleFor(p => p.TotalPrice)
                .NotEmpty().WithMessage($"TotalPrice is required.")
                .GreaterThan(0).WithMessage($"TotalPrice must be greater than 0");
        }
    }
}
