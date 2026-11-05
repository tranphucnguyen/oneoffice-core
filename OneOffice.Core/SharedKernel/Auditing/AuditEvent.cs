namespace OneOffice.Core.SharedKernel.Auditing;

// Payloads must be field-allowlisted and sanitized before construction.
public sealed record AuditEvent(
    Guid Id, DateTimeOffset OccurredAt, Guid? ActorUserId, string ActorKind,
    string Action, string? TargetType, Guid? TargetId, string Outcome,
    string? ReasonCode, string CorrelationId,
    IReadOnlyDictionary<string, string?> Changes,
    IReadOnlyDictionary<string, string?> Metadata);

public interface IAuditWriter
{
    // Stage within the caller's scoped unit of work. Never save/commit independently.
    // C1-06 adapter must use the same OneOfficeDbContext and transaction as the mutation.
    ValueTask StageAsync(AuditEvent auditEvent, CancellationToken cancellationToken = default);
}
