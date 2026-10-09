import { TerraDrawSelectMode } from 'terra-draw';

/**
 * Фабрика для получения чистой конфигурации Select режима TerraDraw
 */
export function getSelectModeConfig(): TerraDrawSelectMode {
  return new TerraDrawSelectMode({
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
  });
}
