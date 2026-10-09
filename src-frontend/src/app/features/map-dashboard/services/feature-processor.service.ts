import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class FeatureProcessorService {

  /**
   * Конвертирует сырую фичу TerraDraw типа Point в строгую доменную DTO
   */
  public processPoint(terraDrawFeature: any): any {
    return {
      id: terraDrawFeature.id !== undefined ? String(terraDrawFeature.id) : crypto.randomUUID(),
      type: 'Feature',
      geometry: {
        type: 'Point',
        // Просто передаем массив координат [lng, lat] как есть
        coordinates: terraDrawFeature.geometry.coordinates 
      },
      properties: terraDrawFeature.properties || {}
    };
  }

  /**
   * Логирует и подготавливает LineString для будущей отправки (трамвайные пути)
   */
  public processLineString(terraDrawFeature: any): void {
    const coordinates = terraDrawFeature.geometry.coordinates as [number, number][];
    console.log('--- Обработка новой линии трамвайной сети ---');
    coordinates.forEach((coord, index) => {
      console.log(`  Пикет #${index + 1}: Lat: ${coord[1]}, Lng: ${coord[0]}`);
    });
    // Тут в будущем будет маппинг в TramPathFeature
  }

  /**
   * Логирует и подготавливает Polygon (депо, охранные зоны, районы)
   */
  public processPolygon(terraDrawFeature: any): void {
    const outerRing = terraDrawFeature.geometry.coordinates[0] as [number, number][];
    console.log('--- Обработка полигона зоны инфраструктуры ---');
    outerRing.forEach((coord, index) => {
      console.log(`  Вершина контура #${index + 1}: Lat: ${coord[1]}, Lng: ${coord[0]}`);
    });
  }
}
