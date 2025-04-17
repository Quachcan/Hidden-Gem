using System.Collections.Generic;
using Game._Script.Cells;
using Game._Script.Rock;
using UnityEngine;

namespace Game._Script.Manager
{
    public class RockLayerManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject rockPrefab;

        public void BuildRockLayer()
        {
            List<Cell> cells = GameManager.Instance.gridManager.GetAllCells();
            foreach (var cell in cells)
            {
                var rockGo = Instantiate(rockPrefab, cell.transform.position, Quaternion.identity, transform);

                var tile = rockGo.GetComponent<RockTile>();
                if (tile != null)
                {
                    tile.gridPosition = cell.position;
                }
                else
                {
                    Debug.LogWarning("Rock Prefab is missing");
                }
            }
            
        }
    }
}
