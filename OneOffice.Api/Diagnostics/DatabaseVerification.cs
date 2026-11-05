using Microsoft.EntityFrameworkCore;
using Npgsql;
using OneOffice.Infrastructure.Persistence;

namespace OneOffice.Api.Diagnostics;

internal static class DatabaseVerification
{
    public static async Task<int> RunAsync(IServiceProvider services)
    {
        try
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<OneOfficeDbContext>();
            var connection = context.Database.GetDbConnection();
            var settings = new NpgsqlConnectionStringBuilder(connection.ConnectionString);
            if (settings.Host != "127.0.0.1" || settings.Port != 5433 ||
                settings.Database != "oneoffice_dev" || settings.Username != "oneoffice_app")
                throw new InvalidOperationException("Unexpected verification target.");

            await context.Database.OpenConnectionAsync();
            await using var transaction = await connection.BeginTransactionAsync();
            await using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = "SET TRANSACTION READ ONLY";
            await command.ExecuteNonQueryAsync();
            command.CommandText = """
                SELECT version(), current_database(), current_user, inet_server_port(),
                    (SELECT pg_get_userbyid(datdba) FROM pg_database WHERE datname=current_database()),
                    (SELECT count(*) FROM pg_class c JOIN pg_namespace n ON n.oid=c.relnamespace
                     WHERE n.nspname NOT IN ('pg_catalog','information_schema')
                       AND n.nspname NOT LIKE 'pg_toast%' AND c.relkind IN ('r','p','v','m','f')),
                    (SELECT count(*) FROM pg_roles WHERE rolname=current_user
                       AND (rolsuper OR rolcreatedb OR rolcreaterole OR rolreplication OR rolbypassrls)),
                    has_database_privilege(current_user,current_database(),'CONNECT'),
                    has_schema_privilege(current_user,'public','USAGE'),
                    has_schema_privilege(current_user,'public','CREATE'),
                    current_setting('server_version_num')::int
                """;
            await using var reader = await command.ExecuteReaderAsync();
            await reader.ReadAsync();
            Console.WriteLine($"Server: {reader.GetString(0)}");
            Console.WriteLine($"Database: {reader.GetString(1)}; Role: {reader.GetString(2)}; Port: {reader.GetInt32(3)}; Owner: {reader.GetString(4)}");
            var tables = reader.GetInt64(5);
            Console.WriteLine($"User relations: {tables}; Unsafe role flags: {reader.GetInt64(6)}; CONNECT: {reader.GetBoolean(7)}; USAGE: {reader.GetBoolean(8)}; CREATE: {reader.GetBoolean(9)}");
            var valid = tables == 0 && reader.GetInt64(6) == 0 && reader.GetBoolean(7)
                && reader.GetBoolean(8) && !reader.GetBoolean(9)
                && reader.GetInt32(10) / 10000 == 18 && reader.GetString(4) == "postgres";
            await reader.DisposeAsync();
            await transaction.RollbackAsync();
            Console.WriteLine(valid ? "PASS: EF connection and empty development schema." : "FAIL: Review database invariants.");
            return valid ? 0 : 1;
        }
        catch (Exception)
        {
            Console.Error.WriteLine("Database verification failed. Check private configuration, credentials and server availability; details suppressed to protect secrets.");
            return 1;
        }
    }
}
