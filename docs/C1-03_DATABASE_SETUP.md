# C1-03 Development database setup

PostgreSQL18 native Windows service is running on127.0.0.1:5433. PostgreSQL17 also runs; never assume5432 is the target. Client18.6, server SQL version unverified until authentication succeeds.

Owner runs in their terminal (password entered at hidden prompt):

```powershell
& 'C:/Program Files/PostgreSQL/18/bin/psql.exe' -X -W -h127.0.0.1 -p5433 -U postgres -d postgres -f scripts/database/Inspect-Development.sql
```

Review inventory before any creation. If role/database exists, stop creation and inspect ownership, memberships and permissions; do not reset existing password or alter database belonging to another application. If both absent and server version18 confirmed, operator executes individually in psql:

```sql
CREATE ROLE oneoffice_app LOGIN NOSUPERUSER NOCREATEDB NOCREATEROLE NOREPLICATION NOBYPASSRLS;
\password oneoffice_app
CREATE DATABASE oneoffice_dev OWNER postgres;
\connect oneoffice_dev
REVOKE ALL ON DATABASE oneoffice_dev FROM PUBLIC;
GRANT CONNECT ON DATABASE oneoffice_dev TO oneoffice_app;
REVOKE CREATE ON SCHEMA public FROM PUBLIC;
GRANT USAGE ON SCHEMA public TO oneoffice_app;
```

Password prompt is private. Runtime role does not own database/schema and has no DDL rights. C1-06 migrations run using authorized owner/deployment credentials, then grant necessary DML to runtime; audit privileges follow design. Never run migrations with postgres as API runtime account. This SQL is prepared, NOT EXECUTED by agent.

Store full connection string via Visual Studio Manage User Secrets for API (UserSecretsId oneoffice-api-development). Enter ConnectionStrings:OneOfficeDb locally with Host127.0.0.1,Port5433,Databaseoneoffice_dev,Usernameoneoffice_app and your private password. Do not paste secret into chat or CLI arguments/history. User Secrets is local development storage, not an encrypted vault.

EF commands use API as startup project and Infrastructure as target, Development environment. Standard API configuration loads User Secrets; no separate factory needed. Use dotnet ef dbcontext info --project OneOffice.Infrastructure --startup-project OneOffice.Api -- --environment Development. This does not prove database authentication. Database SELECT and connection tests require actual credentials.

No foundation schema objects needed. Migration execution DEFERRED TO C1-06. Do not use database update, EnsureCreated or startup Migrate in C1-03.
