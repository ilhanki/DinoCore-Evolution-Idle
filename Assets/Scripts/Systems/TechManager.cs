using System;
using System.Collections.Generic;
using DinoCore.Core;
using DinoCore.Data;
using DinoCore.Managers;
using UnityEngine;

namespace DinoCore.Systems
{
    /// <summary>
    /// Manages technology upgrades purchased with TechCurrency.
    /// </summary>
    public class TechManager : MonoBehaviour
    {
        public static TechManager Instance { get; private set; }

        [Header("Tech Templates (assign all TechData SOs)")]
        [SerializeField] private List<TechData> techTemplates = new();

        private readonly Dictionary<string, TechRuntime> techs = new();

        // ── Cached global multipliers (recalculated on upgrade) ──
        private double globalIncomeMultiplier = 1.0;
        private float globalSpeedMultiplier = 1.0f;
        private double globalStorageMultiplier = 1.0;
        private float globalRebirthBoostMultiplier = 1.0f;
        private double globalClickMultiplier = 1.0;
        private double globalDinoEfficiency = 1.0;

        public double GlobalIncomeMultiplier => globalIncomeMultiplier;
        public float GlobalSpeedMultiplier => globalSpeedMultiplier;
        public double GlobalStorageMultiplier => globalStorageMultiplier;
        public float GlobalRebirthBoostMultiplier => globalRebirthBoostMultiplier;
        public double GlobalClickMultiplier => globalClickMultiplier;
        public double GlobalDinoEfficiency => globalDinoEfficiency;

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public void InitializeTechs()
        {
            foreach (var template in techTemplates)
            {
                if (techs.ContainsKey(template.techId)) continue;
                techs[template.techId] = new TechRuntime { data = template, level = 0 };
            }
            RecalculateMultipliers();
        }

        public bool TryUpgrade(string techId)
        {
            if (!techs.TryGetValue(techId, out TechRuntime tech)) return false;
            if (tech.level >= tech.data.maxLevel) return false;

            double cost = tech.UpgradeCost;
            if (!CurrencyManager.Instance.SpendTechCurrency(cost)) return false;

            tech.level++;
            RecalculateMultipliers();
            GameEvents.FireTechUpgraded(techId, tech.level);
            return true;
        }

        public TechRuntime GetTech(string techId)
        {
            techs.TryGetValue(techId, out TechRuntime tech);
            return tech;
        }

        public List<TechRuntime> GetAllTechs()
        {
            var list = new List<TechRuntime>(techs.Values);
            return list;
        }

        private void RecalculateMultipliers()
        {
            globalIncomeMultiplier = 1.0;
            globalSpeedMultiplier = 1.0f;
            globalStorageMultiplier = 1.0;
            globalRebirthBoostMultiplier = 1.0f;
            globalClickMultiplier = 1.0;
            globalDinoEfficiency = 1.0;

            foreach (var kvp in techs)
            {
                var t = kvp.Value;
                if (t.level <= 0) continue;

                double bonus = t.data.bonusPerLevel * t.level;

                switch (t.data.category)
                {
                    case TechCategory.Income:
                        globalIncomeMultiplier += bonus;
                        break;
                    case TechCategory.Speed:
                        globalSpeedMultiplier += (float)bonus;
                        break;
                    case TechCategory.Storage:
                        globalStorageMultiplier += bonus;
                        break;
                    case TechCategory.RebirthBoost:
                        globalRebirthBoostMultiplier += (float)bonus;
                        break;
                    case TechCategory.ClickPower:
                        globalClickMultiplier += bonus;
                        break;
                    case TechCategory.DinoEfficiency:
                        globalDinoEfficiency += bonus;
                        break;
                }
            }
        }

        // ── Reset (tech persists through rebirth by default) ──
        public void ResetAllForRebirth()
        {
            // Tech upgrades are kept across rebirths.
            // Only reset if a specific design change is needed.
        }

        public void ResetAllForPlanet()
        {
            foreach (var kvp in techs)
                kvp.Value.level = 0;
            RecalculateMultipliers();
        }

        // ── Save / Load ───────────────────────────────────────
        public void WriteToSave(SaveData data)
        {
            data.techs.Clear();
            foreach (var kvp in techs)
            {
                data.techs.Add(new TechSaveData
                {
                    techId = kvp.Key,
                    level = kvp.Value.level
                });
            }
        }

        public void LoadFromSave(SaveData data)
        {
            foreach (var techSave in data.techs)
            {
                if (techs.TryGetValue(techSave.techId, out TechRuntime tech))
                    tech.level = techSave.level;
            }
            RecalculateMultipliers();
        }
    }

    [Serializable]
    public class TechRuntime
    {
        public TechData data;
        public int level;

        public double UpgradeCost => data.baseCost * Math.Pow(data.costExponent, level);
        public float CurrentBonus => data.bonusPerLevel * level;
        public bool IsMaxed => level >= data.maxLevel;
    }
}
