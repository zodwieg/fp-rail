import { Injectable, inject } from '@angular/core';
import { FeatureProcessorService } from './feature-processor.service';
import { NodeApiService } from './api/node-api.service';

@Injectable({
  providedIn: 'root'
})
export class DrawingEventHandlerService {
  private processor = inject(FeatureProcessorService);
  private nodeApiService = inject(NodeApiService);

  /**
   * Главный диспетчер завершения рисования
   */
  public handleFeatureFinish(finishedFeature: any): void {
    if (!finishedFeature || !finishedFeature.geometry) return;

    const geometryType = finishedFeature.geometry.type;
    console.log(`=== Рисование завершено [Тип: ${geometryType}] ===`);

    switch (geometryType) {
      case 'Point':
        this.handlePointSave(finishedFeature);
        break;

      case 'LineString':
        this.processor.processLineString(finishedFeature);
        break;

      case 'Polygon':
        this.processor.processPolygon(finishedFeature);
        break;

      default:
        console.warn(`Неподдерживаемый тип геометрии: ${geometryType}`);
    }
  }

  private handlePointSave(feature: any): void {
    const pointDto = this.processor.processPoint(feature);
    
    // Отправляем в API layer
    this.nodeApiService.sendPoint(pointDto).subscribe({
      next: (response) => console.log('Данные точки успешно синхронизированы с бэкендом:', response),
      error: (error) => console.error('Ошибка сохранения точки инфраструктуры:', error)
    });
  }
}
