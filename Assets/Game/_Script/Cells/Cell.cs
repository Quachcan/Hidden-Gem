using UnityEngine;

namespace Game._Script.Cells
{
    public class Cell : MonoBehaviour
    {
        public Vector2Int position;
        public bool isRevealed;
        public bool isOccupied;
        

        public void MarkOccupied()
        {
            isOccupied = true;
        }

        public void MarkCellRevealed()
        {
            isRevealed = true;
        }
    }
}
