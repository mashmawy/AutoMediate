using System;
using System.Threading;
using System.Threading.Tasks;

namespace AutoMediate
{
    /// <summary>
    /// Default implementation of IMediator
    /// </summary>
    internal class Mediator : IMediator
    {
        private readonly IServiceProvider _serviceProvider;

        public Mediator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var requestType = request.GetType();
            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));

            var handler = _serviceProvider.GetService(handlerType);
            if (handler == null)
            {
                throw new InvalidOperationException($"No handler registered for request type {requestType.Name}");
            }

            var handleMethod = handlerType.GetMethod("Handle");
            if (handleMethod == null)
            {
                throw new InvalidOperationException($"Handle method not found on handler for {requestType.Name}");
            }

            var result = handleMethod.Invoke(handler, new object[] { request, cancellationToken });

            if (result is Task<TResponse> taskResult)
            {
                return await taskResult;
            }

            throw new InvalidOperationException($"Handler for {requestType.Name} did not return expected Task<{typeof(TResponse).Name}>");
        }

        public async Task<Unit> Send(IRequest request, CancellationToken cancellationToken = default)
        {
            return await Send<Unit>(request, cancellationToken);
        }
    }
}
