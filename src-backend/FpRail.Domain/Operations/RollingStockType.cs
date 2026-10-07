using FpRail.Domain.Common;

namespace FpRail.Domain.Operations;

public class RollingStockType : Entity
{
    public string ModelName { get; private set; } // Например, "71-931M Витязь-М"
    
    // Физические параметры для вместимости депо/колец
    public double LengthInMeters { get; private set; } 
    
    // Динамические параметры для расчета эпюры скоростей
    public double MaxConstructiveSpeedKmh { get; private set; } // Конструктивная скорость
    public double MaxAcceleration { get; private set; }         // Ускорение при разгоне (м/с²)
    public double MaxDeceleration { get; private set; }         // Замедление при торможении (м/с²)
    
    // Экономика/Пассажиропоток (может пригодиться в будущем)
    public int PassengerCapacity { get; private set; }

    public RollingStockType(
        string modelName, 
        double lengthInMeters, 
        double maxConstructiveSpeedKmh = 60.0, 
        double maxAcceleration = 1.2, 
        double maxDeceleration = 1.3, 
        int passengerCapacity = 150)
    {
        if (string.IsNullOrWhiteSpace(modelName)) throw new ArgumentException("Модель вагона должна иметь название.");
        if (lengthInMeters <= 0) throw new ArgumentException("Длина вагона должна быть больше нуля.");
        if (maxConstructiveSpeedKmh <= 0) throw new ArgumentException("Скорость должна быть больше нуля.");

        ModelName = modelName;
        LengthInMeters = lengthInMeters;
        MaxConstructiveSpeedKmh = maxConstructiveSpeedKmh;
        MaxAcceleration = maxAcceleration;
        MaxDeceleration = maxDeceleration;
        PassengerCapacity = passengerCapacity;
    }
}
