using System.Threading;
using System.Threading.Tasks;

namespace AutoMediate
{
    /// <summary>
    /// Mediator interface for sending requests
    /// </summary>
    public interface IMediator
    {
        /// <summary>
        /// Send a request that returns a response
        /// </summary>
        Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Send a request that doesn't return a response
        /// </summary>
        Task<Unit> Send(IRequest request, CancellationToken cancellationToken = default);
    }
}
