using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game._Script.Manager
{
    public class UIManager : MonoBehaviour
    {
        [Header("Pickaxe UI")]
        [SerializeField] private PickaxeManager  pickaxeManager;
        [SerializeField] private TextMeshProUGUI pickaxeCountText;
        [SerializeField] private GameObject      outOfPickaxePopup;
        
        [Header("Panels")]
        [SerializeField] private GameObject      pausePanel;
        [SerializeField] private GameObject      menuPanel;
        [SerializeField] private GameObject      endPanel;
        [SerializeField] private GameObject      inGamePanel;
        
        
        

        private void OnDisable()
        {
            if (pickaxeManager != null)
                GameManager.instance.pickaxeManager.OnPickaxeCountChanged -= UpdatePickaxeUI;
        }

        public void Initialize()
        {
            if(pickaxeManager != null)
                GameManager.instance.pickaxeManager.OnPickaxeCountChanged += UpdatePickaxeUI;
            
            inGamePanel.SetActive(false);
            menuPanel.SetActive(true);
        }

        private void UpdatePickaxeUI(int count)
        {
            if (pickaxeCountText != null)
                pickaxeCountText.text = count.ToString();
        }

        public void HandleOutOfPickaxe()
        {
            if (outOfPickaxePopup != null) 
                outOfPickaxePopup.SetActive(true);
        }

        public void HandleMenuPanel()
        {
            menuPanel.SetActive(true);
            inGamePanel.SetActive(false);
            GameManager.instance.GameMenu();
        }

        public void HandlePausePanel()
        {
            pausePanel.SetActive(true);
            GameManager.instance.GamePause();
        }

        public void HandleEndPanel()
        {
            endPanel.SetActive(true);
        }

        public void HandleResumeButton()
        {
            pausePanel.SetActive(false);
            GameManager.instance.GameResume();
        }
        
        public void HandleStartButton()
        {
            menuPanel.SetActive(false);
            inGamePanel.SetActive(true);
            GameManager.instance.StartGame();
        }
        
    }
        
}
