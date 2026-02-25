using DinoCore.Core;
using DinoCore.Data;
using DinoCore.Managers;
using UnityEngine;

namespace DinoCore.Systems
{
    /// <summary>
    /// Handles tap/click income with all multipliers applied.
    /// </summary>
    public class ClickManager : MonoBehaviour
    {
        public static ClickManager Instance { get; private set; }

        [SerializeField] private GameConfig config;

        private int totalClicks;
        public int TotalClicks => totalClicks;

        public double ClickIncome
        {
            get
            {
                double baseClick = config.baseClickIncome;
                double techMult = TechManager.Instance != null ? TechManager.Instance.GlobalClickMultiplier : 1.0;
                double rebirthMult = RebirthManager.Instance != null ? RebirthManager.Instance.RebirthMultiplier : 1.0;
                double planetMult = PlanetManager.Instance != null ? PlanetManager.Instance.PlanetMultiplier : 1.0;
                return baseClick * techMult * rebirthMult * planetMult;
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

        /// <summary>
        /// Call this from UI button tap.
        /// </summary>
        public void DoClick()
        {
            double income = ClickIncome;
            CurrencyManager.Instance.AddMoney(income);
            totalClicks++;
            GameEvents.FireClick(income);
        }

        public void WriteToSave(SaveData data)
        {
            data.clickIncome = config.baseClickIncome;
            data.totalClicks = totalClicks;
        }

        public void LoadFromSave(SaveData data)
        {
            totalClicks = data.totalClicks;
        }
    }
}
