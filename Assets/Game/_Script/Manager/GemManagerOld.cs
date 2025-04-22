using System;
using System.Collections.Generic;
using System.Linq;
using Game._Script.Cells;
using Game._Script.Gems;
using Game._Script.Rock;
using Game._Script.Stages;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game._Script.Manager
{
    public class GemManagerOld : MonoBehaviour
    {
        public StageConfig stageConfig;
        private GridManager _gridManager;
        
        [SerializeField]
        private List<GameObject> gemPrefabs;
        private Queue<GameObject> _gemQueue;

        [Header("Spawn setting")] 
        [SerializeField]
        private float spawnChance = 0.3f;
        [SerializeField]
        private float increaseChance = 0.1f;
        [SerializeField]
        private float _currentSpawnChance;
        private int _failDigs;
        
        private void OnEnable() => RockTile.OnRockDug += HandleRockDug;
        private void OnDisable() => RockTile.OnRockDug -= HandleRockDug;
        
        public void Initialize()
        {
            _gridManager = GameManager.instance.gridManager;
            _gemQueue = new Queue<GameObject>();
            
            foreach (var cfg in stageConfig.gemConfig)
            {
                var prefab = GetGemPrefabById(cfg.gemTypeId);
                for (int i = 0; i < cfg.count; i++)
                    _gemQueue.Enqueue(prefab);
            }

            _currentSpawnChance = spawnChance;
            _failDigs = 0;
        }

        private void HandleRockDug(Vector2Int gridPos)
        {
            if (_gemQueue.Count == 0)
                return;

            var prefab = _gemQueue.Peek();
            var cfg    = prefab.GetComponent<Gem>().config;
            
            var allRegions = FindAllRegions(cfg);
            
            var viable = new List<(Vector2Int anchor, bool rot)>();
            foreach (var region in allRegions)
            {
                if (WouldStayViableAfterPlacing(region.anchor, region.rotation, cfg))
                    viable.Add(region);
            }
            
            if (viable.Count == 1)
            {
                var choice = viable[0];
                _gemQueue.Dequeue();
                SpawnGemAt(prefab, choice.anchor, choice.rot);
                _failDigs = 0;
                _currentSpawnChance = spawnChance;
                return;
            }
            
            if (viable.Count > 0)
            {
                float chance = _currentSpawnChance;
                if (Random.value < chance)
                {
                    _gemQueue.Dequeue();
                    var choice = viable[Random.Range(0, viable.Count)];
                    SpawnGemAt(prefab, choice.anchor, choice.rot);
                }
                

                _currentSpawnChance = Mathf.Min(1f, spawnChance + increaseChance);
            }
        }

        private List<(Vector2Int anchor, bool rotation)> FindAllRegions(GemConfig cfg)
        {
            var list = new List<(Vector2Int anchor, bool)>();
            int rows = stageConfig.rows;
            int columns = stageConfig.columns;
            for (int r = 0; r < 2; r++)
            {
                bool rotation = r == 1;
                
                int w, h;
                if (rotation)
                {
                    w = cfg.height;
                    h = cfg.width;
                }
                else
                {
                    w = cfg.width;
                    h = cfg.height;
                }

                for (int y = 0; y < rows - h; y++)
                for (int x = 0; x < columns - x; x++)
                {
                    var anchor = new Vector2Int(x, y);
                    if (IsRegionFits(anchor, w, h))
                    {
                        list.Add((anchor, rotation));
                    }
                }
            }

            return list;
        }
        
        private bool IsRegionFits(Vector2Int anchor, int w, int h)
        {
            if (anchor.x < 0 || anchor.y < 0 ||
                anchor.x + w > stageConfig.columns ||
                anchor.y + h > stageConfig.rows)
                return false;

            for (int dy = 0; dy < h; dy++)
            for (int dx = 0; dx < w; dx++)
            {
                var cell = _gridManager.GetCellAt(anchor + new Vector2Int(dx, dy));
                if (cell == null || cell.isRevealed)
                    return false;

                var rockTile = cell.GetComponentInChildren<RockTile>();
                if (rockTile.IsOccupied && rockTile != null)
                    return false;
            }
            return true;
        }
        
        private bool WouldStayViableAfterPlacing(Vector2Int anchor, bool rot, GemConfig placedCfg)
        {
            int R = stageConfig.rows, C = stageConfig.columns;

            bool[,] rev = new bool[R,C];
            for(int y=0;y<R;y++) for(int x=0;x<C;x++)
                rev[y,x] = _gridManager.GetCellAt(new Vector2Int(x,y)).isRevealed;
            
            int w = rot? placedCfg.height: placedCfg.width;
            int h = rot? placedCfg.width : placedCfg.height;
            for(int dy=0; dy<h; dy++)
            for(int dx=0; dx<w; dx++)
                rev[anchor.y+dy, anchor.x+dx] = true;
            
            foreach(var pf in _gemQueue.Skip(1))
            {
                var cfg = pf.GetComponent<Gem>().config;
                bool foundSpot = false;
                foreach(bool r in new[]{false,true})
                {
                    int w2 = r? cfg.height:cfg.width;
                    int h2 = r? cfg.width :cfg.height;
                    for(int y=0; y<=R-h2 && !foundSpot; y++)
                    for(int x=0; x<=C-w2 && !foundSpot; x++)
                    {
                        bool ok = true;
                        for(int dy2=0;dy2<h2&&ok;dy2++)
                        for(int dx2=0;dx2<w2;dx2++)
                            if(rev[y+dy2, x+dx2]) { ok=false; break; }
                        if(ok) foundSpot = true;
                    }
                    if(foundSpot) break;
                }
                if(!foundSpot) return false;
            }
            return true;
        }

        private void SpawnGemAt(GameObject pf, Vector2Int anchor, bool rotation)
        {
            var config = pf.GetComponent<Gem>().config;
            
            int w = rotation ? config.height : config.width;
            int h = rotation ? config.width  : config.height;
            var footprint = new List<Cell>();
            for (int dy = 0; dy < h; dy++)
            for (int dx = 0; dx < w; dx++)
            {
                var cell = _gridManager.GetCellAt(anchor + new Vector2Int(dx, dy));
                footprint.Add(cell);

                var rockTile = cell.GetComponentInChildren<RockTile>();
                if (rockTile != null)
                {
                    rockTile.MarkOccupied();
                }
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
            if (sr == null) return gem.GetComponent<Gem>();
            sr.sortingLayerName = "GemFG";
            sr.sortingOrder = Mathf.RoundToInt(-anchorY);

            return gem.GetComponent<Gem>();
        }

        private GameObject GetGemPrefabById(int id)
        {
            foreach (var prefab in gemPrefabs)
            {
                Gem gemComponent = prefab.GetComponent<Gem>();
                if (gemComponent.config != null && gemComponent.config.gemId == id)
                    return prefab;
            }
            return null;
        }
    }
}
