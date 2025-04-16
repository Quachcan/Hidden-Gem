using System;
using System.Security.Cryptography;
using UnityEngine;

namespace Game._Script.Manager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public GridManager gridManager;
        public GemManager gemManager;


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
            gridManager = GetComponentInChildren<GridManager>();
            gemManager = GetComponentInChildren<GemManager>();
            
            InitializeManagers();
        }

        private void InitializeManagers()
        {
            gridManager.Initialize();
            gemManager.Initialize();
        }
    }
}
