# Test foundation

OneOffice.Tests targets net10.0 with xUnit and WebApplicationFactory.
- Category=Unit tests pure ProblemDetails mappings.
- Category=Integration tests HTTP pipeline via TestServer, controlled health results, no database.
- Category=PostgreSQL reserved for future isolated database tests. None implemented/run in C1-05. Requires explicit approved disposable test database; never oneoffice_dev. No EF InMemory, automatic database creation/deletion, migration or schema mutation.

Factory clears configuration sources, replaces DbContext resolution with a throwing factory, replaces health registrations, and injects test-only controller from the test assembly. HTTPS test base address avoids redirect. Test controller is never included in published API. Readiness tests verify HTTP policy, not PostgreSQL behavior (real connectivity evidence belongs to C1-03/C1-04).

Commands: dotnet restore OneOffice.slnx; dotnet build OneOffice.slnx -c Release --no-restore; dotnet test OneOffice.slnx -c Release --no-build --filter "Category!=PostgreSQL"; dotnet format OneOffice.slnx --no-restore --verify-no-changes.
CI uses no product secrets and no PostgreSQL service. GitHub execution must be reported separately from local execution.
