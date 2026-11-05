# C1-04 Verification

DONE within ticket scope. Audit persistence DEFERRED TO C1-06.
C1-03 checkpoint fcd467d; source review and credential/build-output scan PASS; working tree clean immediately after checkpoint. Production --verify-database refused before DB execution. Existing diagnostic scoped to Development, explicit local target, SELECT/read-only transaction.

## Design
Correlation middleware generates trace ID and X-Correlation-ID → exception handler → status code pages → health/HTTPS → authorization/controllers. ProblemDetails returns status,type,title,code,traceId without stack/raw messages. Invalid binding400 request.invalid; MVC validation400 validation.failed; unexpected500 server.unexpected; missing404 resource.not_found. Business error mappings belong to later use cases.
JSON structured logs allow only OneOffice.Api.Foundation categories with status/duration/trace ID. Sensitive request/exception values are omitted, not regex-rewritten. Framework/EF raw diagnostics suppressed; future operational diagnostic expansion needs safe policy review.
GET /health liveness200; /health/ready database connectivity200 or503. Responses only healthy/unavailable, Cache-Control no-store. PostgreSQL check resolves scoped DbContext and has5second timeout.
OpenAPI only Development. Explicit --foundation-smoke enables deterministic error/binding routes only in Development, never Production. This test hook is an engineering choice to test actual middleware without another project.

## Durable audit contract
Core SharedKernel/Auditing has AuditEvent and IAuditWriter.StageAsync. No EF dependency. C1-06 scoped implementation MUST receive same OneOfficeDbContext as mutation repositories, stage sanitized AuditLog without SaveChanges/Commit, then application unit of work saves mutation+audit in one transaction. Audit failure rolls back mutation. Standalone failed security attempts use separate explicit transaction. No writer implementation registered yet, resolution fails rather than silently discarding audit. No schema/migration. Audit persistence/E2E NOT TESTED, DEFERRED TO C1-06. Changes/Metadata allowlisted snapshots; forbid raw request/entity/secret. Retention remains pending.

## Evidence
Restore PASS. Final build PASS0warnings/0errors4.11s. Commands dotnet restore OneOffice.slnx --disable-parallel -m:1; dotnet build OneOffice.slnx --no-restore -m:1 -p:UseSharedCompilation=false.
Initial WriteAsJsonAsync overload errors corrected with explicit serializer options.
Verify-Foundation.ps1 real Kestrel loopback5191/5192 PASS: Development health200, readiness200 using real User Secrets, OpenAPI200, thrown500 and binding400 ProblemDetails, missing404. Production invalid loopback DB port1 health200/readiness503, OpenAPI404, smoke route404. Only child API processes stopped; no PostgreSQL service affected.
Synthetic password in exception/query absent from responses and logs; JSON TraceId present. Actual User Secrets password/full connection absent from smoke logs, independently scanned in memory (empty log handling corrected before final PASS).
Production --verify-database refused with requires Development and no server output.
Logs under ignored .local-dotnet/smoke. No mutation/migration/auth/RBAC/frontend. No impact PostgreSQL17 or other databases.
C1-04 changes uncommitted for review against fcd467d. Stop before C1-05/C1-06.
.NET10 diagnostics reference https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling?view=aspnetcore-10.0
