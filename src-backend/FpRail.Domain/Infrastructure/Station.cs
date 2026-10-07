using FpRail.Domain.Common;
using FpRail.Domain.Coordinates;

namespace FpRail.Domain.Infrastructure;

public class Station : Entity
{
    public string Name { get; private set; }
    
    // Линейная координата привязки к конкретному рельсовому пути (Track)
    public LinearCoordinate Location { get; private set; }
    
    // Длина посадочной платформы в метрах (важно для CAD и вместимости СМЕ/длинных вагонов)
    public double PlatformLengthInMeters { get; private set; }
    
    // Нормативное время на открытие/закрытие дверей и посадку пассажиров (в секундах)
    public double StandardDwellTimeInSeconds { get; private set; }

    public Station(string name, LinearCoordinate location, double platformLengthInMeters = 30.0, double standardDwellTimeInSeconds = 20.0)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Название остановки не может быть пустым.");
        
        if (platformLengthInMeters <= 0)
            throw new ArgumentException("Длина платформы должна быть больше нуля.");

        if (standardDwellTimeInSeconds < 0)
            throw new ArgumentException("Время стоянки не может быть отрицательным.");

        Name = name;
        Location = location;
        PlatformLengthInMeters = platformLengthInMeters;
        StandardDwellTimeInSeconds = standardDwellTimeInSeconds;
    }

    // Метод для переноса остановки (например, в CAD-редакторе передвинули мышкой)
    public void MoveTo(LinearCoordinate newLocation)
    {
        ArgumentNullException.ThrowIfNull(newLocation);
        Location = newLocation;
    }
}
