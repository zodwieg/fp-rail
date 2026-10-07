using FpRail.Domain.Common;

namespace FpRail.Domain.Infrastructure;

public record SpeedRestrictionSegment : ValueObject
{
    // Начало ограничения от начала пути (в метрах)
    public double StartDistance { get; }
    
    // Конец ограничения от начала пути (в метрах)
    public double EndDistance { get; }
    
    // Максимально разрешенная скорость на этом участке (км/ч)
    public double MaxSpeedKmh { get; }
    
    // Причина ограничения (например: "Кривая R=20", "Совмещенное полотно", "Стрелка")
    public string Reason { get; }

    public SpeedRestrictionSegment(double startDistance, double endDistance, double maxSpeedKmh, string reason = "")
    {
        if (startDistance < 0)
            throw new ArgumentException("Начало сегмента не может быть отрицательным.");
        
        if (endDistance <= startDistance)
            throw new ArgumentException("Конец сегмента должен быть строго больше его начала.");
        
        if (maxSpeedKmh <= 0)
            throw new ArgumentException("Ограничение скорости должно быть больше нуля.");

        StartDistance = startDistance;
        EndDistance = endDistance;
        MaxSpeedKmh = maxSpeedKmh;
        Reason = reason;
    }

    // Метод проверки, попадает ли точка (метр пути) под это ограничение
    public bool IsInside(double distance) => distance >= StartDistance && distance <= EndDistance;
}
