using System.Threading;
using System.Threading.Tasks;

namespace AutoMediate
{
    /// <summary>
    /// Handler for requests that return a response
    /// </summary>
    /// <typeparam name="TRequest">The type of request</typeparam>
    /// <typeparam name="TResponse">The type of response</typeparam>
    public interface IRequestHandler<in TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default);
    }




    /// <summary>
    /// Handler for requests that don't return a response
    /// </summary>
    /// <typeparam name="TRequest">The type of request</typeparam>
    public interface IRequestHandler<in TRequest> : IRequestHandler<TRequest, Unit>
        where TRequest : IRequest<Unit>
    {
        new Task<Unit> Handle(TRequest request, CancellationToken cancellationToken = default);

        // Explicit implementation to avoid ambiguity
        Task<Unit> IRequestHandler<TRequest, Unit>.Handle(TRequest request, CancellationToken cancellationToken)
            => Handle(request, cancellationToken);
    }
}
