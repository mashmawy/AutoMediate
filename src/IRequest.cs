namespace AutoMediate
{
    /// <summary>
    /// Marker interface for requests that return a response
    /// </summary>
    /// <typeparam name="TResponse">The type of response</typeparam>
    public interface IRequest<out TResponse>
    {
    }


    /// <summary>
    /// Marker interface for requests that don't return a response
    /// </summary>
    public interface IRequest : IRequest<Unit>
    {
    }
}
