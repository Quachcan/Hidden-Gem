using System;
using Game._Script.Cells;
using Game._Script.Manager;
using UnityEngine;

namespace Game._Script.Rock
{
    public class RockTile : MonoBehaviour
    {
        public static event Action<Vector2Int> OnRockDug; 
        
        public Vector2Int gridPosition;
        private bool _isOccupied;
        public bool IsOccupied => _isOccupied;
        private bool _isDug;
        
        [SerializeField]
        private Cell _cell;

        private void Awake()
        {
            _cell = GetComponentInParent<Cell>();
        }

        public void MarkOccupied()
        {
            _isOccupied = true;
        }

        public static void RaiseOnRockDug(Vector2Int pos)
        {
            OnRockDug?.Invoke(pos);
        }

        private void OnMouseDown()
        {
            if(GameManager.instance.IsPaused) return;
            if (!GameManager.instance.pickaxeManager.TryUsePickaxe())
            {
                GameManager.instance.uiManager.HandleOutOfPickaxe();
                return;
            }
            
            _cell.MarkCellRevealed();
            _isDug = true;
            OnRockDug?.Invoke(gridPosition);
            Destroy(gameObject);
        }
    }
}
