# C1-05 Verification

Status DONE for local foundation and prepared CI. GitHub Actions execution NOT VERIFIED (git remote -v empty); owner review and remote run remain required. C1-04 checkpoint eb94b6c, working tree clean immediately after commit. C1-03 checkpoint fcd467d. No business/schema/auth/frontend changes.

## Changes and boundaries
Added tests/OneOffice.Tests targeting net10.0, referencing Api, with Unit and Integration categories. Three product project boundaries preserved; solution now additionally includes test project. Added public partial Program and InternalsVisibleTo only to enable testing. Test-only controller lives in test assembly, never product API. Scaffold script now counts product projects separately.
Added .editorconfig (UTF-8/LF/4spaces,2spaces JSON/YAML), Directory.Build.props nullable/NET analyzers10.0, and GitHub workflow. dotnet format changes limited to formatting of existing C# (newlines/BOM and switch layout); no broad refactor.
Tests discovered correlation header missing on500 because exception handler clears response. Fixed with Response.OnStarting; Development/Production regression tests verify header matches ProblemDetails traceId.
Test build also exposed EF Relational10.0.4/10.0.14 conflict; pinned Infrastructure Microsoft.EntityFrameworkCore.Relational10.0.14. Final build warning-free.

## Versions
xunit2.9.3, xunit.runner.visualstudio3.1.4, Microsoft.NET.Test.Sdk17.14.1, Microsoft.AspNetCore.Mvc.Testing10.0.14, coverlet.collector6.0.4. SDK10.0.403. Package compatibility verified by restore/build/run.

## Commands/results
- dotnet restore OneOffice.slnx --disable-parallel -m:1 PASS.
- dotnet format OneOffice.slnx --no-restore applied controlled formatting.
- dotnet build OneOffice.slnx --no-restore -c Release -m:1 -p:UseSharedCompilation=false PASS,0warnings/0errors11.34s.
- dotnet test OneOffice.slnx --no-build -c Release -m:1 --logger trx --results-directory TestResults PASS18,fail0,skip0. Final artifact TestResults/c1-05.trx (ignored).
- dotnet format OneOffice.slnx --no-restore --verify-no-changes PASS exit0 after sandbox named-pipe restriction resolved by escalation.
- Verify-Scaffold.ps1 PASS.

## Test cases
Unit7: HTTP400/401/403/404/409/429/500 stable code/type/status and no detail.
Integration4: liveness independent and readiness healthy/unhealthy in Development/Production, exact safe body and no-store.
Integration2: OpenAPI Development200/Production404, smoke hooks absent by default.
Integration4: Development+Production thrown500, MVC binding400, missing404; sanitized responses and matching trace/correlation.
Integration1: real DbContext resolution explicitly throws in harness.
Total18. Initial run16pass/2fail caught missing correlation header; fixed and all18passed.

## Isolation and CI
Factory clears configuration sources and forbids real DbContext resolution; health checks use controlled IHealthCheck, not EF InMemory. No connection to oneoffice_dev, no database create/delete/migration. Tests exercise HTTP behavior rather than actual PostgreSQL readiness implementation. PostgreSQL-dependent tests NOT IMPLEMENTED/NOT RUN in C1-05; category PostgreSQL reserved and requires explicitly approved isolated test database.
Workflow .github/workflows/dotnet.yml: push/PR, read-only contents permission, Ubuntu, setup-dotnet10.0.x, restore/buildRelease/format/test excluding PostgreSQL, upload TRX. No product secrets or DB service. Local Windows execution PASS; GitHub/Linux run NOT VERIFIED until remote dispatch. Reference Microsoft guidance https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-10.0

Audit persistence remains DEFERRED TO C1-06. Actual effort untracked vs4h reference. C1-05 changes uncommitted against eb94b6c. Stop for Owner/Architecture Reviewer.

## Pre-push review 06/11/2026
Historical82blobs and49working files scanned against actual local database credentials plus token/private-key patterns. No match to real credentials. Ignore checks PASS for bin/obj/.vs/node_modules/.env/secrets.json/TestResults/.local-dotnet. Synthetic test sentinels retained. Documentation includes local workspace paths, DB/role names and synthetic examples. GitHub authentication failed during read-only ls-remote; remote visibility/refs not independently verified. No push performed; Owner confirmation required.
