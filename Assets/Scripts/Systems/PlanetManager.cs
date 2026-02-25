using System;
using System.Collections.Generic;
using DinoCore.Core;
using DinoCore.Data;
using DinoCore.Managers;
using UnityEngine;

namespace DinoCore.Systems
{
    /// <summary>
    /// Upper prestige layer. Planets give permanent multipliers but require
    /// sacrificing dinos and resetting rebirth progress.
    /// </summary>
    public class PlanetManager : MonoBehaviour
    {
        public static PlanetManager Instance { get; private set; }

        [Header("Planet Templates (assign all PlanetData SOs)")]
        [SerializeField] private List<PlanetData> planetTemplates = new();

        [SerializeField] private GameConfig config;

        private int planetCount;
        private readonly List<string> unlockedPlanetIds = new();

        private static readonly int[] PlanetRebirthRequirements = { 10, 50, 200, 500, 1000 };

        public int PlanetCount => planetCount;
        public double PlanetMultiplier => Math.Pow(5, planetCount);
        public IReadOnlyList<string> UnlockedPlanetIds => unlockedPlanetIds;

        public int RequiredRebirthCountForNextPlanet
        {
            get
            {
                if (planetCount < PlanetRebirthRequirements.Length)
                    return PlanetRebirthRequirements[planetCount];
                return PlanetRebirthRequirements[PlanetRebirthRequirements.Length - 1];
            }
        }

        public bool CanUnlockNextPlanet
        {
            get
            {
                if (planetCount >= planetTemplates.Count) return false;
                return RebirthManager.Instance.RebirthCount >= RequiredRebirthCountForNextPlanet;
            }
        }

        public PlanetData NextPlanet => planetCount < planetTemplates.Count ? planetTemplates[planetCount] : null;

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public bool TryUnlockPlanet()
        {
            if (!CanUnlockNextPlanet) return false;

            PlanetData planet = planetTemplates[planetCount];

            // Dinos reset fully with ResetAllForRebirth below

            // Full reset (rooms, dinos, money, tech)
            RoomManager.Instance.ResetAllForRebirth();
            DinoManager.Instance.ResetAllForRebirth();
            TechManager.Instance.ResetAllForPlanet();
            CurrencyManager.Instance.ResetForPlanet();
            RebirthManager.Instance.ResetForPlanet();

            // Record planet
            unlockedPlanetIds.Add(planet.planetId);
            planetCount++;

            // Force all room UIs to refresh immediately
            RoomManager.Instance.RefreshAllPanels();

            GameEvents.FirePlanetUnlocked(planetCount);
            return true;
        }

        // ── Save / Load ──────────────────────────────────────
        public void WriteToSave(SaveData data)
        {
            data.planetCount = planetCount;
            data.unlockedPlanets = new List<string>(unlockedPlanetIds);
        }

        public void LoadFromSave(SaveData data)
        {
            planetCount = data.planetCount;
            unlockedPlanetIds.Clear();
            if (data.unlockedPlanets != null)
                unlockedPlanetIds.AddRange(data.unlockedPlanets);
        }
    }
}
