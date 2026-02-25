using System;
using System.Collections;
using System.Collections.Generic;
using DinoCore.Core;
using DinoCore.Data;
using DinoCore.Managers;
using DinoCore.Utils;
using UnityEngine;

namespace DinoCore.Systems
{
    /// <summary>
    /// Individual room component. Handles its own production loop via coroutine.
    /// Attached to each room GameObject in the scroll view.
    /// </summary>
    public class Room : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private RoomData data;

        // ── Runtime state ─────────────────────────────────────
        private int level;
        private bool isUnlocked;
        private bool hasAutoCollector;
        private double currentStored;
        private float productionTimer;
        private bool productionReady; // true = cycle complete, waiting for collection

        private Coroutine productionCoroutine;
        private bool pendingProduction;

        // ── Public accessors ──────────────────────────────────
        public RoomData Data => data;
        public string RoomId => data != null ? data.roomId : string.Empty;
        public int Level => level;
        public bool IsUnlocked => isUnlocked;
        public bool HasAutoCollector => hasAutoCollector;
        public double CurrentStored => currentStored;
        public bool IsProductionReady => productionReady;

        /// <summary>
        /// Production cycle progress (0 = just started, 1 = complete and waiting).
        /// </summary>
        public float ProductionProgress => EffectiveProductionTime > 0 ? Mathf.Clamp01(productionTimer / EffectiveProductionTime) : 0f;

        // ── Milestones ────────────────────────────────────────
        private static readonly int[] Milestones = { 10, 25, 50, 75, 100, 150, 200, 300, 400, 500 };

        private int GetMilestoneCount()
        {
            int count = 0;
            for (int i = 0; i < Milestones.Length; i++)
            {
                if (level >= Milestones[i]) count++;
                else break;
            }
            return count;
        }

        // ── Computed economy values ───────────────────────────
        public double UpgradeCost => data != null ? data.unlockCost * 0.15 * Math.Pow(data.costExponent, level) : 0;

        public double AutoCollectorCost
        {
            get
            {
                if (data == null) return double.MaxValue;
                return data.unlockCost * 10;
            }
        }

        public float EffectiveProductionTime
        {
            get
            {
                if (data == null || GameManager.Instance == null) return 1f;
                float time = data.baseProductionTime;

                // 1. Apply Dino Speed Reduction (from owned RoomSpeed dinos for this room)
                float speedReduction = DinoManager.Instance != null ? DinoManager.Instance.GetSpeedReductionForRoom(RoomId) : 0f;
                time *= (1f - speedReduction);

                // 2. Apply Tech Speed Multiplier (divisive)
                float techSpeed = TechManager.Instance != null ? TechManager.Instance.GlobalSpeedMultiplier : 1f;
                if (techSpeed > 0f) time /= techSpeed;

                // 3. Apply Milestone Speed Multiplier (divisive, 2x per milestone)
                int milestones = GetMilestoneCount();
                if (milestones > 0)
                    time /= Mathf.Pow(2f, milestones);

                float minTime = GameManager.Instance.Config.minProductionTime;
                return Mathf.Max(time, minTime);
            }
        }

        public double IncomePerCycle
        {
            get
            {
                if (data == null) return 0;
                double income = data.baseIncome;
                
                // Linear level bonus (1 + level * step)
                double levelMult = 1 + (level * data.levelMultiplierStep);

                // Milestone exponential bonus (2^milestones)
                int milestones = GetMilestoneCount();
                double milestoneMult = Math.Pow(2, milestones);

                // External multipliers
                double dinoMult   = DinoManager.Instance    != null ? DinoManager.Instance.GetIncomeMultiplierForRoom(RoomId) : 1.0;
                double globalMult = TechManager.Instance    != null ? TechManager.Instance.GlobalIncomeMultiplier             : 1.0;
                double rebirthMult= RebirthManager.Instance != null ? RebirthManager.Instance.RebirthMultiplier               : 1.0;
                double planetMult = PlanetManager.Instance  != null ? PlanetManager.Instance.PlanetMultiplier                 : 1.0;

                return income * levelMult * milestoneMult * dinoMult * globalMult * rebirthMult * planetMult;
            }
        }

        public double IncomePerSecond => EffectiveProductionTime > 0 ? IncomePerCycle / EffectiveProductionTime : 0;

        // ── Init ──────────────────────────────────────────────
        public void Initialize(RoomData roomData)
        {
            data = roomData;
        }

        // ── Unlock ────────────────────────────────────────────
        public bool TryUnlock()
        {
            if (isUnlocked) return false;
            if (Managers.CurrencyManager.Instance == null) return false;
            if (!Managers.CurrencyManager.Instance.SpendMoney(data.unlockCost)) return false;

            isUnlocked = true;
            level = 1;
            productionTimer = 0f;
            StartProduction();
            GameEvents.FireRoomUnlocked(RoomId);
            return true;
        }

        public void ForceUnlock()
        {
            isUnlocked = true;
            if (level < 1) level = 1;
            productionTimer = 0f;
            StartProduction();
        }

        private void OnEnable()
        {
            if (pendingProduction)
            {
                pendingProduction = false;
                StartProduction();
            }
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
            productionCoroutine = null;
        }

        // ── Upgrade ───────────────────────────────────────────
        public bool TryUpgrade()
        {
            if (!isUnlocked) return false;
            if (Managers.CurrencyManager.Instance == null) return false;

            double cost = UpgradeCost;
            if (!Managers.CurrencyManager.Instance.SpendMoney(cost)) return false;

            level++;
            GameEvents.FireRoomUpgraded(RoomId, level);

            if (!productionReady)
            {
                productionTimer = Mathf.Min(productionTimer, EffectiveProductionTime);
                StartProduction();
            }
            return true;
        }

        // ── Buy Auto Collector ────────────────────────────────
        public bool TryBuyAutoCollector()
        {
            if (!isUnlocked || hasAutoCollector) return false;
            if (Managers.CurrencyManager.Instance == null) return false;

            if (!Managers.CurrencyManager.Instance.SpendMoney(AutoCollectorCost)) return false;

            hasAutoCollector = true;
            GameEvents.FireAutoCollectorChanged(RoomId, true);

            if (productionReady)
                Collect();

            return true;
        }

        // ── Collection ────────────────────────────────────────
        public double Collect()
        {
            double amount = 0;

            if (currentStored > 0)
            {
                if (Managers.CurrencyManager.Instance != null)
                {
                    amount = currentStored;
                    currentStored = 0;
                    productionReady = false;
                    Managers.CurrencyManager.Instance.AddMoney(amount);
                    GameEvents.FireRoomCollected(RoomId, amount);
                }
                else
                {
                    Debug.LogWarning($"[Room {RoomId}] CurrencyManager missing during Collect. Amount {currentStored} lost.");
                    currentStored = 0;
                    productionReady = false;
                }
            }

            if (isUnlocked)
            {
                productionTimer = 0f;
                StartProduction();
            }
            
            return amount;
        }

        // ── Production coroutine ──────────────────────────────
        private void StartProduction()
        {
            if (productionCoroutine != null) StopCoroutine(productionCoroutine);
            pendingProduction = false;
            productionCoroutine = StartCoroutine(ProductionLoop());
        }

        private IEnumerator ProductionLoop()
        {
            while (productionTimer < EffectiveProductionTime)
            {
                yield return null;
                productionTimer += Time.deltaTime;
            }

            productionTimer = EffectiveProductionTime;
            double income = IncomePerCycle;

            // Tech dinos apply rebirth tech multiplier (handled by RebirthManager)
            // No per-cycle tech production from dinos

            currentStored = income;
            productionReady = true;

            GameEvents.FireRoomProductionReady(RoomId);

            if (hasAutoCollector)
                Collect();
        }

        // ── Offline simulation ────────────────────────────────
        public double SimulateOffline(double seconds)
        {
            if (!isUnlocked) return 0;
            float prodTime = EffectiveProductionTime;
            if (prodTime <= 0) return 0;

            double totalElapsed = productionTimer + seconds;
            double cycles = Math.Floor(totalElapsed / prodTime);
            productionTimer = (float)(totalElapsed - cycles * prodTime);

            if (cycles <= 0)
                return 0;

            double totalIncome = cycles * IncomePerCycle;

            if (currentStored > 0)
            {
                totalIncome += currentStored;
                currentStored = 0;
            }

            productionReady = false;
            return totalIncome;
        }

        // ── Reset ─────────────────────────────────────────────
        public void ResetForRebirth()
        {
            if (productionCoroutine != null) StopCoroutine(productionCoroutine);
            productionCoroutine = null;
            level = 0;
            isUnlocked = false;
            hasAutoCollector = false;
            currentStored = 0;
            productionTimer = 0;
            productionReady = false;
            pendingProduction = false;
        }

        // ── Save / Load ───────────────────────────────────────
        public RoomSaveData ToSaveData()
        {
            return new RoomSaveData
            {
                roomId = RoomId,
                level = level,
                isUnlocked = isUnlocked,
                hasAutoCollector = hasAutoCollector,
                currentStored = currentStored,
                productionTimer = productionTimer
            };
        }

        public void LoadFromSave(RoomSaveData save)
        {
            level = save.level;
            isUnlocked = save.isUnlocked;
            hasAutoCollector = save.hasAutoCollector;
            currentStored = save.currentStored;
            productionTimer = save.productionTimer;

            productionReady = currentStored > 0;

            if (isUnlocked && !productionReady)
                StartProduction();
        }
    }
}
