using System;

namespace DinoCore.Core
{
    /// <summary>
    /// Central event hub. All inter-system communication flows through here.
    /// No direct references between managers – everything is decoupled via events.
    /// </summary>
    public static class GameEvents
    {
        // ── Currency ──────────────────────────────────────────────
        public static event Action<double> OnMoneyChanged;
        public static event Action<double> OnTechCurrencyChanged;

        // ── Room ──────────────────────────────────────────────────
        public static event Action<string> OnRoomUnlocked;        // roomId
        public static event Action<string, int> OnRoomUpgraded;   // roomId, newLevel
        public static event Action<string, double> OnRoomCollected; // roomId, amount
        public static event Action<string> OnRoomProductionReady; // roomId
        public static event Action<string, bool> OnAutoCollectorChanged; // roomId, hasAuto

        // ── Dino ──────────────────────────────────────────────────
        public static event Action<string> OnDinoPurchased;          // dinoId

        // ── Tech ──────────────────────────────────────────────────
        public static event Action<string, int> OnTechUpgraded; // techId, newLevel

        // ── Prestige ──────────────────────────────────────────────
        public static event Action<double> OnRebirth;          // rebirthCurrencyGained
        public static event Action<int> OnPlanetUnlocked;      // planetCount

        // ── Save / Offline ────────────────────────────────────────
        public static event Action OnGameSaved;
        public static event Action<double> OnOfflineIncomeCollected; // amount

        // ── Click ─────────────────────────────────────────────────
        public static event Action<double> OnClick; // clickIncome

        // ── Fire helpers ──────────────────────────────────────────
        public static void FireMoneyChanged(double amount) => OnMoneyChanged?.Invoke(amount);
        public static void FireTechCurrencyChanged(double amount) => OnTechCurrencyChanged?.Invoke(amount);

        public static void FireRoomUnlocked(string roomId) => OnRoomUnlocked?.Invoke(roomId);
        public static void FireRoomUpgraded(string roomId, int level) => OnRoomUpgraded?.Invoke(roomId, level);
        public static void FireRoomCollected(string roomId, double amount) => OnRoomCollected?.Invoke(roomId, amount);
        public static void FireRoomProductionReady(string roomId) => OnRoomProductionReady?.Invoke(roomId);
        public static void FireAutoCollectorChanged(string roomId, bool has) => OnAutoCollectorChanged?.Invoke(roomId, has);


        public static void FireDinoPurchased(string dinoId) => OnDinoPurchased?.Invoke(dinoId);

        public static void FireTechUpgraded(string techId, int level) => OnTechUpgraded?.Invoke(techId, level);

        public static void FireRebirth(double gained) => OnRebirth?.Invoke(gained);
        public static void FirePlanetUnlocked(int count) => OnPlanetUnlocked?.Invoke(count);

        public static void FireGameSaved() => OnGameSaved?.Invoke();
        public static void FireOfflineIncomeCollected(double amount) => OnOfflineIncomeCollected?.Invoke(amount);
        public static void FireClick(double income) => OnClick?.Invoke(income);

        /// <summary>
        /// Call on scene unload / quit to prevent memory leaks.
        /// </summary>
        public static void ClearAll()
        {
            OnMoneyChanged = null;
            OnTechCurrencyChanged = null;
            OnRoomUnlocked = null;
            OnRoomUpgraded = null;
            OnRoomCollected = null;
            OnRoomProductionReady = null;
            OnAutoCollectorChanged = null;

            OnDinoPurchased = null;
            OnTechUpgraded = null;
            OnRebirth = null;
            OnPlanetUnlocked = null;
            OnGameSaved = null;
            OnOfflineIncomeCollected = null;
            OnClick = null;
        }
    }
}
