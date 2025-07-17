using Common.Messaging.DependencyInjection;
using PublicApi.Consumers;
using Domain.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add RabbitMQ messaging
builder.Services.AddRabbitMQMessaging(builder.Configuration);

// Add message consumers
builder.Services.AddMessageConsumer<ServerRegisteredEventConsumer, ServerRegisteredEvent>();

// Add logging
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();