using FpRail.Domain.Common;
using FpRail.Domain.Exceptions;
using NetTopologySuite.Geometries;

namespace FpRail.Domain.Coordinates;

public record GeographicCoordinate : ValueObject
{
    public double Latitude { get; }
    public double Longitude { get; }

    public GeographicCoordinate(double latitude, double longitude)
    {
        // Доменная валидация границ земного шара
        if (latitude is < -90.0 or > 90.0)
            throw new InvalidCoordinateException($"Некорректная широта (Latitude): {latitude}. Должна быть от -90 до 90.");

        if (longitude is < -180.0 or > 180.0)
            throw new InvalidCoordinateException($"Некорректная долгота (Longitude): {longitude}. Должна быть от -180 до 180.");

        Latitude = latitude;
        Longitude = longitude;
    }

    public Coordinate ToNtsCoordinate() => new(Longitude, Latitude);

    public static GeographicCoordinate FromNtsCoordinate(Coordinate coordinate) 
        => new(coordinate.Y, coordinate.X);
}
