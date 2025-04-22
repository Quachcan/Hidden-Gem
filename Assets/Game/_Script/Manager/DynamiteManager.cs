using System.Collections.Generic;
using System.Linq;
using Game._Script.Stages;
using UnityEngine;

namespace Game._Script.Manager
{
    public class DynamiteManager : MonoBehaviour
    {
        [SerializeField] private GameObject dynamitePrefab;

        private GridManager _gridManager;
        private HashSet<Vector2Int> _dynamitePositions = new HashSet<Vector2Int>();

        public void Initialize(StageConfig config)
        {
            _gridManager = GameManager.instance.gridManager;
            _dynamitePositions.Clear();
            
            var gemCells = GameManager.instance.gemManager.GetAllGemFootPrint();
            
            var candidates = _gridManager.GetAllCells()
                .Where(cell => !gemCells.Contains(cell.position))
                .ToList();
            
            for (int i = 0; i < candidates.Count; i++)
            {
                int j = Random.Range(i, candidates.Count);
                (candidates[i], candidates[j]) = (candidates[j], candidates[i]);
            }
            
            int toSpawn = Mathf.Min(config.dynamite, candidates.Count);
            for (int k = 0; k < toSpawn; k++)
            {
                var cell = candidates[k];
                var pos = cell.position;
                _dynamitePositions.Add(pos);
                
                Debug.Log($"[DynamiteManager] Spawning dynamite at cell {pos}");
                var dyn = Instantiate(dynamitePrefab, cell.transform, false);
                dyn.transform.localPosition = Vector3.zero;
                dyn.transform.localRotation = Quaternion.identity;
                
                if (dyn.TryGetComponent<SpriteRenderer>(out var sr))
                {
                    float cellSize = _gridManager.cellSize;
                    Vector2 spriteSize = sr.sprite.bounds.size;
                    Vector3 parentScale = cell.transform.lossyScale;
                    float sX = cellSize / (spriteSize.x * parentScale.x);
                    float sY = cellSize / (spriteSize.y * parentScale.y);
                    dyn.transform.localScale = new Vector3(sX, sY, 1f);
                }
                
                if (dyn.TryGetComponent<Dynamite.Dynamite>(out var dt))
                {
                    dt.gridPosition = pos;
                }
            }
        }
    }
}
