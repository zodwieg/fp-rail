using Microsoft.AspNetCore.Mvc;
using FPRail.Application.DTOs;
// using MediatR; // Если планируешь использовать MediatR для CQRS

namespace FPRail.Presentation.Controllers;

[ApiController]
[Route("api/v1/tram-points")]
public class TramPointsController : ControllerBase
{
    // private readonly IMediator _mediator;
    // public TramPointsController(IMediator mediator) => _mediator = mediator;

    public TramPointsController()
    {
        // Пока конструктор пустой, логику добавим на следующем шаге
    }

    [HttpPost]
    public async Task<IActionResult> CreatePoint([FromBody] CreateTramPointFeatureDto featureDto)
    {
        // 1. Валидация формата (базовая)
        if (featureDto.Geometry == null || featureDto.Geometry.Type != "Point" || featureDto.Geometry.Coordinates.Length < 2)
        {
            return BadRequest("Некорректный формат геометрии Point. Ожидаются координаты [lng, lat].");
        }

        // 2. Точка расширения: здесь мы будем отправлять команду в слой Application
        // var command = new CreateTramPointCommand(featureDto);
        // var result = await _mediator.Send(command);

        // Пока возвращаем заглушку Ok, чтобы протестировать проливку данных с фронта
        return Ok(new { 
            Message = "Геометрия успешно получена бэкендом", 
            ReceivedId = featureDto.Id,
            Coordinates = featureDto.Geometry.Coordinates
        });
    }
}
