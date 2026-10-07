using FpRail.Domain.Common;
using FpRail.Domain.Exceptions;

namespace FpRail.Domain.Coordinates;

public record LinearCoordinate : ValueObject
{
    public Guid TrackId { get; }
    public double DistanceFromStart { get; }

    public LinearCoordinate(Guid trackId, double distanceFromStart)
    {
        if (trackId == Guid.Empty)
            throw new InvalidCoordinateException("Идентификатор пути (TrackId) не может быть пустым.");

        if (distanceFromStart < 0.0)
            throw new InvalidCoordinateException($"Дистанция не может быть отрицательной: {distanceFromStart}.");

        TrackId = trackId;
        DistanceFromStart = distanceFromStart;
    }
}
