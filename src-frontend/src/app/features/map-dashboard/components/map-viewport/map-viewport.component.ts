import { Component, ElementRef, AfterViewInit, ViewChild, inject } from '@angular/core';
import { MapService } from '../../services/map-service';
import { DrawingService, DrawingMode } from '../../services/drawing-service';

@Component({
  selector: 'fp-map-viewport',
  standalone: true,
  templateUrl: './map-viewport.component.html',
  styleUrls: ['map-viewport.component.scss'],
  // Подключаем как провайдер на уровне компонента, чтобы жизненный цикл 
  // DrawingService был связан с этим компонентом карты (опционально)
  providers: [DrawingService] 
})
export class MapViewportComponent implements AfterViewInit {
  @ViewChild('mapContainer') mapContainer!: ElementRef<HTMLDivElement>;
  
  private mapService = inject(MapService);
  private drawingService = inject(DrawingService); // Инициализирует подписку на карту

  ngAfterViewInit(): void {
    this.mapService.initMap(this.mapContainer.nativeElement);
  }

  changeMode(mode: DrawingMode): void {
    this.drawingService.setMode(mode);
  }

  clearAll(): void {
    this.drawingService.clear();
  }
}
