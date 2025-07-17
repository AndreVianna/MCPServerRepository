using Common.Messaging.DependencyInjection;

using Domain.Commands;

using SearchService.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add RabbitMQ messaging
builder.Services.AddRabbitMQMessaging(builder.Configuration);

// Add message consumers
builder.Services.AddMessageConsumer<IndexServerCommandConsumer, IndexServerCommand>();

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