using FpRail.Domain.Common;
using FpRail.Domain.Infrastructure;

namespace FpRail.Domain.Network;

public class Track : Entity
{
    public Guid ConnectionId { get; private set; }
    public bool IsForwardDirection { get; private set; }
    public double LengthInMeters { get; private set; }

    // Список остановок на данном пути
    private readonly List<Station> _stations = new();
    public IReadOnlyCollection<Station> Stations => _stations.AsReadOnly();

    // Профиль ограничений скорости на данном пути
    private readonly List<SpeedRestrictionSegment> _speedRestrictions = new();
    public IReadOnlyCollection<SpeedRestrictionSegment> SpeedRestrictions => _speedRestrictions.AsReadOnly();

    public Track(Guid connectionId, bool isForwardDirection, double lengthInMeters)
    {
        if (connectionId == Guid.Empty) throw new ArgumentException("ConnectionId не может быть пустым.");
        if (lengthInMeters <= 0) throw new ArgumentException("Длина пути должна быть больше нуля.");

        ConnectionId = connectionId;
        IsForwardDirection = isForwardDirection;
        LengthInMeters = lengthInMeters;
    }

    // Бизнес-метод добавления остановки с валидацией границ пути
    public void AddStation(Station station)
    {
        ArgumentNullException.ThrowIfNull(station);

        if (station.Location.TrackId != Id)
            throw new InvalidOperationException("Остановка должна принадлежать именно этому пути (Track).");

        if (station.Location.DistanceFromStart > LengthInMeters)
            throw new InvalidOperationException($"Остановка находится за пределами длины пути ({station.Location.DistanceFromStart}м > {LengthInMeters}м).");

        _stations.Add(station);
    }

    // Бизнес-метод добавления ограничения скорости
    public void AddSpeedRestriction(SpeedRestrictionSegment restriction)
    {
        ArgumentNullException.ThrowIfNull(restriction);

        if (restriction.EndDistance > LengthInMeters)
            throw new InvalidOperationException("Сегмент ограничения скорости выходит за физические границы пути.");

        // Здесь в будущем можно добавить логику проверки пересечений ограничений, 
        // например, если новое ограничение жестче существующего — перекрывать его
        _speedRestrictions.Add(restriction);
    }
}
