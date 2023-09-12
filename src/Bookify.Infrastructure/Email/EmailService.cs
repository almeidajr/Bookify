using Bookify.Application.Abstractions.Email;
using Microsoft.Extensions.Logging;

namespace Bookify.Infrastructure.Email;

internal sealed class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(
        Domain.Users.Email recipient,
        string subject,
        string body,
        CancellationToken cancellationToken = default)
    {
        _logger.LogWarning(
            "Attempted to send email to {Recipient} with subject: {Subject}, but the method is not yet implemented",
            recipient.Value, subject);
        return Task.CompletedTask;
    }
}