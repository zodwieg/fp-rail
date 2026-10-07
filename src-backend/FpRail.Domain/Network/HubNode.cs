using FpRail.Domain.Common;
using FpRail.Domain.Coordinates;

namespace FpRail.Domain.Network;

public class HubNode : Node
{
    // Может ли узел служить конечной точкой маршрута (смена расписания, отстой, обед водителя)
    public bool IsTerminal { get; private set; }

    // Может ли узел обеспечивать ночной отстой и выпуск на линию (расчет нулевых рейсов)
    public bool IsDepot { get; private set; }

    // Внутренние специализированные пути накопители (веера депо, пути отстоя кольца)
    private readonly List<HubTrack> _internalHubTracks = new();
    public IReadOnlyCollection<HubTrack> InternalHubTracks => _internalHubTracks.AsReadOnly();

    public HubNode(string name, GeographicCoordinate position, bool isTerminal, bool isDepot) 
        : base(name, position)
    {
        IsTerminal = isTerminal;
        IsDepot = isDepot;
    }

    // Метод добавления паркового пути / пути отстоя
    public void AddHubTrack(Guid trackId, double capacityInMeters, string trackName = "")
    {
        if (trackId == Guid.Empty)
            throw new ArgumentException("TrackId не может быть пустым.");
        
        if (capacityInMeters <= 0)
            throw new ArgumentException("Вместимость пути должна быть больше нуля.");

        _internalHubTracks.Add(new HubTrack(trackId, capacityInMeters, trackName));
    }

    // Бизнес-логика изменения назначения узла (например, закрыли депо, оставили только кольцо)
    public void UpdateCapabilities(bool isTerminal, bool isDepot)
    {
        IsTerminal = isTerminal;
        IsDepot = isDepot;
    }
}
