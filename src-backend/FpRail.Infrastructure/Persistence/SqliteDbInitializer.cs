using Dapper;

namespace FpRail.Infrastructure.Persistence;

public interface IDbInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
}

public class SqliteDbInitializer : IDbInitializer
{
    private readonly IDbConnectionFactory _connectionFactory;

    public SqliteDbInitializer(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);

        // 1. Инициализируем метаданные SpatiaLite
        var tableExists = await connection.QueryFirstOrDefaultAsync<int>(
            "SELECT count(*) FROM sqlite_master WHERE type='table' AND name='spatial_ref_sys';"
        );

        if (tableExists == 0)
        {
            await connection.ExecuteAsync("SELECT InitSpatialMetaData(1);");
        }

        // 2. Создаем стандартные таблицы структуры данных
        await connection.ExecuteAsync(SqliteSchemaScripts.CreateTables);

        // 3. Создаем пространственные ГИС-колонки и индексы
        await connection.ExecuteAsync(SqliteSchemaScripts.CreateSpatialColumns);
    }
}
