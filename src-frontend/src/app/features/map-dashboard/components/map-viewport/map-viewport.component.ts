import { Component, ElementRef, AfterViewInit, ViewChild, inject } from '@angular/core';
import { MapService } from '../../services/map-service';

@Component({
  selector: 'fp-map-viewport',
  standalone: true,
  templateUrl: './map-viewport.component.html',
  styleUrls: ['map-viewport.component.scss']
})
export class MapViewportComponent implements AfterViewInit {
  @ViewChild('mapContainer') mapContainer!: ElementRef<HTMLDivElement>;
  
  private mapService = inject(MapService);

  ngAfterViewInit(): void {
    this.mapService.initMap(this.mapContainer.nativeElement);
  }
}
