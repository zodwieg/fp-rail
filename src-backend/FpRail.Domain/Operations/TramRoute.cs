using FpRail.Domain.Common;

namespace FpRail.Domain.Operations;

public class TramRoute : Entity
{
    public string RouteNumber { get; private set; } // Например, "№ 3" или "№ A"
    public string Description { get; private set; } // Например, "Площадь Репина — Ланская площадь"
    
    // Закрепленный тип вагона
    public Guid TargetRollingStockTypeId { get; private set; }

    // Траектории движения
    public RouteDirection ForwardDirection { get; private set; }
    public RouteDirection BackwardDirection { get; private set; }

    // Эксплуатационные параметры (целевые показатели для пользователя)
    public double TargetIntervalInMinutes { get; private set; }

    public TramRoute(string routeNumber, string description, Guid targetRollingStockTypeId, double targetIntervalInMinutes = 10.0)
    {
        if (string.IsNullOrWhiteSpace(routeNumber)) throw new ArgumentException("Номер маршрута не может быть пустым.");
        if (targetRollingStockTypeId == Guid.Empty) throw new ArgumentException("Необходимо указать тип подвижного состава.");
        if (targetIntervalInMinutes <= 0) throw new ArgumentException("Интервал движения должен быть больше нуля.");

        RouteNumber = routeNumber;
        Description = description;
        TargetRollingStockTypeId = targetRollingStockTypeId;
        TargetIntervalInMinutes = targetIntervalInMinutes;
        
        ForwardDirection = new RouteDirection($"{routeNumber} - Прямое направление");
        BackwardDirection = new RouteDirection($"{routeNumber} - Обратное направление");
    }

    // Бизнес-метод обновления интервала (если пользователь в CAD меняет параметры парка)
    public void UpdateTargetInterval(double newInterval)
    {
        if (newInterval <= 0) throw new ArgumentException("Интервал должен быть больше нуля.");
        TargetIntervalInMinutes = newInterval;
    }
}
