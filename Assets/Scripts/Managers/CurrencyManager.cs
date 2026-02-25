using DinoCore.Core;
using DinoCore.Data;
using UnityEngine;

namespace DinoCore.Managers
{
    /// <summary>
    /// Single source of truth for all currencies.
    /// Other systems request Add / Spend; this validates and fires events.
    /// </summary>
    public class CurrencyManager : MonoBehaviour
    {
        public static CurrencyManager Instance { get; private set; }

        [Header("Read-Only Runtime")]
        [SerializeField] private double money;
        [SerializeField] private double totalMoneyEarned;
        [SerializeField] private double techCurrency;

        public double Money => money;
        public double TotalMoneyEarned => totalMoneyEarned;
        public double TechCurrency => techCurrency;

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        // ── Money ────────────────────────────────────────────────
        public void AddMoney(double amount)
        {
            if (amount <= 0) return;
            money += amount;
            totalMoneyEarned += amount;
            GameEvents.FireMoneyChanged(money);
        }

        public bool SpendMoney(double amount)
        {
            if (amount <= 0 || money < amount) return false;
            money -= amount;
            GameEvents.FireMoneyChanged(money);
            return true;
        }

        public bool CanAfford(double amount) => money >= amount;

        // ── Tech Currency ────────────────────────────────────────
        public void AddTechCurrency(double amount)
        {
            if (amount <= 0) return;
            techCurrency += amount;
            GameEvents.FireTechCurrencyChanged(techCurrency);
        }

        public bool SpendTechCurrency(double amount)
        {
            if (amount <= 0 || techCurrency < amount) return false;
            techCurrency -= amount;
            GameEvents.FireTechCurrencyChanged(techCurrency);
            return true;
        }

        public bool CanAffordTech(double amount) => techCurrency >= amount;

        // ── Reset (called on Rebirth) ────────────────────────────
        public void ResetForRebirth()
        {
            money = 0;
            totalMoneyEarned = 0;
            // techCurrency korunur — rebirth sonrası tech upgrade alabilsin
            GameEvents.FireMoneyChanged(money);
        }

        // ── Reset everything (called on Planet prestige) ─────────
        public void ResetForPlanet()
        {
            ResetForRebirth();
        }

        // ── Save / Load ──────────────────────────────────────────
        public void LoadFromSave(SaveData data)
        {
            money = data.money;
            totalMoneyEarned = data.totalMoneyEarned;
            techCurrency = data.techCurrency;
            GameEvents.FireMoneyChanged(money);
            GameEvents.FireTechCurrencyChanged(techCurrency);
        }

        public void WriteToSave(SaveData data)
        {
            data.money = money;
            data.totalMoneyEarned = totalMoneyEarned;
            data.techCurrency = techCurrency;
        }
    }
}
