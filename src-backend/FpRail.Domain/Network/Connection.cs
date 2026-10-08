using FpRail.Domain.Common;
using NetTopologySuite.Geometries;

namespace FpRail.Domain.Network;

public class Connection : Entity
{
    /// <summary>
    /// Идентификатор начального узла перегона
    /// </summary>
    public Guid FromNodeId { get; private set; }

    /// <summary>
    /// Идентификатор конечного узла перегона
    /// </summary>
    public Guid ToNodeId { get; private set; }

    public LineString Geometry { get; private set; }

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
        double initialLength = geometry.Length; 
        _tracks.Add(new Track(Id, isForwardDirection: true, initialLength));
        _tracks.Add(new Track(Id, isForwardDirection: false, initialLength));
    }

    public void AddCustomTrack(bool isForward, double length)
    {
        _tracks.Add(new Track(Id, isForward, length));
    }
}
