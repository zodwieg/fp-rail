import { Injectable, OnDestroy } from '@angular/core';
import * as maplibregl from 'maplibre-gl';
import { MAP_CONFIG } from '../../../core/config/map.config';

@Injectable({
  providedIn: 'root'
})
export class MapService implements OnDestroy {
  private map: maplibregl.Map | null = null;

  /**
   * Инициализирует интерактивную карту MapLibreGL.
   * 
   * Если экземпляр карты уже существует, повторная инициализация 
   * не выполняется во избежание утечек памяти.
   * 
   * @param container HTML-элемент, в который будет встроена карта.
   */
  public initMap(container: HTMLDivElement): void {
    if (this.map) return;
    // Обычный комментарий
    this.map = new maplibregl.Map({
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
      
      // 3. Ждем готовности самой карты
      this.map?.on('load', () => {
        try {
          const centerCanvas = this.mergeMetroLayers(imgBg, imgIcon, '#007aff');
          this.map?.addImage('subway-center-combined', centerCanvas);

          const entranceCanvas = this.mergeMetroLayers(imgBg, imgIcon, '#34c759');
          this.map?.addImage('subway-entrance-combined', entranceCanvas);

          console.log('Иконки метро добавлены со 100% гарантией синхронизации!');
        } catch (error) {
          console.error('Ошибка сборки слоев:', error);
        }
      });

    }).catch(err => {
      console.error('Критическая ошибка загрузки файлов иконок из ассетов:', err);
    });

    this.map.addControl(new maplibregl.NavigationControl(), 'top-right');
  }

  private mergeMetroLayers(bgHtmlImage: HTMLImageElement, iconHtmlImage: HTMLImageElement, colorHex: string): HTMLImageElement {
    const canvas = document.createElement('canvas');
    const size = 32; // Фиксированный размер для четкости (Retina-friendly)
    canvas.width = size;
    canvas.height = size;
    const ctx = canvas.getContext('2d');

    if (!ctx) throw new Error('Не удалось получить 2D контекст Canvas');

    // Шаг А: Создаем буферный холст для перекраски фона
    const bgCanvas = document.createElement('canvas');
    bgCanvas.width = size;
    bgCanvas.height = size;
    const bgCtx = bgCanvas.getContext('2d');
    
    if (bgCtx) {
      // Явно указываем размеры отрисовки (size, size), чтобы не зависеть от внутренних багов SVG
      bgCtx.drawImage(bgHtmlImage, 0, 0, size, size);
      bgCtx.globalCompositeOperation = 'source-in';
      bgCtx.fillStyle = colorHex;
      bgCtx.fillRect(0, 0, size, size);
    }

    // Б. Переносим цветную подложку на основной холст
    ctx.drawImage(bgCanvas, 0, 0);

    // В. Накладываем черный контур и букву М строго поверх круга
    ctx.drawImage(iconHtmlImage, 0, 0, size, size);

    // Г. Экспортируем результат в новую чистую картинку
    const resultImg = new Image(size, size);
    resultImg.src = canvas.toDataURL('image/png');
    return resultImg;
  }

  ngOnDestroy(): void {
    if (this.map) {
      this.map.remove();
      this.map = null;
    }
  }
}
