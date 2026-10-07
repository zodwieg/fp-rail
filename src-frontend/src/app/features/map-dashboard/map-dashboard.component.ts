import { Component } from '@angular/core';
import { MapViewportComponent } from './components/map-viewport/map-viewport.component';

@Component({
  selector: 'fp-map-dashboard',
  standalone: true,
  imports: [MapViewportComponent],
  templateUrl: './map-dashboard.component.html',
  styleUrls: ["map-dashboard.component.scss"]
})
export class MapDashboardComponent {}
