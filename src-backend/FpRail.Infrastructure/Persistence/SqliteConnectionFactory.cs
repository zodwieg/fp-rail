using System.Data;
using Microsoft.Data.Sqlite;

namespace FpRail.Infrastructure.Persistence;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
}

public class SqliteConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqliteConnectionFactory(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public async Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new SqliteConnection(_connectionString);
        
        try
        {
            await connection.OpenAsync(cancellationToken);
            
            // Включаем поддержку расширений и загружаем SpatiaLite
            connection.EnableExtensions(true);
            
            // Имя расширения теперь унифицировано для всех платформ внутри экосистемы MS
            connection.LoadExtension("mod_spatialite");
            
            return connection;
        }
        catch (Exception ex)
        {
            await connection.DisposeAsync();
            throw new InvalidOperationException(
                "Критическая ошибка при создании подключения к БД или загрузке расширения SpatiaLite. " +
                "Проверьте, установлен ли пакет Microsoft.EntityFrameworkCore.Sqlite.NetTopologySuite.", ex);
        }
    }
}
