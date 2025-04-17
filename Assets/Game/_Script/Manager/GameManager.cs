using System;
using System.Security.Cryptography;
using Game._Script.Stages;
using UnityEngine;

namespace Game._Script.Manager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public GridManager gridManager;
        public GemManager gemManager;
        public RockLayerManager rockLayerManager;
        public StageManager stageManager;


        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            stageManager = GetComponentInChildren<StageManager>();
            gridManager = GetComponentInChildren<GridManager>();
            rockLayerManager = GetComponentInChildren<RockLayerManager>();
            gemManager = GetComponentInChildren<GemManager>();
            
            InitializeManagers();
        }

        private void InitializeManagers()
        {
            stageManager.Initialize();
            //gridManager.Initialize();
            //rockLayerManager.BuildRockLayer();
            //gemManager.Initialize();
        }
    }
}
