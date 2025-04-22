using System;
using Game._Script.Cells;
using Game._Script.Manager;
using Game._Script.Rock;
using UnityEngine;

namespace Game._Script.Dynamite
{
    public class Dynamite : MonoBehaviour
    {
        public Vector2Int gridPosition;

        private GridManager _gridManager;
        private Cell _cell;
        private bool _isDug;

        private const int ExplosionRadius = 1;

        private void Awake()
        {
            _gridManager = GameManager.instance.gridManager;
            _cell = GetComponentInParent<Cell>();
        }

        private void OnEnable()
        {
            RockTile.OnRockDug += HandleRockDug;
        }

        private void OnDisable()
        {
            RockTile.OnRockDug -= HandleRockDug;
        }

        private void HandleRockDug(Vector2Int pos)
        {
            if(pos != gridPosition) return;
            
            _cell.MarkCellRevealed();

            Explode();
        }

        private void Explode()
        {
            for (int dy = -ExplosionRadius; dy <= ExplosionRadius; dy++)
            {
                for (int dx = -ExplosionRadius; dx <= ExplosionRadius; dx++)
                {
                    var p = new Vector2Int(gridPosition.x + dx, gridPosition.y + dy);
                    var rock = _cell.GetComponentInChildren<RockTile>();
                    if(rock != null)
                        Destroy(rock.gameObject);

                    RockTile.RaiseOnRockDug(p);
                }
            }
            Debug.Log("Explode");
        }
    }
}
