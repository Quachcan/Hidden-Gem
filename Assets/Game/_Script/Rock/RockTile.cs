using System;
using Game._Script.Cells;
using Game._Script.Manager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Game._Script.Rock
{
    public class RockTile : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private AudioClip audioClip;
        [SerializeField] private AudioSource _audioSource;
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
            _audioSource = GetComponent<AudioSource>();
        }

        public void MarkOccupied()
        {
            _isOccupied = true;
        }

        public static void RaiseOnRockDug(Vector2Int pos)
        {
            OnRockDug?.Invoke(pos);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log($"[RockTile] OnPointerDown at {gridPosition}, digClip={(audioClip!=null)}, audioSource={( _audioSource!=null )}");
            if(GameManager.instance.IsPaused || GameManager.instance.IsEnd) return;
            if (!GameManager.instance.pickaxeManager.TryUsePickaxe())
            {
                GameManager.instance.uiManager.HandleOutOfPickaxe();
                return;
            }

            if (Camera.main != null) AudioSource.PlayClipAtPoint(audioClip, Camera.main.transform.position);

            _cell.MarkCellRevealed();
            _isDug = true;
            OnRockDug?.Invoke(gridPosition);
            Destroy(gameObject);
            
            //TODO: Add dig sound effect
        }
    }
}
