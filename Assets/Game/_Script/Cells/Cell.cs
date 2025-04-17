using UnityEngine;

namespace Game._Script.Cells
{
    public class Cell : MonoBehaviour
    {
        public Vector2Int position;
        public bool isOccupied;
        public bool isDig;
        

        public void MarkOccupied()
        {
            isOccupied = true;
        }
    }
}
