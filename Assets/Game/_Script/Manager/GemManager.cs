using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Game._Script.Cells;
using Game._Script.Gems;
using Game._Script.Rock;
using Game._Script.Stages;

namespace Game._Script.Manager
{

    public class GemManager : MonoBehaviour
    {
        [Header("Stage Configuration")]
        public StageConfig stageConfig;

        [Header("References")]
        [SerializeField] private GridManager gridManager;
        
        [SerializeField] private List<GameObject> gemPrefabs;

        private readonly List<HiddenGem> _hiddenGems = new List<HiddenGem>();

        public event Action AllGemsCollected;
        
        private struct Placement
        {
            public GemConfig   Config;
            public Vector2Int  Anchor;
            public bool        Rotated;
        }

        private class HiddenGem
        {
            public GemConfig        Config;
            public List<Vector2Int> Footprint;
            public Gem              Instance;       
            public int              RevealedCount;
            public bool             Collected;
        }

        private void OnEnable() => RockTile.OnRockDug += HandleRockDug;
        private void OnDisable() => RockTile.OnRockDug -= HandleRockDug;

        public void Initialize(StageConfig config)
        {
            stageConfig = config;
            gridManager = GameManager.instance.gridManager;
            SpawnAllGems();
        }
        
        private void SpawnAllGems()
        {
            var placements = ComputePlacements();
            _hiddenGems.Clear();
            foreach (var p in placements)
            {
                var prefab = GetGemPrefabById(p.Config.gemId);
                if (prefab == null)
                    continue;
                
                int w = p.Rotated ? p.Config.height : p.Config.width;
                int h = p.Rotated ? p.Config.width  : p.Config.height;
                var footprint = new List<Vector2Int>();
                Vector3 sum = Vector3.zero;
                
                for (int dy = 0; dy < h; dy++)
                for (int dx = 0; dx < w; dx++)
                {
                    var pos = p.Anchor + new Vector2Int(dx, dy);
                    footprint.Add(pos);
                    sum += gridManager.GetCellAt(p.Anchor + new Vector2Int(dx, dy)).transform.position;
                }
                Vector3 center = sum / footprint.Count;
                
                var go = Instantiate(prefab, center, Quaternion.identity, transform);
                if (p.Rotated)
                    go.transform.Rotate(0f, 0f, 90f);

                if (go.TryGetComponent<SpriteRenderer>(out var sr))
                {
                    sr.sortingLayerName = "GemFG";
                    sr.sortingOrder     = 0;
                }

                var gem = go.GetComponent<Gem>();
                gem.InitializeGem();
                gem.GemPlaced();
                _hiddenGems.Add(new HiddenGem
                {
                    Instance = gem,
                    Footprint = footprint,
                    RevealedCount = 0,
                    Collected = false
                });
                
            }
        }

        private void HandleRockDug(Vector2Int gridPos)
        {
            foreach (var hiddenGem in _hiddenGems)
            {
                if(hiddenGem.Collected) continue;
                if(!hiddenGem.Footprint.Contains(gridPos)) continue;

                hiddenGem.RevealedCount++;
                if (hiddenGem.RevealedCount >= hiddenGem.Footprint.Count)
                {
                    hiddenGem.Instance.GemReveal();
                    hiddenGem.Instance.GemCollected();
                    hiddenGem.Collected = true;

                    if (_hiddenGems.All(x => x.Collected))
                    {
                        AllGemsCollected?.Invoke();
                    }
                }
                break;
            }
        }
        
        private List<Placement> ComputePlacements()
        {
            int rows = stageConfig.rows;
            int cols = stageConfig.columns;
            bool[,] occupied = new bool[rows, cols];
            
            var gemList = new List<GemConfig>();
            foreach (var entry in stageConfig.gemConfig)
            {
                var prefab = GetGemPrefabById(entry.gemTypeId);
                if (prefab == null)
                {
                    Debug.LogError($"No prefab for gemId {entry.gemTypeId}");
                    continue;
                }
                var cfg = prefab.GetComponent<Gem>().config;
                for (int i = 0; i < entry.count; i++)
                    gemList.Add(cfg);
            }
            for (int i = 0; i < gemList.Count; i++)
            {
                int j = UnityEngine.Random.Range(i, gemList.Count);
                (gemList[i], gemList[j]) = (gemList[j], gemList[i]);
            }

            var placements = new List<Placement>();
            
            foreach (var cfg in gemList)
            {
                var candidates = new List<Placement>();
                foreach (bool rot in new[] { false, true })
                {
                    int w = rot ? cfg.height : cfg.width;
                    int h = rot ? cfg.width  : cfg.height;
                    for (int y = 0; y <= rows - h; y++)
                    {
                        for (int x = 0; x <= cols - w; x++)
                        {
                            bool ok = true;
                            for (int dy = 0; dy < h && ok; dy++)
                                for (int dx = 0; dx < w; dx++)
                                    if (occupied[y + dy, x + dx]) ok = false;
                            if (!ok) continue;
                            candidates.Add(new Placement { Config = cfg, Anchor = new Vector2Int(x, y), Rotated = rot });
                        }
                    }
                }

                if (candidates.Count == 0)
                {
                    Debug.LogError($"Cannot place gemId={cfg.gemId}: no space");
                    continue;
                }
                
                var choice = candidates[UnityEngine.Random.Range(0, candidates.Count)];
                int wC = choice.Rotated ? choice.Config.height : choice.Config.width;
                int hC = choice.Rotated ? choice.Config.width  : choice.Config.height;

                for (int dy = 0; dy < hC; dy++)
                    for (int dx = 0; dx < wC; dx++)
                        occupied[choice.Anchor.y + dy, choice.Anchor.x + dx] = true;

                placements.Add(choice);
            }

            return placements;
        }
        
        private GameObject GetGemPrefabById(int id)
        {
            return gemPrefabs.FirstOrDefault(pf => pf.GetComponent<Gem>().config.gemId == id);
        }

        public HashSet<Vector2Int> GetAllGemFootPrint()
        {
            return new HashSet<Vector2Int>(
                _hiddenGems.Where(h => !h.Collected)
                                    .SelectMany(h => h.Footprint)
                );
        }
    }
}
