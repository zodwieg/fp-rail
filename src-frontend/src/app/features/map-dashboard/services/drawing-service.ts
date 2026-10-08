import { Injectable, inject, OnDestroy } from '@angular/core';
import { MapService } from './map-service';
import { Subscription } from 'rxjs';
import { 
  TerraDraw,
  TerraDrawPolygonMode, 
  TerraDrawLineStringMode, 
  TerraDrawPointMode, 
  TerraDrawSelectMode 
} from 'terra-draw';
import { TerraDrawMapLibreGLAdapter } from 'terra-draw-maplibre-gl-adapter';

export type DrawingMode = 'static' | 'polygon' | 'linestring' | 'point' | 'select';

@Injectable({
  providedIn: 'root'
})
export class DrawingService implements OnDestroy {
  private mapService = inject(MapService);
  private drawInstance: TerraDraw | null = null;
  private mapSubscription: Subscription;

  constructor() {
    // Автоматически подписываемся на готовность карты
    this.mapSubscription = this.mapService.mapReady$.subscribe((map) => {
        console.log("Map is ready!");
      this.initTerraDraw(map);
    });
  }

  /**
   * Инициализация TerraDraw с необходимыми режимами
   */
  private initTerraDraw(map: any): void {
    // Конфигурируем стили и режимы (здесь можно кастомизировать цвета точек/полигонов)
    this.drawInstance = new TerraDraw({
      adapter: new TerraDrawMapLibreGLAdapter({ map }),
      modes: [
        new TerraDrawSelectMode({
            flags: {
                polygon: {
                feature: { 
                    draggable: true, 
                    rotateable: true,
                    scaleable: true,
                    coordinates: { 
                    midpoints: true, 
                    draggable: true, 
                    deletable: true 
                    }
                }
                },
                lineString: {
                feature: { 
                    draggable: true,
                    coordinates: { 
                    midpoints: true, 
                    draggable: true, 
                    deletable: true 
                    }
                }
                },
                point: {
                feature: { 
                    draggable: true 
                }
                }
            }
            }),
        new TerraDrawPolygonMode(),
        new TerraDrawLineStringMode(),
        new TerraDrawPointMode()
      ]
    });

    // Стартуем TerraDraw
    this.drawInstance.start();
    
    // По умолчанию включаем режим select или оставляем пустой (static)
    this.setMode('static');

    this.drawInstance.on('finish', (id) => {
        // 1. Получаем весь снимок данных в формате GeoJSON FeatureCollection
        const snapshot = this.drawInstance?.getSnapshot();
        if (!snapshot) return;

        // 2. Ищем именно ту фигуру, рисование которой только что завершилось
        const finishedFeature = snapshot.find((f: any) => f.id === id);

        if (finishedFeature) {
        const geometryType = finishedFeature.geometry.type; // 'Point', 'LineString' или 'Polygon'
        const coordinates = finishedFeature.geometry.coordinates;

        console.log(`=== Рисование завершено [Тип: ${geometryType}] ===`);
        
        // Разбираем координаты в зависимости от типа геометрии
        if (geometryType === 'Point') {
            // Для точки координаты — это простой массив [lng, lat]
            const [lng, lat] = coordinates;
            console.log(`Координаты точки: Широта (Lat): ${lat}, Долгота (Lng): ${lng}`);
        } 
        else if (geometryType === 'LineString') {
            console.log('Координаты линии (список точек):');
            
            // Приводим к any, чтобы зафиксировать массив координат [lng, lat]
            (coordinates as any).forEach((coord: [number, number], index: number) => {
                console.log(`  Точка #${index + 1}: Lat: ${coord[1]}, Lng: ${coord[0]}`);
            });
            }
            else if (geometryType === 'Polygon') {
            console.log('Координаты полигона (внешний контур):');
            
            // У полигона первый элемент массива — это всегда внешнее кольцо координат
            const outerRing = coordinates[0] as any;
            
            outerRing.forEach((coord: [number, number], index: number) => {
                console.log(`  Вершина #${index + 1}: Lat: ${coord[0]}, Lng: ${coord[1]}`);
            });
        }

        console.log('Полный объект геометрии:', finishedFeature);
        }
    });

    // Пример подписки на события рисования (фичи созданы/изменены)
    this.drawInstance.on('change', (ids, type) => {
      // console.log(`TerraDraw event: ${type}`, ids);
    });
  }

  /**
   * Переключение текущего режима рисования
   */
  public setMode(mode: DrawingMode): void {
    if (!this.drawInstance) {
      console.warn('TerraDraw еще не инициализирован');
      return;
    }
    this.drawInstance.setMode(mode);
  }

  /**
   * Получить нарисованные данные в формате GeoJSON
   */
  public getSnapshot(): any {
    if (!this.drawInstance) return null;
    return this.drawInstance.getSnapshot();
  }

  /**
   * Очистить карту от всех нарисованных элементов
   */
  public clear(): void {
    if (this.drawInstance) {
      this.drawInstance.clear();
    }
  }

  ngOnDestroy(): void {
    this.mapSubscription.unsubscribe();
    if (this.drawInstance) {
      this.drawInstance.stop();
      this.drawInstance = null;
    }
  }
}
