namespace Common.Services;

/// <summary>
/// Email service interface for sending notifications
/// </summary>
public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
    Task SendEmailAsync(List<string> to, string subject, string body, CancellationToken cancellationToken = default);
    Task SendTemplatedEmailAsync(string to, string templateName, object templateData, CancellationToken cancellationToken = default);
    Task SendBulkEmailAsync(List<string> to, string subject, string body, CancellationToken cancellationToken = default);
}