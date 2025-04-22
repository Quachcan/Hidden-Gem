using System;
using System.Security.Cryptography;
using Game._Script.Stages;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game._Script.Manager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        public GridManager gridManager;
        public GemManager gemManager;
        public RockLayerManager rockLayerManager;
        public StageManager stageManager;
        public UIManager uiManager;
        public PickaxeManager pickaxeManager;
        public DynamiteManager dynamiteManager;

        public enum GameState{ Menu, Playing, Paused, Ended}

        private GameState _currGameState = GameState.Menu;
        public GameState CurrentGameState => _currGameState;
        public bool IsPlaying => _currGameState == GameState.Playing;
        public bool IsPaused => _currGameState == GameState.Paused;
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            pickaxeManager   = GetComponentInChildren<PickaxeManager>();
            uiManager        = GetComponentInChildren<UIManager>();
            stageManager     = GetComponentInChildren<StageManager>();
            gridManager      = GetComponentInChildren<GridManager>();
            rockLayerManager = GetComponentInChildren<RockLayerManager>();
            gemManager       = GetComponentInChildren<GemManager>();
            dynamiteManager  = GetComponentInChildren<DynamiteManager>();
            
            
            gemManager.AllGemsCollected += OnAllStageGemsCollected;
            
            InitializeManagers();
            SetState(GameState.Menu);
        }

        private void InitializeManagers()
        {
            uiManager.Initialize();
        }

        private void SetState(GameState newState)
        {
            _currGameState = newState;
            switch (_currGameState)
            {
                case GameState.Menu:
                    break;
                case GameState.Playing:
                    InitializeStages();
                    break;
                case GameState.Paused:
                    break;
                case GameState.Ended:
                    break;
            }
        }

        public void StartGame()
        {
            SetState(GameState.Playing);
            Debug.Log("Playing");
        }

        private void InitializeStages()
        {
            stageManager.Initialize();
            pickaxeManager.Initialize();
            
        }

        public void PauseGame()
        {
            if(_currGameState != GameState.Playing) return;
            SetState(GameState.Paused);
            Debug.Log("Pause");
        }

        public void ResumeGame()
        {
            if(_currGameState != GameState.Paused) return;
            SetState(GameState.Playing);
            Debug.Log("Resume game");
        }
        
        private void OnAllStageGemsCollected()
        {
            stageManager.NextStage();
        }

        public void EndGame()
        {
            uiManager.HandleEndPanel();
            SetState(GameState.Ended);
            Debug.Log("Game ended");
        }
    }
}
