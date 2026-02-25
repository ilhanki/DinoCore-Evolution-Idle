using DinoCore.Core;
using DinoCore.Managers;
using DinoCore.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DinoCore.UI
{
    /// <summary>
    /// Displays the player's current money and income rate in the HUD.
    /// Updates only when money changes (event-driven).
    /// </summary>
    public class CurrencyHUD : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text moneyText;
        [SerializeField] private TMP_Text incomePerSecondText;
        [SerializeField] private TMP_Text techCurrencyText;
        [SerializeField] private TMP_Text rebirthCurrencyText;
        [SerializeField] private Button moneyIconButton;

        private const double CheatMoneyAmount = 1_000_000;

        private void OnEnable()
        {
            GameEvents.OnMoneyChanged += UpdateMoney;
            GameEvents.OnTechCurrencyChanged += UpdateTech;
            GameEvents.OnRebirth += OnRebirth;
            GameEvents.OnPlanetUnlocked += OnPlanetUnlocked;
            GameEvents.OnRoomUpgraded += OnRoomChanged;


            if (moneyIconButton != null)
                moneyIconButton.onClick.AddListener(AddCheatMoney);
        }

        private void OnDisable()
        {
            GameEvents.OnMoneyChanged -= UpdateMoney;
            GameEvents.OnTechCurrencyChanged -= UpdateTech;
            GameEvents.OnRebirth -= OnRebirth;
            GameEvents.OnPlanetUnlocked -= OnPlanetUnlocked;
            GameEvents.OnRoomUpgraded -= OnRoomChanged;


            if (moneyIconButton != null)
                moneyIconButton.onClick.RemoveListener(AddCheatMoney);
        }

        private void Start()
        {
            // Initial display
            UpdateMoney(CurrencyManager.Instance.Money);
            UpdateTech(CurrencyManager.Instance.TechCurrency);
            UpdateRebirth();
            UpdateIncomeRate();
        }

        private void UpdateMoney(double amount)
        {
            if (moneyText != null)
                moneyText.text = BigNumberFormatter.Format(amount);
            UpdateIncomeRate();
        }

        private void UpdateTech(double amount)
        {
            if (techCurrencyText != null)
                techCurrencyText.text = BigNumberFormatter.Format(amount);
        }

        private void UpdateRebirth()
        {
            if (rebirthCurrencyText != null && Systems.RebirthManager.Instance != null)
                rebirthCurrencyText.text = BigNumberFormatter.Format(Systems.RebirthManager.Instance.RebirthCount);
        }

        private void OnRebirth(double gained) => UpdateRebirth();
        private void OnPlanetUnlocked(int count) => UpdateRebirth();

        private void OnRoomChanged(string roomId, int level) => UpdateIncomeRate();


        private void UpdateIncomeRate()
        {
            if (incomePerSecondText != null && Systems.RoomManager.Instance != null)
                incomePerSecondText.text = BigNumberFormatter.Format(Systems.RoomManager.Instance.TotalIncomePerSecond) + "/s";
        }

        private void AddCheatMoney()
        {
            if (CurrencyManager.Instance != null)
                CurrencyManager.Instance.AddMoney(CheatMoneyAmount);
        }
    }
}
