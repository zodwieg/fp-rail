using Dapper;
using FluentAssertions;
using FpRail.Infrastructure.Persistence;
using Xunit;

namespace FpRail.Infrastructure.Tests;

public class SqliteConnectionFactoryTests
{
    [Fact] // Этот атрибут указывает xUnit, что метод является тестом
    public async Task CreateConnectionAsync_ShouldOpenConnection_AndLoadSpatialite()
    {
        // 1. ARRANGE (Подготовка)
        // Используем строку подключения для базы данных в оперативной памяти (In-Memory).
        // Cache=Shared нужен для того, чтобы база данных не уничтожалась сразу после открытия соединения.
        string connectionString = "Data Source=TestTramDb;Mode=Memory;Cache=Shared;";
        
        var factory = new SqliteConnectionFactory(connectionString);

        // 2. ACT (Действие)
        // Пытаемся создать и открыть наше подключение
        using var connection = await factory.CreateConnectionAsync();

        // Проверяем версию SpatiaLite с помощью SQL-запроса через Dapper
        string? resultVersion = await connection.QueryFirstOrDefaultAsync<string>("SELECT spatialite_version();");

        // 3. ASSERT (Проверка)
        // Проверяем, что соединение не null и действительно открыто
        connection.Should().NotBeNull();
        connection.State.Should().Be(System.Data.ConnectionState.Open);

        // Проверяем, что SpatiaLite успешно загрузился и вернул строку версии (например, "5.1.0")
        resultVersion.Should().NotBeNullOrEmpty();
        resultVersion.Should().Contain("5."); // Проверяем, что мажорная версия совпадает с ожидаемой
        
        // Дополнительно: проверим работу простейшей ГИС-функции
        string? wktPoint = await connection.QueryFirstOrDefaultAsync<string>(
            "SELECT AsText(GeomFromText('POINT(30.3 59.9)', 4326));"
        );
        wktPoint.Should().Be("POINT(30.3 59.9)");
    }
}
