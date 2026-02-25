using DinoCore.Core;
using DinoCore.Data;
using DinoCore.Systems;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DinoCore.Managers
{
    /// <summary>
    /// Top-level orchestrator. Initializes all subsystems in the correct order.
    /// Place on a root GameObject that persists across scenes.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Configuration")]
        [SerializeField] private GameConfig config;
        public GameConfig Config => config;

        [Header("Systems (auto-found if null)")]
        [SerializeField] private CurrencyManager currencyManager;
        [SerializeField] private RoomManager roomManager;
        [SerializeField] private DinoManager dinoManager;
        [SerializeField] private TechManager techManager;
        [SerializeField] private RebirthManager rebirthManager;
        [SerializeField] private PlanetManager planetManager;
        [SerializeField] private SaveManager saveManager;
        [SerializeField] private ClickManager clickManager;
        [SerializeField] private OfflineIncomeSystem offlineSystem;

        private bool isInitialized;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Re-find all systems from the NEW scene and re-initialize
            isInitialized = false;
            AutoFindSystems();
            InitializeGame();
        }

        private void AutoFindSystems()
        {
            // Always re-find: after scene reload, old refs point to destroyed objects
            currencyManager = FindAnyObjectByType<CurrencyManager>();
            roomManager = FindAnyObjectByType<RoomManager>();
            dinoManager = FindAnyObjectByType<DinoManager>();
            techManager = FindAnyObjectByType<TechManager>();
            rebirthManager = FindAnyObjectByType<RebirthManager>();
            planetManager = FindAnyObjectByType<PlanetManager>();
            saveManager = FindAnyObjectByType<SaveManager>();
            clickManager = FindAnyObjectByType<ClickManager>();
            offlineSystem = FindAnyObjectByType<OfflineIncomeSystem>();
        }

        private void InitializeGame()
        {
            if (isInitialized) return;

            // 1. Initialize data containers (creates runtime objects from ScriptableObjects)
            roomManager.InitializeRooms();
            dinoManager.InitializeDinos();
            techManager.InitializeTechs();

            // 2. Load save if exists
            SaveData saveData = saveManager.Load();
            if (saveData != null)
            {
                saveManager.ApplySaveData(saveData);

                // 3. Calculate offline income
                offlineSystem.CalculateOfflineIncome(saveData.lastSaveTimestamp);
            }
            else
            {
                // First launch – give starting money and unlock first room
                currencyManager.AddMoney(100);
                var firstRoom = roomManager.GetRoom("room_01");
                firstRoom?.ForceUnlock();
            }

            // Refresh all room UIs
            roomManager.RefreshAllPanels();

            isInitialized = true;
            Debug.Log("[GameManager] Game initialized successfully.");
        }

        private void OnApplicationQuit()
        {
            GameEvents.ClearAll();
        }

#if UNITY_EDITOR
        [ContextMenu("Delete Save Data")]
        private void DebugDeleteSave()
        {
            if (saveManager != null)
                saveManager.DeleteSave();
            else
                PlayerPrefs.DeleteAll();

            PlayerPrefs.Save();
            Debug.Log("[GameManager] Save data deleted.");

            if (Application.isPlaying)
            {
                isInitialized = false;
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
            }
            else
            {
                Debug.Log("[GameManager] Not in Play mode — restart Play to see fresh state.");
            }
        }

        [ContextMenu("Add 1M Money")]
        private void DebugAddMoney()
        {
            currencyManager.AddMoney(1_000_000);
            Debug.Log("[GameManager] Added 1M money.");
        }

        [ContextMenu("Add 100 Tech Currency")]
        private void DebugAddTech()
        {
            currencyManager.AddTechCurrency(100);
            Debug.Log("[GameManager] Added 100 tech currency.");
        }

        [ContextMenu("Force Rebirth")]
        private void DebugRebirth()
        {
            rebirthManager.ForceRebirth();
            Debug.Log($"[GameManager] Force rebirth! Count: {rebirthManager.RebirthCount}, Multiplier: x{rebirthManager.RebirthMultiplier}");
        }
#endif
    }
}
