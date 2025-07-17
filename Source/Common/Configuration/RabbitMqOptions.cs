namespace Common.Configuration;

public class RabbitMqOptions {
    public const string SectionName = "RabbitMQ";

    public string ConnectionString { get; set; } = string.Empty;
    public string VirtualHost { get; set; } = "/";
    public int Port { get; set; } = 5672;
    public string Username { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public bool EnableSsl { get; set; } = false;
    public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(30);
    public int MaxRetryCount { get; set; } = 3;
    public TimeSpan HealthCheckTimeout { get; set; } = TimeSpan.FromSeconds(5);
}