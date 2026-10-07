using FpRail.Domain.Common;

namespace FpRail.Domain.Network;

public class HubTrack : Entity
{
    // Ссылка на логический Track, который физически представляет этот путь в графе
    public Guid TrackId { get; private set; }
    
    // Название пути для диспетчера (например, "Веер №3", "Обгонной путь кольца")
    public string DisplayName { get; private set; }
    
    // Физическая вместимость пути в метрах. 
    // Зная длину вагона (например, 15м или 27м), Application легко посчитает лимит емкости.
    public double CapacityInMeters { get; private set; }

    public HubTrack(Guid trackId, double capacityInMeters, string displayName = "")
    {
        TrackId = trackId;
        CapacityInMeters = capacityInMeters;
        DisplayName = string.IsNullOrWhiteSpace(displayName) ? $"Путь {Id.ToString()[..4]}" : displayName;
    }
}
