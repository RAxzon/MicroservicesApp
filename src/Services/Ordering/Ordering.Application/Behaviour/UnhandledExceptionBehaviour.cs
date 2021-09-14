using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Ordering.Application.Behaviour
{
    public class UnhandledExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {

        private readonly ILogger<TRequest> _logger;
        
        public UnhandledExceptionBehaviour(ILogger<TRequest> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            try
            {
                return await next();
            }
            catch (Exception ex)
            {
                var requestName = typeof(TRequest).Name;
                _logger.LogError(ex, "Application Request: Unhandled exception for request {Name} {@Request}", requestName, request);
                throw;
            }
        }
    }
}
