using System.Diagnostics.CodeAnalysis;

namespace Domain.Messaging;

/// <summary>
/// Interface for event handlers
/// </summary>
[SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Required")]
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "Intended")]
public interface IEventHandler<in TEvent> where TEvent : BaseEvent {
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}