using System;
using System.Collections.Generic;
using Game._Script.Cells;
using Game._Script.Stages;
using UnityEngine;

namespace Game._Script.Manager
{
    public class GridManager : MonoBehaviour
    {
        public StageConfig stageConfig;
        
        [Header("Board Settings")]
        private int _rows;
        private int _columns;
        public float cellSize;

        public GameObject cellPrefab;

        private Cell[,] _gridCells;

        public void Initialize()
        {
            _rows = stageConfig.rows;
            _columns = stageConfig.columns;
            CreateBoardGrid();
        }

        private void CreateBoardGrid()
        {
            _gridCells = new Cell[_rows, _columns];
            Vector3 gridOrigin = transform.position - 
                                 new Vector3((_columns - 1) * cellSize * 0.5f,
                                     (_rows - 1) * cellSize * 0.5f,
                                     0f);    
            
            for (var row = 0; row < _rows; row++)
            {
                for (var col = 0; col < _columns; col++)
                {
                    Vector3 cellPosition = gridOrigin + new Vector3(col * cellSize, row * -cellSize, 0);
                    GameObject  cellGo= Instantiate(cellPrefab, cellPosition, Quaternion.identity, transform);

                    cellGo.name = "Cell" + row + "_" + col;

                    Cell cell = cellGo.GetComponent<Cell>();
                    cell.position = new Vector2Int(col, row);
                    cell.isOccupied = false;
                    _gridCells[row, col] = cell;
                }   
            }
        }

        public  List<Cell> GetAllCells()
        {
            var list = new List<Cell>(_rows * _columns);
            foreach (var cell in _gridCells)
            {
                list.Add(cell);
            }

            return list;
        }
        
        public List<Cell> GetAvailableCells()
        {
            List<Cell> available = new List<Cell>();
            foreach (var cell in _gridCells)
            {
                if (!cell.isOccupied)
                {
                    available.Add(cell);
                }
            }
            return available;
        }

        public Cell GetCellAt(Vector2Int pos)
        {
            if (pos.y >= 0 && pos.y < _rows && pos.x >= 0 && pos.x < _columns)
            {
                return _gridCells[pos.y, pos.x];
            }

            return null;
        }
    }
}
