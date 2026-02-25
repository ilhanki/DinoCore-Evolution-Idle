using UnityEngine;

namespace DinoCore.Data
{
    /// <summary>
    /// Central game balance configuration.
    /// Create one via Assets → Create → DinoCore → Game Config and assign it to GameManager.
    /// </summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "DinoCore/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Click")]
        public double baseClickIncome = 2;
        public float clickIncomeGrowthPerLevel = 0.10f;

        [Header("Rebirth")]
        public double rebirthMinTotalIncome = 1e6;
        public double rebirthCurrencyFormula_Divisor = 1e6;

        [Header("Planet")]
        public int planetMinRebirthCount = 5;
        public float planetMultiplierPerPlanet = 0.25f;

        [Header("Auto Collector")]
        public double autoCollectorBaseCost = 10000;
        public float autoCollectorCostExponent = 2.0f;

        [Header("Offline")]
        public float maxOfflineHours = 1f;
        public float offlineEfficiency = 0.50f;
        public float offlineIncomeMultiplier = 0.10f;

        [Header("Save")]
        public float autoSaveIntervalSeconds = 30f;

        [Header("Production")]
        public float minProductionTime = 0.5f;
    }
}
