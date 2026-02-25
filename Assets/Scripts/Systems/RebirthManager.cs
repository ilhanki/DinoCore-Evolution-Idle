using System;
using DinoCore.Core;
using DinoCore.Data;
using DinoCore.Managers;
using UnityEngine;

namespace DinoCore.Systems
{
    /// <summary>
    /// Handles rebirth (soft prestige) logic.
    /// </summary>
    public class RebirthManager : MonoBehaviour
    {
        public static RebirthManager Instance { get; private set; }

        [SerializeField] private GameConfig config;

        private int rebirthCount;

        public int RebirthCount => rebirthCount;

        /// <summary>
        /// Rebirth multiplier: increases by +1 per rebirth (x1 → x2 → x3 …)
        /// </summary>
        public double RebirthMultiplier => 1 + rebirthCount;

        public int RebirthCountIncrement
        {
            get
            {
                int planetCount = PlanetManager.Instance != null ? PlanetManager.Instance.PlanetCount : 0;
                return (int)Math.Pow(2, planetCount);
            }
        }

        public double RequiredTotalIncome => config.rebirthMinTotalIncome * Math.Pow(1.5, rebirthCount);

        public bool CanRebirth => CurrencyManager.Instance.TotalMoneyEarned >= RequiredTotalIncome;

        /// <summary>
        /// How much rebirth currency the player would gain if they rebirth now.
        /// </summary>
        public double PotentialRebirthCurrency
        {
            get
            {
                double total = CurrencyManager.Instance.TotalMoneyEarned;
                if (total < RequiredTotalIncome) return 0;
                double raw = Math.Floor(total / config.rebirthCurrencyFormula_Divisor);
                double techMult = DinoManager.Instance != null ? DinoManager.Instance.GetTechMultiplier() : 1.0;
                return raw * TechManager.Instance.GlobalRebirthBoostMultiplier * techMult;
            }
        }

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public bool TryRebirth()
        {
            if (!CanRebirth) return false;
            ExecuteRebirth();
            return true;
        }

        public void ResetForPlanet()
        {
            rebirthCount = 0;
        }

        /// <summary>
        /// Force rebirth without checking requirements (for debug).
        /// </summary>
        public void ForceRebirth()
        {
            ExecuteRebirth();
        }

        private void ExecuteRebirth()
        {
            double gained = PotentialRebirthCurrency;

            // Reset everything
            RoomManager.Instance.ResetAllForRebirth();
            DinoManager.Instance.ResetAllForRebirth();
            CurrencyManager.Instance.ResetForRebirth();

            // Award rebirth currency
            CurrencyManager.Instance.AddTechCurrency(gained);
            rebirthCount += RebirthCountIncrement;

            // Force all room UIs to refresh immediately
            RoomManager.Instance.RefreshAllPanels();

            GameEvents.FireRebirth(gained);
        }

        // ── Save / Load ──────────────────────────────────────
        public void WriteToSave(SaveData data)
        {
            data.rebirthCount = rebirthCount;
        }

        public void LoadFromSave(SaveData data)
        {
            rebirthCount = data.rebirthCount;
        }
    }
}
