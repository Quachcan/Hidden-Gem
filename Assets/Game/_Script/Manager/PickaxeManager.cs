using System;
using UnityEngine;

namespace Game._Script.Manager
{
    public class PickaxeManager : MonoBehaviour
    {
        [Header("Pickaxe Setting")] 
        public int startingPickaxe = 100;

        private int _currentPickaxe;

        public event Action<int> OnPickaxeCountChanged;


        public void Initialize()
        {
            _currentPickaxe = startingPickaxe;
            Debug.Log($"{_currentPickaxe}");
            OnPickaxeCountChanged?.Invoke(_currentPickaxe);
        }

        public bool TryUsePickaxe()
        {
            if (_currentPickaxe > 0)
            {
                _currentPickaxe--;
                OnPickaxeCountChanged?.Invoke(_currentPickaxe);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddPickaxe(int amount)
        {
            _currentPickaxe += amount;
            OnPickaxeCountChanged?.Invoke(_currentPickaxe);
        }
    }
}
