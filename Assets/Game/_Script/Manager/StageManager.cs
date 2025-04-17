using System.Collections.Generic;
using Game._Script.Stages;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game._Script.Manager
{
    public class StageManager : MonoBehaviour
    {
        [SerializeField]
        private List<StageConfig> configs;
        private GridManager _gridManager;
        private RockLayerManager _rockLayerManager;
        private GemManager _gemManager;

        public StageConfig CurrentStageConfig => configs[_currentStageIndex];

        private int _currentStageIndex = 0;
        
        public void Initialize()
        {
            _gridManager = GameManager.Instance.gridManager;
            _rockLayerManager = GameManager.Instance.rockLayerManager;
            LoadStage(_currentStageIndex);
        }

        private void LoadStage(int index)
        {
            if (index < 0 || index >= configs.Count)
            {
                Debug.Log($"Stage index {index} is not valid!");
                return;
            }

            _currentStageIndex = index;
            var config = CurrentStageConfig;
            
            _gridManager.stageConfig = config;
            _gridManager.Initialize();
            
            _rockLayerManager.BuildRockLayer();
        }

        private void NextStage()
        {
            if (_currentStageIndex + 1 < configs.Count)
            {
                LoadStage(_currentStageIndex + 1);
            }
            else
            {
                Debug.Log("All stages complete");
            }
        }
    }
}
