using UnityEngine;

namespace Game._Script.Cells
{
    public class Cell : MonoBehaviour
    {
        public Vector2Int position;
        public bool isOccupied;
        public bool isRevealed;

        public void Reveal()
        {
            if(isRevealed) return;
            isRevealed = true;
        }

        public void MarkOccupied()
        {
            isOccupied = true;
        }
    }
}
