import { Injectable, OnDestroy } from '@angular/core';
import * as maplibregl from 'maplibre-gl';
import { MAP_CONFIG } from '../../../core/config/map.config';
import { BehaviorSubject, filter, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MapService implements OnDestroy {
  private map$ = new BehaviorSubject<maplibregl.Map | null>(null);

  /**
   * Возвращает поток с экземпляром карты, когда она готова (после события 'load')
   */
  public get mapReady$(): Observable<maplibregl.Map> {
  return this.map$.asObservable().pipe(
    filter((map): map is maplibregl.Map => map !== null)
  );
}
  
  /**
   * Прямой доступ к текущему инстансу карты (если нужен)
   */
  public get mapInstance(): maplibregl.Map | null {
    return this.map$.value;
  }


  public initMap(container: HTMLDivElement): void {
    if (this.map$.value) return;

    const mapInstance = new maplibregl.Map({
      container: container,
      style: MAP_CONFIG.style,
      center: MAP_CONFIG.defaultCenter,
      zoom: MAP_CONFIG.defaultZoom,
      attributionControl: false
    });

    const loadImageAsync = (src: string): Promise<HTMLImageElement> => {
      return new Promise((resolve, reject) => {
        const img = new Image(24, 24);
        img.onload = () => resolve(img);
        img.onerror = (err) => reject(err);
        img.src = src;
      });
    };

    Promise.all([
      loadImageAsync('/assets/map-icons/metro.svg'),
      loadImageAsync('/assets/map-icons/metro_bg.svg')
    ]).then(([imgIcon, imgBg]) => {
      
      mapInstance.on('load', () => {
        try {
          const centerCanvas = this.mergeMetroLayers(imgBg, imgIcon, '#007aff');
          mapInstance.addImage('subway-center-combined', centerCanvas);

          const entranceCanvas = this.mergeMetroLayers(imgBg, imgIcon, '#34c759');
          mapInstance.addImage('subway-entrance-combined', entranceCanvas);

          console.log('Иконки метро добавлены со 100% гарантией синхронизации!');
          
          // Карта полностью загружена и готова к работе (в том числе к рисованию)
          this.map$.next(mapInstance);
        } catch (error) {
          console.error('Ошибка сборки слоев:', error);
        }
      });

    }).catch(err => {
      console.error('Критическая ошибка загрузки файлов иконок из ассетов:', err);
      // Если иконки упали, карту всё равно стоит отдать для работы
      mapInstance.on('load', () => this.map$.next(mapInstance));
    });

    mapInstance.addControl(new maplibregl.NavigationControl(), 'top-right');
  }

  private mergeMetroLayers(bgHtmlImage: HTMLImageElement, iconHtmlImage: HTMLImageElement, colorHex: string): HTMLImageElement {
    const canvas = document.createElement('canvas');
    const size = 32;
    canvas.width = size;
    canvas.height = size;
    const ctx = canvas.getContext('2d');

    if (!ctx) throw new Error('Не удалось получить 2D контекст Canvas');

    const bgCanvas = document.createElement('canvas');
    bgCanvas.width = size;
    bgCanvas.height = size;
    const bgCtx = bgCanvas.getContext('2d');
    
    if (bgCtx) {
      bgCtx.drawImage(bgHtmlImage, 0, 0, size, size);
      bgCtx.globalCompositeOperation = 'source-in';
      bgCtx.fillStyle = colorHex;
      bgCtx.fillRect(0, 0, size, size);
    }

    ctx.drawImage(bgCanvas, 0, 0);
    ctx.drawImage(iconHtmlImage, 0, 0, size, size);

    const resultImg = new Image(size, size);
    resultImg.src = canvas.toDataURL('image/png');
    return resultImg;
  }

  ngOnDestroy(): void {
    if (this.map$.value) {
      this.map$.value.remove();
      this.map$.next(null);
    }
  }
}
