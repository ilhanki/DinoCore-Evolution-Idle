using System;
using System.Collections.Generic;
using DinoCore.Core;
using DinoCore.Data;
using DinoCore.Managers;
using UnityEngine;

namespace DinoCore.Systems
{
    /// <summary>
    /// Manages sequential dinosaur purchases. Auto-generates dinos from 4 templates.
    /// Each dino is a one-time purchase that gives a permanent multiplier.
    /// No leveling — buy it and move on.
    /// </summary>
    public class DinoManager : MonoBehaviour
    {
        public static DinoManager Instance { get; private set; }

        [Header("Dino Templates (one per type)")]
        [Tooltip("Template for room income dinos")]
        [SerializeField] private DinoData roomIncomeTemplate;
        [Tooltip("Template for room speed dinos")]
        [SerializeField] private DinoData roomSpeedTemplate;
        [Tooltip("Template for tech multiplier dinos")]
        [SerializeField] private DinoData techMultiplierTemplate;
        [Tooltip("Template for global income multiplier dinos")]
        [SerializeField] private DinoData globalIncomeTemplate;

        [Header("Generation Settings")]
        [Tooltip("Cost of the very first dino")]
        [SerializeField] private double startingCost = 1000;
        [Tooltip("Cost multiplier for each subsequent tier (per room)")]
        [SerializeField] private float costMultiplierPerTier = 3.5f;
        [Tooltip("Bonus added per tier (e.g. 0.05 means +5% more per tier)")]
        [SerializeField] private float bonusGrowthPerTier = 0.03f;

        // Runtime dino list, sorted by progression order
        private readonly List<Dino> sortedDinos = new();
        private readonly Dictionary<string, Dino> dinoLookup = new();
        private readonly List<DinoData> generatedData = new();

        /// <summary>All dinos sorted by progression order.</summary>
        public IReadOnlyList<Dino> AllDinos => sortedDinos;

        /// <summary>Lookup dino by ID.</summary>
        public IReadOnlyDictionary<string, Dino> Dinos => dinoLookup;

        /// <summary>The next dino available for purchase. Null if all bought.</summary>
        public Dino CurrentDino
        {
            get
            {
                foreach (var d in sortedDinos)
                    if (!d.isOwned) return d;
                return null;
            }
        }

        /// <summary>Index of the next unpurchased dino (0-based).</summary>
        public int CurrentDinoIndex
        {
            get
            {
                for (int i = 0; i < sortedDinos.Count; i++)
                    if (!sortedDinos[i].isOwned) return i;
                return sortedDinos.Count;
            }
        }

        /// <summary>Total number of owned dinos.</summary>
        public int OwnedCount
        {
            get
            {
                int c = 0;
                foreach (var d in sortedDinos) if (d.isOwned) c++;
                return c;
            }
        }

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;

            foreach (var d in generatedData)
                if (d != null) Destroy(d);
        }

        // ── Initialization ───────────────────────────────────────
        public void InitializeDinos()
        {
            int roomCount = 10;
            if (RoomManager.Instance != null)
                roomCount = Mathf.Max(RoomManager.Instance.AllRooms.Count, 1);

            DinoData[] templates = { roomIncomeTemplate, roomSpeedTemplate, techMultiplierTemplate, globalIncomeTemplate };

            // Base costs per template type (proportional to starting cost)
            // RoomIncome = 1x, RoomSpeed = 1.2x, TechMultiplier = 1.5x, GlobalIncome = 1.8x
            double[] costRatios = { 1.0, 1.2, 1.5, 1.8 };

            int sortOrder = 0;
            for (int tier = 0; tier < roomCount; tier++)
            {
                DinoRarity rarity = GetRarityForTier(tier, roomCount);

                for (int t = 0; t < templates.Length; t++)
                {
                    DinoData template = templates[t];
                    if (template == null) continue;

                    int targetRoom;
                    if (template.dinoType == DinoType.TechIncome || template.dinoType == DinoType.GlobalIncome)
                        targetRoom = -1;
                    else
                        targetRoom = tier;

                    DinoData generated = ScriptableObject.CreateInstance<DinoData>();
                    generated.name = $"{template.displayName}_tier{tier}";
                    generated.dinoId = $"{template.dinoId}_t{tier}";
                    generated.displayName = GetTierDisplayName(template.displayName, tier);
                    generated.description = template.description;
                    generated.dinoType = template.dinoType;
                    generated.icon = template.icon;
                    generated.idleSprite = template.idleSprite;
                    generated.targetRoomIndex = targetRoom;
                    generated.sortOrder = sortOrder;

                    // Cost: starting cost × cost ratio × tier multiplier
                    generated.baseCost = startingCost * costRatios[t] * Math.Pow(costMultiplierPerTier, tier);
                    generated.costExponent = template.costExponent;

                    // Bonus: template base + growth per tier
                    generated.baseBonus = template.baseBonus + (bonusGrowthPerTier * tier);

                    generated.rarity = rarity;

                    generatedData.Add(generated);

                    var dino = new Dino { data = generated };
                    sortedDinos.Add(dino);
                    dinoLookup[generated.dinoId] = dino;
                    sortOrder++;
                }
            }
        }

        private DinoRarity GetRarityForTier(int tier, int totalTiers)
        {
            float ratio = (float)tier / Mathf.Max(totalTiers - 1, 1);
            if (ratio < 0.2f) return DinoRarity.Common;
            if (ratio < 0.4f) return DinoRarity.Uncommon;
            if (ratio < 0.6f) return DinoRarity.Rare;
            if (ratio < 0.8f) return DinoRarity.Epic;
            return DinoRarity.Legendary;
        }

        private string GetTierDisplayName(string baseName, int tier)
        {
            if (tier == 0) return baseName;
            string[] roman = { "", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X" };
            string suffix = tier < roman.Length ? roman[tier] : $"Mk.{tier + 1}";
            return $"{baseName} {suffix}";
        }

        // ── Purchase ──────────────────────────────────────────
        public bool TryPurchaseDino(string dinoId)
        {
            if (!dinoLookup.TryGetValue(dinoId, out Dino dino)) return false;
            if (dino.isOwned) return false;

            var current = CurrentDino;
            if (current == null || current.DinoId != dinoId) return false;

            double cost = dino.PurchaseCost;
            if (!CurrencyManager.Instance.SpendMoney(cost)) return false;

            dino.isOwned = true;

            GameEvents.FireDinoPurchased(dinoId);
            return true;
        }

        // ── Bonus Calculations ────────────────────────────────

        /// <summary>
        /// Income multiplier for a specific room from RoomIncome dinos
        /// targeting that room + all GlobalIncome dinos.
        /// </summary>
        public double GetIncomeMultiplierForRoom(string roomId)
        {
            int roomIndex = GetRoomIndex(roomId);
            double mult = 1.0;

            foreach (var d in sortedDinos)
            {
                if (!d.isOwned) continue;

                if (d.Type == DinoType.RoomIncome && d.data.targetRoomIndex == roomIndex)
                    mult *= (1.0 + d.EffectiveBonus);
                else if (d.Type == DinoType.GlobalIncome)
                    mult *= (1.0 + d.EffectiveBonus);
            }
            return mult;
        }

        /// <summary>
        /// Speed reduction for a specific room from RoomSpeed dinos.
        /// Returns 0..0.9 (max 90% reduction).
        /// </summary>
        public float GetSpeedReductionForRoom(string roomId)
        {
            int roomIndex = GetRoomIndex(roomId);
            float reduction = 0f;

            foreach (var d in sortedDinos)
            {
                if (!d.isOwned) continue;
                if (d.Type == DinoType.RoomSpeed && d.data.targetRoomIndex == roomIndex)
                    reduction += d.EffectiveBonus;
            }
            return Mathf.Min(reduction, 0.9f);
        }

        /// <summary>
        /// Tech currency multiplier from all owned TechIncome dinos.
        /// Applied when tech currency is earned (e.g. on rebirth).
        /// A return of 1.0 means no bonus, 1.15 means +15% etc.
        /// </summary>
        public double GetTechMultiplier()
        {
            double mult = 1.0;
            foreach (var d in sortedDinos)
            {
                if (!d.isOwned) continue;
                if (d.Type == DinoType.TechIncome)
                    mult *= (1.0 + d.EffectiveBonus);
            }
            return mult;
        }

        private int GetRoomIndex(string roomId)
        {
            if (RoomManager.Instance == null) return -1;

            var rooms = RoomManager.Instance.AllRooms;
            for (int i = 0; i < rooms.Count; i++)
            {
                if (rooms[i].RoomId == roomId) return i;
            }
            return -1;
        }

        // ── Reset ─────────────────────────────────────────────
        public void ResetAllForRebirth()
        {
            foreach (var d in sortedDinos)
                d.isOwned = false;
        }

        // ── Save / Load ───────────────────────────────────────
        public void WriteToSave(SaveData data)
        {
            data.dinos.Clear();
            foreach (var d in sortedDinos)
                data.dinos.Add(d.ToSaveData());
        }

        public void LoadFromSave(SaveData data)
        {
            foreach (var dinoSave in data.dinos)
            {
                if (dinoLookup.TryGetValue(dinoSave.dinoId, out Dino dino))
                    dino.LoadFromSave(dinoSave);
            }
        }
    }
}
