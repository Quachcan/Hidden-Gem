using System;
using System.Collections.Generic;
using Game._Script.Cells;
using Game._Script.Gems;
using Game._Script.Rock;
using Game._Script.Stages;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game._Script.Manager
{
    public class GemManager : MonoBehaviour
    {
        public StageConfig stageConfig;
        private GridManager _gridManager;
        
        [SerializeField]
        private List<GameObject> gemPrefabs;
        private List<Gem> _activeGem = new List<Gem>();

        private Queue<GemConfig> _gemQueue;
        private int _gemRemaining;

        private void OnEnable()
        {
            //RockTile.OnRockDug += 
        }

        private void OnDisable()
        {
            
        }


        public void Initialize()
        {
            _gridManager = GameManager.Instance.gridManager;
            var list = new List<GemConfig>();
            foreach (var config in stageConfig.gemConfig)
            {
                for (int i = 0; i < config.count; i++)
                {
                    //list.Add(GetGemPrefabById(config.gemTypeId));
                }
            }
            //Shuffle(list);
            _gemQueue = new Queue<GemConfig>(list);
            _gemRemaining = _gemQueue.Count;
            PlaceGamesOnBoard();
        }

        private void HandleRockDug(Vector2Int gridPos)
        {
            if(_gemQueue.Count == 0) return;

            var nextGem = _gemQueue.Peek();

            var gems = new List<(Vector2Int anchor, bool rot)>();
            foreach (var rotation in new[] {false, true})
            {
                int w = rotation ? nextGem.height : nextGem.width;
                int h = rotation ? nextGem.width : nextGem.height;
                for (int y = 0; y < _gridManager.stageConfig.rows - h; y++)
                for (int x = 0; x < _gridManager.stageConfig.columns - w; x++)
                {
                    var start = new Vector2Int(x, y);
                    // if (RegionFits(start, w, h, mustContain: null)) 
                    //     gems.Add((start, rotation));
                }
            }

            if (gems.Count == 1)
            {
                var (anchor, rotation) = gems[0];
                int w = rotation ? nextGem.height : nextGem.width;
                int h = rotation ? nextGem.width : nextGem.height;
                if (gridPos.x >= anchor.x && gridPos.x < anchor.x + w &&
                    gridPos.y >= anchor.y && gridPos.y < anchor.y + h)
                {
                    _gemQueue.Dequeue();
                    _gemRemaining--;
                    //SpawnGemAt(nextGem, anchor, rot);
                    return;
                }
            }

            if (Random.value >= 0.3f) return;

            nextGem = _gemQueue.Dequeue();
            _gemRemaining--;
            TryPlaceGemUnder(nextGem, gridPos);
        }

        private void TryPlaceGemUnder(GemConfig config, Vector2Int dugPos)
        {
            foreach (var rotation in new[] {false, true})
            {
                int w = rotation ? config.height : config.width;
                int h = rotation ? config.width  : config.height;
                for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    var anchor = dugPos - new Vector2Int(x, y);
                    // if (RegionFits(anchor, w, h, mustContain: dugPos))
                    // {
                    //     SpawnGemAt(config, anchor, rotation);
                    //     return;
                    // }
                }
            }
            Debug.LogError($"[GemManager] cannot place gem {config.gemId} at {dugPos}");
        }

        private bool IsRegionFits(Vector2Int anchor, int w, int h, Vector2Int? mustContain)
        {
            if (anchor.x < 0 || anchor.y < 0 || anchor.x + w > _gridManager.stageConfig.columns ||
                anchor.y + h > _gridManager.stageConfig.rows)
                return false;
            if (mustContain.HasValue)
            {
                var p = mustContain.Value;
                if (p.x < anchor.x || p.x >= anchor.x + w ||
                    p.y < anchor.y || p.y >= anchor.y + h)
                    return false;
            }

            for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                var c = _gridManager.GetCellAt(anchor + new Vector2Int(x, y));
                if (c == null || c.isDig)
                    return false;
            }

            return true;
        }

        private void SpawnGemAt(GemConfig config, Vector2Int anchor, bool rotation)
        {
            int w = rotation ? config.height : config.width;
            int h = rotation ? config.width  : config.height;
            var footprint = new List<Cell>();
            for (int dy = 0; dy < h; dy++)
            for (int dx = 0; dx < w; dx++)
            {
                var cell = _gridManager.GetCellAt(anchor + new Vector2Int(dx, dy));
                cell.isDig = true;
                footprint.Add(cell);
            }
            var center = ComputeSpawnCenter(footprint);
            var prefab = GetGemPrefabById(config.gemId);
            var go = Instantiate(prefab, center, Quaternion.identity, transform);
            if (rotation) go.transform.Rotate(0, 0, 90);
            if (go.TryGetComponent<SpriteRenderer>(out var sr))
            {
                sr.sortingLayerName = "GemFG";
                sr.sortingOrder     = 0;
            }
            var gemComp = go.GetComponent<Gem>();
            gemComp.InitializeGem();
            gemComp.GemPlaced();
            gemComp.isRevealed = true;
        }

        private static Vector3 ComputeSpawnCenter(IList<Cell> cells)
        {
            var sum = Vector3.zero;
            foreach (var cell in cells)
            {
                sum += cell.transform.position;
            }

            return sum / cells.Count;
        }
        
        private static void Shuffle<T>(IList<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int j = Random.Range(i, list.Count);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
        
        
        
        private void ShuffleCells(List<Cell> cells)
        {
            for (int i = 0; i < cells.Count; i++)
            {
                int j = Random.Range(i, cells.Count);
                (cells[i], cells[j]) = (cells[j], cells[i]);
            }
        }

        private bool TryPlaceGemOnBoard(GameObject prefab, List<Cell> availableCells)
        {
            foreach (var cell in availableCells)
            {
                if(!CanPlaceGem(cell, prefab)) continue;

                var gemCells = GetCellsOccupied(cell, prefab);
                if(gemCells == null || gemCells.Count == 0) continue;

                MarkOccupied(gemCells);

                var spawnPos = ComputeSpawnPosition(cell, prefab.GetComponent<Gem>().config);
                var gem = InstantiateGem(prefab, spawnPos, cell.position.y);
                gem.InitializeGem();
                gem.GemPlaced();
                _activeGem.Add(gem);

                return true;
            }

            return false;
        }
        
        private void MarkOccupied(List<Cell> gemCells)
        {
            foreach (var c in gemCells)
                c.isOccupied = true;
        }
        
        private Vector3 ComputeSpawnPosition(Cell anchor, GemConfig cfg)
        {
            float cs      = _gridManager.cellSize;
            float offX    = ((cfg.width  - 1) * cs) / 2f;
            float offY    = ((cfg.height - 1) * cs) / 2f;
            return anchor.transform.position + new Vector3(offX, offY - cs, 0);
        }

        private Gem InstantiateGem(GameObject prefab, Vector3 pos, float anchorY)
        {
            var gem = Instantiate(prefab, pos, Quaternion.identity, transform);
            var sr = gem.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingLayerName = "GemFG";
                sr.sortingOrder = Mathf.RoundToInt(-anchorY);
            }

            return gem.GetComponent<Gem>();
        }

        private GameObject GetGemPrefabById(int gemType)
        {
            foreach (var prefab in gemPrefabs)
            {
                Gem gemComponent = prefab.GetComponent<Gem>();
                if (gemComponent != null && gemComponent.config != null)
                {
                    if (gemComponent.config.gemId == gemType)
                    {
                        return prefab;
                    }
                }
            }
            return null;
        }
        
        private void PlaceGamesOnBoard()
        {
            var availableCells = _gridManager.GetAvailableCells();
            ShuffleCells(availableCells);

            foreach (var cfg in stageConfig.gemConfig)
            {
                for (int i = 0; i < cfg.count; i++)
                {
                    var prefab = GetGemPrefabById(cfg.gemTypeId);
                    if (prefab == null)
                    {
                        Debug.LogError($"Cannot find prefab for gem type {cfg.gemTypeId}");
                        continue;
                    }

                    if (!TryPlaceGemOnBoard(prefab, availableCells))
                        Debug.LogError($"Cannot place gem type {cfg.gemTypeId} #{i+1}");
                }
            }
        }

        private bool CanPlaceGem(Cell startCell, GameObject gemPrefab)
        {
            Gem gem = gemPrefab.GetComponent<Gem>();
            if (gem == null || gem.config == null)
            {
                return false;
            }

            int gemWidth = gem.config.width;
            int gemHeight = gem.config.height;
            Vector2Int startPos = startCell.position;

            // if (startPos.x + gemWidth > gridManager.columns || startPos.y + gemHeight > gridManager.rows)
            // {
            //     return false;
            // }

            for (int y = 0; y < gemHeight; y++)
            {
                for (int x = 0; x < gemWidth; x++)
                {
                    Vector2Int checkPos = new Vector2Int(startPos.x + x, startPos.y + y);
                    Cell checkCell = _gridManager.GetCellAt(checkPos);
                    if (checkCell == null || checkCell.isOccupied)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private List<Cell> GetCellsOccupied(Cell startCell, GameObject gemPrefab)
        {
            Gem gem = gemPrefab.GetComponent<Gem>();
            if (gem == null || gem.config == null) return null;

            int gemWidth = gem.config.width;
            int gemHeight = gem.config.height;

            List<Cell> cells = new List<Cell>();
            Vector2Int startPos = startCell.position;

            for (int y = 0; y < gemHeight; y++)
            {
                for (int x = 0; x < gemWidth; x++)
                {
                    Vector2Int pos = new Vector2Int(startPos.x + x, startPos.y + y);
                    Cell cell = _gridManager.GetCellAt(pos);
                    if (cell != null)
                    {
                        cells.Add(cell);
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            return cells;
        }
    }
}
