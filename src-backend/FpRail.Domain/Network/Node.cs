using FpRail.Domain.Common;
using FpRail.Domain.Coordinates;
namespace FpRail.Domain.Network;

// Класс, описывающий разрешенный маневр (стрелку/переход) внутри Узла
public record Turn(Guid FromTrackId, Guid ToTrackId, double MaxSpeedLimit);

public class Node : Entity
{
    public string Name { get; private set; }
    
    // Географический центр узла для отображения на карте
    public GeographicCoordinate Position { get; private set; }

    // Матрица соединенности путей внутри этого узла (наши стрелки)
    private readonly HashSet<Turn> _connectionsMatrix = new();
    public IReadOnlyCollection<Turn> ConnectionsMatrix => _connectionsMatrix.ToList().AsReadOnly();

    public Node(string name, GeographicCoordinate position)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Имя узла не может быть пустым.");

        Name = name;
        Position = position;
    }

    // Добавить связь между путями (разрешить трамваю проехать с одного трека на другой)
    public void RegisterTurn(Guid fromTrackId, Guid toTrackId, double maxSpeedLimit = 15.0)
    {
        if (fromTrackId == Guid.Empty || toTrackId == Guid.Empty)
            throw new ArgumentException("Track ID не могут быть пустыми при настройке стрелки.");

        var turn = new Turn(fromTrackId, toTrackId, maxSpeedLimit);
        _connectionsMatrix.Add(turn);
    }

    // Удалить связь (например, демонтировали стрелку)
    public void RemoveTurn(Guid fromTrackId, Guid toTrackId)
    {
        _connectionsMatrix.RemoveWhere(t => t.FromTrackId == fromTrackId && t.ToTrackId == toTrackId);
    }
}
