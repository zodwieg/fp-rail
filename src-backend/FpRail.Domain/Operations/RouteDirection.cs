using FpRail.Domain.Common;

namespace FpRail.Domain.Operations;

public class RouteDirection : Entity
{
    public string DirectionName { get; private set; } // Например, "Прямое (А -> Б)" или "Обратное (Б -> А)"
    
    // Упорядоченная последовательность путей графа. 
    // Важно: именно по ней алгоритм будет собирать SpeedRestrictionSegments для расчета времени
    private readonly List<Guid> _trackSequence = new();
    public IReadOnlyList<Guid> TrackSequence => _trackSequence.AsReadOnly();

    public RouteDirection(string directionName)
    {
        if (string.IsNullOrWhiteSpace(directionName)) throw new ArgumentException("Название направления не может быть пустым.");
        DirectionName = directionName;
    }

    // Метод добавления следующего участка пути в маршрут
    public void AppendTrack(Guid trackId)
    {
        if (trackId == Guid.Empty) throw new ArgumentException("TrackId не может быть пустым.");
        // Тут можно в будущем добавить доменную валидацию: 
        // Проверить через матрицу соединенности узла, что новый Track физически соединен с предыдущим.
        _trackSequence.Add(trackId);
    }
    
    public void ClearRoute() => _trackSequence.Clear();
}
