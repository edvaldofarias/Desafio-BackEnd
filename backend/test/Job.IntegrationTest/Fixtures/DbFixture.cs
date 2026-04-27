using Job.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Job.IntegrationTest.Fixtures;

public sealed class DbFixture : IAsyncLifetime
{
    private readonly string _databaseName = $"job_it_{Guid.NewGuid():N}";
    private string? _adminConnectionString;

    public string ConnectionString { get; private set; } = string.Empty;
    public bool IsAvailable { get; private set; }
    public string? UnavailableReason { get; private set; }

    public async Task InitializeAsync()
    {
        var host = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost";
        var port = Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432";
        var user = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres";
        var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "postgres";

        _adminConnectionString = $"Server={host};Port={port};User Id={user};Password={password};Database=postgres;";
        ConnectionString = $"Server={host};Port={port};User Id={user};Password={password};Database={_databaseName};";

        try
        {
            await using var probe = new NpgsqlConnection(_adminConnectionString);
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            await probe.OpenAsync(cts.Token);

            await using (var create = new NpgsqlCommand($"CREATE DATABASE \"{_databaseName}\";", probe))
            {
                await create.ExecuteNonQueryAsync(cts.Token);
            }
        }
        catch (Exception ex)
        {
            IsAvailable = false;
            UnavailableReason = $"Postgres indisponível em {host}:{port} ({ex.GetType().Name}: {ex.Message})";
            return;
        }

        var options = new DbContextOptionsBuilder<JobContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        await using var context = new JobContext(options);
        await context.Database.MigrateAsync();

        IsAvailable = true;
    }

    public async Task DisposeAsync()
    {
        if (!IsAvailable || _adminConnectionString is null)
            return;

        try
        {
            await using var conn = new NpgsqlConnection(_adminConnectionString);
            await conn.OpenAsync();

            await using (var terminate = new NpgsqlCommand(
                "SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE datname = @db AND pid <> pg_backend_pid();",
                conn))
            {
                terminate.Parameters.AddWithValue("db", _databaseName);
                await terminate.ExecuteNonQueryAsync();
            }

            await using var drop = new NpgsqlCommand($"DROP DATABASE IF EXISTS \"{_databaseName}\";", conn);
            await drop.ExecuteNonQueryAsync();
        }
        catch
        {
            // best effort cleanup
        }
    }
}
