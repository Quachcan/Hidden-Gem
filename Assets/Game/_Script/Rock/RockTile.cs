using System;
using UnityEngine;

namespace Game._Script.Rock
{
    public class RockTile : MonoBehaviour
    {
        public static event Action<Vector2Int> OnRockDug; 
        
        public Vector2Int gridPosition;
        private void OnMouseDown()
        {
            OnRockDug?.Invoke(gridPosition);
            Destroy(gameObject);
        }
    }
}
