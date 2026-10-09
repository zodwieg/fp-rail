import { Injectable } from '@angular/core';
import { from, Observable } from 'rxjs';
import { invoke } from '@tauri-apps/api/core';

@Injectable({
  providedIn: 'root'
})
export class NodeApiService {
  /**
   * Просто кидаем любую фичу транзитом в Rust без лишней типизации
   */
  public sendPoint(feature: any): Observable<any> {
    return from(invoke<any>('create_tram_point', { feature }));
  }
}
