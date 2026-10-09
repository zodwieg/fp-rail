import { Injectable, inject, OnDestroy } from '@angular/core';
import { MapService } from './map-service';
import { Subscription } from 'rxjs';
import { TerraDraw, TerraDrawPolygonMode, TerraDrawLineStringMode, TerraDrawPointMode } from 'terra-draw';
import { TerraDrawMapLibreGLAdapter } from 'terra-draw-maplibre-gl-adapter';
import { DrawingEventHandlerService } from './drawing-event-handler.service';
import { getSelectModeConfig } from './configs/terradraw-select.config';

export type DrawingMode = 'static' | 'polygon' | 'linestring' | 'point' | 'select';

@Injectable({
  providedIn: 'root'
})
export class DrawingService implements OnDestroy {
  private mapService = inject(MapService);
  private eventHandler = inject(DrawingEventHandlerService);
  
  private drawInstance: TerraDraw | null = null;
  private mapSubscription: Subscription;

  constructor() {
    this.mapSubscription = this.mapService.mapReady$.subscribe((map) => {
      this.initTerraDraw(map);
    });
  }

  private initTerraDraw(map: any): void {
    this.drawInstance = new TerraDraw({
      adapter: new TerraDrawMapLibreGLAdapter({ map }),
      modes: [
        getSelectModeConfig(), // Чисто и лаконично
        new TerraDrawPolygonMode(),
        new TerraDrawLineStringMode(),
        new TerraDrawPointMode()
      ]
    });

    this.drawInstance.start();
    this.setMode('static');

    // Подписка на окончание рисования делегируется специализированному сервису
    this.drawInstance.on('finish', (id) => {
      const snapshot = this.drawInstance?.getSnapshot();
      const finishedFeature = snapshot?.find((f: any) => f.id === id);
      this.eventHandler.handleFeatureFinish(finishedFeature);
    });
  }

  public setMode(mode: DrawingMode): void {
    this.drawInstance?.setMode(mode);
  }

  public clear(): void {
    this.drawInstance?.clear();
  }

  ngOnDestroy(): void {
    this.mapSubscription.unsubscribe();
    if (this.drawInstance) {
      this.drawInstance.stop();
      this.drawInstance = null;
    }
  }
}
