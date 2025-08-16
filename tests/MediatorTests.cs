using Microsoft.Extensions.DependencyInjection;

namespace AutoMediate.Tests
{
    public class MediatorTests
    {
        private class Ping : IRequest<string>
        {
            public string Message { get; set; }
        }

        private class PingHandler : IRequestHandler<Ping, string>
        {
            public Task<string> Handle(Ping request, CancellationToken cancellationToken)
            {
                return Task.FromResult($"Handled: {request.Message}");
            }
        }
        private class Ping2 : IRequest<string>
        {
            public string Message { get; set; }
        }



        private class VoidPing : IRequest { }

        private class VoidPingHandler : IRequestHandler<VoidPing, Unit>
        {
            public Task<Unit> Handle(VoidPing request, CancellationToken cancellationToken)
            {
                return Task.FromResult(Unit.Value);
            }
        }

        [Fact]
        public async Task Send_Should_Invoke_Handler_And_Return_Result()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddAutoMediate(typeof(PingHandler)); 

            var provider = services.BuildServiceProvider();
            var scope = provider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var request = new Ping { Message = "Hello" };

            // Act
            var result = await mediator.Send(request);

            // Assert
            Assert.Equal("Handled: Hello", result);
        }

        [Fact]
        public async Task Send_Should_Throw_When_Request_Is_Null()
        {
            var services = new ServiceCollection();
            services.AddAutoMediate(typeof(PingHandler));
            var provider = services.BuildServiceProvider();
            var scope = provider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            await Assert.ThrowsAsync<ArgumentNullException>(() => mediator.Send<string>(null!));
        }

        [Fact]
        public async Task Send_Should_Throw_When_Handler_Not_Registered()
        {
            var services = new ServiceCollection();
            services.AddAutoMediate(typeof(PingHandler)); 
            var provider = services.BuildServiceProvider();
            var scope = provider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var request = new Ping2 { Message = "Hello" };

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => mediator.Send(request));
            Assert.Contains("No handler registered", ex.Message);
        }

        [Fact]
        public async Task Send_Should_Work_With_NonGeneric_Request()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddAutoMediate(typeof(PingHandler)); 
            var provider = services.BuildServiceProvider();
            var scope = provider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var request = new VoidPing();

            // Act
            var result = await mediator.Send(request);

            // Assert
            Assert.Equal(Unit.Value, result);
        }
    }
}
