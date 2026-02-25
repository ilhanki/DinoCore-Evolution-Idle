using System;
using DinoCore.Core;
using DinoCore.Data;
using DinoCore.Managers;
using DinoCore.Utils;
using UnityEngine;

namespace DinoCore.Systems
{
    /// <summary>
    /// Calculates and awards offline income when the player returns.
    /// Called once during game initialization after save is loaded.
    /// </summary>
    public class OfflineIncomeSystem : MonoBehaviour
    {
        public static OfflineIncomeSystem Instance { get; private set; }

        [SerializeField] private GameConfig config;

        /// <summary>
        /// Seconds the player was offline. Set after calculation.
        /// </summary>
        public double LastOfflineSeconds { get; private set; }
        public double LastOfflineIncome { get; private set; }

        /// <summary>
        /// Event fired when offline income is ready to be shown in UI.
        /// Parameters: offlineSeconds, offlineIncome
        /// </summary>
        public event Action<double, double> OnOfflineIncomeReady;

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        /// <summary>
        /// Call after save data is applied to all managers.
        /// </summary>
        public void CalculateOfflineIncome(long lastSaveTimestamp)
        {
            if (lastSaveTimestamp <= 0)
            {
                LastOfflineSeconds = 0;
                LastOfflineIncome = 0;
                return;
            }

            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            double offlineSeconds = now - lastSaveTimestamp;

            // Clamp to max offline hours
            double maxSeconds = config.maxOfflineHours * 3600;
            offlineSeconds = Math.Min(offlineSeconds, maxSeconds);

            // Must be at least 60 seconds to count
            if (offlineSeconds < 60)
            {
                LastOfflineSeconds = 0;
                LastOfflineIncome = 0;
                return;
            }

            // Apply efficiency penalty
            double effectiveSeconds = offlineSeconds * config.offlineEfficiency;

            // Let each room simulate
            double totalIncome = RoomManager.Instance.SimulateAllOffline(effectiveSeconds);

            // Apply global offline multiplier
            totalIncome *= config.offlineIncomeMultiplier;

            // For rooms with auto-collector, money was already added by SimulateAllOffline
            // For rooms without auto-collector, stored money increased

            LastOfflineSeconds = offlineSeconds;
            LastOfflineIncome = totalIncome;

            if (totalIncome > 0)
            {
                // Auto-collector income is returned by SimulateAllOffline but NOT added to wallet.
                // We must add it here.
                CurrencyManager.Instance.AddMoney(totalIncome);
                GameEvents.FireOfflineIncomeCollected(totalIncome);
            }

            OnOfflineIncomeReady?.Invoke(offlineSeconds, totalIncome);

            Debug.Log($"[Offline] {BigNumberFormatter.FormatTime(offlineSeconds)} offline → " +
                      $"{BigNumberFormatter.Format(totalIncome)} income (efficiency: {config.offlineEfficiency:P0})");
        }
    }
}
