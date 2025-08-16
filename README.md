# AutoMediate

AutoMediate is a lightweight and flexible mediator implementation for .NET applications.  
It helps decouple request/response logic by routing requests to their corresponding handlers through a central mediator.

## Installation

You can install AutoMediate via NuGet (coming soon):

```bash
dotnet add package AutoMediate
```

## Features

- Simple and lightweight mediator pattern implementation.
- Supports request/response messaging (`IRequest<TResponse>`).
- Supports unit (command) messaging (`IRequest`).
- No external dependencies — uses `IServiceProvider` for handler resolution.

## Usage

### Step 1: Define a Request and Response

```csharp
public class PingRequest : IRequest<string>
{
    public string Message { get; set; } = "Ping";
}
```

### Step 2: Implement a Handler

```csharp
public class PingHandler : IRequestHandler<PingRequest, string>
{
    public Task<string> Handle(PingRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult($"Pong: {request.Message}");
    }
}
```

### Step 3: Register Services

```csharp
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddAutoMediate(typeof(PingRequest)); 

var provider = services.BuildServiceProvider();
var mediator = provider.GetRequiredService<IMediator>();
```

### Step 4: Send a Request

```csharp
var response = await mediator.Send(new PingRequest { Message = "Hello" });
Console.WriteLine(response); // Output: Pong: Hello
```

## Unit Test Example (xUnit)

```csharp
public class MediatorTests
{
    [Fact]
    public async Task Mediator_Should_Invoke_Handler_And_Return_Response()
    {
        var services = new ServiceCollection();
        services.AddAutoMediate(typeof(PingHandler)); 
        var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        var result = await mediator.Send(new PingRequest { Message = "Test" });

        Assert.Equal("Pong: Test", result);
    }
}
```

## License

MIT License
