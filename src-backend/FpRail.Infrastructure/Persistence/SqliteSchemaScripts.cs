namespace FpRail.Infrastructure.Persistence;

public static class SqliteSchemaScripts
{
    public const string CreateTables = @"
        -- 1. Таблица Узлов (Nodes)
        CREATE TABLE IF NOT EXISTS Nodes (
            Id TEXT PRIMARY KEY NOT NULL,          -- Guid сохраняем как строку
            Name TEXT NOT NULL,
            IsHub INTEGER NOT NULL DEFAULT 0,      -- Булево как 0 или 1
            ConnectivityMatrix TEXT NULL           -- HashSet<Turn> сериализуем в JSON text
        );

        -- 2. Таблица Перегонов (Connections)
        CREATE TABLE IF NOT EXISTS Connections (
            Id TEXT PRIMARY KEY NOT NULL,
            FromNodeId TEXT NOT NULL,
            ToNodeId TEXT NOT NULL,
            FOREIGN KEY (FromNodeId) REFERENCES Nodes(Id) ON DELETE RESTRICT,
            FOREIGN KEY (ToNodeId) REFERENCES Nodes(Id) ON DELETE RESTRICT
        );
    ";

    public const string CreateSpatialColumns = @"
        -- Добавляем пространственную колонку для Nodes (Точка в системе координат WGS84 - SRID 4326)
        -- Параметры: имя_таблицы, имя_колонки, SRID, тип_геометрии, размерность (2 = XY)
        SELECT IFNULL(
            (SELECT 1 FROM geometry_columns WHERE f_table_name = 'nodes' AND f_geometry_column = 'Location'),
            AddGeometryColumn('Nodes', 'Location', 4326, 'POINT', 2)
        );

        -- Добавляем пространственную колонку для Connections (Ось пути - ломаная линия LineString)
        SELECT IFNULL(
            (SELECT 1 FROM geometry_columns WHERE f_table_name = 'connections' AND f_geometry_column = 'Geometry'),
            AddGeometryColumn('Connections', 'Geometry', 4326, 'LINESTRING', 2)
        );

        -- Создаем пространственные R-Tree индексы для мгновенного ГИС-поиска по координатам
        SELECT CreateSpatialIndex('Nodes', 'Location');
        SELECT CreateSpatialIndex('Connections', 'Geometry');
    ";
}
