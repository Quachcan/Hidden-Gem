using System.Collections;
using Game._Script.Manager;
using UnityEngine;

namespace Game._Script.Gems
{
    public class Gem : MonoBehaviour
    {
        public GemConfig config;
        private SpriteRenderer _sr;

        public bool isPlaced;
        public bool isRevealed;
        public bool isCollected;

        public void InitializeGem()
        {
            _sr = GetComponent<SpriteRenderer>();

            if (config == null)
            {
                Debug.Log("GemConfig is not available");
            }

            float cellSize = GameManager.instance.gridManager.cellSize;

            float desireWidth = config.width * cellSize;
            float desireHeight = config.height * cellSize;

            float nativeWidth = _sr.sprite.bounds.size.x;
            float nativeHeight = _sr.sprite.bounds.size.y;

            float scaleX = desireWidth / nativeWidth;
            float scaleY = desireHeight / nativeHeight;
            
            transform.localScale = new Vector3(scaleX, scaleY, 1f);
        }

        public void GemPlaced()
        {
            isPlaced = true;
        }

        public void GemReveal()
        {
            if (isRevealed) return;
            isRevealed = true;
            Debug.Log("Revealed!");
        }

        public void GemCollected()
        {
            if(isCollected) return;
            isCollected = true;
            Debug.Log("Collected");
            StartCoroutine(DestroyAfterDelay());
        }
        
        private IEnumerator DestroyAfterDelay()
        {
            yield return new WaitForSeconds(1f);
            Destroy(gameObject);
        }
    }
}
