using MCPHub.Common.Messaging;
using MCPHub.Domain.Messaging;
using MCPHub.SecurityService.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Note: Messaging implementations removed - only interfaces available
// Add minimal stub implementations for messaging
builder.Services.AddScoped<IMessagePublisher>(provider =>
    new StubMessagePublisher(provider.GetRequiredService<ILogger<StubMessagePublisher>>()));

// Register consumer services
builder.Services.AddScoped<ScanServerCommandConsumer>();

// Add logging
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Temporary stub implementation for messaging
public class StubMessagePublisher(ILogger<StubMessagePublisher> logger) : IMessagePublisher {
    private readonly ILogger<StubMessagePublisher> _logger = logger;

    public Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : BaseMessage {
        _logger.LogInformation("Stub: Publishing message of type {MessageType}", typeof(T).Name);
        return Task.CompletedTask;
    }

    public Task PublishAsync<T>(T message, string routingKey, CancellationToken cancellationToken = default) where T : BaseMessage {
        _logger.LogInformation("Stub: Publishing message of type {MessageType} with routing key {RoutingKey}", typeof(T).Name, routingKey);
        return Task.CompletedTask;
    }
}