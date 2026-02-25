using System;
using DinoCore.Core;
using DinoCore.Data;
using DinoCore.Managers;
using UnityEngine;

namespace DinoCore.Systems
{
    /// <summary>
    /// JSON-based save system using PlayerPrefs as storage.
    /// Handles auto-save, manual save, and load.
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        private const string SaveKey = "DinoCore_SaveData";
        private const string BackupKey = "DinoCore_SaveData_Backup";

        [SerializeField] private GameConfig config;

        private float autoSaveTimer;

        public bool HasSave => PlayerPrefs.HasKey(SaveKey);

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void OnEnable()
        {
            // Save on app pause / quit
            Application.focusChanged += OnFocusChanged;
        }

        private void OnDisable()
        {
            Application.focusChanged -= OnFocusChanged;
        }

        private void OnFocusChanged(bool hasFocus)
        {
            if (!hasFocus) Save();
        }

        private void OnApplicationQuit()
        {
            Save();
        }

        // Using Update only for auto-save timer; lightweight check
        private void Update()
        {
            autoSaveTimer += Time.unscaledDeltaTime;
            if (autoSaveTimer >= config.autoSaveIntervalSeconds)
            {
                autoSaveTimer = 0f;
                Save();
            }
        }

        public void Save()
        {
            try
            {
                SaveData data = new SaveData();
                data.lastSaveTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                if (CurrencyManager.Instance != null) CurrencyManager.Instance.WriteToSave(data);
                if (RoomManager.Instance     != null) RoomManager.Instance.WriteToSave(data);
                if (DinoManager.Instance     != null) DinoManager.Instance.WriteToSave(data);
                if (TechManager.Instance     != null) TechManager.Instance.WriteToSave(data);
                if (RebirthManager.Instance  != null) RebirthManager.Instance.WriteToSave(data);
                if (PlanetManager.Instance   != null) PlanetManager.Instance.WriteToSave(data);
                if (ClickManager.Instance    != null) ClickManager.Instance.WriteToSave(data);

                string json = JsonUtility.ToJson(data, false);

                // Backup previous save before overwriting
                if (PlayerPrefs.HasKey(SaveKey))
                    PlayerPrefs.SetString(BackupKey, PlayerPrefs.GetString(SaveKey));

                PlayerPrefs.SetString(SaveKey, json);
                PlayerPrefs.Save();

                GameEvents.FireGameSaved();
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] Save failed: {e.Message}");
            }
        }

        public SaveData Load()
        {
            if (!HasSave) return null;

            try
            {
                string json = PlayerPrefs.GetString(SaveKey);
                SaveData data = JsonUtility.FromJson<SaveData>(json);
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] Load failed, trying backup: {e.Message}");
                return LoadBackup();
            }
        }

        private SaveData LoadBackup()
        {
            if (!PlayerPrefs.HasKey(BackupKey)) return null;
            try
            {
                string json = PlayerPrefs.GetString(BackupKey);
                return JsonUtility.FromJson<SaveData>(json);
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] Backup load failed: {e.Message}");
                return null;
            }
        }

        public void DeleteSave()
        {
            PlayerPrefs.DeleteKey(SaveKey);
            PlayerPrefs.DeleteKey(BackupKey);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Applies loaded data to all managers.
        /// </summary>
        public void ApplySaveData(SaveData data)
        {
            if (data == null) return;

            CurrencyManager.Instance.LoadFromSave(data);
            RoomManager.Instance.LoadFromSave(data);
            DinoManager.Instance.LoadFromSave(data);
            TechManager.Instance.LoadFromSave(data);
            RebirthManager.Instance.LoadFromSave(data);
            PlanetManager.Instance.LoadFromSave(data);
            ClickManager.Instance.LoadFromSave(data);
        }
    }
}
