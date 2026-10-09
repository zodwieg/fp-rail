using System.Text.Json.Nodes;

namespace FPRail.Application.DTOs;

public record CreateTramPointFeatureDto(
    Guid Id,
    string Type, // Будет "Feature"
    PointGeometryDto Geometry,
    JsonNode? Properties // Гибкий объект для будущих атрибутов предметной области
);

public record PointGeometryDto(
    string Type, // Будет "Point"
    double[] Coordinates // [longitude, latitude]
);
