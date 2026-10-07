import { Component } from '@angular/core';
import { MapDashboardComponent } from './features/map-dashboard/map-dashboard.component'; 

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  imports: [MapDashboardComponent],
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
}
