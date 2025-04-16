using System;
using System.Collections.Generic;
using Game._Script.Cells;
using UnityEngine;

namespace Game._Script.Manager
{
    public class GridManager : MonoBehaviour
    {
        [Header("Board Settings")]
        public int rows;
        public int columns;
        public float cellSize;

        public GameObject cellPrefab;

        private Cell[,] _gridCells;

        public void Initialize()
        {
            CreateBoardGrid();
        }

        private void CreateBoardGrid()
        {
            _gridCells = new Cell[rows, columns];
            Vector3 gridOrigin = transform.position - new Vector3((columns - 1) * cellSize / 2f, (rows - 1) * cellSize / 2f, 0);    
            
            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < columns; col++)
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
            if (pos.y >= 0 && pos.y < rows && pos.x >= 0 && pos.x < columns)
            {
                return _gridCells[pos.y, pos.x];
            }

            return null;
        }
    }
}
