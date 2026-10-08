using Dapper;
using FluentAssertions;
using FpRail.Infrastructure.Persistence;
using Xunit;

namespace FpRail.Infrastructure.Tests;

public class SqliteDbInitializerTests
{
    [Fact]
    public async Task InitializeAsync_ShouldCreateSpatialiteMetadataTables()
    {
        // 1. ARRANGE
        string connectionString = "Data Source=InitializerTestDb;Mode=Memory;Cache=Shared;";
        var factory = new SqliteConnectionFactory(connectionString);
        var initializer = new SqliteDbInitializer(factory);

        // Открываем мастер-соединение, которое будет держать базу данных «живой» весь тест
        using var masterConnection = await factory.CreateConnectionAsync();

        // Проверяем, что ГИС-таблиц изначально нет
        var tableCountBefore = await masterConnection.QueryFirstOrDefaultAsync<int>(
            "SELECT count(*) FROM sqlite_master WHERE type='table' AND name='spatial_ref_sys';"
        );
        tableCountBefore.Should().Be(0);

        // 2. ACT
        // Запускаем инициализатор (он создаст свое соединение к той же живой базе данных)
        await initializer.InitializeAsync();

        // 3. ASSERT
        // Проверяем результат через наше мастер-соединение, база никуда не исчезла!
        var tableCountAfter = await masterConnection.QueryFirstOrDefaultAsync<int>(
            "SELECT count(*) FROM sqlite_master WHERE type='table' AND name='spatial_ref_sys';"
        );

        // Теперь ГИС-метаданные гарантированно на месте
        tableCountAfter.Should().Be(1);

        // Проверим, что одна из ГИС-таблиц (например, geometry_columns) тоже создалась
        var geomTableExists = await masterConnection.QueryFirstOrDefaultAsync<int>(
            "SELECT count(*) FROM sqlite_master WHERE type='table' AND name='geometry_columns';"
        );
        geomTableExists.Should().Be(1);
    }

    [Fact]
    public async Task InitializeAsync_ShouldCreateDomainTables_AndRegisterGeometryColumns()
    {
        // 1. ARRANGE
        string connectionString = "Data Source=DomainInfrastructureTestDb;Mode=Memory;Cache=Shared;";
        var factory = new SqliteConnectionFactory(connectionString);
        var initializer = new SqliteDbInitializer(factory);

        // Держим мастер-соединение для сохранения In-Memory базы данных
        using var masterConnection = await factory.CreateConnectionAsync();

        // 2. ACT
        await initializer.InitializeAsync();

        // 3. ASSERT
        // А. Проверяем физическое наличие таблиц в SQLite
        var tables = (await masterConnection.QueryAsync<string>(
            "SELECT name FROM sqlite_master WHERE type='table';"
        )).ToList();

        tables.Should().Contain("Nodes");
        tables.Should().Contain("Connections");

        // Б. Проверяем, что SpatiaLite успешно зарегистрировал наши ГИС-колонки в системном каталоге
        var registeredGeometriesCount = await masterConnection.QueryFirstOrDefaultAsync<int>(
            "SELECT count(*) FROM geometry_columns WHERE f_table_name IN ('nodes', 'connections');"
        );
        
        // Должно быть зарегистрировано ровно 2 колонки: Location для Nodes и Geometry для Connections
        registeredGeometriesCount.Should().Be(2);

        // В. Проверяем, что создались виртуальные таблицы пространственных индексов R-Tree (имеют суффикс _spatialindex)
        var rTreeIndexExists = await masterConnection.QueryFirstOrDefaultAsync<int>(
            "SELECT count(*) FROM sqlite_master WHERE type='table' AND name='idx_Connections_Geometry';"
        );
        rTreeIndexExists.Should().Be(1);
    }

}
