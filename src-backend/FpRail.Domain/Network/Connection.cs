using FpRail.Domain.Common;
using NetTopologySuite.Geometries;

namespace FpRail.Domain.Network;

public class Connection : Entity
{
    public Guid FromNodeId { get; private set; }
    public Guid ToNodeId { get; private set; }

    // Базовая ось пути (LineString из NTS). 
    // Хранит в себе всю сырую ломаную со всеми микро-изгибами (Kinks)
    public LineString Geometry { get; private set; }

    // Внутренние «настоящие» ребра движения (пути)
    private readonly List<Track> _tracks = new();
    public IReadOnlyCollection<Track> Tracks => _tracks.AsReadOnly();

    public Connection(Guid fromNodeId, Guid toNodeId, LineString geometry)
    {
        if (fromNodeId == Guid.Empty || toNodeId == Guid.Empty)
            throw new ArgumentException("Идентификаторы узлов не могут быть пустыми.");
        
        if (geometry == null || geometry.IsEmpty)
            throw new ArgumentException("Геометрия перегона не может быть пустой.");

        FromNodeId = fromNodeId;
        ToNodeId = toNodeId;
        Geometry = geometry;

        // По умолчанию при создании перегона, пускай в нем автоматически 
        // создается классическая двухпутка (один путь туда, один обратно)
        // Длину берем физическую из геометрии NTS (в зависимости от проекции)
        double initialLength = geometry.Length; 
        _tracks.Add(new Track(Id, isForwardDirection: true, initialLength));
        _tracks.Add(new Track(Id, isForwardDirection: false, initialLength));
    }

    // Бизнес-метод для изменения количества путей на перегоне 
    // (например, добавление третьего пути перед перекрестком)
    public void AddCustomTrack(bool isForward, double length)
    {
        _tracks.Add(new Track(Id, isForward, length));
    }
}
