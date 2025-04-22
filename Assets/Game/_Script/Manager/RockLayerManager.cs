using System.Collections.Generic;
using Game._Script.Cells;
using Game._Script.Rock;
using UnityEngine;

namespace Game._Script.Manager
{
    public class RockLayerManager : MonoBehaviour
    {
        [Header("Rock Tile Prefab")]
        [SerializeField] private GameObject rockPrefab;
        
        public void BuildRockLayer()
        {
            var cells    = GameManager.instance.gridManager.GetAllCells();
            float cellSize = GameManager.instance.gridManager.cellSize;

            foreach (var cell in cells)
            {
                var rockGo = Instantiate(rockPrefab, cell.transform, true);
                
                rockGo.transform.localPosition = Vector3.zero;
                rockGo.transform.localRotation = Quaternion.identity;
                
                if (rockGo.TryGetComponent<SpriteRenderer>(out var sr))
                {
                    Vector2 spriteSize = sr.sprite.bounds.size;
                    // Account for parent (cell) scale: worldSize = localScale * spriteSize * parentScale
                    // So localScale = cellSize / (spriteSize * parentScale)
                    Vector3 parentScale = cell.transform.lossyScale;
                    float localScaleX = cellSize / (spriteSize.x * parentScale.x);
                    float localScaleY = cellSize / (spriteSize.y * parentScale.y);
                    rockGo.transform.localScale = new Vector3(localScaleX, localScaleY, 1f);
                }
                
                var tile = rockGo.GetComponent<RockTile>();
                if (tile != null)
                {
                    tile.gridPosition = cell.position;
                }
                else
                {
                    Debug.LogWarning("RockPrefab thiếu component RockTile");
                }
            }
        }
    }
}