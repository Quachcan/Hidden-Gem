using System;
using System.Collections.Generic;
using Game._Script.Cells;
using Game._Script.Gems;
using Game._Script.Stages;
using UnityEngine;

namespace Game._Script.Manager
{
    public class GemManager : MonoBehaviour
    {
        public StageConfig stageConfig;
        [SerializeField]
        private GridManager gridManager;
        
        [SerializeField]
        private List<GameObject> gemPrefabs;
        private List<Gem> _activeGem = new List<Gem>();
        

        public void Initialize()
        {
            gridManager = GameManager.Instance.gridManager;
            PlaceGamesOnBoard();
        }

        private void PlaceGamesOnBoard()
        {
            List<Cell> availableCells = GameManager.Instance.gridManager.GetAvailableCells();

            foreach (var gemConfig in stageConfig.gemConfig)
            {
                int gemTypeId = gemConfig.gemTypeId;
                int count = gemConfig.count;

                for (int i = 0; i < count; i++)
                {
                    GameObject prefab = GetGemPrefabById(gemTypeId);
                    if (prefab == null)
                    {
                        Debug.LogError("Cannot found prefab for gem type: " + gemTypeId );
                        continue;
                    }

                    bool placed = false;

                    foreach (var cell in availableCells)
                    {
                        if (CanPlaceGem(cell, prefab))
                        {
                            List<Cell> gemCells = GetCellsOccupied(cell, prefab);
                            if(gemCells == null || gemCells.Count == 0)
                                continue;
                            foreach (var gemCell in gemCells)
                            {
                                gemCell.isOccupied = true;
                            }

                            float cellSize = gridManager.cellSize;

                            Gem gem = prefab.GetComponent<Gem>();

                            float gemWidthInCell = gem.config.width;
                            float gemHeightInCell = gem.config.height;

                            float offsetX = ((gemWidthInCell - 1) * cellSize) / 2f;
                            float offsetY = ((gemHeightInCell - 1) * cellSize) / 2f;

                            Vector3 spawnPosition = cell.transform.position + new Vector3(offsetX, offsetY - cellSize, 0);

                            Debug.Log("Spawning gem at: " + spawnPosition + " from anchor cell: " + cell.transform.position);
                            
                            GameObject gemInstance = Instantiate(prefab, spawnPosition, Quaternion.identity, transform);

                            Gem gemComponent = gemInstance.GetComponent<Gem>();
                            if (gemComponent != null)
                            {
                                gemComponent.InitializeGem();
                                gemComponent.GemPlaced();
                                _activeGem.Add(gemComponent);
                            }

                            placed = true;
                            break;
                        }
                    }

                    if (!placed)
                    {
                        Debug.LogError("Cannot place gem type " + gemTypeId + "number" + (i +1));
                    }
                }
            }
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

            if (startPos.x + gemWidth > gridManager.columns || startPos.y + gemHeight > gridManager.rows)
            {
                return false;
            }

            for (int y = 0; y < gemHeight; y++)
            {
                for (int x = 0; x < gemWidth; x++)
                {
                    Vector2Int checkPos = new Vector2Int(startPos.x + x, startPos.y + y);
                    Cell checkCell = gridManager.GetCellAt(checkPos);
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
                    Cell cell = gridManager.GetCellAt(pos);
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
