using System;
using System.Collections;
using System.Collections.Generic;
using Game._Script.Cells;
using Game._Script.Manager;
using Game._Script.Rock;
using UnityEngine;

namespace Game._Script.Dynamite
{
    public class Dynamite : MonoBehaviour
    {
        public Vector2Int gridPosition;

        [SerializeField] private LayerMask whatCanBeDestroy;
        
        private GridManager _gridManager;
        private Cell _cell;
        private bool _isDug;
        private bool _hasExploded;

        private const int ExplosionRadius = 1;

        private void Awake()
        {
            _gridManager = GameManager.instance.gridManager;
            _cell = GetComponentInParent<Cell>();
        }

        private void OnEnable()
        {
            RockTile.OnRockDug += HandleRockDug;
            Debug.Log("Event Sign");
        }

        private void OnDisable()
        {
            RockTile.OnRockDug -= HandleRockDug;
        }

        private void HandleRockDug(Vector2Int pos)
        {
            //Debug.Log(
                //$"[DynamiteTile] HandleRockDug: received pos={pos}, my gridPosition={gridPosition}");
            if(_hasExploded) return;
            if(pos != gridPosition) return;

            _hasExploded = true;
            _cell.MarkCellRevealed();

            StartCoroutine(ExplodeDelay());
        }

        private void Explode()
        {
            Vector3 center = _gridManager.GetCellAt(gridPosition).transform.position;
            float cellSize = _gridManager.cellSize;
            float radius = cellSize * ExplosionRadius + 0.01f;

            Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius, whatCanBeDestroy);
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<RockTile>(out var rock))
                {
                    Vector2Int p = rock.gridPosition;
                    Destroy(hit.gameObject);
                    RockTile.RaiseOnRockDug(p);
                    Destroy(gameObject);
                }
            }
            
            //TODO: Add Sound Effect
        }

        private IEnumerator ExplodeDelay()
        {
            yield return new WaitForSeconds(1.5f);
            Explode();
        }
    }
}
