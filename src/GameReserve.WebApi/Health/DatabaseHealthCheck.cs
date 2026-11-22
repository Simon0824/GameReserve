using Dapper;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;


namespace GameReserve.WebApi.Health;
public class DatabaseHealthCheck(NpgsqlDataSource dataSource) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            using NpgsqlConnection connection = dataSource.OpenConnection();

            await connection.ExecuteScalarAsync("SELECT 1");

            return HealthCheckResult.Healthy();
        }
        catch(Exception ex)
        {
            return HealthCheckResult.Unhealthy(exception: ex);
        }
    }
}